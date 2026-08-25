'use strict';

const { chromium } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const { queryFingerprint: queryOdbcFingerprint } = require('./support/doc32-e2e-odbc.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const allowedLoserCodes = new Set([
  'WORKFLOW_RETURN_IN_PROGRESS',
  'WORKFLOW_RETURN_VERSION_CONFLICT',
  'WORKFLOW_RETURN_TASK_UNAVAILABLE'
]);

function required(name) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function normalizeActivityName(value) {
  return typeof value === 'string' ? value.normalize('NFKC').trim().toLocaleLowerCase() : '';
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') ||
      /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

function baseUrl() {
  const value = required('DOC32_E2E_BASE_URL');
  if (!value) throw new Error('DOC32_E2E_BASE_URL es obligatoria.');
  return new URL(value).toString();
}

function endpoint(name) {
  return new URL(`webservice/WebServiceWorkflowModern.asmx/${name}`, baseUrl()).toString();
}

async function queryFingerprint(sql, idTarea) {
  try {
    return await queryOdbcFingerprint(sql, idTarea);
  } catch {
    throw new Error('No fue posible ejecutar el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.');
  }
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC32_E2E_MODULE',
    userEnvironmentVariable: 'DOC32_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC32_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC32_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

function launchOptions() {
  const options = { headless: true };
  if (required('DOC32_E2E_BROWSER_PATH')) options.executablePath = required('DOC32_E2E_BROWSER_PATH');
  else if (required('DOC32_E2E_BROWSER_CHANNEL')) options.channel = required('DOC32_E2E_BROWSER_CHANNEL');
  return options;
}

async function invoke(context, name, payload) {
  const started = performance.now();
  try {
    const response = await context.request.post(endpoint(name), {
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
      data: payload,
      timeout: 60000
    });
    const elapsedMs = Math.round(performance.now() - started);
    if (!response.ok()) return { exito: false, codigo: `HTTP_${response.status()}`, estadoFinal: null, latenciaMs: elapsedMs };
    const envelope = await response.json();
    const dto = envelope?.d;
    if (!dto || typeof dto !== 'object' || /System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i.test(JSON.stringify(dto))) {
      return { exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null, latenciaMs: elapsedMs };
    }
    return { exito: Boolean(dto.Exito), codigo: dto.CodigoBloqueo || dto.Error?.Codigo || null, estadoFinal: dto.EstadoFinal || null, latenciaMs: elapsedMs, dto };
  } catch {
    return { exito: false, codigo: 'REQUEST_FAILED', estadoFinal: null, latenciaMs: Math.round(performance.now() - started) };
  }
}

async function invokePreview(context, idTarea) {
  const result = await invoke(context, 'PreviewDevolverActividad', { idTarea, termino: '', cursor: '', tamanoPagina: 50 });
  const preview = result.dto;
  if (!preview || preview.Error || !preview.TokenVersion || preview.HayMas || !Array.isArray(preview.Destinos) || preview.Destinos.length === 0) {
    throw new Error('El preview vigente no entregó una arista entrante utilizable para la carrera.');
  }
  return preview;
}

function selectedDestination(preview, activityName) {
  const expected = normalizeActivityName(activityName);
  if (!expected) throw new Error('La actividad de concurrencia configurada no es válida.');
  const matches = preview.Destinos.filter((candidate) => normalizeActivityName(candidate?.NombreActividad) === expected);
  if (matches.length !== 1 || !Number.isSafeInteger(matches[0]?.IdConector) || matches[0].IdConector <= 0) {
    throw new Error('El preview vigente no entregó una actividad de concurrencia utilizable.');
  }
  return matches[0];
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration)) {
    throw new Error('El gate local debe permanecer apagado y sin alcance para DOC-32.');
  }
}

function evidencePath() {
  const configured = required('DOC32_E2E_CONCURRENCY_EVIDENCE_PATH');
  const fallback = path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', 'doc32-return-activity-concurrency.json');
  const destination = configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
  if (path.relative(repositoryRoot, destination).startsWith('..')) throw new Error('La evidencia DOC-32 debe permanecer dentro del repositorio.');
  return destination;
}

async function writeEvidence(evidence) {
  const serialized = JSON.stringify(evidence);
  if (/password|cookie|token|destino|usuario|mysql|connection/i.test(serialized)) {
    throw new Error('La evidencia DOC-32 contiene un campo sensible no permitido.');
  }
  const destination = evidencePath();
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

function assertConfiguration() {
  const names = [
    'DOC32_E2E_BASE_URL', 'DOC32_E2E_MODULE', 'DOC32_E2E_AUTHORIZED_USER', 'DOC32_E2E_AUTHORIZED_PASSWORD',
    'DOC32_E2E_ENVIRONMENT', 'DOC32_E2E_ENVIRONMENT_AUTHORIZED', 'DOC32_E2E_EXECUTION_AUTHORIZED',
    'DOC32_E2E_CONCURRENCY_AUTHORIZED', 'DOC32_E2E_CONCURRENCY_TASK_ID', 'DOC32_E2E_ODBC_DSN', 'DOC32_E2E_MYSQL_USER', 'DOC32_E2E_MYSQL_PASSWORD',
    'DOC32_E2E_CONCURRENCY_ACTIVITY_NAME', 'DOC32_E2E_TASK_STATE_SQL', 'DOC32_E2E_AUDIT_SQL', 'DOC32_E2E_CONCURRENCY_MAX_MS'
  ];
  const missing = names.filter((name) => !required(name));
  if (missing.length > 0) throw new Error(`Faltan variables DOC-32: ${missing.join(', ')}.`);
  if (required('DOC32_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true' ||
      required('DOC32_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true' ||
      required('DOC32_E2E_CONCURRENCY_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La carrera DOC-32 requiere autorizaciones explícitas.');
  }
  positiveInteger('DOC32_E2E_CONCURRENCY_TASK_ID');
  positiveInteger('DOC32_E2E_CONCURRENCY_MAX_MS');
  assertReadOnlySql(required('DOC32_E2E_TASK_STATE_SQL'), 'DOC32_E2E_TASK_STATE_SQL');
  assertReadOnlySql(required('DOC32_E2E_AUDIT_SQL'), 'DOC32_E2E_AUDIT_SQL');
}

async function main() {
  assertConfiguration();
  await assertLocalGateOff();
  const idTarea = positiveInteger('DOC32_E2E_CONCURRENCY_TASK_ID');
  const budgetMs = positiveInteger('DOC32_E2E_CONCURRENCY_MAX_MS');
  const stateSql = required('DOC32_E2E_TASK_STATE_SQL');
  const auditSql = required('DOC32_E2E_AUDIT_SQL');
  const browser = await chromium.launch(launchOptions());
  let contexts = [];
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let results = [];
  try {
    beforeState = await queryFingerprint(stateSql, idTarea);
    beforeAudit = await queryFingerprint(auditSql, idTarea);
    contexts = [await login(browser), await login(browser)];
    const preview = await invokePreview(contexts[0], idTarea);
    const destination = selectedDestination(preview, required('DOC32_E2E_CONCURRENCY_ACTIVITY_NAME'));
    const payload = { idTarea, idConector: destination.IdConector, tokenVersion: preview.TokenVersion };
    results = await Promise.all(contexts.map((context) => invoke(context, 'EjecutarDevolverActividad', payload)));
  } finally {
    try {
      afterState = await queryFingerprint(stateSql, idTarea);
      afterAudit = await queryFingerprint(auditSql, idTarea);
    } finally {
      await Promise.all(contexts.map((context) => context.close()));
      await browser.close();
      await assertLocalGateOff();
    }
  }

  const exitos = results.filter((result) => result.exito);
  const bloqueados = results.filter((result) => !result.exito);
  const latenciasCumplen = results.length === 2 && results.every((result) => result.latenciaMs <= budgetMs);
  const approved = exitos.length === 1 && exitos[0].estadoFinal === 'completada' && bloqueados.length === 1 &&
    allowedLoserCodes.has(bloqueados[0].codigo) && beforeState !== afterState && beforeAudit !== afterAudit && latenciasCumplen;
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverActividad',
    solicitudes: 2,
    exitos: exitos.length,
    codigosBloqueo: bloqueados.map((result) => result.codigo),
    estadoFinalGanador: exitos[0]?.estadoFinal || null,
    latenciasMs: results.map((result) => result.latenciaMs),
    presupuestoMs: budgetMs,
    estadoCambio: beforeState !== afterState,
    auditoriaCambio: beforeAudit !== afterAudit,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit },
    aprobada: approved
  });
  console.log(`DOC-32 concurrencia: ${exitos.length}/2 transiciones efectivas; evidencia saneada generada.`);
  if (!approved) process.exitCode = 1;
}

main().catch(() => {
  console.error('La carrera DOC-32 no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});

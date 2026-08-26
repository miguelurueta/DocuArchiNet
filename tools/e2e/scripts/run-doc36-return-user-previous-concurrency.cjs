'use strict';

const { chromium } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const { queryFingerprint: queryOdbcFingerprint } = require('./support/doc32-e2e-odbc.cjs');

const prefix = 'DOC36_E2E';
const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const allowedLoserCodes = new Set([
  'WORKFLOW_RETURN_USER_IN_PROGRESS',
  'WORKFLOW_RETURN_USER_VERSION_CONFLICT',
  'WORKFLOW_RETURN_USER_TASK_UNAVAILABLE',
  'WORKFLOW_RETURN_USER_HISTORY_UNAVAILABLE'
]);

function variable(suffix) {
  return `${prefix}_${suffix}`;
}

function required(suffix) {
  const value = process.env[variable(suffix)];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function positiveInteger(suffix) {
  const value = Number(required(suffix));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${variable(suffix)} debe ser un entero positivo.`);
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
  const value = required('BASE_URL');
  if (!value) throw new Error(`${variable('BASE_URL')} es obligatoria.`);
  return new URL(value).toString();
}

function endpoint(name) {
  return new URL(`webservice/WebServiceWorkflowModern.asmx/${name}`, baseUrl()).toString();
}

function launchOptions() {
  const options = { headless: true };
  if (required('BROWSER_PATH')) options.executablePath = required('BROWSER_PATH');
  else if (required('BROWSER_CHANNEL')) options.channel = required('BROWSER_CHANNEL');
  return options;
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: variable('MODULE'),
    userEnvironmentVariable: variable('AUTHORIZED_USER'),
    passwordEnvironmentVariable: variable('AUTHORIZED_PASSWORD'),
    ignoreHTTPSErrors: required('IGNORE_HTTPS_ERRORS') === 'true'
  });
}

async function queryFingerprint(sql, taskId) {
  return queryOdbcFingerprint(sql, taskId, process.env, prefix);
}

function assertPublicResponse(dto) {
  if (!dto || typeof dto !== 'object' || /System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i.test(JSON.stringify(dto))) {
    throw new Error('El endpoint devolvió una respuesta no pública.');
  }
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
    assertPublicResponse(dto);
    return { exito: Boolean(dto.Exito), codigo: dto.CodigoBloqueo || dto.Error?.Codigo || null, estadoFinal: dto.EstadoFinal || null, latenciaMs: elapsedMs, dto };
  } catch {
    return { exito: false, codigo: 'REQUEST_FAILED', estadoFinal: null, latenciaMs: Math.round(performance.now() - started) };
  }
}

async function invokePreview(context, taskId) {
  const result = await invoke(context, 'PreviewDevolverUsuarioAnterior', { idTarea: taskId });
  const preview = result.dto;
  if (!preview || preview.Error || !preview.TokenVersion || !preview.Contexto || !preview.Contexto.ActividadAnterior) {
    throw new Error('El preview vigente no entregó el usuario histórico preparado para la carrera.');
  }
  return preview;
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration)) {
    throw new Error('El gate local debe permanecer apagado y sin alcance para DOC-36.');
  }
}

function evidencePath() {
  const configured = required('CONCURRENCY_EVIDENCE_PATH');
  const fallback = path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', 'doc36-return-user-previous-concurrency.json');
  const destination = configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
  if (path.relative(repositoryRoot, destination).startsWith('..')) throw new Error('La evidencia DOC-36 debe permanecer dentro del repositorio.');
  return destination;
}

async function writeEvidence(evidence) {
  const serialized = JSON.stringify(evidence);
  if (/password|cookie|token|destino|usuario|mysql|connection/i.test(serialized)) {
    throw new Error('La evidencia DOC-36 contiene un campo sensible no permitido.');
  }
  const destination = evidencePath();
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

function assertConfiguration() {
  const names = [
    'BASE_URL', 'MODULE', 'AUTHORIZED_USER', 'AUTHORIZED_PASSWORD', 'ENVIRONMENT', 'ENVIRONMENT_AUTHORIZED',
    'EXECUTION_AUTHORIZED', 'CONCURRENCY_AUTHORIZED', 'CONCURRENCY_TASK_ID', 'ODBC_DSN', 'MYSQL_USER', 'MYSQL_PASSWORD',
    'TASK_STATE_SQL', 'AUDIT_SQL', 'CONCURRENCY_MAX_MS'
  ];
  const missing = names.filter((suffix) => !required(suffix));
  if (missing.length > 0) throw new Error(`Faltan variables DOC-36: ${missing.map(variable).join(', ')}.`);
  if (required('ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true' ||
      required('EXECUTION_AUTHORIZED').toLowerCase() !== 'true' ||
      required('CONCURRENCY_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La carrera DOC-36 requiere autorizaciones explícitas.');
  }
  positiveInteger('CONCURRENCY_TASK_ID');
  positiveInteger('CONCURRENCY_MAX_MS');
  assertReadOnlySql(required('TASK_STATE_SQL'), variable('TASK_STATE_SQL'));
  assertReadOnlySql(required('AUDIT_SQL'), variable('AUDIT_SQL'));
}

async function main() {
  assertConfiguration();
  await assertLocalGateOff();
  const taskId = positiveInteger('CONCURRENCY_TASK_ID');
  const stateSql = required('TASK_STATE_SQL');
  const auditSql = required('AUDIT_SQL');
  const budgetMs = positiveInteger('CONCURRENCY_MAX_MS');
  const browser = await chromium.launch(launchOptions());
  let contexts = [];
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let results = [];
  try {
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    contexts = [await login(browser), await login(browser)];
    const preview = await invokePreview(contexts[0], taskId);
    results = await Promise.all(contexts.map((context) => invoke(context, 'EjecutarDevolverUsuarioAnterior', {
      idTarea: taskId,
      tokenVersion: preview.TokenVersion
    })));
  } finally {
    try {
      afterState = await queryFingerprint(stateSql, taskId);
      afterAudit = await queryFingerprint(auditSql, taskId);
    } finally {
      await Promise.all(contexts.map((context) => context.close()));
      await browser.close();
      await assertLocalGateOff();
    }
  }
  const successes = results.filter((result) => result.exito);
  const blocked = results.filter((result) => !result.exito);
  const withinBudget = results.length === 2 && results.every((result) => result.latenciaMs <= budgetMs);
  const approved = successes.length === 1 && successes[0].estadoFinal === 'completada' && blocked.length === 1 &&
    allowedLoserCodes.has(blocked[0].codigo) && beforeState !== afterState && beforeAudit !== afterAudit && withinBudget;
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverUsuarioAnterior',
    solicitudes: 2,
    exitos: successes.length,
    codigosBloqueo: blocked.map((result) => result.codigo),
    estadoFinalGanador: successes[0]?.estadoFinal || null,
    latenciasMs: results.map((result) => result.latenciaMs),
    presupuestoMs: budgetMs,
    estadoCambio: beforeState !== afterState,
    auditoriaCambio: beforeAudit !== afterAudit,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit },
    aprobada: approved
  });
  console.log(`DOC-36 concurrencia: ${successes.length}/2 transiciones efectivas; evidencia saneada generada.`);
  if (!approved) process.exitCode = 1;
}

main().catch(() => {
  console.error('La carrera DOC-36 no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});

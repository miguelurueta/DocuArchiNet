'use strict';

const { chromium } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const allowedLoserCodes = new Set([
  'WORKFLOW_TRANSITION_IN_PROGRESS',
  'WORKFLOW_VERSION_CONFLICT',
  'WORKFLOW_TASK_UNAVAILABLE'
]);

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function assertRequiredConfiguration() {
  const requiredNames = [
    'DOC28_E2E_BASE_URL',
    'DOC28_E2E_MODULE',
    'DOC28_E2E_AUTHORIZED_USER',
    'DOC28_E2E_AUTHORIZED_PASSWORD',
    'DOC28_E2E_EXECUTION_AUTHORIZED',
    'DOC28_E2E_CONCURRENCY_AUTHORIZED',
    'DOC28_E2E_TASK_ID',
    'DOC28_E2E_MYSQL_URL',
    'DOC28_E2E_TASK_STATE_SQL',
    'DOC28_E2E_AUDIT_SQL'
  ];
  const missing = requiredNames.filter((name) => !required(name));
  if (missing.length > 0) {
    throw new Error(`Faltan variables DOC-28 requeridas: ${missing.join(', ')}.`);
  }
  if (required('DOC28_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('DOC28_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para permitir una carrera mutante.');
  }
  if (required('DOC28_E2E_CONCURRENCY_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('DOC28_E2E_CONCURRENCY_AUTHORIZED debe ser exactamente true para permitir una carrera mutante.');
  }
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function baseUrl() {
  const value = required('DOC28_E2E_BASE_URL');
  if (!value) throw new Error('DOC28_E2E_BASE_URL es obligatoria.');
  try {
    return new URL(value.endsWith('/') ? value : `${value}/`).toString();
  } catch {
    throw new Error('DOC28_E2E_BASE_URL debe ser una URL absoluta válida.');
  }
}

function previewUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/PreviewEnviarUsuario', baseUrl()).toString();
}

function executionUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioUsuario', baseUrl()).toString();
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) || (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

function evidencePath() {
  const configured = required('DOC28_CONCURRENCY_EVIDENCE_PATH');
  const fallback = path.join(__dirname, '..', 'artifacts', 'doc28-user-send-concurrency.json');
  return configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
}

async function writeEvidence(evidence) {
  const destination = evidencePath();
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration)) {
    throw new Error('El gate local debe permanecer apagado y sin alcance para la carrera DOC-28.');
  }
}

function launchOptions() {
  const options = { headless: true };
  if (required('DOC28_E2E_BROWSER_PATH')) options.executablePath = required('DOC28_E2E_BROWSER_PATH');
  else if (required('DOC28_E2E_BROWSER_CHANNEL')) options.channel = required('DOC28_E2E_BROWSER_CHANNEL');
  return options;
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC28_E2E_MODULE',
    userEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC28_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

async function invokePreview(context, idTarea) {
  const response = await context.request.post(previewUrl(), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: { idTarea, consulta: '', cursor: '', tamanoPagina: 1 },
    timeout: 60000
  });
  if (!response.ok()) throw new Error('El preview de concurrencia no devolvió HTTP 200.');
  const envelope = await response.json();
  const preview = envelope && envelope.d;
  if (!preview || typeof preview !== 'object' || preview.Error || !preview.TokenVersion ||
      !Array.isArray(preview.Destinos) || preview.Destinos.length === 0) {
    throw new Error('El preview vigente no entregó un destino utilizable para la carrera.');
  }
  return preview;
}

async function invokeExecution(context, payload) {
  try {
    const response = await context.request.post(executionUrl(), {
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
      data: payload,
      timeout: 60000
    });
    if (!response.ok()) return { exito: false, codigo: `HTTP_${response.status()}`, estadoFinal: null };
    const envelope = await response.json();
    const dto = envelope && envelope.d;
    if (!dto || typeof dto !== 'object') return { exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
    const serialized = JSON.stringify(dto);
    if (/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE)\s/i.test(serialized)) {
      return { exito: false, codigo: 'UNSAFE_RESPONSE', estadoFinal: null };
    }
    return {
      exito: Boolean(dto.Exito),
      codigo: dto.CodigoBloqueo || dto.Error?.Codigo || null,
      estadoFinal: dto.EstadoFinal || null
    };
  } catch {
    return { exito: false, codigo: 'REQUEST_FAILED', estadoFinal: null };
  }
}

async function queryFingerprint(pool, sql, idTarea) {
  const [rows] = await pool.execute(sql, [idTarea]);
  return fingerprint(rows);
}

async function main() {
  assertRequiredConfiguration();
  const idTarea = positiveInteger('DOC28_E2E_TASK_ID');
  const stateSql = required('DOC28_E2E_TASK_STATE_SQL');
  const auditSql = required('DOC28_E2E_AUDIT_SQL');
  assertReadOnlySql(stateSql, 'DOC28_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC28_E2E_AUDIT_SQL');
  await assertLocalGateOff();

  const browser = await chromium.launch(launchOptions());
  const pool = mysql.createPool(required('DOC28_E2E_MYSQL_URL'));
  let contexts = [];
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let results = [];
  try {
    beforeState = await queryFingerprint(pool, stateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, auditSql, idTarea);
    contexts = [await login(browser), await login(browser)];
    const preview = await invokePreview(contexts[0], idTarea);
    const destination = preview.Destinos[0];
    const payload = {
      idTarea,
      idUsuarioWorkflowDestino: destination.IdUsuarioWorkflowDestino,
      idActividadDestino: destination.IdActividadDestino,
      tokenVersion: preview.TokenVersion
    };
    results = await Promise.all(contexts.map((context) => invokeExecution(context, payload)));
  } finally {
    try {
      afterState = await queryFingerprint(pool, stateSql, idTarea);
      afterAudit = await queryFingerprint(pool, auditSql, idTarea);
    } finally {
      await Promise.all(contexts.map((context) => context.close()));
      await pool.end();
      await browser.close();
      await assertLocalGateOff();
    }
  }

  const exitos = results.filter((result) => result.exito);
  const bloqueados = results.filter((result) => !result.exito);
  const stateChanged = beforeState !== afterState;
  const auditChanged = beforeAudit !== afterAudit;
  const approved = exitos.length === 1 && exitos[0].estadoFinal === 'completada' &&
    bloqueados.length === 1 && allowedLoserCodes.has(bloqueados[0].codigo) && stateChanged && auditChanged;
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarEnvioUsuario',
    solicitudes: 2,
    exitos: exitos.length,
    codigosBloqueo: bloqueados.map((result) => result.codigo),
    estadoFinalGanador: exitos[0]?.estadoFinal || null,
    estadoCambio: stateChanged,
    auditoriaCambio: auditChanged,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit },
    aprobada: approved
  });
  console.log(`DOC-28 concurrencia: ${exitos.length}/2 envíos efectivos; evidencia saneada generada.`);
  if (!approved) process.exitCode = 1;
}

main().catch(() => {
  console.error('La carrera DOC-28 no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});

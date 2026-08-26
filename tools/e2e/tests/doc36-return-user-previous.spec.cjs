'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');
const {
  queryFinalActivity,
  queryFingerprint: queryOdbcFingerprint
} = require('../scripts/support/doc32-e2e-odbc.cjs');

const prefix = 'DOC36_E2E';
const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const launchOptions = { headless: true };
if (process.env.DOC36_E2E_BROWSER_PATH?.trim()) launchOptions.executablePath = process.env.DOC36_E2E_BROWSER_PATH.trim();
else if (process.env.DOC36_E2E_BROWSER_CHANNEL?.trim()) launchOptions.channel = process.env.DOC36_E2E_BROWSER_CHANNEL.trim();
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });

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

function baseUrl() {
  const value = required('BASE_URL');
  if (!value) throw new Error(`${variable('BASE_URL')} es obligatoria.`);
  return new URL(value).toString();
}

function endpoint(name) {
  return new URL(`webservice/WebServiceWorkflowModern.asmx/${name}`, baseUrl()).toString();
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') ||
      /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

function authenticatedNames() {
  return ['BASE_URL', 'MODULE', 'AUTHORIZED_USER', 'AUTHORIZED_PASSWORD'];
}

function protectedNames() {
  return [
    ...authenticatedNames(),
    'ENVIRONMENT',
    'ENVIRONMENT_AUTHORIZED',
    'ODBC_DSN',
    'MYSQL_USER',
    'MYSQL_PASSWORD',
    'EXECUTION_TASK_ID',
    'TASK_STATE_SQL',
    'AUDIT_SQL',
    'PREVIEW_MAX_MS'
  ];
}

function executionNames() {
  return [
    ...protectedNames(),
    'EXECUTION_AUTHORIZED',
    'EXECUTION_MAX_MS'
  ];
}

function requireNames(names) {
  const missing = names.filter((suffix) => !required(suffix));
  if (missing.length > 0) throw new Error(`Faltan variables DOC-36: ${missing.map(variable).join(', ')}.`);
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

function normalizeActivityName(value) {
  return typeof value === 'string' ? value.normalize('NFKC').trim().toLocaleLowerCase() : '';
}

function expectedPreviewActivity() {
  let names;
  try {
    names = JSON.parse(required('PREVIEW_ACTIVITY_NAMES'));
  } catch {
    throw new Error('El perfil DOC-36 debe declarar la actividad histórica esperada.');
  }
  expect(Array.isArray(names)).toBeTruthy();
  expect(names).toHaveLength(1);
  const activity = normalizeActivityName(names[0]);
  expect(activity).toBeTruthy();
  return activity;
}

function assertPublicResponse(dto) {
  expect(JSON.stringify(dto)).not.toMatch(/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i);
}

async function invoke(context, name, payload) {
  const started = performance.now();
  const response = await context.request.post(endpoint(name), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: 60000
  });
  const elapsedMs = Math.round(performance.now() - started);
  expect(response.ok(), 'El ASMX debe responder HTTP 200.').toBeTruthy();
  const envelope = await response.json();
  expect(envelope).toHaveProperty('d');
  expect(typeof envelope.d).toBe('object');
  assertPublicResponse(envelope.d);
  return { dto: envelope.d, elapsedMs };
}

async function queryFingerprint(sql, taskId) {
  return queryOdbcFingerprint(sql, taskId, process.env, prefix);
}

function assertLatency(elapsedMs, budgetMs) {
  expect(elapsedMs, 'El preview DOC-36 excedió el presupuesto configurado.').toBeLessThanOrEqual(budgetMs);
}

function assertExecutionLatency(elapsedMs, budgetMs) {
  expect(elapsedMs, 'La ejecución DOC-36 excedió el presupuesto configurado.').toBeLessThanOrEqual(budgetMs);
}

function evidencePath() {
  const configured = required('PREVIEW_EVIDENCE_PATH');
  const fallback = path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', 'doc36-return-user-previous-preview.json');
  const destination = configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
  if (path.relative(repositoryRoot, destination).startsWith('..')) throw new Error('La evidencia DOC-36 debe permanecer dentro del repositorio.');
  return destination;
}

async function writeEvidence(evidence) {
  const serialized = JSON.stringify(evidence);
  if (/password|cookie|token|mysql|connection/i.test(serialized)) {
    throw new Error('La evidencia DOC-36 contiene un campo sensible no permitido.');
  }
  const destination = evidencePath();
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
}

test.beforeAll(async () => {
  await assertLocalGateOff();
});

test.afterAll(async () => {
  await assertLocalGateOff();
});

test('@doc36-preview El preview de Usuario anterior no cambia estado ni auditoría', async ({ browser }) => {
  requireNames(protectedNames());
  if (required('ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true') throw new Error('La autorización de ambiente debe ser true.');
  const taskId = positiveInteger('EXECUTION_TASK_ID');
  const stateSql = required('TASK_STATE_SQL');
  const auditSql = required('AUDIT_SQL');
  assertReadOnlySql(stateSql, variable('TASK_STATE_SQL'));
  assertReadOnlySql(auditSql, variable('AUDIT_SQL'));
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let preview;
  let elapsedMs;
  try {
    context = await login(browser);
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    ({ dto: preview, elapsedMs } = await invoke(context, 'PreviewDevolverUsuarioAnterior', { idTarea: taskId }));
  } finally {
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, positiveInteger('PREVIEW_MAX_MS'));
  expect(preview.Error, 'La tarea preparada debe tener un usuario histórico elegible.').toBeNull();
  expect(preview.TokenVersion, 'El preview debe emitir token opaco.').toBeTruthy();
  expect(normalizeActivityName(preview.Contexto?.ActividadAnterior)).toBeTruthy();
  expect(typeof preview.Contexto?.UsuarioAnterior).toBe('string');
  expect(preview.Contexto.UsuarioAnterior.trim()).toBeTruthy();
  expect(afterState).toBe(beforeState);
  expect(afterAudit).toBe(beforeAudit);
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'PreviewDevolverUsuarioAnterior',
    codigo: preview?.Error?.Codigo || null,
    actividadEsperadaCoincide: true,
    latenciaMs: elapsedMs,
    estadoSinCambio: true,
    auditoriaSinCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc36-execute La devolución a Usuario anterior usa exclusivamente el preview vigente', async ({ browser }) => {
  requireNames(executionNames());
  if (required('ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true' || required('EXECUTION_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La ejecución DOC-36 requiere autorizaciones explícitas de ambiente y ejecución.');
  }
  const taskId = positiveInteger('EXECUTION_TASK_ID');
  const stateSql = required('TASK_STATE_SQL');
  const auditSql = required('AUDIT_SQL');
  assertReadOnlySql(stateSql, variable('TASK_STATE_SQL'));
  assertReadOnlySql(auditSql, variable('AUDIT_SQL'));
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let preview;
  let execution;
  try {
    context = await login(browser);
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    ({ dto: preview } = await invoke(context, 'PreviewDevolverUsuarioAnterior', { idTarea: taskId }));
    expect(preview.Error, 'La tarea descartable debe entregar un preview autorizado.').toBeNull();
    expect(preview.TokenVersion, 'La ejecución debe recibir el token del preview vigente.').toBeTruthy();
    expect(normalizeActivityName(preview.Contexto?.ActividadAnterior)).toBeTruthy();
    execution = await invoke(context, 'EjecutarDevolverUsuarioAnterior', { idTarea: taskId, tokenVersion: preview.TokenVersion });
  } finally {
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertExecutionLatency(execution.elapsedMs, positiveInteger('EXECUTION_MAX_MS'));
  expect(execution.dto.Exito, `La devolución fue rechazada con ${execution.dto.CodigoBloqueo || execution.dto.Error?.Codigo || 'código no público'}.`).toBeTruthy();
  expect(execution.dto.EstadoFinal).toBe('completada');
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  const finalActivityMatches = await queryFinalActivity(taskId, preview.Contexto?.ActividadAnterior, process.env, prefix);
  expect(finalActivityMatches, 'La actividad final no coincide con la actividad histórica configurada.').toBeTruthy();
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverUsuarioAnterior',
    exito: true,
    estadoFinal: execution.dto.EstadoFinal,
    actividadEsperadaCoincide: true,
    latenciaMs: execution.elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

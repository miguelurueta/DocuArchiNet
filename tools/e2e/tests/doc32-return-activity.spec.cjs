'use strict';

const { test, expect, chromium } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');
const {
  queryFinalActivity: queryOdbcFinalActivity,
  queryFingerprint: queryOdbcFingerprint
} = require('../scripts/support/doc32-e2e-odbc.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
// La navegación a Gestión se ejecuta siempre sin interfaz. Además de ser el
// modo de la configuración compartida, declararlo aquí mantiene idéntico el
// Chromium usado por el fixture y por la sonda manual del helper.
const launchOptions = { headless: true };
if (process.env.DOC32_E2E_BROWSER_PATH?.trim()) launchOptions.executablePath = process.env.DOC32_E2E_BROWSER_PATH.trim();
else if (process.env.DOC32_E2E_BROWSER_CHANNEL?.trim()) launchOptions.channel = process.env.DOC32_E2E_BROWSER_CHANNEL.trim();
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });

function required(name) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function baseUrl() {
  const value = required('DOC32_E2E_BASE_URL');
  if (!value) return null;
  // Conserva la semántica estándar de URL: una base sin barra final se
  // interpreta como el documento de entrada del sitio, igual que el
  // diagnóstico público y los clientes ya usados en este ambiente.
  return new URL(value).toString();
}

function endpoint(name) {
  return new URL(`webservice/WebServiceWorkflowModern.asmx/${name}`, baseUrl()).toString();
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') ||
      /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

async function queryFingerprint(sql, idTarea) {
  return queryOdbcFingerprint(sql, idTarea);
}

async function finalActivityMatches(idTarea, expectedActivityName) {
  return queryOdbcFinalActivity(idTarea, expectedActivityName);
}

function latencyBudget(name) {
  const budget = positiveInteger(name);
  return budget;
}

function assertLatency(elapsedMs, budgetMs, label) {
  expect(elapsedMs, `${label} excedió el presupuesto configurado.`).toBeLessThanOrEqual(budgetMs);
}

function login(browser, environment = process.env, preflightOnly = false, timeoutMilliseconds) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC32_E2E_MODULE',
    userEnvironmentVariable: 'DOC32_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC32_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC32_E2E_IGNORE_HTTPS_ERRORS === 'true',
    environment,
    preflightOnly,
    timeoutMilliseconds
  });
}

function assertPublicResponse(dto) {
  const serialized = JSON.stringify(dto);
  expect(serialized).not.toMatch(/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i);
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

async function invokePreview(context, idTarea) {
  // La selección configurada puede estar después del primer resultado. El
  // servicio limita este preview a 50, por lo que se mantiene acotado y el
  // conector sigue derivándose exclusivamente de la misma respuesta vigente.
  return invoke(context, 'PreviewDevolverActividad', { idTarea, termino: '', cursor: '', tamanoPagina: 50 });
}

function normalizeActivityName(value) {
  return typeof value === 'string' ? value.normalize('NFKC').trim().toLocaleLowerCase() : '';
}

function expectedPreviewActivities() {
  let names;
  try {
    names = JSON.parse(required('DOC32_E2E_PREVIEW_ACTIVITY_NAMES'));
  } catch {
    throw new Error('El perfil DOC-32 debe declarar las actividades esperadas del preview.');
  }
  expect(Array.isArray(names) && names.length > 0).toBeTruthy();
  const normalized = names.map(normalizeActivityName);
  expect(normalized.every(Boolean) && new Set(normalized).size === normalized.length).toBeTruthy();
  return normalized.sort();
}

function previewMatchesExpectedActivities(preview) {
  const expected = expectedPreviewActivities();
  expect(preview.HayMas, 'El preview configurado debe entregar todas las actividades esperadas.').toBeFalsy();
  const actual = preview.Destinos.map((destination) => normalizeActivityName(destination?.NombreActividad));
  expect(actual.every(Boolean) && new Set(actual).size === actual.length).toBeTruthy();
  expect(actual.sort(), 'El preview no coincide con las actividades configuradas.').toEqual(expected);
  return true;
}

function currentDestination(preview, activityName) {
  expect(preview.Error, 'La tarea descartable debe entregar preview autorizado.').toBeNull();
  expect(preview.TokenVersion, 'El token debe provenir del preview vigente.').toBeTruthy();
  expect(Array.isArray(preview.Destinos)).toBeTruthy();
  expect(preview.Destinos.length).toBeGreaterThan(0);
  const expectedActivity = normalizeActivityName(activityName);
  expect(expectedActivity, 'El perfil DOC-32 debe fijar una actividad de devolución.').toBeTruthy();
  const matches = preview.Destinos.filter((candidate) => normalizeActivityName(candidate?.NombreActividad) === expectedActivity);
  expect(matches, 'La actividad configurada no está disponible en el preview vigente.').toHaveLength(1);
  const [destination] = matches;
  expect(Number.isSafeInteger(destination.IdConector) && destination.IdConector > 0).toBeTruthy();
  return destination;
}

function safeFunctionalCode(result) {
  const candidate = result?.CodigoBloqueo || result?.Error?.Codigo || '';
  return /^[A-Z0-9_]{1,80}$/.test(candidate) ? candidate : 'WORKFLOW_RETURN_UNCLASSIFIED';
}

function safeArtifactPath(kind) {
  const configured = required(`DOC32_E2E_${kind.toUpperCase()}_EVIDENCE_PATH`);
  const fallback = path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', `doc32-return-activity-${kind}.json`);
  const destination = configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
  if (path.relative(repositoryRoot, destination).startsWith('..')) throw new Error('La evidencia DOC-32 debe permanecer dentro del repositorio.');
  return destination;
}

async function writeEvidence(kind, evidence) {
  const serialized = JSON.stringify(evidence);
  if (/password|cookie|token|destino|usuario|mysql|connection/i.test(serialized)) {
    throw new Error('La evidencia DOC-32 contiene un campo sensible no permitido.');
  }
  const destination = safeArtifactPath(kind);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
}

function requireNames(names) {
  const missing = names.filter((name) => !required(name));
  if (missing.length > 0) throw new Error(`Faltan variables DOC-32: ${missing.join(', ')}.`);
}

function authenticatedNames() {
  return ['DOC32_E2E_BASE_URL', 'DOC32_E2E_MODULE', 'DOC32_E2E_AUTHORIZED_USER', 'DOC32_E2E_AUTHORIZED_PASSWORD'];
}

function protectedNames() {
  return [
    ...authenticatedNames(),
    'DOC32_E2E_ENVIRONMENT',
    'DOC32_E2E_ENVIRONMENT_AUTHORIZED',
    'DOC32_E2E_ODBC_DSN',
    'DOC32_E2E_MYSQL_USER',
    'DOC32_E2E_MYSQL_PASSWORD',
    'DOC32_E2E_TASK_STATE_SQL',
    'DOC32_E2E_AUDIT_SQL'
  ];
}

async function assertLegacyPagesUnchanged() {
  const { execFile } = require('node:child_process');
  const { promisify } = require('node:util');
  const execute = promisify(execFile);
  const result = await execute('git', ['diff', '--name-only', '--', 'workflow/Webworkflow.aspx', 'workflow/Webworkflow.aspx.vb'], { cwd: repositoryRoot });
  expect(result.stdout.trim()).toBe('');
}

test.beforeAll(async () => {
  await assertLocalGateOff();
});

test.afterAll(async () => {
  await assertLocalGateOff();
  await assertLegacyPagesUnchanged();
});

test('@doc32-login-probe El formulario público usa el helper compartido sin autenticar', async () => {
  test.skip(process.env.DOC32_E2E_LOGIN_PROBE !== 'true', 'Sonda de diagnóstico explícita.');
  requireNames(['DOC32_E2E_BASE_URL', 'DOC32_E2E_MODULE']);
  const browser = await chromium.launch(launchOptions);
  let context;
  try {
    context = await login(browser, {
      ...process.env,
      DOC32_E2E_AUTHORIZED_USER: 'login-probe',
      DOC32_E2E_AUTHORIZED_PASSWORD: 'login-probe'
  }, true, 15000);
  } finally {
    await context?.close();
    await browser.close();
  }
});

test('@doc32-anonymous PreviewDevolverActividad sin sesión bloquea sin exponer destinos', async ({ browser }) => {
  requireNames(['DOC32_E2E_BASE_URL']);
  const context = await browser.newContext({ ignoreHTTPSErrors: process.env.DOC32_E2E_IGNORE_HTTPS_ERRORS === 'true' });
  try {
    const { dto } = await invokePreview(context, 1);
    expect(dto.Error?.Codigo).toBe('WORKFLOW_RETURN_CONTEXT_INVALID');
    expect(dto.Destinos || []).toHaveLength(0);
  } finally {
    await context.close();
  }
});

test('@doc32-validation Una sesión válida bloquea parámetros inválidos sin mutar', async ({ browser }) => {
  requireNames(authenticatedNames());
  const context = await login(browser);
  try {
    const { dto } = await invoke(context, 'PreviewDevolverActividad', { idTarea: 0, termino: '', cursor: '', tamanoPagina: 1 });
    expect(dto.Error?.Codigo).toBe('WORKFLOW_RETURN_TASK_INVALID');
    expect(dto.Destinos || []).toHaveLength(0);
  } finally {
    await context.close();
  }
});

test('@doc32-preview El preview real conserva las huellas de estado y auditoría', async ({ browser }) => {
  requireNames([...protectedNames(), 'DOC32_E2E_EXECUTION_TASK_ID', 'DOC32_E2E_PREVIEW_ACTIVITY_NAMES', 'DOC32_E2E_PREVIEW_MAX_MS']);
  if (required('DOC32_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true') throw new Error('La autorización de ambiente debe ser true.');
  const idTarea = positiveInteger('DOC32_E2E_EXECUTION_TASK_ID');
  const stateSql = required('DOC32_E2E_TASK_STATE_SQL');
  const auditSql = required('DOC32_E2E_AUDIT_SQL');
  assertReadOnlySql(stateSql, 'DOC32_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC32_E2E_AUDIT_SQL');
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let preview;
  let elapsedMs;
  let actividadesEsperadasCoinciden;
  try {
    // El login sólo crea una sesión Gestión; las huellas siguen leyéndose
    // inmediatamente antes del único endpoint de preview no mutante.
    context = await login(browser);
    beforeState = await queryFingerprint(stateSql, idTarea);
    beforeAudit = await queryFingerprint(auditSql, idTarea);
    ({ dto: preview, elapsedMs } = await invokePreview(context, idTarea));
  } finally {
    afterState = await queryFingerprint(stateSql, idTarea);
    afterAudit = await queryFingerprint(auditSql, idTarea);
    await context?.close();
  }
  assertLatency(elapsedMs, latencyBudget('DOC32_E2E_PREVIEW_MAX_MS'), 'El preview DOC-32');
  actividadesEsperadasCoinciden = previewMatchesExpectedActivities(preview);
  expect(afterState).toBe(beforeState);
  expect(afterAudit).toBe(beforeAudit);
  await writeEvidence('preview', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'PreviewDevolverActividad',
    codigo: preview?.Error?.Codigo || null,
    cantidad: Array.isArray(preview?.Destinos) ? preview.Destinos.length : 0,
    hayMas: Boolean(preview?.HayMas),
    actividadesEsperadasCoinciden,
    latenciaMs: elapsedMs,
    estadoSinCambio: true,
    auditoriaSinCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc32-execute La transición real usa el preview vigente de una tarea descartable', async ({ browser }) => {
  requireNames([...protectedNames(), 'DOC32_E2E_EXECUTION_AUTHORIZED', 'DOC32_E2E_EXECUTION_TASK_ID', 'DOC32_E2E_EXECUTION_ACTIVITY_NAME', 'DOC32_E2E_EXECUTION_FINAL_ACTIVITY_NAME', 'DOC32_E2E_EXECUTION_MAX_MS']);
  if (required('DOC32_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true' || required('DOC32_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La ejecución real DOC-32 requiere autorizaciones explícitas.');
  }
  const idTarea = positiveInteger('DOC32_E2E_EXECUTION_TASK_ID');
  const stateSql = required('DOC32_E2E_TASK_STATE_SQL');
  const auditSql = required('DOC32_E2E_AUDIT_SQL');
  assertReadOnlySql(stateSql, 'DOC32_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC32_E2E_AUDIT_SQL');
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let preview;
  let result;
  let elapsedMs;
  let actividadFinalCoincide;
  try {
    beforeState = await queryFingerprint(stateSql, idTarea);
    beforeAudit = await queryFingerprint(auditSql, idTarea);
    context = await login(browser);
    ({ dto: preview } = await invokePreview(context, idTarea));
    const destination = currentDestination(preview, required('DOC32_E2E_EXECUTION_ACTIVITY_NAME'));
    ({ dto: result, elapsedMs } = await invoke(context, 'EjecutarDevolverActividad', {
      idTarea,
      idConector: destination.IdConector,
      tokenVersion: preview.TokenVersion
    }));
  } finally {
    afterState = await queryFingerprint(stateSql, idTarea);
    afterAudit = await queryFingerprint(auditSql, idTarea);
    await context?.close();
  }
  assertLatency(elapsedMs, latencyBudget('DOC32_E2E_EXECUTION_MAX_MS'), 'La ejecución DOC-32');
  expect(result.Exito, `La devolución fue rechazada con ${safeFunctionalCode(result)}.`).toBeTruthy();
  expect(result.EstadoFinal).toBe('completada');
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  actividadFinalCoincide = await finalActivityMatches(idTarea, required('DOC32_E2E_EXECUTION_FINAL_ACTIVITY_NAME'));
  expect(actividadFinalCoincide, 'La actividad final no coincide con la actividad final esperada.').toBeTruthy();
  await writeEvidence('execution', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverActividad',
    exito: Boolean(result?.Exito),
    estadoFinal: result?.EstadoFinal || null,
    codigo: result?.CodigoBloqueo || result?.Error?.Codigo || null,
    advertencias: Array.isArray(result?.Advertencias) ? result.Advertencias.length : 0,
    latenciaMs: elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    actividadFinalCoincide,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

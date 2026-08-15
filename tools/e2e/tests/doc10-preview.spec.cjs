const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');

const DEFAULT_TASK_STATE_SQL = [
  'SELECT ID_ESTADO, INICIO_TAREAS_WORKFLOW_ID_TAREA, ID_ACTIVIDAD,',
  'FECHA_INICIO, FECHA_SELECCION, FECHA_FIN, ESTADO_TAREA, ID_USUARIO,',
  'ID_FLUJO_TRABAJO, ID_ACTIVIDAD_FLUJO_TRABAJO',
  'FROM estados_tarea_workflow',
  'WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = ?',
  'ORDER BY ID_ESTADO'
].join(' ');

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function normalizeBaseUrl(value) {
  if (!value) return null;
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function getTaskId() {
  const value = required('DOC10_E2E_TASK_ID') || '1';
  const idTarea = Number(value);
  if (!Number.isSafeInteger(idTarea) || idTarea <= 0) {
    throw new Error('DOC10_E2E_TASK_ID debe ser un entero positivo.');
  }
  return idTarea;
}

function getEvidencePath() {
  const configuredPath = required('DOC10_E2E_EVIDENCE_PATH');
  if (!configuredPath) return path.join(__dirname, '..', 'artifacts', 'doc10-preview-e2e.json');
  return path.isAbsolute(configuredPath) ? configuredPath : path.resolve(repositoryRoot, configuredPath);
}

const settings = {
  baseUrl: normalizeBaseUrl(required('DOC10_E2E_BASE_URL')),
  moduleValue: required('DOC10_E2E_MODULE'),
  authorizedUser: required('DOC10_E2E_AUTHORIZED_USER'),
  authorizedPassword: process.env.DOC10_E2E_AUTHORIZED_PASSWORD,
  unauthorizedUser: required('DOC10_E2E_UNAUTHORIZED_USER'),
  unauthorizedPassword: process.env.DOC10_E2E_UNAUTHORIZED_PASSWORD,
  mysqlUrl: process.env.DOC10_E2E_MYSQL_URL,
  auditSql: process.env.DOC10_E2E_AUDIT_SQL,
  authorizedExpectedCode: required('DOC10_E2E_AUTHORIZED_EXPECTED_CODE'),
  taskStateSql: process.env.DOC10_E2E_TASK_STATE_SQL || DEFAULT_TASK_STATE_SQL,
  evidencePath: getEvidencePath()
};

function previewUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea', settings.baseUrl).toString();
}

function loginUrl() {
  return new URL('gestor.aspx', settings.baseUrl).toString();
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql)) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura.`);
  }
  if ((sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe usar exactamente un parámetro posicional ? para idTarea.`);
  }
}

function fullE2EMissingSettings() {
  const requiredNames = [
    ['DOC10_E2E_BASE_URL', settings.baseUrl],
    ['DOC10_E2E_MODULE', settings.moduleValue],
    ['DOC10_E2E_AUTHORIZED_USER', settings.authorizedUser],
    ['DOC10_E2E_AUTHORIZED_PASSWORD', settings.authorizedPassword],
    ['DOC10_E2E_UNAUTHORIZED_USER', settings.unauthorizedUser],
    ['DOC10_E2E_UNAUTHORIZED_PASSWORD', settings.unauthorizedPassword],
    ['DOC10_E2E_MYSQL_URL', settings.mysqlUrl],
    ['DOC10_E2E_AUDIT_SQL', settings.auditSql]
  ];
  return requiredNames.filter(([, value]) => !value || !String(value).trim()).map(([name]) => name);
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

async function queryFingerprint(pool, sql, idTarea) {
  const [rows] = await pool.execute(sql, [idTarea]);
  return fingerprint(rows);
}

async function login(browser, user, password) {
  const context = await browser.newContext({
    ignoreHTTPSErrors: process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
  const page = await context.newPage();
  await page.goto(loginUrl(), { waitUntil: 'domcontentloaded' });
  await page.locator('#ContentPlacenter_DropDownListmodulos').selectOption({ value: settings.moduleValue });
  await page.locator('#ContentPlacenter_TextBoxuser').fill(user);
  await page.locator('#ContentPlacenter_TextBoxpasw').fill(password);
  const postback = page.waitForResponse((response) => {
    const request = response.request();
    return request.method() === 'POST' && response.url().split('?')[0] === loginUrl();
  });
  await page.locator('a.da-login-submit').click();
  await postback;
  await page.waitForLoadState('domcontentloaded');
  return context;
}

async function invokePreview(context, idTarea) {
  const response = await context.request.post(previewUrl(), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: { idTarea }
  });

  expect(response.ok(), 'El ASMX debe devolver HTTP 200.').toBeTruthy();
  const envelope = await response.json();
  expect(envelope, 'El ASMX debe devolver el contenedor JSON d.').toHaveProperty('d');
  return envelope.d;
}

async function writeEvidence(evidence) {
  const evidencePath = settings.evidencePath;
  await fs.mkdir(path.dirname(evidencePath), { recursive: true });
  await fs.writeFile(evidencePath, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

test('@anonymous PreviewEnviarTarea sin sesión bloquea el ASMX sin exponer destinos', async ({ browser }) => {
  if (!settings.baseUrl) {
    throw new Error('Configure DOC10_E2E_BASE_URL para probar el ASMX sin sesión.');
  }
  const context = await browser.newContext({
    ignoreHTTPSErrors: process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
  try {
    const response = await context.request.post(previewUrl(), {
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
      data: { idTarea: getTaskId() }
    });

    expect(response.ok()).toBeTruthy();
    const envelope = await response.json();
    const preview = envelope.d;
    expect(preview.Error?.Codigo).toBe('WORKFLOW_CONTEXT_INVALID');
    expect(preview.Destinos || []).toHaveLength(0);
  } finally {
    await context.close();
  }
});

test('@session La sesión Gestión válida resuelve el contexto Workflow en el ASMX', async ({ browser }) => {
  const missing = [
    ['DOC10_E2E_BASE_URL', settings.baseUrl],
    ['DOC10_E2E_MODULE', settings.moduleValue],
    ['DOC10_E2E_AUTHORIZED_USER', settings.authorizedUser],
    ['DOC10_E2E_AUTHORIZED_PASSWORD', settings.authorizedPassword]
  ].filter(([, value]) => !value || !String(value).trim()).map(([name]) => name);
  if (missing.length > 0) {
    throw new Error(`Faltan variables para verificar sesión: ${missing.join(', ')}.`);
  }

  const context = await login(browser, settings.authorizedUser, settings.authorizedPassword);
  try {
    const preview = await invokePreview(context, getTaskId());
    expect(preview.Error?.Codigo, 'La sesión Gestión no resolvió contexto Workflow.').not.toBe('WORKFLOW_CONTEXT_INVALID');
  } finally {
    await context.close();
  }
});

test('@authorization El piloto supera el gate y el usuario autenticado fuera del piloto queda bloqueado', async ({ browser }) => {
  const missing = [
    ['DOC10_E2E_BASE_URL', settings.baseUrl],
    ['DOC10_E2E_MODULE', settings.moduleValue],
    ['DOC10_E2E_AUTHORIZED_USER', settings.authorizedUser],
    ['DOC10_E2E_AUTHORIZED_PASSWORD', settings.authorizedPassword],
    ['DOC10_E2E_UNAUTHORIZED_USER', settings.unauthorizedUser],
    ['DOC10_E2E_UNAUTHORIZED_PASSWORD', settings.unauthorizedPassword]
  ].filter(([, value]) => !value || !String(value).trim()).map(([name]) => name);
  if (missing.length > 0) {
    throw new Error(`Faltan variables para verificar autorización: ${missing.join(', ')}.`);
  }

  const authorizedContext = await login(browser, settings.authorizedUser, settings.authorizedPassword);
  const unauthorizedContext = await login(browser, settings.unauthorizedUser, settings.unauthorizedPassword);
  try {
    const authorized = await invokePreview(authorizedContext, getTaskId());
    const unauthorized = await invokePreview(unauthorizedContext, getTaskId());

    expect(authorized.Error?.Codigo, 'El piloto no debe quedar bloqueado por el gate.').not.toBe('WORKFLOW_MODERN_INACTIVE');
    expect(authorized.Error?.Codigo, 'El piloto debe resolver contexto Workflow.').not.toBe('WORKFLOW_CONTEXT_INVALID');
    expect(unauthorized.Error?.Codigo, 'El usuario fuera del piloto debe quedar bloqueado.').toBe('WORKFLOW_MODERN_INACTIVE');
    expect(unauthorized.Destinos || []).toHaveLength(0);
  } finally {
    await authorizedContext.close();
    await unauthorizedContext.close();
  }
});

test('@full PreviewEnviarTarea preserva estado y auditoría para piloto y no piloto', async ({ browser }) => {
  const missing = fullE2EMissingSettings();
  if (missing.length > 0) {
    throw new Error(`Faltan variables E2E: ${missing.join(', ')}.`);
  }

  const idTarea = getTaskId();
  assertReadOnlySql(settings.taskStateSql, 'DOC10_E2E_TASK_STATE_SQL');
  assertReadOnlySql(settings.auditSql, 'DOC10_E2E_AUDIT_SQL');

  const pool = mysql.createPool(settings.mysqlUrl);
  let authorizedContext;
  let unauthorizedContext;
  let beforeTask;
  let beforeAudit;
  let afterTask;
  let afterAudit;
  let authorized;
  let unauthorized;

  try {
    beforeTask = await queryFingerprint(pool, settings.taskStateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, settings.auditSql, idTarea);

    authorizedContext = await login(browser, settings.authorizedUser, settings.authorizedPassword);
    authorized = await invokePreview(authorizedContext, idTarea);

    unauthorizedContext = await login(browser, settings.unauthorizedUser, settings.unauthorizedPassword);
    unauthorized = await invokePreview(unauthorizedContext, idTarea);
  } finally {
    afterTask = await queryFingerprint(pool, settings.taskStateSql, idTarea);
    afterAudit = await queryFingerprint(pool, settings.auditSql, idTarea);
    await authorizedContext?.close();
    await unauthorizedContext?.close();
    await pool.end();
  }

  if (settings.authorizedExpectedCode) {
    expect(authorized.Error?.Codigo, 'El piloto debe recibir el bloqueo funcional esperado.').toBe(settings.authorizedExpectedCode);
    expect(authorized.Destinos || []).toHaveLength(0);
  } else {
    expect(authorized.Error).toBeNull();
    expect(Array.isArray(authorized.Destinos)).toBeTruthy();
    expect(authorized.Destinos.length).toBeGreaterThan(0);
  }
  expect(unauthorized.Error?.Codigo).toBe('WORKFLOW_MODERN_INACTIVE');
  expect(unauthorized.Destinos || []).toHaveLength(0);
  expect(afterTask, 'El preview no debe modificar estado de tarea.').toBe(beforeTask);
  expect(afterAudit, 'El preview no debe modificar auditoría.').toBe(beforeAudit);

  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: previewUrl(),
    idTarea,
    autorizado: { destinos: (authorized.Destinos || []).length, tipoDecision: authorized.TipoDecision, bloqueo: authorized.Error?.Codigo || null },
    noAutorizado: { destinos: 0, bloqueo: unauthorized.Error.Codigo },
    estadoSinMutacion: true,
    auditoriaSinMutacion: true,
    huellas: { estadoAntes: beforeTask, estadoDespues: afterTask, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

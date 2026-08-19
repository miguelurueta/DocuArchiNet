const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const launchOptions = {};
if (process.env.DOC11_E2E_BROWSER_PATH && process.env.DOC11_E2E_BROWSER_PATH.trim()) {
  launchOptions.executablePath = process.env.DOC11_E2E_BROWSER_PATH.trim();
} else if (process.env.DOC11_E2E_BROWSER_CHANNEL && process.env.DOC11_E2E_BROWSER_CHANNEL.trim()) {
  launchOptions.channel = process.env.DOC11_E2E_BROWSER_CHANNEL.trim();
}
test.use({ launchOptions });

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function baseUrl() {
  const value = required('DOC11_E2E_BASE_URL');
  return value ? new URL(value.endsWith('/') ? value : `${value}/`).toString() : null;
}

function executionUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea', baseUrl()).toString();
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql)) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura.`);
  }
  if ((sql.match(/\?/g) || []).length !== 1) throw new Error(`${name} debe usar exactamente un parámetro posicional ? para idTarea.`);
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

function evidencePath() {
  const configured = required('DOC11_E2E_EVIDENCE_PATH');
  if (!configured) return path.join(__dirname, '..', 'artifacts', 'doc11-execution-e2e.json');
  return path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured);
}

async function writeEvidence(evidence) {
  const destination = evidencePath();
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC11_E2E_MODULE',
    userEnvironmentVariable: 'DOC11_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC11_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC11_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

async function invoke(context, payload) {
  const response = await context.request.post(executionUrl(), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: 60000
  });
  expect(response.ok(), 'El ASMX debe devolver HTTP 200.').toBeTruthy();
  const envelope = await response.json();
  expect(envelope, 'El ASMX debe devolver el contenedor JSON d.').toHaveProperty('d');
  expect(typeof envelope.d, 'El ASMX debe devolver un objeto JSON, no HTML ni una excepción.').toBe('object');
  expect(JSON.stringify(envelope.d)).not.toMatch(/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE)\s/i);
  return envelope.d;
}

test('@doc11-anonymous EjecutarEnvioTarea sin sesión se bloquea sin ejecutar', async ({ browser }) => {
  if (!baseUrl()) throw new Error('Configure DOC11_E2E_BASE_URL.');
  const context = await browser.newContext({ ignoreHTTPSErrors: process.env.DOC11_E2E_IGNORE_HTTPS_ERRORS === 'true' });
  try {
    const result = await invoke(context, { idTarea: 1, idConector: 1, tokenVersion: 'anonymous' });
    expect(result.Exito).toBeFalsy();
    expect(result.CodigoBloqueo).toBe('WORKFLOW_CONTEXT_INVALID');
  } finally {
    await context.close();
  }
});

test('@doc11-validation El piloto recibe bloqueo funcional para parámetros inválidos', async ({ browser }) => {
  const context = await login(browser);
  try {
    const result = await invoke(context, { idTarea: 0, idConector: 0, tokenVersion: '' });
    expect(result.Exito).toBeFalsy();
    expect(result.CodigoBloqueo).toBe('WORKFLOW_TASK_INVALID');
    expect(result.EsReintentable).toBeFalsy();
  } finally {
    await context.close();
  }
});

test('@doc11-execute La tarea descartable devuelve el resultado y las huellas esperadas', async ({ browser }) => {
  const idTarea = positiveInteger('DOC11_E2E_TASK_ID');
  const idConector = positiveInteger('DOC11_E2E_CONNECTOR_ID');
  const tokenVersion = required('DOC11_E2E_TOKEN_VERSION');
  const expectedOutcome = required('DOC11_E2E_EXPECTED_OUTCOME').toLowerCase();
  const expectedCode = required('DOC11_E2E_EXPECTED_CODE');
  const stateSql = process.env.DOC11_E2E_TASK_STATE_SQL;
  const auditSql = process.env.DOC11_E2E_AUDIT_SQL;
  assertReadOnlySql(stateSql, 'DOC11_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC11_E2E_AUDIT_SQL');

  const pool = mysql.createPool(process.env.DOC11_E2E_MYSQL_URL);
  let context;
  let result;
  let beforeState;
  let afterState;
  let beforeAudit;
  let afterAudit;
  try {
    [beforeState] = await pool.execute(stateSql, [idTarea]);
    [beforeAudit] = await pool.execute(auditSql, [idTarea]);
    context = await login(browser);
    result = await invoke(context, { idTarea, idConector, tokenVersion });
  } finally {
    [afterState] = await pool.execute(stateSql, [idTarea]);
    [afterAudit] = await pool.execute(auditSql, [idTarea]);
    await context?.close();
    await pool.end();
  }

  const stateChanged = fingerprint(beforeState) !== fingerprint(afterState);
  const auditChanged = fingerprint(beforeAudit) !== fingerprint(afterAudit);
  if (expectedOutcome === 'success') {
    expect(result.Exito, 'La tarea descartable debía completar el envío.').toBeTruthy();
    expect(stateChanged, 'El éxito debe reflejar un cambio de estado en la consulta aprobada.').toBeTruthy();
    expect(auditChanged, 'El éxito debe dejar la huella de auditoría consultada.').toBeTruthy();
  } else {
    expect(result.Exito, 'El escenario bloqueado no debe confirmar transición.').toBeFalsy();
    if (!expectedCode) throw new Error('DOC11_E2E_EXPECTED_CODE es obligatorio cuando el resultado esperado es blocked.');
    expect(result.CodigoBloqueo).toBe(expectedCode);
    expect(stateChanged, 'Un bloqueo funcional no debe cambiar el estado de la tarea.').toBeFalsy();
  }

  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: executionUrl(),
    idTarea,
    idConector,
    resultadoEsperado: expectedOutcome,
    exito: Boolean(result.Exito),
    codigo: result.CodigoBloqueo || null,
    estadoCambio: stateChanged,
    auditoriaCambio: auditChanged,
    advertencias: Array.isArray(result.Advertencias) ? result.Advertencias.length : 0
  });
});

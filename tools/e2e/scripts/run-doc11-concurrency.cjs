const { chromium } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function baseUrl() {
  const value = required('DOC11_E2E_BASE_URL');
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function executionUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea', baseUrl()).toString();
}

function loginUrl() {
  return new URL('gestor.aspx', baseUrl()).toString();
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) || (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT con un parámetro ?.`);
  }
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

function evidencePath() {
  const configured = required('DOC11_CONCURRENCY_EVIDENCE_PATH');
  if (!configured) return path.join(__dirname, '..', 'artifacts', 'doc11-execution-concurrency.json');
  return path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured);
}

async function login(browser) {
  const context = await browser.newContext({ ignoreHTTPSErrors: process.env.DOC11_E2E_IGNORE_HTTPS_ERRORS === 'true' });
  const page = await context.newPage();
  try {
    await page.goto(loginUrl(), { waitUntil: 'domcontentloaded' });
    await page.locator('#ContentPlacenter_DropDownListmodulos').selectOption({ value: required('DOC11_E2E_MODULE') });
    await page.locator('#ContentPlacenter_TextBoxuser').fill(required('DOC11_E2E_AUTHORIZED_USER'));
    await page.locator('#ContentPlacenter_TextBoxpasw').fill(process.env.DOC11_E2E_AUTHORIZED_PASSWORD);
    const postback = page.waitForResponse((response) => response.request().method() === 'POST' && response.url().split('?')[0] === loginUrl());
    await page.locator('a.da-login-submit').click();
    await postback;
    return context;
  } catch (error) {
    await context.close();
    throw error;
  } finally {
    await page.close();
  }
}

async function invoke(context, payload) {
  try {
    const response = await context.request.post(executionUrl(), {
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
      data: payload,
      timeout: 60000
    });
    if (!response.ok()) return { exito: false, codigo: `HTTP_${response.status()}` };
    const envelope = await response.json();
    const dto = envelope && envelope.d;
    if (!dto || typeof dto !== 'object') return { exito: false, codigo: 'INVALID_RESPONSE' };
    return { exito: Boolean(dto.Exito), codigo: dto.CodigoBloqueo || null };
  } catch {
    return { exito: false, codigo: 'REQUEST_FAILED' };
  }
}

async function main() {
  const idTarea = positiveInteger('DOC11_E2E_TASK_ID');
  const idConector = positiveInteger('DOC11_E2E_CONNECTOR_ID');
  const tokenVersion = required('DOC11_E2E_TOKEN_VERSION');
  const stateSql = process.env.DOC11_E2E_TASK_STATE_SQL;
  const auditSql = process.env.DOC11_E2E_AUDIT_SQL;
  assertReadOnlySql(stateSql, 'DOC11_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC11_E2E_AUDIT_SQL');

  const browser = await chromium.launch({ headless: true, channel: required('DOC11_E2E_BROWSER_CHANNEL') || undefined });
  const pool = mysql.createPool(process.env.DOC11_E2E_MYSQL_URL);
  let contexts = [];
  let beforeState;
  let afterState;
  let beforeAudit;
  let afterAudit;
  let results = [];
  try {
    [beforeState] = await pool.execute(stateSql, [idTarea]);
    [beforeAudit] = await pool.execute(auditSql, [idTarea]);
    contexts = await Promise.all([login(browser), login(browser)]);
    results = await Promise.all(contexts.map((context) => invoke(context, { idTarea, idConector, tokenVersion })));
  } finally {
    [afterState] = await pool.execute(stateSql, [idTarea]);
    [afterAudit] = await pool.execute(auditSql, [idTarea]);
    await Promise.all(contexts.map((context) => context.close()));
    await pool.end();
    await browser.close();
  }

  const exitos = results.filter((result) => result.exito).length;
  const bloqueosPermitidos = new Set(['WORKFLOW_TRANSITION_IN_PROGRESS', 'WORKFLOW_VERSION_CONFLICT', 'WORKFLOW_TASK_UNAVAILABLE']);
  const segundoControlado = results.filter((result) => !result.exito).every((result) => bloqueosPermitidos.has(result.codigo));
  const stateChanged = fingerprint(beforeState) !== fingerprint(afterState);
  const auditChanged = fingerprint(beforeAudit) !== fingerprint(afterAudit);
  const evidencia = {
    fechaUtc: new Date().toISOString(),
    endpoint: executionUrl(),
    idTarea,
    idConector,
    solicitudes: 2,
    exitos,
    codigos: results.map((result) => result.codigo),
    estadoCambio: stateChanged,
    auditoriaCambio: auditChanged,
    aprobada: exitos === 1 && segundoControlado && stateChanged && auditChanged
  };
  await fs.mkdir(path.dirname(evidencePath()), { recursive: true });
  await fs.writeFile(evidencePath(), `${JSON.stringify(evidencia, null, 2)}\n`, 'utf8');
  console.log(`DOC-11 concurrencia: ${exitos}/2 envíos efectivos; evidencia segura generada.`);
  if (!evidencia.aprobada) process.exitCode = 1;
}

main().catch(() => {
  console.error('La prueba de concurrencia DOC-11 no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});

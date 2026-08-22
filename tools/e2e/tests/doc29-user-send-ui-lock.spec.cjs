'use strict';

const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const mysql = require('mysql2/promise');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const launchOptions = {};
if (process.env.DOC28_E2E_BROWSER_PATH && process.env.DOC28_E2E_BROWSER_PATH.trim()) {
  launchOptions.executablePath = process.env.DOC28_E2E_BROWSER_PATH.trim();
} else if (process.env.DOC28_E2E_BROWSER_CHANNEL && process.env.DOC28_E2E_BROWSER_CHANNEL.trim()) {
  launchOptions.channel = process.env.DOC28_E2E_BROWSER_CHANNEL.trim();
}
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });
test.setTimeout(240000);

function required(name) {
  const value = process.env[name];
  if (typeof value !== 'string' || value.trim().length === 0) throw new Error(`Falta ${name}.`);
  return value.trim();
}

function baseUrl() {
  const value = required('DOC28_E2E_BASE_URL');
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function executionUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioUsuario', baseUrl()).toString();
}

function idTarea() {
  const value = Number(required('DOC28_E2E_TASK_ID'));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error('DOC28_E2E_TASK_ID debe ser entero positivo.');
  return value;
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

function summarizeExecutionResponse(response, body) {
  let envelope;
  let dto;
  try {
    envelope = JSON.parse(Buffer.from(body).toString('utf8'));
    dto = envelope && envelope.d;
    if (typeof dto === 'string') dto = JSON.parse(dto);
  } catch {
    return { httpOk: response.ok(), exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
  }
  if (!dto || typeof dto !== 'object') {
    return { httpOk: response.ok(), exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
  }
  return {
    httpOk: response.ok(),
    exito: dto.Exito === true,
    codigo: dto.CodigoBloqueo || (dto.Error && dto.Error.Codigo) || null,
    estadoFinal: dto.EstadoFinal || null
  };
}

async function queryFingerprint(pool, sql, taskId) {
  const [rows] = await pool.execute(sql, [taskId]);
  return fingerprint(rows);
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
}

async function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC28_E2E_MODULE',
    userEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC28_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

async function writeEvidence(evidence) {
  const target = path.join(__dirname, '..', 'artifacts', 'doc29-user-send-ui-lock-e2e.json');
  await fs.mkdir(path.dirname(target), { recursive: true });
  await fs.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

test.beforeAll(assertLocalGateOff);
test.afterAll(assertLocalGateOff);

test('@doc29-ui-lock El envío UI bloquea todo cierre mientras el único POST está pendiente', async ({ browser }) => {
  if (required('DOC28_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true' ||
      required('DOC29_E2E_UI_LOCK_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La prueba UI mutante exige las dos autorizaciones explícitas.');
  }

  const taskId = idTarea();
  const pool = mysql.createPool(required('DOC28_E2E_MYSQL_URL'));
  const stateSql = required('DOC28_E2E_TASK_STATE_SQL');
  const auditSql = required('DOC28_E2E_AUDIT_SQL');
  let context;
  let page;
  let releaseResponse;
  let signalResponseReady;
  let executionRequests = 0;
  let beforeState;
  let afterState;
  let beforeAudit;
  let afterAudit;
  let locked = false;
  let unloadGuarded = false;
  let dialogClosedAfterResponse = false;
  let executionResult = { httpOk: false, exito: false, codigo: 'NOT_STARTED', estadoFinal: null };
  let routeStarted = false;
  let resolveResponseFulfilled;
  let rejectResponseFulfilled;

  const responseReady = new Promise((resolve) => { signalResponseReady = resolve; });
  const responseRelease = new Promise((resolve) => { releaseResponse = resolve; });
  const responseFulfilled = new Promise((resolve, reject) => {
    resolveResponseFulfilled = resolve;
    rejectResponseFulfilled = reject;
  });

  try {
    beforeState = await queryFingerprint(pool, stateSql, taskId);
    beforeAudit = await queryFingerprint(pool, auditSql, taskId);
    context = await login(browser);
    page = await context.newPage();
    await page.goto(new URL('workflow/Webworkflow.aspx', baseUrl()).toString(), { waitUntil: 'domcontentloaded' });

    await expect(page.locator('#Hidden_id_tarea_selecionada')).toHaveValue(String(taskId));
    await page.locator('#workflow-user-send-trigger').click();
    await expect(page.locator('#workflow-user-send-modern-modal')).toHaveAttribute('data-workflow-user-send-state', 'lista-disponible');
    await page.locator('#workflow-user-send-modern-table-body .workflow-transition-modal__select:not([disabled])').first().click();

    const dialog = page.locator('.confirmation-dialog');
    const close = dialog.locator('.confirmation-dialog__close');
    const cancel = dialog.locator('.confirmation-dialog__cancel');
    const primary = dialog.locator('.confirmation-dialog__primary');
    const backdrop = dialog.locator('.confirmation-dialog__backdrop');
    await expect(dialog).toBeVisible();

    await page.route(executionUrl(), async (route) => {
      routeStarted = true;
      try {
        executionRequests += 1;
        const response = await route.fetch({ timeout: 180000 });
        const body = await response.body();
        executionResult = summarizeExecutionResponse(response, body);
        console.log(`DOC-29 respuesta ASMX saneada: ${JSON.stringify(executionResult)}`);
        signalResponseReady();
        await responseRelease;
        await route.fulfill({ response, body });
        resolveResponseFulfilled();
      } catch (error) {
        rejectResponseFulfilled(error);
        throw error;
      }
    });

    await primary.click();
    await expect(dialog).toHaveAttribute('data-confirmation-dialog-state', 'enviando');
    await responseReady;
    await expect(primary).toBeDisabled();
    await expect(cancel).toBeDisabled();
    await expect(close).toBeDisabled();

    await backdrop.click({ force: true });
    await page.keyboard.press('Escape');
    const apiCloseAccepted = await page.evaluate(() => window.ConfirmationDialog.close());
    unloadGuarded = await page.evaluate(() => {
      const event = new Event('beforeunload', { cancelable: true });
      window.dispatchEvent(event);
      return event.defaultPrevented;
    });
    await expect(dialog).toHaveAttribute('data-confirmation-dialog-state', 'enviando');
    await expect(dialog.locator('.confirmation-dialog__status')).toContainText('Espere la respuesta antes de cerrar');
    expect(apiCloseAccepted).toBeFalsy();
    expect(unloadGuarded).toBeTruthy();
    locked = true;

    releaseResponse();
    await responseFulfilled;
    if (executionResult.exito) {
      await expect(dialog).toBeHidden({ timeout: 30000 });
      dialogClosedAfterResponse = true;
    } else {
      await expect(dialog).toBeVisible({ timeout: 30000 });
    }
  } finally {
    releaseResponse?.();
    if (routeStarted) {
      try { await responseFulfilled; } catch (ignored) {}
    }
    await page?.unroute(executionUrl());
    afterState = await queryFingerprint(pool, stateSql, taskId);
    afterAudit = await queryFingerprint(pool, auditSql, taskId);
    await context?.close();
    await pool.end();
  }

  const stateChanged = beforeState !== afterState;
  const auditChanged = beforeAudit !== afterAudit;
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarEnvioUsuario',
    solicitudesMutantes: executionRequests,
    cierreBloqueado: locked,
    recargaBloqueada: unloadGuarded,
    cierreTrasRespuesta: dialogClosedAfterResponse,
    respuestaAsmx: executionResult,
    estadoCambio: stateChanged,
    auditoriaCambio: auditChanged,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
  expect(executionRequests).toBe(1);
  expect(locked).toBeTruthy();
  expect(unloadGuarded).toBeTruthy();
  expect(executionResult.httpOk).toBeTruthy();
  expect(executionResult.exito, `El ASMX bloqueó la transición con ${executionResult.codigo || 'un código no informado'}.`).toBeTruthy();
  expect(dialogClosedAfterResponse).toBeTruthy();
  expect(stateChanged).toBeTruthy();
  expect(auditChanged).toBeTruthy();
});

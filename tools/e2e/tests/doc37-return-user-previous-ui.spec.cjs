'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');
const {
  queryFinalActivity: queryOdbcFinalActivity,
  queryFingerprint: queryOdbcFingerprint
} = require('../scripts/support/doc32-e2e-odbc.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const e2ePrefix = 'DOC37_E2E';
test.use({ screenshot: 'off', trace: 'off', video: 'off' });

function required(name) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function baseUrl() {
  const value = required(`${e2ePrefix}_BASE_URL`);
  if (!value) throw new Error(`${e2ePrefix}_BASE_URL es obligatoria.`);
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

function requireNames(names) {
  const missing = names.filter((name) => !required(name));
  if (missing.length) throw new Error(`Faltan variables DOC-37: ${missing.join(', ')}.`);
}

function authenticatedNames() {
  return [`${e2ePrefix}_BASE_URL`, `${e2ePrefix}_MODULE`, `${e2ePrefix}_AUTHORIZED_USER`, `${e2ePrefix}_AUTHORIZED_PASSWORD`];
}

function protectedNames() {
  return [
    ...authenticatedNames(),
    `${e2ePrefix}_ENVIRONMENT`,
    `${e2ePrefix}_ENVIRONMENT_AUTHORIZED`,
    `${e2ePrefix}_ODBC_DSN`,
    `${e2ePrefix}_MYSQL_USER`,
    `${e2ePrefix}_MYSQL_PASSWORD`,
    `${e2ePrefix}_TASK_STATE_SQL`,
    `${e2ePrefix}_AUDIT_SQL`
  ];
}

function assertLatency(elapsedMs, name, label) {
  expect(elapsedMs, `${label} excedió el presupuesto configurado.`).toBeLessThanOrEqual(positiveInteger(name));
}

function summarizeExecutionResponse(response, body) {
  let dto;
  try {
    dto = JSON.parse(Buffer.from(body).toString('utf8')).d;
    if (typeof dto === 'string') dto = JSON.parse(dto);
  } catch {
    return { httpOk: response.ok(), exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
  }
  if (!dto || typeof dto !== 'object') return { httpOk: response.ok(), exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
  return {
    httpOk: response.ok(),
    exito: dto.Exito === true,
    codigo: dto.CodigoBloqueo || dto.Error?.Codigo || null,
    estadoFinal: dto.EstadoFinal || null
  };
}

function safeArtifactPath(stage) {
  return path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', `doc37-return-user-previous-ui-${stage}.json`);
}

async function writeEvidence(stage, evidence) {
  const forbidden = /passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|usuario|user|destino|destination|actividad|activity/i;
  for (const key of Object.keys(evidence)) {
    if (forbidden.test(key)) throw new Error('La evidencia DOC-37 contiene un campo sensible no permitido.');
  }
  const serialized = JSON.stringify(evidence);
  if (forbidden.test(serialized)) throw new Error('La evidencia DOC-37 contiene un campo sensible no permitido.');
  const destination = safeArtifactPath(stage);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
}

async function assertWorkflowPagesCommitted() {
  const { execFile } = require('node:child_process');
  const { promisify } = require('node:util');
  const execute = promisify(execFile);
  const result = await execute('git', ['diff', '--name-only', '--', 'workflow/Webworkflow.aspx', 'workflow/Webworkflow.aspx.vb'], { cwd: repositoryRoot });
  expect(result.stdout.trim()).toBe('');
}

async function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: `${e2ePrefix}_MODULE`,
    userEnvironmentVariable: `${e2ePrefix}_AUTHORIZED_USER`,
    passwordEnvironmentVariable: `${e2ePrefix}_AUTHORIZED_PASSWORD`,
    ignoreHTTPSErrors: required(`${e2ePrefix}_IGNORE_HTTPS_ERRORS`) === 'true'
  });
}

function queryFingerprint(sql, taskId) {
  return queryOdbcFingerprint(sql, taskId, process.env, e2ePrefix);
}

function finalActivityMatches(taskId, expectedActivityName) {
  return queryOdbcFinalActivity(taskId, expectedActivityName, process.env, e2ePrefix);
}

function selectedTaskInput(page) {
  return page.locator('#Hidden_id_tarea_selecionada');
}

async function selectAuthorizedTask(page, taskId) {
  await page.goto(new URL('workflow/Webworkflow.aspx', baseUrl()).toString(), { waitUntil: 'domcontentloaded' });
  const selectedTask = selectedTaskInput(page);
  const expectedTaskId = String(taskId);
  if (await selectedTask.inputValue() !== expectedTaskId) {
    const selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]`);
    await expect(selectCommand, 'La tarea autorizada no está disponible para seleccionarse en la UI Workflow.').toHaveCount(1);
    await selectCommand.click();
    await expect(selectedTask, 'La UI Workflow no confirmó la selección de la tarea autorizada.').toHaveValue(expectedTaskId, { timeout: 30000 });
  }
}

async function openPreview(page, taskId) {
  const modal = page.locator('#workflow-return-user-previous-modern-modal');
  const trigger = page.locator('#workflow-return-user-previous-trigger');
  await expect(selectedTaskInput(page)).toHaveValue(String(taskId));
  if (!await trigger.isVisible()) await page.getByRole('link', { name: 'Devolver', exact: true }).click();
  await expect(trigger).toBeVisible();
  await trigger.click();
  await expect(modal).toHaveAttribute('data-workflow-return-user-previous-state', 'listo-para-confirmar', { timeout: 30000 });
  const context = await page.locator('#workflow-return-user-previous-modern-context dd').allTextContents();
  expect(context).toHaveLength(3);
  const actividadAnterior = typeof context[1] === 'string' ? context[1].normalize('NFKC').trim() : '';
  expect(actividadAnterior, 'El preview UI debe representar la actividad histórica resuelta por el servidor.').toBeTruthy();
  return { modal, actividadAnterior };
}

async function openConfirmation(page) {
  await page.locator('#workflow-return-user-previous-modern-confirm:not([disabled])').click();
  const dialog = page.locator('.confirmation-dialog');
  await expect(dialog).toBeVisible();
  return dialog;
}

test.beforeAll(assertLocalGateOff);
test.afterAll(async () => {
  await assertLocalGateOff();
  await assertWorkflowPagesCommitted();
});

test('@doc37-ui-preview El preview UI de Usuario anterior no cambia estado ni auditoría', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_UI_EXECUTION_TASK_ID`, `${e2ePrefix}_PREVIEW_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true') throw new Error('La autorización de ambiente debe ser true.');
  const taskId = positiveInteger(`${e2ePrefix}_UI_EXECUTION_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let elapsedMs = 0;
  try {
    context = await login(browser);
    const page = await context.newPage();
    await selectAuthorizedTask(page, taskId);
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    const started = performance.now();
    await openPreview(page, taskId);
    elapsedMs = Math.round(performance.now() - started);
  } finally {
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, `${e2ePrefix}_PREVIEW_MAX_MS`, 'El preview UI DOC-37');
  expect(afterState).toBe(beforeState);
  expect(afterAudit).toBe(beforeAudit);
  await writeEvidence('preview', {
    fechaUtc: new Date().toISOString(),
    latenciaMs: elapsedMs,
    estadoSinCambio: true,
    auditoriaSinCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc37-ui-execute La interfaz ejecuta únicamente el preview vigente de Usuario anterior', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_EXECUTION_AUTHORIZED`, `${e2ePrefix}_UI_EXECUTION_TASK_ID`, `${e2ePrefix}_UI_EXECUTION_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true' || required(`${e2ePrefix}_EXECUTION_AUTHORIZED`).toLowerCase() !== 'true') {
    throw new Error('La ejecución UI DOC-37 requiere autorizaciones explícitas.');
  }
  const taskId = positiveInteger(`${e2ePrefix}_UI_EXECUTION_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  const executionEndpoint = endpoint('EjecutarDevolverUsuarioAnterior');
  let context;
  let page;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let elapsedMs = 0;
  let actividadAnterior = null;
  let payloadWasMinimal = false;
  let executionResult = { httpOk: false, exito: false, codigo: 'NOT_STARTED', estadoFinal: null };
  try {
    context = await login(browser);
    page = await context.newPage();
    await selectAuthorizedTask(page, taskId);
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    await page.route(executionEndpoint, async (route) => {
      const payload = route.request().postDataJSON();
      payloadWasMinimal = !!payload && Object.keys(payload).sort().join(',') === 'idTarea,tokenVersion' && payload.idTarea === taskId && typeof payload.tokenVersion === 'string' && !!payload.tokenVersion.trim();
      const response = await route.fetch({ timeout: 60000 });
      const body = await response.body();
      executionResult = summarizeExecutionResponse(response, body);
      await route.fulfill({ response, body });
    });
    ({ actividadAnterior } = await openPreview(page, taskId));
    const dialog = await openConfirmation(page);
    const started = performance.now();
    await dialog.locator('.confirmation-dialog__primary').click();
    await expect(dialog).toBeHidden({ timeout: 30000 });
    elapsedMs = Math.round(performance.now() - started);
    await expect(page.locator('#workflow-return-user-previous-success-message')).toBeVisible();
  } finally {
    await page?.unroute(executionEndpoint);
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, `${e2ePrefix}_UI_EXECUTION_MAX_MS`, 'La ejecución UI DOC-37');
  expect(payloadWasMinimal).toBeTruthy();
  expect(executionResult.httpOk).toBeTruthy();
  expect(executionResult.exito, `La devolución UI fue rechazada con ${executionResult.codigo || 'un código no informado'}.`).toBeTruthy();
  expect(executionResult.estadoFinal).toBe('completada');
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  expect(await finalActivityMatches(taskId, actividadAnterior)).toBeTruthy();
  await writeEvidence('execution', {
    fechaUtc: new Date().toISOString(),
    exito: true,
    estadoFinal: executionResult.estadoFinal,
    payloadMinimo: payloadWasMinimal,
    latenciaMs: elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc37-ui-lock Una respuesta pendiente no permite duplicar ni abandonar la devolución', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_UI_LOCK_AUTHORIZED`, `${e2ePrefix}_UI_LOCK_TASK_ID`, `${e2ePrefix}_UI_LOCK_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true' || required(`${e2ePrefix}_UI_LOCK_AUTHORIZED`).toLowerCase() !== 'true') {
    throw new Error('El bloqueo UI DOC-37 requiere autorizaciones explícitas.');
  }
  const taskId = positiveInteger(`${e2ePrefix}_UI_LOCK_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  const executionEndpoint = endpoint('EjecutarDevolverUsuarioAnterior');
  let context;
  let page;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let releaseResponse;
  let signalResponseReady;
  let resolveResponseFulfilled;
  let rejectResponseFulfilled;
  let routeStarted = false;
  let executionRequests = 0;
  let elapsedMs = 0;
  let actividadAnterior = null;
  let pendingControlsLocked = false;
  let modalWasLocked = false;
  let unloadGuarded = false;
  let dialogClosedAfterResponse = false;
  let executionResult = { httpOk: false, exito: false, codigo: 'NOT_STARTED', estadoFinal: null };
  const responseReady = new Promise((resolve) => { signalResponseReady = resolve; });
  const responseRelease = new Promise((resolve) => { releaseResponse = resolve; });
  const responseFulfilled = new Promise((resolve, reject) => {
    resolveResponseFulfilled = resolve;
    rejectResponseFulfilled = reject;
  });
  try {
    context = await login(browser);
    page = await context.newPage();
    await selectAuthorizedTask(page, taskId);
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    await page.route(executionEndpoint, async (route) => {
      routeStarted = true;
      executionRequests += 1;
      try {
        const response = await route.fetch({ timeout: 180000 });
        const body = await response.body();
        executionResult = summarizeExecutionResponse(response, body);
        signalResponseReady();
        await responseRelease;
        await route.fulfill({ response, body });
        resolveResponseFulfilled();
      } catch (error) {
        rejectResponseFulfilled(error);
        throw error;
      }
    });
    const { modal, actividadAnterior: previewActividadAnterior } = await openPreview(page, taskId);
    actividadAnterior = previewActividadAnterior;
    const dialog = await openConfirmation(page);
    const primary = dialog.locator('.confirmation-dialog__primary');
    const cancel = dialog.locator('.confirmation-dialog__cancel');
    const close = dialog.locator('.confirmation-dialog__close');
    const backdrop = dialog.locator('.confirmation-dialog__backdrop');
    const started = performance.now();
    await primary.click();
    await expect(dialog).toHaveAttribute('data-confirmation-dialog-state', 'enviando');
    await responseReady;
    await expect(primary).toBeDisabled();
    await expect(cancel).toBeDisabled();
    await expect(close).toBeDisabled();
    await backdrop.click({ force: true });
    await page.keyboard.press('Escape');
    const confirmationApiCloseAccepted = await page.evaluate(() => window.ConfirmationDialog.close());
    await page.locator('#workflow-return-user-previous-modern-modal [data-workflow-return-user-previous-close]').click({ force: true });
    await expect(modal).toBeVisible();
    unloadGuarded = await page.evaluate(() => {
      const event = new Event('beforeunload', { cancelable: true });
      window.dispatchEvent(event);
      return event.defaultPrevented;
    });
    pendingControlsLocked = true;
    modalWasLocked = !confirmationApiCloseAccepted;
    expect(unloadGuarded).toBeTruthy();
    releaseResponse();
    await responseFulfilled;
    elapsedMs = Math.round(performance.now() - started);
    if (executionResult.exito) {
      await expect(dialog).toBeHidden({ timeout: 30000 });
      await expect(modal).toBeHidden({ timeout: 30000 });
      dialogClosedAfterResponse = true;
    }
  } finally {
    releaseResponse?.();
    if (routeStarted) {
      try { await responseFulfilled; } catch (ignored) {}
    }
    await page?.unroute(executionEndpoint);
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, `${e2ePrefix}_UI_LOCK_MAX_MS`, 'El bloqueo UI DOC-37');
  expect(executionRequests).toBe(1);
  expect(pendingControlsLocked).toBeTruthy();
  expect(modalWasLocked).toBeTruthy();
  expect(unloadGuarded).toBeTruthy();
  expect(executionResult.httpOk).toBeTruthy();
  expect(executionResult.exito, `La devolución UI fue rechazada con ${executionResult.codigo || 'un código no informado'}.`).toBeTruthy();
  expect(executionResult.estadoFinal).toBe('completada');
  expect(dialogClosedAfterResponse).toBeTruthy();
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  expect(await finalActivityMatches(taskId, actividadAnterior)).toBeTruthy();
  await writeEvidence('ui-lock', {
    fechaUtc: new Date().toISOString(),
    solicitudesMutantes: executionRequests,
    controlesBloqueados: pendingControlsLocked,
    modalBloqueado: modalWasLocked,
    abandonoBloqueado: unloadGuarded,
    cierreTrasRespuesta: dialogClosedAfterResponse,
    exito: executionResult.exito,
    estadoFinal: executionResult.estadoFinal,
    latenciaMs: elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

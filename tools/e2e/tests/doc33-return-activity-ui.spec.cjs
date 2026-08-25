'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('node:fs/promises');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');
const {
  queryActiveActivity: queryOdbcActiveActivity,
  queryFinalActivity: queryOdbcFinalActivity,
  queryFingerprint: queryOdbcFingerprint
} = require('../scripts/support/doc32-e2e-odbc.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const e2ePrefix = 'DOC33_E2E';
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

function normalizeActivityName(value) {
  return typeof value === 'string' ? value.normalize('NFKC').trim().toLocaleLowerCase() : '';
}

function expectedPreviewActivities() {
  let values;
  try {
    values = JSON.parse(required(`${e2ePrefix}_PREVIEW_ACTIVITY_NAMES`));
  } catch {
    throw new Error('El perfil DOC-33 debe declarar las actividades esperadas del preview.');
  }
  expect(Array.isArray(values) && values.length > 0).toBeTruthy();
  const normalized = values.map(normalizeActivityName);
  expect(normalized.every(Boolean) && new Set(normalized).size === normalized.length).toBeTruthy();
  return normalized.sort();
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
  if (missing.length) throw new Error(`Faltan variables DOC-33: ${missing.join(', ')}.`);
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

function latencyBudget(name) {
  return positiveInteger(name);
}

function assertLatency(elapsedMs, name, label) {
  expect(elapsedMs, `${label} excedió el presupuesto configurado.`).toBeLessThanOrEqual(latencyBudget(name));
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
  if (!dto || typeof dto !== 'object') return { httpOk: response.ok(), exito: false, codigo: 'INVALID_RESPONSE', estadoFinal: null };
  return {
    httpOk: response.ok(),
    exito: dto.Exito === true,
    codigo: dto.CodigoBloqueo || dto.Error?.Codigo || null,
    estadoFinal: dto.EstadoFinal || null
  };
}

function safeArtifactPath(stage) {
  return path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', `doc33-return-activity-ui-${stage}.json`);
}

async function writeEvidence(stage, evidence) {
  const forbiddenFields = /passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|usuario|user|destino|destination|actividad|activity/i;
  for (const key of Object.keys(evidence)) {
    if (forbiddenFields.test(key)) throw new Error('La evidencia DOC-33 contiene un campo sensible no permitido.');
  }
  const serialized = JSON.stringify(evidence);
  if (/passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|usuario|user/i.test(serialized)) {
    throw new Error('La evidencia DOC-33 contiene un campo sensible no permitido.');
  }
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

async function assertLegacyPagesUnchanged() {
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

function activeActivityName(taskId) {
  return queryOdbcActiveActivity(taskId, process.env, e2ePrefix);
}

async function openPreview(page, taskId) {
  const modal = page.locator('#workflow-return-activity-modern-modal');
  const trigger = page.locator('#workflow-return-activity-trigger');
  await page.goto(new URL('workflow/Webworkflow.aspx', baseUrl()).toString(), { waitUntil: 'domcontentloaded' });
  await expect(page.locator('#Hidden_id_tarea_selecionada')).toHaveValue(String(taskId));
  if (!await trigger.isVisible()) {
    await page.getByRole('link', { name: 'Devolver', exact: true }).click();
  }
  await expect(trigger).toBeVisible();
  await trigger.click();
  await expect(modal).toHaveAttribute('data-workflow-return-activity-state', 'lista-disponible', { timeout: 30000 });
  const destinations = await page.locator('#workflow-return-activity-modern-table-body .workflow-transition-modal__destination').allTextContents();
  const actual = destinations.map(normalizeActivityName).sort();
  expect(actual, 'El preview UI no coincide con las actividades configuradas.').toEqual(expectedPreviewActivities());
  return { modal, count: destinations.length };
}

async function selectDestination(page, activityName) {
  const rows = page.locator('#workflow-return-activity-modern-table-body tr');
  const expected = normalizeActivityName(activityName);
  const count = await rows.count();
  let match = -1;
  for (let index = 0; index < count; index += 1) {
    const name = normalizeActivityName(await rows.nth(index).locator('.workflow-transition-modal__destination').textContent());
    if (name === expected) {
      if (match !== -1) throw new Error('La actividad configurada aparece más de una vez en el preview UI.');
      match = index;
    }
  }
  expect(match, 'La actividad configurada no está disponible en el preview UI.').toBeGreaterThanOrEqual(0);
  await rows.nth(match).locator('.workflow-transition-modal__select:not([disabled])').click();
}

test.beforeAll(assertLocalGateOff);
test.afterAll(async () => {
  await assertLocalGateOff();
  await assertLegacyPagesUnchanged();
});

test('@doc33-ui-preview El preview UI real no cambia estado ni auditoría', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_UI_EXECUTION_TASK_ID`, `${e2ePrefix}_PREVIEW_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true') throw new Error('La autorización de ambiente debe ser true.');
  const taskId = positiveInteger(`${e2ePrefix}_UI_EXECUTION_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  let context;
  let page;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let destinationCount = 0;
  let elapsedMs = 0;
  try {
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    context = await login(browser);
    page = await context.newPage();
    const started = performance.now();
    ({ count: destinationCount } = await openPreview(page, taskId));
    elapsedMs = Math.round(performance.now() - started);
  } finally {
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, `${e2ePrefix}_PREVIEW_MAX_MS`, 'El preview UI DOC-33');
  expect(afterState).toBe(beforeState);
  expect(afterAudit).toBe(beforeAudit);
  await writeEvidence('preview', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'PreviewDevolverActividad',
    cantidad: destinationCount,
    latenciaMs: elapsedMs,
    estadoSinCambio: true,
    auditoriaSinCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc33-ui-execute La devolución se confirma desde el preview UI vigente', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_EXECUTION_AUTHORIZED`, `${e2ePrefix}_UI_EXECUTION_TASK_ID`, `${e2ePrefix}_UI_EXECUTION_ACTIVITY_NAME`, `${e2ePrefix}_UI_EXECUTION_FINAL_ACTIVITY_NAME`, `${e2ePrefix}_UI_EXECUTION_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true' || required(`${e2ePrefix}_EXECUTION_AUTHORIZED`).toLowerCase() !== 'true') {
    throw new Error('La ejecución UI DOC-33 requiere autorizaciones explícitas.');
  }
  const taskId = positiveInteger(`${e2ePrefix}_UI_EXECUTION_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  let context;
  let page;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let executionResult = { httpOk: false, exito: false, codigo: 'NOT_STARTED', estadoFinal: null };
  let elapsedMs = 0;
  let destinationCount = 0;
  let finalActivityMatched = false;
  let observedFinalActivity = null;
  const executionEndpoint = endpoint('EjecutarDevolverActividad');
  try {
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    context = await login(browser);
    page = await context.newPage();
    await page.route(executionEndpoint, async (route) => {
      const response = await route.fetch({ timeout: 60000 });
      const body = await response.body();
      executionResult = summarizeExecutionResponse(response, body);
      await route.fulfill({ response, body });
    });
    ({ count: destinationCount } = await openPreview(page, taskId));
    await selectDestination(page, required(`${e2ePrefix}_UI_EXECUTION_ACTIVITY_NAME`));
    const dialog = page.locator('.confirmation-dialog');
    await expect(dialog).toBeVisible();
    const started = performance.now();
    await dialog.locator('.confirmation-dialog__primary').click();
    await expect(dialog).toBeHidden({ timeout: 30000 });
    elapsedMs = Math.round(performance.now() - started);
    await expect(page.locator('#workflow-return-activity-success-message')).toBeVisible();
  } finally {
    await page?.unroute(executionEndpoint);
    afterState = await queryFingerprint(stateSql, taskId);
    afterAudit = await queryFingerprint(auditSql, taskId);
    await context?.close();
  }
  assertLatency(elapsedMs, `${e2ePrefix}_UI_EXECUTION_MAX_MS`, 'La devolución UI DOC-33');
  expect(executionResult.httpOk).toBeTruthy();
  expect(executionResult.exito, `La devolución UI fue rechazada con ${executionResult.codigo || 'un código no informado'}.`).toBeTruthy();
  expect(executionResult.estadoFinal).toBe('completada');
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  finalActivityMatched = await finalActivityMatches(taskId, required(`${e2ePrefix}_UI_EXECUTION_FINAL_ACTIVITY_NAME`));
  if (!finalActivityMatched) observedFinalActivity = await activeActivityName(taskId);
  expect(finalActivityMatched, `La actividad final no coincide con la actividad final esperada. Actividad activa observada: ${observedFinalActivity || 'ambigua'}.`).toBeTruthy();
  await writeEvidence('execution', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverActividad',
    cantidad: destinationCount,
    exito: executionResult.exito,
    estadoFinal: executionResult.estadoFinal,
    codigo: executionResult.codigo,
    latenciaMs: elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    coincidenciaFinal: finalActivityMatched,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc33-ui-lock Una respuesta pendiente bloquea confirmación, modal y abandono', async ({ browser }) => {
  requireNames([...protectedNames(), `${e2ePrefix}_UI_LOCK_AUTHORIZED`, `${e2ePrefix}_UI_LOCK_TASK_ID`, `${e2ePrefix}_UI_LOCK_ACTIVITY_NAME`, `${e2ePrefix}_UI_LOCK_FINAL_ACTIVITY_NAME`, `${e2ePrefix}_UI_LOCK_MAX_MS`]);
  if (required(`${e2ePrefix}_ENVIRONMENT_AUTHORIZED`).toLowerCase() !== 'true' || required(`${e2ePrefix}_UI_LOCK_AUTHORIZED`).toLowerCase() !== 'true') {
    throw new Error('El bloqueo UI DOC-33 requiere autorizaciones explícitas.');
  }
  const taskId = positiveInteger(`${e2ePrefix}_UI_LOCK_TASK_ID`);
  const stateSql = required(`${e2ePrefix}_TASK_STATE_SQL`);
  const auditSql = required(`${e2ePrefix}_AUDIT_SQL`);
  assertReadOnlySql(stateSql, `${e2ePrefix}_TASK_STATE_SQL`);
  assertReadOnlySql(auditSql, `${e2ePrefix}_AUDIT_SQL`);
  const executionEndpoint = endpoint('EjecutarDevolverActividad');
  let context;
  let page;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let releaseResponse;
  let signalResponseReady;
  let routeStarted = false;
  let resolveResponseFulfilled;
  let rejectResponseFulfilled;
  let executionRequests = 0;
  let executionResult = { httpOk: false, exito: false, codigo: 'NOT_STARTED', estadoFinal: null };
  let confirmationLocked = false;
  let returnModalLocked = false;
  let unloadGuarded = false;
  let dialogClosedAfterResponse = false;
  let elapsedMs = 0;
  let finalActivityMatched = false;
  let observedFinalActivity = null;
  const responseReady = new Promise((resolve) => { signalResponseReady = resolve; });
  const responseRelease = new Promise((resolve) => { releaseResponse = resolve; });
  const responseFulfilled = new Promise((resolve, reject) => {
    resolveResponseFulfilled = resolve;
    rejectResponseFulfilled = reject;
  });
  try {
    beforeState = await queryFingerprint(stateSql, taskId);
    beforeAudit = await queryFingerprint(auditSql, taskId);
    context = await login(browser);
    page = await context.newPage();
    await page.route(executionEndpoint, async (route) => {
      routeStarted = true;
      try {
        executionRequests += 1;
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
    await openPreview(page, taskId);
    await selectDestination(page, required(`${e2ePrefix}_UI_LOCK_ACTIVITY_NAME`));
    const dialog = page.locator('.confirmation-dialog');
    const modal = page.locator('#workflow-return-activity-modern-modal');
    const primary = dialog.locator('.confirmation-dialog__primary');
    const cancel = dialog.locator('.confirmation-dialog__cancel');
    const close = dialog.locator('.confirmation-dialog__close');
    const backdrop = dialog.locator('.confirmation-dialog__backdrop');
    await expect(dialog).toBeVisible();
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
    await page.locator('#workflow-return-activity-modern-modal [data-workflow-return-activity-close]').click({ force: true });
    await expect(modal).toBeVisible();
    unloadGuarded = await page.evaluate(() => {
      const event = new Event('beforeunload', { cancelable: true });
      window.dispatchEvent(event);
      return event.defaultPrevented;
    });
    await expect(dialog).toHaveAttribute('data-confirmation-dialog-state', 'enviando');
    await expect(dialog.locator('.confirmation-dialog__status')).toContainText('Espere la respuesta antes de cerrar');
    expect(confirmationApiCloseAccepted).toBeFalsy();
    expect(unloadGuarded).toBeTruthy();
    confirmationLocked = true;
    returnModalLocked = true;
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
  assertLatency(elapsedMs, `${e2ePrefix}_UI_LOCK_MAX_MS`, 'El bloqueo UI DOC-33');
  expect(executionRequests).toBe(1);
  expect(confirmationLocked).toBeTruthy();
  expect(returnModalLocked).toBeTruthy();
  expect(unloadGuarded).toBeTruthy();
  expect(executionResult.httpOk).toBeTruthy();
  expect(executionResult.exito, `La devolución UI fue rechazada con ${executionResult.codigo || 'un código no informado'}.`).toBeTruthy();
  expect(executionResult.estadoFinal).toBe('completada');
  expect(dialogClosedAfterResponse).toBeTruthy();
  expect(afterState).not.toBe(beforeState);
  expect(afterAudit).not.toBe(beforeAudit);
  finalActivityMatched = await finalActivityMatches(taskId, required(`${e2ePrefix}_UI_LOCK_FINAL_ACTIVITY_NAME`));
  if (!finalActivityMatched) observedFinalActivity = await activeActivityName(taskId);
  expect(finalActivityMatched, `La actividad final no coincide con la actividad final esperada. Actividad activa observada: ${observedFinalActivity || 'ambigua'}.`).toBeTruthy();
  await writeEvidence('ui-lock', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarDevolverActividad',
    solicitudesMutantes: executionRequests,
    confirmacionBloqueada: confirmationLocked,
    modalBloqueado: returnModalLocked,
    abandonoBloqueado: unloadGuarded,
    cierreTrasRespuesta: dialogClosedAfterResponse,
    exito: executionResult.exito,
    estadoFinal: executionResult.estadoFinal,
    codigo: executionResult.codigo,
    latenciaMs: elapsedMs,
    estadoCambio: true,
    auditoriaCambio: true,
    coincidenciaFinal: finalActivityMatched,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

'use strict';

const fs = require('node:fs');
const fsp = require('node:fs/promises');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { chromium } = require('@playwright/test');
const {
  collectConfirmation, collectValue, requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { queryFingerprint } = require('./support/doc32-e2e-odbc.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const {
  selectWorkflowTask, waitForViewerFrame
} = require('./run-workflow-pdf-viewer-performance-interactive.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const profileFields = new Set([
  'baseUrl', 'module', 'taskId', 'documentOrdinal', 'documentTitle', 'timeoutMilliseconds',
  'headless', 'browser', 'ignoreHttpsErrors', 'odbcDsn', 'documentStateSql'
]);
const forbiddenEvidence = /https?:|[A-Za-z]:\\|(?:cookie|token|password|contrase(?:ñ|n)a|base64|usuario|taskId|documentTitle|documentOrdinal|baseUrl|url|path|query)/i;

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function positiveInteger(value, code, maximum = Number.MAX_SAFE_INTEGER) {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number < 1 || number > maximum) fail(code);
  return number;
}

function validateSelect(sql, code) {
  if (typeof sql !== 'string' || !/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) fail(code);
  return sql;
}

function loadProfile(target) {
  let input;
  try { input = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, '')); } catch { fail('DOC103_PROFILE_INVALID'); }
  if (!input || typeof input !== 'object' || Array.isArray(input) || Object.keys(input).some(key => !profileFields.has(key))) {
    fail('DOC103_PROFILE_INVALID');
  }
  let baseUrl;
  try { baseUrl = new URL(input.baseUrl); } catch { fail('DOC103_BASE_URL_INVALID'); }
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('DOC103_BASE_URL_INVALID');
  if (typeof input.module !== 'string' || !/^[A-Za-z0-9][A-Za-z0-9 _-]{1,79}$/.test(input.module)) fail('DOC103_MODULE_INVALID');
  if (typeof input.odbcDsn !== 'string' || !/^[A-Za-z0-9 _.-]+$/.test(input.odbcDsn)) fail('DOC103_ODBC_DSN_INVALID');
  if (input.ignoreHttpsErrors !== true && input.ignoreHttpsErrors !== false) fail('DOC103_TLS_FLAG_INVALID');
  let browser;
  if (input.browser !== undefined) {
    if (!input.browser || typeof input.browser !== 'object' || Array.isArray(input.browser) ||
        Object.keys(input.browser).length !== 1 || !['chrome', 'msedge'].includes(input.browser.channel)) fail('DOC103_BROWSER_INVALID');
    browser = Object.freeze({ channel: input.browser.channel });
  }
  return Object.freeze({
    baseUrl: baseUrl.toString(), module: input.module.trim(),
    taskId: positiveInteger(input.taskId, 'DOC103_TASK_INVALID'),
    documentOrdinal: positiveInteger(input.documentOrdinal ?? 1, 'DOC103_DOCUMENT_INVALID', 1000),
    documentTitle: input.documentTitle === undefined ? undefined : String(input.documentTitle).trim(),
    timeoutMilliseconds: positiveInteger(input.timeoutMilliseconds ?? 120000, 'DOC103_TIMEOUT_INVALID', 600000),
    headless: input.headless === true, browser, ignoreHttpsErrors: input.ignoreHttpsErrors,
    odbcDsn: input.odbcDsn,
    documentStateSql: validateSelect(input.documentStateSql, 'DOC103_DOCUMENT_SELECT_INVALID')
  });
}

function sanitizeEvidence(input) {
  if (forbiddenEvidence.test(JSON.stringify(input))) fail('DOC103_EVIDENCE_INVALID');
  const evidence = Object.freeze({
    mode: input.mode === 'real' ? 'real' : 'double',
    result: input.result === 'PASSED' ? 'PASSED' : 'FAILED',
    scenarios: Number(input.scenarios) || 0,
    postCount: Number(input.postCount) || 0,
    normalizedResult: String(input.normalizedResult || 'not_run'),
    overlayRemoved: input.overlayRemoved === true,
    activePagePreserved: input.activePagePreserved === true,
    zoomPreserved: input.zoomPreserved === true,
    scrollPreserved: input.scrollPreserved === true,
    singleEffect: input.singleEffect === true,
    selectControlsCompleted: input.selectControlsCompleted === true
  });
  if (forbiddenEvidence.test(JSON.stringify(evidence))) fail('DOC103_EVIDENCE_INVALID');
  return evidence;
}

async function writeEvidence(evidence) {
  const target = path.join(e2eRoot, 'artifacts', 'doc103-graph-save.json');
  await fsp.mkdir(path.dirname(target), { recursive: true });
  await fsp.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function writeFailure(code) {
  const target = path.join(e2eRoot, 'artifacts', 'doc103-graph-save-failure.json');
  await fsp.mkdir(path.dirname(target), { recursive: true });
  await fsp.writeFile(target, `${JSON.stringify({ result: 'BLOCKED', code }, null, 2)}\n`, 'utf8');
}

async function runDouble() {
  const files = [
    'tests/workflow-pdf-viewer-graph-save-policy.test.cjs',
    'tests/workflow-pdf-viewer-graph-save.test.cjs'
  ];
  const run = spawnSync(process.execPath, ['--test', ...files], { cwd: e2eRoot, stdio: 'inherit', shell: false });
  const passed = run.status === 0;
  const evidence = sanitizeEvidence({
    mode: 'double', result: passed ? 'PASSED' : 'FAILED', scenarios: 11,
    postCount: passed ? 1 : 0, normalizedResult: passed ? 'matrix_passed' : 'not_run',
    overlayRemoved: passed, activePagePreserved: passed, zoomPreserved: passed,
    scrollPreserved: passed, singleEffect: passed, selectControlsCompleted: false
  });
  await writeEvidence(evidence);
  if (!passed) fail('DOC103_DOUBLE_FAILED');
  return evidence;
}

async function selectDocument(page, profile) {
  const rows = page.locator('#GridView_list_documento_relacion_wf tr[id_wf]:visible');
  const documents = page.locator('#GridView_list_documento_relacion_wf [tip_event="vis_doc_selecion_wf"]:visible');
  await documents.first().waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds }).catch(() => fail('DOC103_DOCUMENTS_UNAVAILABLE'));
  if (!profile.documentTitle) {
    if (await documents.count() < profile.documentOrdinal) fail('DOC103_DOCUMENT_UNAVAILABLE');
    await documents.nth(profile.documentOrdinal - 1).click();
    return;
  }
  const expected = profile.documentTitle.toLocaleLowerCase('es');
  const indexes = await rows.evaluateAll((items, title) => items.map((row, index) => ({ index, text: String(row.textContent || '').toLocaleLowerCase('es') }))
    .filter(item => item.text.includes(title)).map(item => item.index), expected);
  if (indexes.length !== 1) fail('DOC103_DOCUMENT_TITLE_NOT_UNIQUE');
  await rows.nth(indexes[0]).locator('[tip_event="vis_doc_selecion_wf"]:visible').first().click();
}

async function controlFingerprint(sql, taskId, environment, dsn, code) {
  try {
    return await queryFingerprint(sql, taskId, { ...environment, DOC103_E2E_ODBC_DSN: dsn }, 'DOC103_E2E');
  } catch {
    fail(code);
  }
}

async function runReal(profile, environment) {
  const beforeDocument = await controlFingerprint(profile.documentStateSql, profile.taskId, environment, profile.odbcDsn, 'DOC103_DOCUMENT_SELECT_PREFLIGHT_FAILED');
  let browser;
  try { browser = await chromium.launch({ ...(profile.browser || {}), headless: profile.headless }); }
  catch { fail('DOC103_BROWSER_LAUNCH_FAILED'); }
  let context;
  try {
    try {
      context = await createAuthenticatedWorkflowSession(browser, {
        baseUrl: profile.baseUrl, environment,
        moduleEnvironmentVariable: 'DOC103_E2E_MODULE',
        userEnvironmentVariable: 'DOC103_E2E_AUTHORIZED_USER',
        passwordEnvironmentVariable: 'DOC103_E2E_AUTHORIZED_PASSWORD',
        timeoutMilliseconds: profile.timeoutMilliseconds, ignoreHTTPSErrors: profile.ignoreHttpsErrors
      });
    } catch { fail('DOC103_AUTHENTICATION_FAILED'); }
    const page = await context.newPage();
    let postCount = 0;
    page.on('request', request => {
      try {
        if (request.method() === 'POST' && /\/webservice\/WebService_itext\.asmx\/service_anotate_pdf_image$/i.test(new URL(request.url()).pathname)) postCount += 1;
      } catch { /* No se conserva la URL. */ }
    });
    page.on('dialog', dialog => dialog.accept());
    await selectWorkflowTask(page, profile);
    await selectDocument(page, profile);
    const frame = await waitForViewerFrame(page, profile.timeoutMilliseconds);
    await frame.locator('#pdf-container_ canvas').first().waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
    const before = await frame.evaluate(async () => {
      await window.loadViewerCapability('graph');
      await ensureStampConfiguration(false);
      const container = document.querySelector('#pdf-container_');
      const existing = new Set(Array.from(document.querySelectorAll('div.ui-widget-content[id^="div-drav-image-"]')).map(item => item.id));
      pdfViewer._Add_interface_anotate(1, 0, 0);
      const overlay = Array.from(document.querySelectorAll('div.ui-widget-content[id^="div-drav-image-"]')).find(item => !existing.has(item.id));
      if (!overlay) throw new Error('DOC103_OVERLAY_UNAVAILABLE');
      return { overlayId: overlay.id, page: pdfViewer.getActivePageNumber(), zoom: pdfViewer.getZoom(), left: container.scrollLeft, top: container.scrollTop };
    }).catch(error => fail(/^[A-Z0-9_]+$/.test(error?.message || '') ? error.message : 'DOC103_OVERLAY_UNAVAILABLE'));
    await frame.locator(`#${before.overlayId} [event_drag="save"]`).click();
    await frame.locator(`#${before.overlayId}`).waitFor({ state: 'detached', timeout: profile.timeoutMilliseconds }).catch(() => fail('DOC103_SAVE_NOT_CONFIRMED'));
    const after = await frame.evaluate(() => {
      const container = document.querySelector('#pdf-container_');
      return { page: pdfViewer.getActivePageNumber(), zoom: pdfViewer.getZoom(), left: container.scrollLeft, top: container.scrollTop };
    });
    const afterDocument = await controlFingerprint(profile.documentStateSql, profile.taskId, environment, profile.odbcDsn, 'DOC103_DOCUMENT_SELECT_POSTCHECK_FAILED');
    const evidence = sanitizeEvidence({
      mode: 'real', result: postCount === 1 ? 'PASSED' : 'FAILED', scenarios: 1, postCount,
      normalizedResult: 'committed', overlayRemoved: true,
      activePagePreserved: before.page === after.page, zoomPreserved: before.zoom === after.zoom,
      scrollPreserved: before.left === after.left && before.top === after.top,
      singleEffect: postCount === 1,
      selectControlsCompleted: true
    });
    await writeEvidence(evidence);
    if (beforeDocument !== afterDocument || evidence.result !== 'PASSED' || !evidence.singleEffect ||
        !evidence.activePagePreserved || !evidence.zoomPreserved || !evidence.scrollPreserved) fail('DOC103_REAL_VALIDATION_FAILED');
    return evidence;
  } finally {
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

function argument(name) {
  const index = process.argv.indexOf(name);
  return index >= 0 ? process.argv[index + 1] : undefined;
}

async function main() {
  const mode = argument('--mode') || 'double';
  if (mode === 'double') {
    const evidence = await runDouble();
    console.log(`DOC103_GRAPH_SAVE_RESULT=${evidence.result}`);
    return;
  }
  if (mode !== 'real') fail('DOC103_MODE_INVALID');
  requireInteractiveConsole();
  const profileTarget = argument('--profile');
  if (!profileTarget) fail('DOC103_PROFILE_REQUIRED');
  const profile = loadProfile(path.resolve(profileTarget));
  const values = { DOC103_E2E_MODULE: profile.module, DOC103_E2E_ODBC_DSN: profile.odbcDsn };
  await collectConfirmation(values, 'DOC103_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza el ambiente de prueba para guardar un grafo real?');
  await collectConfirmation(values, 'DOC103_E2E_ACCOUNT_AUTHORIZED', '¿Autoriza la cuenta para esta prueba mutadora?');
  await collectConfirmation(values, 'DOC103_E2E_DISPOSABLE_PDF_AUTHORIZED', '¿Confirma que el PDF seleccionado es descartable y puede modificarse?');
  await collectValue(values, 'DOC103_E2E_AUTHORIZED_USER', 'Cuenta autorizada de Workflow');
  await collectValue(values, 'DOC103_E2E_AUTHORIZED_PASSWORD', 'Contraseña de Workflow', { secret: true });
  await collectValue(values, 'DOC103_E2E_MYSQL_USER', 'Usuario ODBC de solo lectura');
  await collectValue(values, 'DOC103_E2E_MYSQL_PASSWORD', 'Contraseña ODBC de solo lectura', { secret: true });
  const evidence = await runReal(profile, { ...process.env, ...values });
  console.log(`DOC103_GRAPH_SAVE_POST_COUNT=${evidence.postCount}`);
  console.log(`DOC103_GRAPH_SAVE_RESULT=${evidence.result}`);
}

if (require.main === module) {
  main().catch(async error => {
    const code = /^[A-Z0-9_]{3,120}$/.test(error?.code || error?.message || '') ? (error.code || error.message) : 'DOC103_GRAPH_SAVE_FAILED';
    console.error(`La E2E DOC-103 se detuvo de forma segura (${code}). No se mostraron rutas, credenciales ni documentos.`);
    await writeFailure(code).catch(() => {});
    process.exitCode = 2;
  });
}

module.exports = { loadProfile, runDouble, sanitizeEvidence, validateSelect };

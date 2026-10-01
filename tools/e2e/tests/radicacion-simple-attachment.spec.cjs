'use strict';

const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const mysql = require('mysql2/promise');
const path = require('node:path');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');
const protectedPaths = [
  'generic_control/FileUploadHandler_.ashx.vb',
  'generic_control/FileUploadHandler.js',
  'js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js'
];

const launchOptions = {};
if (process.env.DOC85_E2E_BROWSER_PATH?.trim()) launchOptions.executablePath = process.env.DOC85_E2E_BROWSER_PATH.trim();
else if (process.env.DOC85_E2E_BROWSER_CHANNEL?.trim()) launchOptions.channel = process.env.DOC85_E2E_BROWSER_CHANNEL.trim();
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });
test.setTimeout(Number(process.env.DOC85_E2E_TIMEOUT_MS || 120000) * 3);

function required(name) {
  const value = process.env[name];
  if (typeof value !== 'string' || value.trim().length === 0) throw new Error(`Falta ${name}.`);
  return value.trim();
}

function baseUrl() {
  const value = required('DOC85_E2E_BASE_URL');
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function hash(value) {
  return crypto.createHash('sha256').update(value).digest('hex');
}

async function protectedHashes() {
  const entries = await Promise.all(protectedPaths.map(async relativePath => [
    relativePath,
    hash(await fs.readFile(path.join(repositoryRoot, relativePath)))
  ]));
  return Object.fromEntries(entries);
}

async function assertGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  const value = name => configuration.match(new RegExp(`<add\\s+key=["']${name}["']\\s+value=["']([^"']*)["']`, 'i'))?.[1];
  const active = value('WorkflowCentroTrabajoModernActive');
  const users = value('WorkflowCentroTrabajoModernUsers');
  const groups = value('WorkflowCentroTrabajoModernGroups');
  expect(active === undefined || active.toLowerCase() === 'false').toBeTruthy();
  expect(users === undefined || users.trim() === '').toBeTruthy();
  expect(groups === undefined || groups.trim() === '').toBeTruthy();
}

function mysqlOptions() {
  return {
    host: required('DOC85_E2E_MYSQL_HOST'),
    port: Number(required('DOC85_E2E_MYSQL_PORT')),
    database: required('DOC85_E2E_MYSQL_DATABASE'),
    user: required('DOC85_E2E_MYSQL_USER'),
    password: required('DOC85_E2E_MYSQL_PASSWORD'),
    multipleStatements: false
  };
}

async function queryRows(connection, sqlName, parameter) {
  const [rows] = await connection.execute(required(sqlName), [parameter]);
  return rows;
}

function documentTotal(rows) {
  const value = rows?.[0]?.total;
  const total = Number(value);
  if (!Number.isSafeInteger(total) || total < 0) throw new Error('DOC85_E2E_DOCUMENT_COUNT_SQL debe retornar total entero no negativo.');
  return total;
}

async function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC85_E2E_MODULE',
    userEnvironmentVariable: 'DOC85_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC85_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC85_E2E_IGNORE_HTTPS_ERRORS === 'true',
    timeoutMilliseconds: Number(process.env.DOC85_E2E_TIMEOUT_MS || 120000)
  });
}

async function selectDisposableRecord(page, expectedRadicado) {
  await page.waitForLoadState('load');
  await page.waitForFunction(radicado => {
    const activeText = document.querySelector('#Label_estado_transac')?.textContent?.trim() || '';
    const pendingCount = document.querySelector('#Label_numero_item')?.textContent?.trim() || '';
    const progress = document.querySelector('#progres_bar');
    const initializationFinished = !progress || window.getComputedStyle(progress).display === 'none';
    return initializationFinished && (activeText.includes(radicado) || pendingCount.length > 0);
  }, expectedRadicado, { timeout: 30000 });

  const activeLabel = page.locator('#Label_estado_transac');
  const activeText = String(await activeLabel.textContent() || '').trim();
  if (activeText.includes(expectedRadicado)) {
    await expect(page.locator('#table_doc_flow_select')).toBeVisible();
    return;
  }
  if (activeText.length > 0) {
    throw new Error('La sesión ya tiene asignado un radicado distinto del recurso descartable autorizado.');
  }

  await page.locator('#boton_rad_list_task').click();
  await expect(page.locator('#modal_content_lista_radicados_pendientes')).toBeVisible();
  const row = page.locator('#table_list_radicados_table tbody tr', { hasText: expectedRadicado })
    .filter({ has: page.locator('.asing_task_radic') });
  await expect(row).toHaveCount(1);
  await row.locator('.asing_task_radic').click();
  await expect(page.locator('#Label_estado_transac')).toContainText(expectedRadicado);
  await expect(page.locator('#modal_content_lista_radicados_pendientes')).toBeHidden();
}

async function prepareUpload(page, fixturePath, typologyText) {
  await page.locator('#delete_wodloa_file_rad').click();
  const modal = page.locator('#modal_adjunta_documeto_load_documento_006');
  await expect(modal).toBeVisible();
  const input = page.locator('#file_element_adjunta_documeto_load_documento_006');
  await input.setInputFiles(fixturePath);
  const pendingRow = page.locator('#table_file_element_adjunta_documeto_load_documento_006 tr[NameFile]');
  await expect(pendingRow).toHaveCount(1);
  const typology = pendingRow.locator('select[id^="element_input_"]');
  if (await typology.count()) await typology.selectOption({ label: typologyText });
  return pendingRow.locator('a[title="Guardar archivo"]');
}

async function uploadResponse(page, save) {
  const responsePromise = page.waitForResponse(response =>
    response.request().method() === 'POST' && /generic_control\/fileuploadhandler_\.ashx(?:\?|$)/i.test(response.url())
  );
  await save.click();
  const response = await responsePromise;
  let payload;
  try { payload = await response.json(); } catch { throw new Error('El handler DOC-85 no retornó el contrato JSON esperado.'); }
  return { response, payload };
}

async function bootstrapRowById(page, tableSelector, id) {
  await expect.poll(async () => page.evaluate(({ selector, expectedId }) => {
    const table = document.querySelector(selector);
    if (!table || !window.jQuery) return -1;
    const rows = window.jQuery(table).bootstrapTable('getData');
    return rows.findIndex(row => Number(row.ID) === expectedId);
  }, { selector: tableSelector, expectedId: id })).toBeGreaterThanOrEqual(0);

  const resolvedIndex = await page.evaluate(({ selector, expectedId }) => {
    const rows = window.jQuery(document.querySelector(selector)).bootstrapTable('getData');
    return rows.findIndex(row => Number(row.ID) === expectedId);
  }, { selector: tableSelector, expectedId: id });
  return page.locator(`${tableSelector} tbody tr[data-index="${resolvedIndex}"]`);
}

async function installRadicadoMismatch(page, expectedRadicado) {
  let altered = false;
  await page.route(/generic_control\/fileuploadhandler_\.ashx(?:\?|$)/i, async route => {
    const request = route.request();
    const body = request.postDataBuffer();
    const marker = Buffer.from(`name="radicado_radicacion"\r\n\r\n${expectedRadicado}`, 'utf8');
    const index = body ? body.indexOf(marker) : -1;
    if (index < 0) throw new Error('No se encontró el campo informativo radicado_radicacion en el multipart real.');
    const replacement = Buffer.from(`name="radicado_radicacion"\r\n\r\nDOC85-RADICADO-INCONSISTENTE`, 'utf8');
    const modified = Buffer.concat([body.subarray(0, index), replacement, body.subarray(index + marker.length)]);
    const headers = { ...request.headers() };
    delete headers['content-length'];
    altered = true;
    await route.continue({ postData: modified, headers });
  }, { times: 1 });
  return () => altered;
}

async function writeEvidence(evidence) {
  const target = path.join(e2eRoot, 'artifacts', 'doc85-radicacion-simple-attachment-e2e.json');
  await fs.mkdir(path.dirname(target), { recursive: true });
  await fs.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

test.beforeAll(assertGateOff);
test.afterAll(assertGateOff);

test('DOC-85 adjunta con contexto autoritativo y rechaza el radicado informativo inconsistente', async ({ browser }) => {
  for (const authorization of [
    'DOC85_E2E_ENVIRONMENT_AUTHORIZED', 'DOC85_E2E_ACCOUNT_AUTHORIZED',
    'DOC85_E2E_EXECUTION_AUTHORIZED', 'DOC85_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
  ]) expect(required(authorization).toLowerCase()).toBe('true');

  const stateId = Number(required('DOC85_E2E_STATE_RECORD_ID'));
  const expectedRadicado = required('DOC85_E2E_EXPECTED_RADICADO');
  const typologyText = required('DOC85_E2E_TYPOLOGY_TEXT');
  const positiveFixture = path.resolve(e2eRoot, required('DOC85_E2E_POSITIVE_FIXTURE_PATH'));
  const negativeFixture = path.resolve(e2eRoot, required('DOC85_E2E_NEGATIVE_FIXTURE_PATH'));
  const hashesBefore = await protectedHashes();
  let connection;
  let context;
  let page;
  let taskId;
  let beforeTotal;
  let positiveTotal;
  let negativeTotal;
  let navigationCount = 0;
  let postbackCount = 0;
  let operationStarted = false;
  let positiveContract = false;
  let positiveInterface = false;
  let mismatchAltered = false;
  let mismatchRejected = false;

  try {
    connection = await mysql.createConnection(mysqlOptions());
    const contextRows = await queryRows(connection, 'DOC85_E2E_CONTEXT_SQL', stateId);
    expect(contextRows).toHaveLength(1);
    expect(String(contextRows[0].radicado || '').trim()).toBe(expectedRadicado);
    taskId = Number(contextRows[0].tarea);
    expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();
    expect(Number(contextRows[0].plantilla) > 0 && Number(contextRows[0].tramite) > 0).toBeTruthy();

    const datAdicRows = await queryRows(connection, 'DOC85_E2E_DAT_ADIC_SQL', taskId);
    expect(datAdicRows).toHaveLength(1);
    expect(datAdicRows.every(row => String(row.radicado ?? '').trim() === '')).toBeTruthy();
    beforeTotal = documentTotal(await queryRows(connection, 'DOC85_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));

    context = await login(browser);
    page = await context.newPage();
    page.on('request', request => {
      const isMainDocumentNavigation = operationStarted
        && request.isNavigationRequest()
        && request.resourceType() === 'document'
        && request.frame() === page.mainFrame();
      if (isMainDocumentNavigation) navigationCount += 1;
      if (isMainDocumentNavigation && request.method() === 'POST') postbackCount += 1;
    });
    await page.goto(new URL(required('DOC85_E2E_PAGE_PATH'), baseUrl()).toString(), { waitUntil: 'domcontentloaded' });
    await selectDisposableRecord(page, expectedRadicado);

    const rowsBefore = await page.locator('#table_doc_flow_select tbody tr').count();
    const positiveSave = await prepareUpload(page, positiveFixture, typologyText);
    operationStarted = true;
    const positive = await uploadResponse(page, positiveSave);
    expect(positive.response.ok()).toBeTruthy();
    expect(Array.isArray(positive.payload) && positive.payload.length === 1).toBeTruthy();
    const uploaded = positive.payload[0];
    expect(uploaded.error_sistema).toBe('YES');
    expect(String(uploaded.radicado || '').trim()).toBe(expectedRadicado);
    expect(String(uploaded.notitipodocumental || '').trim()).toBe(typologyText);
    expect(Number(uploaded.id_image) > 0).toBeTruthy();
    expect(String(uploaded.icono_icono_awe_some || '').trim().length > 0).toBeTruthy();
    positiveContract = true;

    const insertedRow = await bootstrapRowById(page, '#table_doc_flow_select', Number(uploaded.id_image));
    await expect(insertedRow).toHaveCount(1);
    await expect(insertedRow).toContainText(typologyText);
    for (const action of ['vis_doc_selecion_rad', 'delete_file_document', 'change_tipo_document', 'stamp_file_document', 'list_version_document', 'replace_version_document']) {
      expect(await insertedRow.locator(`.${action}`).count()).toBeGreaterThan(0);
    }
    expect(await page.locator('#table_doc_flow_select tbody tr').count()).toBe(rowsBefore + 1);
    positiveInterface = true;
    positiveTotal = documentTotal(await queryRows(connection, 'DOC85_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));
    expect(positiveTotal).toBe(beforeTotal + 1);

    const negativeSave = await prepareUpload(page, negativeFixture, typologyText);
    const wasAltered = await installRadicadoMismatch(page, expectedRadicado);
    const negativeRowsBefore = await page.locator('#table_doc_flow_select tbody tr').count();
    const negative = await uploadResponse(page, negativeSave);
    mismatchAltered = wasAltered();
    expect(mismatchAltered).toBeTruthy();
    expect(Array.isArray(negative.payload) && negative.payload.length === 1).toBeTruthy();
    expect(negative.payload[0].error_sistema).not.toBe('YES');
    await expect(page.locator('#table_doc_flow_select tbody tr')).toHaveCount(negativeRowsBefore);
    negativeTotal = documentTotal(await queryRows(connection, 'DOC85_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));
    expect(negativeTotal).toBe(positiveTotal);
    mismatchRejected = true;
  } finally {
    await page?.unroute(/generic_control\/fileuploadhandler_\.ashx(?:\?|$)/i).catch(() => {});
    await context?.close();
    await connection?.end();
    await assertGateOff();
  }

  const hashesAfter = await protectedHashes();
  expect(hashesAfter).toEqual(hashesBefore);
  expect(navigationCount).toBe(0);
  expect(postbackCount).toBe(0);
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    recurso: hash(`${stateId}:${expectedRadicado}`).slice(0, 16),
    precondicionDatAdicSinRadicado: true,
    contratoUploadFilesConservado: positiveContract,
    insercionJavaScriptSinRecarga: positiveInterface && navigationCount === 0 && postbackCount === 0,
    persistencia: { antes: beforeTotal, despuesPositivo: positiveTotal, despuesNegativo: negativeTotal },
    discrepanciaInformativaAlterada: mismatchAltered,
    discrepanciaInformativaRechazada: mismatchRejected,
    archivosCompartidosIntactos: JSON.stringify(hashesAfter) === JSON.stringify(hashesBefore),
    gateIntacto: true
  });
});

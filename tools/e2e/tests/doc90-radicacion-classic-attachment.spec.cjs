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
  'workflow/ClassAlmacenamiento.vb',
  'js/radicacion/WebFormRadicacionEntrante.js',
  'js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js'
];

const launchOptions = {};
if (process.env.DOC90_E2E_BROWSER_PATH?.trim()) launchOptions.executablePath = process.env.DOC90_E2E_BROWSER_PATH.trim();
else if (process.env.DOC90_E2E_BROWSER_CHANNEL?.trim()) launchOptions.channel = process.env.DOC90_E2E_BROWSER_CHANNEL.trim();
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });
test.setTimeout(Number(process.env.DOC90_E2E_TIMEOUT_MS || 120000) * 2);

function required(name) {
  const value = process.env[name];
  if (typeof value !== 'string' || value.trim().length === 0) throw new Error(`Falta ${name}.`);
  return value.trim();
}

function baseUrl() {
  const value = required('DOC90_E2E_BASE_URL');
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
  expect(value('WorkflowCentroTrabajoModernActive')?.toLowerCase() === 'true').toBeFalsy();
  expect((value('WorkflowCentroTrabajoModernUsers') || '').trim()).toBe('');
  expect((value('WorkflowCentroTrabajoModernGroups') || '').trim()).toBe('');
}

function mysqlOptions() {
  return {
    host: required('DOC90_E2E_MYSQL_HOST'),
    port: Number(required('DOC90_E2E_MYSQL_PORT')),
    database: required('DOC90_E2E_MYSQL_DATABASE'),
    user: required('DOC90_E2E_MYSQL_USER'),
    password: required('DOC90_E2E_MYSQL_PASSWORD'),
    multipleStatements: false
  };
}

async function queryRows(connection, sqlName, parameter) {
  const [rows] = await connection.execute(required(sqlName), [parameter]);
  return rows;
}

function documentTotal(rows) {
  const total = Number(rows?.[0]?.total);
  if (!Number.isSafeInteger(total) || total < 0) throw new Error('DOC90_E2E_DOCUMENT_COUNT_SQL debe retornar total entero no negativo.');
  return total;
}

async function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC90_E2E_MODULE',
    userEnvironmentVariable: 'DOC90_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC90_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC90_E2E_IGNORE_HTTPS_ERRORS === 'true',
    timeoutMilliseconds: Number(process.env.DOC90_E2E_TIMEOUT_MS || 120000)
  });
}

async function selectIncomingTemplate(page, templateId, templateName) {
  await page.goto(new URL('Defaul/WebFormInicioDocuarchiGestion.aspx', baseUrl()).toString(), {
    waitUntil: 'commit',
    timeout: 30000
  });
  await page.locator('#CR-PR-00').waitFor({ state: 'attached', timeout: 30000 });

  const resolveMenuItemId = async () => page.evaluate(({ expectedId, expectedName }) => {
    const items = Array.isArray(window.ITEMS_DATOS_TOKENIZE_2) ? window.ITEMS_DATOS_TOKENIZE_2 : [];
    const matches = items.filter(item =>
      item.nodo_plantilla_radicado === 'yes' &&
      Number(item.id_plantilla) === expectedId &&
      String(item.tipo_plantilla || '').trim() === 'RADICACION ENTRANTE' &&
      String(item.Text_node || '').trim() === expectedName
    );
    return matches.length === 1 ? String(matches[0].value_node || '') : '';
  }, { expectedId: templateId, expectedName: templateName });
  await expect.poll(resolveMenuItemId, {
    timeout: 30000,
    message: 'La plantilla autorizada no apareció en el menú dinámico de Radicación Entrante.'
  }).not.toBe('');
  const menuItemId = await resolveMenuItemId();

  const menuItem = page.locator(`[id="${menuItemId}"]`);
  await expect(menuItem).toHaveCount(1);
  await page.locator('#CR-PR-00 > a').click({ timeout: 15000 });
  await page.locator('#CR-PR-11 > a').click({ timeout: 15000 });
  await expect(menuItem).toBeVisible({ timeout: 15000 });
  await menuItem.click({ timeout: 15000 });
  await expect.poll(async () => String(await page.locator('#ContentPlacenter_ifrm_ds_').getAttribute('src') || ''), {
    timeout: 30000,
    message: 'La selección de plantilla no abrió Radicación Entrante en el iframe oficial.'
  })
    .toMatch(/radicador\/WebFormRadicacionEntrante\.aspx/i);
}

async function currentClassicPendingPage(page) {
  const activePage = page.locator('#GridView_list_registro_rad .pagination-ys span').first();
  if (await activePage.count() === 0) return 1;
  return Number(String(await activePage.textContent() || '').trim());
}

async function findClassicAssignment(page, stateId) {
  const selector = `#GridView_list_registro_rad a[tip_event="a_s_r_p_333"][idd="${stateId}"]`;
  const visitedPages = new Set();

  for (let attempt = 0; attempt < 100; attempt += 1) {
    const assignment = page.locator(selector);
    if (await assignment.count() === 1) return assignment;

    const currentPage = await currentClassicPendingPage(page);
    if (!Number.isSafeInteger(currentPage) || currentPage <= 0 || visitedPages.has(currentPage)) break;
    visitedPages.add(currentPage);

    const pager = page.locator('#GridView_list_registro_rad .pagination-ys');
    if (await pager.count() === 0) break;

    let nextPage = pager.locator('a').filter({ hasText: new RegExp(`^\\s*${currentPage + 1}\\s*$`) }).first();
    if (await nextPage.count() === 0 && currentPage % 10 === 0) {
      nextPage = pager.locator('a').filter({ hasText: /^\s*\.\.\.\s*$/ }).last();
    }
    if (await nextPage.count() === 0) break;

    await nextPage.click();
    await expect.poll(() => currentClassicPendingPage(page)).not.toBe(currentPage);
    await expect(page.locator('#modal_content_list_registro_rad')).toBeVisible();
  }

  throw new Error(`El registro descartable autorizado ${stateId} no aparece en las paginas de pendientes disponibles para la cuenta.`);
}

async function selectClassicRecord(page, stateId, expectedRadicado) {
  await page.waitForLoadState('load');
  await page.waitForFunction(() => {
    const progress = document.querySelector('#progres_bar');
    return !progress || window.getComputedStyle(progress).display === 'none';
  });

  const activeRadicado = String(await page.locator('#Hidden_radicado_seleccion').inputValue() || '').trim();
  if (activeRadicado === expectedRadicado) {
    await expect(page.locator('#soporte-envio_nav')).toHaveClass(/\bactive\b/);
    await expect(page.locator('#soporte-envio_nav')).not.toHaveClass(/\bdisabled\b/);
    await expect(page.locator('#home-radicador')).toHaveClass(/\bdisabled\b/);
    return;
  }
  if (activeRadicado.length > 0) {
    throw new Error('La sesión ya tiene asignado un radicado distinto del recurso descartable autorizado.');
  }

  await expect(page.locator('#home-radicador')).toHaveClass(/\bactive\b/);
  await expect(page.locator('#home-radicador')).not.toHaveClass(/\bdisabled\b/);
  await expect(page.locator('#soporte-envio_nav')).toHaveClass(/\bdisabled\b/);
  await expect(page.locator('#soporte_envio')).not.toHaveClass(/\bactive\b/);
  await expect(page.locator('#a_load_file_nav')).toBeHidden();
  await expect(page.locator('#A1')).toBeVisible();

  await page.locator('#A1').click();
  await expect(page.locator('#modal_content_list_registro_rad')).toBeVisible();
  const assignment = await findClassicAssignment(page, stateId);
  await assignment.click();
  await expect(page.locator('#h_radicado_title')).toContainText(expectedRadicado);
  await expect(page.locator('#Hidden_radicado_seleccion')).toHaveValue(expectedRadicado);
  await expect(page.locator('#modal_content_list_registro_rad')).toBeHidden();
  await expect(page.locator('#soporte-envio_nav')).toHaveClass(/\bactive\b/);
  await expect(page.locator('#soporte-envio_nav')).not.toHaveClass(/\bdisabled\b/);
  await expect(page.locator('#home-radicador')).toHaveClass(/\bdisabled\b/);
}

async function prepareUpload(page, fixturePath, typologyText) {
  const openUpload = page.locator('#a_load_file:visible, #a_load_file_nav:visible').first();
  await expect(openUpload).toBeVisible();
  await openUpload.click();
  const modal = page.locator('#modal_adjunta_documeto_load_documento_006');
  await expect(modal).toBeVisible();
  await page.locator('#file_element_adjunta_documeto_load_documento_006').setInputFiles(fixturePath);
  const pendingRow = page.locator('#table_file_element_adjunta_documeto_load_documento_006 tr[NameFile]');
  await expect(pendingRow).toHaveCount(1);
  const typology = pendingRow.locator('select[id^="element_input_"]');
  if (await typology.count()) await typology.selectOption({ label: typologyText });
  return pendingRow.locator('a[title="Guardar archivo"]');
}

async function installClassicMultipartProbe(page) {
  await page.evaluate(() => {
    const originalFetch = window.fetch;
    window.__doc90ClassicMultipart = null;
    window.fetch = function doc90ObservedFetch(resource, options) {
      const url = typeof resource === 'string' ? resource : resource?.url;
      if (/generic_control\/fileuploadhandler_\.ashx(?:\?|$)/i.test(String(url || '')) &&
          String(options?.method || 'GET').toUpperCase() === 'POST' && options?.body instanceof FormData) {
        window.__doc90ClassicMultipart = Array.from(options.body.entries(), ([name, value]) => ({
          name,
          value: typeof value === 'string' ? value : '[binary]'
        }));
      }
      return originalFetch.apply(this, arguments);
    };
  });
}

function inspectClassicMultipart(contentType, entries) {
  expect(contentType).toMatch(/^multipart\/form-data;\s*boundary=/i);
  expect(Array.isArray(entries)).toBeTruthy();
  const field = name => entries.filter(entry => entry.name === name);
  expect(field('file1')).toHaveLength(1);
  expect(field('evento_adjunta')).toEqual([{ name: 'evento_adjunta', value: 'ADJUNTARADICACION_CLASICA' }]);
  expect(field('id_registro_estado_radicacion')).toHaveLength(0);
  expect(field('radicado_radicacion')).toHaveLength(0);
}

async function writeEvidence(evidence) {
  const target = path.join(e2eRoot, 'artifacts', 'doc90-radicacion-classic-attachment-e2e.json');
  await fs.mkdir(path.dirname(target), { recursive: true });
  await fs.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

test.beforeAll(assertGateOff);
test.afterAll(assertGateOff);

test('DOC-90 adjunta una vez al radicado clásico seleccionado en servidor', async ({ browser }) => {
  for (const authorization of [
    'DOC90_E2E_ENVIRONMENT_AUTHORIZED', 'DOC90_E2E_ACCOUNT_AUTHORIZED',
    'DOC90_E2E_EXECUTION_AUTHORIZED', 'DOC90_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
  ]) expect(required(authorization).toLowerCase()).toBe('true');

  const expectedRadicado = required('DOC90_E2E_EXPECTED_RADICADO');
  const typologyText = required('DOC90_E2E_TYPOLOGY_TEXT');
  const fixture = path.resolve(e2eRoot, required('DOC90_E2E_FIXTURE_PATH'));
  const hashesBefore = await protectedHashes();
  let connection;
  let context;
  let page;
  let stateId;
  let taskId;
  let templateId;
  let templateName;
  let beforeTotal;
  let afterTotal;
  let navigationCount = 0;
  let postbackCount = 0;
  let multipartVerified = false;
  let operationStarted = false;

  try {
    connection = await mysql.createConnection(mysqlOptions());
    const contextRows = await queryRows(connection, 'DOC90_E2E_CONTEXT_SQL', expectedRadicado);
    expect(contextRows).toHaveLength(1);
    expect(String(contextRows[0].radicado || '').trim()).toBe(expectedRadicado);
    stateId = Number(contextRows[0].estado);
    expect(Number.isSafeInteger(stateId) && stateId > 0).toBeTruthy();
    expect(Number(contextRows[0].estado_pendiente)).toBe(1);
    templateId = Number(contextRows[0].plantilla);
    expect(Number.isSafeInteger(templateId) && templateId > 0).toBeTruthy();
    templateName = String(contextRows[0].plantilla_nombre || '').trim();
    expect(templateName.length).toBeGreaterThan(0);
    taskId = Number(contextRows[0].tarea);
    expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();
    expect(Number(contextRows[0].tramite) > 0).toBeTruthy();
    beforeTotal = documentTotal(await queryRows(connection, 'DOC90_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));

    context = await login(browser);
    page = await context.newPage();
    await selectIncomingTemplate(page, templateId, templateName);
    page.on('request', request => {
      const isMainDocumentNavigation = operationStarted && request.isNavigationRequest() &&
        request.resourceType() === 'document' && request.frame() === page.mainFrame();
      if (isMainDocumentNavigation) navigationCount += 1;
      if (isMainDocumentNavigation && request.method() === 'POST') postbackCount += 1;
    });
    await page.goto(new URL(required('DOC90_E2E_PAGE_PATH'), baseUrl()).toString(), { waitUntil: 'domcontentloaded' });
    await selectClassicRecord(page, stateId, expectedRadicado);

    const rowsBefore = await page.locator('#GridView_list_documento_relacion tr[id_rad]').count();
    const save = await prepareUpload(page, fixture, typologyText);
    await installClassicMultipartProbe(page);
    const responsePromise = page.waitForResponse(response =>
      response.request().method() === 'POST' && /generic_control\/fileuploadhandler_\.ashx(?:\?|$)/i.test(response.url())
    );
    operationStarted = true;
    await save.click();
    const response = await responsePromise;
    const multipartEntries = await page.evaluate(() => window.__doc90ClassicMultipart);
    inspectClassicMultipart(response.request().headers()['content-type'] || '', multipartEntries);
    multipartVerified = true;
    expect(response.ok()).toBeTruthy();
    let payload;
    try { payload = await response.json(); } catch { throw new Error('El handler DOC-90 no retornó el contrato JSON esperado.'); }
    expect(Array.isArray(payload) && payload.length === 1).toBeTruthy();
    const uploaded = payload[0];
    expect(uploaded.error_sistema).toBe('YES');
    expect(String(uploaded.radicado || '').trim()).toBe(expectedRadicado);
    expect(Number(uploaded.id_tarea_workflow)).toBe(taskId);
    expect(String(uploaded.notitipodocumental || '').trim()).toBe(typologyText);
    expect(Number(uploaded.id_image) > 0).toBeTruthy();
    expect(multipartVerified).toBeTruthy();

    const inserted = page.locator(`#GridView_list_documento_relacion tr[id_rad="${Number(uploaded.id_image)}"]`);
    await expect(inserted).toHaveCount(1);
    await expect(inserted).toContainText(typologyText);
    await expect(page.locator('#GridView_list_documento_relacion tr[id_rad]')).toHaveCount(rowsBefore + 1);
    afterTotal = documentTotal(await queryRows(connection, 'DOC90_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));
    expect(afterTotal).toBe(beforeTotal + 1);
  } catch (error) {
    if (operationStarted && connection && Number.isSafeInteger(beforeTotal)) {
      try {
        afterTotal = documentTotal(await queryRows(connection, 'DOC90_E2E_DOCUMENT_COUNT_SQL', expectedRadicado));
      } catch (reconciliationError) {
        throw new AggregateError(
          [error, reconciliationError],
          'La carga DOC-90 inició, falló y no fue posible reconciliar la persistencia con SELECT. No reintente con el mismo recurso.'
        );
      }
      if (afterTotal !== beforeTotal) {
        throw new Error(
          `La carga DOC-90 falló después de iniciar el POST y el conteo cambió de ${beforeTotal} a ${afterTotal}. El servidor pudo persistir el documento; no reintente con el mismo recurso.`,
          { cause: error }
        );
      }
    }
    throw error;
  } finally {
    try { await context?.close(); } catch { /* El timeout no debe ocultar el paso funcional que falló. */ }
    try { await connection?.end(); } catch { /* La limpieza no reemplaza la evidencia del fallo. */ }
    await assertGateOff();
  }

  const hashesAfter = await protectedHashes();
  expect(hashesAfter).toEqual(hashesBefore);
  expect(navigationCount).toBe(0);
  expect(postbackCount).toBe(0);
  await writeEvidence({
    fechaUtc: new Date().toISOString(),
    recurso: hash(`${stateId}:${expectedRadicado}`).slice(0, 16),
    contratoMultipartClasico: multipartVerified,
    persistencia: { antes: beforeTotal, despues: afterTotal },
    insercionSinRecarga: navigationCount === 0 && postbackCount === 0,
    archivosProtegidosIntactos: JSON.stringify(hashesAfter) === JSON.stringify(hashesBefore),
    gateIntacto: true
  });
});

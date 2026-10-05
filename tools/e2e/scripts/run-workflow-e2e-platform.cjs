'use strict';

const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { chromium, request } = require('playwright');
const {
  collectConfirmation,
  collectValue,
  promptWithTimeout,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { queryFingerprint } = require('./support/doc32-e2e-odbc.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const {
  executePlatformRun,
  assertPlatformIntegrity,
  captureLegacyIntegrityBaseline,
  normalizeDocumentUrl,
  preflightPlatform,
  requiredAuthorizationsFor
} = require('./support/workflow-e2e-platform.cjs');
const { loadProfile } = require('./support/workflow-e2e-platform-profile.cjs');
const { resolveScenario } = require('./support/workflow-e2e-platform-registry.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');
const SAFE_IDENTIFIER = /^[a-z][a-z0-9-]{1,79}$/;
const AUTHORIZATION_LABELS = Object.freeze({
  environment: '¿Autoriza este ambiente de pruebas?',
  'local-tls': '¿Autoriza temporalmente el certificado local autofirmado?',
  execution: '¿Autoriza la ejecución sobre un recurso descartable?',
  concurrency: '¿Autoriza la concurrencia sobre un recurso descartable?',
  'ui-lock': '¿Autoriza el bloqueo UI sobre un recurso descartable?',
  'discardable-resource': '¿Confirma que el recurso es descartable?'
  ,gate: '¿Autoriza activar temporalmente el gate moderno y restaurarlo al finalizar?'
});
const SECRET_LABELS = Object.freeze({
  'workflow-account': Object.freeze({ label: 'Cuenta Workflow autorizada', secret: false }),
  'workflow-password': Object.freeze({ label: 'Contraseña Workflow', secret: true }),
  'readonly-db-user': Object.freeze({ label: 'Usuario MySQL de solo lectura', secret: false }),
  'readonly-db-password': Object.freeze({ label: 'Contraseña MySQL de solo lectura', secret: true })
});

function fail(code) {
  const error = new Error(`La plataforma E2E no inició la corrida (${code}).`);
  error.code = code;
  throw error;
}

function parseArguments(argv) {
  const result = { scenarioId: null, profilePath: null, requestedAuthorizations: new Set() };
  for (let index = 0; index < argv.length; index += 1) {
    const argument = argv[index];
    if (!['--scenario', '--profile', '--authorize'].includes(argument)) fail('E2E_PLATFORM_ARGUMENT_INVALID');
    const value = argv[index + 1];
    if (!value || value.startsWith('--')) fail('E2E_PLATFORM_ARGUMENT_VALUE_REQUIRED');
    index += 1;
    if (argument === '--scenario') {
      if (!SAFE_IDENTIFIER.test(value) || result.scenarioId) fail('E2E_PLATFORM_ARGUMENT_INVALID');
      result.scenarioId = value;
    } else if (argument === '--profile') {
      if (result.profilePath) fail('E2E_PLATFORM_ARGUMENT_INVALID');
      result.profilePath = value;
    } else {
      for (const authorization of value.split(',')) {
        if (!SAFE_IDENTIFIER.test(authorization)) fail('E2E_PLATFORM_ARGUMENT_INVALID');
        result.requestedAuthorizations.add(authorization);
      }
    }
  }
  if (!result.scenarioId || !result.profilePath) fail('E2E_PLATFORM_ARGUMENT_REQUIRED');
  return result;
}

async function collectAuthorizations(required, requested) {
  if (requested.size !== required.length || required.some((authorization) => !requested.has(authorization))) {
    fail('E2E_PLATFORM_AUTHORIZATION_ARGUMENT_REQUIRED');
  }
  if (required.length === 0) return new Set();
  requireInteractiveConsole();
  const confirmations = {};
  for (const authorization of required) {
    const label = AUTHORIZATION_LABELS[authorization];
    if (!label) fail('E2E_PLATFORM_AUTHORIZATION_INVALID');
    await collectConfirmation(confirmations, authorization, label);
  }
  return new Set(required);
}

async function collectSecrets(plan) {
  if (plan.scenario.requiredSecrets.length === 0) return {};
  requireInteractiveConsole();
  const values = {};
  for (const secretName of plan.scenario.requiredSecrets) {
    const instruction = SECRET_LABELS[secretName];
    if (!instruction) fail('E2E_PLATFORM_SECRET_REGISTRY_INVALID');
    await collectValue(values, secretName, instruction.label, { secret: instruction.secret });
  }
  return values;
}

function endpoint(baseUrl, servicePath, operation) {
  return new URL(`${servicePath}/${operation}`, baseUrl).toString();
}

function assertPublicResponse(dto) {
  const serialized = JSON.stringify(dto);
  if (/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i.test(serialized)) fail('E2E_PLATFORM_RESPONSE_REJECTED');
}

async function createClient({ context, plan }) {
  if (context && !plan.profile.ignoreHttpsErrors) return { request: context.request, dispose: async () => {} };
  const options = context ? { storageState: await context.storageState() } : {};
  if (plan.profile.ignoreHttpsErrors) options.ignoreHTTPSErrors = true;
  const api = await request.newContext(options);
  return { request: api, dispose: () => api.dispose() };
}

async function invokeNotes({ client, servicePath, operation, payload, plan }) {
  const started = performance.now();
  const response = await client.request.post(endpoint(plan.profile.baseUrl, servicePath, operation), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: Math.min(plan.profile.budgetMs, 60000)
  });
  const elapsedMs = Math.round(performance.now() - started);
  if (!response.ok()) fail('E2E_PLATFORM_HTTP_FAILED');
  const envelope = await response.json();
  if (!envelope || typeof envelope.d !== 'object' || envelope.d === null) fail('E2E_PLATFORM_RESPONSE_INVALID');
  assertPublicResponse(envelope.d);
  return { dto: envelope.d, elapsedMs };
}

async function consumeImportPreview({ client, plan, descriptorId, method }) {
  if (typeof descriptorId !== 'string' || !/^[A-Za-z0-9_-]{43}$/.test(descriptorId)) fail('IMPORT_E2E_PREVIEW_DESCRIPTOR_INVALID');
  if (!['GET', 'HEAD'].includes(method)) fail('IMPORT_E2E_PREVIEW_METHOD_INVALID');
  const started = performance.now();
  const url = new URL('workflow/ImportarServicioWebPreview.ashx', plan.profile.baseUrl);
  url.searchParams.set('d', descriptorId);
  const response = await client.request.fetch(url.toString(), { method, timeout: Math.min(plan.profile.budgetMs, 60000) });
  const headers = response.headers();
  const body = method === 'GET' && response.status() === 200 ? await response.body() : Buffer.alloc(0);
  return Object.freeze({
    status: response.status(), elapsedMs: Math.round(performance.now() - started),
    contentType: headers['content-type'] || '', contentLength: Number(headers['content-length'] || 0),
    contentDisposition: headers['content-disposition'] || '', cacheControl: headers['cache-control'] || '',
    noSniff: headers['x-content-type-options'] || '', frameOptions: headers['x-frame-options'] || '',
    bodyLength: body.length
  });
}

async function writeEvidence(evidence) {
  const destination = path.join(e2eRoot, 'artifacts', `workflow-e2e-platform-${evidence.scenario}.json`);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function selectWorkflowTask(page, plan) {
    const taskId = plan.profile[plan.scenario.resource.profileField];
    if (!Number.isSafeInteger(taskId) || taskId <= 0) fail('E2E_PLATFORM_TASK_CONTEXT_INVALID');
    await page.goto(new URL('workflow/Webworkflow.aspx', plan.profile.baseUrl).toString(), {
      waitUntil: 'domcontentloaded',
      timeout: Math.min(plan.profile.budgetMs, 60000)
    });
    const expectedTaskId = String(taskId);
    if (['import-sii-enlase-read', 'import-sii-enlase-ui', 'import-sii-enlase-layout-review', 'import-sii-enlase-execution', 'import-sii-enlase-manual-visual'].includes(plan.scenario.id) ||
        plan.scenario.id === 'import-sii-enlase-assignment') {
      const enlaceSelection = page.locator('#HiddenIdFlujo');
      await enlaceSelection.waitFor({ state: 'attached', timeout: Math.min(plan.profile.budgetMs, 60000) });
      const isExpectedEnlase = (value) => {
        const parts = String(value || '').split('|');
        return parts.length >= 4 && parts[0] === expectedTaskId && parts[3].toUpperCase() === 'ENLASE';
      };
      if (!isExpectedEnlase(await enlaceSelection.inputValue())) {
        const selectionTimeout = Math.min(plan.profile.budgetMs, 60000);
        await page.evaluate(() => {
          if (typeof window.hide_area_workflow_seleccion === 'function') window.hide_area_workflow_seleccion();
        });
        const selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]`).first();
        if (!await selectCommand.count()) {
          const taskSearch = page.locator('#auto_complex:visible');
          await taskSearch.waitFor({ state: 'visible', timeout: selectionTimeout });
          await taskSearch.fill(expectedTaskId);
          await page.locator('button[title="consultar lista"]:visible').click();
        }
        await Promise.race([
          selectCommand.waitFor({ state: 'attached', timeout: selectionTimeout }),
          page.waitForFunction(
            (expected) => {
              const parts = String(document.querySelector('#HiddenIdFlujo')?.value || '').split('|');
              return parts.length >= 4 && parts[0] === expected && parts[3].toUpperCase() === 'ENLASE';
            },
            expectedTaskId,
            { timeout: selectionTimeout }
          )
        ]).catch(() => fail('E2E_PLATFORM_ENLASE_TASK_NOT_LISTED'));
        if (isExpectedEnlase(await enlaceSelection.inputValue())) return;
        await page.evaluate((expected) => {
          const candidate = document.querySelector(`[tip_event="seleccion_tarea_wf"][idd="${expected}"]`);
          const stagedTask = document.querySelector('#Hidden_id_tarea_sel');
          const officialSelector = document.querySelector('#ButtonSeleccionGrupo');
          if (!candidate || !stagedTask || !officialSelector) throw new Error('E2E_PLATFORM_TASK_SELECTION_CONTROLS_UNAVAILABLE');
          stagedTask.value = expected;
          officialSelector.click();
        }, expectedTaskId).catch(() => fail('E2E_PLATFORM_TASK_SELECTION_CONTROLS_UNAVAILABLE'));
        await page.waitForFunction(
          (expected) => {
            const parts = String(document.querySelector('#HiddenIdFlujo')?.value || '').split('|');
            return parts.length >= 4 && parts[0] === expected && parts[3].toUpperCase() === 'ENLASE';
          },
          expectedTaskId,
          { timeout: selectionTimeout }
        ).catch(() => fail('E2E_PLATFORM_ENLASE_CONTEXT_REJECTED'));
      }
      if (!isExpectedEnlase(await enlaceSelection.inputValue())) fail('E2E_PLATFORM_ENLASE_CONTEXT_UNAVAILABLE');
      return;
    }
    const selectedTask = page.locator('#Hidden_id_tarea_selecionada');
    await selectedTask.waitFor({ state: 'attached', timeout: Math.min(plan.profile.budgetMs, 60000) });
    if (await selectedTask.inputValue() !== expectedTaskId) {
      await page.evaluate(() => {
        if (typeof window.hide_area_workflow_seleccion === 'function') window.hide_area_workflow_seleccion();
      });
      const selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]:visible`).first();
      if (!await selectCommand.count()) {
        const taskSearch = page.locator('#auto_complex:visible');
        await taskSearch.waitFor({ state: 'visible', timeout: Math.min(plan.profile.budgetMs, 30000) });
        await taskSearch.fill(expectedTaskId);
        await page.locator('button[title="consultar lista"]:visible').click();
      }
      await selectCommand.waitFor({ state: 'visible', timeout: Math.min(plan.profile.budgetMs, 30000) });
      await page.evaluate((expected) => {
        const candidate = document.querySelector(`[tip_event="seleccion_tarea_wf"][idd="${expected}"]`);
        const stagedTask = document.querySelector('#Hidden_id_tarea_sel');
        const officialSelector = document.querySelector('#ButtonSeleccionGrupo');
        if (!candidate || !stagedTask || !officialSelector) throw new Error('E2E_PLATFORM_TASK_SELECTION_CONTROLS_UNAVAILABLE');
        stagedTask.value = expected;
        officialSelector.click();
      }, expectedTaskId).catch(() => fail('E2E_PLATFORM_TASK_SELECTION_CONTROLS_UNAVAILABLE'));
      await page.waitForFunction(
        ([selector, expected]) => document.querySelector(selector)?.value === expected,
        ['#Hidden_id_tarea_selecionada', expectedTaskId],
        { timeout: Math.min(plan.profile.budgetMs, 30000) }
      ).catch(() => fail('E2E_PLATFORM_TASK_SELECTION_REJECTED'));
      if (await selectedTask.inputValue() !== expectedTaskId) {
        fail('E2E_PLATFORM_TASK_SELECTION_REJECTED');
      }
    }
    if (await selectedTask.inputValue() !== expectedTaskId) fail('E2E_PLATFORM_TASK_CONTEXT_UNAVAILABLE');
}

async function initializeWorkflowContext(context, plan) {
  if (plan.scenario.resource?.profileField !== 'taskId') return context;
  const page = await context.newPage();
  try {
    await selectWorkflowTask(page, plan);
  } finally {
    await page.close();
  }
  return context;
}

async function inspectEnlaseAssignmentUi({ context, plan }) {
  const page = await context.newPage();
  let dialogObserved = false;
  const onDialog = async (dialog) => {
    dialogObserved = true;
    await dialog.dismiss().catch(() => {});
  };
  page.on('dialog', onDialog);
  try {
    await selectWorkflowTask(page, plan);
    const timeout = Math.min(plan.profile.budgetMs, 60000);
    const panel = page.locator('#Panel_admon_documentos');
    const action = page.locator('#enlase-assign-action:visible');
    const officialButton = page.locator('#Buttonaceptar');
    await panel.waitFor({ state: 'visible', timeout });
    await action.waitFor({ state: 'visible', timeout });
    if (await action.getAttribute('data-import-context-action') !== 'true' ||
        await officialButton.getAttribute('data-import-context-action') !== 'true') {
      fail('IMPORT_E2E_ENLASE_ASSIGNMENT_ACTION_INVALID');
    }
    const postback = page.waitForResponse((response) =>
      response.request().method() === 'POST' && /\/workflow\/Webworkflow\.aspx(?:\?|$)/i.test(response.url()),
    { timeout });
    await action.click();
    const response = await postback.catch(() => fail('IMPORT_E2E_ENLASE_ASSIGNMENT_POSTBACK_UNAVAILABLE'));
    if (!response.ok()) fail('IMPORT_E2E_ENLASE_ASSIGNMENT_POSTBACK_FAILED');
    await page.waitForFunction(() => {
      const button = document.querySelector('#Buttonaceptar');
      return button && button.disabled === false && !/espere/i.test(String(button.value || ''));
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_ENLASE_ASSIGNMENT_RESULT_UNAVAILABLE'));
    await page.waitForTimeout(250);
    const panelVisible = await panel.isVisible();
    const assignmentResult = panelVisible ? 'BLOCKED' : 'ASSIGNED';
    if (assignmentResult === 'BLOCKED' && !dialogObserved) {
      fail('IMPORT_E2E_ENLASE_ASSIGNMENT_BLOCK_REASON_UNAVAILABLE');
    }
    return Object.freeze({
      codes: Object.freeze({ assignmentResult, authoritativeValidation: 'CONFIRMED' }),
      count: 1,
      latenciesMs: Object.freeze([])
    });
  } finally {
    page.off('dialog', onDialog);
    await page.close();
  }
}

async function confirmManualVisual(label, deadline) {
  const remaining = deadline - Date.now();
  if (remaining <= 0) fail('IMPORT_E2E_MANUAL_VISUAL_TIMEOUT');
  const confirmation = await promptWithTimeout(`${label} (escriba SI)`, remaining);
  if (confirmation === null) fail('IMPORT_E2E_MANUAL_VISUAL_TIMEOUT');
  if (String(confirmation).toUpperCase() !== 'SI') fail('IMPORT_E2E_MANUAL_VISUAL_REJECTED');
}

function validateManualVisualRows(rows, taskId, projectionCount) {
  const added = (Array.isArray(rows) ? rows : []).filter((row) => row && String(row.id || '').trim());
  if (!Number.isInteger(projectionCount) || projectionCount < 1) fail('IMPORT_E2E_MANUAL_VISUAL_PROJECTION_NOT_OBSERVED');
  if (added.length === 0) fail('IMPORT_E2E_MANUAL_VISUAL_ROW_UNAVAILABLE');
  for (const row of added) {
    const id = String(row.id || '').trim();
    const parts = String(row.contract || '').split('|').map((part) => part.trim());
    if (!/^\d+$/.test(id) || Number(id) <= 0 || parts.length < 8 || parts.slice(0, 8).some((part) => !part) ||
        parts[1] !== id || Number(parts[5]) !== Number(taskId) || !/^-?\d+$/.test(parts[6])) {
      fail('IMPORT_E2E_MANUAL_VISUAL_ROW_CONTRACT_INVALID');
    }
  }
  return added.length;
}

async function inspectEnlaseManualVisual({ context, plan }) {
  requireInteractiveConsole();
  const page = await context.newPage();
  const deadline = Date.now() + Math.min(plan.profile.budgetMs, 600000);
  const marker = `doc83-${Date.now()}`;
  let dialogObserved = false;
  const onDialog = async (dialog) => {
    dialogObserved = true;
    await dialog.dismiss().catch(() => {});
  };
  page.on('dialog', onDialog);
  try {
    await selectWorkflowTask(page, plan);
    await page.bringToFront();
    const timeout = Math.min(plan.profile.budgetMs, 60000);
    await page.waitForFunction(() => {
      const manager = window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager
        ? window.Sys.WebForms.PageRequestManager.getInstance() : null;
      return !manager || !manager.get_isInAsyncPostBack();
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_MANUAL_VISUAL_UI_NOT_STABLE'));
    const trigger = page.locator('#a_adj_service_web');
    await trigger.waitFor({ state: 'visible', timeout });
    await page.waitForFunction(() => {
      const candidate = document.getElementById('a_adj_service_web');
      return candidate && candidate.getAttribute('data-import-modern-active') === 'true' &&
        candidate.getAttribute('data-import-modern-bound') === 'true';
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_MANUAL_VISUAL_BOOTSTRAP_INACTIVE'));
    await trigger.click();
    const modal = page.locator('#importar-servicio-web-modal');
    await modal.waitFor({ state: 'visible', timeout });
    await page.locator('#importar-servicio-web-modal[data-import-state=resultados]').waitFor({ state: 'visible', timeout })
      .catch(() => fail('IMPORT_E2E_MANUAL_VISUAL_ITEMS_UNAVAILABLE'));
    const importActions = page.locator('[data-import-prepare=true]:visible:not([disabled])');
    const importActionCount = await importActions.count();
    if (importActionCount < 1) fail('IMPORT_E2E_MANUAL_VISUAL_ITEMS_UNAVAILABLE');
    const rows = page.locator('#GridView_list_documento_relacion tr[id_rad]');
    const baseline = await page.evaluate((value) => {
      const resolveGrid = () => document.getElementById('GridView_list_documento_relacion');
      const grid = resolveGrid();
      const originalInsert = window.insert_row_documento_relacionado;
      if (!grid) return { ready: false, code: 'GRID_UNAVAILABLE', initialRowCount: 0 };
      if (typeof originalInsert !== 'function') return { ready: false, code: 'INSERTER_UNAVAILABLE', initialRowCount: 0 };
      const initialRows = Array.prototype.slice.call(grid.querySelectorAll('tr[id_rad]'));
      window.__doc83ManualVisualMarker = value;
      window.__doc83ManualVisualProjectionCount = 0;
      window.__doc83ManualVisualProjectionRows = [];
      window.__doc83ManualVisualPostbackCount = 0;
      window.__doc83ManualVisualPostbackKinds = [];
      const manager = window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager
        ? window.Sys.WebForms.PageRequestManager.getInstance() : null;
      if (manager) manager.add_beginRequest(function (sender, args) {
        const element = args && typeof args.get_postBackElement === 'function' ? args.get_postBackElement() : null;
        const id = String(element && element.id || '');
        let kind = 'UNKNOWN';
        if (id === 'Button_actualiza_trevie_seleccion') kind = 'DOCUMENT_REFRESH';
        else if (/selecion_treview_documento|visualiza_documento|visor/i.test(id)) kind = 'DOCUMENT_INTERACTION';
        else if (id) kind = 'OTHER_CONTROL';
        window.__doc83ManualVisualPostbackCount += 1;
        window.__doc83ManualVisualPostbackKinds.push(kind);
      });
      window.insert_row_documento_relacionado = function () {
        const currentGrid = resolveGrid();
        const beforeIds = currentGrid ? Array.prototype.map.call(currentGrid.querySelectorAll('tr[id_rad]'), function (row) {
          return String(row.getAttribute('id_rad') || '').trim();
        }) : [];
        const destination = String(arguments[1] || '');
        const result = originalInsert.apply(this, arguments);
        const liveGrid = resolveGrid();
        const addedRows = liveGrid ? Array.prototype.filter.call(liveGrid.querySelectorAll('tr[id_rad]'), function (row) {
          return beforeIds.indexOf(String(row.getAttribute('id_rad') || '').trim()) < 0;
        }) : [];
        if (destination === 'rad' && addedRows.length) {
          window.__doc83ManualVisualProjectionCount += addedRows.length;
          addedRows.forEach(function (row) {
            window.__doc83ManualVisualProjectionRows.push({
              id: String(row.getAttribute('id_rad') || '').trim(),
              contract: String(row.getAttribute('idd_rad') || '').trim()
            });
          });
        }
        return result;
      };
      return { ready: true, code: 'READY', initialRowCount: initialRows.length };
    }, marker);
    if (!baseline.ready) fail(`IMPORT_E2E_MANUAL_VISUAL_${baseline.code}`);
    console.log(`DOC83_MANUAL_VISUAL_READY initialRows=${baseline.initialRowCount}; siiActions=${importActionCount}. La lista SII está cargada. Use Importar/Reimportar y NO recargue; confirme únicamente cuando aparezca una fila nueva.`);
    await confirmManualVisual('¿Confirma que la fila apareció inmediatamente sin recargar?', deadline);
    if (page.isClosed()) fail('IMPORT_E2E_MANUAL_VISUAL_BROWSER_CLOSED');
    const markerPreserved = await page.evaluate((value) => window.__doc83ManualVisualMarker === value, marker).catch(() => false);
    if (!markerPreserved) fail('IMPORT_E2E_MANUAL_VISUAL_RELOAD_DETECTED');
    const modalVisible = await modal.isVisible().catch(() => true);
    if (modalVisible) fail('IMPORT_E2E_MANUAL_VISUAL_RESULT_NOT_CLOSED');
    const observed = await page.evaluate(() => ({
      projectionCount: Number(window.__doc83ManualVisualProjectionCount) || 0,
      postbackCount: Number(window.__doc83ManualVisualPostbackCount) || 0,
      postbackKinds: Array.isArray(window.__doc83ManualVisualPostbackKinds) ? window.__doc83ManualVisualPostbackKinds.slice(0, 5) : [],
      rows: Array.isArray(window.__doc83ManualVisualProjectionRows) ? window.__doc83ManualVisualProjectionRows.slice(0) : []
    }));
    const addedCount = validateManualVisualRows(observed.rows, plan.profile.taskId, observed.projectionCount);
    if (observed.postbackCount > 0) {
      console.log(`DOC83_MANUAL_VISUAL_POSTBACK_DIAGNOSTIC count=${observed.postbackCount}; kinds=${observed.postbackKinds.join(',') || 'UNKNOWN'}; projectionCount=${observed.projectionCount}; addedRows=${observed.rows.length}`);
      fail('IMPORT_E2E_MANUAL_VISUAL_POSTBACK_OBSERVED');
    }
    if (dialogObserved) fail('IMPORT_E2E_MANUAL_VISUAL_DIALOG_OBSERVED');
    dialogObserved = false;
    console.log('DOC83_MANUAL_VISUAL_ROW_CONFIRMED. Abra una vez el documento recién agregado y verifique que no aparezca un error.');
    await confirmManualVisual('¿Confirma que el documento abrió sin error?', deadline);
    if (page.isClosed()) fail('IMPORT_E2E_MANUAL_VISUAL_BROWSER_CLOSED');
    if (dialogObserved) fail('IMPORT_E2E_MANUAL_VISUAL_INTERACTION_FAILED');
    return Object.freeze({
      codes: Object.freeze({ manualVisual: 'CONFIRMED', gridProjection: 'CONFIRMED', rowInteraction: 'CONFIRMED' }),
      count: addedCount,
      latenciesMs: Object.freeze([])
    });
  } finally {
    page.off('dialog', onDialog);
    await page.close().catch(() => {});
  }
}
async function inspectEnlaseLayoutReview({ context, plan }) {
  requireInteractiveConsole();
  const page = await context.newPage();
  const deadline = Date.now() + Math.min(plan.profile.budgetMs, 600000);
  let mutationRequests = 0;
  const onRequest = (request) => {
    if (/WebServiceImportarServicioWebModern\.asmx\/(?:CreateImportIntent|ExecuteImportIntent)(?:\?|$)/i.test(request.url())) mutationRequests += 1;
  };
  page.on('request', onRequest);
  try {
    await page.setViewportSize({ width: 1100, height: 800 });
    await selectWorkflowTask(page, plan);
    await page.bringToFront();
    const timeout = Math.min(plan.profile.budgetMs, 60000);
    await page.waitForFunction(() => {
      const manager = window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager
        ? window.Sys.WebForms.PageRequestManager.getInstance() : null;
      return !manager || !manager.get_isInAsyncPostBack();
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_UI_NOT_STABLE'));
    const trigger = page.locator('#a_adj_service_web');
    await trigger.waitFor({ state: 'visible', timeout });
    await page.waitForFunction(() => {
      const candidate = document.getElementById('a_adj_service_web');
      return candidate && candidate.getAttribute('data-import-modern-active') === 'true' &&
        candidate.getAttribute('data-import-modern-bound') === 'true';
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_BOOTSTRAP_INACTIVE'));
    await trigger.click();
    const modal = page.locator('#importar-servicio-web-modal');
    await modal.waitFor({ state: 'visible', timeout });
    await page.locator('#importar-servicio-web-modal[data-import-state=resultados]').waitFor({ state: 'visible', timeout })
      .catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_ITEMS_UNAVAILABLE'));
    const tableScroll = page.locator('.importar-servicio-web-sii__table-scroll');
    await tableScroll.waitFor({ state: 'visible', timeout });
    const layout = await tableScroll.evaluate((element) => {
      const dataCells = Array.from(element.querySelectorAll('td.importar-servicio-web-sii__data-cell'));
      const actionCells = Array.from(element.querySelectorAll('td.importar-servicio-web-sii__actions-cell'));
      const rows = Array.from(element.querySelectorAll('tbody tr'));
      const scrollStyle = getComputedStyle(element);
      const invalidDataCells = dataCells.filter((cell) => {
        const style = getComputedStyle(cell);
        const rect = cell.getBoundingClientRect();
        const tableRect = element.querySelector('table')?.getBoundingClientRect();
        return style.overflowX !== 'hidden' || style.textOverflow !== 'ellipsis' ||
          rect.width <= 0 || !tableRect || rect.left < tableRect.left - 1 || rect.right > tableRect.right + 1;
      }).length;
      const invalidActionCells = actionCells.filter((cell) => {
        const style = getComputedStyle(cell);
        return style.position !== 'sticky' || /rgba?\(0,\s*0,\s*0,\s*0\)|transparent/i.test(style.backgroundColor);
      }).length;
      return {
        rows: rows.length,
        dataCells: dataCells.length,
        truncatedCells: dataCells.filter((cell) => cell.scrollWidth > cell.clientWidth + 1).length,
        invalidDataCells,
        invalidActionCells,
        overflowX: scrollStyle.overflowX,
        overflowY: scrollStyle.overflowY,
        clientWidth: element.clientWidth,
        scrollWidth: element.scrollWidth
      };
    });
    if (layout.rows < 1 || layout.dataCells < 1) fail('IMPORT_E2E_LAYOUT_REVIEW_ITEMS_UNAVAILABLE');
    if (layout.invalidDataCells !== 0 || layout.invalidActionCells !== 0 ||
        layout.overflowX !== 'auto' || layout.overflowY !== 'auto' || layout.clientWidth <= 0 || layout.scrollWidth < layout.clientWidth) {
      fail('IMPORT_E2E_LAYOUT_REVIEW_CONTAINMENT_INVALID');
    }
    if (mutationRequests !== 0) fail('IMPORT_E2E_LAYOUT_REVIEW_MUTATION_OBSERVED');
    console.log(`DOC83_LAYOUT_REVIEW_READY rows=${layout.rows}; dataCells=${layout.dataCells}; truncatedCells=${layout.truncatedCells}. Revise que los textos no se superpongan, que Acciones permanezca legible y que el desplazamiento ocurra dentro de la tabla.`);
    await confirmManualVisual('¿Confirma que no hay textos superpuestos y que el scroll permanece dentro de la tabla?', deadline);
    if (page.isClosed()) fail('IMPORT_E2E_LAYOUT_REVIEW_BROWSER_CLOSED');
    if (mutationRequests !== 0) fail('IMPORT_E2E_LAYOUT_REVIEW_MUTATION_OBSERVED');

    const selectAll = page.locator('[data-import-select-all="true"]');
    const selectableCount = await page.locator('[data-import-select="true"]:visible:not([disabled])').count();
    if (selectableCount < 2 || !await selectAll.isVisible()) fail('IMPORT_E2E_LAYOUT_REVIEW_MULTIPLE_ITEMS_UNAVAILABLE');
    await selectAll.check();
    await page.locator('[data-import-prepare-selected="true"]:visible:not([disabled])').click();
    const preparation = page.locator('#importar-servicio-web-preparation');
    await preparation.waitFor({ state: 'visible', timeout });
    await page.waitForFunction(() => {
      const confirm = document.getElementById('importar-servicio-web-preparation-confirm');
      return confirm && confirm.disabled === false;
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_PREPARATION_UNAVAILABLE'));
    const preparationLayout = await preparation.evaluate((panel) => {
      const items = panel.querySelector('#importar-servicio-web-preparation-items');
      const actions = panel.querySelector('.importar-servicio-web__preparation-actions');
      const confirm = panel.querySelector('#importar-servicio-web-preparation-confirm');
      const cancel = panel.querySelector('#importar-servicio-web-preparation-cancel');
      if (!items || !actions || !confirm || !cancel) return { valid: false, itemCount: 0 };
      const panelRect = panel.getBoundingClientRect();
      const actionsRect = actions.getBoundingClientRect();
      const itemsStyle = getComputedStyle(items);
      return {
        valid: actionsRect.top >= panelRect.top - 1 && actionsRect.bottom <= panelRect.bottom + 1 &&
          actionsRect.bottom <= window.innerHeight + 1 && confirm.offsetParent !== null && cancel.offsetParent !== null &&
          itemsStyle.overflowY === 'auto',
        itemCount: items.children.length
      };
    });
    if (!preparationLayout.valid || preparationLayout.itemCount < 2) fail('IMPORT_E2E_LAYOUT_REVIEW_PREPARATION_ACTIONS_INVALID');
    console.log(`DOC83_PREPARATION_REVIEW_READY items=${preparationLayout.itemCount}. Revise que Cancelar y Crear intención permanezcan visibles mientras desplaza únicamente la lista de documentos.`);
    await confirmManualVisual('¿Confirma que los botones de preparación permanecen visibles y que no aparece el bloque Plan previsto?', deadline);
    await page.locator('#importar-servicio-web-preparation-cancel').click();
    await preparation.waitFor({ state: 'hidden', timeout });

    const previewButton = page.locator('[data-import-preview="true"]:visible').first();
    await previewButton.click();
    const preview = page.locator('#importar-servicio-web-preview');
    await page.locator('#importar-servicio-web-preview[data-preview-state="disponible"], #importar-servicio-web-preview[data-preview-state="formato-no-visualizable"]').waitFor({ state: 'visible', timeout })
      .catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_PREVIEW_UNAVAILABLE'));
    const previewLayout = await preview.evaluate((panel) => {
      const back = panel.querySelector('#importar-servicio-web-preview-back');
      const frame = panel.querySelector('#importar-servicio-web-preview-frame');
      const list = document.getElementById('importar-servicio-web-list');
      const style = getComputedStyle(panel);
      const panelRect = panel.getBoundingClientRect();
      const frameRect = frame && !frame.hidden ? frame.getBoundingClientRect() : null;
      return {
        valid: !!back && back.offsetParent !== null && !!list && list.hidden === true &&
          style.display === 'flex' && style.flexDirection === 'column' && panelRect.bottom <= window.innerHeight + 1 &&
          (!frameRect || (frameRect.top >= panelRect.top - 1 && frameRect.bottom <= panelRect.bottom + 1)),
        frameVisible: !!frameRect
      };
    });
    if (!previewLayout.valid) fail('IMPORT_E2E_LAYOUT_REVIEW_PREVIEW_ALIGNMENT_INVALID');
    console.log(`DOC83_PREVIEW_REVIEW_READY frameVisible=${previewLayout.frameVisible ? 'SI' : 'NO'}. Revise que Volver a documentos esté visible y que el visor quede alineado verticalmente.`);
    await confirmManualVisual('¿Confirma que puede volver a documentos y que el preview no presenta desajuste vertical?', deadline);
    if (await preview.isVisible()) {
      await page.locator('#importar-servicio-web-preview-back:visible').click();
      await preview.waitFor({ state: 'hidden', timeout });
    }
    if (!await page.locator('#importar-servicio-web-list').isVisible()) fail('IMPORT_E2E_LAYOUT_REVIEW_LIST_NOT_RESTORED');
    await page.locator('#importar-servicio-web-close').click();
    await modal.waitFor({ state: 'hidden', timeout }).catch(() => fail('IMPORT_E2E_LAYOUT_REVIEW_CLOSE_INVALID'));
    return Object.freeze({
      codes: Object.freeze({ layoutReview: 'CONFIRMED', cellContainment: 'CONFIRMED', internalScroll: 'CONFIRMED', preparationActions: 'CONFIRMED', previewNavigation: 'CONFIRMED', mutations: 'NOT_OBSERVED' }),
      count: layout.rows,
      latenciesMs: Object.freeze([])
    });
  } finally {
    page.off('request', onRequest);
    await page.close().catch(() => {});
  }
}

async function inspectWorkflowSession(options) {
  if (options.plan.scenario.expectations.includes('registro-ruta-sii-ui')) {
    return inspectRegistroRutaSiiUi(options);
  }
  if (options.plan.scenario.expectations.includes('manual-layout-review')) {
    return inspectEnlaseLayoutReview(options);
  }
  if (options.plan.scenario.expectations.includes('manual-visual-execution')) {
    return inspectEnlaseManualVisual(options);
  }
  if (options.plan.scenario.expectations.includes('explicit-assignment-ui')) {
    return inspectEnlaseAssignmentUi(options);
  }
  return inspectImportPreviewUi(options);
}

async function inspectRegistroRutaSiiUi({ context, plan }) {
  const page = await context.newPage();
  const timeout = Math.min(plan.profile.budgetMs, 60000);
  const mutationPattern = /WebServiceWorkflow\.asmx\/Service_registro_tarea_ruta_sii(?:\?|$)/i;
  let mutationRequests = 0;
  let navigationCount = 0;
  let documentUrl = null;
  const onRequest = (request) => { if (request.method() === 'POST' && mutationPattern.test(request.url())) mutationRequests += 1; };
  const onNavigation = (frame) => {
    if (frame !== page.mainFrame() || documentUrl === null) return;
    if (normalizeDocumentUrl(frame.url()) !== documentUrl) navigationCount += 1;
  };
  page.on('request', onRequest);
  page.on('framenavigated', onNavigation);
  try {
    await page.goto(new URL('workflow/WebFormGestionFlujoTrabajoCamaras.aspx', plan.profile.baseUrl).toString(), { waitUntil: 'domcontentloaded', timeout });
    documentUrl = normalizeDocumentUrl(page.url());
    navigationCount = 0;
    const receipt = plan.profile.receipt;
    const prefix = page.locator('#DropDownList_ante_pone_rut');
    const number = page.locator('#TextBox_recibo_caja_rut');
    const query = page.locator('#Button_consultar_recibo_sii_rut');
    const register = page.locator('#Button_registro_actividad_ruta');
    const barcode = page.locator('#TextBox_codigo_barras_ruta');
    const procedure = page.locator('#DropDownList_tramites_rut');
    const activity = page.locator('#DropDownList_actividades_ruta');
    const routeTab = page.locator('#util_sii_registro_tarea_ruta a');
    const routePanel = page.locator('#registro_ruta');

    const routePermission = await page.evaluate(async () => {
      try {
        const response = await fetch('../webservice/WebServiceWorkflow.asmx/Service_Solicita_permisos_usuario_workflow_intgracion_sii', {
          method: 'POST', credentials: 'same-origin', headers: { 'Content-Type': 'application/json; charset=utf-8' },
          body: JSON.stringify({ parameter: 0 })
        });
        if (!response.ok) return 'HTTP_FAILURE';
        const envelope = await response.json();
        const row = envelope && Array.isArray(envelope.d) ? envelope.d[0] : null;
        if (!row) return 'MALFORMED';
        if (row.Error_gestion !== 'YES') {
          const error = String(row.Error_gestion || '');
          if (/usuario sin permisos/i.test(error)) return 'NO_PERMISSION_ROW';
          if (/error funcion|inconsistencia|inonistencia/i.test(error)) return 'QUERY_FAILURE';
          return 'SERVICE_REJECTED';
        }
        if (!row.permisos_int_sii) return 'MALFORMED';
        return Number(row.permisos_int_sii.util_sii_registro_tarea_ruta) === 1 ? 'AUTHORIZED' : 'FORBIDDEN';
      } catch (_) {
        return 'MALFORMED';
      }
    });
    if (['FORBIDDEN', 'NO_PERMISSION_ROW'].includes(routePermission)) fail('REGISTRO_RUTA_SII_E2E_ACCOUNT_FORBIDDEN');
    if (routePermission === 'QUERY_FAILURE') fail('REGISTRO_RUTA_SII_E2E_PERMISSION_QUERY_FAILED');
    if (routePermission === 'SERVICE_REJECTED') fail('REGISTRO_RUTA_SII_E2E_PERMISSION_REJECTED');
    if (routePermission === 'HTTP_FAILURE') fail('REGISTRO_RUTA_SII_E2E_PERMISSION_HTTP_FAILED');
    if (routePermission !== 'AUTHORIZED') fail('REGISTRO_RUTA_SII_E2E_PERMISSION_UNAVAILABLE');

    await routeTab.waitFor({ state: 'visible', timeout })
      .catch(() => fail('REGISTRO_RUTA_SII_E2E_ROUTE_TAB_UNAVAILABLE'));
    await routeTab.click();
    await routePanel.waitFor({ state: 'visible', timeout })
      .catch(() => fail('REGISTRO_RUTA_SII_E2E_ROUTE_PANEL_UNAVAILABLE'));

    const consult = async () => {
      await prefix.selectOption({ label: receipt[0] });
      await number.fill(receipt.slice(1));
      await query.click();
      await page.waitForFunction(() => {
        const barcodeValue = document.querySelector('#TextBox_codigo_barras_ruta')?.value || '';
        const procedureValue = Number(document.querySelector('#DropDownList_tramites_rut')?.value || 0);
        return barcodeValue.trim().length > 0 && procedureValue > 0;
      }, null, { timeout }).catch(() => fail('REGISTRO_RUTA_SII_E2E_QUERY_CONTEXT_UNAVAILABLE'));
    };

    await consult();
    const changedDigit = receipt.endsWith('9') ? '8' : '9';
    await number.fill(receipt.slice(1, -1) + changedDigit);
    await register.click();
    await page.waitForTimeout(200);
    if (mutationRequests !== 0) fail('REGISTRO_RUTA_SII_E2E_STALE_CONTEXT_MUTATED');

    await consult();
    const zeroOption = activity.locator('option[value="0"]');
    if (await zeroOption.count() !== 1) fail('REGISTRO_RUTA_SII_E2E_EMPTY_ACTIVITY_UNAVAILABLE');
    await activity.selectOption('0');
    await register.click();
    await page.waitForTimeout(200);
    if (mutationRequests !== 0) fail('REGISTRO_RUTA_SII_E2E_EMPTY_ACTIVITY_MUTATED');

    await activity.selectOption(String(plan.profile.activityId));
    const procedureId = await procedure.inputValue();
    if (!/^\d+$/.test(procedureId) || Number(procedureId) <= 0) fail('REGISTRO_RUTA_SII_E2E_PROCEDURE_INVALID');
    const responsePromise = page.waitForResponse((response) => mutationPattern.test(response.url()), { timeout });
    await register.click();
    const response = await responsePromise.catch(() => fail('REGISTRO_RUTA_SII_E2E_RESPONSE_UNAVAILABLE'));
    if (!response.ok()) fail('REGISTRO_RUTA_SII_E2E_RESPONSE_FAILED');
    const envelope = await response.json().catch(() => fail('REGISTRO_RUTA_SII_E2E_RESPONSE_INVALID'));
    const resultCode = envelope?.d?.[0]?.error_result;
    if (!['YES', 'REGISTERED_RELATION_PENDING'].includes(resultCode)) fail('REGISTRO_RUTA_SII_E2E_REGISTRATION_REJECTED');
    await page.locator('#error_div_registro_ruta').waitFor({ state: 'visible', timeout }).catch(() => fail('REGISTRO_RUTA_SII_E2E_CONFIRMATION_UNAVAILABLE'));
    const confirmation = String(await page.locator('#error_div_registro_ruta').textContent() || '');
    if (!/registrad/i.test(confirmation)) fail('REGISTRO_RUTA_SII_E2E_CONFIRMATION_INVALID');
    if (navigationCount !== 0 || mutationRequests !== 1) fail('REGISTRO_RUTA_SII_E2E_UI_CONTRACT_INVALID');

    const retry = await page.evaluate(async ({ recibo, idActividad, idTramite }) => {
      const response = await fetch('../webservice/WebServiceWorkflow.asmx/Service_registro_tarea_ruta_sii', {
        method: 'POST', credentials: 'same-origin', headers: { 'Content-Type': 'application/json; charset=utf-8' },
        body: JSON.stringify({ parameter: { recibo, id_tramite: Number(idTramite), id_actividad: Number(idActividad) } })
      });
      return response.json();
    }, { recibo: receipt, idActividad: plan.profile.activityId, idTramite: procedureId });
    if (!['ALREADY_REGISTERED', 'ALREADY_REGISTERED_RELATION_PENDING'].includes(retry?.d?.[0]?.error_result) || mutationRequests !== 2) {
      fail('REGISTRO_RUTA_SII_E2E_RETRY_INVALID');
    }
    return Object.freeze({
      codes: Object.freeze({
        staleContext: 'BLOCKED', emptyActivity: 'BLOCKED', registration: resultCode,
        retry: retry.d[0].error_result, relationMode: retry.d[0].error_result === 'YES' ? 'confirmed' : 'pending', navigation: 'NOT_OBSERVED'
      }),
      count: 1,
      latenciesMs: Object.freeze([])
    });
  } finally {
    page.off('request', onRequest);
    page.off('framenavigated', onNavigation);
    await page.close().catch(() => {});
  }
}

async function inspectImportPreviewUi({ context, plan }) {
  const page = await context.newPage();
  const isEnlaseUi = plan.scenario.expectations.includes('enlase-ui');
  let previewRequests = 0;
  let preflightRequests = 0;
  let mutationRequests = 0;
  let queryRequestsObserved = 0;
  let queryBarcodeSupplied = false;
  let queryContextMismatch = false;
  const queryRoute = /\/webservice\/WebServiceImportarServicioWebModern\.asmx\/QueryItems(?:\?|$)/i;
  const onRequest = (request) => {
    if (/WebServiceImportarServicioWebModern\.asmx\/GetPreview(?:\?|$)/i.test(request.url())) previewRequests += 1;
    if (/WebServiceImportarServicioWebModern\.asmx\/PreflightImport(?:\?|$)/i.test(request.url())) preflightRequests += 1;
    if (/WebServiceImportarServicioWebModern\.asmx\/(?:CreateImportIntent|ExecuteImportIntent)(?:\?|$)/i.test(request.url())) mutationRequests += 1;
  };
  page.on('request', onRequest);
  try {
    await selectWorkflowTask(page, plan);
    const timeout = Math.min(plan.profile.budgetMs, 60000);
    await page.route(queryRoute, async (route) => {
      let payload;
      try { payload = route.request().postDataJSON(); } catch { payload = null; }
      if (!payload?.request || typeof payload.request !== 'object' || Array.isArray(payload.request)) {
        await route.abort('blockedbyclient');
        return;
      }
      queryRequestsObserved += 1;
      if (payload.request.CodigoBarras) {
        queryBarcodeSupplied = true;
        if (String(payload.request.CodigoBarras) !== plan.profile.codigoBarras) queryContextMismatch = true;
      }
      await route.continue();
    });
    const trigger = page.locator(isEnlaseUi ? '#a_adj_service_web' : '#ctw-document-action-service');
    const triggerToggle = page.locator('.ctw-document-more-toggle:visible').first();
    if (await trigger.count() !== 1) fail('IMPORT_E2E_ENLASE_UI_TRIGGER_UNAVAILABLE');
    if (await trigger.getAttribute('data-import-modern-active') !== 'true') fail('IMPORT_E2E_ENLASE_UI_BOOTSTRAP_INACTIVE');
    if (await trigger.getAttribute('data-import-modern-bound') !== 'true') fail('IMPORT_E2E_ENLASE_UI_BINDING_UNAVAILABLE');
    if (isEnlaseUi) {
      if ((await trigger.getAttribute('data-import-capability') || '').toUpperCase() !== 'ANEXOS_RADICADO_ENLASE') {
        fail('IMPORT_E2E_ENLASE_UI_CAPABILITY_INVALID');
      }
    } else {
      await triggerToggle.click();
    }
    await trigger.waitFor({ state: 'visible', timeout });
    await trigger.click();
    const modal = page.locator('#importar-servicio-web-modal');
    await modal.waitFor({ state: 'visible', timeout });
    try {
      await page.waitForFunction(() => {
        const state = document.querySelector('#importar-servicio-web-modal')?.getAttribute('data-import-state');
        return ['resultados', 'vacio', 'error'].includes(state);
      }, null, { timeout });
    } catch {
      fail('IMPORT_E2E_PREVIEW_UI_RESULTS_UNAVAILABLE');
    }
    if (queryRequestsObserved > 1 || queryBarcodeSupplied || queryContextMismatch) {
      fail('IMPORT_E2E_PREVIEW_UI_QUERY_CONTEXT_INVALID');
    }
    if (await modal.getAttribute('data-import-state') !== 'resultados') {
      const publicCode = await page.locator('#importar-servicio-web-status').textContent()
        .then((value) => String(value || '').match(/\b[A-Z][A-Z0-9_]{2,50}\b/)?.[0] || 'RESULTS_EMPTY');
      fail(`IMPORT_E2E_PREVIEW_UI_${publicCode}`);
    }
    await page.setViewportSize({ width: 760, height: 900 });
    const tableScroll = page.locator('.importar-servicio-web-sii__table-scroll');
    await tableScroll.waitFor({ state: 'visible', timeout });
    const responsive = await tableScroll.evaluate((element) => {
      const region = element.getBoundingClientRect();
      const dialog = element.closest('[role="dialog"]')?.getBoundingClientRect();
      const style = getComputedStyle(element);
      return {
        role: element.getAttribute('role'), tabIndex: element.tabIndex,
        overflowX: style.overflowX, overflowY: style.overflowY,
        insideDialog: Boolean(dialog) && region.left >= dialog.left - 1 && region.right <= dialog.right + 1,
        insideViewport: region.left >= -1 && region.right <= window.innerWidth + 1 && region.bottom <= window.innerHeight + 1
      };
    });
    if (responsive.role !== 'region' || responsive.tabIndex !== 0 || responsive.overflowX !== 'auto' ||
        responsive.overflowY !== 'auto' || !responsive.insideDialog || !responsive.insideViewport) {
      fail('IMPORT_E2E_PREPARATION_UI_RESPONSIVE_INVALID');
    }

    const preparationRows = page.locator('[data-import-prepare="true"]:visible');
    const importablePreparationRows = page.locator('[data-import-prepare="true"]:visible:not([disabled])');
    if (await preparationRows.count() === 0) fail('IMPORT_E2E_PREPARATION_UI_ACTIONS_UNAVAILABLE');
    if (await importablePreparationRows.count() === 0) fail('IMPORT_E2E_PREPARATION_UI_NO_IMPORTABLE_ITEMS');
    const prepareButton = importablePreparationRows.first();
    await prepareButton.click();
    const preparation = page.locator('#importar-servicio-web-preparation');
    if (!await preparation.isVisible()) {
      const preparationStatus = await page.locator('#importar-servicio-web-status').textContent()
        .then((value) => String(value || '').trim());
      if (preparationStatus === 'No hay catálogo autorizado para preparar la importación.') {
        fail('IMPORT_E2E_PREPARATION_UI_CATALOG_UNAVAILABLE');
      }
      fail('IMPORT_E2E_PREPARATION_UI_PANEL_UNAVAILABLE');
    }
    await preparation.waitFor({ state: 'visible', timeout });
    const preparationTitle = page.locator('#importar-servicio-web-preparation-title');
    if (await preparationTitle.evaluate((element) => document.activeElement === element) !== true) {
      fail('IMPORT_E2E_PREPARATION_UI_FOCUS_INVALID');
    }
    const preparationConfirm = page.locator('#importar-servicio-web-preparation-confirm');
    const documentTypeOptional = await page.locator('.importar-servicio-web__document-type-optional').count() > 0;
    const documentType = page.locator('[data-import-document-type]').first();
    if (!documentTypeOptional) {
      const options = await documentType.locator('option').count();
      if (options < 2) fail('IMPORT_E2E_PREPARATION_UI_CATALOG_UNAVAILABLE');
      if (Number(await documentType.inputValue()) !== Number(plan.profile.documentTypeId)) fail('IMPORT_E2E_PREPARATION_UI_DEFAULT_TYPE_INVALID');
    } else if (await page.locator('[data-import-document-type]').count() !== 0) {
      fail('IMPORT_E2E_PREPARATION_UI_OPTIONAL_TYPE_INVALID');
    }
    await preparationConfirm.waitFor({ state: 'visible', timeout });
    await page.waitForFunction(() => {
      const confirm = document.querySelector('#importar-servicio-web-preparation-confirm');
      return confirm && confirm.disabled === false;
    }, null, { timeout }).catch(() => fail('IMPORT_E2E_PREPARATION_UI_PLAN_UNAVAILABLE'));
    if (preflightRequests !== 1 || mutationRequests !== 0) fail('IMPORT_E2E_PREPARATION_UI_REQUESTS_INVALID');
    await page.locator('#importar-servicio-web-preparation-cancel').click();
    if (await preparation.isVisible() || await prepareButton.evaluate((element) => document.activeElement === element) !== true) {
      fail('IMPORT_E2E_PREPARATION_UI_CONTEXT_NOT_RESTORED');
    }

    let multiplePreparation = 'INSUFFICIENT_ITEMS';
    const selectable = page.locator('[data-import-select="true"]:visible:not([disabled])');
    const selectableCount = await selectable.count();
    if (plan.profile.sampleSize >= 2 && selectableCount < 2) fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_ITEMS_UNAVAILABLE');
    if (selectableCount >= 2) {
      await selectable.nth(0).check();
      const selectAll = page.locator('[data-import-select-all="true"]');
      if (await selectAll.isChecked() || await selectAll.evaluate((element) => element.indeterminate) !== true) fail('IMPORT_E2E_PREPARATION_UI_SELECT_ALL_PARTIAL_INVALID');
      await selectAll.check();
      if (await selectable.evaluateAll((elements) => elements.some((element) => !element.checked))) fail('IMPORT_E2E_PREPARATION_UI_SELECT_ALL_INVALID');
      await selectAll.uncheck();
      if (await selectable.evaluateAll((elements) => elements.some((element) => element.checked))) fail('IMPORT_E2E_PREPARATION_UI_DESELECT_ALL_INVALID');
      await selectAll.check();
      const prepareSelected = page.locator('[data-import-prepare-selected="true"]:visible:not([disabled])');
      await prepareSelected.click();
      await preparation.waitFor({ state: 'visible', timeout });
      const multipleDocumentTypes = page.locator('[data-import-document-type]');
      if ((!documentTypeOptional && await multipleDocumentTypes.count() !== selectableCount) ||
          (documentTypeOptional && await multipleDocumentTypes.count() !== 0)) fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_INVALID');
      if (!documentTypeOptional && await multipleDocumentTypes.evaluateAll((elements, expected) => elements.some((element) => Number(element.value) !== Number(expected)), plan.profile.documentTypeId)) fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_DEFAULT_TYPE_INVALID');
      await page.waitForFunction(() => {
        const confirm = document.querySelector('#importar-servicio-web-preparation-confirm');
        return confirm && confirm.disabled === false;
      }, null, { timeout }).catch(() => fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_PLAN_UNAVAILABLE'));
      if (preflightRequests !== 2 || mutationRequests !== 0) fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_REQUESTS_INVALID');
      await page.locator('#importar-servicio-web-preparation-cancel').click();
      if (await prepareSelected.evaluate((element) => document.activeElement === element) !== true) {
        fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_CONTEXT_NOT_RESTORED');
      }
      multiplePreparation = 'CONFIRMED';
    }
    const previewButton = page.locator('[data-import-preview="true"]:visible').first();
    await previewButton.waitFor({ state: 'visible', timeout });
    await previewButton.click();
    const panel = page.locator('#importar-servicio-web-preview');
    await page.locator('#importar-servicio-web-preview[data-preview-state="disponible"], #importar-servicio-web-preview[data-preview-state="formato-no-visualizable"]').waitFor({ state: 'visible', timeout });
    if (previewRequests !== 1) fail('IMPORT_E2E_PREVIEW_UI_REQUEST_COUNT_INVALID');
    if (await page.locator('#importar-servicio-web-preview-title').evaluate((element) => document.activeElement === element) !== true) {
      fail('IMPORT_E2E_PREVIEW_UI_FOCUS_INVALID');
    }
    await page.evaluate(() => window.dispatchEvent(new Event('focus')));
    await page.waitForTimeout(100);
    if (previewRequests !== 1) fail('IMPORT_E2E_PREVIEW_UI_DUPLICATE_REQUEST');
    await page.locator('#importar-servicio-web-preview-back').click();
    if (await previewButton.evaluate((element) => document.activeElement === element) !== true || await panel.isVisible()) {
      fail('IMPORT_E2E_PREVIEW_UI_CONTEXT_NOT_RESTORED');
    }
    await page.locator('#importar-servicio-web-close').click();
    const focusRestored = await page.evaluate((enlase) => {
      const active = document.activeElement;
      return active?.id === (enlase ? 'a_adj_service_web' : 'ctw-document-action-service') ||
        (!enlase && active?.classList.contains('ctw-document-more-toggle'));
    }, isEnlaseUi);
    if (!focusRestored || await modal.isVisible()) {
      fail('IMPORT_E2E_PREVIEW_UI_CLOSE_INVALID');
    }
    if (mutationRequests !== 0) fail('IMPORT_E2E_PREPARATION_UI_MUTATION_OBSERVED');
    return Object.freeze({
      codes: Object.freeze({ uiPreview: 'CONFIRMED', uiFocus: 'CONFIRMED', uiSingleFetch: 'CONFIRMED',
        uiResponsiveTable: 'CONFIRMED', uiIndividualPreparation: 'CONFIRMED', uiMultiplePreparation: multiplePreparation,
        uiPreparationMutation: 'NOT_OBSERVED' }),
      count: 1,
      latenciesMs: Object.freeze([])
    });
  } finally {
    await page.unroute(queryRoute).catch(() => {});
    page.off('request', onRequest);
    await page.close();
  }
}

async function readWorkflowControl({ control, taskId, environment }) {
  try {
    const prefix = control.source === 'docuarchi' ? 'DOC87_DA_E2E' : (control.id.startsWith('registro-ruta-sii-') ? 'DOC87_E2E' : 'NOTES_E2E');
    return await queryFingerprint(control.query, taskId, environment, prefix);
  } catch (error) {
    const message = String(error?.message || '');
    if (/tabla requerida/i.test(message)) fail('E2E_PLATFORM_CONTROL_TABLE_UNAVAILABLE');
    if (/columna no disponible/i.test(message)) fail('E2E_PLATFORM_CONTROL_COLUMN_UNAVAILABLE');
    if (/no admite la forma/i.test(message)) fail('E2E_PLATFORM_CONTROL_QUERY_UNSUPPORTED');
    if (/abrir el control ODBC/i.test(message)) fail('E2E_PLATFORM_CONTROL_OPEN_FAILED');
    fail('E2E_PLATFORM_CONTROL_FAILED');
  }
}

async function configureTemporaryScenario(plan) {
  if (!plan.scenario.expectations.includes('temporary-feature-gate')) return async () => {};
  const webConfigPath = path.join(repositoryRoot, 'Web.config');
  const original = await fs.readFile(webConfigPath, 'utf8');
  if (/WorkflowCentroTrabajoModernActive|WorkflowCentroTrabajoModernUsers|WorkflowCentroTrabajoModernGroups/i.test(original)) {
    fail('E2E_PLATFORM_GATE_INTEGRITY_FAILED');
  }
  let configured = original;
  if (plan.scenario.expectations.includes('secure-preview-ui')) {
    if (!/<add key="ImportarServicioWebProviderId" value="(?:|INTEGRACIONSII)"\s*\/>/i.test(original)) {
      fail('E2E_PLATFORM_PROVIDER_INTEGRITY_FAILED');
    }
    configured = configured.replace(/(<add key="ImportarServicioWebProviderId" value=")("\s*\/>)/i, '$1INTEGRACIONSII$2');
  }
  if (plan.profile.previewExpiryMinutes === 1) {
    configured = configured.replace(/(<add key="ImportarServicioWebPreviewTtlMinutes" value=")\d+("\s*\/>)/i,
      (_match, prefix, suffix) => `${prefix}1${suffix}`);
  }
  if (plan.scenario.expectations.includes('secure-preview-ui') &&
      !/<add key="ImportarServicioWebProviderId" value="INTEGRACIONSII"\s*\/>/i.test(configured)) {
    fail('E2E_PLATFORM_PROVIDER_ENABLE_FAILED');
  }
  if (configured !== original) await fs.writeFile(webConfigPath, configured, 'utf8');
  let restored = false;
  return async () => {
    if (restored) return;
    await fs.writeFile(webConfigPath, original, 'utf8');
    restored = true;
  };
}

async function waitForApplicationReload(plan) {
  const api = await request.newContext({ ignoreHTTPSErrors: plan.profile.ignoreHttpsErrors === true });
  try {
    const login = new URL('gestor.aspx', plan.profile.baseUrl).toString();
    let consecutiveSuccesses = 0;
    for (let attempt = 0; attempt < 6; attempt += 1) {
      try {
        const response = await api.get(login, { timeout: Math.min(plan.profile.budgetMs, 5000) });
        consecutiveSuccesses = response.ok() ? consecutiveSuccesses + 1 : 0;
        if (consecutiveSuccesses >= 2) return;
      } catch {
        consecutiveSuccesses = 0;
      }
      await new Promise((resolve) => setTimeout(resolve, 1000));
    }
    fail('E2E_PLATFORM_RELOAD_STABILIZATION_FAILED');
  } finally {
    await api.dispose();
  }
}

async function main() {
  const parsedArguments = parseArguments(process.argv.slice(2));
  const profile = await loadProfile(parsedArguments.profilePath);
  if (profile.scenarioId !== parsedArguments.scenarioId) fail('E2E_PLATFORM_SCENARIO_PROFILE_MISMATCH');
  const scenario = resolveScenario(profile.scenarioId);
  const required = requiredAuthorizationsFor(scenario, profile);
  const authorizations = await collectAuthorizations(required, parsedArguments.requestedAuthorizations);
  const plan = preflightPlatform({ profile, authorizations });
  const legacyBaseline = await captureLegacyIntegrityBaseline({ root: repositoryRoot });
  const temporaryDirectory = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-'));
  const secrets = plan.scenario.requiredSecrets.length > 0 ? await collectSecrets(plan) : {};
  let restoreConfiguration = async () => {};
  try {
    restoreConfiguration = await configureTemporaryScenario(plan);
    await waitForApplicationReload(plan);
    const outcome = await executePlatformRun({
    profile,
    authorizations,
    temporaryDirectory,
    collectSecrets: async () => secrets,
    createBrowser: async (selectedProfile) => chromium.launch({ ...(selectedProfile.browser || {}), headless: !plan.scenario.expectations.some((expectation) => ['manual-visual-execution', 'manual-layout-review'].includes(expectation)) }),
    createSession: async ({ browser, plan: currentPlan, environment }) => {
      const context = await createAuthenticatedWorkflowSession(browser, {
        baseUrl: currentPlan.profile.baseUrl,
        environment,
        moduleEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_MODULE',
        userEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_USER',
        passwordEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_PASSWORD',
        ignoreHTTPSErrors: currentPlan.profile.ignoreHttpsErrors
      });
      return initializeWorkflowContext(context, currentPlan);
    },
    createClient,
    invoke: (requestOptions) => invokeNotes({ ...requestOptions, plan }),
    consumePreview: consumeImportPreview,
    inspectSession: inspectWorkflowSession,
    readControl: readWorkflowControl,
      writeEvidence,
      assertIntegrity: async (options) => {
        await restoreConfiguration();
        await assertPlatformIntegrity({ ...options, legacyBaseline });
      }
    });
    console.log(`La plataforma E2E terminó correctamente (${plan.scenario.id}); controles=${outcome.controls.checked}; sinCambios=${outcome.controls.unchanged === true ? 'SI' : 'NO'}. Evidencia saneada disponible.`);
  } finally {
    await restoreConfiguration();
  }
}

main().catch((error) => {
  const code = /^[A-Z0-9_]{3,120}$/.test(error?.code || '') ? error.code : 'E2E_PLATFORM_RUNNER_FAILED';
  console.error(`La plataforma E2E se detuvo de forma segura (${code}). No se mostraron valores sensibles.`);
  if (typeof error?.diagnostic === 'string' && error.diagnostic) console.error(`LEGACY_FUNCTION_RESPONSE: ${error.diagnostic}`);
  process.exitCode = 2;
});

module.exports = {
  collectAuthorizations,
  inspectEnlaseAssignmentUi,
  inspectImportPreviewUi,
  inspectWorkflowSession,
  parseArguments
};

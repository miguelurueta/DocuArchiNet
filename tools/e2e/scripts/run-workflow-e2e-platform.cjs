'use strict';

const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { chromium, request } = require('playwright');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { queryFingerprint } = require('./support/doc32-e2e-odbc.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const {
  executePlatformRun,
  assertPlatformIntegrity,
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
    const selectedTask = page.locator('#Hidden_id_tarea_selecionada');
    const expectedTaskId = String(taskId);
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
      await selectCommand.click();
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
  const page = await context.newPage();
  try {
    await selectWorkflowTask(page, plan);
  } finally {
    await page.close();
  }
  return context;
}

async function inspectImportPreviewUi({ context, plan }) {
  const page = await context.newPage();
  let previewRequests = 0;
  let preflightRequests = 0;
  let mutationRequests = 0;
  let queryRequestsObserved = 0;
  let queryContextInjected = 0;
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
      if (!payload.request.CodigoBarras) {
        payload.request.CodigoBarras = plan.profile.codigoBarras;
        queryContextInjected += 1;
      } else if (String(payload.request.CodigoBarras) !== plan.profile.codigoBarras) {
        queryContextMismatch = true;
        await route.abort('blockedbyclient');
        return;
      }
      await route.continue({ postData: JSON.stringify(payload) });
    });
    const trigger = page.locator('#ctw-document-action-service');
    const triggerToggle = page.locator('.ctw-document-more-toggle:visible').first();
    if (await trigger.getAttribute('data-import-modern-active') !== 'true' ||
        await trigger.getAttribute('data-import-modern-bound') !== 'true') fail('IMPORT_E2E_PREVIEW_UI_UNAVAILABLE');
    await triggerToggle.click();
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
    if (queryRequestsObserved > 1 || queryContextInjected > 1 || queryContextMismatch) {
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

    const prepareButton = page.locator('[data-import-prepare="true"]:visible:not([disabled])').first();
    await prepareButton.waitFor({ state: 'visible', timeout });
    await prepareButton.click();
    const preparation = page.locator('#importar-servicio-web-preparation');
    await preparation.waitFor({ state: 'visible', timeout });
    const preparationTitle = page.locator('#importar-servicio-web-preparation-title');
    if (await preparationTitle.evaluate((element) => document.activeElement === element) !== true) {
      fail('IMPORT_E2E_PREPARATION_UI_FOCUS_INVALID');
    }
    const preparationConfirm = page.locator('#importar-servicio-web-preparation-confirm');
    if (!await preparationConfirm.isDisabled()) fail('IMPORT_E2E_PREPARATION_UI_PREMATURE_CONFIRM');
    const documentType = page.locator('[data-import-document-type]').first();
    const options = await documentType.locator('option').count();
    if (options < 2) fail('IMPORT_E2E_PREPARATION_UI_CATALOG_UNAVAILABLE');
    await documentType.selectOption({ index: 1 });
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
      await selectable.nth(1).check();
      const prepareSelected = page.locator('[data-import-prepare-selected="true"]:visible:not([disabled])');
      await prepareSelected.click();
      await preparation.waitFor({ state: 'visible', timeout });
      if (await page.locator('[data-import-document-type]').count() !== 2) fail('IMPORT_E2E_PREPARATION_UI_MULTIPLE_INVALID');
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
    const focusRestored = await page.evaluate(() => {
      const active = document.activeElement;
      return active?.id === 'ctw-document-action-service' || active?.classList.contains('ctw-document-more-toggle');
    });
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
    return await queryFingerprint(control.query, taskId, environment, 'NOTES_E2E');
  } catch (error) {
    const message = String(error?.message || '');
    if (/tabla requerida/i.test(message)) fail('E2E_PLATFORM_CONTROL_TABLE_UNAVAILABLE');
    if (/columna no disponible/i.test(message)) fail('E2E_PLATFORM_CONTROL_COLUMN_UNAVAILABLE');
    if (/no admite la forma/i.test(message)) fail('E2E_PLATFORM_CONTROL_QUERY_UNSUPPORTED');
    if (/abrir el control ODBC/i.test(message)) fail('E2E_PLATFORM_CONTROL_OPEN_FAILED');
    fail('E2E_PLATFORM_CONTROL_FAILED');
  }
}

async function enableTemporaryGate(plan) {
  if (!plan.scenario.expectations.includes('temporary-feature-gate')) return async () => {};
  const webConfigPath = path.join(repositoryRoot, 'Web.config');
  const original = await fs.readFile(webConfigPath, 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(original) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(original) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(original)) fail('E2E_PLATFORM_GATE_INTEGRITY_FAILED');
  let enabled = original.replace(/(<add key="WorkflowCentroTrabajoModernActive" value=")false("\s*\/>)/i, '$1true$2');
  if (plan.scenario.expectations.includes('secure-preview-ui')) {
    if (!/<add key="ImportarServicioWebProviderId" value=""\s*\/>/i.test(original)) {
      fail('E2E_PLATFORM_PROVIDER_INTEGRITY_FAILED');
    }
    enabled = enabled.replace(/(<add key="ImportarServicioWebProviderId" value=")("\s*\/>)/i, '$1INTEGRACIONSII$2');
  }
  if (plan.profile.previewExpiryMinutes === 1) {
    enabled = enabled.replace(/(<add key="ImportarServicioWebPreviewTtlMinutes" value=")\d+("\s*\/>)/i,
      (_match, prefix, suffix) => `${prefix}1${suffix}`);
  }
  if (enabled === original || (plan.scenario.expectations.includes('secure-preview-ui') &&
      !/<add key="ImportarServicioWebProviderId" value="INTEGRACIONSII"\s*\/>/i.test(enabled))) {
    fail('E2E_PLATFORM_GATE_ENABLE_FAILED');
  }
  await fs.writeFile(webConfigPath, enabled, 'utf8');
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
    for (let attempt = 0; attempt < 3; attempt += 1) {
      const response = await api.get(login, { timeout: Math.min(plan.profile.budgetMs, 60000) });
      if (!response.ok()) fail('E2E_PLATFORM_RELOAD_STABILIZATION_FAILED');
      await new Promise((resolve) => setTimeout(resolve, 1500));
    }
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
  const temporaryDirectory = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-'));
  let restoreGate = async () => {};
  try {
    restoreGate = await enableTemporaryGate(plan);
    await waitForApplicationReload(plan);
    const outcome = await executePlatformRun({
    profile,
    authorizations,
    temporaryDirectory,
    collectSecrets,
    createBrowser: async (selectedProfile) => chromium.launch(selectedProfile.browser || {}),
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
    inspectSession: inspectImportPreviewUi,
    readControl: readWorkflowControl,
      writeEvidence,
      assertIntegrity: async (options) => {
        await restoreGate();
        await assertPlatformIntegrity(options);
      }
    });
    console.log(`La plataforma E2E terminó correctamente (${plan.scenario.id}); controles=${outcome.controls.checked}; sinCambios=${outcome.controls.unchanged === true ? 'SI' : 'NO'}. Evidencia saneada disponible.`);
  } finally {
    await restoreGate();
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
  inspectImportPreviewUi,
  parseArguments
};

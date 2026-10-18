'use strict';

const fs = require('node:fs');
const fsp = require('node:fs/promises');
const path = require('node:path');
const { performance } = require('node:perf_hooks');
const { chromium } = require('@playwright/test');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const {
  loadProfile: loadGraphSaveProfile,
  runReal: runGraphSaveReal
} = require('./run-doc103-graph-save-interactive.cjs');
const {
  captureViewerMetrics,
  installPerformanceProbe,
  measureScroll,
  measureZoom,
  loadProfile: loadWorkflowPerformanceProfile,
  selectWorkflowDocument,
  selectWorkflowTask,
  waitForFirstCanvas,
  waitForViewerFrame
} = require('./run-workflow-pdf-viewer-performance-interactive.cjs');

const e2eRoot = path.resolve(__dirname, '..');

const CONSUMER_KINDS = Object.freeze([
  'workflow', 'correspondence', 'production', 'approvalExternal', 'simpleHandler'
]);
const CACHE_MODES = Object.freeze(['cold', 'warm']);
const REQUIRED_NETWORK_MBPS = Object.freeze([10, 20, 50, 100]);
const RESULT_STATES = Object.freeze(['PASSED', 'FAILED', 'BLOCKED', 'NOT_APPLICABLE']);
const SIMPLE_HANDLER_CLASSES = Object.freeze([
  'docuarchiDownload', 'docuarchiExternal', 'gestionExternal',
  'versions', 'migration', 'workflowOtherFormats'
]);
const allowedProfileFields = new Set([
  'baseUrl', 'module', 'timeoutMilliseconds', 'headless', 'browser', 'ignoreHttpsErrors',
  'iterations', 'scrollSteps', 'networkProfiles', 'cacheModes', 'consumers', 'mutation'
]);
const allowedNetworkFields = new Set(['name', 'downloadMbps', 'latencyMilliseconds', 'firstPageBudget']);
const allowedFirstPageBudgetFields = new Set([
  'maxMilliseconds', 'maxBytesBeforeCanvas', 'maxDocumentFractionBeforeCanvas', 'maxRequestsBeforeCanvas'
]);
const allowedConsumerFields = new Set([
  'kind', 'enabled', 'simpleClass', 'taskId', 'treeOrdinal', 'contextOrdinal', 'contextTitle', 'documentOrdinal', 'documentTitle', 'entryPath', 'rowSelector',
  'menuSelector', 'contextSelector', 'openSelector', 'containerFrameSelector', 'viewerFrameSelector'
]);
const allowedMutationFields = new Set(['enabled', 'consumer', 'profile']);
const forbiddenEvidence = /https?:\/\/|[A-Za-z]:\\|(?:cookie|token|password|contrase(?:ñ|n)a|credential|connection|string|payload|documentTitle|taskId|baseUrl|entryPath|selector|query)/i;

const adapterRegistry = Object.freeze({
  workflow: Object.freeze({ viewer: true, scrollZoom: true, mutation: true, entry: 'workflow-main' }),
  correspondence: Object.freeze({ viewer: false, scrollZoom: false, mutation: false, entry: 'correspondence-readonly-handler' }),
  production: Object.freeze({ viewer: true, scrollZoom: true, mutation: false, entry: 'production-gestor' }),
  approvalExternal: Object.freeze({ viewer: true, scrollZoom: true, mutation: false, entry: 'approval-authenticated' }),
  simpleHandler: Object.freeze({ viewer: false, scrollZoom: false, mutation: false, entry: 'handler-consumer' })
});

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function assertPlainObject(value, code) {
  if (!value || typeof value !== 'object' || Array.isArray(value)) fail(code);
  return value;
}

function rejectUnknown(value, allowed, code) {
  if (Object.keys(value).some(key => !allowed.has(key))) fail(code);
}

function positiveInteger(value, code, maximum = Number.MAX_SAFE_INTEGER) {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number < 1 || number > maximum) fail(code);
  return number;
}

function boundedFraction(value, code) {
  const number = Number(value);
  if (!Number.isFinite(number) || number <= 0 || number >= 1) fail(code);
  return number;
}

function parseFirstPageBudget(input) {
  assertPlainObject(input, 'DOC105_FIRST_PAGE_BUDGET_REQUIRED');
  rejectUnknown(input, allowedFirstPageBudgetFields, 'DOC105_FIRST_PAGE_BUDGET_FIELD_NOT_ALLOWED');
  return Object.freeze({
    maxMilliseconds: positiveInteger(input.maxMilliseconds, 'DOC105_FIRST_PAGE_TIME_BUDGET_INVALID', 600000),
    maxBytesBeforeCanvas: positiveInteger(input.maxBytesBeforeCanvas, 'DOC105_FIRST_PAGE_BYTES_BUDGET_INVALID'),
    maxDocumentFractionBeforeCanvas: boundedFraction(
      input.maxDocumentFractionBeforeCanvas,
      'DOC105_FIRST_PAGE_FRACTION_BUDGET_INVALID'
    ),
    maxRequestsBeforeCanvas: positiveInteger(input.maxRequestsBeforeCanvas, 'DOC105_FIRST_PAGE_REQUEST_BUDGET_INVALID', 10000)
  });
}

function safeText(value, code, maximum = 120) {
  if (typeof value !== 'string') fail(code);
  const result = value.trim();
  if (!result || result.length > maximum || /[\r\n\t]/.test(result)) fail(code);
  return result;
}

function safeRelativePath(value, code) {
  const result = safeText(value, code, 240).replace(/\\/g, '/');
  if (result.startsWith('/') || result.includes('..') || /:|[?#]/.test(result) || !/^[A-Za-z0-9_./-]+\.aspx$/i.test(result)) fail(code);
  return result;
}

function safeSelector(value, code) {
  const result = safeText(value, code, 240);
  if (/[\r\n<>]/.test(result)) fail(code);
  return result;
}

function safeRuntimeProfile(value, code) {
  const relative = safeText(value, code, 160).replace(/\\/g, '/');
  if (relative.startsWith('/') || relative.includes('..') || !/^profiles\/[A-Za-z0-9._-]+\.runtime\.json$/i.test(relative)) fail(code);
  return relative;
}

function parseNetworkProfile(input) {
  assertPlainObject(input, 'DOC105_NETWORK_PROFILE_INVALID');
  rejectUnknown(input, allowedNetworkFields, 'DOC105_NETWORK_PROFILE_FIELD_NOT_ALLOWED');
  const downloadMbps = positiveInteger(input.downloadMbps, 'DOC105_NETWORK_DOWNLOAD_INVALID', 1000);
  if (!REQUIRED_NETWORK_MBPS.includes(downloadMbps)) fail('DOC105_NETWORK_DOWNLOAD_NOT_REQUIRED');
  return Object.freeze({
    name: safeText(input.name, 'DOC105_NETWORK_NAME_INVALID', 40),
    downloadMbps,
    latencyMilliseconds: positiveInteger(input.latencyMilliseconds, 'DOC105_NETWORK_LATENCY_INVALID', 5000),
    firstPageBudget: parseFirstPageBudget(input.firstPageBudget)
  });
}

function parseConsumer(input) {
  assertPlainObject(input, 'DOC105_CONSUMER_INVALID');
  rejectUnknown(input, allowedConsumerFields, 'DOC105_CONSUMER_FIELD_NOT_ALLOWED');
  if (!CONSUMER_KINDS.includes(input.kind)) fail('DOC105_CONSUMER_KIND_INVALID');
  if (typeof input.enabled !== 'boolean') fail('DOC105_CONSUMER_ENABLED_INVALID');
  const result = { kind: input.kind, enabled: input.enabled };
  if (input.simpleClass !== undefined) {
    const simpleClass = safeText(input.simpleClass, 'DOC105_SIMPLE_HANDLER_CLASS_INVALID', 80);
    if (!SIMPLE_HANDLER_CLASSES.includes(simpleClass)) fail('DOC105_SIMPLE_HANDLER_CLASS_INVALID');
    result.simpleClass = simpleClass;
  }
  if (input.taskId !== undefined) result.taskId = positiveInteger(input.taskId, 'DOC105_TASK_INVALID');
  if (input.treeOrdinal !== undefined) result.treeOrdinal = positiveInteger(input.treeOrdinal, 'DOC105_TREE_ORDINAL_INVALID', 1000);
  if (input.contextOrdinal !== undefined) result.contextOrdinal = positiveInteger(input.contextOrdinal, 'DOC105_CONTEXT_ORDINAL_INVALID', 1000);
  if (input.contextTitle !== undefined) result.contextTitle = safeText(input.contextTitle, 'DOC105_CONTEXT_TITLE_INVALID', 255);
  if (input.documentOrdinal !== undefined) result.documentOrdinal = positiveInteger(input.documentOrdinal, 'DOC105_DOCUMENT_ORDINAL_INVALID', 1000);
  if (input.documentTitle !== undefined) result.documentTitle = safeText(input.documentTitle, 'DOC105_DOCUMENT_TITLE_INVALID', 255);
  if (input.entryPath !== undefined) result.entryPath = safeRelativePath(input.entryPath, 'DOC105_ENTRY_PATH_INVALID');
  for (const field of ['rowSelector', 'menuSelector', 'contextSelector', 'openSelector', 'containerFrameSelector', 'viewerFrameSelector']) {
    if (input[field] !== undefined) result[field] = safeSelector(input[field], `DOC105_${field.toUpperCase()}_INVALID`);
  }
  if (input.kind === 'workflow' && input.enabled && !result.taskId) fail('DOC105_WORKFLOW_TASK_REQUIRED');
  if (input.kind !== 'workflow' && input.enabled && !result.entryPath) fail('DOC105_CONSUMER_ENTRY_REQUIRED');
  if (adapterRegistry[input.kind].viewer && input.enabled && input.kind !== 'workflow' && !result.viewerFrameSelector) {
    fail('DOC105_VIEWER_FRAME_REQUIRED');
  }
  if (input.enabled && ['correspondence', 'production', 'approvalExternal'].includes(input.kind) &&
      (!result.rowSelector || !result.openSelector)) fail('DOC105_NAVIGATION_CONTRACT_REQUIRED');
  if (input.enabled && input.kind === 'approvalExternal' &&
      (!result.contextSelector || !result.containerFrameSelector)) {
    fail('DOC105_APPROVAL_ENTRY_REQUIRED');
  }
  if (input.enabled && input.kind === 'correspondence' &&
      (!result.menuSelector || !result.containerFrameSelector || !result.viewerFrameSelector)) {
    fail('DOC105_CORRESPONDENCE_ENTRY_REQUIRED');
  }
  if (input.enabled && input.kind === 'production' &&
      (!result.menuSelector || !result.containerFrameSelector || !result.contextSelector || !result.viewerFrameSelector)) {
    fail('DOC105_PRODUCTION_ENTRY_REQUIRED');
  }
  if (input.enabled && input.kind === 'simpleHandler' && !result.viewerFrameSelector) {
    fail('DOC105_SIMPLE_HANDLER_FRAME_REQUIRED');
  }
  if (input.enabled && input.kind === 'simpleHandler' && !result.simpleClass) {
    fail('DOC105_SIMPLE_HANDLER_CLASS_REQUIRED');
  }
  return Object.freeze(result);
}

function loadProfile(target) {
  let input;
  try { input = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, '')); } catch { fail('DOC105_PROFILE_INVALID'); }
  assertPlainObject(input, 'DOC105_PROFILE_INVALID');
  rejectUnknown(input, allowedProfileFields, 'DOC105_PROFILE_FIELD_NOT_ALLOWED');
  let baseUrl;
  try { baseUrl = new URL(input.baseUrl); } catch { fail('DOC105_BASE_URL_INVALID'); }
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('DOC105_BASE_URL_INVALID');
  if (input.ignoreHttpsErrors !== true && input.ignoreHttpsErrors !== false) fail('DOC105_TLS_FLAG_INVALID');
  if (input.headless !== undefined && typeof input.headless !== 'boolean') fail('DOC105_HEADLESS_INVALID');
  let browser;
  if (input.browser !== undefined) {
    assertPlainObject(input.browser, 'DOC105_BROWSER_INVALID');
    if (Object.keys(input.browser).length !== 1 || !['chrome', 'msedge'].includes(input.browser.channel)) fail('DOC105_BROWSER_INVALID');
    browser = Object.freeze({ channel: input.browser.channel });
  }
  if (!Array.isArray(input.networkProfiles) || input.networkProfiles.length !== 4) fail('DOC105_NETWORK_MATRIX_INVALID');
  const networkProfiles = input.networkProfiles.map(parseNetworkProfile);
  const speeds = [...new Set(networkProfiles.map(item => item.downloadMbps))].sort((a, b) => a - b);
  if (JSON.stringify(speeds) !== JSON.stringify(REQUIRED_NETWORK_MBPS)) fail('DOC105_NETWORK_MATRIX_INVALID');
  if (new Set(networkProfiles.map(item => item.name)).size !== networkProfiles.length) fail('DOC105_NETWORK_NAME_DUPLICATED');
  if (!Array.isArray(input.cacheModes) || input.cacheModes.length !== 2 ||
      !CACHE_MODES.every(mode => input.cacheModes.includes(mode)) || new Set(input.cacheModes).size !== 2) fail('DOC105_CACHE_MODES_INVALID');
  if (!Array.isArray(input.consumers) || input.consumers.length !== CONSUMER_KINDS.length) fail('DOC105_CONSUMER_MATRIX_INVALID');
  const consumers = input.consumers.map(parseConsumer);
  if (new Set(consumers.map(item => item.kind)).size !== CONSUMER_KINDS.length ||
      !CONSUMER_KINDS.every(kind => consumers.some(item => item.kind === kind))) fail('DOC105_CONSUMER_MATRIX_INVALID');
  if (!consumers.some(item => item.enabled)) fail('DOC105_CONSUMER_REQUIRED');
  const mutationInput = assertPlainObject(input.mutation, 'DOC105_MUTATION_INVALID');
  rejectUnknown(mutationInput, allowedMutationFields, 'DOC105_MUTATION_FIELD_NOT_ALLOWED');
  if (typeof mutationInput.enabled !== 'boolean' || mutationInput.consumer !== 'workflow') fail('DOC105_MUTATION_INVALID');
  if (mutationInput.enabled && !consumers.find(item => item.kind === 'workflow')?.enabled) fail('DOC105_MUTATION_WORKFLOW_DISABLED');
  if (mutationInput.enabled && mutationInput.profile === undefined) fail('DOC105_MUTATION_PROFILE_REQUIRED');
  const mutationProfile = mutationInput.profile === undefined
    ? undefined
    : safeRuntimeProfile(mutationInput.profile, 'DOC105_MUTATION_PROFILE_INVALID');
  return Object.freeze({
    baseUrl: baseUrl.toString(),
    module: safeText(input.module, 'DOC105_MODULE_INVALID', 80),
    timeoutMilliseconds: positiveInteger(input.timeoutMilliseconds ?? 120000, 'DOC105_TIMEOUT_INVALID', 600000),
    headless: input.headless !== false,
    browser,
    ignoreHttpsErrors: input.ignoreHttpsErrors,
    iterations: positiveInteger(input.iterations ?? 3, 'DOC105_ITERATIONS_INVALID', 10),
    scrollSteps: positiveInteger(input.scrollSteps ?? 12, 'DOC105_SCROLL_STEPS_INVALID', 100),
    networkProfiles: Object.freeze(networkProfiles),
    cacheModes: Object.freeze([...input.cacheModes]),
    consumers: Object.freeze(consumers),
    mutation: Object.freeze({ enabled: mutationInput.enabled, consumer: 'workflow', profile: mutationProfile })
  });
}

function loadWorkflowBootstrapProfile(target) {
  const workflow = loadWorkflowPerformanceProfile(target);
  return Object.freeze({
    baseUrl: workflow.baseUrl,
    module: workflow.module,
    timeoutMilliseconds: workflow.timeoutMilliseconds,
    headless: workflow.headless,
    browser: workflow.browser,
    ignoreHttpsErrors: workflow.ignoreHttpsErrors,
    iterations: workflow.iterations,
    scrollSteps: workflow.scrollSteps,
    networkProfiles: Object.freeze([
      Object.freeze({ name: 'remote-10', downloadMbps: 10, latencyMilliseconds: 100, firstPageBudget: defaultFirstPageBudget(60000) }),
      Object.freeze({ name: 'remote-20', downloadMbps: 20, latencyMilliseconds: 80, firstPageBudget: defaultFirstPageBudget(35000) }),
      Object.freeze({ name: 'remote-50', downloadMbps: 50, latencyMilliseconds: 60, firstPageBudget: defaultFirstPageBudget(20000) }),
      Object.freeze({ name: 'remote-100', downloadMbps: 100, latencyMilliseconds: 40, firstPageBudget: defaultFirstPageBudget(15000) })
    ]),
    cacheModes: Object.freeze(['cold', 'warm']),
    consumers: Object.freeze([
      Object.freeze({
        kind: 'workflow', enabled: true, taskId: workflow.taskId,
        documentOrdinal: workflow.documentOrdinal, documentTitle: workflow.documentTitle
      }),
      Object.freeze({
        kind: 'correspondence', enabled: false,
        entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', menuSelector: '#CR-GT-01_card_boton',
        containerFrameSelector: '#ContentPlacenter_ifrm_ds_',
        contextOrdinal: 1, rowSelector: '[tip_event="documento_solic_tramite"]',
        openSelector: '[tip_event="vis_doc_selecion_wf"]:has(.fa-file-pdf)', viewerFrameSelector: '#ifrm_visor_', documentOrdinal: 1
      }),
      Object.freeze({
        kind: 'production', enabled: false, entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx',
        menuSelector: '#GD-GD-14_card_boton', containerFrameSelector: '#ContentPlacenter_ifrm_ds_',
        contextSelector: '#TreeViewArchivo a', contextOrdinal: 1,
        rowSelector: '#data_grid tr[id]', openSelector: 'a[title="Visualiza archivo"]',
        viewerFrameSelector: '#Iframe_visor_externo_da_', documentOrdinal: 1
      }),
      Object.freeze({
        kind: 'approvalExternal', enabled: false, entryPath: 'Gestion/WebFormGaconsultaclasificacion.aspx',
        contextSelector: '#TreeViewEstructura a[href*="__doPostBack"]',
        contextOrdinal: 1, rowSelector: '[tip_event="ver_doc_col"][title="Con documentos"]', openSelector: '[tip_event="ver_doc"]',
        containerFrameSelector: '#Iframe_visor_externo__', viewerFrameSelector: '#ifrm_visor_', documentOrdinal: 1
      }),
      Object.freeze({
        kind: 'simpleHandler', enabled: false, entryPath: 'workflow/WebFormVisor_Otros_Formatos.aspx',
        viewerFrameSelector: '#ifrm_visor_', simpleClass: 'workflowOtherFormats',
        taskId: workflow.taskId, documentOrdinal: workflow.documentOrdinal, documentTitle: workflow.documentTitle
      })
    ]),
    mutation: Object.freeze({ enabled: false, consumer: 'workflow', profile: undefined })
  });
}

function defaultFirstPageBudget(maxMilliseconds) {
  return Object.freeze({
    maxMilliseconds,
    maxBytesBeforeCanvas: 33554432,
    maxDocumentFractionBeforeCanvas: 0.5,
    maxRequestsBeforeCanvas: 64
  });
}

function enableInventoriedConsumers(profile) {
  return Object.freeze({
    ...profile,
    timeoutMilliseconds: Math.min(profile.timeoutMilliseconds, 30000),
    consumers: Object.freeze(profile.consumers.map(consumer => Object.freeze({ ...consumer, enabled: true })))
  });
}

function selectConsumer(profile, kind) {
  if (!CONSUMER_KINDS.includes(kind)) fail('DOC105_CONSUMER_ARGUMENT_INVALID');
  if (profile.mutation.enabled && kind !== 'workflow') fail('DOC105_MUTATION_CONSUMER_FORBIDDEN');
  return Object.freeze({
    ...profile,
    consumers: Object.freeze(profile.consumers.map(consumer => Object.freeze({
      ...consumer,
      enabled: consumer.kind === kind
    })))
  });
}

function overrideProductionSelection(profile, contextTitle, documentTitle, contextPath) {
  if (contextTitle === undefined && documentTitle === undefined && contextPath === undefined) return profile;
  const production = profile.consumers.find(consumer => consumer.kind === 'production' && consumer.enabled);
  if (!production) fail('DOC105_PRODUCTION_SELECTION_FORBIDDEN');
  const safeContextTitle = contextTitle === undefined
    ? production.contextTitle
    : safeText(contextTitle, 'DOC105_CONTEXT_TITLE_INVALID', 255);
  const safeDocumentTitle = documentTitle === undefined
    ? production.documentTitle
    : safeText(documentTitle, 'DOC105_DOCUMENT_TITLE_INVALID', 255);
  let safeContextPath = production.contextPath;
  if (contextPath !== undefined) {
    if (!Array.isArray(contextPath) || contextPath.length < 1 || contextPath.length > 12) fail('DOC105_CONTEXT_PATH_INVALID');
    safeContextPath = Object.freeze(contextPath.map(value => safeText(value, 'DOC105_CONTEXT_PATH_INVALID', 255)));
  }
  return Object.freeze({
    ...profile,
    consumers: Object.freeze(profile.consumers.map(consumer => Object.freeze(consumer.kind === 'production'
      ? { ...consumer, contextTitle: safeContextTitle, documentTitle: safeDocumentTitle, contextPath: safeContextPath }
      : consumer)))
  });
}

function selectAuthenticationModule(profile, module) {
  return Object.freeze({
    ...profile,
    module: safeText(module, 'DOC105_MODULE_ARGUMENT_INVALID', 80)
  });
}

function parseContentRange(value) {
  if (typeof value !== 'string') return null;
  const match = value.trim().match(/^bytes\s+(\d+)-(\d+)\/(\d+)$/i);
  if (!match) return null;
  const start = Number(match[1]);
  const end = Number(match[2]);
  const total = Number(match[3]);
  if (![start, end, total].every(Number.isSafeInteger) || start < 0 || end < start || total <= end) return null;
  return Object.freeze({ start, end, total, length: end - start + 1 });
}

function unionIntervals(intervals = []) {
  const sorted = intervals
    .filter(item => item && Number.isSafeInteger(item.start) && Number.isSafeInteger(item.end) && item.start >= 0 && item.end >= item.start)
    .map(item => ({ start: item.start, end: item.end }))
    .sort((left, right) => left.start - right.start || left.end - right.end);
  const merged = [];
  for (const interval of sorted) {
    const previous = merged[merged.length - 1];
    if (!previous || interval.start > previous.end + 1) merged.push({ ...interval });
    else previous.end = Math.max(previous.end, interval.end);
  }
  return Object.freeze({
    intervals: Object.freeze(merged.map(Object.freeze)),
    bytes: merged.reduce((sum, item) => sum + item.end - item.start + 1, 0)
  });
}

function summarizeTransfers(exchanges = []) {
  const safe = exchanges.map(item => {
    const status = Number(item?.status);
    const range = parseContentRange(item?.contentRange);
    const contentLength = Number(item?.contentLength);
    const transferSize = Number(item?.transferSize);
    return Object.freeze({
      status: Number.isInteger(status) ? status : 0,
      range,
      contentLength: Number.isSafeInteger(contentLength) && contentLength >= 0 ? contentLength : 0,
      transferSize: Number.isSafeInteger(transferSize) && transferSize >= 0 ? transferSize : 0,
      beforeFirstCanvas: item?.beforeFirstCanvas === true
    });
  });
  const allRanges = safe.filter(item => item.status === 206 && item.range).map(item => item.range);
  const earlyRanges = safe.filter(item => item.beforeFirstCanvas && item.status === 206 && item.range).map(item => item.range);
  const totals = [...new Set(allRanges.map(item => item.total))];
  const documentLengthBytes = totals.length === 1 ? totals[0] : 0;
  const unique = unionIntervals(allRanges);
  const uniqueBeforeFirstCanvas = unionIntervals(earlyRanges);
  const physicalDeclaredBytes = safe.reduce((sum, item) => sum + item.contentLength, 0);
  const physicalTransferredBytes = safe.reduce((sum, item) => sum + item.transferSize, 0);
  const requestCountBeforeFirstCanvas = safe.filter(item => item.beforeFirstCanvas).length;
  const physicalDeclaredBytesBeforeFirstCanvas = safe
    .filter(item => item.beforeFirstCanvas)
    .reduce((sum, item) => sum + item.contentLength, 0);
  const streamLengths = safe.filter(item => item.status === 200 && item.contentLength > 0).map(item => item.contentLength);
  const effectiveDocumentLengthBytes = documentLengthBytes || (streamLengths.length ? Math.max(...streamLengths) : 0);
  const bytesBeforeFirstCanvas = allRanges.length > 0
    ? uniqueBeforeFirstCanvas.bytes
    : physicalDeclaredBytesBeforeFirstCanvas;
  return Object.freeze({
    requestCount: safe.length,
    requestCountBeforeFirstCanvas,
    rangeResponseCount: safe.filter(item => item.status === 206 && item.range).length,
    documentLengthBytes: effectiveDocumentLengthBytes,
    uniqueBytes: unique.bytes,
    uniqueBytesBeforeFirstCanvas: uniqueBeforeFirstCanvas.bytes,
    bytesBeforeFirstCanvas,
    physicalDeclaredBytes,
    physicalDeclaredBytesBeforeFirstCanvas,
    physicalTransferredBytes,
    partialBeforeFirstCanvas: effectiveDocumentLengthBytes > 0 && earlyRanges.length > 0 &&
      uniqueBeforeFirstCanvas.bytes < effectiveDocumentLengthBytes
  });
}

function evaluateFirstPageBudget(measurement, budget) {
  if (!budget) return Object.freeze({ state: 'BLOCKED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_BUDGET_REQUIRED' });
  const elapsed = Number(measurement?.firstCanvasMilliseconds || 0);
  const bytes = Number(measurement?.bytesBeforeFirstCanvas || 0);
  const total = Number(measurement?.documentLengthBytes || 0);
  const requests = Number(measurement?.requestCountBeforeFirstCanvas || 0);
  if (!(elapsed > 0) || !(bytes > 0) || !(total > 0) || !(requests > 0)) {
    return Object.freeze({ state: 'FAILED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_METRICS_UNAVAILABLE' });
  }
  if (elapsed > budget.maxMilliseconds) {
    return Object.freeze({ state: 'FAILED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_TIME_BUDGET_EXCEEDED' });
  }
  const isPreview = measurement?.firstCanvasSource === 'preview';
  if (bytes > budget.maxBytesBeforeCanvas || (!isPreview && bytes / total > budget.maxDocumentFractionBeforeCanvas)) {
    return Object.freeze({ state: 'FAILED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_BYTES_BUDGET_EXCEEDED' });
  }
  if (requests > budget.maxRequestsBeforeCanvas) {
    return Object.freeze({ state: 'FAILED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_REQUEST_BUDGET_EXCEEDED' });
  }
  return Object.freeze({ state: 'PASSED', passed: true, errorCategory: '' });
}

function evaluateConsumerTransportPolicy(metrics, transfers, firstCanvasSource,
  bytesBeforeFirstCanvas, documentLengthBytes) {
  if (firstCanvasSource !== 'preview') return metrics?.transportPolicySatisfied === true;
  const loadingMode = String(metrics?.pdfLoading?.mode || '').toLowerCase();
  return loadingMode === 'range' && Number(transfers?.rangeResponseCount || 0) > 0 &&
    Number(bytesBeforeFirstCanvas || 0) > 0 && Number(documentLengthBytes || 0) > 0 &&
    Number(bytesBeforeFirstCanvas) < Number(documentLengthBytes);
}

function networkConditions(profile) {
  return Object.freeze({
    offline: false,
    latency: profile.latencyMilliseconds,
    downloadThroughput: Math.floor(profile.downloadMbps * 1000000 / 8),
    uploadThroughput: Math.floor(profile.downloadMbps * 1000000 / 8)
  });
}

async function applyNetworkProfile(cdpSession, profile) {
  const conditions = networkConditions(profile);
  await cdpSession.send('Network.enable');
  await cdpSession.send('Network.emulateNetworkConditions', conditions);
  return conditions;
}

function buildSamplePlan(profile) {
  const plan = [];
  for (const consumer of profile.consumers) {
    if (!consumer.enabled) {
      plan.push(Object.freeze({ consumer: consumer.kind, state: 'BLOCKED', reason: 'consumer_not_enabled' }));
      continue;
    }
    for (const network of profile.networkProfiles) {
      for (const cacheMode of CACHE_MODES) {
        plan.push(Object.freeze({ consumer: consumer.kind, network: network.name, cacheMode, state: 'PENDING' }));
      }
    }
  }
  return Object.freeze(plan);
}

function aggregateResults(rows = []) {
  if (!Array.isArray(rows) || rows.length === 0) return 'BLOCKED';
  if (rows.some(row => row?.state === 'FAILED')) return 'FAILED';
  if (rows.some(row => row?.required !== false && row?.state === 'BLOCKED')) return 'BLOCKED';
  return rows.every(row => RESULT_STATES.includes(row?.state) && ['PASSED', 'NOT_APPLICABLE'].includes(row.state))
    ? 'PASSED'
    : 'BLOCKED';
}

function classifyRuntimeError(input = {}) {
  const type = ['page', 'console', 'network'].includes(input.type) ? input.type : 'unknown';
  return Object.freeze({ code: `DOC105_${type.toUpperCase()}_ERROR`, type });
}

function evaluateInteractionBudget(scrollMilliseconds, zoomMilliseconds, budget = {}) {
  const scrollLimit = Number.isFinite(Number(budget.scrollMilliseconds)) ? Number(budget.scrollMilliseconds) : 3548;
  const zoomLimit = Number.isFinite(Number(budget.zoomMilliseconds)) ? Number(budget.zoomMilliseconds) : 435;
  return Object.freeze({
    scrollPassed: Number(scrollMilliseconds || 0) < scrollLimit,
    zoomPassed: Number(zoomMilliseconds || 0) < zoomLimit,
    scrollLimit,
    zoomLimit
  });
}

function sanitizeEvidence(input = {}) {
  const rows = Array.isArray(input.rows) ? input.rows.map(row => Object.freeze({
    consumer: CONSUMER_KINDS.includes(row.consumer) ? row.consumer : 'unknown',
    consumerClass: SIMPLE_HANDLER_CLASSES.includes(row.consumerClass) ? row.consumerClass : '',
    network: typeof row.network === 'string' && /^[A-Za-z0-9_-]{1,40}$/.test(row.network) ? row.network : 'unknown',
    cacheMode: CACHE_MODES.includes(row.cacheMode) ? row.cacheMode : 'unknown',
    state: RESULT_STATES.includes(row.state) ? row.state : 'BLOCKED',
    httpStatus: Number.isInteger(Number(row.httpStatus)) ? Number(row.httpStatus) : 0,
    loadingMode: ['RANGE', 'STREAM', 'UNKNOWN'].includes(row.loadingMode) ? row.loadingMode : 'UNKNOWN',
    firstCanvasMilliseconds: Math.max(0, Number(row.firstCanvasMilliseconds) || 0),
    domInteractiveMilliseconds: Math.max(0, Number(row.domInteractiveMilliseconds) || 0),
    longTaskCount: Math.max(0, Number(row.longTaskCount) || 0),
    uniqueBytes: Math.max(0, Number(row.uniqueBytes) || 0),
    documentLengthBytes: Math.max(0, Number(row.documentLengthBytes) || 0),
    uniqueBytesBeforeFirstCanvas: Math.max(0, Number(row.uniqueBytesBeforeFirstCanvas) || 0),
    bytesBeforeFirstCanvas: Math.max(0, Number(row.bytesBeforeFirstCanvas) || 0),
    partialBeforeFirstCanvas: row.partialBeforeFirstCanvas === true,
    physicalBytes: Math.max(0, Number(row.physicalBytes) || 0),
    requestCount: Math.max(0, Number(row.requestCount) || 0),
    requestCountBeforeFirstCanvas: Math.max(0, Number(row.requestCountBeforeFirstCanvas) || 0),
    firstPageBudgetSatisfied: row.firstPageBudgetSatisfied === true,
    firstCanvasSource: row.firstCanvasSource === 'preview' ? 'preview' : 'full',
    scrollBlockingMilliseconds: Math.max(0, Number(row.scrollBlockingMilliseconds) || 0),
    zoomBlockingMilliseconds: Math.max(0, Number(row.zoomBlockingMilliseconds) || 0),
    errorCategory: typeof row.errorCategory === 'string' && /^DOC105_[A-Z0-9_]+$/.test(row.errorCategory) ? row.errorCategory : '',
    observationCategory: typeof row.observationCategory === 'string' && /^DOC105_[A-Z0-9_]+$/.test(row.observationCategory)
      ? row.observationCategory
      : ''
  })) : [];
  const mutation = Object.freeze({
    state: RESULT_STATES.includes(input.mutation?.state) ? input.mutation.state : 'NOT_APPLICABLE',
    postCount: Math.max(0, Number(input.mutation?.postCount) || 0)
  });
  let result = aggregateResults(rows.map(row => ({ state: row.state })));
  if (mutation.state === 'FAILED') result = 'FAILED';
  else if (mutation.state === 'BLOCKED' && result !== 'FAILED') result = 'BLOCKED';
  const evidence = Object.freeze({
    schemaVersion: 1,
    scenario: 'doc105-pdf-cross-module',
    rows: Object.freeze(rows),
    mutation,
    result
  });
  if (forbiddenEvidence.test(JSON.stringify(evidence))) fail('DOC105_EVIDENCE_INVALID');
  return evidence;
}

function createNetworkObservation(page) {
  const observation = { exchanges: [], failures: [], warnings: [], handlerFailureCount: 0, firstCanvasReached: false };
  page.on('response', response => {
    let pathname = '';
    try { pathname = new URL(response.url()).pathname; } catch { return; }
    if (!/\/workflow\/Handler_image_wf\.ashx$/i.test(pathname)) return;
    const headers = response.headers();
    const requestHeaders = response.request().headers();
    observation.exchanges.push(Object.freeze({
      requestUrl: response.url(), // In-memory correlation only; never included by sanitizeEvidence.
      status: response.status(),
      requestRange: requestHeaders.range || '',
      contentRange: headers['content-range'] || '',
      contentLength: Number(headers['content-length'] || 0),
      transferSize: 0,
      beforeFirstCanvas: observation.firstCanvasReached !== true
    }));
  });
  page.on('requestfailed', request => {
    try {
      if (/\/workflow\/Handler_image_wf\.ashx$/i.test(new URL(request.url()).pathname)) observation.handlerFailureCount += 1;
    } catch { observation.failures.push('DOC105_NETWORK_ERROR'); }
  });
  page.on('pageerror', () => observation.failures.push('DOC105_PAGE_ERROR'));
  page.on('console', message => {
    if (message.type() !== 'error') return;
    const category = classifyConsoleFailure(message.text());
    if (category === 'DOC105_CONSOLE_RESOURCE_ERROR') observation.warnings.push(category);
    else observation.failures.push(category);
  });
  return observation;
}

function classifyConsoleFailure(message) {
  const value = String(message || '');
  if (/failed to load resource|net::err_|status of (?:4|5)\d\d/i.test(value)) {
    return 'DOC105_CONSOLE_RESOURCE_ERROR';
  }
  if (/\btypeerror\b/i.test(value)) return 'DOC105_CONSOLE_TYPE_ERROR';
  if (/\breferenceerror\b/i.test(value)) return 'DOC105_CONSOLE_REFERENCE_ERROR';
  if (/\bsyntaxerror\b/i.test(value)) return 'DOC105_CONSOLE_SYNTAX_ERROR';
  return 'DOC105_CONSOLE_ERROR';
}

function hasFatalHandlerFailure(failureCount, transfers, viewerSucceeded) {
  return Number(failureCount) > 0 && !(viewerSucceeded === true && Number(transfers?.rangeResponseCount) > 0);
}

async function navigateConsumerEntry(page, profile, consumer) {
  await page.goto(new URL(consumer.entryPath, profile.baseUrl).toString(), {
    waitUntil: 'domcontentloaded', timeout: profile.timeoutMilliseconds
  });
}

async function clickConsumerTarget(root, selector, ordinal, timeout, unavailableCode) {
  const targets = root.locator(`${selector}:visible`);
  await targets.first().waitFor({ state: 'visible', timeout }).catch(() => fail(unavailableCode));
  if (await targets.count() < ordinal) fail(unavailableCode);
  await targets.nth(ordinal - 1).click();
}

async function clickConsumerTargetByExactText(root, selector, expected, timeout, unavailableCode) {
  const normalizedExpected = safeText(expected, unavailableCode, 255);
  const argument = { selector, expected: normalizedExpected };
  await root.waitForFunction(({ selector: targetSelector, expected: targetText }) => {
    const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
    return Array.from(document.querySelectorAll(targetSelector))
      .some(element => normalize(element.innerText || element.textContent) === targetText);
  }, argument, { timeout }).catch(() => fail(unavailableCode));
  const clicked = await root.locator(selector).evaluateAll((elements, targetText) => {
    const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
    const target = elements.find(element => normalize(element.innerText || element.textContent) === targetText);
    if (!target) return false;
    target.click();
    return true;
  }, normalizedExpected);
  if (!clicked) fail(unavailableCode);
}

async function openProductionTreePath(root, selector, pathLabels, timeout) {
  const waitForPostback = async () => root.waitForFunction(() => {
    const manager = globalThis.Sys?.WebForms?.PageRequestManager?.getInstance?.();
    return !manager || manager.get_isInAsyncPostBack() === false;
  }, undefined, { timeout }).catch(() => fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE'));

  for (let index = 0; index < pathLabels.length; index += 1) {
    const label = pathLabels[index];
    const isFinal = index === pathLabels.length - 1;
    const argument = { selector, label };
    await root.waitForFunction(({ selector: targetSelector, label: targetLabel }) => {
      const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
      return Array.from(document.querySelectorAll(targetSelector))
        .some(element => normalize(element.innerText || element.textContent) === targetLabel);
    }, argument, { timeout }).catch(() => fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE'));
    const revealResult = await root.locator(selector).evaluateAll((elements, targetLabel) => {
      const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
      const target = elements.find(element => normalize(element.innerText || element.textContent) === targetLabel);
      if (!target) return 'missing';
      const collapsedAncestors = [];
      let ancestor = target.parentElement;
      while (ancestor) {
        if (ancestor.matches?.('div[id$="Nodes"]') && getComputedStyle(ancestor).display === 'none') {
          collapsedAncestors.push(ancestor);
        }
        ancestor = ancestor.parentElement;
      }
      // Los hijos ya vienen renderizados por WebForms. Solo se revela su
      // contenedor para poder seleccionar el nodo; usar los enlaces de expandir
      // iniciaría postbacks adicionales que compiten con OnNodeClicked.
      for (const container of collapsedAncestors.reverse()) container.style.display = 'block';
      return collapsedAncestors.length > 0 ? 'ancestors-revealed' : 'already-visible';
    }, label);
    if (revealResult === 'missing') fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE');
    await root.waitForFunction(({ selector: targetSelector, label: targetLabel }) => {
      const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
      return Array.from(document.querySelectorAll(targetSelector)).some(element => {
        const visible = !!(element.offsetWidth || element.offsetHeight || element.getClientRects().length);
        return visible && normalize(element.innerText || element.textContent) === targetLabel;
      });
    }, argument, { timeout }).catch(() => fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE'));
    const treeNodes = root.locator(selector);
    const targetIndex = await treeNodes.evaluateAll((elements, targetLabel) => {
      const normalize = value => String(value || '').replace(/\s+/g, ' ').trim();
      return elements.findIndex(element => {
        const visible = !!(element.offsetWidth || element.offsetHeight || element.getClientRects().length);
        return visible && normalize(element.innerText || element.textContent) === targetLabel;
      });
    }, label);
    if (targetIndex < 0) fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE');
    // El clic real de Playwright reproduce el enlace WebForms y su handler
    // delegado OnNodeClicked; HTMLElement.click() no garantiza esa secuencia.
    await treeNodes.nth(targetIndex).click({ timeout })
      .catch(() => fail('DOC105_PRODUCTION_CONTEXT_UNAVAILABLE'));
    await waitForPostback();
  }
}

async function clickProductionDocumentAction(root, consumer, timeout) {
  const rows = root.locator(`${consumer.rowSelector}:visible`);
  let row;
  if (consumer.documentTitle) {
    const matchingRows = rows.filter({ hasText: consumer.documentTitle });
    row = matchingRows.first();
    await row.waitFor({ state: 'visible', timeout }).catch(() => fail('DOC105_PRODUCTION_DOCUMENT_UNAVAILABLE'));
    if (await matchingRows.count() !== 1) fail('DOC105_PRODUCTION_DOCUMENT_AMBIGUOUS');
  } else {
    const ordinal = consumer.documentOrdinal || 1;
    await rows.first().waitFor({ state: 'visible', timeout }).catch(() => fail('DOC105_PRODUCTION_DOCUMENT_UNAVAILABLE'));
    if (await rows.count() < ordinal) fail('DOC105_PRODUCTION_DOCUMENT_UNAVAILABLE');
    row = rows.nth(ordinal - 1);
  }
  const action = row.locator(`${consumer.openSelector}:visible`).first();
  await action.waitFor({ state: 'visible', timeout }).catch(() => fail('DOC105_PRODUCTION_ACTION_UNAVAILABLE'));
  const measurementStarted = performance.now();
  await action.click();
  return measurementStarted;
}

async function resolveContainerFrame(page, selector, timeout) {
  const locator = page.locator(selector);
  await locator.waitFor({ state: 'attached', timeout }).catch(() => fail('DOC105_CONTAINER_FRAME_UNAVAILABLE'));
  const deadline = Date.now() + timeout;
  while (Date.now() < deadline) {
    const handle = await locator.elementHandle();
    const frame = await handle?.contentFrame();
    if (frame) return frame;
    await page.waitForTimeout(50);
  }
  fail('DOC105_CONTAINER_FRAME_UNAVAILABLE');
}

async function waitForOfficialMenuRoute(page, route, timeout, unavailableCode = 'DOC105_MENU_UNAVAILABLE') {
  await page.waitForFunction(expected => Array.isArray(window.ITEMS_DATOS_TOKENIZE_2) &&
    window.ITEMS_DATOS_TOKENIZE_2.some(item => item && item.value_node === expected && item.url_node),
  route, { timeout }).catch(() => fail(unavailableCode));
}

async function openCorrespondenceConsumer(page, profile, consumer) {
  await navigateConsumerEntry(page, profile, consumer);
  await waitForOfficialMenuRoute(page, 'CR-GT-01', profile.timeoutMilliseconds, 'DOC105_CORRESPONDENCE_MENU_UNAVAILABLE');
  await clickConsumerTarget(page, consumer.menuSelector, 1,
    profile.timeoutMilliseconds, 'DOC105_CORRESPONDENCE_MENU_UNAVAILABLE');
  const root = await resolveContainerFrame(page, consumer.containerFrameSelector, profile.timeoutMilliseconds);
  await clickConsumerTarget(root, consumer.rowSelector, consumer.contextOrdinal || 1,
    profile.timeoutMilliseconds, 'DOC105_CORRESPONDENCE_TASK_UNAVAILABLE');
  const measurementStarted = performance.now();
  await clickConsumerTarget(root, consumer.openSelector, consumer.documentOrdinal || 1,
    profile.timeoutMilliseconds, 'DOC105_CORRESPONDENCE_PDF_UNAVAILABLE');
  return Object.freeze({ root, frameSelector: consumer.viewerFrameSelector, measurementStarted });
}

async function openProductionConsumer(page, profile, consumer) {
  await navigateConsumerEntry(page, profile, consumer);
  await waitForOfficialMenuRoute(page, 'GD-GD-14', profile.timeoutMilliseconds, 'DOC105_PRODUCTION_MENU_UNAVAILABLE');
  await clickConsumerTarget(page, consumer.menuSelector, 1,
    profile.timeoutMilliseconds, 'DOC105_PRODUCTION_MENU_UNAVAILABLE');
  const root = await resolveContainerFrame(page, consumer.containerFrameSelector, profile.timeoutMilliseconds);
  if (consumer.contextPath?.length) {
    await openProductionTreePath(root, consumer.contextSelector, consumer.contextPath,
      profile.timeoutMilliseconds);
  } else if (consumer.contextTitle) {
    await clickConsumerTargetByExactText(root, consumer.contextSelector, consumer.contextTitle,
      profile.timeoutMilliseconds, 'DOC105_PRODUCTION_CONTEXT_UNAVAILABLE');
  } else {
    await clickConsumerTarget(root, consumer.contextSelector, consumer.contextOrdinal || 1,
      profile.timeoutMilliseconds, 'DOC105_PRODUCTION_CONTEXT_UNAVAILABLE');
  }
  const measurementStarted = await clickProductionDocumentAction(root, consumer, profile.timeoutMilliseconds);
  await root.waitForFunction(frameSelector => {
    const source = document.querySelector(frameSelector)?.getAttribute('src') || '';
    return /pdfjs\/pdf_anotate_view\/index\.html|Docuarchi\/WebFormDaVisorExterno\.aspx/i.test(source);
  }, consumer.viewerFrameSelector, { timeout: profile.timeoutMilliseconds })
    .catch(() => fail('DOC105_PRODUCTION_VIEWER_NOT_OPENED'));
  const source = await root.locator(consumer.viewerFrameSelector).getAttribute('src');
  if (/Docuarchi\/WebFormDaVisorExterno\.aspx/i.test(source || '')) {
    fail('DOC105_PRODUCTION_EXPRESS_PERMISSION_DENIED');
  }
  return Object.freeze({ root, frameSelector: consumer.viewerFrameSelector, measurementStarted });
}

async function waitForApprovalPostback(page, timeout) {
  await page.waitForFunction(() => {
    const manager = globalThis.Sys?.WebForms?.PageRequestManager?.getInstance?.();
    return !manager || manager.get_isInAsyncPostBack() === false;
  }, undefined, { timeout })
    .catch(() => fail('DOC105_APPROVAL_CONTEXT_UNAVAILABLE'));
}

async function selectApprovalContext(page, consumer, timeout) {
  let treeTargets = page.locator(`${consumer.contextSelector}:visible`);
  await treeTargets.first().waitFor({ state: 'visible', timeout })
    .catch(() => fail('DOC105_APPROVAL_TREE_UNAVAILABLE'));
  let index = consumer.treeOrdinal ? consumer.treeOrdinal - 1 : 0;
  for (let attempt = 0; attempt < 100; attempt += 1) {
    treeTargets = page.locator(`${consumer.contextSelector}:visible`);
    const count = await treeTargets.count();
    if (index >= count) break;
    await treeTargets.nth(index).click();
    await waitForApprovalPostback(page, timeout);
    const contexts = page.locator(`${consumer.rowSelector}:visible`);
    if (await contexts.count() >= (consumer.contextOrdinal || 1)) return;
    if (consumer.treeOrdinal) break;
    index += 1;
  }
  fail('DOC105_APPROVAL_CONTEXT_UNAVAILABLE');
}

async function openApprovalExternalConsumer(page, profile, consumer) {
  await navigateConsumerEntry(page, profile, consumer);
  await selectApprovalContext(page, consumer, profile.timeoutMilliseconds);
  await clickConsumerTarget(page, consumer.rowSelector, consumer.contextOrdinal || 1,
    profile.timeoutMilliseconds, 'DOC105_APPROVAL_CONTEXT_UNAVAILABLE');
  await waitForApprovalPostback(page, profile.timeoutMilliseconds);
  const measurementStarted = performance.now();
  await clickConsumerTarget(page, consumer.openSelector, consumer.documentOrdinal || 1,
    profile.timeoutMilliseconds, 'DOC105_APPROVAL_DOCUMENT_UNAVAILABLE');
  // El RowCommand reemplaza el iframe del visor dentro de un UpdatePanel. Esperar
  // el postback evita conservar el contentFrame inicial, que todavía no contiene
  // el documento seleccionado.
  await waitForApprovalPostback(page, profile.timeoutMilliseconds);
  const root = await resolveContainerFrame(page, consumer.containerFrameSelector, profile.timeoutMilliseconds);
  await root.waitForFunction(frameSelector => {
    const source = document.querySelector(frameSelector)?.getAttribute('src') || '';
    const classic = document.querySelector('#noaming');
    const classicMode = document.querySelector('#Hidden_tipo_visor_externo')?.value === '0';
    return /pdfjs\/pdf_anotate_view\/index\.html/i.test(source) ||
      /workflow\/Handler_image_wf\.ashx/i.test(source) || (classicMode && !!classic);
  }, consumer.viewerFrameSelector, { timeout: profile.timeoutMilliseconds })
    .catch(() => fail('DOC105_APPROVAL_VIEWER_UNAVAILABLE'));
  const viewerMode = await root.evaluate(frameSelector => {
    const source = document.querySelector(frameSelector)?.getAttribute('src') || '';
    if (/pdfjs\/pdf_anotate_view\/index\.html/i.test(source)) return 'pdfjs';
    if (/workflow\/Handler_image_wf\.ashx/i.test(source)) return 'handler';
    const classic = document.querySelector('#noaming');
    const classicMode = document.querySelector('#Hidden_tipo_visor_externo')?.value === '0';
    if (classicMode && classic) return 'classic';
    return 'unavailable';
  }, consumer.viewerFrameSelector).catch(() => fail('DOC105_APPROVAL_VIEWER_UNAVAILABLE'));
  if (viewerMode === 'unavailable') fail('DOC105_APPROVAL_VIEWER_UNAVAILABLE');
  return Object.freeze({
    root,
    frameSelector: consumer.viewerFrameSelector,
    measurementStarted,
    viewer: viewerMode === 'pdfjs',
    viewerMode
  });
}

async function openSimpleHandlerConsumer(page, profile, consumer) {
  if (consumer.simpleClass === 'migration' && consumer.cabinetName) return openMigrationConsumer(page, profile, consumer);
  if (consumer.simpleClass === 'versions' && consumer.templateName) {
    return openIncomingVersionsConsumer(page, profile, consumer);
  }
  if (['docuarchiExternal', 'docuarchiDownload'].includes(consumer.simpleClass) && consumer.cabinetName) {
    return openDocuarchiExternalConsumer(page, profile, consumer);
  }
  if (consumer.simpleClass === 'workflowOtherFormats') {
    const workflowProfile = { ...profile, ...consumer };
    await selectWorkflowTask(page, workflowProfile);
    const document = await selectWorkflowDocument(page, workflowProfile);
    await document.click();
    await page.waitForFunction(() => {
      const manager = globalThis.Sys?.WebForms?.PageRequestManager?.getInstance?.();
      return !manager || manager.get_isInAsyncPostBack() === false;
    }, undefined, { timeout: profile.timeoutMilliseconds })
      .catch(() => fail('DOC105_SIMPLE_HANDLER_CONTEXT_UNAVAILABLE'));
  }
  const measurementStarted = performance.now();
  await navigateConsumerEntry(page, profile, consumer);
  if (consumer.rowSelector) {
    await clickConsumerTarget(page, consumer.rowSelector, consumer.contextOrdinal || consumer.documentOrdinal || 1,
      profile.timeoutMilliseconds, 'DOC105_SIMPLE_HANDLER_CONTEXT_UNAVAILABLE');
  }
  if (consumer.openSelector) {
    await clickConsumerTarget(page, consumer.openSelector, consumer.documentOrdinal || 1,
      profile.timeoutMilliseconds, 'DOC105_SIMPLE_HANDLER_ACTION_UNAVAILABLE');
  }
  const root = consumer.containerFrameSelector
    ? await resolveContainerFrame(page, consumer.containerFrameSelector, profile.timeoutMilliseconds)
    : page;
  return Object.freeze({ root, frameSelector: consumer.viewerFrameSelector, measurementStarted });
}

function loadMigrationSelection(profile, target) {
  let selection;
  try { selection = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, '')); }
  catch { fail('DOC105_MIGRATION_SELECTION_INVALID'); }
  assertPlainObject(selection, 'DOC105_MIGRATION_SELECTION_INVALID');
  rejectUnknown(selection, new Set(['cabinetName', 'searchField', 'searchValue', 'documentPrefix', 'view']), 'DOC105_MIGRATION_SELECTION_INVALID');
  const view = selection.view === undefined ? 'converted' : selection.view;
  if (!['source', 'converted'].includes(view)) fail('DOC105_MIGRATION_SELECTION_INVALID');
  const values = {};
  for (const field of ['cabinetName', 'searchField', 'searchValue', 'documentPrefix']) {
    values[field] = safeText(selection[field], 'DOC105_MIGRATION_SELECTION_INVALID', 255);
  }
  return Object.freeze({ ...profile, mutation: { enabled: false, consumer: 'workflow' },
    consumers: Object.freeze(profile.consumers.map(c => Object.freeze(c.kind === 'simpleHandler'
      ? { kind: 'simpleHandler', enabled: true, simpleClass: 'migration', ...values, migrationView: view,
          entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', viewerFrameSelector: '#Iframe_visor_pdf' }
      : { ...c, enabled: false }))) });
}

function findMigrationRow({ searchField, searchValue, documentPrefix }) {
  const normalize = value => String(value || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, ' ').trim().toUpperCase();
  const table = document.querySelector('#table_consulta_migracion');
  if (!table) return null;
  const headings = Array.from(table.querySelectorAll('thead tr:last-child th'));
  const column = headings.findIndex(h => normalize(h.getAttribute('data-field')) === normalize(searchField) ||
    normalize(h.textContent) === normalize(searchField));
  if (column < 0) return null;
  const matches = Array.from(table.querySelectorAll('tbody tr')).map((row, index) => ({ row, index }))
    .filter(({ row }) => normalize(row.children[column]?.textContent) === normalize(searchValue) &&
      Array.from(row.children).some(cell => normalize(cell.textContent).startsWith(normalize(documentPrefix))) &&
      row.querySelector('.active_view_document'));
  return matches.length ? { count: matches.length, index: matches.length === 1 ? matches[0].index : -1 } : null;
}

async function openMigrationConsumer(page, profile, consumer) {
  const timeout = profile.timeoutMilliseconds;
  await navigateConsumerEntry(page, profile, consumer);
  console.log('DOC105_MIGRATION_STAGE=MODULE_OPEN');
  await waitForOfficialMenuRoute(page, 'GD-MR-99', timeout, 'DOC105_MIGRATION_MENU_UNAVAILABLE');
  const menu = page.locator('#GD-MR-99 > a');
  if (!(await page.locator('#GD-MR-15 > a').isVisible())) {
    await page.locator('#GD-PR-00 > a').click({ timeout });
  }
  if (!(await menu.isVisible())) await page.locator('#GD-MR-15 > a').click({ timeout });
  await menu.click({ timeout })
    .catch(() => fail('DOC105_MIGRATION_MENU_ACTION_UNAVAILABLE'));
  await page.waitForFunction(() => /Gestion_migracion\/Web_form_gestion_migracion_documento\.aspx/i.test(
    document.querySelector('#ContentPlacenter_ifrm_ds_')?.getAttribute('src') || ''), undefined, { timeout })
    .catch(() => fail('DOC105_MIGRATION_FRAME_NAVIGATION_FAILED'));
  console.log('DOC105_MIGRATION_STAGE=MODULE_LOADED');
  const root = await resolveContainerFrame(page, '#ContentPlacenter_ifrm_ds_', timeout);
  await root.waitForFunction(() => Array.from(document.querySelectorAll('#option_gabinet option'))
    .some(option => option.value && !['0', '-1'].includes(option.value)), undefined, { timeout })
    .catch(() => fail('DOC105_MIGRATION_CABINET_LIST_UNAVAILABLE'));
  console.log('DOC105_MIGRATION_STAGE=CABINET_LIST');
  const options = await root.locator('#option_gabinet option').evaluateAll(elements =>
    elements.map(e => ({ label: e.textContent, value: e.value })));
  const value = resolveDocuarchiCabinetValue(options, consumer.cabinetName);
  await root.locator('#option_gabinet').selectOption(value)
    .catch(() => fail('DOC105_MIGRATION_CABINET_SELECTION_FAILED'));
  await root.waitForFunction(() => !!document.querySelector('#table_consulta_migracion thead th'), undefined, { timeout })
    .catch(() => fail('DOC105_MIGRATION_SEARCH_FORM_UNAVAILABLE'));
  console.log('DOC105_MIGRATION_STAGE=CABINET_SELECTED');
  const field = root.locator('#div_consulta_gabinetes_migracion input[atrib_campo_beetwen="0"]:visible');
  const fieldIndex = await field.evaluateAll((inputs, name) => {
    const matches = inputs.map((input, index) => ({ index, name: input.getAttribute('atrib_campo_n') || '' }))
      .filter(input => input.name.trim().toUpperCase() === name.trim().toUpperCase());
    return matches.length === 1 ? matches[0].index : -1;
  }, consumer.searchField);
  if (fieldIndex < 0) fail('DOC105_MIGRATION_SEARCH_FIELD_UNAVAILABLE');
  await field.nth(fieldIndex).fill(consumer.searchValue);
  const response = page.waitForResponse(r => /\/Service_consulta_gabinete_migracion$/i.test(new URL(r.url()).pathname), { timeout });
  await root.locator('#Button_search_gabinete').click();
  await response;
  console.log('DOC105_MIGRATION_STAGE=DOCUMENT_SEARCH');
  const selection = { searchField: consumer.searchField, searchValue: consumer.searchValue, documentPrefix: consumer.documentPrefix };
  await root.waitForFunction(findMigrationRow, selection, { timeout })
    .catch(() => fail('DOC105_MIGRATION_DOCUMENT_UNAVAILABLE'));
  const match = await root.evaluate(findMigrationRow, selection);
  if (!match || match.count !== 1) fail('DOC105_MIGRATION_DOCUMENT_NOT_UNIQUE');
  const row = root.locator('#table_consulta_migracion tbody tr').nth(match.index);
  if (consumer.migrationView === 'source') {
    const measurementStarted = performance.now();
    await row.locator('.active_view_document.nav-link').click();
    await root.locator('#modal_visor_migracion_documento').waitFor({ state: 'visible', timeout });
    await root.waitForFunction(() => /Docuarchi\/WebFormDaVisorExterno\.aspx/i.test(
      document.querySelector('#Iframe_visor_migracion_documento')?.getAttribute('src') || ''), undefined, { timeout })
      .catch(() => fail('DOC105_MIGRATION_SOURCE_VIEWER_UNAVAILABLE'));
    const sourceRoot = await resolveContainerFrame(root, '#Iframe_visor_migracion_documento', timeout);
    await sourceRoot.waitForFunction(() => /workflow\/Handler_image_wf\.ashx/i.test(
      document.querySelector('#ifrm_visor_')?.getAttribute('src') || ''), undefined, { timeout });
    const handlerUrl = await sourceRoot.locator('#ifrm_visor_').evaluate(frame => frame.src);
    console.log('DOC105_MIGRATION_STAGE=SOURCE_PREVIEW_OPEN');
    return Object.freeze({ root: sourceRoot, frameSelector: '#ifrm_visor_', measurementStarted, handlerUrl,
      observationCategory: 'DOC105_MIGRATION_SOURCE_PREVIEW' });
  }
  await row.locator('[data-toggle="dropdown"]').click();
  const measurementStarted = performance.now();
  // This menu action only loads existing source/target metadata; conversion is a separate button.
  await row.locator('.active_migra_documento').click();
  await root.locator('#modal_migracion').waitFor({ state: 'visible', timeout });
  await root.waitForFunction(() => /workflow\/Handler_image_wf\.ashx/i.test(
    document.querySelector('#Iframe_visor_pdf')?.getAttribute('src') || ''), undefined, { timeout: Math.min(timeout, 30000) })
    .catch(() => fail('DOC105_MIGRATION_EXISTING_TARGET_UNAVAILABLE'));
  console.log('DOC105_MIGRATION_STAGE=EXISTING_TARGET_OPEN');
  const handlerUrl = await root.locator('#Iframe_visor_pdf').evaluate(frame => frame.src);
  return Object.freeze({ root, frameSelector: '#Iframe_visor_pdf', measurementStarted, handlerUrl });
}

function loadIncomingVersionsSelection(profile, target) {
  let selection;
  try { selection = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, '')); }
  catch { fail('DOC105_VERSIONS_SELECTION_INVALID'); }
  assertPlainObject(selection, 'DOC105_VERSIONS_SELECTION_INVALID');
  rejectUnknown(selection, new Set(['templateName', 'radicado', 'documentTitle']), 'DOC105_VERSIONS_SELECTION_INVALID');
  const templateName = safeText(selection.templateName, 'DOC105_VERSIONS_SELECTION_INVALID');
  const radicado = safeText(selection.radicado, 'DOC105_VERSIONS_SELECTION_INVALID', 30);
  if (!/^\d+$/.test(radicado)) fail('DOC105_VERSIONS_SELECTION_INVALID');
  const documentTitle = safeText(selection.documentTitle, 'DOC105_VERSIONS_SELECTION_INVALID', 255);
  return Object.freeze({ ...profile, mutation: { enabled: false, consumer: 'workflow' },
    consumers: Object.freeze(profile.consumers.map(c => Object.freeze(c.kind === 'simpleHandler'
      ? { kind: 'simpleHandler', enabled: true, simpleClass: 'versions', templateName, radicado, documentTitle,
          entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', viewerFrameSelector: '#Iframe_document_visor_version' }
      : { ...c, enabled: false }))) });
}

function resolveIncomingTemplate(items, name) {
  const normalize = value => String(value || '').trim().toUpperCase();
  const matches = items.filter(item => item.nodo_plantilla_radicado === 'yes' &&
    normalize(item.tipo_plantilla) === 'RADICACION ENTRANTE' && normalize(item.Text_node) === normalize(name));
  if (matches.length !== 1 || !matches[0].value_node) fail('DOC105_VERSIONS_TEMPLATE_NOT_UNIQUE');
  return String(matches[0].value_node);
}

function findIncomingVersionDocument(title) {
  const normalize = value => String(value || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, ' ').trim().toUpperCase();
  const actions = Array.from(document.querySelectorAll('[tip_event="lista_ver_doc_selecion_rad"]'));
  const matches = actions.map((action, index) => ({ index, title: (action.getAttribute('idd_rad') || '').split('|')[4] }))
    .filter(item => normalize(item.title) === normalize(title));
  return { count: matches.length, index: matches.length === 1 ? matches[0].index : -1 };
}

async function openIncomingVersionsConsumer(page, profile, consumer) {
  const timeout = profile.timeoutMilliseconds;
  await navigateConsumerEntry(page, profile, consumer);
  console.log('DOC105_VERSIONS_STAGE=TEMPLATE_SELECTION');
  await page.waitForFunction(() => Array.isArray(window.ITEMS_DATOS_TOKENIZE_2) &&
    window.ITEMS_DATOS_TOKENIZE_2.some(item => item.nodo_plantilla_radicado === 'yes'), undefined, { timeout });
  const items = await page.evaluate(() => window.ITEMS_DATOS_TOKENIZE_2.map(item => ({
    nodo_plantilla_radicado: item.nodo_plantilla_radicado, tipo_plantilla: item.tipo_plantilla,
    Text_node: item.Text_node, value_node: item.value_node
  })));
  const menuId = resolveIncomingTemplate(items, consumer.templateName);
  const menu = page.locator(`[id=${JSON.stringify(menuId)}]`);
  for (const selector of ['#CR-PR-00 > a', '#CR-PR-11 > a']) {
    if (!(await menu.isVisible())) await page.locator(selector).click();
  }
  await menu.click();
  await page.waitForFunction(() => /radicador\/WebFormRadicacionEntrante\.aspx/i.test(
    document.querySelector('#ContentPlacenter_ifrm_ds_')?.getAttribute('src') || ''), undefined, { timeout });
  const root = await resolveContainerFrame(page, '#ContentPlacenter_ifrm_ds_', timeout);
  await root.locator('#Hidden_radicado_seleccion').waitFor({ state: 'attached', timeout });
  // Never select a pending assignment: only inspect the already assigned authorized record.
  await root.waitForFunction(expected => document.querySelector('#Hidden_radicado_seleccion')?.value.trim() === expected,
    consumer.radicado, { timeout: Math.min(timeout, 30000) })
    .catch(() => fail('DOC105_VERSIONS_ASSIGNED_RECORD_MISMATCH'));
  console.log('DOC105_VERSIONS_STAGE=ASSIGNED_RECORD_CONFIRMED');
  await root.locator('#soporte-envio_nav').click();
  await root.locator('[tip_event="lista_ver_doc_selecion_rad"]').first().waitFor({ state: 'attached', timeout });
  const match = await root.evaluate(findIncomingVersionDocument, consumer.documentTitle);
  if (match.count !== 1) fail('DOC105_VERSIONS_DOCUMENT_NOT_UNIQUE');
  const action = root.locator('[tip_event="lista_ver_doc_selecion_rad"]').nth(match.index);
  // The version action can be inside the row dropdown. Open its menu through the UI.
  if (!(await action.isVisible())) await action.locator('xpath=ancestor::tr[1]').locator('[data-toggle="dropdown"]').first().click();
  await action.click();
  console.log('DOC105_VERSIONS_STAGE=HISTORY_OPEN');
  await root.locator('#modal_version_document').waitFor({ state: 'visible', timeout });
  const rows = root.locator('#tabl_lista_version_documento tbody tr').filter({
    has: root.locator('.active_version_document_view_document')
  });
  await root.waitForFunction(() => document.querySelectorAll(
    '#tabl_lista_version_documento tbody tr .active_version_document_view_document.nav-link').length >= 2,
    undefined, { timeout }).catch(() => fail('DOC105_VERSIONS_HISTORY_UNAVAILABLE'));
  const measurementStarted = performance.now();
  await rows.first().locator('.active_version_document_view_document.nav-link').click();
  console.log('DOC105_VERSIONS_STAGE=VERSION_OPEN');
  return Object.freeze({ root, frameSelector: '#Iframe_document_visor_version', measurementStarted });
}

function loadDocuarchiSelection(profile, target) {
  let selection;
  try { selection = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, '')); }
  catch { fail('DOC105_DOCUARCHI_SELECTION_INVALID'); }
  assertPlainObject(selection, 'DOC105_DOCUARCHI_SELECTION_INVALID');
  rejectUnknown(selection, new Set(['cabinetName', 'searchField', 'searchValue', 'simpleClass']), 'DOC105_DOCUARCHI_SELECTION_INVALID');
  const simpleClass = selection.simpleClass === undefined ? 'docuarchiExternal' : selection.simpleClass;
  if (!['docuarchiExternal', 'docuarchiDownload'].includes(simpleClass)) fail('DOC105_DOCUARCHI_SELECTION_INVALID');
  const cabinetName = safeText(selection.cabinetName, 'DOC105_DOCUARCHI_SELECTION_INVALID', 120);
  const searchField = safeText(selection.searchField, 'DOC105_DOCUARCHI_SELECTION_INVALID', 120);
  const searchValue = safeText(selection.searchValue, 'DOC105_DOCUARCHI_SELECTION_INVALID', 255);
  return Object.freeze({ ...profile, mutation: { enabled: false, consumer: 'workflow' },
    consumers: Object.freeze(profile.consumers.map(consumer => Object.freeze(consumer.kind === 'simpleHandler'
      ? { kind: 'simpleHandler', enabled: true, simpleClass,
          entryPath: 'Docuarchi/WebFormDaPrincipal.aspx', cabinetName, searchField, searchValue,
          viewerFrameSelector: '#ifrm_visor_' }
      : { ...consumer, enabled: false }))) });
}

function findExactDocuarchiRow({ field, value }) {
  const table = document.querySelector('#table_consulta_gabinete');
  if (!table) return null;
  const normalize = text => String(text ?? '').trim();
  const headings = Array.from(table.querySelectorAll('thead tr:last-child th'));
  const column = headings.findIndex(th => normalize(th.getAttribute('data-field')).toUpperCase() === field.toUpperCase());
  if (column < 0) return null;
  const matches = Array.from(table.querySelectorAll('tbody tr')).map((row, index) => ({ row, index }))
    .filter(({ row }) => normalize(row.children[column]?.textContent) === value && row.querySelector('.active_view_document'));
  return matches.length ? { count: matches.length, index: matches.length === 1 ? matches[0].index : -1 } : null;
}

async function openDocuarchiExternalConsumer(page, profile, consumer) {
  const timeout = profile.timeoutMilliseconds;
  await navigateConsumerEntry(page, profile, consumer);
  console.log('DOC105_DOCUARCHI_STAGE=CABINET_LIST');
  await page.waitForFunction(() => Array.from(document.querySelectorAll('#DropDownList_gabinetes option'))
    .some(option => option.value && option.value !== '0'), undefined, { timeout: Math.min(timeout, 60000) })
    .catch(() => fail('DOC105_DOCUARCHI_CABINET_LIST_UNAVAILABLE'));
  const options = await page.locator('#DropDownList_gabinetes option').evaluateAll(elements =>
    elements.map(option => ({ label: option.textContent, value: option.value })));
  const selectedValue = resolveDocuarchiCabinetValue(options, consumer.cabinetName);
  await page.locator('#DropDownList_gabinetes').selectOption({ value: selectedValue });
  if (await page.locator('#DropDownList_gabinetes').inputValue() !== selectedValue) {
    fail('DOC105_DOCUARCHI_CABINET_SELECTION_FAILED');
  }
  console.log('DOC105_DOCUARCHI_STAGE=CABINET_SELECTED');
  await page.locator('#btnSearhGabinete').click();
  await page.locator('#textBox_buequeda_general_gabinete').waitFor({ state: 'visible', timeout });
  await page.locator('#textBox_buequeda_general_gabinete').fill(consumer.searchValue);
  console.log('DOC105_DOCUARCHI_STAGE=DOCUMENT_SEARCH');
  const queryFinished = page.waitForResponse(response => /\/ServiceConsultaGabinete$/i.test(new URL(response.url()).pathname), { timeout });
  await page.locator('#BtnsearchgabineteGeneral').click();
  await queryFinished;
  const selection = { field: consumer.searchField, value: consumer.searchValue };
  await page.waitForFunction(findExactDocuarchiRow, selection, { timeout, polling: 100 })
    .catch(() => fail('DOC105_DOCUARCHI_DOCUMENT_UNAVAILABLE'));
  // The query may match other fields. Open only the exact authorized column/value.
  const match = await page.evaluate(findExactDocuarchiRow, selection);
  if (!match || match.count !== 1) fail('DOC105_DOCUARCHI_DOCUMENT_NOT_UNIQUE');
  const measurementStarted = performance.now();
  console.log('DOC105_DOCUARCHI_STAGE=DOCUMENT_OPEN');
  await page.locator('#table_consulta_gabinete tbody tr').nth(match.index).locator('.active_view_document').click();
  await page.waitForFunction(() => /\/Docuarchi\/WebFormDaVisorExterno\.aspx/i.test(
    document.querySelector('#Iframe_visor_migracion_documento')?.getAttribute('src') || ''), undefined, { timeout })
    .catch(() => fail('DOC105_DOCUARCHI_VIEWER_UNAVAILABLE'));
  const root = await resolveContainerFrame(page, '#Iframe_visor_migracion_documento', timeout);
  if (consumer.simpleClass === 'docuarchiDownload') {
    await root.waitForFunction(() => /\/Docuarchi\/WebFormDaVisorExternoDescarga\.aspx/i.test(
      document.querySelector('#ifrm_visor_')?.getAttribute('src') || ''), undefined, { timeout })
      .catch(() => fail('DOC105_DOCUARCHI_DOWNLOAD_UNAVAILABLE'));
    const downloadRoot = await resolveContainerFrame(root, '#ifrm_visor_', timeout);
    console.log('DOC105_DOCUARCHI_STAGE=DOWNLOAD_START');
    const [download] = await Promise.all([
      page.waitForEvent('download', { timeout }),
      downloadRoot.locator('#ImageButton_descarga').click()
    ]);
    const downloadedBytes = await verifyCompletedDownload(download, timeout);
    console.log('DOC105_DOCUARCHI_STAGE=DOWNLOAD_COMPLETED');
    return Object.freeze({ root: downloadRoot, frameSelector: '#ifmExcel_', measurementStarted, downloadedBytes });
  }
  return Object.freeze({ root, frameSelector: '#ifrm_visor_', measurementStarted });
}

async function verifyCompletedDownload(download, timeout) {
  let timer;
  try {
    return await Promise.race([
      (async () => {
        if (await download.failure()) fail('DOC105_DOWNLOAD_FAILED');
        const stream = await download.createReadStream();
        if (!stream) fail('DOC105_DOWNLOAD_EMPTY');
        let bytes = 0;
        for await (const chunk of stream) bytes += chunk.length;
        if (bytes === 0) fail('DOC105_DOWNLOAD_EMPTY');
        return bytes;
      })(),
      new Promise((resolve, reject) => {
        timer = setTimeout(() => reject(Object.assign(new Error('DOC105_DOWNLOAD_TIMEOUT'),
          { code: 'DOC105_DOWNLOAD_TIMEOUT' })), timeout);
      })
    ]);
  } catch (error) {
    await download.cancel().catch(() => {});
    if (/^DOC105_DOWNLOAD_/.test(error?.code || '')) throw error;
    fail('DOC105_DOWNLOAD_FAILED');
  } finally {
    clearTimeout(timer);
    // Only the browser-owned temporary download; never save document content as evidence.
    await download.delete();
  }
}

function downloadTransportPassed(bytes, exchanges) {
  return bytes > 0 && exchanges.length === 1 && exchanges[0].status === 200 &&
    exchanges[0].contentLength === bytes;
}

function resolveDocuarchiCabinetValue(options, name) {
  const normalize = value => String(value ?? '').replace(/\s+/g, ' ').trim().toUpperCase();
  const matches = options.filter(option => option.value && option.value !== '0' && normalize(option.label) === normalize(name));
  if (!matches.length) fail('DOC105_DOCUARCHI_CABINET_NOT_FOUND');
  if (matches.length !== 1) fail('DOC105_DOCUARCHI_CABINET_NOT_UNIQUE');
  return matches[0].value;
}

async function openConsumer(page, profile, consumer) {
  if (consumer.kind === 'workflow') {
    const workflowProfile = { ...profile, ...consumer };
    await selectWorkflowTask(page, workflowProfile);
    const document = await selectWorkflowDocument(page, workflowProfile);
    const measurementStarted = performance.now();
    await document.click();
    return Object.freeze({ root: page, frameSelector: '#ifrm_visor_', measurementStarted });
  }
  if (consumer.kind === 'correspondence') return openCorrespondenceConsumer(page, profile, consumer);
  if (consumer.kind === 'production') return openProductionConsumer(page, profile, consumer);
  if (consumer.kind === 'approvalExternal') return openApprovalExternalConsumer(page, profile, consumer);
  if (consumer.kind === 'simpleHandler') return openSimpleHandlerConsumer(page, profile, consumer);
  fail('DOC105_CONSUMER_KIND_INVALID');
}

async function waitForHandlerConsumer(page, selector, timeout, observation, consumerKind) {
  const prefix = consumerKind === 'correspondence'
    ? 'DOC105_CORRESPONDENCE'
    : consumerKind === 'approvalExternal' ? 'DOC105_APPROVAL' : 'DOC105_SIMPLE_HANDLER';
  await page.waitForFunction(frameSelector => {
    const source = document.querySelector(frameSelector)?.getAttribute('src') || '';
    return /workflow\/Handler_image_wf\.ashx/i.test(source);
  }, selector, { timeout }).catch(() => fail(`${prefix}_NOT_OPENED`));
  const deadline = Date.now() + timeout;
  while (observation.exchanges.length === 0 && Date.now() < deadline) await page.waitForTimeout(50);
  if (observation.exchanges.length === 0) fail(`${prefix}_RESPONSE_UNAVAILABLE`);
}

async function diagnoseFirstCanvasFailure(page, frameSelector, observation) {
  if (page.isClosed?.() || page.isDetached?.()) return 'DOC105_PAGE_CLOSED_BEFORE_CANVAS';
  const state = await page.evaluate(selector => {
    const frame = document.querySelector(selector);
    const frameDocument = frame?.contentDocument;
    const canvases = frameDocument ? Array.from(frameDocument.querySelectorAll('#pdf-container_ canvas')) : [];
    return {
      loginVisible: !!document.querySelector('#txt_usuario, #txt_clave, #ButtonIngresar'),
      framePresent: !!frame,
      viewerSourceSet: /pdfjs\/pdf_anotate_view\/index\.html/i.test(frame?.getAttribute('src') || ''),
      frameDocumentAvailable: !!frameDocument,
      pageCount: frameDocument?.querySelectorAll('#pdf-container_ .pdfpage').length || 0,
      canvasCount: canvases.length,
      nonZeroCanvasCount: canvases.filter(canvas => canvas.width > 0 && canvas.height > 0).length
    };
  }, frameSelector).catch(() => null);
  if (!state) return 'DOC105_FIRST_CANVAS_STATE_UNAVAILABLE';
  if (state.loginVisible) return 'DOC105_SESSION_NOT_AUTHENTICATED';
  if (!state.framePresent) return 'DOC105_VIEWER_FRAME_MISSING';
  if (!state.viewerSourceSet) return 'DOC105_VIEWER_NOT_OPENED';
  if (!state.frameDocumentAvailable) return 'DOC105_VIEWER_DOCUMENT_UNAVAILABLE';
  if (observation.exchanges.length === 0) return 'DOC105_HANDLER_RESPONSE_NOT_OBSERVED';
  if (state.pageCount === 0) return 'DOC105_PDF_PAGES_NOT_CREATED';
  if (state.canvasCount === 0) return 'DOC105_FIRST_CANVAS_NOT_CREATED';
  if (state.nonZeroCanvasCount === 0) return 'DOC105_FIRST_CANVAS_ZERO_SIZE';
  return 'DOC105_FIRST_CANVAS_UNKNOWN_FAILURE';
}

async function measureConsumer(page, profile, consumer, networkProfile, observation) {
  let stage = 'OPEN_CONSUMER';
  let frameSelector = consumer.viewerFrameSelector || '#ifrm_visor_';
  let measurementRoot = page;
  try {
    let started = performance.now();
    const opened = await openConsumer(page, profile, consumer);
    started = opened.measurementStarted || started;
    measurementRoot = opened.root;
    frameSelector = opened.frameSelector;
    if (opened.viewerMode === 'classic') {
      stage = 'WAIT_CLASSIC_VIEWER';
      await measurementRoot.waitForFunction(() => {
        const image = document.querySelector('#noaming');
        const source = image?.getAttribute('src') || '';
        return !!image && !!source && image.complete === true && image.naturalWidth > 0;
      }, undefined, { timeout: profile.timeoutMilliseconds })
        .catch(() => fail('DOC105_APPROVAL_CLASSIC_VIEWER_NOT_READY'));
      return Object.freeze({
        httpStatus: 0,
        loadingMode: 'UNKNOWN',
        firstCanvasMilliseconds: 0,
        domInteractiveMilliseconds: 0,
        longTaskCount: 0,
        uniqueBytes: 0,
        documentLengthBytes: 0,
        uniqueBytesBeforeFirstCanvas: 0,
        bytesBeforeFirstCanvas: 0,
        partialBeforeFirstCanvas: false,
        physicalBytes: 0,
        requestCount: 0,
        requestCountBeforeFirstCanvas: 0,
        firstPageBudgetSatisfied: false,
        firstCanvasSource: 'full',
        scrollBlockingMilliseconds: 0,
        zoomBlockingMilliseconds: 0,
        state: observation.failures.length === 0 ? 'PASSED' : 'FAILED',
        errorCategory: observation.failures[0] || '',
        observationCategory: opened.observationCategory || observation.warnings[0] || ''
      });
    }
    if (!adapterRegistry[consumer.kind].viewer || opened.viewer === false) {
      stage = 'WAIT_HANDLER';
      await waitForHandlerConsumer(measurementRoot, frameSelector, profile.timeoutMilliseconds, observation, consumer.kind);
      if (consumer.simpleClass === 'migration') {
        const deadline = Date.now() + profile.timeoutMilliseconds;
        while (!observation.exchanges.some(e => e.requestUrl === opened.handlerUrl) && Date.now() < deadline) {
          await page.waitForTimeout(50);
        }
        observation.exchanges = observation.exchanges.filter(e => e.requestUrl === opened.handlerUrl);
        if (!observation.exchanges.length) fail('DOC105_MIGRATION_TARGET_RESPONSE_UNAVAILABLE');
      }
      const transfers = summarizeTransfers(observation.exchanges);
      const isDownload = consumer.simpleClass === 'docuarchiDownload';
      const downloadPassed = isDownload && downloadTransportPassed(opened.downloadedBytes, observation.exchanges);
      // Chromium may report navigation aborted when handing a response to its download manager.
      // Only a completed, length-checked download can satisfy this branch.
      const fatalHandlerFailure = isDownload ? !downloadPassed : hasFatalHandlerFailure(observation.handlerFailureCount, transfers, true);
      return Object.freeze({
        httpStatus: observation.exchanges.at(-1)?.status || 0,
        loadingMode: transfers.rangeResponseCount > 0 ? 'RANGE' : 'STREAM',
        firstCanvasMilliseconds: 0,
        domInteractiveMilliseconds: 0,
        longTaskCount: 0,
        uniqueBytes: transfers.uniqueBytes,
        documentLengthBytes: transfers.documentLengthBytes,
        uniqueBytesBeforeFirstCanvas: transfers.uniqueBytesBeforeFirstCanvas,
        partialBeforeFirstCanvas: transfers.partialBeforeFirstCanvas,
        physicalBytes: transfers.physicalTransferredBytes || transfers.physicalDeclaredBytes,
        requestCount: transfers.requestCount,
        scrollBlockingMilliseconds: 0,
        zoomBlockingMilliseconds: 0,
        state: observation.failures.length === 0 && !fatalHandlerFailure ? 'PASSED' : 'FAILED',
        errorCategory: observation.failures[0] || (fatalHandlerFailure ? 'DOC105_HANDLER_NETWORK_ERROR' : ''),
        observationCategory: opened.observationCategory || observation.warnings[0] || ''
      });
    }
    stage = 'FIRST_CANVAS';
    const network = { handlerStatuses: [], viewerHttpErrors: 0, failedHandlerRequests: observation.failures.length };
    await waitForViewerFrame(measurementRoot, profile.timeoutMilliseconds, frameSelector);
    const frame = await waitForFirstCanvas(measurementRoot, profile.timeoutMilliseconds, network, frameSelector);
    const firstCanvasMilliseconds = performance.now() - started;
    observation.firstCanvasReached = true;
    await frame.waitForFunction(() => {
      const preview = document.querySelector('#pdf-first-page-preview');
      const canvas = document.querySelector('#pdf-container_ .pdfpage canvas');
      return !preview && !!canvas && canvas.width > 0 && canvas.height > 0 &&
        typeof pdfViewer !== 'undefined' && !!pdfViewer?.pdf;
    }, undefined, { timeout: profile.timeoutMilliseconds }).catch(() => fail('DOC105_FULL_VIEWER_NOT_READY'));
    stage = 'CAPTURE_METRICS';
    const metrics = await captureViewerMetrics(frame, firstCanvasMilliseconds, observation.exchanges.map(item => ({
      status: item.status,
      requestHeaders: { range: item.requestRange },
      responseHeaders: {
        'content-range': item.contentRange,
        'content-length': String(item.contentLength || 0),
        'accept-ranges': item.status === 206 ? 'bytes' : ''
      }
    })));
    let interactions = { scroll: null, zoom: null };
    if (adapterRegistry[consumer.kind].scrollZoom) {
      stage = 'SCROLL';
      interactions.scroll = await measureScroll(frame, profile.scrollSteps);
      stage = 'ZOOM';
      interactions.zoom = await measureZoom(frame);
    }
    const transfers = summarizeTransfers(observation.exchanges.map((item, index) => ({
      ...item,
      transferSize: index === 0 ? Number(metrics.handler?.transferBytes || 0) : 0
    })));
    const scrollBlockingMilliseconds = Number(interactions.scroll?.maxBlockingMs || interactions.scroll?.blockingMs || 0);
    const zoomBlockingMilliseconds = Number(interactions.zoom?.maxBlockingMs || interactions.zoom?.blockingMs || 0);
    const interactionBudget = evaluateInteractionBudget(scrollBlockingMilliseconds, zoomBlockingMilliseconds);
    const progressiveLoaded = Number(metrics.firstCanvasProgress?.loaded || 0);
    const progressiveTotal = Number(metrics.firstCanvasProgress?.total || 0);
    const firstCanvasSource = metrics.firstCanvasProgress?.source === 'preview' ? 'preview' : 'full';
    const bytesBeforeFirstCanvas = progressiveLoaded > 0 && progressiveTotal > 0
      ? progressiveLoaded
      : transfers.bytesBeforeFirstCanvas;
    const documentLengthBytes = transfers.documentLengthBytes || progressiveTotal;
    const budgetDocumentLengthBytes = progressiveTotal > 0 ? progressiveTotal : documentLengthBytes;
    const firstPageBudget = evaluateFirstPageBudget({
      firstCanvasMilliseconds: metrics.firstCanvasMs,
      bytesBeforeFirstCanvas,
      documentLengthBytes: budgetDocumentLengthBytes,
      requestCountBeforeFirstCanvas: transfers.requestCountBeforeFirstCanvas,
      firstCanvasSource
    }, networkProfile.firstPageBudget);
    const fatalHandlerFailure = hasFatalHandlerFailure(observation.handlerFailureCount, transfers, true);
    const transportPolicySatisfied = evaluateConsumerTransportPolicy(metrics, transfers, firstCanvasSource,
      bytesBeforeFirstCanvas, documentLengthBytes);
    const failed = observation.failures.length > 0 || fatalHandlerFailure || (metrics.classifiedErrors || []).length > 0 ||
      !interactionBudget.scrollPassed || !interactionBudget.zoomPassed || !transportPolicySatisfied ||
      !firstPageBudget.passed;
    return Object.freeze({
      httpStatus: observation.exchanges.at(-1)?.status || 0,
      loadingMode: String(metrics.pdfLoading?.mode || 'unknown').toUpperCase(),
      firstCanvasMilliseconds: metrics.firstCanvasMs,
      domInteractiveMilliseconds: metrics.domInteractiveMs,
      longTaskCount: metrics.longTaskCount,
      uniqueBytes: transfers.uniqueBytes,
      documentLengthBytes,
      uniqueBytesBeforeFirstCanvas: transfers.uniqueBytesBeforeFirstCanvas,
      bytesBeforeFirstCanvas,
      partialBeforeFirstCanvas: bytesBeforeFirstCanvas > 0 && documentLengthBytes > 0 && bytesBeforeFirstCanvas < documentLengthBytes,
      physicalBytes: Number(metrics.handler?.transferBytes || transfers.physicalDeclaredBytes),
      requestCount: transfers.requestCount,
      requestCountBeforeFirstCanvas: transfers.requestCountBeforeFirstCanvas,
      firstPageBudgetSatisfied: firstPageBudget.passed,
      firstCanvasSource,
      scrollBlockingMilliseconds,
      zoomBlockingMilliseconds,
      state: failed ? 'FAILED' : 'PASSED',
      errorCategory: observation.failures[0] || (fatalHandlerFailure ? 'DOC105_HANDLER_NETWORK_ERROR' : '') ||
        ((metrics.classifiedErrors || []).length ? 'DOC105_VIEWER_ERROR' : '') ||
        (!interactionBudget.scrollPassed ? 'DOC105_SCROLL_BUDGET_EXCEEDED' : '') ||
        (!interactionBudget.zoomPassed ? 'DOC105_ZOOM_BUDGET_EXCEEDED' : '') ||
        (!transportPolicySatisfied ? 'DOC105_TRANSPORT_POLICY_FAILED' : '') ||
        firstPageBudget.errorCategory,
      observationCategory: observation.warnings[0] || ''
    });
  } catch (error) {
    if (/^[A-Z0-9_]+$/.test(error?.code || error?.message || '')) throw error;
    if (stage === 'FIRST_CANVAS') fail(await diagnoseFirstCanvasFailure(measurementRoot, frameSelector, observation));
    fail(`DOC105_${stage}_FAILED`);
  }
}

function isBlockedError(error) {
  return /(?:UNAVAILABLE|NOT_OPENED|NOT_FOUND|PERMISSION_DENIED)/.test(String(error?.code || error?.message || ''));
}

function normalizeExecutionErrorCode(error, stage) {
  const rawCode = String(error?.code || error?.message || '');
  if (/^DOC105_[A-Z0-9_]+$/.test(rawCode)) return rawCode;
  const workflowCode = /^WORKFLOW_PDF_PERF_([A-Z0-9_]+)$/.exec(rawCode);
  if (workflowCode) return `DOC105_WORKFLOW_${workflowCode[1]}`;
  return `DOC105_${stage}_FAILED`;
}

async function executeCell(browser, storageState, profile, consumer, network, cacheMode, existingContext) {
  let stage = 'CONTEXT';
  let context;
  let page;
  try {
    context = existingContext || await browser.newContext({
      storageState,
      ...(profile.ignoreHttpsErrors ? { ignoreHTTPSErrors: true } : {})
    });
    stage = 'PROBE';
    await installPerformanceProbe(context);
    stage = 'PAGE';
    page = await context.newPage();
    stage = 'CDP_SESSION';
    const cdp = await context.newCDPSession(page);
    stage = 'NETWORK_PROFILE';
    await applyNetworkProfile(cdp, network);
    const observation = createNetworkObservation(page);
    stage = 'MEASUREMENT';
    const measured = await measureConsumer(page, profile, consumer, network, observation);
    return { context, row: {
      consumer: consumer.kind,
      consumerClass: consumer.kind === 'simpleHandler' ? consumer.simpleClass : '',
      network: network.name,
      cacheMode,
      ...measured
    } };
  } catch (error) {
    return {
      context,
      row: {
        consumer: consumer.kind,
        consumerClass: consumer.kind === 'simpleHandler' ? consumer.simpleClass : '',
        network: network.name,
        cacheMode,
        state: isBlockedError(error) ? 'BLOCKED' : 'FAILED',
        errorCategory: normalizeExecutionErrorCode(error, stage)
      }
    };
  } finally {
    await page?.close().catch(() => {});
  }
}

async function runReadMatrix(profile, environment, options = {}) {
  const browser = await chromium.launch({ ...(profile.browser || {}), headless: profile.headless });
  let authenticationContext;
  try {
    const authenticationOptions = {
      baseUrl: profile.baseUrl,
      environment,
      moduleEnvironmentVariable: 'DOC105_E2E_MODULE',
      userEnvironmentVariable: 'DOC105_E2E_AUTHORIZED_USER',
      passwordEnvironmentVariable: 'DOC105_E2E_AUTHORIZED_PASSWORD',
      timeoutMilliseconds: profile.timeoutMilliseconds,
      ignoreHTTPSErrors: profile.ignoreHttpsErrors
    };
    const rows = [];
    for (const consumer of profile.consumers) {
      if (!consumer.enabled) {
        rows.push({ consumer: consumer.kind, state: 'BLOCKED', errorCategory: 'DOC105_CONSUMER_NOT_AUTHORIZED' });
        continue;
      }
      const selectedNetworks = options.networkName
        ? profile.networkProfiles.filter(item => item.name === options.networkName)
        : profile.networkProfiles;
      if (selectedNetworks.length === 0) fail('DOC105_NETWORK_NAME_NOT_FOUND');
      for (const network of selectedNetworks) {
        try {
          authenticationContext = await createAuthenticatedWorkflowSession(browser, authenticationOptions);
          const cold = await executeCell(browser, undefined, profile, consumer, network, 'cold', authenticationContext);
          rows.push(cold.row);
          if (options.derivativeWaitMilliseconds > 0 && cold.row.loadingMode === 'STREAM') {
            await new Promise(resolve => setTimeout(resolve, options.derivativeWaitMilliseconds));
          }
          const warm = await executeCell(browser, undefined, profile, consumer, network, 'warm', cold.context);
          rows.push(warm.row);
        } finally {
          await authenticationContext?.close().catch(() => {});
          authenticationContext = null;
        }
      }
    }
    return sanitizeEvidence({ rows });
  } finally {
    await authenticationContext?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

async function writeEvidence(evidence) {
  const target = path.join(e2eRoot, 'artifacts', 'doc105-pdf-cross-module.json');
  await fsp.mkdir(path.dirname(target), { recursive: true });
  await fsp.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function writeStoppedEvidence(error) {
  const rawCode = String(error?.code || error?.message || '');
  const errorCategory = /^DOC105_[A-Z0-9_]{1,100}$/.test(rawCode) ? rawCode : 'DOC105_FAILED';
  const target = path.join(e2eRoot, 'artifacts', 'doc105-pdf-cross-module-stop.json');
  await fsp.mkdir(path.dirname(target), { recursive: true });
  await fsp.writeFile(target, `${JSON.stringify({ result: 'BLOCKED', errorCategory }, null, 2)}\n`, 'utf8');
}

function classifyGraphSaveEvidence(graphEvidence) {
  const postCount = Math.max(0, Number(graphEvidence?.postCount) || 0);
  return Object.freeze({
    state: graphEvidence?.result === 'PASSED' && graphEvidence?.singleEffect === true && postCount === 1
      ? 'PASSED'
      : 'FAILED',
    postCount
  });
}

async function runAuthorizedGraphSave(profile, values) {
  if (!profile.mutation.enabled) return Object.freeze({ state: 'NOT_APPLICABLE', postCount: 0 });
  await collectConfirmation(values, 'DOC105_GRAPH_ACCOUNT_AUTHORIZED', '¿Autoriza nuevamente la cuenta y tarea para el único guardado de grafo?');
  await collectConfirmation(values, 'DOC105_GRAPH_DISPOSABLE_PDF_AUTHORIZED', '¿Confirma que el PDF es descartable y puede modificarse una sola vez?');
  await collectValue(values, 'DOC105_GRAPH_MYSQL_USER', 'Usuario ODBC de solo lectura');
  await collectValue(values, 'DOC105_GRAPH_MYSQL_PASSWORD', 'Contraseña ODBC de solo lectura', { secret: true });
  const graphProfile = loadGraphSaveProfile(path.resolve(e2eRoot, profile.mutation.profile));
  const graphEvidence = await runGraphSaveReal(graphProfile, {
    ...process.env,
    DOC103_E2E_MODULE: graphProfile.module,
    DOC103_E2E_ENVIRONMENT_AUTHORIZED: 'true',
    DOC103_E2E_ACCOUNT_AUTHORIZED: 'true',
    DOC103_E2E_DISPOSABLE_PDF_AUTHORIZED: 'true',
    DOC103_E2E_AUTHORIZED_USER: values.DOC105_E2E_AUTHORIZED_USER,
    DOC103_E2E_AUTHORIZED_PASSWORD: values.DOC105_E2E_AUTHORIZED_PASSWORD,
    DOC103_E2E_MYSQL_USER: values.DOC105_GRAPH_MYSQL_USER,
    DOC103_E2E_MYSQL_PASSWORD: values.DOC105_GRAPH_MYSQL_PASSWORD,
    DOC103_E2E_ODBC_DSN: graphProfile.odbcDsn
  }, { failurePrefix: 'DOC105' });
  return classifyGraphSaveEvidence(graphEvidence);
}

function validateMutationPolicy(profile) {
  if (!profile.mutation.enabled) return true;
  if (profile.mutation.consumer !== 'workflow') fail('DOC105_MUTATION_CONSUMER_FORBIDDEN');
  return true;
}

function loadExecutionProfile(argv = process.argv) {
  const index = argv.indexOf('--profile');
  const workflowIndex = argv.indexOf('--workflow-profile');
  if (index >= 0 && workflowIndex >= 0) fail('DOC105_PROFILE_ARGUMENT_CONFLICT');
  if (index >= 0 && argv[index + 1]) return loadProfile(path.resolve(argv[index + 1]));
  if (workflowIndex >= 0 && argv[workflowIndex + 1]) return loadWorkflowBootstrapProfile(path.resolve(argv[workflowIndex + 1]));
  fail('DOC105_PROFILE_REQUIRED');
}

async function measureHorizontalLayout(frame) {
  const sample = await frame.evaluate(() => {
    const el = document.querySelector('#pdf-container_');
    if (!el) throw new Error('DOC105_LAYOUT_CONTAINER_UNAVAILABLE');
    const rect = el.getBoundingClientRect();
    const original = el.scrollLeft;
    const maximum = Math.max(0, el.scrollWidth - el.clientWidth);
    el.scrollLeft = maximum;
    const reached = el.scrollLeft;
    el.scrollLeft = original;
    return {
      zoom: Number(pdfViewer.getZoom()), clientWidth: el.clientWidth, scrollWidth: el.scrollWidth,
      clientHeight: el.clientHeight, viewportHeight: innerHeight,
      containerTop: rect.top, containerBottom: rect.bottom,
      bottomProbeX: Math.max(0, Math.min(innerWidth - 1, rect.left + el.clientWidth / 2)),
      horizontalRange: maximum, reachedRight: reached,
      horizontalEnabled: ['auto', 'scroll'].includes(getComputedStyle(el).overflowX),
      scrollbarGutter: Math.max(0, el.offsetHeight - el.clientHeight - el.clientTop * 2),
      bottomClippedPixels: Math.max(0, rect.bottom - innerHeight)
    };
  });
  // Proyectar el borde inferior por cada iframe, sin leer URLs ni contenido documental.
  let bottom = sample.containerBottom - 1;
  let probeX = sample.bottomProbeX;
  let current = frame;
  let ancestorViewportClippedPixels = 0;
  let ancestorBottomVisible = true;
  while (current.parentFrame()) {
    const host = await current.frameElement();
    const childViewport = await current.evaluate(() => ({ width: innerWidth, height: innerHeight }));
    const projection = await host.evaluate((el, point) => {
      const r = el.getBoundingClientRect();
      const scaleX = r.width / (el.offsetWidth || r.width);
      const scaleY = r.height / (el.offsetHeight || r.height);
      const x = r.left + el.clientLeft * scaleX + point.x * el.clientWidth * scaleX / point.width;
      const projected = r.top + el.clientTop * scaleY + point.bottom * el.clientHeight * scaleY / point.height;
      // Hit testing respeta containing blocks, clipping y overlays reales; no todos
      // los ancestros overflow:hidden recortan un descendiente absolute/fixed.
      return { x, bottom: projected, clipped: Math.max(0, projected + 1 - innerHeight),
        visible: document.elementFromPoint(x, projected) === el };
    }, { x: probeX, bottom, ...childViewport });
    ancestorViewportClippedPixels = Math.max(ancestorViewportClippedPixels, projection.clipped);
    ancestorBottomVisible = ancestorBottomVisible && projection.visible;
    bottom = projection.bottom;
    probeX = projection.x;
    await host.dispose();
    current = current.parentFrame();
  }
  return Object.freeze({ ...sample, ancestorViewportClippedPixels, ancestorBottomVisible,
    rightReachable: sample.horizontalRange > 0 && Math.abs(sample.reachedRight - sample.horizontalRange) <= 1,
    bottomVisible: sample.bottomClippedPixels <= 1 && ancestorBottomVisible });
}

function validateLayoutDiagnostic(profile) {
  const enabled = profile.consumers.filter(item => item.enabled);
  if (profile.mutation.enabled || enabled.length !== 1 || enabled[0].kind !== 'production') {
    fail('DOC105_LAYOUT_REQUIRES_READONLY_PRODUCTION');
  }
}

async function runLayoutDiagnostic(profile, environment) {
  validateLayoutDiagnostic(profile);
  const browser = await chromium.launch({ ...(profile.browser || {}), headless: profile.headless });
  let context;
  try {
    console.log('DOC105_LAYOUT_STAGE=LOGIN');
    context = await createAuthenticatedWorkflowSession(browser, {
      baseUrl: profile.baseUrl, environment, moduleEnvironmentVariable: 'DOC105_E2E_MODULE',
      userEnvironmentVariable: 'DOC105_E2E_AUTHORIZED_USER', passwordEnvironmentVariable: 'DOC105_E2E_AUTHORIZED_PASSWORD',
      timeoutMilliseconds: profile.timeoutMilliseconds, ignoreHttpsErrors: profile.ignoreHttpsErrors
    });
    // Defensa específica: este diagnóstico no admite persistencia de anotaciones.
    await context.route(/service_anotate_pdf_image/i, route => route.abort());
    const page = await context.newPage();
    await page.setViewportSize({ width: 1500, height: 880 });
    console.log('DOC105_LAYOUT_STAGE=PRODUCTION_NAVIGATION');
    const opened = await openProductionConsumer(page, profile, profile.consumers.find(item => item.enabled));
    const frame = await resolveContainerFrame(opened.root, opened.frameSelector, profile.timeoutMilliseconds);
    console.log('DOC105_LAYOUT_STAGE=WAIT_VIEWER');
    const ready = () => typeof pdfViewer !== 'undefined' && !!pdfViewer?.pdf &&
      !document.querySelector('#tool_pdf')?.disabled && !document.querySelector('#pdf-first-page-preview') &&
      !!document.querySelector('#pdf-container_ canvas');
    await frame.waitForFunction(ready, undefined, { timeout: profile.timeoutMilliseconds });
    const samples = [];
    for (let step = 0; step < 4; step++) {
      if (step > 0) {
        const previousZoom = await frame.evaluate(() => pdfViewer.getZoom());
        await frame.locator('button:has(#pdf_btn_zon_plus)').click();
        await frame.waitForFunction(previous => pdfViewer.getZoom() > previous,
          previousZoom, { timeout: profile.timeoutMilliseconds });
        await frame.waitForFunction(ready, undefined, { timeout: profile.timeoutMilliseconds });
        await frame.evaluate(() => new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve))));
      }
      const sample = await measureHorizontalLayout(frame);
      samples.push(sample);
      console.log(`DOC105_LAYOUT_SAMPLE=${JSON.stringify(sample)}`);
    }
    const overflow = samples.filter(sample => sample.horizontalRange > 0);
    const result = overflow.length === 0 ? 'INCONCLUSIVE' :
      overflow.every(sample => sample.rightReachable && sample.horizontalEnabled && sample.bottomVisible) ? 'PASSED' : 'FAILED';
    const target = path.join(e2eRoot, 'artifacts', 'doc105-horizontal-layout.json');
    await fsp.mkdir(path.dirname(target), { recursive: true });
    await fsp.writeFile(target, JSON.stringify({ result, samples }, null, 2) + '\n', 'utf8');
    console.log(`DOC105_LAYOUT_RESULT=${result}`);
    if (result !== 'PASSED') process.exitCode = 2;
  } finally {
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

async function main() {
  requireInteractiveConsole();
  let profile = loadExecutionProfile();
  const moduleArgument = process.argv.indexOf('--module');
  if (moduleArgument >= 0) {
    const module = process.argv[moduleArgument + 1];
    if (!module || module.startsWith('--')) fail('DOC105_MODULE_ARGUMENT_INVALID');
    profile = selectAuthenticationModule(profile, module);
  }
  const enableAll = process.argv.includes('--enable-inventoried-consumers');
  const consumerArgument = process.argv.indexOf('--consumer');
  if (enableAll && consumerArgument >= 0) fail('DOC105_CONSUMER_ARGUMENT_CONFLICT');
  if (consumerArgument >= 0) {
    const consumer = process.argv[consumerArgument + 1];
    if (!consumer || consumer.startsWith('--')) fail('DOC105_CONSUMER_ARGUMENT_INVALID');
    profile = selectConsumer(profile, consumer);
  } else if (enableAll) profile = enableInventoriedConsumers(profile);
  const contextTitleArgument = process.argv.indexOf('--context-title');
  const documentTitleArgument = process.argv.indexOf('--document-title');
  const contextTitle = contextTitleArgument >= 0 ? process.argv[contextTitleArgument + 1] : undefined;
  const documentTitle = documentTitleArgument >= 0 ? process.argv[documentTitleArgument + 1] : undefined;
  if (contextTitleArgument >= 0 && (!contextTitle || contextTitle.startsWith('--'))) fail('DOC105_CONTEXT_TITLE_INVALID');
  if (documentTitleArgument >= 0 && (!documentTitle || documentTitle.startsWith('--'))) fail('DOC105_DOCUMENT_TITLE_INVALID');
  let contextPath;
  if (process.env.DOC105_E2E_CONTEXT_PATH_JSON) {
    try { contextPath = JSON.parse(process.env.DOC105_E2E_CONTEXT_PATH_JSON); } catch { fail('DOC105_CONTEXT_PATH_INVALID'); }
  }
  const effectiveDocumentTitle = documentTitle ?? process.env.DOC105_E2E_DOCUMENT_TITLE;
  profile = overrideProductionSelection(profile, contextTitle, effectiveDocumentTitle, contextPath);
  validateMutationPolicy(profile);
  const docuarchiArgument = process.argv.indexOf('--docuarchi-selection');
  const versionsArgument = process.argv.indexOf('--versions-selection');
  const migrationArgument = process.argv.indexOf('--migration-selection');
  if ([docuarchiArgument, versionsArgument, migrationArgument].filter(i => i >= 0).length > 1) fail('DOC105_SELECTION_CONFLICT');
  if (migrationArgument >= 0) {
    const target = safeRuntimeProfile(process.argv[migrationArgument + 1], 'DOC105_MIGRATION_SELECTION_INVALID');
    profile = loadMigrationSelection(profile, path.resolve(target));
  }
  if (docuarchiArgument >= 0 && versionsArgument >= 0) fail('DOC105_SELECTION_CONFLICT');
  if (versionsArgument >= 0) {
    const target = safeRuntimeProfile(process.argv[versionsArgument + 1], 'DOC105_VERSIONS_SELECTION_INVALID');
    profile = loadIncomingVersionsSelection(profile, path.resolve(target));
  }
  if (docuarchiArgument >= 0) {
    const target = safeRuntimeProfile(process.argv[docuarchiArgument + 1], 'DOC105_DOCUARCHI_SELECTION_INVALID');
    profile = loadDocuarchiSelection(profile, path.resolve(target));
  }
  const layoutDiagnostic = process.argv.includes('--diagnose-horizontal-layout');
  if (layoutDiagnostic) validateLayoutDiagnostic(profile);
  const values = { DOC105_E2E_MODULE: profile.module };
  await collectConfirmation(values, 'DOC105_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza el ambiente para la matriz de lectura DOC-105?');
  await collectConfirmation(values, 'DOC105_E2E_ACCOUNT_AUTHORIZED', '¿Autoriza esta cuenta y los consumidores configurados?');
  await collectConfirmation(values, 'DOC105_E2E_VIEW_LOG_AUTHORIZED', '¿Autoriza los registros normales de visualización generados al abrir documentos?');
  await collectValue(values, 'DOC105_E2E_AUTHORIZED_USER', 'Cuenta autorizada');
  await collectValue(values, 'DOC105_E2E_AUTHORIZED_PASSWORD', 'Contraseña', { secret: true });
  if (layoutDiagnostic) return runLayoutDiagnostic(profile, { ...process.env, ...values });
  const networkArgument = process.argv.indexOf('--network');
  const networkName = networkArgument >= 0 ? process.argv[networkArgument + 1] : undefined;
  if (networkArgument >= 0 && (!networkName || !profile.networkProfiles.some(item => item.name === networkName))) {
    fail('DOC105_NETWORK_NAME_INVALID');
  }
  const waitArgument = process.argv.indexOf('--derivative-wait-ms');
  const derivativeWaitMilliseconds = waitArgument >= 0
    ? positiveInteger(process.argv[waitArgument + 1], 'DOC105_DERIVATIVE_WAIT_INVALID', 120000)
    : 0;
  const readEvidence = await runReadMatrix(profile, { ...process.env, ...values }, { networkName, derivativeWaitMilliseconds });
  const mutation = await runAuthorizedGraphSave(profile, values);
  const evidence = sanitizeEvidence({ rows: readEvidence.rows, mutation });
  await writeEvidence(evidence);
  const workflowRows = evidence.rows.filter(row => row.consumer === 'workflow');
  const workflowResult = aggregateResults(workflowRows.map(row => ({ state: row.state })));
  const enabledConsumers = profile.consumers.filter(consumer => consumer.enabled);
  const blockedConsumerCount = new Set(evidence.rows
    .filter(row => row.consumer !== 'workflow' && row.state === 'BLOCKED')
    .map(row => row.consumer)).size;
  console.log(`DOC105_MATRIX_ROW_COUNT=${evidence.rows.length}`);
  console.log(`DOC105_WORKFLOW_RESULT=${workflowResult}`);
  for (const consumer of enabledConsumers) {
    const consumerRows = evidence.rows.filter(row => row.consumer === consumer.kind);
    const consumerResult = aggregateResults(consumerRows.map(row => ({ state: row.state })));
    console.log(`DOC105_${consumer.kind.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase()}_RESULT=${consumerResult}`);
  }
  console.log(`DOC105_BLOCKED_CONSUMER_COUNT=${blockedConsumerCount}`);
  console.log(`DOC105_RESULT=${evidence.result}`);
  if (evidence.result !== 'PASSED') process.exitCode = 2;
}

if (require.main === module) {
  main().catch(async error => {
    const code = /^[A-Z0-9_]{3,120}$/.test(error?.code || error?.message || '') ? (error.code || error.message) : 'DOC105_FAILED';
    await writeStoppedEvidence(error).catch(() => {});
    console.error(`La matriz DOC-105 se detuvo de forma segura (${code}). No se mostraron rutas, credenciales ni documentos.`);
    process.exitCode = 2;
  });
}

module.exports = {
  measureHorizontalLayout,
  validateLayoutDiagnostic,
  loadMigrationSelection,
  findMigrationRow,
  loadIncomingVersionsSelection,
  resolveIncomingTemplate,
  findIncomingVersionDocument,
  verifyCompletedDownload,
  downloadTransportPassed,
  resolveDocuarchiCabinetValue,
  loadDocuarchiSelection,
  findExactDocuarchiRow,
  CACHE_MODES,
  CONSUMER_KINDS,
  REQUIRED_NETWORK_MBPS,
  RESULT_STATES,
  SIMPLE_HANDLER_CLASSES,
  adapterRegistry,
  aggregateResults,
  applyNetworkProfile,
  buildSamplePlan,
  classifyRuntimeError,
  classifyConsoleFailure,
  hasFatalHandlerFailure,
  evaluateInteractionBudget,
  evaluateFirstPageBudget,
  evaluateConsumerTransportPolicy,
  enableInventoriedConsumers,
  executeCell,
  diagnoseFirstCanvasFailure,
  loadProfile,
  loadExecutionProfile,
  loadWorkflowBootstrapProfile,
  networkConditions,
  normalizeExecutionErrorCode,
  parseContentRange,
  sanitizeEvidence,
  summarizeTransfers,
  unionIntervals,
  validateMutationPolicy,
  writeStoppedEvidence,
  clickConsumerTarget,
  clickProductionDocumentAction,
  openApprovalExternalConsumer,
  openCorrespondenceConsumer,
  openConsumer,
  openProductionConsumer,
  openProductionTreePath,
  openSimpleHandlerConsumer,
  overrideProductionSelection,
  resolveContainerFrame,
  waitForOfficialMenuRoute,
  runAuthorizedGraphSave,
  classifyGraphSaveEvidence,
  selectConsumer,
  selectAuthenticationModule,
  runReadMatrix
};

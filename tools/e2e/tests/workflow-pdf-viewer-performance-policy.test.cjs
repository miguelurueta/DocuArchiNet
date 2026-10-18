'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  allowedFailureFields,
  classifyViewerRequest,
  createRequestCounts,
  classifyBrowserFailure,
  classifyPageError,
  deriveResult,
  evaluateFirstCanvasTransport,
  installPerformanceProbe,
  isViewerFrameRequest,
  loadProfile,
  requestCountCategories,
  sanitizeHandlerExchange,
  summarizeHandlerExchanges,
  summarize
} = require('../scripts/run-workflow-pdf-viewer-performance-interactive.cjs');

const runnerPath = path.resolve(__dirname, '../scripts/run-workflow-pdf-viewer-performance-interactive.cjs');
const runner = fs.readFileSync(runnerPath, 'utf8');

test('DOC-105 preview completo pequeño no equivale al PDF principal completo', () => {
  const metrics = { pdfLoading: { mode: 'RANGE' }, firstCanvasSnapshot: {
    source: 'preview', loaded: 390850, total: 390850, completedResourceBytes: 390850
  }, handler: { transferBytes: 10040894 } };
  const contract = { statuses: { '200': 2, '206': 10 }, rangeRequestCount: 10, documentLengthBytes: 9646444 };
  const result = evaluateFirstCanvasTransport(metrics, contract);
  assert.equal(result.transportPolicySatisfied, true);
  assert.equal(result.firstCanvasByteScope, 'preview_representation');
  for (const replacement of [
    { statuses: { '200': 1 } }, { rangeRequestCount: 0 }, { documentLengthBytes: 0 },
    { documentLengthBytes: 390850 }
  ]) assert.equal(evaluateFirstCanvasTransport(metrics, { ...contract, ...replacement }).transportPolicySatisfied, false);
  assert.equal(evaluateFirstCanvasTransport({ ...metrics, firstCanvasSnapshot: {
    source: 'preview', loaded: 0, total: 390850
  } }, contract).transportPolicySatisfied, false);
  for (const completedResourceBytes of [0, 9646444, 10040894]) {
    assert.equal(evaluateFirstCanvasTransport({ ...metrics, firstCanvasSnapshot: {
      ...metrics.firstCanvasSnapshot, completedResourceBytes
    } }, contract).transportPolicySatisfied, false);
  }
});

test('DOC-105 principal completo y bytes ausentes no pasan como carga parcial', () => {
  const contract = { statuses: { '206': 1 }, rangeRequestCount: 1, documentLengthBytes: 900000 };
  const metrics = { pdfLoading: { mode: 'range' }, firstCanvasProgress: { source: 'full', loaded: 900000, total: 900000 } };
  assert.equal(evaluateFirstCanvasTransport(metrics, contract).transportPolicySatisfied, false);
  assert.equal(evaluateFirstCanvasTransport({ ...metrics, firstCanvasProgress: null }, contract).transportPolicySatisfied, false);
  const partial = { ...metrics, firstCanvasProgress: { source: 'full', loaded: 100000, total: 900000 } };
  assert.equal(evaluateFirstCanvasTransport(partial, contract).transportPolicySatisfied, true);
  assert.equal(evaluateFirstCanvasTransport(partial, { ...contract, statuses: { '200': 1 } }).transportPolicySatisfied, false);
  const stream = evaluateFirstCanvasTransport({ ...partial, pdfLoading: { mode: 'stream' } }, {
    statuses: { '200': 1 }, rangeRequestCount: 0, documentLengthBytes: 900000
  });
  assert.equal(stream.transportPolicySatisfied, true);
  assert.equal(stream.partialBeforeFirstCanvas, false);
});

test('DOC-105 snapshot no se sustituye por tráfico y progreso posteriores', async () => {
  const vm = require('node:vm');
  let init;
  await installPerformanceProbe({ addInitScript: async callback => { init = callback; } });
  const listeners = {};
  let mutation;
  let ready = false;
  let bytes = 390850;
  const window = { addEventListener: (name, callback) => { listeners[name] = callback; },
    getDocumentLoadingTelemetry: () => ({ firstCanvasLoaded: bytes, firstCanvasTotal: bytes }) };
  const context = { window, location: { pathname: '/pdfjs/pdf_anotate_view/index.html' }, URL,
    document: { documentElement: {}, querySelector: selector =>
      ready && selector.includes('data-ready') ? { width: 100, height: 200 } : null },
    performance: { now: () => 419, getEntriesByType: () => [{ name: 'https://example.invalid/workflow/Handler_image_wf.ashx', encodedBodySize: bytes }] },
    MutationObserver: class { constructor(callback) { mutation = callback; } observe() {} } };
  vm.runInNewContext(`(${init.toString()})()`, context);
  listeners.DOMContentLoaded();
  assert.equal(window.__workflowPdfPerformance.firstCanvas, undefined);
  ready = true;
  mutation();
  const snapshot = JSON.stringify(window.__workflowPdfPerformance.firstCanvas);
  bytes = 9646444;
  mutation();
  assert.equal(JSON.stringify(window.__workflowPdfPerformance.firstCanvas), snapshot);
  assert.equal(window.__workflowPdfPerformance.firstCanvas.loaded, 390850);
  assert.doesNotMatch(snapshot, /https?:|example|ashx/);
});

function withProfile(value, action) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'workflow-pdf-perf-policy-'));
  const target = path.join(directory, 'profile.json');
  fs.writeFileSync(target, JSON.stringify(value), 'utf8');
  try { return action(target); } finally { fs.rmSync(directory, { recursive: true, force: true }); }
}

function validProfile() {
  return {
    baseUrl: 'https://workflow.example.invalid/app/', module: 'GESTOR', taskId: 1,
    documentOrdinal: 1, documentTitle: 'Estados Financieros', iterations: 3, scrollSteps: 12, timeoutMilliseconds: 120000,
    headless: true, browser: { channel: 'chrome' }, ignoreHttpsErrors: false
  };
}

test('el perfil del visor admite solo datos no sensibles y límites acotados', () => {
  const profile = withProfile(validProfile(), loadProfile);
  assert.equal(profile.taskId, 1);
  assert.equal(profile.iterations, 3);
  assert.equal(profile.documentOrdinal, 1);
  assert.equal(profile.documentTitle, 'Estados Financieros');
  assert.throws(() => withProfile({ ...validProfile(), password: 'secreto' }, loadProfile), /WORKFLOW_PDF_PERF_PROFILE_FIELD_NOT_ALLOWED/);
  assert.throws(() => withProfile({ ...validProfile(), iterations: 11 }, loadProfile), /WORKFLOW_PDF_PERF_ITERATIONS_INVALID/);
});

test('la evidencia resume tiempos sin incluir identidad de tarea, documento ni URL', () => {
  assert.deepEqual(summarize([{ firstCanvasMs: 900 }, { firstCanvasMs: 300 }, { firstCanvasMs: 600 }]), {
    minimumMs: 300, medianMs: 600, maximumMs: 900
  });
  assert.doesNotMatch(runner, /evidence[\s\S]{0,160}(?:taskId|documentOrdinal|documentTitle|baseUrl)/);
  assert.doesNotMatch(runner, /console\.(?:log|error)\([^\n]*(?:url|src|rut_image|cookie)/i);
});

test('clasifica solicitudes en conteos cerrados sin conservar URL ni query', () => {
  const cases = [
    ['https://host.invalid/pdfjs/pdf_anotate_view/script.js?task=77', 'core'],
    ['https://host.invalid/pdfjs/wacom/signature_sdk.js?token=secret', 'wacom'],
    ['https://host.invalid/printer_js/print.min.js', 'print'],
    ['https://cdn.invalid/fabric.min.js', 'fabric'],
    ['https://cdn.invalid/prettify.min.js', 'prettify'],
    ['https://host.invalid/pdfjs/pdf_anotate_view/js/jquery.contextMenu.js', 'graph'],
    ['https://host.invalid/bootstrap.min.css', 'other']
  ];
  for (const [url, expected] of cases) assert.equal(classifyViewerRequest(url), expected);
  const counts = createRequestCounts();
  assert.deepEqual(Object.keys(counts), requestCountCategories);
  assert.deepEqual(Object.values(counts), requestCountCategories.map(() => 0));
  assert.match(runner, /requestCounts:\s*\{\s*\.\.\.network\.requestCounts\s*\}/);
  assert.doesNotMatch(JSON.stringify(counts), /https?:|task=|token|secret/i);
});

test('los conteos incluyen solo solicitudes originadas por el iframe oficial', () => {
  const viewer = 'https://host.invalid/pdfjs/pdf_anotate_view/index.html';
  assert.equal(isViewerFrameRequest(viewer, 'about:blank'), true);
  assert.equal(isViewerFrameRequest('https://host.invalid/pdfjs/wacom/q.js', viewer), true);
  assert.equal(isViewerFrameRequest('https://host.invalid/js/jquery-ui.min.js', 'https://host.invalid/workflow/Webworkflow.aspx'), false);
  assert.equal(isViewerFrameRequest('https://host.invalid/main.css', ''), false);
  assert.match(runner, /if \(!isViewerFrameRequest\(request\.url\(\), frameUrl\)\) return/);
});

test('DOC-102 sanea contrato Range y bytes sin conservar cabeceras sensibles', () => {
  const rawExchange = {
    status: 206,
    requestHeaders: { range: 'bytes=0-65535', cookie: 'sesion=privada' },
    responseHeaders: {
      'accept-ranges': 'bytes', 'content-range': 'bytes 0-65535/900000',
      'content-length': '65536', etag: '"secreto"', 'last-modified': 'Mon, 12 Oct 2026 12:00:00 GMT',
      'cache-control': 'private, max-age=0, must-revalidate', vary: 'Accept-Encoding, Cookie'
    }
  };
  const exchange = sanitizeHandlerExchange(rawExchange);
  assert.deepEqual(exchange, {
    status: 206, rangeRequested: true, acceptRangesBytes: true, contentRangePresent: true,
    contentLengthBytes: 65536, documentLengthBytes: 900000, etagPresent: true, lastModifiedPresent: true,
    privateCache: true, varyCookie: true
  });
  const summary = summarizeHandlerExchanges([rawExchange]);
  assert.equal(summary.rangeRequestCount, 1);
  assert.equal(summary.statuses['206'], 1);
  assert.equal(summary.declaredResponseBytes, 65536);
  assert.equal(summary.documentLengthBytes, 900000);
  assert.deepEqual(summary.contract, {
    acceptRanges: true, contentRange: true, etag: true, lastModified: true,
    validators: true, privateCache: true, varyCookie: true
  });
  assert.doesNotMatch(JSON.stringify(summary), /bytes=|sesion|privada|secreto|0-65535|Mon,|GMT|\"[a-f0-9]{16,}\"/i);
  assert.match(runner, /const streamingContract = summarizeHandlerExchanges/);
  assert.match(runner, /partialBeforeFirstCanvas/);
  assert.match(runner, /loadingParams\?\.disableAutoFetch === true/);
  assert.match(runner, /loadingParams\?\.disableRange === true/);
  assert.match(runner, /transportPolicySatisfied/);
  assert.match(runner, /String\(browserMetrics\.pdfLoading\?\.mode \|\| ''\)\.toLowerCase\(\)/);
  assert.match(runner, /rangeRequestCount === 0/);
  assert.match(runner, /WORKFLOW_PDF_PERF_LOADING_MODE=/);
  assert.match(runner, /WORKFLOW_PDF_PERF_TRANSPORT_POLICY=/);
  assert.match(runner, /failedHandlerRequests > 0 && !network\.handlerStatuses\.some/);
});

test('la medición abre el visor oficial sin invocar persistencia de grafos', () => {
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /tip_event=\"seleccion_tarea_wf\"/);
  assert.match(runner, /tip_event=\"vis_doc_selecion_wf\"/);
  assert.match(runner, /#ifrm_visor_/);
  assert.match(runner, /pdf_anotate_view/);
  assert.match(runner, /WORKFLOW_PDF_PERF_TASK_LIST_PERMISSION_DENIED/);
  assert.match(runner, /WORKFLOW_PDF_PERF_DIAGNOSTIC/);
  assert.match(runner, /handlerStatuses/);
  assert.match(runner, /WORKFLOW_PDF_PERF_DOCUMENT_TITLE_NOT_FOUND/);
  assert.match(runner, /contentWindow/);
  assert.match(runner, /typeof pdfViewer === 'undefined'/);
  assert.match(runner, /WORKFLOW_PDF_PERF_SCROLL_COMPLETED/);
  assert.doesNotMatch(runner, /service_anotate_pdf_image/);
  assert.doesNotMatch(runner, /Pdf_btn_update_insert/);
});

test('la matriz opcional es no persistente y produce evidencia separada', () => {
  assert.match(runner, /--capability-matrix/);
  assert.match(runner, /workflow-pdf-viewer-capability-matrix\.json/);
  assert.match(runner, /persistedDocumentChange:\s*false/);
  assert.match(runner, /physicalWacomPresent:\s*false/);
  assert.match(runner, /overlay\?\.remove\(\)/);
  assert.match(runner, /route\.abort\('failed'\)/);
  assert.match(runner, /wacomUnavailable\.statePreserved/);
  assert.doesNotMatch(runner, /exerciseCapabilityMatrix[\s\S]*service_anotate_pdf_image/);
  assert.doesNotMatch(runner, /exerciseCapabilityMatrix[\s\S]*Pdf_btn_update_insert/);
});

test('clasifica errores con un contrato cerrado y saneado', () => {
  const cases = [
    [{ type: 'error', source: 'main_frame', fileName: 'https://host/app.js?task=99', line: 8 }, 'WORKFLOW_PDF_SCRIPT_ERROR'],
    [{ type: 'pageerror', source: 'viewer', fileName: 'C:\\private\\viewer.js', line: 19, message: 'documento secreto' }, 'WORKFLOW_PDF_PAGE_ERROR'],
    [{ type: 'error', source: 'worker', fileName: '/pdf.worker.js#token', line: 4 }, 'WORKFLOW_PDF_WORKER_ERROR'],
    [{ type: 'resource', source: 'optional_resource', fileName: '/fonts/ui.woff2?user=ana' }, 'WORKFLOW_PDF_OPTIONAL_RESOURCE_FAILED'],
    [{ type: 'not-known', source: 'not-known', fileName: '/folder/file.js' }, 'WORKFLOW_PDF_UNKNOWN_FAILURE']
  ];
  for (const [input, code] of cases) {
    const classified = classifyBrowserFailure(input);
    assert.deepEqual(Object.keys(classified), allowedFailureFields);
    assert.equal(classified.code, code);
    assert.doesNotMatch(JSON.stringify(classified), /https?:|private|task=|token|documento secreto|user=ana/i);
  }
});

test('pageerror extrae solo archivo y línea sin conservar stack, URL ni mensaje', () => {
  const classified = classifyPageError({
    name: 'ReferenceError',
    message: 'documento privado',
    stack: 'ReferenceError: documento privado\n    at https://host.invalid/pdfjs/pdf_anotate_view/pdf.worker.entry.min.js?token=secret:1:45'
  });
  assert.deepEqual(classified, {
    code: 'WORKFLOW_PDF_PAGE_ERROR', source: 'viewer', type: 'pageerror', fileBase: 'pdf.worker.entry.min.js', line: 1
  });
  assert.doesNotMatch(JSON.stringify(classified), /https?:|token|secret|documento|stack/i);
});

test('deriva PASSED, PASSED_WITH_OBSERVATIONS y FAILED sin falsos limpios', () => {
  const milestones = { documentSelected: true, firstCanvas: true, scroll: true, zoom: true, reloads: true };
  const optional = classifyBrowserFailure({ type: 'resource', source: 'optional_resource', fileName: 'ui.woff2' });
  const optionalScript = classifyBrowserFailure({ type: 'error', source: 'optional_resource', fileName: 'ui.js' });
  const viewer = classifyBrowserFailure({ type: 'pageerror', source: 'viewer', fileName: 'viewer.js' });
  const workerEntry = classifyPageError({
    stack: 'ReferenceError\n at https://host.invalid/pdfjs/pdf_anotate_view/pdf.worker.entry.min.js:1:2'
  });
  assert.equal(deriveResult({ milestones, failures: [] }), 'PASSED');
  assert.equal(deriveResult({ milestones: { ...milestones, partialFirstCanvas: true }, failures: [] }), 'PASSED');
  assert.equal(deriveResult({ milestones: { ...milestones, partialFirstCanvas: false }, failures: [] }), 'FAILED');
  assert.equal(deriveResult({ milestones: { ...milestones, transportPolicy: true }, failures: [] }), 'PASSED');
  assert.equal(deriveResult({ milestones: { ...milestones, transportPolicy: false }, failures: [] }), 'FAILED');
  assert.equal(deriveResult({ milestones, failures: [optional] }), 'PASSED_WITH_OBSERVATIONS');
  assert.equal(deriveResult({ milestones, failures: [optionalScript] }), 'FAILED');
  assert.equal(deriveResult({ milestones, failures: [viewer] }), 'FAILED');
  assert.equal(workerEntry.fileBase, 'pdf.worker.entry.min.js');
  assert.equal(deriveResult({ milestones, failures: [workerEntry] }), 'FAILED');
  assert.equal(deriveResult({ milestones: { ...milestones, zoom: false }, failures: [] }), 'FAILED');
  assert.doesNotMatch(runner, /WORKFLOW_PDF_PERF_RESULT=PASSED['"`]/);
});

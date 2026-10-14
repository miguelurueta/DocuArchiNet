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
const {
  createAuthenticatedWorkflowSession
} = require('../tests/support/authenticated-workflow-session.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const allowedProfileFields = new Set([
  'baseUrl', 'module', 'taskId', 'documentOrdinal', 'documentTitle', 'iterations', 'scrollSteps',
  'timeoutMilliseconds', 'headless', 'browser', 'ignoreHttpsErrors'
]);
const allowedFailureFields = Object.freeze(['code', 'source', 'type', 'fileBase', 'line']);
const requiredSources = new Set(['main_frame', 'viewer', 'worker']);
const requestCountCategories = Object.freeze(['core', 'graph', 'wacom', 'print', 'fabric', 'prettify', 'other']);

function classifyViewerRequest(value) {
  let pathname = '';
  try { pathname = new URL(value, 'https://viewer.invalid/').pathname.toLowerCase(); } catch { return 'other'; }
  if (/\/wacom\//.test(pathname)) return 'wacom';
  if (/\/printer_js\//.test(pathname)) return 'print';
  if (/fabric(?:\.min)?\.js$/.test(pathname)) return 'fabric';
  if (/prettify(?:\.min)?\.js$/.test(pathname)) return 'prettify';
  if (/jquery-ui|contextmenu|touch-punch/.test(pathname)) return 'graph';
  if (/\/pdfjs\/pdf_anotate_view\/|\/workflow\/handler_image_wf\.ashx$/.test(pathname)) return 'core';
  return 'other';
}

function createRequestCounts() {
  return Object.seal(Object.fromEntries(requestCountCategories.map(category => [category, 0])));
}

function sanitizeHandlerExchange(input = {}) {
  const requestHeaders = input.requestHeaders || {};
  const responseHeaders = input.responseHeaders || {};
  const status = Number(input.status);
  const contentLength = Number(responseHeaders['content-length']);
  const contentRangeMatch = String(responseHeaders['content-range'] || '').match(/\/(\d+)$/);
  const contentRangeLength = Number(contentRangeMatch?.[1]);
  return Object.freeze({
    status: Number.isInteger(status) && status >= 100 && status <= 599 ? status : 0,
    rangeRequested: typeof requestHeaders.range === 'string' && /^bytes=/i.test(requestHeaders.range),
    acceptRangesBytes: String(responseHeaders['accept-ranges'] || '').toLowerCase() === 'bytes',
    contentRangePresent: typeof responseHeaders['content-range'] === 'string',
    contentLengthBytes: Number.isSafeInteger(contentLength) && contentLength >= 0 ? contentLength : 0,
    documentLengthBytes: Number.isSafeInteger(contentRangeLength) && contentRangeLength > 0
      ? contentRangeLength
      : (status === 200 && Number.isSafeInteger(contentLength) && contentLength > 0 ? contentLength : 0),
    etagPresent: typeof responseHeaders.etag === 'string' && responseHeaders.etag.length > 0,
    lastModifiedPresent: typeof responseHeaders['last-modified'] === 'string',
    privateCache: /(?:^|,)\s*private(?:\s|,|$)/i.test(String(responseHeaders['cache-control'] || '')),
    varyCookie: String(responseHeaders.vary || '').split(',').some(value => value.trim().toLowerCase() === 'cookie')
  });
}

function summarizeHandlerExchanges(exchanges = []) {
  const safe = exchanges.map(sanitizeHandlerExchange);
  return Object.freeze({
    requestCount: safe.length,
    rangeRequestCount: safe.filter(item => item.rangeRequested).length,
    statuses: Object.freeze(safe.reduce((counts, item) => {
      const key = String(item.status);
      counts[key] = (counts[key] || 0) + 1;
      return counts;
    }, {})),
    declaredResponseBytes: safe.reduce((sum, item) => sum + item.contentLengthBytes, 0),
    documentLengthBytes: safe.reduce((maximum, item) => Math.max(maximum, item.documentLengthBytes), 0),
    contract: Object.freeze({
      acceptRanges: safe.some(item => item.acceptRangesBytes),
      contentRange: safe.some(item => item.contentRangePresent),
      etag: safe.some(item => item.etagPresent),
      lastModified: safe.some(item => item.lastModifiedPresent),
      validators: safe.some(item => item.etagPresent && item.lastModifiedPresent),
      privateCache: safe.some(item => item.privateCache),
      varyCookie: safe.some(item => item.varyCookie)
    })
  });
}

function isViewerFrameRequest(requestUrl, frameUrl) {
  const viewerPath = /\/pdfjs\/pdf_anotate_view\/index\.html$/i;
  try {
    if (viewerPath.test(new URL(requestUrl).pathname)) return true;
  } catch { return false; }
  try { return viewerPath.test(new URL(frameUrl).pathname); } catch { return false; }
}

function safeFileBase(value) {
  if (typeof value !== 'string' || !value) return '';
  const withoutQuery = value.split(/[?#]/, 1)[0];
  const base = withoutQuery.split(/[\\/]/).pop() || '';
  return /^[A-Za-z0-9._-]{1,120}$/.test(base) ? base : '';
}

function classifyBrowserFailure(input = {}) {
  const knownSources = new Set(['main_frame', 'viewer', 'worker', 'optional_resource']);
  const source = knownSources.has(input.source) ? input.source : 'main_frame';
  const types = new Set(['error', 'unhandledrejection', 'pageerror', 'http', 'resource']);
  const type = types.has(input.type) ? input.type : 'unknown';
  const status = Number.isInteger(Number(input.status)) ? Number(input.status) : 0;
  let code = 'WORKFLOW_PDF_UNKNOWN_FAILURE';
  if (source === 'optional_resource' && ['http', 'resource'].includes(type)) {
    code = 'WORKFLOW_PDF_OPTIONAL_RESOURCE_FAILED';
  } else if (type === 'http') {
    code = status >= 500 ? 'WORKFLOW_PDF_HTTP_SERVER_ERROR' : 'WORKFLOW_PDF_HTTP_ERROR';
  } else if (type === 'resource') {
    code = 'WORKFLOW_PDF_RESOURCE_FAILED';
  } else if (type === 'unhandledrejection') {
    code = 'WORKFLOW_PDF_UNHANDLED_REJECTION';
  } else if (type === 'pageerror') {
    code = 'WORKFLOW_PDF_PAGE_ERROR';
  } else if (type === 'error') {
    code = source === 'worker' ? 'WORKFLOW_PDF_WORKER_ERROR' : 'WORKFLOW_PDF_SCRIPT_ERROR';
  }
  return Object.freeze({
    code,
    source,
    type,
    fileBase: safeFileBase(input.fileName),
    line: Math.max(0, Number.isSafeInteger(Number(input.line)) ? Number(input.line) : 0)
  });
}

function classifyPageError(error) {
  const stack = typeof error?.stack === 'string' ? error.stack : '';
  const match = stack.match(/((?:https?:\/\/)?[^\s()]+?\.js)(?:[?#][^\s():]*)?(?::(\d+))(?::\d+)?/i);
  const location = match?.[1] || '';
  const source = /\/pdfjs\/pdf_anotate_view\//i.test(location) || /(?:pdf[_-]viewer|pdf\.worker)/i.test(location)
    ? 'viewer'
    : 'main_frame';
  return classifyBrowserFailure({
    type: 'pageerror',
    source,
    fileName: location,
    line: match ? Number(match[2]) : 0
  });
}

function deriveResult(input = {}) {
  const milestones = input.milestones || {};
  const requiredMilestones = ['documentSelected', 'firstCanvas', 'scroll', 'zoom', 'reloads'];
  if (Object.prototype.hasOwnProperty.call(milestones, 'transportPolicy')) requiredMilestones.push('transportPolicy');
  if (Object.prototype.hasOwnProperty.call(milestones, 'partialFirstCanvas')) requiredMilestones.push('partialFirstCanvas');
  const complete = requiredMilestones
    .every(name => milestones[name] === true);
  const failures = Array.isArray(input.failures) ? input.failures : [];
  const hasRequiredFailure = failures.some(item => !item || requiredSources.has(item.source) ||
    item.code !== 'WORKFLOW_PDF_OPTIONAL_RESOURCE_FAILED');
  if (!complete || hasRequiredFailure) {
    return 'FAILED';
  }
  return failures.length > 0 ? 'PASSED_WITH_OBSERVATIONS' : 'PASSED';
}

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function failWithDiagnostic(code, diagnostic) {
  const error = new Error(code);
  error.code = code;
  error.diagnostic = diagnostic;
  throw error;
}

function positiveInteger(value, code, maximum) {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number <= 0 || (maximum && number > maximum)) fail(code);
  return number;
}

function profilePath(argv = process.argv) {
  const index = argv.indexOf('--profile');
  if (index < 0 || !argv[index + 1]) fail('WORKFLOW_PDF_PERF_PROFILE_REQUIRED');
  return path.resolve(argv[index + 1]);
}

function loadProfile(target) {
  let input;
  try {
    input = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, ''));
  } catch {
    fail('WORKFLOW_PDF_PERF_PROFILE_INVALID');
  }
  if (!input || typeof input !== 'object' || Array.isArray(input)) fail('WORKFLOW_PDF_PERF_PROFILE_INVALID');
  if (Object.keys(input).some(key => !allowedProfileFields.has(key))) fail('WORKFLOW_PDF_PERF_PROFILE_FIELD_NOT_ALLOWED');
  let baseUrl;
  try {
    baseUrl = new URL(input.baseUrl);
  } catch {
    fail('WORKFLOW_PDF_PERF_BASE_URL_INVALID');
  }
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('WORKFLOW_PDF_PERF_BASE_URL_INVALID');
  if (typeof input.module !== 'string' || !/^[A-Za-z0-9][A-Za-z0-9 _-]{1,79}$/.test(input.module)) fail('WORKFLOW_PDF_PERF_MODULE_INVALID');
  if (input.ignoreHttpsErrors !== true && input.ignoreHttpsErrors !== false) fail('WORKFLOW_PDF_PERF_TLS_FLAG_INVALID');
  if (input.headless !== undefined && typeof input.headless !== 'boolean') fail('WORKFLOW_PDF_PERF_HEADLESS_INVALID');
  let browser;
  if (input.browser !== undefined) {
    if (!input.browser || typeof input.browser !== 'object' || Array.isArray(input.browser) ||
        Object.keys(input.browser).length !== 1 || !['chrome', 'msedge'].includes(input.browser.channel)) {
      fail('WORKFLOW_PDF_PERF_BROWSER_INVALID');
    }
    browser = Object.freeze({ channel: input.browser.channel });
  }
  return Object.freeze({
    baseUrl: baseUrl.toString(),
    module: input.module.trim(),
    taskId: positiveInteger(input.taskId, 'WORKFLOW_PDF_PERF_TASK_INVALID'),
    documentOrdinal: positiveInteger(input.documentOrdinal ?? 1, 'WORKFLOW_PDF_PERF_DOCUMENT_ORDINAL_INVALID', 1000),
    documentTitle: input.documentTitle === undefined
      ? undefined
      : (() => {
          const value = String(input.documentTitle).trim();
          if (!value || value.length > 255 || /[\r\n\t]/.test(value)) fail('WORKFLOW_PDF_PERF_DOCUMENT_TITLE_INVALID');
          return value;
        })(),
    iterations: positiveInteger(input.iterations ?? 3, 'WORKFLOW_PDF_PERF_ITERATIONS_INVALID', 10),
    scrollSteps: positiveInteger(input.scrollSteps ?? 12, 'WORKFLOW_PDF_PERF_SCROLL_STEPS_INVALID', 100),
    timeoutMilliseconds: positiveInteger(input.timeoutMilliseconds ?? 120000, 'WORKFLOW_PDF_PERF_TIMEOUT_INVALID', 600000),
    headless: input.headless !== false,
    browser,
    ignoreHttpsErrors: input.ignoreHttpsErrors
  });
}

async function selectWorkflowTask(page, profile) {
  await page.goto(new URL('workflow/Webworkflow.aspx', profile.baseUrl).toString(), {
    waitUntil: 'domcontentloaded', timeout: profile.timeoutMilliseconds
  });
  const taskListPermissionDenied = await page.locator('#LabelEspera').evaluate(element =>
    /(?:el usuario )?no tiene permiso para listar tareas/i.test(String(element?.textContent || '').trim())
  ).catch(() => false);
  if (taskListPermissionDenied) fail('WORKFLOW_PDF_PERF_TASK_LIST_PERMISSION_DENIED');
  const expected = String(profile.taskId);
  const selectedTask = page.locator('#Hidden_id_tarea_selecionada');
  await selectedTask.waitFor({ state: 'attached', timeout: profile.timeoutMilliseconds });
  if (await selectedTask.inputValue() === expected) return;
  await page.evaluate(() => {
    if (typeof window.hide_area_workflow_seleccion === 'function') window.hide_area_workflow_seleccion();
  });
  let selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${profile.taskId}"]:visible`).first();
  if (!await selectCommand.count()) {
    const search = page.locator('#auto_complex:visible');
    await search.waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
    await search.fill(expected);
    await page.locator('button[title="consultar lista"]:visible').click();
    selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${profile.taskId}"]:visible`).first();
  }
  await selectCommand.waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
  await selectCommand.click();
  await page.waitForFunction(
    value => document.querySelector('#Hidden_id_tarea_selecionada')?.value === value,
    expected,
    { timeout: profile.timeoutMilliseconds }
  ).catch(() => fail('WORKFLOW_PDF_PERF_TASK_SELECTION_FAILED'));
}

async function installPerformanceProbe(context) {
  await context.addInitScript(() => {
    const probe = window.__workflowPdfPerformance = { longTasks: [], errors: [] };
    const fileBase = value => {
      const clean = String(value || '').split(/[?#]/, 1)[0];
      const base = clean.split(/[\\/]/).pop() || '';
      return /^[A-Za-z0-9._-]{1,120}$/.test(base) ? base : '';
    };
    const source = () => /\/pdfjs\/pdf_anotate_view\//i.test(location.pathname) ? 'viewer' : 'main_frame';
    window.addEventListener('error', event => {
      const fromWorker = typeof Worker === 'function' && event.target instanceof Worker;
      probe.errors.push({ code: fromWorker ? 'WORKFLOW_PDF_WORKER_ERROR' : 'WORKFLOW_PDF_SCRIPT_ERROR', source: fromWorker ? 'worker' : source(), type: 'error', fileBase: fileBase(event.filename), line: Math.max(0, Number(event.lineno || 0)) });
    });
    window.addEventListener('unhandledrejection', () => {
      probe.errors.push({ code: 'WORKFLOW_PDF_UNHANDLED_REJECTION', source: source(), type: 'unhandledrejection', fileBase: '', line: 0 });
    });
    try {
      const observer = new PerformanceObserver(list => {
        for (const entry of list.getEntries()) {
          probe.longTasks.push({ startTime: Math.round(entry.startTime), duration: Math.round(entry.duration) });
        }
      });
      observer.observe({ type: 'longtask', buffered: true });
    } catch {
      probe.longTaskObserverUnavailable = true;
    }
  });
}

async function waitForViewerFrame(page, timeout) {
  await page.waitForFunction(() => {
    const raw = document.querySelector('#ifrm_visor_')?.getAttribute('src') || '';
    return /pdfjs\/pdf_anotate_view\/index\.html/i.test(raw);
  }, null, { timeout }).catch(() => fail('WORKFLOW_PDF_PERF_ANNOTATION_VIEWER_NOT_OPENED'));
  const handle = await page.locator('#ifrm_visor_').elementHandle();
  const frame = await handle?.contentFrame();
  if (!frame) fail('WORKFLOW_PDF_PERF_VIEWER_FRAME_UNAVAILABLE');
  return frame;
}

async function waitForFirstCanvas(page, timeout, network) {
  try {
    await page.waitForFunction(() => {
      const iframe = document.querySelector('#ifrm_visor_');
      const frameDocument = iframe?.contentDocument;
      const canvas = frameDocument?.querySelector('#pdf-container_ canvas');
      return !!canvas && canvas.width > 0 && canvas.height > 0 &&
        frameDocument.querySelectorAll('#pdf-container_ .pdfpage').length > 0;
    }, null, { timeout });
  } catch {
    const state = await page.evaluate(() => {
      const iframe = document.querySelector('#ifrm_visor_');
      const frameWindow = iframe?.contentWindow;
      const frameDocument = iframe?.contentDocument;
      if (!frameWindow || !frameDocument) return { liveFrameUnavailable: true };
      const probe = frameWindow.__workflowPdfPerformance || {};
      const canvases = Array.from(frameDocument.querySelectorAll('#pdf-container_ canvas'));
      const container = frameDocument.querySelector('#pdf-container_');
      const pageElements = frameDocument.querySelectorAll('#pdf-container_ .pdfpage').length;
      return {
        documentReadyState: frameDocument.readyState,
        pdfLoaded: pageElements > 0,
        pageCount: Number(frameDocument.querySelector('#num_page_total')?.value || pageElements || 0),
        pageElements,
        canvasCount: canvases.length,
        nonZeroCanvasCount: canvases.filter(canvas => canvas.width > 0 && canvas.height > 0).length,
        containerWidth: Math.round(container?.clientWidth || 0),
        containerHeight: Math.round(container?.clientHeight || 0),
        classifiedErrors: Array.isArray(probe.errors) ? probe.errors : []
      };
    }).catch(() => ({ frameEvaluationFailed: true }));
    const diagnostic = Object.freeze({
      ...state,
      handlerStatuses: [...network.handlerStatuses],
      viewerHttpErrors: network.viewerHttpErrors,
      failedHandlerRequests: network.failedHandlerRequests
    });
    let code = 'WORKFLOW_PDF_PERF_FIRST_CANVAS_TIMEOUT';
    if (diagnostic.frameEvaluationFailed || diagnostic.liveFrameUnavailable) code = 'WORKFLOW_PDF_PERF_LIVE_FRAME_UNAVAILABLE';
    else if (!diagnostic.pdfLoaded) code = 'WORKFLOW_PDF_PERF_PDF_NOT_LOADED';
    else if (diagnostic.pageElements === 0) code = 'WORKFLOW_PDF_PERF_PAGE_STRUCTURE_NOT_CREATED';
    else if (diagnostic.canvasCount === 0) code = 'WORKFLOW_PDF_PERF_FIRST_CANVAS_NOT_RENDERED';
    else if (diagnostic.nonZeroCanvasCount === 0) code = 'WORKFLOW_PDF_PERF_CANVAS_ZERO_SIZE';
    failWithDiagnostic(code, diagnostic);
  }
  return waitForViewerFrame(page, timeout);
}

async function captureViewerMetrics(frame, elapsedMs, handlerExchanges = []) {
  const browserMetrics = await frame.evaluate((outerElapsed) => {
    const viewer = typeof pdfViewer === 'undefined' ? null : pdfViewer;
    const probe = window.__workflowPdfPerformance || {};
    const resources = performance.getEntriesByType('resource');
    const navigation = performance.getEntriesByType('navigation')[0];
    const pdfResources = resources.filter(entry => {
      try { return /\/workflow\/Handler_image_wf\.ashx$/i.test(new URL(entry.name).pathname); } catch { return false; }
    });
    const canvases = Array.from(document.querySelectorAll('#pdf-container_ canvas'));
    const longTasks = Array.isArray(probe.longTasks) ? probe.longTasks : [];
    return {
      firstCanvasMs: Math.round(outerElapsed),
      domInteractiveMs: Math.round(navigation?.domInteractive || 0),
      viewerReadyMs: Math.round(performance.now()),
      pageCount: Number(viewer?.pdf?.numPages || document.querySelectorAll('#pdf-container_ .pdfpage').length || 0),
      canvasCount: canvases.length,
      canvasPixels: canvases.reduce((sum, canvas) => sum + (canvas.width * canvas.height), 0),
      pdfLoading: {
        mode: viewer?.loadingMode === 'range' ? 'range' : 'stream',
        disableRange: viewer?.pdf?.loadingParams?.disableRange === true,
        disableStream: viewer?.pdf?.loadingParams?.disableStream === true,
        disableAutoFetch: viewer?.pdf?.loadingParams?.disableAutoFetch === true
      },
      handler: pdfResources.length > 0 ? {
        requestCount: pdfResources.length,
        durationMs: Math.round(Math.max(...pdfResources.map(entry => entry.responseEnd)) - Math.min(...pdfResources.map(entry => entry.startTime))),
        ttfbMs: Math.round(Math.min(...pdfResources.map(entry => entry.responseStart - entry.startTime))),
        downloadMs: Math.round(Math.max(...pdfResources.map(entry => entry.responseEnd - entry.responseStart))),
        transferBytes: pdfResources.reduce((sum, entry) => sum + Number(entry.transferSize || 0), 0),
        encodedBytes: pdfResources.reduce((sum, entry) => sum + Number(entry.encodedBodySize || 0), 0),
        decodedBytes: pdfResources.reduce((sum, entry) => sum + Number(entry.decodedBodySize || 0), 0)
      } : null,
      longTaskCount: longTasks.length,
      longTaskTotalMs: longTasks.reduce((sum, entry) => sum + Number(entry.duration || 0), 0),
      longestTaskMs: longTasks.reduce((maximum, entry) => Math.max(maximum, Number(entry.duration || 0)), 0),
      classifiedErrors: Array.isArray(probe.errors) ? probe.errors : []
    };
  }, elapsedMs);
  const streamingContract = summarizeHandlerExchanges(handlerExchanges);
  const transferred = Number(browserMetrics.handler?.transferBytes || 0);
  const partialBeforeFirstCanvas = streamingContract.rangeRequestCount > 0 &&
    streamingContract.documentLengthBytes > 0 && transferred < streamingContract.documentLengthBytes;
  const transportPolicySatisfied = browserMetrics.pdfLoading.mode === 'range'
    ? partialBeforeFirstCanvas
    : streamingContract.rangeRequestCount === 0 && Number(streamingContract.statuses['200'] || 0) > 0;
  return Object.freeze({ ...browserMetrics, streamingContract, partialBeforeFirstCanvas, transportPolicySatisfied });
}

async function measureScroll(frame, steps) {
  return frame.evaluate(async count => {
    const container = document.querySelector('#pdf-container_');
    if (!container) throw new Error('WORKFLOW_PDF_PERF_SCROLL_CONTAINER_UNAVAILABLE');
    const probe = window.__workflowPdfPerformance || { longTasks: [] };
    const before = Array.isArray(probe.longTasks) ? probe.longTasks.length : 0;
    const started = performance.now();
    const maximum = Math.max(0, container.scrollHeight - container.clientHeight);
    for (let index = 1; index <= count; index += 1) {
      container.scrollTop = Math.round(maximum * index / count);
      await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
    }
    const durationMs = Math.round(performance.now() - started);
    const observed = (Array.isArray(probe.longTasks) ? probe.longTasks : []).slice(before);
    return {
      durationMs,
      steps: count,
      distancePixels: maximum,
      longTaskCount: observed.length,
      longTaskTotalMs: observed.reduce((sum, entry) => sum + Number(entry.duration || 0), 0)
    };
  }, steps).catch(error => fail(error?.message === 'WORKFLOW_PDF_PERF_SCROLL_CONTAINER_UNAVAILABLE'
    ? error.message : 'WORKFLOW_PDF_PERF_SCROLL_FAILED'));
}

async function measureZoom(frame) {
  return frame.evaluate(async () => {
    const viewer = typeof pdfViewer === 'undefined' ? null : pdfViewer;
    if (!viewer || typeof viewer.setZoom !== 'function') throw new Error('WORKFLOW_PDF_PERF_ZOOM_UNAVAILABLE');
    const probe = window.__workflowPdfPerformance || { longTasks: [] };
    const beforeTasks = Array.isArray(probe.longTasks) ? probe.longTasks.length : 0;
    const beforeZoom = Number(viewer.getZoom?.() || 0);
    const started = performance.now();
    viewer.setZoom('in');
    const deadline = performance.now() + 15000;
    let stableSamples = 0;
    let previousSignature = '';
    while (performance.now() < deadline && stableSamples < 3) {
      await new Promise(resolve => setTimeout(resolve, 100));
      const canvases = Array.from(document.querySelectorAll('#pdf-container_ canvas'));
      const signature = canvases.map(canvas => `${canvas.width}x${canvas.height}`).join(',');
      if (signature && signature === previousSignature) stableSamples += 1;
      else stableSamples = 0;
      previousSignature = signature;
    }
    const observed = (Array.isArray(probe.longTasks) ? probe.longTasks : []).slice(beforeTasks);
    return {
      durationMs: Math.round(performance.now() - started),
      beforeZoom,
      afterZoom: Number(viewer.getZoom?.() || 0),
      stable: stableSamples >= 3,
      longTaskCount: observed.length,
      longTaskTotalMs: observed.reduce((sum, entry) => sum + Number(entry.duration || 0), 0)
    };
  }).catch(error => fail(error?.message === 'WORKFLOW_PDF_PERF_ZOOM_UNAVAILABLE'
    ? error.message : 'WORKFLOW_PDF_PERF_ZOOM_FAILED'));
}

function summarize(samples) {
  const values = samples.map(sample => sample.firstCanvasMs).sort((left, right) => left - right);
  const median = values[Math.floor(values.length / 2)];
  return Object.freeze({ minimumMs: values[0], medianMs: median, maximumMs: values[values.length - 1] });
}

async function writeEvidence(evidence, fileName = 'workflow-pdf-viewer-performance.json') {
  const destination = path.join(e2eRoot, 'artifacts', fileName);
  await fsp.mkdir(path.dirname(destination), { recursive: true });
  await fsp.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function exerciseCapabilityMatrix(page, frame) {
  const graphAndSignature = await frame.evaluate(async () => {
    await window.loadViewerCapability('graph');
    await ensureStampConfiguration(false);
    initializeViewerContextMenus();
    const permissionGranted = Number(pdfViewer.anotate_add_firma) !== 0;
    const before = new Set(Array.from(document.querySelectorAll('[id^="div-drav-image-"]')).map(item => item.id));
    if (permissionGranted) pdfViewer._Add_interface_anotate(1, 0, 0);
    const overlay = Array.from(document.querySelectorAll('[id^="div-drav-image-"]')).find(item => !before.has(item.id));
    const overlayCreated = Boolean(overlay);
    overlay?.remove();
    return {
      ready: Boolean(window.jQuery?.fn?.draggable && window.jQuery?.fn?.resizable && window.jQuery?.fn?.contextMenu),
      configurationLoaded: Boolean(pdfViewer.itex_config_stamp?.[0]?.error_sistema === 'YES'),
      signature: { permissionGranted, overlayCreated, removedWithoutSave: overlayCreated && !document.getElementById(overlay.id) }
    };
  });

  await page.route('**/wacom/q.js', route => route.abort('failed'));
  const wacomUnavailable = await frame.evaluate(async () => {
    const container = document.querySelector('#pdf-container_');
    const pdfReference = pdfViewer.pdf;
    const before = {
      activePage: pdfViewer.getActivePageNumber(),
      pageCount: Number(pdfViewer.pdf?.numPages || 0),
      scrollTop: Number(container?.scrollTop || 0),
      zoom: Number(pdfViewer.getZoom?.() || 0)
    };
    let rejected = false;
    try { await window.loadViewerCapability('wacom'); } catch { rejected = true; }
    return {
      rejected,
      statePreserved: pdfReference === pdfViewer.pdf &&
        before.activePage === pdfViewer.getActivePageNumber() &&
        before.pageCount === Number(pdfViewer.pdf?.numPages || 0) &&
        before.scrollTop === Number(container?.scrollTop || 0) &&
        before.zoom === Number(pdfViewer.getZoom?.() || 0)
    };
  });
  await page.unroute('**/wacom/q.js');

  const remaining = await frame.evaluate(async () => {
    await window.loadViewerCapability('wacom');
    await window.loadViewerCapability('print');
    await window.loadViewerCapability('fabric');
    await window.loadViewerCapability('prettify');
    const canvas = document.querySelector('#pdf-container_ canvas');
    return {
      wacomSdkReady: typeof window.captureFromSTU === 'function',
      printSdkReady: typeof window.printJS === 'function',
      fabricReady: Boolean(window.fabric?.Canvas),
      prettifyReady: Boolean(window.PR?.prettyPrint),
      downloadContractReady: typeof donwload_pdfTool_viwer === 'function' && Boolean(document.getElementById('pdf_btn_down_load')),
      downloadPermissionGranted: Number(pdfViewer.anotate_save) !== 0,
      readStable: Boolean(canvas && canvas.width > 0 && canvas.height > 0 && pdfViewer.pdf?.numPages > 0)
    };
  });

  const signatureAccepted = graphAndSignature.signature.permissionGranted
    ? graphAndSignature.signature.overlayCreated && graphAndSignature.signature.removedWithoutSave
    : !graphAndSignature.signature.overlayCreated;
  const passed = graphAndSignature.ready && graphAndSignature.configurationLoaded && signatureAccepted &&
    wacomUnavailable.rejected && wacomUnavailable.statePreserved && remaining.wacomSdkReady &&
    remaining.printSdkReady && remaining.fabricReady && remaining.prettifyReady &&
    remaining.downloadContractReady && remaining.readStable;
  return Object.freeze({
    graphAndSignature,
    wacomUnavailable,
    remaining,
    physicalWacomPresent: false,
    persistedDocumentChange: false,
    result: passed ? 'PASSED' : 'FAILED'
  });
}

async function runMeasurement(profile, environment, options = {}) {
  const browser = await chromium.launch({ ...(profile.browser || {}), headless: profile.headless });
  let context;
  try {
    context = await createAuthenticatedWorkflowSession(browser, {
      baseUrl: profile.baseUrl,
      environment,
      moduleEnvironmentVariable: 'WORKFLOW_PDF_PERF_MODULE',
      userEnvironmentVariable: 'WORKFLOW_PDF_PERF_AUTHORIZED_USER',
      passwordEnvironmentVariable: 'WORKFLOW_PDF_PERF_AUTHORIZED_PASSWORD',
      timeoutMilliseconds: profile.timeoutMilliseconds,
      ignoreHTTPSErrors: profile.ignoreHttpsErrors
    });
    await installPerformanceProbe(context);
    const page = await context.newPage();
    const failures = [];
    const network = {
      handlerStatuses: [], viewerHttpErrors: 0, failedHandlerRequests: 0,
      handlerExchanges: [],
      requestCounts: createRequestCounts()
    };
    page.on('request', request => {
      let frameUrl = '';
      try { frameUrl = request.frame().url(); } catch { /* Solicitudes sin frame no se atribuyen al visor. */ }
      if (!isViewerFrameRequest(request.url(), frameUrl)) return;
      const category = classifyViewerRequest(request.url());
      network.requestCounts[category] += 1;
    });
    page.on('response', response => {
      let pathname = '';
      try { pathname = new URL(response.url()).pathname; } catch { return; }
      if (/\/workflow\/Handler_image_wf\.ashx$/i.test(pathname)) {
        network.handlerStatuses.push(response.status());
        network.handlerExchanges.push({
          status: response.status(),
          requestHeaders: response.request().headers(),
          responseHeaders: response.headers()
        });
      }
      if (response.status() >= 400) {
        let source = 'main_frame';
        const category = classifyViewerRequest(pathname);
        if (!['core', 'other'].includes(category)) {
          source = 'optional_resource';
        } else if (/\/pdfjs\/pdf_anotate_view\//i.test(pathname)) {
          source = 'viewer';
          network.viewerHttpErrors += 1;
        } else if (/\.(?:map|ico|woff2?|ttf)$/i.test(pathname)) {
          source = 'optional_resource';
        }
        failures.push(classifyBrowserFailure({ type: 'http', source, status: response.status(), fileName: pathname }));
      }
    });
    page.on('requestfailed', request => {
      let source = 'main_frame';
      let fileName = '';
      try {
        const pathname = new URL(request.url()).pathname;
        fileName = pathname;
        if (/\/workflow\/Handler_image_wf\.ashx$/i.test(pathname)) {
          network.failedHandlerRequests += 1;
          return;
        }
        const category = classifyViewerRequest(pathname);
        if (!['core', 'other'].includes(category) || /\.(?:map|ico|woff2?|ttf)$/i.test(pathname)) {
          source = 'optional_resource';
        } else if (/\/pdfjs\/pdf_anotate_view\//i.test(pathname)) {
          source = 'viewer';
        }
      } catch { /* La evidencia no conserva URL ni causa de transporte. */ }
      failures.push(classifyBrowserFailure({ type: 'resource', source, fileName }));
    });
    page.on('pageerror', error => {
      failures.push(classifyPageError(error));
    });
    await selectWorkflowTask(page, profile);
    const rows = page.locator('#GridView_list_documento_relacion_wf tr[id_wf]:visible');
    const documents = page.locator('#GridView_list_documento_relacion_wf [tip_event="vis_doc_selecion_wf"]:visible');
    await documents.first().waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds })
      .catch(() => fail('WORKFLOW_PDF_PERF_DOCUMENTS_UNAVAILABLE'));
    let selectedDocument;
    if (profile.documentTitle) {
      const expectedTitle = profile.documentTitle.toLocaleLowerCase('es');
      const matchingIndexes = await rows.evaluateAll((elements, expected) => elements
        .map((row, index) => ({ index, text: String(row.textContent || '').replace(/\s+/g, ' ').trim().toLocaleLowerCase('es') }))
        .filter(item => item.text.includes(expected))
        .map(item => item.index), expectedTitle);
      if (matchingIndexes.length === 0) fail('WORKFLOW_PDF_PERF_DOCUMENT_TITLE_NOT_FOUND');
      if (matchingIndexes.length > 1) fail('WORKFLOW_PDF_PERF_DOCUMENT_TITLE_AMBIGUOUS');
      selectedDocument = rows.nth(matchingIndexes[0]).locator('[tip_event="vis_doc_selecion_wf"]:visible').first();
    } else {
      if (await documents.count() < profile.documentOrdinal) fail('WORKFLOW_PDF_PERF_DOCUMENT_ORDINAL_NOT_FOUND');
      selectedDocument = documents.nth(profile.documentOrdinal - 1);
    }

    const samples = [];
    let handlerExchangeStart = network.handlerExchanges.length;
    let started = performance.now();
    await selectedDocument.click();
    console.log('WORKFLOW_PDF_PERF_DOCUMENT_SELECTED=true');
    let frame = await waitForViewerFrame(page, profile.timeoutMilliseconds);
    frame = await waitForFirstCanvas(page, profile.timeoutMilliseconds, network);
    samples.push(await captureViewerMetrics(frame, performance.now() - started,
      network.handlerExchanges.slice(handlerExchangeStart)));
    console.log('WORKFLOW_PDF_PERF_ITERATION_1_FIRST_CANVAS=true');
    const scroll = await measureScroll(frame, profile.scrollSteps);
    console.log('WORKFLOW_PDF_PERF_SCROLL_COMPLETED=true');
    const zoom = await measureZoom(frame);
    console.log('WORKFLOW_PDF_PERF_ZOOM_COMPLETED=true');

    for (let iteration = 1; iteration < profile.iterations; iteration += 1) {
      handlerExchangeStart = network.handlerExchanges.length;
      started = performance.now();
      const navigated = page.waitForEvent('framenavigated', {
        predicate: candidate => candidate === frame,
        timeout: profile.timeoutMilliseconds
      });
      await frame.evaluate(() => window.location.reload()).catch(error => {
        if (!/Execution context was destroyed|Target page, context or browser has been closed/i.test(String(error?.message || ''))) throw error;
      });
      await navigated;
      frame = await waitForViewerFrame(page, profile.timeoutMilliseconds);
      frame = await waitForFirstCanvas(page, profile.timeoutMilliseconds, network);
      samples.push(await captureViewerMetrics(frame, performance.now() - started,
        network.handlerExchanges.slice(handlerExchangeStart)));
      console.log(`WORKFLOW_PDF_PERF_ITERATION_${iteration + 1}_FIRST_CANVAS=true`);
    }

    const capabilityMatrix = options.capabilityMatrix ? await exerciseCapabilityMatrix(page, frame) : null;
    if (network.failedHandlerRequests > 0 && !network.handlerStatuses.some(status => status === 200 || status === 206)) {
      failures.push(classifyBrowserFailure({ type: 'resource', source: 'main_frame', fileName: 'Handler_image_wf.ashx' }));
    }
    const probeFailures = samples.flatMap(sample => sample.classifiedErrors || []);
    const uniqueFailures = [...new Map([...failures, ...probeFailures].map(item => [JSON.stringify(item), item])).values()];
    const allRangeMode = samples.every(sample => sample.pdfLoading?.mode === 'range');
    const milestones = Object.freeze({
      documentSelected: true,
      firstCanvas: samples.length === profile.iterations,
      scroll: scroll?.steps === profile.scrollSteps,
      zoom: zoom?.stable === true,
      reloads: samples.length === profile.iterations,
      transportPolicy: samples.every(sample => sample.transportPolicySatisfied === true),
      ...(allRangeMode ? { partialFirstCanvas: samples.every(sample => sample.partialBeforeFirstCanvas === true) } : {})
    });
    const result = deriveResult({ milestones, failures: uniqueFailures });
    const evidence = Object.freeze({
      schemaVersion: 4,
      scenario: capabilityMatrix ? 'workflow-pdf-viewer-capability-matrix' : 'workflow-pdf-viewer-performance-read',
      capturedAtUtc: new Date().toISOString(),
      mode: profile.headless ? 'headless' : 'headed',
      iterations: samples.length,
      summary: summarize(samples),
      samples,
      interactions: { scroll, zoom },
      requestCounts: { ...network.requestCounts },
      ...(capabilityMatrix ? { capabilityMatrix } : {}),
      milestones,
      failures: uniqueFailures,
      result
    });
    await writeEvidence(evidence, capabilityMatrix
      ? 'workflow-pdf-viewer-capability-matrix.json'
      : 'workflow-pdf-viewer-performance.json');
    return evidence;
  } finally {
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

async function main() {
  requireInteractiveConsole();
  const profile = loadProfile(profilePath());
  const values = { WORKFLOW_PDF_PERF_MODULE: profile.module };
  await collectValue(values, 'WORKFLOW_PDF_PERF_AUTHORIZED_USER', 'Cuenta autorizada de Workflow');
  await collectValue(values, 'WORKFLOW_PDF_PERF_AUTHORIZED_PASSWORD', 'Contraseña de Workflow', { secret: true });
  await collectConfirmation(values, 'WORKFLOW_PDF_PERF_ENVIRONMENT_AUTHORIZED', '¿Autoriza el ambiente para medir el visor PDF?');
  await collectConfirmation(values, 'WORKFLOW_PDF_PERF_ACCOUNT_AUTHORIZED', '¿Autoriza esta cuenta y tarea para la medición?');
  await collectConfirmation(values, 'WORKFLOW_PDF_PERF_VIEW_LOG_AUTHORIZED', '¿Autoriza el registro normal de visualización generado al abrir el documento?');
  const capabilityMatrix = process.argv.includes('--capability-matrix');
  const evidence = await runMeasurement(profile, { ...process.env, ...values }, { capabilityMatrix });
  console.log(`WORKFLOW_PDF_PERF_FIRST_CANVAS_MEDIAN_MS=${evidence.summary.medianMs}`);
  console.log(`WORKFLOW_PDF_PERF_PAGE_COUNT=${evidence.samples[0].pageCount}`);
  console.log(`WORKFLOW_PDF_PERF_LONG_TASKS=${evidence.samples.reduce((sum, sample) => sum + sample.longTaskCount, 0)}`);
  console.log(`WORKFLOW_PDF_PERF_LOADING_MODE=${evidence.samples[0].pdfLoading.mode.toUpperCase()}`);
  console.log(`WORKFLOW_PDF_PERF_TRANSPORT_POLICY=${evidence.milestones.transportPolicy ? 'PASSED' : 'FAILED'}`);
  console.log(`WORKFLOW_PDF_PERF_RESULT=${evidence.result}`);
  if (capabilityMatrix) console.log(`WORKFLOW_PDF_CAPABILITY_MATRIX_RESULT=${evidence.capabilityMatrix.result}`);
  if (evidence.result === 'FAILED' || evidence.capabilityMatrix?.result === 'FAILED') process.exitCode = 2;
}

if (require.main === module) {
  main().catch(error => {
    const code = /^[A-Z0-9_]{3,120}$/.test(error?.code || error?.message || '')
      ? (error.code || error.message)
      : 'WORKFLOW_PDF_PERF_FAILED';
    console.error(`La medición E2E del visor PDF se detuvo de forma segura (${code}). No se mostraron rutas, credenciales ni documentos.`);
    if (error?.diagnostic && typeof error.diagnostic === 'object') {
      console.error(`WORKFLOW_PDF_PERF_DIAGNOSTIC=${JSON.stringify(error.diagnostic)}`);
    }
    process.exitCode = 2;
  });
}

module.exports = {
  allowedProfileFields,
  allowedFailureFields,
  classifyViewerRequest,
  createRequestCounts,
  captureViewerMetrics,
  classifyBrowserFailure,
  classifyPageError,
  deriveResult,
  exerciseCapabilityMatrix,
  loadProfile,
  isViewerFrameRequest,
  measureScroll,
  measureZoom,
  requestCountCategories,
  selectWorkflowTask,
  sanitizeHandlerExchange,
  summarizeHandlerExchanges,
  summarize,
  waitForViewerFrame
};

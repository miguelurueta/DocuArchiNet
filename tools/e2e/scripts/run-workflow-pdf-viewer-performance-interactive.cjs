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
    const probe = window.__workflowPdfPerformance = { longTasks: [], errors: 0, rejections: 0 };
    window.addEventListener('error', () => { probe.errors += 1; });
    window.addEventListener('unhandledrejection', () => { probe.rejections += 1; });
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
        scriptErrorCount: Number(probe.errors || 0),
        unhandledRejectionCount: Number(probe.rejections || 0)
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

async function captureViewerMetrics(frame, elapsedMs) {
  return frame.evaluate((outerElapsed) => {
    const viewer = typeof pdfViewer === 'undefined' ? null : pdfViewer;
    const probe = window.__workflowPdfPerformance || {};
    const resources = performance.getEntriesByType('resource');
    const navigation = performance.getEntriesByType('navigation')[0];
    const pdfResource = [...resources].reverse().find(entry => {
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
      handler: pdfResource ? {
        durationMs: Math.round(pdfResource.duration),
        ttfbMs: Math.round(pdfResource.responseStart - pdfResource.startTime),
        downloadMs: Math.round(pdfResource.responseEnd - pdfResource.responseStart),
        transferBytes: Number(pdfResource.transferSize || 0),
        encodedBytes: Number(pdfResource.encodedBodySize || 0),
        decodedBytes: Number(pdfResource.decodedBodySize || 0)
      } : null,
      longTaskCount: longTasks.length,
      longTaskTotalMs: longTasks.reduce((sum, entry) => sum + Number(entry.duration || 0), 0),
      longestTaskMs: longTasks.reduce((maximum, entry) => Math.max(maximum, Number(entry.duration || 0)), 0),
      scriptErrorCount: Number(probe.errors || 0),
      unhandledRejectionCount: Number(probe.rejections || 0)
    };
  }, elapsedMs);
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

async function writeEvidence(evidence) {
  const destination = path.join(e2eRoot, 'artifacts', 'workflow-pdf-viewer-performance.json');
  await fsp.mkdir(path.dirname(destination), { recursive: true });
  await fsp.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function runMeasurement(profile, environment) {
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
    const failures = { requests: 0, pageErrors: 0 };
    const network = { handlerStatuses: [], viewerHttpErrors: 0, failedHandlerRequests: 0 };
    page.on('response', response => {
      let pathname = '';
      try { pathname = new URL(response.url()).pathname; } catch { return; }
      if (/\/workflow\/Handler_image_wf\.ashx$/i.test(pathname)) network.handlerStatuses.push(response.status());
      if (/\/pdfjs\/pdf_anotate_view\//i.test(pathname) && response.status() >= 400) network.viewerHttpErrors += 1;
    });
    page.on('requestfailed', request => {
      failures.requests += 1;
      try {
        if (/\/workflow\/Handler_image_wf\.ashx$/i.test(new URL(request.url()).pathname)) network.failedHandlerRequests += 1;
      } catch { /* La evidencia no conserva URL ni causa de transporte. */ }
    });
    page.on('pageerror', () => { failures.pageErrors += 1; });
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
    let started = performance.now();
    await selectedDocument.click();
    console.log('WORKFLOW_PDF_PERF_DOCUMENT_SELECTED=true');
    let frame = await waitForViewerFrame(page, profile.timeoutMilliseconds);
    frame = await waitForFirstCanvas(page, profile.timeoutMilliseconds, network);
    samples.push(await captureViewerMetrics(frame, performance.now() - started));
    console.log('WORKFLOW_PDF_PERF_ITERATION_1_FIRST_CANVAS=true');
    const scroll = await measureScroll(frame, profile.scrollSteps);
    console.log('WORKFLOW_PDF_PERF_SCROLL_COMPLETED=true');
    const zoom = await measureZoom(frame);
    console.log('WORKFLOW_PDF_PERF_ZOOM_COMPLETED=true');

    for (let iteration = 1; iteration < profile.iterations; iteration += 1) {
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
      samples.push(await captureViewerMetrics(frame, performance.now() - started));
      console.log(`WORKFLOW_PDF_PERF_ITERATION_${iteration + 1}_FIRST_CANVAS=true`);
    }

    const evidence = Object.freeze({
      schemaVersion: 1,
      scenario: 'workflow-pdf-viewer-performance-read',
      capturedAtUtc: new Date().toISOString(),
      mode: profile.headless ? 'headless' : 'headed',
      iterations: samples.length,
      summary: summarize(samples),
      samples,
      interactions: { scroll, zoom },
      failures
    });
    await writeEvidence(evidence);
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
  const evidence = await runMeasurement(profile, { ...process.env, ...values });
  console.log(`WORKFLOW_PDF_PERF_FIRST_CANVAS_MEDIAN_MS=${evidence.summary.medianMs}`);
  console.log(`WORKFLOW_PDF_PERF_PAGE_COUNT=${evidence.samples[0].pageCount}`);
  console.log(`WORKFLOW_PDF_PERF_LONG_TASKS=${evidence.samples.reduce((sum, sample) => sum + sample.longTaskCount, 0)}`);
  console.log('WORKFLOW_PDF_PERF_RESULT=PASSED');
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
  captureViewerMetrics,
  loadProfile,
  measureScroll,
  measureZoom,
  summarize
};

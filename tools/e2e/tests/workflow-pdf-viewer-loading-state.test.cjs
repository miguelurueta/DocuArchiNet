'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const root = path.resolve(__dirname, '../../..');
const read = relative => fs.readFileSync(path.join(root, relative), 'utf8');
const index = read('pdfjs/pdf_anotate_view/index.html');
const styles = read('pdfjs/pdf_anotate_view/styles.css');
const script = read('pdfjs/pdf_anotate_view/script.js');
const viewer = read('pdfjs/pdf_anotate_view/pdf_viewer_anotate.js');

function previewPriorityHarness() {
  const callbacks = new Map(), frames = new Map(), events = [];
  let sequence = 0, resolvePdf, resolveRender;
  const wrapper = { isConnected: true, dataset: {}, setAttribute() {}, appendChild() {},
    remove() { this.isConnected = false; events.push('remove'); } };
  const canvas = { getContext: () => ({}) };
  const task = { promise: new Promise(resolve => { resolvePdf = resolve; }), destroy() { events.push('destroy'); return Promise.resolve(); } };
  const renderTask = { promise: new Promise(resolve => { resolveRender = resolve; }), cancel() { events.push('cancel'); } };
  const page = { getViewport: () => ({ width: 100, height: 200 }), render: () => renderTask };
  const context = vm.createContext({
    document: { getElementById: () => ({ insertBefore() {} }), createElement: tag => tag === 'canvas' ? canvas : wrapper },
    pdfjsLib: { getDocument: () => task },
    fullDocumentFirstPageReady: false, documentLoadFailed: false, documentLoadingTelemetry: {},
    setDocumentLoadingState: state => events.push(state), updateDocumentPageCounter: () => events.push('counter'),
    setTimeout: (callback, ms) => { assert.equal(ms, 3000); callbacks.set(++sequence, callback); return sequence; },
    clearTimeout: id => callbacks.delete(id),
    requestAnimationFrame: callback => { frames.set(++sequence, callback); return sequence; },
    cancelAnimationFrame: id => frames.delete(id)
  });
  vm.runInContext(script.slice(script.indexOf('async function loadFirstPagePreview'), script.indexOf('function removeFirstPagePreview')), context);
  const flush = async () => { for (let i = 0; i < 12; i++) await Promise.resolve(); };
  return { context, events, task, wrapper, callbacks, frames, flush,
    resolvePdf: () => resolvePdf({ getPage: async () => page }), resolveRender: () => resolveRender(),
    frame: () => { const [id, callback] = frames.entries().next().value; frames.delete(id); callback(); } };
}

test('preview: principal espera render y oportunidad de pintura; limpia recursos', async () => {
  const h = previewPriorityHarness();
  let principal = 0;
  const pending = h.context.loadFirstPagePreview('fixture').then(shown => { principal++; return shown; });
  h.task.onProgress({ loaded: 100, total: 100 });
  h.resolvePdf(); await h.flush();
  assert.equal(principal, 0);
  h.resolveRender(); await h.flush();
  assert.equal(h.wrapper.dataset.ready, 'true');
  assert.equal(principal, 0);
  h.frame(); await h.flush();
  assert.equal(principal, 0);
  h.frame();
  assert.equal(await pending, true);
  assert.equal(principal, 1);
  assert.equal(h.wrapper.isConnected, true);
  assert.equal(h.callbacks.size, 0);
  assert.ok(h.events.includes('destroy'));
  assert.match(script, /if \(!preservePreview\) setDocumentLoadingState\("start"\)/);
  assert.match(script, /loadFirstPagePreview\(first_page_url\)\.then\(function \(shown\)/);
});

for (const phase of ['loading', 'render', 'paint']) {
  test(`preview: timeout durante ${phase} continúa y rechaza callbacks tardíos`, async () => {
    const h = previewPriorityHarness();
    const pending = h.context.loadFirstPagePreview('fixture');
    if (phase !== 'loading') { h.resolvePdf(); await h.flush(); }
    if (phase === 'paint') { h.resolveRender(); await h.flush(); }
    h.callbacks.values().next().value();
    assert.equal(await pending, false);
    assert.equal(h.wrapper.isConnected, false);
    const events = h.events.length;
    h.task.onProgress({ loaded: 999, total: 999 });
    h.resolvePdf(); h.resolveRender(); await h.flush();
    assert.equal(h.events.length, events);
    assert.equal(h.frames.size, 0);
    assert.equal(h.callbacks.size, 0);
  });
}

test('preview: sin referencia o error continúa sin agotar el plazo', async () => {
  const h = previewPriorityHarness();
  assert.equal(await h.context.loadFirstPagePreview(null), false);
  assert.equal(h.callbacks.size, 0);
  h.context.pdfjsLib.getDocument = () => { throw new Error('private'); };
  assert.equal(await h.context.loadFirstPagePreview('fixture'), false);
  assert.equal(h.callbacks.size, 0);
  assert.equal(h.wrapper.isConnected, false);
});

function printHarness() {
  const control = { disabled: false, setAttribute() {}, removeAttribute() {} };
  const status = { hidden: true, textContent: '' };
  const requests = [], prints = [];
  const context = vm.createContext({
    document: { getElementById: id => id === 'pdf-print-status' ? status : { closest: () => control } },
    isViewerInteractionReady: () => true,
    location: { origin: 'https://fixture.invalid' },
    $: { ajax: (_url, options) => requests.push(options) },
    printJS: options => prints.push(options),
    pdfViewer: {}
  });
  vm.runInContext(viewer.slice(viewer.indexOf('function service_printer_pdf_visor_plus'), viewer.indexOf('async function service_config_itex_stamp_interface')), context);
  context.pdfViewer._PrinterPdf = () => context.service_printer_pdf_visor_plus('fixture', [], 'Imprime');
  vm.runInContext(script.slice(script.indexOf('let viewerPrintPending'), script.indexOf('//------------------FIN', script.indexOf('let viewerPrintPending'))), context);
  return { context, control, status, requests, prints };
}

test('impresión: espera servidor y PrintJS, una sola solicitud y permite repetir al terminar', async () => {
  const h = printHarness();
  const pending = h.context.pdfTool_viwer_Printer();
  assert.equal(h.context.pdfTool_viwer_Printer(), pending);
  assert.equal(h.control.disabled, true);
  assert.match(h.status.textContent, /Preparando impresión/);
  await Promise.resolve();
  assert.equal(h.requests.length, 1);
  h.requests[0].success({ d: [{ error_sistema: 'YES', url_path: '/fixture.pdf' }] });
  assert.equal(h.prints.length, 1);
  assert.equal(h.control.disabled, true);
  assert.equal(h.status.hidden, false);
  h.prints[0].onLoadingEnd();
  await Promise.resolve();
  await Promise.resolve();
  assert.equal(h.control.disabled, true);
  assert.equal(h.status.hidden, false);
  assert.equal(h.context.pdfTool_viwer_Printer(), pending);
  assert.equal(h.requests.length, 1);
  h.prints[0].onPrintDialogClose();
  assert.equal(await pending, true);
  assert.equal(h.control.disabled, false);
  assert.equal(h.status.hidden, true);
  assert.equal(h.status.textContent, '');
  const retry = h.context.pdfTool_viwer_Printer();
  await Promise.resolve();
  h.requests[1].success({ d: [{ error_sistema: 'YES', url_path: '/fixture.pdf' }] });
  h.prints[1].onPrintDialogClose();
  assert.equal(await retry, true);
  // A late close from the previous job cannot unlock the next job.
  const next = h.context.pdfTool_viwer_Printer();
  await Promise.resolve();
  h.prints[0].onPrintDialogClose();
  await Promise.resolve();
  assert.equal(h.control.disabled, true);
  h.requests[2].success({ d: [{ error_sistema: 'YES', url_path: '/fixture.pdf' }] });
  h.prints[2].onIncompatibleBrowser();
  assert.equal(await next, true);
});

for (const failure of ['http', 'response', 'print', 'loading-before-error', 'capability']) {
  test('impresión: error saneado y desbloqueo ' + failure, async () => {
    const h = printHarness();
    if (failure === 'capability') h.context.pdfViewer._PrinterPdf = () => Promise.reject(new Error('PRIVATE'));
    const pending = h.context.pdfTool_viwer_Printer();
    await Promise.resolve();
    if (failure === 'http') h.requests[0].error({ responseText: 'PRIVATE' });
    else if (failure === 'response') h.requests[0].success({ d: [{ error_sistema: 'PRIVATE' }] });
    else if (failure !== 'capability') {
      h.requests[0].success({ d: [{ error_sistema: 'YES', url_path: '/fixture.pdf' }] });
      if (failure === 'loading-before-error') h.prints[0].onLoadingEnd();
      h.prints[0].onError(new Error('PRIVATE'));
      h.prints[0].onLoadingEnd();
    }
    assert.equal(await pending, false);
    assert.equal(h.control.disabled, false);
    assert.equal(h.status.hidden, false);
    assert.match(h.status.textContent, /No fue posible preparar la impresión/);
    assert.doesNotMatch(h.status.textContent, /PRIVATE/);
  });
}

test('impresión: permiso y carga inicial no invocan servicio', async () => {
  const h = printHarness();
  h.context.isViewerInteractionReady = () => false;
  assert.equal(h.context.pdfTool_viwer_Printer(), false);
  assert.equal(h.status.hidden, true);
  h.context.isViewerInteractionReady = () => true;
  h.context.pdfViewer._PrinterPdf = () => false;
  assert.equal(await h.context.pdfTool_viwer_Printer(), false);
  assert.match(h.status.textContent, /No tiene permiso/);
  assert.equal(h.requests.length, 0);
  assert.match(viewer, /return service_printer_pdf_visor_plus/);
  assert.match(index, /id="pdf-print-status" role="status" aria-live="polite"/);
});

test('motor de impresión conserva permiso y espera capacidad antes del servicio', async () => {
  const h = printHarness();
  const start = viewer.indexOf('async _PrinterPdf()');
  const end = viewer.indexOf('/**', start);
  h.context.window = { isViewerInteractionReady: () => true };
  let loadCount = 0, ready;
  h.context.window.loadViewerCapability = () => { loadCount++; return new Promise(resolve => { ready = resolve; }); };
  vm.runInContext('pdfViewer = new (class { ' + viewer.slice(start, end) + ' })();', h.context);
  Object.assign(h.context.pdfViewer, { anotate_printer: 0, PdfViwerAlert() {}, _PropertiTransac: () => [], getUrlPage: () => 'fixture' });
  assert.equal(await h.context.pdfViewer._PrinterPdf(), false);
  assert.equal(loadCount, 0);
  assert.equal(h.requests.length, 0);
  h.context.pdfViewer.anotate_printer = 1;
  const pending = h.context.pdfTool_viwer_Printer();
  await Promise.resolve();
  assert.equal(loadCount, 1);
  assert.equal(h.requests.length, 0);
  assert.equal(h.control.disabled, true);
  h.context.window.isViewerInteractionReady = () => false;
  ready();
  assert.equal(await pending, false);
  assert.equal(h.requests.length, 0);
  assert.match(h.status.textContent, /No fue posible/);
});

function downloadHarness() {
  const control = { disabled: false, setAttribute() {}, removeAttribute() {} };
  const status = { hidden: true, textContent: '' };
  const requests = [];
  const downloads = [];
  const timers = [];
  const context = vm.createContext({
    setTimeout(callback, delay) { const timer = { callback, delay }; timers.push(timer); return timer; },
    clearTimeout(timer) { if (timer) timer.cancelled = true; },
    document: { getElementById: id => id === 'pdf-download-status' ? status : { closest: () => control } },
    isViewerInteractionReady: () => true,
    $: { ajax: (_url, options) => requests.push(options) },
    dowload_file: (...args) => downloads.push(args),
    pdfViewer: { PdfViwerAlert() {} }
  });
  vm.runInContext(viewer.slice(viewer.indexOf('function service_download_pdf_visor_plus'), viewer.indexOf('function service_printer_pdf_visor_plus')), context);
  context.pdfViewer._DonwloadPage = () => context.service_download_pdf_visor_plus('fixture', [], 'Descarga');
  vm.runInContext(script.slice(script.indexOf('let viewerDownloadPending'), script.indexOf('function pdfTool_viwer_rotate_redo')), context);
  return { context, control, status, requests, downloads, timers };
}

test('confirmación se retira a los cinco segundos sin ocultar una preparación posterior', async () => {
  const { context, status, timers, control } = downloadHarness();
  context.pdfViewer._DonwloadPage = () => true;
  await context.donwload_pdfTool_viwer();
  assert.equal(status.hidden, false);
  assert.equal(timers[0].delay, 5000);
  timers[0].callback();
  assert.equal(status.hidden, true);
  assert.equal(status.textContent, '');
  await context.donwload_pdfTool_viwer();
  let finish;
  context.pdfViewer._DonwloadPage = () => new Promise(resolve => { finish = resolve; });
  const pending = context.donwload_pdfTool_viwer();
  await Promise.resolve();
  assert.equal(timers[1].cancelled, true);
  timers[1].callback(); // incluso un callback obsoleto ya encolado
  assert.equal(status.hidden, false);
  assert.match(status.textContent, /Preparando descarga/);
  assert.equal(control.disabled, true);
  finish(true);
  await pending;
  timers[1].callback();
  assert.equal(status.hidden, false);
  timers[2].callback();
  assert.equal(status.hidden, true);
  assert.equal(control.disabled, false);
});

test('descarga muestra espera, bloquea duplicados y libera solo al entregar al navegador', async () => {
  const { context, control, status, requests, downloads } = downloadHarness();
  const first = context.donwload_pdfTool_viwer();
  assert.equal(context.donwload_pdfTool_viwer(), first);
  assert.equal(control.disabled, true);
  assert.match(status.textContent, /Preparando descarga/);
  await Promise.resolve();
  assert.equal(requests.length, 1);
  requests[0].success({ d: [{ error_sistema: 'YES', error_log: 'YES', url_salida: 'fixture.pdf', name_file: 'fixture.pdf' }] });
  await first;
  assert.equal(downloads.length, 1);
  assert.equal(control.disabled, false);
  assert.match(status.textContent, /enviada al navegador/);
});

test('fallo de descarga libera botón, no revela respuesta ni reintenta automáticamente', async () => {
  const { context, control, status, requests, downloads } = downloadHarness();
  const first = context.donwload_pdfTool_viwer();
  await Promise.resolve();
  requests[0].error({ responseText: 'PRIVATE_FIXTURE' }, 'error');
  assert.equal(await first, false);
  assert.equal(control.disabled, false);
  assert.doesNotMatch(status.textContent, /PRIVATE_FIXTURE/);
  assert.equal(downloads.length, 0);
  assert.equal(requests.length, 1);
  const retry = context.donwload_pdfTool_viwer();
  await Promise.resolve();
  requests[1].success({ d: 'YES' });
  assert.equal(await retry, false);
  assert.equal(control.disabled, false);
});

test('descarga sin permiso no informa entrega ni llama al servicio', async () => {
  const { context, status, requests, control } = downloadHarness();
  context.pdfViewer._DonwloadPage = () => false;
  assert.equal(await context.donwload_pdfTool_viwer(), false);
  assert.equal(requests.length, 0);
  assert.equal(control.disabled, false);
  assert.match(status.textContent, /permiso/);
  assert.match(viewer, /_DonwloadPage\(\)[\s\S]*?anotate_save == 0[\s\S]*?return false;[\s\S]*?return service_download_pdf_visor_plus/);
});

function createElement() {
  const attributes = new Map();
  const classes = new Set();
  return {
    hidden: false,
    value: undefined,
    textContent: '',
    classList: { toggle(name, active) { active ? classes.add(name) : classes.delete(name); }, contains: name => classes.has(name) },
    setAttribute(name, value) { attributes.set(name, String(value)); },
    removeAttribute(name) { attributes.delete(name); if (name === 'value') this.value = undefined; },
    getAttribute(name) { return attributes.get(name); }
  };
}

function createHarness() {
  const ids = ['pdf-document-loading', 'pdf-document-loading-text', 'pdf-document-loading-progress', 'pdf-document-loading-percent', 'container-general', 'tool_pdf', 'pdf_page_find', 'num_page_total'];
  const elements = Object.fromEntries(ids.map(id => [id, createElement()]));
  const start = script.indexOf('let documentLoadingTerminal');
  const end = script.indexOf('async function loadFirstPagePreview', start);
  assert.ok(start >= 0 && end > start);
  const window = {};
  const context = vm.createContext({
    window,
    document: { getElementById: id => elements[id] || null },
    Number,
    Math,
    String
  });
  vm.runInContext(script.slice(start, end), context);
  return { window, elements, context };
}

test('callback real de render habilita barra tras metadatos y preview', async () => {
  const { window, elements, context } = createHarness();
  let callbacks;
  context.$ = () => ({});
  context.removeFirstPagePreview = () => {};
  context.reportDocumentLoadError = () => assert.fail('unexpected load error');
  context.PDFjsViewer = function (_, settings) {
    callbacks = settings;
    this.getPageCount = () => 287;
    this.setZoom = () => {};
    this.loadDocument = () => Promise.resolve();
  };
  const start = script.indexOf('function load_pdf_viwer_anotation');
  const end = script.indexOf('/** Create the thumbnails */', start);
  vm.runInContext(script.slice(start, end), context);
  await context.load_pdf_viwer_anotation('local-fixture', '', '', {});
  window.updateDocumentPageCounter(1, null);
  window.setDocumentLoadingState('ready');
  callbacks.onDocumentMetadata(287);
  callbacks.onDocumentReady.call(context.pdfViewer);
  assert.equal(elements.tool_pdf.disabled, true);
  callbacks.onPageRender();
  assert.equal(elements.tool_pdf.disabled, false);
  assert.equal(elements.num_page_total.value, '287');
  assert.equal(elements['pdf-document-loading'].hidden, true);
});

test('HTML solicita revisión común de coordinación, motor y estilos', () => {
  const revisions = ['script.js', 'pdf_viewer_anotate.js', 'styles.css'].map(name => {
    const attribute = name.endsWith('.css') ? 'href' : 'src';
    const match = index.match(new RegExp(attribute + '="\\./' + name.replaceAll('.', '\\.') + '\\?v=([^"&]+)"'));
    assert.ok(match, name + ' requiere revisión de caché');
    return match[1];
  });
  assert.equal(new Set(revisions).size, 1);
});

test('overlay inicial es accesible y está limitado al contenedor documental', () => {
  assert.match(index, /id="container-general" aria-busy="true"[\s\S]*id="pdf-document-loading"/);
  assert.match(index, /role="status" aria-live="polite" aria-atomic="true" aria-busy="true"/);
  assert.match(index, /<progress[^>]+max="100"[^>]+aria-label="Progreso de carga del documento"/);
  assert.match(styles, /#container-general\s*\{[\s\S]*?position:\s*relative/);
  assert.match(styles, /\.pdf-document-loading\s*\{[\s\S]*?position:\s*absolute[\s\S]*?pointer-events:\s*none/);
  assert.match(styles, /@media \(prefers-reduced-motion: reduce\)/);
});

test('progreso real se acota y el total desconocido permanece indeterminado', () => {
  const { window } = createHarness();
  assert.deepEqual({ ...window.normalizeDocumentLoadProgress({ loaded: 50, total: 200 }) }, { determinate: true, percent: 25 });
  assert.deepEqual({ ...window.normalizeDocumentLoadProgress({ loaded: 300, total: 200 }) }, { determinate: true, percent: 100 });
  assert.deepEqual({ ...window.normalizeDocumentLoadProgress({ loaded: 10, total: 0 }) }, { determinate: false, percent: 0 });
});

test('primer canvas oculta el overlay y un progreso tardío no lo reabre', () => {
  const { window, elements } = createHarness();
  window.setDocumentLoadingState('start');
  window.setDocumentLoadingState('loading', { loaded: 150, total: 200 });
  assert.equal(elements['pdf-document-loading-progress'].value, 75);
  assert.equal(elements['pdf-document-loading-percent'].textContent, '75%');
  window.setDocumentLoadingState('ready');
  assert.deepEqual({ ...window.getDocumentLoadingTelemetry() }, {
    firstCanvasLoaded: 150, firstCanvasTotal: 200, firstCanvasSource: 'full'
  });
  assert.equal(elements['pdf-document-loading'].hidden, true);
  assert.equal(elements['container-general'].getAttribute('aria-busy'), 'false');
  window.setDocumentLoadingState('loading', { loaded: 200, total: 200 });
  assert.equal(elements['pdf-document-loading'].hidden, true);
  window.setDocumentLoadingState('start');
  assert.equal(elements['pdf-document-loading'].hidden, false);
});

test('error queda visible y el arranque no fuerza descarga completa', () => {
  const { window, elements } = createHarness();
  window.setDocumentLoadingState('start');
  window.setDocumentLoadingState('error');
  assert.equal(elements['pdf-document-loading'].hidden, false);
  assert.equal(elements['pdf-document-loading'].classList.contains('pdf-document-loading--error'), true);
  assert.match(elements['pdf-document-loading-text'].textContent, /No fue posible abrir/);

  const loadStart = script.indexOf('function load_pdf_viwer_anotation');
  const loadEnd = script.indexOf('/** Create the thumbnails */', loadStart);
  const startup = script.slice(loadStart, loadEnd);
  assert.doesNotMatch(startup, /\.getData\s*\(/);
  assert.match(startup, /onDocumentProgress/);
  assert.match(startup, /onPageRender/);
  assert.match(viewer, /loadingTask\.onProgress/);
});

test('contador nunca vacío y preview no habilita herramientas ni inventa total', () => {
  const { window, elements } = createHarness();
  window.setDocumentLoadingState('start');
  assert.equal(elements.pdf_page_find.value, '…');
  assert.equal(elements.num_page_total.value, '…');
  assert.equal(elements.tool_pdf.disabled, true);
  window.updateDocumentPageCounter(1, null);
  window.setDocumentLoadingState('ready');
  assert.equal(elements.pdf_page_find.value, '1');
  assert.equal(elements.num_page_total.value, '…');
  assert.equal(elements.tool_pdf.disabled, true);
  window.updateDocumentPageCounter(null, 287);
  window.updateDocumentPageCounter(undefined, NaN);
  assert.equal(elements.pdf_page_find.value, '1');
  assert.equal(elements.num_page_total.value, '287');
  assert.equal(elements.pdf_page_find.getAttribute('max'), '287');
  window.markFullDocumentPageReady();
  assert.equal(elements.tool_pdf.disabled, false);
  window.updateDocumentPageCounter(10, 287);
  assert.equal(elements.pdf_page_find.value, '10');
  window.setDocumentLoadingState('start');
  assert.equal(elements.num_page_total.value, '…');
  assert.equal(elements.tool_pdf.disabled, true);
});

test('error principal después del preview no queda oculto ni permite render tardío', () => {
  const { window, elements } = createHarness();
  window.setDocumentLoadingState('start');
  window.updateDocumentPageCounter(1, 287);
  window.setDocumentLoadingState('ready');
  window.setDocumentLoadingState('error');
  window.markFullDocumentPageReady();
  window.setDocumentLoadingState('ready');
  assert.equal(elements['pdf-document-loading'].hidden, false);
  assert.equal(elements.tool_pdf.disabled, true);
  assert.equal(elements.num_page_total.value, '287');
});

test('HTML bloquea controles antes de JavaScript y los metadatos preceden al render', () => {
  assert.match(index, /<fieldset[^>]+id="tool_pdf"[^>]+disabled/);
  assert.match(index, /value="…" id="pdf_page_find"/);
  assert.match(index, /value="…" id="num_page_total"/);
  assert.ok(viewer.indexOf('this.settings.onDocumentMetadata.call') < viewer.indexOf('return this.forceViewerInitialization();', viewer.indexOf('this.pageCount = pdf.numPages')));
  assert.match(script, /onDocumentMetadata: function \(total\)\s*\{\s*updateDocumentPageCounter\(null, total\)/);
  assert.match(script, /const scale = 1\.25/);
  assert.match(viewer, /this.current = 1\.25/);
});

test('capacidad diferida no ejecuta la acción si la carga falla mientras espera', async () => {
  let ready = false;
  let loads = 0;
  let actions = 0;
  let resolveLoad;
  const control = { disabled: false };
  const start = script.indexOf('async function runOptionalCapability');
  const end = script.indexOf('window.viewerCapabilities', start);
  const context = {
    isViewerInteractionReady: () => ready,
    optionalCapabilityControl: () => control,
    loadViewerCapability: () => { loads++; return new Promise(resolve => { resolveLoad = resolve; }); },
    reportOptionalCapabilityError: () => assert.fail('unexpected capability error')
  };
  vm.createContext(context);
  vm.runInContext(script.slice(start, end), context);
  await context.runOptionalCapability(null, 'graph', 'grafo', () => actions++);
  assert.equal(loads, 0);
  ready = true;
  const pending = context.runOptionalCapability(null, 'graph', 'grafo', () => actions++);
  assert.equal(control.disabled, true);
  ready = false;
  resolveLoad();
  await pending;
  assert.equal(actions, 0);
  assert.equal(control.disabled, false); // The independent fieldset remains locked.
});

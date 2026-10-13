'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

const viewerPath = path.resolve(__dirname, '../../../pdfjs/pdf_anotate_view/pdf_viewer_anotate.js');
const source = fs.readFileSync(viewerPath, 'utf8');

function deferred() {
  let resolve;
  let reject;
  const promise = new Promise((ok, fail) => { resolve = ok; reject = fail; });
  return { promise, resolve, reject };
}

function loadViewer(windowOverrides = {}, jqueryOverride) {
  const window = { devicePixelRatio: 1, setTimeout, ...windowOverrides };
  const jquery = jqueryOverride || function () { throw new Error('JQUERY_DOUBLE_NOT_CONFIGURED'); };
  jquery.extend = (defaults, options) => ({ ...defaults, ...options });
  const context = { window, jQuery: jquery, console, setTimeout, clearTimeout, printMousePos() {}, context_menu() {} };
  vm.runInNewContext(source, context, { filename: viewerPath });
  return { PDFjsViewer: window.PDFjsViewer, window };
}

function scheduler(overrides = {}) {
  const { PDFjsViewer } = loadViewer();
  const viewer = Object.create(PDFjsViewer.prototype);
  Object.assign(viewer, {
    pdf: { numPages: 4, getPage: async pageNumber => ({ pageNumber, _pageIndex: pageNumber - 1 }) },
    pages: [undefined, {}, {}, {}, {}],
    pages_anotate: [],
    settings: { errorPage: () => null },
    _renderGeneration: 1,
    _renderJobs: new Map(),
    _renderTasks: new Map(),
    _pagesLoading: [],
    _loading: false,
    _loadingPromise: null,
    _visibles: [1, 2, 3, 4],
    _setPageContent() {},
    _renderPage: async () => {},
    ...overrides
  });
  return viewer;
}

test('la cola vacía resuelve y duplicados de página/generación renderizan una sola vez', async () => {
  const gate = deferred();
  const rendered = [];
  const viewer = scheduler({
    pdf: { numPages: 2, getPage: async pageNumber => ({ pageNumber, _pageIndex: pageNumber - 1 }) },
    _renderPage: async (_page, pageNumber) => { rendered.push(pageNumber); await gate.promise; }
  });
  await viewer._loadingTask();
  const work = viewer.loadPages(1, 1, 0, 3, 'invalid');
  assert.equal(viewer._renderJobs.get('1:1').state, 'rendering');
  assert.equal(viewer._pagesLoading.length, 0);
  gate.resolve();
  await work;
  assert.deepEqual(rendered, [1]);
  assert.equal(viewer._renderJobs.get('1:1').state, 'rendered');
  await viewer.loadPages(1);
  assert.deepEqual(rendered, [1]);
});

test('el trabajador es secuencial y continúa después de errores de getPage o render', async () => {
  const events = [];
  let errorPages = 0;
  const viewer = scheduler({
    pdf: {
      numPages: 4,
      getPage: async pageNumber => {
        events.push(`get:${pageNumber}`);
        if (pageNumber === 2) throw new Error('GET_FAILED');
        return { pageNumber, _pageIndex: pageNumber - 1 };
      }
    },
    _renderPage: async (_page, pageNumber) => {
      events.push(`render:${pageNumber}`);
      if (pageNumber === 3) throw new Error('RENDER_FAILED');
    },
    settings: { errorPage: () => { errorPages += 1; return {}; } }
  });
  await viewer.loadPages(1, 2, 3, 4);
  assert.deepEqual(events, ['get:1', 'render:1', 'get:2', 'get:3', 'render:3', 'get:4', 'render:4']);
  assert.equal(viewer._loading, false);
  assert.equal(errorPages, 2);
  assert.equal(viewer._renderJobs.get('4:1').state, 'rendered');
});

test('cambiar generación cancela RenderTask y una finalización tardía no hace commit', async () => {
  const render = deferred();
  let cancelled = 0;
  let commits = 0;
  const canvas = { setAttribute() {}, getContext: () => ({}), addEventListener() {} };
  const wrapper = { 0: canvas, get: () => canvas };
  const jquery = () => wrapper;
  const { PDFjsViewer } = loadViewer({}, jquery);
  const viewer = Object.create(PDFjsViewer.prototype);
  Object.assign(viewer, {
    pages: [undefined, { $div: { data() {} } }],
    settings: { contentClass: 'content-wrapper', onPageRender: () => { throw new Error('STALE_CALLBACK'); } },
    _zoom: { current: 1 }, _rotation: 0, _renderGeneration: 2,
    _renderJobs: new Map(), _renderTasks: new Map(), _pagesLoading: [], _visibles: [1],
    _setPageContent: () => { commits += 1; }
  });
  const page = {
    getViewport: () => ({ width: 100, height: 200 }),
    render: () => ({ promise: render.promise, cancel: () => { cancelled += 1; } })
  };
  const job = { id: '1:2', pageNumber: 1, generation: 2, state: 'rendering' };
  const pending = viewer._renderPage(page, 1, job);
  viewer._advanceRenderGeneration();
  render.resolve();
  await pending;
  assert.equal(cancelled, 1);
  assert.equal(commits, 0);
  assert.equal(job.state, 'cancelled');
});

test('IntersectionObserver limita el conjunto y conserva página activa y adyacentes', () => {
  const observedPages = [];
  observedPages[1] = { $div: { data: () => 1 } };
  observedPages[2] = { $div: { data: () => 2 } };
  observedPages[3] = { $div: { data: () => 3 } };
  observedPages[4] = { $div: { data: () => 4 } };
  const viewer = scheduler({
    settings: { visibleThreshold: 0.5, extraPagesToLoad: 1 },
    pages: observedPages,
    _viewportObserver: {}, _observerHasEntries: true,
    _intersections: new Map([[2, 0.8], [3, 0.2]]),
    _visibles: [],
    _setActivePage(page) { this._activePage = page; },
    loadPages(...pages) { this.loaded = pages; },
    _areaOfPageVisible() { throw new Error('OBSERVER_MUST_NOT_SCAN_ALL_PAGES'); }
  });
  viewer._visiblePages();
  assert.equal(viewer._activePage, 2);
  assert.deepEqual([...viewer._visibles].sort(), [1, 2, 3, 4]);
  assert.deepEqual([...viewer.loaded].sort(), [1, 2, 3, 4]);

  viewer._visiblePages(true);
  assert.deepEqual(viewer.loaded, [2, 3]);
});

test('salir del viewport cancela incluso una página renderizada y permite reencolarla', async () => {
  const viewer = scheduler({
    settings: { visibleThreshold: 0.5, extraPagesToLoad: 0 },
    _renderPage: async () => {}
  });
  await viewer.loadPages(1);
  const firstJob = viewer._renderJobs.get('1:1');
  assert.equal(firstJob.state, 'rendered');
  viewer._cancelPageRender(1);
  assert.equal(firstJob.state, 'cancelled');
  await viewer.loadPages(1);
  assert.notEqual(viewer._renderJobs.get('1:1'), firstJob);
  assert.equal(viewer._renderJobs.get('1:1').state, 'rendered');
});

test('el fallback geométrico funciona y las ráfagas se coalescen a un frame', () => {
  let callback;
  let reconciliations = 0;
  const { PDFjsViewer } = loadViewer({ requestAnimationFrame: next => { callback = next; return 7; } });
  const viewer = Object.create(PDFjsViewer.prototype);
  Object.assign(viewer, {
    _visibleFrame: null, _forceVisibleRedraw: false,
    _visiblePages(force) { reconciliations += 1; this.forced = force; }
  });
  viewer._scheduleVisiblePages();
  viewer._scheduleVisiblePages(true);
  viewer._scheduleVisiblePages();
  assert.equal(reconciliations, 0);
  callback();
  assert.equal(reconciliations, 1);
  assert.equal(viewer.forced, true);

  const fallbackPages = [];
  fallbackPages[1] = { $div: { data: () => 1 } };
  fallbackPages[2] = { $div: { data: () => 2 } };
  const fallback = scheduler({
    settings: { visibleThreshold: 0.5, extraPagesToLoad: 0 },
    pages: fallbackPages,
    _viewportObserver: null, _observerHasEntries: false, _visibles: [],
    _areaOfPageVisible: page => page.data() === 2 ? 20 : 10,
    _setActivePage(page) { this._activePage = page; },
    loadPages(...pages) { this.loaded = pages; }
  });
  fallback._visiblePages();
  assert.equal(fallback._activePage, 2);
  assert.deepEqual(fallback.loaded, [1, 2]);
});

test('zoom y rotación preservan scroll proporcional y coalescen reconciliación', async () => {
  const { PDFjsViewer } = loadViewer();
  const container = { scrollTop: 100, scrollLeft: 40, scrollHeight: 1000, scrollWidth: 400 };
  let generations = 0;
  let scheduled = 0;
  const viewer = Object.create(PDFjsViewer.prototype);
  Object.assign(viewer, {
    $container: { get: () => container },
    _zoom: { current: 1, zoomPages() { this.current = 2; } },
    settings: { onZoomChange() {} },
    _advanceRenderGeneration() { generations += 1; },
    _scheduleVisiblePages(force) { if (force) scheduled += 1; },
    _rotation: 0
  });
  viewer.setZoom('in');
  assert.equal(container.scrollTop, 200);
  assert.equal(container.scrollLeft, 80);
  assert.equal(generations, 1);
  assert.equal(scheduled, 1);

  viewer.forceViewerInitialization = async () => {
    container.scrollHeight = 2000;
    container.scrollWidth = 800;
  };
  await viewer.rotate(90);
  assert.equal(viewer._rotation, 90);
  assert.equal(container.scrollTop, 400);
  assert.equal(container.scrollLeft, 160);
});

test('conserva contratos públicos, DOM y coordinación de anotaciones', () => {
  for (const signature of ['setZoom(zoom)', 'loadPages(...pages)', 'scrollToPage(i)', 'rotate(deg, accumulate = false)']) {
    assert.match(source, new RegExp(signature.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')));
  }
  assert.match(source, /page-' \+ \(i\) \+ '-canvas/);
  assert.match(source, /this\.pages_anotate/);
  assert.match(source, /this\._setActivePage\(i_page\)/);
});

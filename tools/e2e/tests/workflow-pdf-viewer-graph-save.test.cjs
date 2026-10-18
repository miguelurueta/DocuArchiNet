'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

const root = path.resolve(__dirname, '../../..');
const viewer = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/pdf_viewer_anotate.js'), 'utf8');

function transactionSource() {
  const start = viewer.indexOf('let STRU_ANOTATE_IMAGE');
  const middle = viewer.indexOf('let Top_event', start);
  const serviceStart = viewer.indexOf('function service_anotate_pdf_image', middle);
  const serviceEnd = viewer.indexOf('function service_download_pdf_visor_plus', serviceStart);
  assert.ok(start > 0 && middle > start && serviceStart > middle && serviceEnd > serviceStart);
  return `${viewer.slice(start, middle)}\n${viewer.slice(serviceStart, serviceEnd)}\n` +
    'this.doc103 = { event_drag_anotate, save_anotate_firma, service_anotate_pdf_image, ' +
    'normalizeAnotateResponse, classifyAnotateTransportFailure, cancelPendingAnnotations, pending: DOC103_PENDING_ANNOTATIONS };';
}

function createElement(id, values = {}) {
  return {
    id, attrs: {}, style: {}, children: [], parentElement: null,
    clientWidth: 0, clientHeight: 0, removed: false, ...values,
    setAttribute(name, value) { this.attrs[name] = String(value); },
    removeAttribute(name) { delete this.attrs[name]; }
  };
}

function createHarness(scenario = { type: 'success', data: { d: [{ error_sistema: 'YES', error_log: 'YES', url_salida: '/page' }] } }) {
  const events = [];
  const elements = new Map();
  const controls = [createElement('close'), createElement('save')];
  const bar = createElement('overlay_bar', { children: controls });
  const container = createElement('container', {
    removeChild(child) { child.removed = true; child.parentElement = null; events.push('overlay-removed'); }
  });
  const overlay = createElement('overlay', { style: { top: '25px', left: '30px' }, parentElement: container });
  const canvas = createElement('page-2-canvas', { clientWidth: 800, clientHeight: 1000 });
  const image = createElement('overlayimg', { clientWidth: 90, clientHeight: 60 });
  canvas.getBoundingClientRect = () => ({ left: 100, top: -200, width: canvas.clientWidth, height: canvas.clientHeight });
  image.getBoundingClientRect = () => ({ left: 100 + Number.parseFloat(overlay.style.left) + 1,
    top: -200 + Number.parseFloat(overlay.style.top) + 1, width: image.clientWidth, height: image.clientHeight });
  for (const item of [bar, overlay, canvas, image]) elements.set(item.id, item);

  let imageMode = 'success';
  class FakeImage {
    set src(value) {
      this.value = value;
      queueMicrotask(() => {
        events.push(imageMode === 'success' ? 'image-loaded' : 'image-failed');
        (imageMode === 'success' ? this.onload : this.onerror)?.();
      });
    }
  }

  let ajaxCalls = 0;
  let abortCalls = 0;
  let pagehide;
  let ajaxOptions;
  const jquery = element => ({ attr: name => element.attrs[name] });
  jquery.ajax = (_url, options) => {
    ajaxCalls += 1;
    ajaxOptions = options;
    events.push('post');
    const request = {
      abort() {
        abortCalls += 1;
        queueMicrotask(() => options.error({ status: 0 }, 'abort'));
      }
    };
    if (scenario.type === 'pending') return request;
    queueMicrotask(() => {
      if (scenario.type === 'success') options.success(scenario.data);
      else options.error({ status: scenario.status || 0 }, scenario.textStatus || 'error');
    });
    return request;
  };

  const navigation = { page: 2, zoom: 1.25, scrollLeft: 41, scrollTop: 93, url: '/document' };
  const alerts = [];
  const pdfViewer = {
    anotate_id_imagen: 'opaque', anotate_cabinete_imagen: 'opaque', anotate_radicado: 'opaque',
    anotate_id_workflow: 'opaque', anotate_desc_transacion: 'opaque', _rotation: 0,
    itex_config_stamp: [{ error_sistema: 'YES', itex_value_stamp_transparent: 70, itex_aplicate_transparent_stamp: 1 }],
    getActivePageNumber: () => navigation.page,
    getUrlPage: () => navigation.url,
    getZoom: () => navigation.zoom,
    PdfViwerAlert: message => alerts.push(message),
    async _updatePage(page, url) { events.push('refresh'); assert.equal(page, 2); assert.equal(url, '/page'); }
  };
  const context = vm.createContext({
    $: jquery, jQuery: jquery, document: { getElementById: id => elements.get(id) || null },
    window: { addEventListener(name, listener) { if (name === 'pagehide') pagehide = listener; } },
    Image: FakeImage, pdfViewer, confirm: () => true, Promise, Map, Object, Array, String, Number,
    setTimeout, clearTimeout, queueMicrotask
  });
  context.$ = jquery;
  vm.runInContext(transactionSource(), context);
  const button = { attrs: { parent_id: 'overlay', event_drag: 'save', ur_image_drag: '/image', ur_image_drag64: '' } };
  return {
    api: context.doc103, button, overlay, controls, alerts, events, navigation, pdfViewer,
    addOverlay(id, left = 45) {
      const extraControls = [createElement(`${id}-close`), createElement(`${id}-save`)];
      const extraBar = createElement(`${id}_bar`, { children: extraControls });
      const extraOverlay = createElement(id, { style: { top: '35px', left: `${left}px` }, parentElement: container });
      const extraImage = createElement(`${id}img`, { clientWidth: 80, clientHeight: 55 });
      extraImage.getBoundingClientRect = () => ({ left: 100 + left + 1, top: -164, width: 80, height: 55 });
      for (const item of [extraBar, extraOverlay, extraImage]) elements.set(item.id, item);
      return {
        overlay: extraOverlay, controls: extraControls,
        button: { attrs: { parent_id: id, event_drag: 'save', ur_image_drag: '/image', ur_image_drag64: '' } }
      };
    },
    setImageMode(value) { imageMode = value; },
    pagehide: () => pagehide(),
    get ajaxCalls() { return ajaxCalls; }, get abortCalls() { return abortCalls; }, get ajaxOptions() { return ajaxOptions; }
  };
}

test('normaliza únicamente colecciones ASMX autoritativas', () => {
  const h = createHarness();
  const cases = [
    [{ d: [{ error_sistema: 'YES', error_log: 'YES', url_salida: '/page' }] }, 'committed'],
    [{ d: [{ error_sistema: 'YES', error_log: 'NO', url_salida: '/page' }] }, 'committed_with_warning'],
    [{ d: [{ error_sistema: 'NO', error_log: 'YES', url_salida: '/page' }] }, 'rejected'],
    [{ d: [] }, 'invalid_response'], [{ d: 'YES' }, 'invalid_response'], [{}, 'invalid_response'],
    [{ d: [{ error_sistema: 'YES', error_log: 'YES' }] }, 'invalid_response']
  ];
  for (const [input, state] of cases) assert.equal(h.api.normalizeAnotateResponse(input).state, state);
});

test('espera imagen, POST y refresco antes de retirar el overlay', async () => {
  const h = createHarness();
  const before = { ...h.navigation };
  const pending = h.api.event_drag_anotate({}, h.button);
  assert.equal(h.overlay.removed, false);
  assert.equal(h.ajaxCalls, 0);
  assert.equal(h.controls.every(control => control.attrs['aria-disabled'] === 'true'), true);
  assert.equal(h.controls.every(control => control.attrs['aria-busy'] === 'true' && control.attrs.tabindex === '-1'), true);
  const result = await pending;
  assert.equal(result.state, 'committed');
  assert.deepEqual(h.events, ['image-loaded', 'post', 'refresh', 'overlay-removed']);
  assert.equal(h.ajaxCalls, 1);
  assert.equal(h.ajaxOptions.timeout, 30000);
  assert.equal(h.overlay.removed, true);
  assert.equal(h.controls.every(control => control.attrs['aria-disabled'] === 'false'), true);
  assert.equal(h.controls.every(control => control.attrs['aria-busy'] === undefined && control.attrs.tabindex === undefined), true);
  assert.deepEqual(h.navigation, before);
});

test('doble clic comparte single-flight y produce exactamente un POST', async () => {
  const h = createHarness();
  const first = h.api.event_drag_anotate({}, h.button);
  const second = h.api.event_drag_anotate({}, h.button);
  const results = await Promise.all([first, second]);
  assert.equal(results[0].state, 'committed');
  assert.equal(results[1].state, 'committed');
  assert.equal(h.ajaxCalls, 1);
  assert.equal(h.events.filter(value => value === 'overlay-removed').length, 1);
});

test('overlays distintos no se bloquean entre sí', async () => {
  const h = createHarness();
  const second = h.addOverlay('overlay-2');
  const results = await Promise.all([
    h.api.event_drag_anotate({}, h.button),
    h.api.event_drag_anotate({}, second.button)
  ]);
  assert.deepEqual(results.map(item => item.state), ['committed', 'committed']);
  assert.equal(h.ajaxCalls, 2);
  assert.equal(h.overlay.removed, true);
  assert.equal(second.overlay.removed, true);
});

test('entrada incompleta no confirma ni inicia transporte', async () => {
  const h = createHarness();
  h.overlay.style.left = '';
  const result = await h.api.event_drag_anotate({}, h.button);
  assert.equal(result.code, 'INVALID_INPUT');
  assert.equal(h.ajaxCalls, 0);
  assert.equal(h.overlay.removed, false);
  assert.equal(h.alerts.length, 1);
});

test('DOC-105 envía rectángulo real de imagen relativo al canvas sin borde ni barra', async () => {
  const h = createHarness();
  await h.api.event_drag_anotate({}, h.button);
  const encoded = h.ajaxOptions.data;
  const payload = JSON.parse(encoded.slice("{'anotate':'".length, -2))[0];
  assert.equal(payload.anotate_x, 31);
  assert.equal(payload.anotate_y, 974);
  assert.equal(payload.anotate_width, 90);
  assert.equal(payload.anotate_heigth, 60);
  assert.equal(payload.pdf_file_width, 800);
  assert.equal(payload.pdf_file_heigth, 1000);
});

test('fallos conservan overlay, saneán aviso y no reintentan', async () => {
  const matrix = [
    [{ type: 'success', data: { d: [{ error_sistema: 'NO', error_log: 'YES', url_salida: '/page' }] } }, 'rejected'],
    [{ type: 'success', data: { d: 'invalid' } }, 'invalid_response'],
    [{ type: 'error', status: 401 }, 'rejected'], [{ type: 'error', status: 403 }, 'rejected'],
    [{ type: 'error', status: 404 }, 'rejected'], [{ type: 'error', status: 500 }, 'uncertain'],
    [{ type: 'error', status: 0, textStatus: 'parsererror' }, 'invalid_response'],
    [{ type: 'error', status: 0, textStatus: 'timeout' }, 'uncertain']
  ];
  for (const [scenario, expected] of matrix) {
    const h = createHarness(scenario);
    const result = await h.api.event_drag_anotate({}, h.button);
    assert.equal(result.state, expected);
    assert.equal(h.overlay.removed, false);
    assert.equal(h.ajaxCalls, 1);
    assert.equal(h.alerts.length, 1);
    assert.doesNotMatch(h.alerts[0], /response|stack|\/page|opaque|500|404|401/i);
  }
});

test('commit con advertencia refresca y retira sin duplicar', async () => {
  const h = createHarness({ type: 'success', data: { d: [{ error_sistema: 'YES', error_log: 'fallo privado', url_salida: '/page' }] } });
  const result = await h.api.event_drag_anotate({}, h.button);
  assert.equal(result.state, 'committed_with_warning');
  assert.equal(h.overlay.removed, true);
  assert.equal(h.ajaxCalls, 1);
  assert.equal(h.alerts.length, 1);
  assert.doesNotMatch(h.alerts[0], /fallo privado|\/page/);
});

test('pagehide aborta una vez, libera single-flight y conserva overlay', async () => {
  const h = createHarness({ type: 'pending' });
  const pending = h.api.event_drag_anotate({}, h.button);
  await new Promise(resolve => setImmediate(resolve));
  assert.equal(h.ajaxCalls, 1);
  h.pagehide();
  h.pagehide();
  const result = await pending;
  assert.equal(result.code, 'ABORTED');
  assert.equal(h.abortCalls, 1);
  assert.equal(h.overlay.removed, false);
  assert.equal(h.api.pending.size, 0);
});

test('cerrar no retira un overlay pendiente aunque cambie la página activa', async () => {
  const h = createHarness({ type: 'pending' });
  const pending = h.api.event_drag_anotate({}, h.button);
  await new Promise(resolve => setImmediate(resolve));
  h.navigation.page = 3;
  const close = h.api.event_drag_anotate({}, { attrs: { parent_id: 'overlay', event_drag: 'close' } });
  assert.equal(h.overlay.removed, false);
  h.pagehide();
  const [saveResult, closeResult] = await Promise.all([pending, close]);
  assert.equal(saveResult.code, 'ABORTED');
  assert.equal(closeResult.code, 'ABORTED');
  assert.equal(h.overlay.removed, false);
});

test('imagen inválida y refresco fallido conservan el overlay', async () => {
  const imageFailure = createHarness();
  imageFailure.setImageMode('error');
  assert.equal((await imageFailure.api.event_drag_anotate({}, imageFailure.button)).code, 'IMAGE_LOAD_FAILED');
  assert.equal(imageFailure.ajaxCalls, 0);
  assert.equal(imageFailure.overlay.removed, false);

  const refreshFailure = createHarness();
  refreshFailure.pdfViewer._updatePage = async () => { throw new Error('detalle privado'); };
  assert.equal((await refreshFailure.api.event_drag_anotate({}, refreshFailure.button)).code, 'REFRESH_FAILED');
  assert.equal(refreshFailure.ajaxCalls, 1);
  assert.equal(refreshFailure.overlay.removed, false);
});

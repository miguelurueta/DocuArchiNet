const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const root = path.resolve(__dirname, '../../..');
const source = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/script.js'), 'utf8');
const start = source.indexOf('const viewerCapabilityPromises');
const endMarker = 'window.loadViewerCapability = loadViewerCapability;';
const end = source.indexOf(endMarker, start) + endMarker.length;
assert.ok(start >= 0 && end > start, 'bloque del cargador encontrado');
const loaderSource = source.slice(start, end);

function createHarness() {
  const elements = [];
  const appended = [];
  const failures = new Set();
  const window = { jQuery: { fn: {} } };
  const document = {
    baseURI: 'https://example.test/pdfjs/pdf_anotate_view/index.html',
    querySelectorAll(selector) {
      if (selector.startsWith('script')) return elements.filter(item => item.tagName === 'SCRIPT');
      if (selector.startsWith('link')) return elements.filter(item => item.tagName === 'LINK');
      return [];
    },
    createElement(tag) {
      const attributes = {};
      return {
        tagName: tag.toUpperCase(),
        setAttribute(name, value) { attributes[name] = value; },
        getAttribute(name) { return name === 'src' ? this.src : name === 'href' ? this.href : attributes[name]; },
        remove() { const index = elements.indexOf(this); if (index >= 0) elements.splice(index, 1); }
      };
    },
    head: {
      appendChild(element) {
        elements.push(element);
        const resource = new URL(element.src || element.href, document.baseURI).href;
        appended.push(resource);
        setImmediate(() => {
          if (failures.has(resource)) return element.onerror();
          if (/jquery-ui\.min\.js/.test(resource)) {
            window.jQuery.fn.draggable = function () {};
            window.jQuery.fn.resizable = function () {};
          }
          if (/jquery\.contextMenu\.js/.test(resource)) window.jQuery.fn.contextMenu = function () {};
          if (/wacom\.js/.test(resource)) window.captureFromSTU = function () {};
          if (/print\.min\.js/.test(resource)) window.printJS = function () {};
          if (/fabric\.min\.js/.test(resource)) window.fabric = { Canvas: function () {} };
          if (/prettify\.min\.js/.test(resource)) window.PR = { prettyPrint() {} };
          element.onload();
        });
        return element;
      }
    }
  };
  const context = { window, document, URL, Promise, Array, Object, Error, console, setImmediate };
  vm.runInNewContext(loaderSource, context);
  return { window, document, appended, failures };
}

test('solicitudes concurrentes comparten promesa y recursos', async () => {
  const harness = createHarness();
  const first = harness.window.loadViewerCapability('print');
  const second = harness.window.loadViewerCapability('print');
  assert.equal(first, second);
  await Promise.all([first, second]);
  assert.equal(harness.appended.filter(item => /print\.min\.js/.test(item)).length, 1);
  assert.equal(harness.appended.filter(item => /print\.min\.css/.test(item)).length, 1);
  await harness.window.loadViewerCapability('print');
  assert.equal(harness.appended.filter(item => /print\.min\.js/.test(item)).length, 1);
});

test('Wacom respeta orden y no duplica SDKs', async () => {
  const harness = createHarness();
  await harness.window.loadViewerCapability('wacom');
  const scripts = harness.appended.filter(item => /\/wacom\/.*\.js/.test(item));
  assert.equal(scripts.filter(item => item.endsWith('/signature_sdk.js')).length, 1);
  assert.equal(scripts.filter(item => item.endsWith('/wgssStuSdk.js')).length, 1);
  assert.ok(scripts.findIndex(item => item.endsWith('/q.js')) < scripts.findIndex(item => item.endsWith('/wacom.js')));
  assert.equal(typeof harness.window.captureFromSTU, 'function');
});

test('un fallo requerido limpia la promesa y permite reintento', async () => {
  const harness = createHarness();
  const printUrl = 'https://example.test/printer_js/print.min.js';
  harness.failures.add(printUrl);
  await assert.rejects(harness.window.loadViewerCapability('print'), /VIEWER_SCRIPT_LOAD_FAILED/);
  harness.failures.delete(printUrl);
  await harness.window.loadViewerCapability('print');
  assert.equal(harness.appended.filter(item => item === printUrl).length, 2);
  assert.equal(typeof harness.window.printJS, 'function');
});

test('fallo táctil opcional no invalida los plugins de grafo', async () => {
  const harness = createHarness();
  const touchUrl = 'https://cdnjs.cloudflare.com/ajax/libs/jqueryui-touch-punch/0.2.3/jquery.ui.touch-punch.min.js';
  harness.failures.add(touchUrl);
  await harness.window.loadViewerCapability('graph');
  assert.equal(typeof harness.window.jQuery.fn.draggable, 'function');
  assert.equal(typeof harness.window.jQuery.fn.resizable, 'function');
  assert.equal(typeof harness.window.jQuery.fn.contextMenu, 'function');
});

test('Fabric y Prettify quedan disponibles solo al solicitarlos', async () => {
  const harness = createHarness();
  assert.equal(harness.window.fabric, undefined);
  assert.equal(harness.window.PR, undefined);
  await harness.window.loadViewerCapability('fabric');
  await harness.window.loadViewerCapability('prettify');
  assert.equal(typeof harness.window.fabric.Canvas, 'function');
  assert.equal(typeof harness.window.PR.prettyPrint, 'function');
});

test('cada capacidad pasa de no solicitada a fallida, reintentable y lista', async () => {
  const cases = [
    ['graph', 'https://example.test/js/jquery-ui-1.12.1.custom/jquery-ui.min.js', harness => harness.window.jQuery.fn.contextMenu],
    ['wacom', 'https://example.test/pdfjs/wacom/q.js', harness => harness.window.captureFromSTU],
    ['print', 'https://example.test/printer_js/print.min.js', harness => harness.window.printJS],
    ['fabric', 'https://cdnjs.cloudflare.com/ajax/libs/fabric.js/4.3.0/fabric.min.js', harness => harness.window.fabric?.Canvas],
    ['prettify', 'https://cdnjs.cloudflare.com/ajax/libs/prettify/r298/prettify.min.js', harness => harness.window.PR?.prettyPrint]
  ];
  for (const [name, failingUrl, ready] of cases) {
    const harness = createHarness();
    assert.equal(ready(harness), undefined, `${name}: no solicitada`);
    harness.failures.add(failingUrl);
    const loading = harness.window.loadViewerCapability(name);
    assert.equal(loading, harness.window.loadViewerCapability(name), `${name}: comparte carga`);
    await assert.rejects(loading, /VIEWER_(?:SCRIPT|CAPABILITY)_/);
    harness.failures.delete(failingUrl);
    await harness.window.loadViewerCapability(name);
    assert.equal(typeof ready(harness), 'function', `${name}: lista tras reintento`);
  }
  await assert.rejects(createHarness().window.loadViewerCapability('unknown'), /VIEWER_CAPABILITY_UNKNOWN/);
});

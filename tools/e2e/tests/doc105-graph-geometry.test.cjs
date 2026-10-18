'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const root = path.resolve(__dirname, '../../..');
const run = (exe, args) => execFileSync(exe, args, { cwd: root, encoding: 'utf8' });
const compiler = name => run('powershell.exe', ['-NoProfile', '-Command', `(Get-Command ${name} -ErrorAction Stop).Source`]).trim();

test('DOC-105 inserción de grafo desplaza solo el visor, conserva modal y cierre', async () => {
  const { chromium } = require('playwright');
  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    const page = await browser.newPage();
    await page.route('**/*', route => route.abort());
    await page.setContent(`<div id="modal" style="height:650px;overflow:hidden">
      <header id="header" style="height:40px"><button>Cerrar</button></header>
      <iframe style="width:700px;height:600px;border:0"></iframe><div style="height:200px"></div></div>`);
    const frame = page.frames()[1];
    await frame.setContent(`<style>body{margin:0} #container-general{height:560px}
      #viewport{height:500px;width:650px;overflow:auto} #sheet{position:relative;width:1400px;height:1800px}</style>
      <div style="height:40px">Herramientas</div><div id="container-general">
      <div id="viewport"><div id="sheet"></div></div></div>`);
    await frame.addStyleTag({ content: fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/styles.css'), 'utf8') });
    const viewer = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/pdf_viewer_anotate.js'), 'utf8');
    const methods = viewer.slice(viewer.indexOf('        _Add_interface_anotate('), viewer.indexOf('        isInViewport('));
    const visibility = viewer.slice(viewer.indexOf('        isInViewport('), viewer.indexOf('        _PropertiTransac()'));
    await frame.addScriptTag({ content: `window.Subject = class { ${methods} ${visibility} };` });
    await frame.evaluate(() => {
      // Plugins de interacción no intervienen en la inserción/desplazamiento probado.
      window.$ = () => ({ css() {}, draggable() {}, resizable() {} });
      window.subject = new Subject();
      Object.assign(subject, { anotate_add_firma: 1, settings: { Inconter_Stamp: 0 },
        ur_firma_user: '', ur_data64: '', getActivePage: () => [document.getElementById('sheet')],
        getActivePageNumber: () => 1, $container: { get: () => document.getElementById('viewport') } });
    });
    const before = await page.locator('#header').boundingBox();
    for (const [top, left] of [[900, 1000], [50, 30], [900, 1000]]) {
      const result = await frame.evaluate(([top, left]) => {
        subject._Add_interface_anotate(0, top, left);
        const viewport = document.getElementById('viewport');
        const stamp = document.getElementById(`div-drav-image-${subject.settings.Inconter_Stamp - 1}`);
        const bounds = viewport.getBoundingClientRect();
        const rect = stamp.getBoundingClientRect();
        return { visible: rect.top >= bounds.top && rect.bottom <= bounds.top + viewport.clientHeight &&
          rect.left >= bounds.left && rect.right <= bounds.left + viewport.clientWidth,
          documentScroll: document.scrollingElement.scrollTop };
      }, [top, left]);
      assert.equal(result.visible, true);
      assert.equal(result.documentScroll, 0);
      assert.deepEqual(await page.locator('#header').boundingBox(), before);
      assert.equal(await page.locator('#modal').evaluate(el => el.scrollTop), 0);
      const stationary = await frame.evaluate(() => {
        const viewport = document.getElementById('viewport');
        const before = [viewport.scrollTop, viewport.scrollLeft];
        subject._revealAnnotation(document.getElementById(`div-drav-image-${subject.settings.Inconter_Stamp - 1}`));
        subject._revealAnnotation(document.createElement('div'));
        return { before, after: [viewport.scrollTop, viewport.scrollLeft] };
      });
      assert.deepEqual(stationary.after, stationary.before);
    }
    await frame.evaluate(() => {
      subject.ur_data64 = 'data:image/svg+xml,' + encodeURIComponent('<svg xmlns="http://www.w3.org/2000/svg" width="100" height="60"><path d="M0 30L100 40" stroke="black"/></svg>');
    });
    for (const scale of [0.5, 1, 1.25, 2]) {
      for (const offset of [0, 700]) {
        const beforeScroll = await frame.evaluate(({ scale, offset }) => {
          const viewport = document.getElementById('viewport');
          const sheet = document.getElementById('sheet');
          sheet.style.transformOrigin = 'top left';
          sheet.style.transform = `scale(${scale})`;
          viewport.scrollTop = offset;
          viewport.scrollLeft = offset / 2;
          const before = [viewport.scrollTop, viewport.scrollLeft];
          subject._Add_interface_anotate(0, 0, 0);
          return before;
        }, { scale, offset });
        await frame.waitForFunction(() => {
          const stamp = document.getElementById(`div-drav-image-${subject.settings.Inconter_Stamp - 1}`);
          return stamp.querySelector('img').naturalWidth > 0;
        });
        const measured = await frame.evaluate(() => {
          const viewport = document.getElementById('viewport');
          const sheet = document.getElementById('sheet');
          const stamp = document.getElementById(`div-drav-image-${subject.settings.Inconter_Stamp - 1}`);
          const v = viewport.getBoundingClientRect(), p = sheet.getBoundingClientRect(), s = stamp.getBoundingClientRect();
          const expectedX = (Math.max(v.left, p.left) + Math.min(v.left + viewport.clientWidth, p.right)) / 2;
          const expectedY = (Math.max(v.top, p.top) + Math.min(v.top + viewport.clientHeight, p.bottom)) / 2;
          return { scroll: [viewport.scrollTop, viewport.scrollLeft], dx: s.left + s.width / 2 - expectedX,
            dy: s.top + s.height / 2 - expectedY, opacity: getComputedStyle(stamp).opacity,
            imageOpacity: getComputedStyle(stamp.querySelector('img')).opacity,
            saveText: stamp.querySelector('[event_drag="save"]').textContent.trim(),
            button: stamp.querySelector('[event_drag="save"]').tagName };
        });
        assert.deepEqual(measured.scroll, beforeScroll);
        assert.ok(Math.abs(measured.dx) < 2 && Math.abs(measured.dy) < 2);
        assert.equal(measured.opacity, '1');
        assert.equal(measured.imageOpacity, '0.65');
        assert.equal(measured.saveText, 'Guardar');
        assert.equal(measured.button, 'BUTTON');
      }
    }
    const delayed = await frame.evaluate(() => {
      const NativeImage = window.Image;
      let pending;
      window.Image = class { constructor() { pending = this; this.naturalWidth = 100; this.naturalHeight = 60; } };
      try {
        subject._Add_interface_anotate(0, 0, 0);
        const stamp = document.getElementById(`div-drav-image-${subject.settings.Inconter_Stamp - 1}`);
        stamp.dispatchEvent(new PointerEvent('pointerdown'));
        stamp.style.left = '123px';
        stamp.style.top = '456px';
        pending.onload();
        const unchanged = stamp.style.left === '123px' && stamp.style.top === '456px';
        stamp.remove();
        pending.onload();
        return unchanged;
      } finally { window.Image = NativeImage; }
    });
    assert.equal(delayed, true);
  } finally { await browser.close(); }
});

test('DOC-105 DOM Chrome: mide imagen sin barra con scroll, escala CSS y DPR 2', async () => {
  const { chromium } = require('playwright');
  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    const page = await browser.newPage({ deviceScaleFactor: 2 });
    await page.route('**/*', route => route.abort());
    await page.setContent(`<div style="height:300px"></div>
      <div id="sheet" style="position:relative;width:600px;height:800px;transform-origin:top left">
        <canvas id="page-1-canvas" style="display:block;width:600px;height:800px"></canvas>
        <div id="overlay" style="position:absolute;left:30px;top:40px;border:1px solid black">
          <img id="overlayimg" style="display:block;width:100px;height:50px">
          <div id="overlay_bar" style="height:30px"><button id="save" parent_id="overlay"
            parent_page="1" event_drag="save" ur_image_drag="fixture">Guardar</button></div>
        </div>
      </div><div style="height:2000px"></div>`);
    await page.evaluate(() => {
      window.$ = element => ({ attr: name => element.getAttribute(name) });
      window.confirm = () => true;
      window.pdfViewer = { getActivePageNumber: () => 1, getUrlPage: () => 'fixture', getZoom: () => 1,
        _rotation: 0, PdfViwerAlert() {} };
    });
    const viewer = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/pdf_viewer_anotate.js'), 'utf8');
    await page.addScriptTag({ content: viewer.slice(viewer.indexOf('let STRU_ANOTATE_IMAGE'), viewer.indexOf('let Top_event')) });
    await page.evaluate(() => {
      // Captura local del contrato: no se invoca servicio ni se guarda un PDF.
      save_anotate_firma = (...args) => { window.geometry = args.slice(0, 10); return Promise.reject({ code: 'REJECTED' }); };
      window.scrollTo(0, 200);
    });
    for (const zoom of [0.5, 1, 1.25, 2]) {
      const values = await page.evaluate(async zoom => {
        document.getElementById('sheet').style.transform = `scale(${zoom})`;
        await event_drag_anotate({}, document.getElementById('save'));
        return window.geometry;
      }, zoom);
      for (const [index, expected] of [[0, 31], [1, 759], [2, 100], [3, 50], [8, 600], [9, 800]]) {
        assert.ok(Math.abs(values[index] - expected * zoom) < 0.01);
      }
    }
  } finally { await browser.close(); }
});

test('DOC-105 geometría VB compilada: zoom, cuatro giros, origen y validación', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc105-geometry-'));
  try {
    const assembly = path.join(directory, 'Geometry.dll');
    const executable = path.join(directory, 'GeometryTests.exe');
    run(compiler('vbc.exe'), ['/nologo', '/target:library', `/out:${assembly}`,
      path.join(root, 'itextshare/GeometriaGrafoPdf.vb')]);
    run(compiler('csc.exe'), ['/nologo', `/out:${executable}`, `/reference:${assembly}`,
      path.join(__dirname, 'fixtures/Doc105GraphGeometryTests.cs')]);
    assert.match(run(executable, []), /48 cases passed/);
  } finally {
    // Solo el directorio temporal único creado por esta prueba.
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

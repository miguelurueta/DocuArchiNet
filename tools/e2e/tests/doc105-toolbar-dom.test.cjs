'use strict';

// Local DOM integration only: no authentication, PDF, network or business writes.
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { chromium } = require('playwright');
const root = path.resolve(__dirname, '../../..');
const script = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/script.js'), 'utf8');
const html = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/index.html'), 'utf8');

test('zoom y rotación: borde, icono y teclado ejecutan una acción; carga y error ninguna', async () => {
  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    const page = await browser.newPage();
    await page.route('**/*', route => route.abort());
    await page.setContent(html.slice(html.indexOf('<body>'), html.indexOf('</body>'))
      .replace(/<script\b[^>]*>[\s\S]*?<\/script>/gi, '')
      .replace(/<link\b[^>]*>/gi, ''));
    // Icon font is external; give the empty local <i> a hit area without fetching it.
    await page.addStyleTag({ content: '#tool_pdf i { display:inline-block; width:14px; height:14px; }' });
    await page.evaluate(() => {
      window.zoomCalls = [];
      window.pdfViewer = { setZoom: value => window.zoomCalls.push(value), rotate: (value, accumulate) => {
        if (!accumulate) throw new Error('rotation must accumulate');
        window.zoomCalls.push(value); return Promise.resolve();
      } };
      window.reportDocumentLoadError = () => { throw new Error('unexpected rotation error'); };
      for (const name of ['add_pdfTool_viwer_stamp', 'donwload_pdfTool_viwer', 'pdfTool_viwer_rotate_redo',
        'pdfTool_viwer_rotate_undo', 'pdfTool_viwer_captureFromSTU', 'pdfTool_viwer_Printer',
        'pdfTool_parameter_grafo', 'pdfBtn_update_insert', 'activateViewerContextMenu']) window[name] = () => {};
    });
    const states = script.slice(script.indexOf('let documentLoadingTerminal'), script.indexOf('async function loadFirstPagePreview'));
    const config = script.slice(script.indexOf('const configToolbarPdfView'), script.indexOf('const pdfTool_parameter_grafo'));
    const zoom = script.slice(script.indexOf('function scale_pdfTool_viwer_plus'), script.indexOf('async function add_pdfTool_viwer_stamp'));
    const rotation = script.slice(script.indexOf('function pdfTool_viwer_rotate_redo'), script.indexOf('async function pdfTool_viwer_captureFromSTU'));
    const download = script.slice(script.indexOf('let viewerDownloadPending'), script.indexOf('function pdfTool_viwer_rotate_redo'));
    const print = script.slice(script.indexOf('let viewerPrintPending'), script.indexOf('//------------------FIN', script.indexOf('let viewerPrintPending')));
    await page.addScriptTag({ content: states + config + zoom + rotation + download + print + '\nconfigToolbarPdfView();' });
    await page.evaluate(() => {
      setDocumentLoadingState('start');
      document.getElementById('pdf_btn_zon_plus').click();
      document.getElementById('pdf_btn_zon_minus').closest('button').click();
      document.getElementById('pdf_btn_tbl_desk_redo').click();
      document.getElementById('pdf_btn_tbl_desk_undo').closest('button').click();
    });
    assert.deepEqual(await page.evaluate(() => zoomCalls), []);
    await page.evaluate(() => markFullDocumentPageReady());
    for (const [id, direction] of [['pdf_btn_zon_plus', 'in'], ['pdf_btn_zon_minus', 'out'],
      ['pdf_btn_tbl_desk_redo', 90], ['pdf_btn_tbl_desk_undo', -90]]) {
      const button = page.locator('button').filter({ has: page.locator('#' + id) });
      const count = await page.evaluate(() => zoomCalls.length);
      await button.click({ position: { x: 2, y: 2 } });
      await page.locator('#' + id).click();
      await button.focus();
      await page.keyboard.press('Enter');
      await page.keyboard.press('Space');
      assert.deepEqual(await page.evaluate(n => zoomCalls.slice(n), count), Array(4).fill(direction));
    }
    await page.evaluate(() => {
      setDocumentLoadingState('error');
      document.getElementById('pdf_btn_tbl_desk_redo').click();
      document.getElementById('pdf_btn_tbl_desk_undo').closest('button').click();
      document.getElementById('pdf_btn_zon_plus').click();
      document.getElementById('pdf_btn_zon_minus').closest('button').click();
    });
    assert.equal(await page.evaluate(() => zoomCalls.length), 16);
    await page.evaluate(() => {
      setDocumentLoadingState('start');
      markFullDocumentPageReady();
      window.downloadCalls = 0;
      pdfViewer._DonwloadPage = () => {
        downloadCalls++;
        return new Promise(resolve => { window.finishDownload = resolve; });
      };
    });
    const downloadButton = page.getByRole('button', { name: 'Descargar documento' });
    await downloadButton.click({ position: { x: 2, y: 2 } });
    assert.equal(await downloadButton.isDisabled(), true);
    assert.match(await page.locator('#pdf-download-status').textContent(), /Preparando descarga/);
    await page.evaluate(() => {
      document.getElementById('pdf_btn_down_load').click();
      donwload_pdfTool_viwer();
    });
    assert.equal(await page.evaluate(() => downloadCalls), 1);
    await page.evaluate(async () => {
      const pending = viewerDownloadPending;
      finishDownload(true);
      await pending;
    });
    assert.equal(await downloadButton.isDisabled(), false);
    assert.match(await page.locator('#pdf-download-status').textContent(), /enviada al navegador/);
    await page.locator('#pdf-download-status').waitFor({ state: 'hidden', timeout: 7000 });
    assert.equal(await page.locator('#pdf-download-status').textContent(), '');
    assert.equal(await downloadButton.isDisabled(), false);
    await page.evaluate(() => {
      window.printCalls = 0;
      pdfViewer._PrinterPdf = () => { printCalls++; return new Promise(resolve => { window.finishPrint = resolve; }); };
    });
    const printButton = page.getByRole('button', { name: 'Imprimir', exact: true });
    for (const action of ['border', 'icon', 'Enter', 'Space']) {
      if (action === 'border') await printButton.click({ position: { x: 2, y: 2 } });
      else if (action === 'icon') await page.locator('#pdf_btn_printer_page').click();
      else { await printButton.focus(); await page.keyboard.press(action); }
      assert.equal(await printButton.isDisabled(), true);
      assert.match(await page.locator('#pdf-print-status').textContent(), /Preparando impresión/);
      await page.evaluate(() => { document.getElementById('pdf_btn_printer_page').click(); pdfTool_viwer_Printer(); });
      await page.evaluate(async () => { const pending = viewerPrintPending; finishPrint(true); await pending; });
      assert.equal(await printButton.isDisabled(), false);
      assert.equal(await page.locator('#pdf-print-status').isVisible(), false);
    }
    assert.equal(await page.evaluate(() => printCalls), 4);
  } finally { await browser.close(); }
});

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '../../..');
const indexSource = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/index.html'), 'utf8');
const scriptSource = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/script.js'), 'utf8');
const viewerSource = fs.readFileSync(path.join(root, 'pdfjs/pdf_anotate_view/pdf_viewer_anotate.js'), 'utf8');

const scriptSources = [...indexSource.matchAll(/<script\b[^>]*\bsrc=["']([^"']+)["'][^>]*>/gi)].map(match => match[1]);

test('el camino crítico no contiene capacidades opcionales', () => {
  const forbidden = [
    '/wacom/', 'print.min.js', 'fabric.min.js', 'prettify.min.js',
    'jquery.contextMenu.js', 'jquery-ui-touch-punch', 'jspdf.umd.min.js'
  ];
  for (const fragment of forbidden) {
    assert.equal(scriptSources.some(source => source.includes(fragment)), false, fragment);
  }
  const loadStart = scriptSource.indexOf('function load_pdf_viwer_anotation');
  const loadEnd = scriptSource.indexOf('/** Create the thumbnails */', loadStart);
  assert.ok(loadStart >= 0 && loadEnd > loadStart);
  assert.doesNotMatch(scriptSource.slice(loadStart, loadEnd), /pdfViewer\.config_stamp\(\)/);
});

test('PDF.js usa un único script de frame y configura el worker', () => {
  const pdfScripts = scriptSources.filter(source => /pdf(?:\.worker(?:\.entry)?|\.sandbox)?\.min\.js/i.test(source));
  assert.deepEqual(pdfScripts, ['https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.4.120/pdf.min.js']);
  assert.match(scriptSource, /GlobalWorkerOptions\.workerSrc\s*=\s*PDFJS_WORKER_SRC/);
  assert.match(scriptSource, /pdf\.worker\.min\.js/);
  assert.doesNotMatch(indexSource, /<script[^>]+pdf\.(?:worker|sandbox|worker\.entry)\.min\.js/i);
});

test('cada familia conserva un propietario declarado', () => {
  assert.equal((scriptSource.match(/jquery-ui-1\.12\.1\.custom\/jquery-ui\.min\.js/g) || []).length, 1);
  assert.doesNotMatch(indexSource, /jqueryui\/1\.13\.2|jquery-ui-1\.8\.22/i);
  assert.doesNotMatch(indexSource, /popper(?:\.min)?\.js/i);
  assert.equal(scriptSources.filter(source => /bootstrap\.bundle\.min\.js/i.test(source)).length, 1);
  assert.equal(scriptSources.filter(source => /Awesome\/js\/all\.js/i.test(source)).length, 1);
  assert.equal(scriptSources.some(source => /Awesome\/js\/(?:brands|solid)\.js/i.test(source)), false);
  assert.doesNotMatch(indexSource, /rawgit|jspdf|prettify\.min\.js/i);
  assert.equal((scriptSource.match(/prettify\/r298\/prettify\.min\.js/g) || []).length, 1);
  assert.equal((scriptSource.match(/\.\.\/wacom\/signature_sdk\.js/g) || []).length, 1);
  assert.equal((scriptSource.match(/\.\.\/wacom\/wgssStuSdk\.js/g) || []).length, 1);
});

test('el núcleo respeta el orden jQuery, PDF.js, visor y coordinación', () => {
  const positions = [
    indexSource.indexOf('jquery/3.5.1/jquery.min.js'),
    indexSource.indexOf('pdf.js/3.4.120/pdf.min.js'),
    indexSource.indexOf('./pdf_viewer_anotate.js'),
    indexSource.indexOf('./script.js')
  ];
  assert.equal(positions.every(position => position >= 0), true);
  assert.deepEqual([...positions].sort((a, b) => a - b), positions);
});

test('la matriz conserva IDs, handlers y símbolos de cada capacidad', () => {
  const contracts = [
    ['pdf-container_', /load_pdf_viwer_anotation/],
    ['dropdownMenuButton1', /pdfTool_parameter_grafo/],
    ['pdf_btn_tbl_sing', /add_pdfTool_viwer_stamp/],
    ['pdf_btn_tbl_digital', /pdfTool_viwer_captureFromSTU/],
    ['pdf_btn_printer_page', /pdfTool_viwer_Printer/],
    ['pdf_btn_down_load', /donwload_pdfTool_viwer/]
  ];
  for (const [id, handler] of contracts) {
    assert.match(indexSource, new RegExp(`id=["']${id}["']`));
    assert.match(scriptSource, handler);
  }
  assert.match(viewerSource, /window\.loadViewerCapability\("wacom"\)/);
  assert.match(viewerSource, /window\.loadViewerCapability\("print"\)/);
  assert.match(scriptSource, /addEventListener\("contextmenu", activateViewerContextMenu, true\)/);
  assert.match(scriptSource, /initializeViewerContextMenus\(\)/);
  assert.match(scriptSource, /anotate_add_firma == 0/);
  assert.match(scriptSource, /anotate_printer == 0/);
  for (const iconClass of ['fal', 'fad', 'far']) assert.match(indexSource + viewerSource, new RegExp(`\\b${iconClass}\\b`));
});

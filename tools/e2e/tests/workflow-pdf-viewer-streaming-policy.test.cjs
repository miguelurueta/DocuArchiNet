'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

const root = path.resolve(__dirname, '../../..');
const read = relative => fs.readFileSync(path.join(root, relative), 'utf8');
const script = read('pdfjs/pdf_anotate_view/script.js');
const viewer = read('pdfjs/pdf_anotate_view/pdf_viewer_anotate.js');
const handler = read('workflow/Handler_image_wf.ashx.vb');
const strategy = read('Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb');
const emitters = [
  'Docuarchi/ClassVisualisaDocumento.vb',
  'workflow/ClassWorflowVisor.vb',
  'Gestion/ClassGaProducionDocumental.vb'
];

test('DOC-102 construye una URL autorizada y falla cerrada sin parámetros', () => {
  const match = script.match(/function buildAuthorizedPdfUrl\(urlParams\) \{[\s\S]*?\n\}/);
  assert.ok(match, 'Debe existir el constructor aislado de URL.');
  const context = { URLSearchParams, encodeURIComponent };
  vm.runInNewContext(`${match[0]}; this.build = buildAuthorizedPdfUrl;`, context);
  assert.equal(context.build(new URLSearchParams('ash=%2Fhandler.ashx&doc_ref=a%2Bb%2Fc')), '/handler.ashx?doc_ref=a%2Bb%2Fc');
  assert.equal(context.build(new URLSearchParams('ash=%2Fhandler.ashx')), null);
  assert.equal(context.build(new URLSearchParams('doc_ref=token')), null);
});

test('DOC-102 normaliza el modo y usa streaming como fallback compatible', () => {
  const match = script.match(/function normalizePdfLoadingMode\(value\) \{[\s\S]*?\n\}/);
  assert.ok(match, 'Debe existir el normalizador aislado del modo.');
  const context = {};
  vm.runInNewContext(`${match[0]}; this.normalize = normalizePdfLoadingMode;`, context);
  assert.equal(context.normalize('range'), 'range');
  assert.equal(context.normalize('stream'), 'stream');
  assert.equal(context.normalize('unexpected'), 'stream');
  assert.equal(context.normalize(null), 'stream');
});

test('DOC-102 entrega URL a PDF.js sin precargar el documento completo', () => {
  const start = script.indexOf('$(window).on("load"');
  const end = script.indexOf('//------------------ZONA PDFVIWER-2', start);
  const startup = script.slice(start, end);
  assert.match(startup, /buildAuthorizedPdfUrl\(urlParams\)/);
  assert.match(startup, /load_pdf_viwer_anotation\(url_src, url_src_fromat/);
  assert.match(startup, /normalizePdfLoadingMode\(urlParams\.get\("pdf_load_mode"\)\)/);
  assert.doesNotMatch(startup, /\bfetch\s*\(|new\s+FileReader|\.blob\s*\(|new\s+Uint8Array/);
  assert.match(viewer, /this\.url_document\s*=\s*url_src_fromat/);
  assert.match(viewer, /this\.url_block\s*=\s*document/);
  assert.match(viewer, /url:\s*document/);
  assert.match(viewer, /disableStream:\s*true/);
  assert.match(viewer, /disableAutoFetch:\s*true/);
  assert.match(viewer, /rangeChunkSize:\s*262144/);
  assert.match(viewer, /disableRange:\s*true/);
  assert.match(viewer, /disableStream:\s*false/);
  assert.match(viewer, /_createLoadingSource\(document,\s*"stream"\)/,
    'El refresco posterior al pegado de grafo debe usar el transporte compatible.');
  assert.match(viewer, /pdfjsLib\.getDocument\(this\._createLoadingSource\(document\)\)/);
});

test('DOC-102 preserva el pegado de grafo y refresca solo la página afectada', () => {
  const start = viewer.indexOf('async  _updatePage(i,document)');
  const end = viewer.indexOf('//Agrega las paginas con anotacion', start);
  const updatePage = viewer.slice(start, end);
  assert.match(updatePage, /_createLoadingSource\(document,\s*"stream"\)/);
  assert.match(updatePage, /this\._renderPage\(page, i\)/);
  assert.match(updatePage, /this\._add_anotate_page\(page, i\)/);
  assert.doesNotMatch(updatePage, /loadDocument|forceViewerInitialization/);
  assert.match(viewer, /"pdf_file_src":\s*pdfViewer\.getUrlPage\(\)/);
  assert.match(viewer, /service_anotate_pdf_image/);
});

test('DOC-102 selecciona rangos solo para PDF grandes y linearizados', () => {
  assert.match(strategy, /UmbralRangosBytes\s+As Long\s*=\s*1048576L/);
  assert.match(strategy, /EsCabeceraLinearizada/);
  assert.match(strategy, /FileShare\.ReadWrite Or FileShare\.Delete/);
  assert.match(strategy, /Return ModoStreamingCompleto/);
  assert.doesNotMatch(strategy, /File\.(?:Write|Move|Copy|Delete)|\.Write\s*\(/);
});

test('DOC-102 usa doc_ref en emisores enriquecidos y preserva adaptadores simples', () => {
  for (const relative of emitters) {
    const source = read(relative);
    const enriched = [...source.matchAll(/pdf_anotate_view\/index\.html\?[^\r\n]*/g)].map(value => value[0]);
    assert.ok(enriched.length > 0, `${relative} debe tener un emisor enriquecido.`);
    for (const expression of enriched) {
      assert.match(expression, /doc_ref=/, relative);
      assert.match(expression, /pdf_load_mode=/, relative);
      assert.doesNotMatch(expression, /index\.html\?rut_image=/, relative);
    }
  }
  for (const relative of [
    'Docuarchi/WebFormVisorDescarga.aspx.vb',
    'Docuarchi/WebFormDaVisorExterno.aspx.vb',
    'Gestion/Class_ra_ver_version_documento.vb',
    'Gestion/WebFormGaVisorDocumentoExterno.aspx.vb',
    'workflow/WebFormVisor_Otros_Formatos.aspx.vb'
  ]) assert.match(read(relative), /Handler_image_wf\.ashx\?rut_image=/, relative);
});

test('DOC-102 handler separa streaming autorizado y adaptación legacy', () => {
  assert.match(handler, /Implements IHttpHandler, IReadOnlySessionState/);
  assert.match(handler, /QueryString\("doc_ref"\)/);
  assert.match(handler, /QueryString\("rut_image"\)/);
  assert.match(handler, /Accept-Ranges/);
  assert.match(handler, /Content-Range/);
  assert.match(handler, /If-None-Match/);
  assert.match(handler, /If-Range/);
  assert.match(handler, /SetCacheability\(HttpCacheability\.Private\)[\s\S]*AppendCacheExtension\("no-cache"\)[\s\S]*SetRevalidation/);
  assert.match(handler, /AppendHeader\("ETag", etag\)/);
  assert.match(handler, /EscritorStreamingDocumento\.Copiar/);
  assert.match(handler, /Response\.HeadersWritten/);
  assert.doesNotMatch(handler, /ReadFile\s*\(|BinaryWrite\s*\(|Response\.End\s*\(|ex\.Message/);

  const legacyStart = handler.indexOf('Private Shared Sub ProcesarLegacy');
  const legacyEnd = handler.indexOf('Private Shared Sub ConfigurarRepresentacionAutorizada', legacyStart);
  const legacy = handler.slice(legacyStart, legacyEnd);
  assert.doesNotMatch(legacy, /Accept-Ranges|Content-Range|SetETag|StatusCode\s*=\s*206/);
  assert.match(legacy, /SetNoStore/);
});

test('DOC-102 no escribe sesión ni registra efectos de negocio desde el handler', () => {
  assert.doesNotMatch(handler, /Session(?:\.Item)?\s*\([^\)]*\)\s*=/i);
  assert.doesNotMatch(handler, /Registrar|Repository|ExecuteNonQuery|\b(?:INSERT|UPDATE|DELETE)\s+(?:INTO|FROM|[A-Za-z_])/i);
});

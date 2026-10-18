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
const styles = read('pdfjs/pdf_anotate_view/styles.css');
const handler = read('workflow/Handler_image_wf.ashx.vb');
const strategy = read('Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb');
const preparer = read('Infrastructure/Documentos/Streaming/PreparadorVisualizacionPdf.vb');
const linearizationPoc = read('tools/e2e/scripts/run-doc102-linearization-poc.cjs');
const emitters = [
  'Docuarchi/ClassVisualisaDocumento.vb',
  'workflow/ClassWorflowVisor.vb',
  'Gestion/ClassGaProducionDocumental.vb'
];

test('DOC-105 Producción reserva PDF.js a PDF y conserva descarga externa para Word', () => {
  const source = read('Gestion/ClassGaProducionDocumental.vb');
  const start = source.indexOf('Function Activa_visualizacion_documento_producion(');
  assert.ok(start >= 0);
  const method = source.slice(start, source.indexOf('End Function', start));
  assert.match(method, /If String\.Equals\(extension, "\.PDF", StringComparison\.OrdinalIgnoreCase\) AndAlso\s+HttpContext\.Current\.Session\("UTIL_VISOR_EXPRESS_PRODUCION"\) = 1 AndAlso HttpContext\.Current\.Session\("VALIDA_VISOR_EXPRES"\) = 1 Then/);
  assert.match(method, /Else\s+Iframe_visor_externo_da\.Attributes\("SRC"\) = "\.\.\/Docuarchi\/WebFormDaVisorExterno\.aspx"/);
  const fallback = read('Docuarchi/WebFormDaVisorExterno.aspx.vb');
  assert.match(fallback, /If UCase\(file_inf\.Extension\) = "\.PDF" Then[\s\S]*?Else[\s\S]*?WebFormDaVisorExternoDescarga\.aspx/);
});

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
  assert.match(viewer, /disableAutoFetch:\s*true/);
  assert.match(viewer, /rangeChunkSize:\s*1048576/);
  assert.match(viewer, /disableRange:\s*true/);
  assert.match(viewer, /disableStream:\s*false/);
  const rangeSourceStart = viewer.indexOf('return {', viewer.indexOf('if (selectedMode !== "range")') + 1);
  const rangeSource = viewer.slice(rangeSourceStart, viewer.indexOf('/** acive windows config', rangeSourceStart));
  assert.match(rangeSource, /disableRange:\s*false/);
  assert.match(rangeSource, /disableStream:\s*true/);
  assert.match(rangeSource, /disableAutoFetch:\s*true/);
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

test('DOC-105 conserva streaming para PDF no linealizado tras fallar la compuerta remota', () => {
  assert.match(strategy, /UmbralRangosBytes\s+As Long\s*=\s*1048576L/);
  assert.match(strategy, /Return If\(EsCabeceraLinearizada\(cabecera\), ModoRangos, ModoStreamingCompleto\)/);
  assert.match(strategy, /inicioPdf >= 0 AndAlso inicioPdf <= 16/);
  assert.match(strategy, /EsCabeceraLinearizada/);
  assert.match(strategy, /FileShare\.ReadWrite Or FileShare\.Delete/);
  assert.match(strategy, /Return ModoStreamingCompleto/);
  assert.doesNotMatch(strategy, /File\.(?:Write|Move|Copy|Delete)|\.Write\s*\(/);
});

test('DOC-105 prepara un derivado linealizado sin bloquear ni sustituir el original', () => {
  assert.match(preparer, /ThreadPool\.QueueUserWorkItem/);
  assert.match(preparer, /EnPreparacion\.TryAdd/);
  assert.match(preparer, /-dFastWebView=true/);
  assert.match(preparer, /File\.Move\(temporal, rutaDerivado\)/);
  assert.match(preparer, /-dFirstPage=1/);
  assert.match(preparer, /RutaVistaPreviaPrimeraPagina/);
  assert.match(preparer, /EsDerivadoEficaz/);
  assert.match(preparer, /FirstSectionMaximumBytes/);
  assert.match(strategy, /TryObtenerSeccionInicialLinearizada/);
  assert.match(preparer, /EsDerivadoValido\(rutaDerivado\)/);
  assert.match(preparer, /Not Path\.IsPathRooted\(configurado\)/);
  assert.match(preparer, /\.RutaVisualizacion = rutaOriginal/);
  assert.match(preparer, /\.ModoCarga = EstrategiaCargaPdf\.Seleccionar\(rutaOriginal\)/);
  assert.doesNotMatch(preparer, /File\.(?:WriteAllBytes|Copy)\([^\r\n]*rutaOriginal/);

  for (const relative of emitters) {
    const source = read(relative);
    assert.match(source, /PreparadorVisualizacionPdf\.Preparar\(matri_doc\(1\)\)/, relative);
    assert.match(source, /Emitir\(preparacionPdf\.RutaVisualizacion/, relative);
    assert.match(source, /first_page_doc_ref=/, relative);
    assert.match(source, /urimage_format=" & matri_doc\(1\)/, relative);
  }
});

test('DOC-105 prepara la vista previa en la primera apertura y difiere solo el derivado completo', () => {
  assert.match(preparer, /GenerarVistaPrevia\(capturaRuta, capturaLongitud, capturaEscritura, rutaVistaPrevia\)/);
  assert.match(preparer, /RutaVistaPreviaPrimeraPagina = rutaVistaPrevia/);
  assert.match(preparer, /ThreadPool\.QueueUserWorkItem\([\s\S]*GenerarDerivado\(capturaRuta, capturaLongitud, capturaEscritura, rutaDerivado\)/);
  assert.match(preparer, /ModoRangos AndAlso EsDerivadoEficaz\(rutaOriginal\)/);
});

test('DOC-105 muestra una primera página temporal y la retira al renderizar el documento completo', () => {
  assert.match(script, /function loadFirstPagePreview\(url\)/);
  assert.match(script, /first_page_doc_ref/);
  assert.match(script, /pdfjsLib\.getDocument\(\{[\s\S]*disableStream:\s*false/);
  assert.match(script, /documentLoadingTelemetry\.firstCanvasSource = "preview"/);
  assert.match(script, /wrapper\.hidden = true/);
  assert.match(script, /wrapper\.dataset\.ready = "true"/);
  assert.match(script, /fullDocumentFirstPageReady \|\| documentLoadFailed \|\| !wrapper\.isConnected/);
  assert.match(script, /function removeFirstPagePreview\(\)/);
  assert.match(script, /onPageRender:[\s\S]*markFullDocumentPageReady\(\);[\s\S]*removeFirstPagePreview\(\)/);
  assert.match(styles, /\.pdf-first-page-preview\s*\{[\s\S]*position:\s*absolute;[\s\S]*inset:\s*0;/);
  assert.match(styles, /#pdf-container_\s*\{[\s\S]*position:\s*relative;/);
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

test('DOC-102 prueba linearización solo con derivados temporales y servidor local', () => {
  assert.match(linearizationPoc, /mkdtempSync\(path\.join\(os\.tmpdir\(\), 'doc102-linearization-poc-'\)\)/);
  assert.match(linearizationPoc, /'-dFastWebView=true'/);
  assert.match(linearizationPoc, /server\.listen\(0, '127\.0\.0\.1'/);
  assert.match(linearizationPoc, /disableStream: true/);
  assert.match(linearizationPoc, /disableAutoFetch: true/);
  assert.match(linearizationPoc, /fs\.rmSync\(tempDirectory, \{ recursive: true, force: true \}\)/);
  assert.match(linearizationPoc, /fixtureHashBefore === sha256\(fixture\)/);
  assert.doesNotMatch(linearizationPoc, /authenticated-workflow-session|cookie|authorization/i);
});

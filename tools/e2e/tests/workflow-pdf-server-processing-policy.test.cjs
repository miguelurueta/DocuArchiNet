'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '../../..');
const read = relative => fs.readFileSync(path.join(root, relative), 'utf8');
const itext = read('itextshare/Class_ItexShare.vb');
const service = read('webservice/WebService_itext.asmx.vb');
const transparent = read('workflow/Class_transparent.vb');
const viewer = read('pdfjs/pdf_anotate_view/pdf_viewer_anotate.js');
const runner = read('tools/e2e/scripts/run-doc104-server-processing-interactive.cjs');
const profile = read('tools/e2e/profiles/doc104-server-processing.profile.example.json');
const { sanitizeEvidence } = require('../scripts/run-doc104-server-processing-interactive.cjs');
const { classifySaveResponse } = require('../scripts/run-doc103-graph-save-interactive.cjs');

function between(source, start, end) {
  const first = source.indexOf(start);
  const last = source.indexOf(end, first + start.length);
  assert.ok(first >= 0 && last > first, `${start}..${end}`);
  return source.slice(first, last);
}

test('DOC-104 aísla temporales y reemplaza el PDF sin copia destructiva', () => {
  const save = between(
    itext,
    'Function add_anotate_image_pdf(ByVal itext_anotate As Class_itext_anotate,\r\n                                   ByRef top As String,\r\n                                   ByRef url_pagina As String,',
    'Function Solicita_ruta_imagen'
  );
  assert.match(save, /Guid\.NewGuid\(\)\.ToString\("N"\)/);
  assert.match(save, /FileShare\.None/);
  assert.match(save, /File\.Replace\(operation\.StagedPdfPath, originalPath, operation\.BackupPdfPath, True\)/);
  assert.match(save, /ValidatePdf\(operation\.StagedPdfPath/);
  assert.doesNotMatch(save, /File\.Copy\([^\n]+,\s*True\)/);
  assert.doesNotMatch(save, /Create_file_pagina\s*\(/i);
  assert.doesNotMatch(save, /pat_user\s*&\s*"(?:_1\.png|\.png|\.pdf)"/i);
  assert.doesNotMatch(save, /Directory\.GetFiles/);
});

test('DOC-104 libera recursos y limita página antes del stamper', () => {
  const pageCheck = itext.indexOf('pageNumber > originalPageCount');
  const stamper = itext.indexOf('stamper = New PdfStamper', pageCheck);
  assert.ok(pageCheck > 0 && stamper > pageCheck);
  assert.match(itext, /Finally[\s\S]*?stamper\.Close\(\)[\s\S]*?outputStream\.Dispose\(\)[\s\S]*?reader\.Close\(\)/);
  assert.match(itext, /Using sourceBitmap As New System\.Drawing\.Bitmap/);
  assert.match(itext, /If outputBitmap IsNot Nothing Then outputBitmap\.Dispose\(\)/);
});

test('DOC-104 usa LockBits y no simula asincronía ni acceso GetPixel SetPixel', () => {
  assert.match(transparent, /LockBits\(/);
  assert.match(transparent, /UnlockBits\(/);
  assert.match(transparent, /Marshal\.Copy/);
  assert.match(transparent, /pixels\(offset \+ 3\) = 0/);
  assert.doesNotMatch(transparent, /TransparentAsync|GetPixel|SetPixel|MakeTransparent/);
});

test('DOC-104 valida contrato antes de certificado y no expone detalle interno', () => {
  const method = between(service, 'Public Function service_anotate_pdf_image', 'Public Function service_download_pdf_visor_plus');
  const normalize = method.indexOf('NormalizeGraphAnnotation');
  const certificate = method.indexOf('Solicita_registro_certificado_archivo_imagen_gabinete');
  const mutate = method.indexOf('add_anotate_image_pdf');
  assert.ok(normalize > 0 && certificate > normalize && mutate > certificate);
  assert.match(method, /deserialize_parameter\.Length <> 1/);
  assert.match(method, /rotation <> 0 AndAlso rotation <> 90 AndAlso rotation <> 180 AndAlso rotation <> 270/);
  assert.doesNotMatch(method, /item\.error_sistema\s*=\s*ex\.Message/);
  const audit = between(method, 'Dim datos_add_grafo As String', 'Dim cabinet As String');
  assert.doesNotMatch(audit, /url grafo|URL DOC|anotate_file_src|pdf_file_src|base64|ex\.Message/i);
  assert.match(audit, /MODO CARGA/);
});

test('DOC-104 refresca la página afectada desde el documento completo', () => {
  const update = between(viewer, 'async  _updatePage(i,document)', '//Agrega las paginas con anotacion');
  assert.match(update, /_createLoadingSource\(document, "stream"\)/);
  assert.match(update, /pdf_\.getPage\(i\)/);
  assert.doesNotMatch(update, /pdf_\.getPage\(1\)/);
  assert.match(update, /await this\._renderPage\(page, i\)/);
  assert.match(update, /await this\._add_anotate_page\(page, i\)/);
  assert.match(itext, /doc104_refresh=/);
});

test('DOC-104 envía una sola fuente de imagen al servidor', () => {
  const save = between(viewer, 'async function event_drag_anotate', 'function save_anotate_firma');
  assert.match(save, /selectedFileImageSource = hasFileImageSource \? ur_image_drag : ""/);
  assert.match(save, /selectedBase64ImageSource = hasFileImageSource \? "" : ur_image_drag_bit64/);
  assert.match(save, /page_drag, selectedFileImageSource, selectedBase64ImageSource/);
});

test('DOC-105 clasifica únicamente el PDF confirmado y respeta geometría del visor', () => {
  const replace = itext.indexOf('ReplacePdfSafely');
  const classify = itext.indexOf('EstrategiaCargaPdf.Seleccionar', replace);
  assert.ok(replace > 0 && classify > replace);
  assert.doesNotMatch(itext, /graphImage\.ScaleAbsolute\(90, 90\)/);
  assert.match(itext, /GeometriaGrafoPdf\.Matriz\(/);
  assert.match(itext, /stamper\.RotateContents = False/);
  assert.match(itext, /reader\.GetCropBox\(pageNumber\)/);
  assert.match(itext, /AddImage\(graphImage,[\s\S]*?CSng\(matrix\(5\)\)/);
  assert.doesNotMatch(itext, /Ghostscript|FastWebView|Linearized/);
});

test('runner DOC-104 exige autorizaciones antes de solicitar secretos', () => {
  const environmentGate = runner.indexOf("collectConfirmation(values, 'DOC104_E2E_ENVIRONMENT_AUTHORIZED'");
  const accountGate = runner.indexOf("collectConfirmation(values, 'DOC104_E2E_ACCOUNT_AUTHORIZED'");
  const disposableGate = runner.indexOf("collectConfirmation(values, 'DOC104_E2E_DISPOSABLE_PDF_AUTHORIZED'");
  const scenarioGate = runner.indexOf("collectConfirmation(values, 'DOC104_E2E_SCENARIO_CONFIRMED'");
  const userPrompt = runner.indexOf("collectValue(values, 'DOC104_E2E_AUTHORIZED_USER'");
  assert.ok(environmentGate > 0 && environmentGate < accountGate && accountGate < disposableGate &&
    disposableGate < scenarioGate && scenarioGate < userPrompt);
  assert.match(runner, /expectedRejectionCode: 'DOC104_DOCUMENT_SIGNED'/);
  assert.match(runner, /runReal\(profile, environment, runOptions\)/);
  assert.match(profile, /"odbcDsn":\s*"workflowdocument"/);
  assert.match(profile, /"documentStateSql":\s*"SELECT /);
});

test('runner DOC-104 clasifica rechazos ASMX sin revelar el mensaje', () => {
  assert.equal(classifySaveResponse({ d: [{ error_sistema: 'YES' }] }, 'DOC104'), null);
  assert.equal(classifySaveResponse({ d: [{ error_sistema: 'Debe proporcionar una única imagen válida' }] }, 'DOC104'), 'DOC104_IMAGE_SOURCE_INVALID');
  assert.equal(classifySaveResponse({ d: [{ error_sistema: 'El documento está firmado digitalmente' }] }, 'DOC104'), 'DOC104_DOCUMENT_SIGNED');
  assert.equal(classifySaveResponse({ d: [{ error_sistema: 'detalle inesperado' }] }, 'DOC104'), 'DOC104_SERVER_REJECTED');
  assert.equal(classifySaveResponse({ d: [] }, 'DOC104'), 'DOC104_SAVE_RESPONSE_INVALID');
});

test('runner DOC-104 prueba firmado como rechazo inmutable y no como commit', () => {
  assert.ok(require('../scripts/run-doc104-server-processing-interactive.cjs').allowedScenarios.has('signed'));
  const inheritedRunner = read('tools/e2e/scripts/run-doc103-graph-save-interactive.cjs');
  assert.match(inheritedRunner, /browserDocumentFingerprint/);
  assert.match(inheritedRunner, /beforeBinary\.hash === afterBinary\.hash/);
  assert.match(inheritedRunner, /beforeBinary\.lastModified !== ''/);
  assert.match(inheritedRunner, /beforeBinary\.lastModified === afterBinary\.lastModified/);
  assert.match(inheritedRunner, /normalizedResult: 'rejected'/);
});

test('evidencia DOC-104 tiene esquema cerrado y no admite datos sensibles', () => {
  const safe = sanitizeEvidence({
    mode: 'real', scenario: 'rotated', result: 'PASSED', postCount: 1,
    singleEffect: true, selectControlsCompleted: true, policyPassed: true
  });
  assert.deepEqual(Object.keys(safe), [
    'mode', 'scenario', 'result', 'postCount', 'singleEffect', 'selectControlsCompleted', 'policyPassed'
  ]);
  for (const forbidden of [
    { baseUrl: 'https://private.invalid/' }, { query: 'SELECT secreto' }, { cookie: 'private' },
    { token: 'private' }, { path: 'C:\\private\\file.pdf' }, { 'base64-data': 'private' },
    { usuario: 'private' }, { documentTitle: 'private' }
  ]) assert.throws(() => sanitizeEvidence({ ...safe, ...forbidden }), /DOC104_EVIDENCE_INVALID/);
});

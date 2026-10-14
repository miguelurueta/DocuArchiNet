'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '../../..');
const read = relative => fs.readFileSync(path.join(root, relative), 'utf8');
const index = read('pdfjs/pdf_anotate_view/index.html');
const viewer = read('pdfjs/pdf_anotate_view/pdf_viewer_anotate.js');
const runner = read('tools/e2e/scripts/run-doc103-graph-save-interactive.cjs');
const profile = read('tools/e2e/profiles/doc103-graph-save.profile.example.json');
const { classifySaveResponse, sanitizeEvidence, validateSelect } = require('../scripts/run-doc103-graph-save-interactive.cjs');

test('DOC-103 conserva un único propietario productivo del guardado', () => {
  assert.match(index, /<script src="\.\/pdf_viewer_anotate\.js"><\/script>/);
  assert.match(index, /<!--\s*<script src="\.\/pdfannotate\.js"><\/script>\s*-->/);
  assert.equal((index.match(/<script src="\.\/pdf_viewer_anotate\.js"><\/script>/g) || []).length, 1);
  assert.equal((index.match(/^\s*<script src="\.\/pdfannotate\.js"><\/script>/gm) || []).length, 0);
});

test('DOC-103 mantiene intactas las fronteras del servidor y capacidades vecinas', () => {
  const changed = require('node:child_process').execFileSync(
    'git', ['diff', '--name-only', 'origin/main'], { cwd: root, encoding: 'utf8' }
  );
  // DOC-104 es el propietario explícito de la evolución servidor posterior a DOC-103.
  // La regresión cliente sigue prohibiendo reactivar el visor anterior.
  const forbidden = ['pdfjs/pdf_anotate_view/pdfannotate.js'];
  for (const file of forbidden) assert.doesNotMatch(changed.replaceAll('\\', '/'), new RegExp(`^${file}$`, 'm'), file);
  const service = read('webservice/WebService_itext.asmx.vb');
  const itext = read('itextshare/Class_ItexShare.vb');
  assert.match(service, /Public Function service_anotate_pdf_image\(ByVal anotate As Object\) As IEnumerable\(Of service_itex\)/);
  assert.match(itext, /Public Class Class_itext_anotate/);
  assert.match(viewer, /service_download_pdf_visor_plus/);
  assert.match(viewer, /service_printer_pdf_visor_plus/);
  assert.match(viewer, /captureFromSTU/);
  assert.doesNotMatch(viewer, /fetch\s*\([^\n]*service_anotate_pdf_image/);
});

test('DOC-103 no expone errores crudos en el bloque transaccional', () => {
  const start = viewer.indexOf('const DOC103_ANOTATE_TIMEOUT_MS');
  const end = viewer.indexOf('function service_download_pdf_visor_plus', start);
  const transaction = viewer.slice(start, end);
  assert.ok(start > 0 && end > start);
  assert.doesNotMatch(transaction, /responseText|errorThrown|ex\.message|\balert\s*\(/);
  assert.match(transaction, /PdfViwerAlert/);
  assert.match(transaction, /timeout:\s*DOC103_ANOTATE_TIMEOUT_MS/);
  assert.match(transaction, /window\.addEventListener\("pagehide"/);
});

test('DOC-103 espera render y caché conservando el streaming parcial de DOC-102', () => {
  const start = viewer.indexOf('async  _updatePage(i,document)');
  const end = viewer.indexOf('//Agrega las paginas con anotacion', start);
  const update = viewer.slice(start, end);
  assert.match(update, /_createLoadingSource\(document, "stream"\)/);
  assert.match(update, /await this\._renderPage\(page, i\)/);
  assert.match(update, /await this\._add_anotate_page\(page, i\)/);
  assert.doesNotMatch(update, /loadDocument|forceViewerInitialization/);
});

test('runner DOC-103 bloquea antes del login si falta autorización mutadora', () => {
  const environmentGate = runner.indexOf("collectConfirmation(values, 'DOC103_E2E_ENVIRONMENT_AUTHORIZED'");
  const accountGate = runner.indexOf("collectConfirmation(values, 'DOC103_E2E_ACCOUNT_AUTHORIZED'");
  const disposableGate = runner.indexOf("collectConfirmation(values, 'DOC103_E2E_DISPOSABLE_PDF_AUTHORIZED'");
  const userPrompt = runner.indexOf("collectValue(values, 'DOC103_E2E_AUTHORIZED_USER'");
  assert.ok(environmentGate > 0 && environmentGate < accountGate && accountGate < disposableGate && disposableGate < userPrompt);
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /controlFingerprint\(profile\.documentStateSql[\s\S]*?profile\.odbcDsn/);
  assert.doesNotMatch(runner, /auditSql|auditOdbcDsn|logdocuarchi/i);
  assert.match(profile, /"odbcDsn":\s*"workflowdocument"/);
  assert.match(profile, /"documentStateSql":\s*"SELECT /);
  assert.doesNotMatch(profile, /auditSql|logdocuarchi/i);
});

test('controles DOC-103 admiten exclusivamente SELECT parametrizado', () => {
  assert.equal(validateSelect('SELECT ID FROM tabla WHERE TAREA = ?', 'INVALID'), 'SELECT ID FROM tabla WHERE TAREA = ?');
  for (const sql of ['UPDATE tabla SET valor = ?', 'SELECT * FROM tabla', 'SELECT * FROM tabla WHERE id = ?; DELETE FROM tabla']) {
    assert.throws(() => validateSelect(sql, 'INVALID'), /INVALID/);
  }
});

test('runner DOC-103 termina inmediatamente ante rechazo saneado', () => {
  assert.match(runner, /page\.waitForResponse/);
  assert.match(runner, /classifySaveResponse\(savePayload, failurePrefix\)/);
  assert.equal(classifySaveResponse({ d: [{ error_sistema: 'El documento está siendo actualizado' }] }), 'DOC103_DOCUMENT_LOCKED');
});

test('evidencia DOC-103 tiene esquema cerrado y rechaza datos sensibles', () => {
  const safe = sanitizeEvidence({
    mode: 'double', result: 'PASSED', scenarios: 11, postCount: 1, normalizedResult: 'matrix_passed',
    overlayRemoved: true, activePagePreserved: true, zoomPreserved: true, scrollPreserved: true,
    singleEffect: true, selectControlsCompleted: false
  });
  assert.deepEqual(Object.keys(safe), [
    'mode', 'result', 'scenarios', 'postCount', 'normalizedResult', 'overlayRemoved',
    'activePagePreserved', 'zoomPreserved', 'scrollPreserved', 'singleEffect', 'selectControlsCompleted'
  ]);
  for (const forbidden of [
    { baseUrl: 'https://private.invalid/' }, { query: 'SELECT secreto' }, { cookie: 'private' },
    { token: 'private' }, { path: 'C:\\private\\file.pdf' }, { payload: 'base64-private' },
    { usuario: 'private' }, { documentTitle: 'private' }
  ]) assert.throws(() => sanitizeEvidence({ ...safe, ...forbidden }), /DOC103_EVIDENCE_INVALID/);
});

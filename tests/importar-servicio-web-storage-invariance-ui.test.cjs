'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const crypto = require('node:crypto');

const hash = (content) => {
  const normalized = Buffer.from(content.replace(/\r\n/g, '\n'));
  return crypto.createHash('sha1').update(Buffer.concat([Buffer.from(`blob ${normalized.length}\0`), normalized])).digest('hex');
};

test('ClassAlmacenamiento y JSProgresBar conservan sus huellas aprobadas', () => {
  assert.equal(hash(fs.readFileSync('workflow/ClassAlmacenamiento.vb', 'utf8')), 'b875d24f0a9ff63f24a4fff96f637cb04afb1405');
  const progress = fs.readFileSync('js/java_general/JSProgresBar.js', 'utf8');
  assert.doesNotMatch(progress, /WebServiceImportarServicioWebModern|ExecuteImportIntent|CreateImportIntent/);
});

test('la escritura moderna sigue centralizada en un único adaptador', () => {
  const adapter = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  assert.equal((adapter.match(/\.AlmacenaDocumentoTareaWorkflow\(/g) || []).length, 1);
  assert.equal((steps.match(/_storage\.Almacenar\(command\)/g) || []).length, 1);
});

test('la proyección documental deduplica por DocumentId y restringe la tarea', () => {
  const list = fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-document-list-adapter.js', 'utf8');
  assert.match(list, /itemTask !== task \|\| seen\[documentId\]/);
  assert.match(list, /seen\[documentId\] = true/);
});

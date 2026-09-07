const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const src = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');
const legacy = fs.readFileSync('workflow/ClassAlmacenamiento.vb', 'utf8');

test('adaptador es el unico componente moderno que invoca storage legacy', () => {
  assert.match(src, /New ClassAlmacenamiento\(\)\.AlmacenaDocumentoTareaWorkflow/);
  assert.match(src, /1, comando\.NombreGabinete, comando\.Radicado, comando\.RutaArchivo/);
  assert.match(src, /comando\.NombreCaso, comando\.NombreClaseFormatoDocumento/);
});

test('mapea YES, rechazo y excepcion sin filtrar el mensaje legacy', () => {
  assert.match(src, /String\.Equals\(respuesta, "YES"/);
  assert.match(src, /DOCUMENT_STORAGE_REJECTED/);
  assert.match(src, /DOCUMENT_STORAGE_UNCERTAIN/);
  assert.doesNotMatch(src, /MensajeVisible = respuesta/);
});

test('la caja negra permanece declarada con su firma conocida', () => {
  assert.match(legacy, /Function AlmacenaDocumentoTareaWorkflow\([\s\S]*ByRef EstructuraDatosImagen/);
});

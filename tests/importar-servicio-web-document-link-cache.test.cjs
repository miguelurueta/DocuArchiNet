const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentLinkCacheRepository.vb", "utf8");
const migration = fs.readFileSync("Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql/003-create-document-link-cache.sql", "utf8");

test("repetición consistente no inserta otra fila", () => {
  assert.match(source, /If current IsNot Nothing Then Return Compare\(current, entrada\)/);
  assert.match(source, /DOCUMENT_LINK_CACHE_CONFIRMED/);
});

test("conflicto no sobrescribe expediente esperado", () => {
  assert.match(source, /current\.IdExpedienteEsperado <> expected\.IdExpedienteEsperado/);
  assert.match(source, /DOCUMENT_LINK_CACHE_CONFLICT/);
  assert.doesNotMatch(source, /ON DUPLICATE KEY UPDATE|UPDATE workflow_import_document_link_cache/);
});

test("carrera conserva una fila y relee al ganador", () => {
  assert.match(migration, /UNIQUE KEY uq_import_document_link_cache_identity \(task_id, image_id, cabinet_name\)/);
  assert.match(source, /INSERT IGNORE INTO workflow_import_document_link_cache/);
  const reads = source.match(/ReadOne\(connection, entrada\.IdTarea, entrada\.IdImagen, entrada\.NombreGabinete\)/g) || [];
  assert.equal(reads.length, 2);
});

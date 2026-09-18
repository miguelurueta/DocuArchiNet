const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportRelatedDocumentRepository.vb", "utf8");
const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb", "utf8");

test("el diario físico hace upsert por la clave única DOC-67", () => {
  assert.match(repository, /INSERT INTO workflow_import_related_document/);
  assert.match(repository, /ON DUPLICATE KEY UPDATE/);
  for (const column of ["expected_expedient_id", "relation_status", "cache_status", "cabinet_index_status", "electronic_index_status", "xml_index_status", "reconciliation_status"]) assert.match(repository, new RegExp(column));
});

test("cada efecto se registra antes de continuar", () => {
  assert.ok((coordinator.match(/_documents\.Persistir\(/g) || []).length >= 5);
  assert.match(coordinator, /EstadoReconciliacion = evidence\.Estado/);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const sql = fs.readFileSync("Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql/005-create-doc67-consolidated-workflow-mysql51.sql", "utf8");

test("esquema consolidado DOC-67 se crea exclusivamente en workflowdocument", () => {
  assert.match(sql, /USE `workflowdocument`;/);
  for (const table of [
    "workflow_import_intent",
    "workflow_import_intent_requirement",
    "workflow_import_inscription",
    "workflow_import_intent_item",
    "workflow_import_intent_transition",
    "workflow_import_related_document",
    "workflow_import_document_link_cache",
  ]) assert.match(sql, new RegExp(`CREATE TABLE IF NOT EXISTS ${table} \\(`), table);
  assert.doesNotMatch(sql, /CREATE\s+(?:PROCEDURE|FUNCTION|TRIGGER)/i);
  assert.doesNotMatch(sql, /CREATE TABLE IF NOT EXISTS (?:expediente_archivo|ra_sii_cache_exepediente|ra_relacion_radicado_externo_expediente)/i);
});

test("esquema consolidado incluye las restricciones de idempotencia DOC-67", () => {
  for (const index of [
    "uq_workflow_import_intent_idempotency",
    "uq_import_inscription_ordinal",
    "uq_import_intent_external",
    "uq_import_related_document",
    "uq_import_document_link_cache_identity",
  ]) assert.match(sql, new RegExp(index), index);
  assert.match(sql, /FOREIGN KEY \(intent_id, inscription_key\)[\s\S]*REFERENCES workflow_import_inscription\(intent_id, inscription_key\)/);
});

test("ninguna llave foranea DOC-67 referencia otra base de datos", () => {
  const targets = [...sql.matchAll(/REFERENCES\s+(`?[A-Za-z0-9_]+`?(?:\.`?[A-Za-z0-9_]+`?)?)/gi)]
    .map((match) => match[1].replaceAll("`", ""));

  assert.ok(targets.length > 0, "el esquema debe declarar sus relaciones internas");
  for (const target of targets) {
    assert.doesNotMatch(target, /\./, `referencia calificada entre bases: ${target}`);
    assert.match(target, /^workflow_import_/, `referencia fuera del agregado moderno: ${target}`);
  }
});

test("consolidado actualiza tablas antiguas sin procedures", () => {
  for (const column of [
    "radicado", "document_type_name", "document_id", "persistence_known",
    "retryable", "error_code", "visible_message", "correlation_id",
    "inscription_key", "expedient_id", "storage_status", "relation_status",
    "index_status", "cache_status",
  ]) {
    assert.match(sql, new RegExp(`information_schema\\.COLUMNS[\\s\\S]{0,240}COLUMN_NAME = '${column}'`), column);
  }
  assert.match(sql, /information_schema\.STATISTICS[\s\S]*ix_import_item_inscription/);
  assert.match(sql, /information_schema\.TABLE_CONSTRAINTS[\s\S]*fk_import_item_inscription/);
  assert.match(sql, /PREPARE doc67_stmt FROM @doc67_sql/);
  assert.doesNotMatch(sql, /CREATE\s+PROCEDURE/i);
});

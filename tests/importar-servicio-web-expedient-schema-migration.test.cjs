const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const sqlRoot = path.join(root, "Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql");
const up = fs.readFileSync(path.join(sqlRoot, "001-add-expedient-planning-state.sql"), "utf8");
const down = fs.readFileSync(path.join(sqlRoot, "002-rollback-expedient-planning-state.sql"), "utf8");

test("versiona agregado de inscripcion y plan fisico", () => {
  assert.match(up, /CREATE TABLE IF NOT EXISTS workflow_import_inscription/);
  assert.match(up, /PRIMARY KEY \(intent_id, inscription_key\)/);
  assert.match(up, /CREATE TABLE IF NOT EXISTS workflow_import_related_document/);
  assert.match(up, /UNIQUE KEY uq_import_related_document \(intent_id, task_id, image_id, cabinet_name\)/);
});

test("extiende items con asociacion y estados por efecto compatibles", () => {
  for (const column of ["inscription_key", "expedient_id", "storage_status", "relation_status", "index_status", "cache_status"]) {
    assert.match(up, new RegExp(`doc67_add_column\\('workflow_import_intent_item', '${column}'`));
  }
  assert.match(up, /DEFAULT ''Pendiente''/);
  assert.match(up, /fk_import_item_inscription/);
});

test("forward es reejecutable y rollback protege datos materializados", () => {
  assert.match(up, /information_schema\.COLUMNS/);
  assert.match(up, /CREATE TABLE IF NOT EXISTS/g);
  assert.match(down, /SIGNAL SQLSTATE '45000'/);
  assert.match(down, /inscription_key IS NOT NULL OR expedient_id IS NOT NULL/);
  assert.match(down, /DROP TABLE IF EXISTS workflow_import_related_document/);
  assert.match(down, /DROP TABLE IF EXISTS workflow_import_inscription/);
});

test("migracion no altera ni elimina tablas legacy", () => {
  for (const sql of [up, down]) {
    assert.doesNotMatch(sql, /ALTER TABLE\s+(?:expediente_archivo|ra_sii_|ra_relacion_|ra_cert_)/i);
    assert.doesNotMatch(sql, /DROP TABLE IF EXISTS\s+(?:expediente_archivo|ra_sii_|ra_relacion_|ra_cert_)/i);
  }
});


const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const sqlRoot = path.join(root, "Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql");
const up = fs.readFileSync(path.join(sqlRoot, "003-create-document-link-cache.sql"), "utf8");
const down = fs.readFileSync(path.join(sqlRoot, "004-rollback-document-link-cache.sql"), "utf8");

test("cache documental conserva identidad expediente y radicado", () => {
  assert.match(up, /CREATE TABLE IF NOT EXISTS workflow_import_document_link_cache/);
  for (const column of ["task_id", "image_id", "cabinet_name", "expected_expedient_id", "sii_radicado", "relation_status", "created_utc", "verified_utc"]) {
    assert.match(up, new RegExp(`\\b${column}\\b`));
  }
});

test("constraint persistente rechaza el duplicado de una carrera", () => {
  const unique = up.match(/UNIQUE KEY uq_import_document_link_cache_identity \(([^)]+)\)/);
  assert.ok(unique);
  assert.deepEqual(unique[1].split(/,\s*/), ["task_id", "image_id", "cabinet_name"]);
  assert.doesNotMatch(unique[1], /intent_id/);

  const contenders = [
    { task_id: 71, image_id: 9001, cabinet_name: "MERCANTIL", expedient: 100 },
    { task_id: 71, image_id: 9001, cabinet_name: "MERCANTIL", expedient: 200 },
  ];
  const identities = contenders.map((row) => [row.task_id, row.image_id, row.cabinet_name].join("|"));
  assert.equal(new Set(identities).size, 1, "ambos inserts compiten por la misma clave unica");
});

test("rollback es reejecutable y no descarta cache con datos", () => {
  assert.match(down, /information_schema\.TABLES/);
  assert.match(down, /SELECT COUNT\(\*\) INTO @doc67_cache_rows/);
  assert.match(down, /SIGNAL SQLSTATE '45000'/);
  assert.match(down, /DROP TABLE IF EXISTS workflow_import_document_link_cache/);
});

test("migracion no altera cache ni relaciones legacy", () => {
  for (const sql of [up, down]) {
    assert.doesNotMatch(sql, /ALTER TABLE\s+ra_/i);
    assert.doesNotMatch(sql, /DROP TABLE IF EXISTS\s+ra_/i);
  }
});


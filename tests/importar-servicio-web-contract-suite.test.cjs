const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const dto = fs.readFileSync(path.join(root, "DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb"), "utf8");
const fixturesDir = path.join(root, "Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1");
const operations = ["ResolveCapabilities", "QueryItems", "GetPreview", "PreflightImport", "CreateImportIntent", "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"];

test("publica request y response para las ocho operaciones", () => {
  for (const operation of operations) {
    assert.match(dto, new RegExp(`Public Class ${operation}RequestDto`), operation);
    assert.match(dto, new RegExp(`Public Class ${operation}ResponseDto`), operation);
  }
});

test("los fixtures compartidos conservan envelope v1 y correlacion", () => {
  const files = fs.readdirSync(fixturesDir).filter((name) => name.endsWith(".json"));
  assert.equal(files.length, 8);
  for (const name of files) {
    const raw = fs.readFileSync(path.join(fixturesDir, name), "utf8");
    const value = JSON.parse(raw);
    assert.equal(value.schemaVersion, "1.0", name);
    assert.match(value.operationId, /^op-/i, name);
    assert.match(value.correlationId, /^corr-/i, name);
    assert.doesNotMatch(raw, /password|cookie|authorization|connectionString/i, name);
  }
});

test("los estados contractuales pertenecen al vocabulario aprobado", () => {
  const allowed = new Set(["Creada", "Validada", "Parcial", "DocumentoAlmacenado", "Reconciliada", "Completado", "Disponible"]);
  for (const name of fs.readdirSync(fixturesDir).filter((item) => item.endsWith("-response.json"))) {
    const value = JSON.parse(fs.readFileSync(path.join(fixturesDir, name), "utf8"));
    if (value.status) assert.equal(allowed.has(value.status), true, `${name}: ${value.status}`);
    for (const item of value.items || []) if (item.status) assert.equal(allowed.has(item.status), true, `${name}: ${item.status}`);
  }
});

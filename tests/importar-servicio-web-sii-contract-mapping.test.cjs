const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const root = path.resolve(__dirname, "..");
const mapper = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb"), "utf8");
const fixtures = path.join(root, "tests/Fixtures/Workflow/ImportarServicioWeb/sii-v1");

test("fixtures SII saneados expresan la clave estable", () => {
  const raw = JSON.parse(fs.readFileSync(path.join(fixtures, "query-provider-response.json"), "utf8"));
  const normalized = JSON.parse(fs.readFileSync(path.join(fixtures, "normalized-query-response.json"), "utf8"));
  assert.equal(raw.items[0].registration, "REG-DEMO-001");
  assert.equal(normalized.items[0].externalKey, "SII:BOOK-DEMO:REG-DEMO-001:MAT-DEMO-001");
});

test("mapper normaliza campos SII y prepara comando documental", () => {
  assert.match(mapper, /BuildExternalKey/);
  assert.match(mapper, /"SII:" & Segment\(book\)/);
  assert.match(mapper, /CreateDocumentCommand/);
  assert.doesNotMatch(mapper, /Authorization|apiKey|ClassAlmacenamiento/i);
});

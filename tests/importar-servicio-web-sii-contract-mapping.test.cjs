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
  assert.equal(raw.inscripciones[0].registro, "900001");
  assert.equal(raw.inscripciones[0].imagenes[0].idanexo, "attachment-demo-001");
  assert.equal(normalized.items[0].externalKeyPrefix, "SII2.");
});

test("mapper normaliza campos SII y prepara comando documental", () => {
  assert.match(mapper, /BuildExternalKey/);
  assert.match(mapper, /"SII2\." & Encode\(codigoBarras\)/);
  assert.match(mapper, /For Each inscription[\s\S]*For Each image/);
  assert.match(mapper, /Value\(image, "idanexo"\)/);
  assert.match(mapper, /GetValue\(name, StringComparison\.OrdinalIgnoreCase\)/);
  assert.match(mapper, /NormalizeSource/);
  assert.match(mapper, /NullableDateTime\(Token\(source, "expiresAtUtc"\)\)/);
  assert.match(mapper, /token\.Type = JTokenType\.Date/);
  assert.match(mapper, /token\.ToObject\(Of DateTimeOffset\)\(\)\.UtcDateTime/);
  assert.match(mapper, /DateTimeOffset\.TryParse[\s\S]*DateTimeStyles\.RoundtripKind/);
  assert.doesNotMatch(mapper, /DateTime\.TryParse\(token\.ToString\(\)/);
  assert.match(mapper, /SII_INSCRIPTIONS_MISSING|SII_INSCRIPTIONS_EMPTY|SII_IMAGES_EMPTY/);
  assert.match(mapper, /Case "tif", "tiff" : Return "image\/tiff"/);
  assert.match(mapper, /SII_TOKEN_INVALID/);
  assert.match(mapper, /SII_QUERY_PROVIDER_ERROR/);
  assert.match(mapper, /CreateDocumentCommand/);
  assert.match(mapper, /DocumentTypeName = item\.DocumentTypeName/);
  assert.doesNotMatch(mapper, /Authorization|apiKey|ClassAlmacenamiento/i);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const dto = fs.readFileSync("DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb", "utf8");
const mapper = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb", "utf8");
const fixtures = path.join("Tests/Fixtures/Workflow/ImportarServicioWeb/expedient-contracts-v1");

test("contrato 1.1 es aditivo y conserva DTO item 1.0", () => {
  assert.match(dto, /Class ImportItemResultDto[\s\S]*?End Class/);
  assert.match(dto, /Class ImportItemExpedientEffectsDto[\s\S]*SchemaVersion = "1\.1"/);
  const legacyItem = dto.slice(dto.indexOf("Class ImportItemResultDto"), dto.indexOf("End Class", dto.indexOf("Class ImportItemResultDto")));
  assert.doesNotMatch(legacyItem, /Expedient|RelationStatus|IndexStatus|SchemaVersion/);
  for (const response of ["ExecuteImportIntentResponseDto", "GetImportIntentResponseDto", "ReconcileImportIntentResponseDto"]) {
    const start = dto.indexOf(`Class ${response}`);
    const block = dto.slice(start, dto.indexOf("End Class", start));
    assert.match(block, /ExpedientEffects As IList\(Of ImportItemExpedientEffectsDto\)/, response);
  }
});

test("mapeo distingue expediente relacion cache y SQL XML", () => {
  for (const code of ["EXPEDIENT_UNRESOLVED", "DOCUMENT_DESTINATION_CONFLICT", "DOCUMENT_RELATION_MISSING", "DOCUMENT_RELATION_DUPLICATED", "DOCUMENT_RELATION_CROSSED", "DOCUMENT_LINK_CACHE_CONFLICT", "DOCUMENT_LINK_CACHE_WRITE_UNCERTAIN", "DOCUMENT_INDEX_UPDATE_FAILED", "ELECTRONIC_INDEX_SQL_MISSING", "ELECTRONIC_INDEX_XML_MISSING", "EXPEDIENT_EFFECT_UNCERTAIN"]) {
    assert.match(mapper, new RegExp(`"${code}"`), code);
  }
  assert.match(mapper, /EstadoIndiceSql <> EstadoEfectoExpedienteImportacion\.Confirmado/);
  assert.match(mapper, /EstadoIndiceXml <> EstadoEfectoExpedienteImportacion\.Confirmado/);
});

test("fixtures 1.1 cubren resultados y no contienen datos sensibles", () => {
  const files = fs.readdirSync(fixtures).filter((name) => name.endsWith(".json"));
  assert.equal(files.length, 6);
  const codes = new Set();
  for (const name of files) {
    const raw = fs.readFileSync(path.join(fixtures, name), "utf8");
    const value = JSON.parse(raw);
    assert.equal(value.schemaVersion, "1.1", name);
    assert.match(value.resultCode, /^[A-Z0-9_]+$/, name);
    assert.equal(typeof value.retryable, "boolean", name);
    assert.ok(value.effects, name);
    assert.doesNotMatch(raw, /password|cookie|authorization|connection|string|stack|exception|ruta|\b(?:select|insert|update|delete)\b/i, name);
    codes.add(value.resultCode);
  }
  assert.equal(codes.has("EXPEDIENT_EFFECTS_CONFIRMED"), true);
  assert.equal(codes.has("DOCUMENT_RELATION_CROSSED"), true);
  assert.equal(codes.has("DOCUMENT_LINK_CACHE_CONFLICT"), true);
  assert.equal(codes.has("ELECTRONIC_INDEX_XML_MISSING"), true);
});

test("fallos ocultan identificador de expediente", () => {
  for (const name of fs.readdirSync(fixtures).filter((item) => item.endsWith(".json"))) {
    const value = JSON.parse(fs.readFileSync(path.join(fixtures, name), "utf8"));
    if (value.resultCode !== "EXPEDIENT_EFFECTS_CONFIRMED") assert.equal(value.expedientId, null, name);
  }
  assert.match(mapper, /If dto\.ResultCode <> ImportExpedientResultCodes\.Confirmed Then dto\.ExpedientId = Nothing/);
});

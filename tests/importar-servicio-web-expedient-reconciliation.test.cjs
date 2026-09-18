const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const fixtures = JSON.parse(fs.readFileSync("Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v2/expedient-effects.json", "utf8"));
const service = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb", "utf8");
const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb", "utf8");
const mapper = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb", "utf8");
const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");

function classify(x) {
  if (!x.expedient) return "EXPEDIENT_UNRESOLVED";
  if (x.relation === "Ausente") return "DOCUMENT_RELATION_MISSING";
  if (x.relation === "Duplicada") return "DOCUMENT_RELATION_DUPLICATED";
  if (x.relation === "Cruzada") return "DOCUMENT_RELATION_CROSSED";
  if (x.cache === "Conflicto") return "DOCUMENT_LINK_CACHE_CONFLICT";
  if (x.cabinetIndex !== "Confirmado") return "DOCUMENT_INDEX_UPDATE_FAILED";
  if (x.sql !== "Confirmado") return "ELECTRONIC_INDEX_SQL_MISSING";
  if (x.xml !== "Confirmado") return "ELECTRONIC_INDEX_XML_MISSING";
  return "EXPEDIENT_EFFECTS_CONFIRMED";
}

test("fixtures detectan todas las divergencias requeridas y documento nuevo", () => {
  for (const fixture of fixtures) assert.equal(classify(fixture), fixture.expected, fixture.case);
  assert.deepEqual(fixtures.map(x => x.case), ["missing-expedient", "duplicated-relation", "crossed-relation", "stale-cache", "sql-xml-divergence", "new-document"]);
});

test("repositorio carga expediente relación cache tres índices SQL y XML", () => {
  for (const column of ["expected_expedient_id", "destination_status", "relation_status", "cache_status", "cabinet_index_status", "electronic_index_status", "xml_index_status", "reconciliation_status"]) {
    assert.match(repository, new RegExp(column));
  }
  assert.match(repository, /LEFT JOIN workflow_import_document_link_cache/);
  assert.match(repository, /cached_expedient_id/);
});

test("cache obsoleta se convierte en conflicto sin sobrescritura", () => {
  assert.match(repository, /Convert\.ToInt64\(reader\("cached_expedient_id"\)\) <> document\.IdExpedienteEsperado\.Value/);
  assert.match(repository, /document\.EstadoCache = EstadoEfectoExpedienteImportacion\.Conflicto/);
  assert.doesNotMatch(repository, /UPDATE workflow_import_document_link_cache|DELETE FROM workflow_import_document_link_cache/);
});

test("servicio oculta documento hasta confirmar todos los efectos", () => {
  assert.match(service, /ApplyExpedientEvidence\(mapped, item, snapshot\.DocumentosRelacionados\)/);
  assert.match(service, /effect\.ResultCode <> ImportExpedientResultCodes\.Confirmed/);
  assert.match(service, /mapped\.DocumentId = Nothing/);
  assert.match(mapper, /EstadoIndiceSql <> EstadoEfectoExpedienteImportacion\.Confirmado/);
  assert.match(mapper, /EstadoIndiceXml <> EstadoEfectoExpedienteImportacion\.Confirmado/);
});

test("documento nuevo o duplicado queda visible para reconciliación pero no disponible", () => {
  assert.match(service, /If matches\.Count = 0 Then/);
  assert.match(service, /If matches\.Count > 1 Then/);
  assert.match(models, /Property DocumentosRelacionados As IList\(Of DocumentoRelacionadoImportacion\)/);
});

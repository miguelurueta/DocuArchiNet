const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const resolver = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportInscriptionResolver.vb", "utf8");
const service = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb", "utf8");
const composition = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");

test("la intención SII reconstruye inscripciones desde el proveedor y no desde el cliente", () => {
  assert.match(resolver, /TryParseExternalKey/);
  assert.match(resolver, /ResolveInscriptionsAsync/);
  assert.match(resolver, /SII_INSCRIPTION_BARCODE_CONFLICT/);
  assert.doesNotMatch(resolver, /String\.Equals\(barcode, request\.Radicado/);
  assert.match(resolver, /SII_INSCRIPTION_ITEM_UNRESOLVED/);
  assert.match(service, /intent\.Inscripciones = _inscriptions\.Resolver/);
});

test("el radicado Workflow y el código de barras SII conservan identidades independientes", () => {
  assert.match(resolver, /\.CodigoBarras = barcode/);
  assert.match(service, /\.Radicado = request\.Radicado\.Trim\(\)/);
  assert.doesNotMatch(resolver, /SII_INSCRIPTION_RADICADO_CONFLICT/);
});

test("la composición productiva exige el resolvedor autoritativo", () => {
  assert.match(composition, /New SiiImportInscriptionResolver\(siiProvider, expedientConfiguration\)/);
  assert.match(service, /SII_INSCRIPTION_RESOLVER_UNAVAILABLE/);
  assert.match(composition, /Catch ex As InvalidOperationException[\s\S]*SafeIntentCreationCode\(ex\.Message\)/);
  assert.match(composition, /Case "SII_INSCRIPTION_CONTEXT_INVALID"[\s\S]*"EXPEDIENT_CONFIGURATION_UNAVAILABLE"[\s\S]*"SII_INSCRIPTION_ITEM_UNRESOLVED"/);
  assert.match(resolver, /EXPEDIENT_CONFIGURATION_QUERY_FAILED/);
  assert.match(resolver, /Catch ex As InvalidOperationException\s+Throw\s+Catch/);
  assert.match(resolver, /SII_INSCRIPTION_QUERY_FAILED/);
});

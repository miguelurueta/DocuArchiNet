const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const gateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyDocumentExpedientPhysicalGateway.vb", "utf8");
const adapter = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb", "utf8");

test("el gateway moderno parametriza lectura y usa autoridad explícita", () => {
  assert.match(gateway, /WHERE ID=@imageId LIMIT 2/);
  assert.match(gateway, /New MySqlParameter\("@imageId"/);
  assert.match(gateway, /VinculaDocumentoExpedienteConContexto/);
  assert.match(gateway, /SELECT GET_LOCK/);
  assert.doesNotMatch(gateway, /HttpContext|\.Session|EnsureSessionMatches/);
  assert.ok(adapter.indexOf("Dim before = Consultar") < adapter.indexOf("_gateway.VincularDocumento"));
  assert.ok(adapter.indexOf("_gateway.VincularDocumento") < adapter.indexOf("Dim after = Consultar"));
});

test("YES no se usa como prueba de vínculo", () => {
  assert.match(adapter, /respuestaMutador/);
  assert.doesNotMatch(adapter, /If\s+respuestaMutador\s*=\s*"YES"/i);
  assert.match(adapter, /DOCUMENT_RELATION_WRITE_UNKNOWN/);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const adapter = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb", "utf8");
const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb", "utf8");

function verify(sql, xml) {
  return { sql, xml, confirmed: sql && xml };
}

test("SQL y XML se consultan independientemente", () => {
  assert.deepEqual(verify(true, false), { sql: true, xml: false, confirmed: false });
  assert.match(adapter, /evidence\.SqlConfirmado = _gateway\.ExisteIndiceSql/);
  assert.match(adapter, /evidence\.XmlConfirmado = _gateway\.ExisteIndiceXml/);
  assert.doesNotMatch(adapter, /XmlConfirmado\s*=\s*evidence\.SqlConfirmado/);
});

test("XML inconsistente bloquea confirmación aunque SQL exista", () => {
  assert.equal(verify(true, false).confirmed, false);
  assert.match(adapter, /evidence\.SqlConfirmado AndAlso evidence\.XmlConfirmado/);
  assert.match(coordinator, /RELATED_DOCUMENT_SQL_INDEX_MISSING/);
  assert.match(coordinator, /RELATED_DOCUMENT_XML_INDEX_MISSING/);
  assert.match(coordinator, /EstadoIndiceXml = If\(evidence\.XmlConfirmado/);
});

test("los tres índices de gabinete se leen después de actualizar", () => {
  for (const field of ["NITCEDULA", "RAZONSOCIAL", "MATRICULA"]) assert.match(adapter, new RegExp(`AddIfPresent\\(fields, "${field}"`));
  const update = adapter.indexOf("_gateway.ActualizarCampos");
  const read = adapter.indexOf("_gateway.LeerCampos", update);
  assert.ok(update >= 0 && read > update);
});

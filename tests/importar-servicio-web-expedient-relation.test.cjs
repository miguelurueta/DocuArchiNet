const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb", "utf8");

test("repetición conserva relación correcta sin mutar", () => {
  assert.match(source, /If before\.Estado <> EstadoEfectoExpedienteImportacion\.Ausente Then Return before/);
  assert.equal((source.match(/_gateway\.VincularDocumento\(/g) || []).length, 1);
});

test("duplicada o cruzada detiene el efecto", () => {
  assert.match(source, /relations\.Count > 1[\s\S]*DOCUMENT_RELATION_DUPLICATED/);
  assert.match(source, /relations\(0\) <> documento\.IdExpedienteEsperado\.Value[\s\S]*DOCUMENT_RELATION_CROSSED/);
});

test("éxito textual ambiguo exige postcheck", () => {
  const mutate = source.indexOf("_gateway.VincularDocumento");
  const verify = source.indexOf("Dim after = Consultar", mutate);
  assert.ok(mutate >= 0 && verify > mutate);
  assert.doesNotMatch(source, /respuestaMutador\s*=\s*"YES"/);
});

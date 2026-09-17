const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

function inspect(relations, expected) {
  if (!relations.length) return "absent";
  if (relations.length > 1) return "duplicated";
  return relations[0] === expected ? "correct" : "crossed";
}

test("precheck reconoce relación correcta y evita mutación", () => {
  assert.equal(inspect([31], 31), "correct");
  assert.match(source, /If before\.Estado <> EstadoEfectoExpedienteImportacion\.Ausente Then Return before/);
  assert.match(source, /DOCUMENT_RELATION_CONFIRMED/);
});

test("relación ausente se crea una sola vez y se postverifica", () => {
  assert.equal(inspect([], 31), "absent");
  const mutation = source.indexOf("_gateway.VincularDocumento");
  const postcheck = source.indexOf("Dim after = Consultar", mutation);
  assert.ok(mutation >= 0 && postcheck > mutation);
  assert.match(source, /DOCUMENT_RELATION_ABSENT/);
});

test("relaciones duplicada y cruzada son conflictos distintos", () => {
  assert.equal(inspect([31, 31], 31), "duplicated");
  assert.equal(inspect([44], 31), "crossed");
  assert.match(source, /DOCUMENT_RELATION_DUPLICATED/);
  assert.match(source, /EstadoRelacionDocumentoExpediente\.Duplicada/);
  assert.match(source, /DOCUMENT_RELATION_CROSSED/);
  assert.match(source, /EstadoRelacionDocumentoExpediente\.Cruzada/);
});

test("YES ambiguo nunca confirma sin lectura física", () => {
  assert.match(source, /respuestaMutador = _gateway\.VincularDocumento/);
  assert.match(source, /Dim after = Consultar\(contexto, documento\)/);
  assert.doesNotMatch(source, /If respuestaMutador\s*=\s*"YES"/);
  assert.match(source, /DOCUMENT_RELATION_WRITE_UNKNOWN/);
});

test("adaptador usa contexto y no invoca ASMX", () => {
  for (const operation of ["ConsultarExpedientes", "VincularDocumento"]) {
    assert.match(source, new RegExp(`Function ${operation}\\(ByVal contexto As ContextoImportacionServicio`));
  }
  assert.doesNotMatch(source, /HttpWebRequest|WebClient|\.asmx|HttpContext|Session/);
  const include = "Infrastructure\\Workflow\\ImportarServicioWeb\\Expedients\\DocumentExpedientRelationAdapter.vb";
  assert.equal(project.split(include).length - 1, 1);
});

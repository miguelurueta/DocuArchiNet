const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

function fields(cabinet, input) {
  let enrollment = input.normalized || input.enrollment || "";
  if (cabinet === "ESAL" && enrollment.toUpperCase().startsWith("S0")) enrollment = enrollment.slice(2);
  return { ENLASE: input.radicado, RECIBOCAJA: input.radicado, NITCEDULA: input.subject.trim(), RAZONSOCIAL: input.name.trim().slice(0, 40), MATRICULA: enrollment.trim(), LIBRO: input.libro, INSCRIPCION: input.registro };
}

test("candidatos SII se filtran posteriormente por la estructura dinámica", () => {
  const input = { subject: " 9001 ", name: "Sociedad de prueba", enrollment: "S00123", normalized: "", radicado: "R1", libro: "1", registro: "2" };
  assert.equal(fields("ESAL", input).MATRICULA, "0123");
  for (const field of ["NITCEDULA", "RAZONSOCIAL", "MATRICULA"]) assert.match(source, new RegExp(`AddIfPresent\\(fields, "${field}"`));
  for (const field of ["ENLASE", "RECIBOCAJA", "LIBRO", "INSCRIPCION"]) assert.doesNotMatch(source, new RegExp(`AddIfPresent\\(fields, "${field}"`));
  assert.doesNotMatch(source, /cabinet <> "MERCANTIL"/);
});

test("la actualización posterior no reescribe campos propios de incorporación", () => {
  for (const field of ["ENLASE", "RECIBOCAJA", "LIBRO", "INSCRIPCION"]) assert.doesNotMatch(source, new RegExp(`AddIfPresent\\(fields, "${field}"`));
});

test("actualización se confirma releyendo el conjunto efectivo", () => {
  const update = source.indexOf("_gateway.ActualizarCampos");
  const read = source.indexOf("_gateway.LeerCampos", update);
  const compare = source.indexOf("MismatchedField(fields, persisted)", read);
  assert.ok(update >= 0 && read > update && compare > read);
  assert.match(source, /DOCUMENT_INDEX_UPDATE_REJECTED/);
  assert.match(source, /DOCUMENT_INDEX_FIELD_NOT_CONFIRMED_/);
});

test("SQL correcto y XML ausente permanecen diferenciados", () => {
  const evidence = (sql, xml) => ({ SqlConfirmado: sql, XmlConfirmado: xml, state: sql && xml ? "confirmed" : "absent" });
  assert.deepEqual(evidence(true, false), { SqlConfirmado: true, XmlConfirmado: false, state: "absent" });
  assert.match(source, /evidence\.SqlConfirmado = _gateway\.ExisteIndiceSql/);
  assert.match(source, /evidence\.XmlConfirmado = _gateway\.ExisteIndiceXml/);
  assert.match(source, /evidence\.SqlConfirmado AndAlso evidence\.XmlConfirmado/);
});

test("verificaciones físicas son operaciones independientes", () => {
  assert.match(source, /Function ExisteIndiceSql\(ByVal contexto As ContextoImportacionServicio/);
  assert.match(source, /Function ExisteIndiceXml\(ByVal contexto As ContextoImportacionServicio/);
  assert.doesNotMatch(source, /XmlConfirmado\s*=\s*evidence\.SqlConfirmado/);
  assert.doesNotMatch(source, /HttpWebRequest|WebClient|\.asmx|HttpContext|Session/);
});

test("adaptador se registra una sola vez", () => {
  const include = "Infrastructure\\Workflow\\ImportarServicioWeb\\Expedients\\SiiDocumentIndexAdapter.vb";
  assert.equal(project.split(include).length - 1, 1);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const source = fs.readFileSync(path.join(root, "Docuarchi/ClassDaGabinete.vb"), "utf8");
function body(name, next) {
  const start = source.indexOf(`Function ${name}`);
  const end = source.indexOf(next, start);
  assert.notEqual(start, -1); assert.notEqual(end, -1);
  return source.slice(start, end);
}
const single = body("SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII", "Function SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII");
const multiple = body("SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII", "Function ActualizaIndiceDocumentoCacheExpediente");
const indexUpdate = body("ActualizaIndiceDocumentoCacheExpediente", "Function ActualizaIndiceDocumentoIntegracionSII");

test("planificador unico asigna cada imagen al expediente explicito", () => {
  assert.match(single, /SolicitaEstructuraTramite\(IdTramite/);
  assert.match(single, /SolicitaListaImagenesGabineteEnlace\(NombreGabinete,[\s\S]*ReciboSII/);
  assert.match(single, /For z As Integer = 0 To StruImageGabineteWorfkflow\.Length - 1/);
  assert.match(single, /IdExpedienteWeb = IdExpediente/);
  assert.match(single, /IdImagen = StruImageGabineteWorfkflow\(z\)\.id_image/);
});

test("planificador multiple recupera cache y separa primario de secundarios", () => {
  assert.match(multiple, /SolicitaCacheCreacionExepdienteSiiRadicado\(ReciboSII/);
  assert.match(multiple, /SolicitaListaTiposExpedienteSegundarioSii\(IdTramite/);
  assert.match(multiple, /EstadoPadre = 1[\s\S]*If Testigo = 0 Then/);
  assert.match(multiple, /EstadoPadre = 2[\s\S]*ID_TIPODOCUMENTO = StruTiposExpedienteSegundarioSII\(k\)\.IdTipo/);
  assert.match(multiple, /CStruSiiCahcheExpediente\.Count > 1[\s\S]*CIncripcionSII\.Count = 1/);
});

test("ambos casos producen el mismo contrato de destino", () => {
  for (const field of ["Gabinete", "IdExpedienteWeb", "IdImagen", "IdFlujoTarea", "Radicado"]) {
    assert.match(single, new RegExp(`ClsssStructureVinculaDocumento\\.${field} =`));
    assert.match(multiple, new RegExp(`ClsssStructureVinculaDocumento\\.${field} =`));
  }
});

test("recuperacion conserva normalizacion ESAL y no deduplica destinos", () => {
  assert.match(indexUpdate, /NombreGabinete\) = "ESAL"[\s\S]*Matricula\.Replace\("S0", ""\)/);
  assert.doesNotMatch(multiple, /Distinct\(|GroupBy\(|Dictionary\(Of Integer|HashSet/);
  assert.match(multiple, /For i As Integer = 0 To CStruSiiCahcheExpediente\.Count - 1[\s\S]*For z As Integer = 0 To StruImageGabineteWorfkflow\.Length - 1/);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const coordinatorSource = fs.readFileSync(path.join(root, "Integracionccv/ClassRaSIICacheActualizaIndice.vb"), "utf8");
const cabinetSource = fs.readFileSync(path.join(root, "Docuarchi/ClassDaGabinete.vb"), "utf8");

function body(source, name, next) {
  const start = source.indexOf(`Function ${name}`);
  const end = source.indexOf(next, start);
  assert.notEqual(start, -1, `${name} debe existir`);
  assert.notEqual(end, -1, `${name} debe tener limite localizable`);
  return source.slice(start, end);
}

const coordinator = body(coordinatorSource, "ActualizaIndiceDocumentosSII", "Function InsertarCacheIndiceSII");
const cacheUpdate = body(cabinetSource, "ActualizaIndiceDocumentoCacheExpediente", "Function ActualizaIndiceDocumentoIntegracionSII");
const integrationUpdate = body(cabinetSource, "ActualizaIndiceDocumentoIntegracionSII", "Function Solicita_Sql_Consulta_lista_documentos_matricualdo");

test("orquestador omite todo el trabajo si existe cache de indice", () => {
  assert.match(coordinator, /SolicitaCacheIndiceSIIRadicado\(ReciboSII/);
  assert.match(coordinator, /If Not CcacheIndiceSII Is Nothing Then[\s\S]*ActualizaIndiceDocumentosSII = "YES"[\s\S]*Exit Function/);
  assert.doesNotMatch(coordinator.slice(0, coordinator.indexOf("If Not CcacheIndiceSII Is Nothing Then")), /UPDATE_COMMAND|XmlDocument|\.Save\(/);
});

test("orquestador usa maximo dos expedientes o la primera inscripcion", () => {
  assert.match(coordinator, /If IcuntCache > 2 Then[\s\S]*IcuntCache = 2/);
  assert.match(coordinator, /For i As Integer = 0 To IcuntCache - 1[\s\S]*ActualizaIndiceDocumentoCacheExpediente/);
  assert.match(coordinator, /CIncripcionSII\(0\)\.MATRICULA_SII[\s\S]*CIncripcionSII\(0\)\.PROPONENTE_SII/);
  assert.match(coordinator, /ActualizaIndiceDocumentoIntegracionSII/);
});

for (const [name, source] of [["cache", cacheUpdate], ["integracion", integrationUpdate]]) {
  test(`${name} actualiza los tres campos para MERCANTIL ESAL y RUP`, () => {
    for (const cabinet of ["MERCANTIL", "ESAL", "RUP"]) {
      assert.match(source, new RegExp(`If NombreGabinete = "${cabinet}" Then`));
      assert.match(source, new RegExp(`Update ${cabinet} set NITCEDULA=`));
    }
    assert.match(source, /RAZONSOCIAL='/);
    assert.match(source, /MATRICULA='/);
    assert.match(source, /UCase\(NombreGabinete\) = "ESAL"[\s\S]*\.Replace\("S0", ""\)/);
  });
}

test("las variantes difieren solo en el alcance fisico del UPDATE", () => {
  assert.match(cacheUpdate, /WHERE ID_EXPEDIENTE=" & CStruSiiCahcheExpediente\.IdExpediente/);
  assert.match(integrationUpdate, /WHERE ENLASE='" & StruSiiCahcheInscripcion\.RadicadoSII & "'/);
  assert.doesNotMatch(cacheUpdate, /WHERE ENLASE=/);
  assert.doesNotMatch(integrationUpdate, /WHERE ID_EXPEDIENTE=/);
});

test("ninguna de las tres funciones verifica ni escribe el XML del indice", () => {
  for (const source of [coordinator, cacheUpdate, integrationUpdate]) {
    assert.doesNotMatch(source, /XmlDocument|Actualiza_archivo_xml_indice_expediente|\.Save\(|ra_cert_indice_expediente/);
  }
  assert.match(coordinator, /InsertarCacheIndiceSII\(CcacheIndiceSII\)/);
  assert.match(coordinator, /ActualizaIndiceDocumentosSII = "YES"/);
});


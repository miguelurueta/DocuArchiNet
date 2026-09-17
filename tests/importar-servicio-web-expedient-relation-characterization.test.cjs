const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const gestion = fs.readFileSync(path.join(root, "Gestion/ClassGaExpediente.vb"), "utf8");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceGaExpediente.asmx.vb"), "utf8");

function functionBody(source, name, nextMarker) {
  const start = source.indexOf(`Function ${name}`);
  const end = source.indexOf(nextMarker, start);
  assert.notEqual(start, -1, `${name} debe existir`);
  assert.notEqual(end, -1, `${name} debe tener limite localizable`);
  return source.slice(start, end);
}

const vincula = functionBody(gestion, "VinculaDocumentoExpediente", "Function Copia_documento_expediente");
const webMethod = functionBody(service, "ServiceVinculaDocumentoExpediente", "<WebMethod(EnableSession:=True)>");

test("una relacion existente retorna YES sin comprobar el expediente solicitado", () => {
  const check = vincula.match(/If id_expediente_relacion <> 0 Then[\s\S]*?End If/);
  assert.ok(check, "debe existir el atajo por relacion previa");
  assert.match(check[0], /VinculaDocumentoExpediente = "YES"[\s\S]*Exit Function/);
  assert.doesNotMatch(check[0], /id_expediente_relacion\s*(?:=|<>)\s*id_expediente/);
  assert.match(vincula, /Solicita_existencia_produccion_documental\(id_imagen,[\s\S]*?id_expediente_relacion/);
});

test("la ruta ausente crea o actualiza produccion y persiste las relaciones SQL", () => {
  assert.match(vincula, /If estado_existencia_produccion = "NO" Then[\s\S]*insert into registro_producion_documental/);
  assert.match(vincula, /Else[\s\S]*UPDATE registro_producion_documental SET/);
  assert.match(vincula, /UPDATE " & gabinete & " set ID_INVENTARIO_DOCUMENTAL=/);
  assert.match(vincula, /insert into\s+ra_rel_copia_wf_produccion/);
  assert.match(vincula, /insert into\s+ra_cert_indice_expediente/);
  assert.match(vincula, /update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=/);
});

test("el indice XML se guarda antes del commit y no existe postcheck fisico", () => {
  const updateIndex = vincula.indexOf("Actualiza_archivo_xml_indice_expediente");
  const saveXml = vincula.indexOf("xmlArchivo.Save(Ruta_archivo_indice_expediente)");
  const commit = vincula.indexOf("myTrans.Commit()", saveXml);
  assert.ok(updateIndex >= 0 && saveXml > updateIndex && commit > saveXml);
  const afterCommit = vincula.slice(commit);
  assert.doesNotMatch(afterCommit, /Solicita_existencia_produccion_documental|ra_cert_indice_expediente|XmlDocument|\.Load\(/);
  assert.match(afterCommit, /VinculaDocumentoExpediente = "YES"/);
});

test("el WebMethod consume solo el primer destino y colapsa YES a una bandera", () => {
  assert.match(webMethod, /DeserializeObject\(Of List\(Of ClsssStructureVinculaDocumento\)\)/);
  for (const field of ["IdExpedienteWeb", "IdImagen", "Gabinete", "Radicado", "IdFlujoTarea"]) {
    assert.match(webMethod, new RegExp(`_ClsssStructureVinculaDocumento\\.Item\\(0\\)\\.${field}`));
  }
  assert.match(webMethod, /If Result <> "YES" Then[\s\S]*Else[\s\S]*id_imagen_copia = 1/);
  assert.doesNotMatch(webMethod, /For Each|For i As|For z As/);
});


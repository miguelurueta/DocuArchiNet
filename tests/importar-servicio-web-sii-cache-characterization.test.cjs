const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const creationSource = fs.readFileSync(path.join(root, "Integracionccv/ClassRaSIiCacheExpediente.vb"), "utf8");
const linkSource = fs.readFileSync(path.join(root, "Integracionccv/ClassRaSiiCacheVinculacion.vb"), "utf8");
const expeditionSource = fs.readFileSync(path.join(root, "Gestion/ClassGaExpediente.vb"), "utf8");
const clientSource = fs.readFileSync(path.join(root, "js/java_general/JSExpediente.js"), "utf8");

function body(source, name, next) {
  const start = source.indexOf(`Function ${name}`);
  const end = source.indexOf(next, start);
  assert.notEqual(start, -1, `${name} debe existir`);
  assert.notEqual(end, -1, `${name} debe tener limite localizable`);
  return source.slice(start, end);
}

const creation = body(creationSource, "RegistraCacheCreacionExpedienteSII", "Function SolicitaCacheCreacionExepdienteSiiRadicado");
const creationLookup = body(creationSource, "SolicitaCacheCreacionExpedienteSII", "End Class");
const link = body(linkSource, "RegistraCahcheVinculacionSII", "Function SolicitaCahcheVinculacionSII");
const linkLookup = body(linkSource, "SolicitaCahcheVinculacionSII", "End Class");
const orchestration = body(expeditionSource, "CreaExpedienteIntegracionSII", "Function Auto_vincula_documentos_a_expediente_estructura");

test("cache de creacion persiste expediente y relacion externa en una transaccion", () => {
  assert.match(creation, /Insert into\s+ra_sii_cache_exepediente/);
  assert.match(creation, /Insert into ra_relacion_radicado_externo_expediente/);
  assert.match(creation, /BeginTransaction\(\)[\s\S]*myCommand\.Transaction = myTrans/);
  assert.match(creation, /myCommand\.CommandText = SQLInsertInto[\s\S]*myCommand\.CommandText = SqlInsertRelacion[\s\S]*myTrans\.Commit\(\)/);
});

test("cache de creacion normaliza matricula pero no aplica precheck ni devuelve su id", () => {
  assert.match(creation, /Matricula = Matricula\.Replace\("S0", ""\)/);
  assert.doesNotMatch(creation, /Select |SELECT |SolicitaCacheCreacionExpedienteSII\(/);
  assert.doesNotMatch(creation, /LastInsertedId|IdraSiiCacheExepediente\s*=/);
  assert.match(creationLookup, /where Matricula='" & MatriculaSII & "' and NombreGabinete='" & Gabinete & "'/);
  assert.match(creationLookup, /Rows\(0\)/);
});

test("cache de vinculacion no identifica tarea ni imagen y tampoco evita duplicados", () => {
  assert.match(link, /Insert into\s+ra_sii_cache_vinculacion \(RadicadoSII,CodigoBarras,NombreGabinete,[\s\S]*Matricula,IdExpediente,FechaRegistroCache\)/);
  assert.doesNotMatch(link, /IdTarea|id_tarea|IdImagen|id_imagen|Select |SELECT /);
  assert.match(link, /SELECTION_LAST_INSERT_COMMAND\(SQLInsertInto, IdraSiiCacheVicnculacion\)/);
  assert.match(linkLookup, /where RadicadoSII='" & RadicadoSII & "'/);
  assert.match(linkLookup, /Rows\(0\)/);
});

test("creacion fisica precede a su cache y deja una ventana de resultado parcial", () => {
  const creates = orchestration.indexOf("AutoRegistraExpedienteTramite");
  const caches = orchestration.indexOf("RegistraCacheCreacionExpedienteSII", creates);
  assert.ok(creates >= 0 && caches > creates);
  assert.match(orchestration.slice(creates, caches), /If Result <> "YES" Then[\s\S]*Exit Function/);
  assert.match(orchestration.slice(caches), /If Result <> "YES" Then[\s\S]*Exit Function/);
});

test("cliente registra cache de vinculacion despues del efecto fisico", () => {
  const progress = clientSource.indexOf("Result = await JSProgresBarBoot(_OPtionProgresBar);");
  const cache = clientSource.indexOf("Result = await ServiceRESTregistraCahcheVinculacionSII", progress);
  assert.ok(progress >= 0 && cache > progress);
  assert.match(clientSource, /ServiceRESTsolicitaCahcheVinculacionSII[\s\S]*RadicadoSII == null[\s\S]*JSProgresBarBoot/);
  assert.doesNotMatch(linkLookup, /ra_rel_copia_wf_produccion|registro_producion_documental|ra_cert_indice_expediente/);
});


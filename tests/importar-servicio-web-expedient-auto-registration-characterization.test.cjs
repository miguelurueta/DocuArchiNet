const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const source = fs.readFileSync(path.join(root, "Gestion/ClassGaExpediente.vb"), "utf8");
const endpoint = fs.readFileSync(path.join(root, "webservice/WebServiceGaExpediente.asmx.vb"), "utf8");

function between(text, startToken, endToken) {
  const start = text.indexOf(startToken);
  assert.notEqual(start, -1, `${startToken} no fue localizado`);
  const end = text.indexOf(endToken, start + startToken.length);
  assert.notEqual(end, -1, `${endToken} no fue localizado`);
  return text.slice(start, end);
}

const autoRegister = between(
  source,
  "Function AutoRegistraExpedienteTramite",
  "Function Valida_existencia_expediente_auto_registro",
);
const identityCheck = between(
  source,
  "Function Valida_existencia_expediente_auto_registro",
  "Function Auto_vincula_documentos_a_expediente",
);
const physicalCreate = between(
  source,
  "Function Registrar_Expediente_Conservacion",
  "Public Structure stru_values_cambio_indice",
);
const siiCoordinator = between(
  source,
  "Function CreaExpedienteIntegracionSII",
  "Function Auto_vincula_documentos_a_expediente_estructura",
);

test("auto registro exige configuracion campos unicos y contexto de tarea", () => {
  for (const dependency of [
    "SolicitaidAutoRegistroExpediente",
    "SolicitaDatosAutoRegistro",
    "SolicitaDatosGestionCamposAutoRegistro",
    "SolicitaCamposUnicosAutoRegistroExpediente",
    "SolicitaDatosFuncionAutoRegistro",
    "SolicitaNombreGabineteImagenTareaWorkflow",
  ]) assert.match(autoRegister, new RegExp(dependency), dependency);
  assert.match(autoRegister, /estado_obligatorio = 1[\s\S]*debe ser informado/);
  assert.match(identityCheck, /estado_unico = 1/);
  assert.match(identityCheck, /Select ID_EXPEDIENTE from expediente_archivo/);
  assert.match(identityCheck, /no registra campos unicos de comparación/);
});

test("creacion fisica registra SQL XML contadores y relaciones dentro de su frontera legacy", () => {
  assert.match(physicalCreate, /BeginTransaction\(\)/);
  assert.match(physicalCreate, /Insert into expediente_archivo/);
  assert.match(physicalCreate, /LastInsertedId/);
  assert.match(physicalCreate, /Crea_archivo_indice_xml_expediente/);
  assert.match(physicalCreate, /UPDATE ra_consecutivo_expediente_archivo/);
  assert.match(physicalCreate, /insert into ra_pro_relacion_exp_produccion/);
  assert.match(physicalCreate, /Update ra_pro_niveles set conta_expediente/);
  assert.match(physicalCreate, /Insert into ra_pro_niveles_has_expediente_archivo/);
  assert.match(physicalCreate, /myTrans\.Commit\(\)/);
});

test("SII bloquea creacion sin documentos previos y cubre unico primario y secundarios", () => {
  assert.match(siiCoordinator, /SolicitaListaImagenesGabineteEnlace/);
  assert.match(siiCoordinator, /StruImageGabineteWorfkflow Is Nothing[\s\S]*No fue posible encontrar documentos/);
  assert.match(siiCoordinator, /util_Estado_Multiple_expedienteSII = 1/);
  assert.match(siiCoordinator, /MatriculaPropietario = ""[\s\S]*AutoRegistraExpedienteTramite/);
  assert.match(siiCoordinator, /MatriculaPropietario <> ""[\s\S]*AutoRegistraExpedienteTramite/);
  assert.match(siiCoordinator, /util_Estado_Multiple_expedienteSII <> 1[\s\S]*AutoRegistraExpedienteTramite/);
});

test("retorno YES no aporta postcheck y una respuesta perdida oculta el id creado", () => {
  const afterCommit = physicalCreate.slice(physicalCreate.indexOf("myTrans.Commit()"));
  assert.doesNotMatch(afterCommit, /Select ID_EXPEDIENTE|Valida_existencia_expediente_auto_registro/);
  assert.match(autoRegister, /Registrar_Expediente_Conservacion\([\s\S]*If Result <> "YES"/);
  assert.match(endpoint, /Catch ex As Exception[\s\S]*parameter_gestion\.id_expediente = 0/);
});

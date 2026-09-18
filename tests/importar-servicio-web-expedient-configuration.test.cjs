const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb", "utf8");
const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

test("configuracion se resuelve solo por tramite confiable", () => {
  assert.match(repository, /contexto\.IdTramite <= 0/);
  assert.match(repository, /id_Tipo_Doc_Entrante=@procedureId/);
  assert.match(repository, /P\("@procedureId", contexto\.IdTramite\)/);
  assert.doesNotMatch(repository, /HttpContext|Session|RequestDto|CIncripcionSII/);
});

test("consulta parametrizada materializa banderas y gabinete", () => {
  for (const column of ["nombre_gabinete_workflow", "ra_auto_registro_expediente_id_auto_registro", "util_Estado_Crea_ExpedienteSII", "util_Estado_Multiple_expedienteSII"]) {
    assert.match(repository, new RegExp(column));
  }
  assert.match(repository, /CreacionAutomaticaHabilitada = header\.CreateEnabled/);
  assert.match(repository, /MultiplesExpedientes = header\.MultipleEnabled/);
  assert.match(repository, /headers\.Count <> 1/);
  assert.match(repository, /_docuarchiConnections\.CreateOpenConnection/);
  assert.match(repository, /_radicacionConnections\.CreateOpenConnection/);
  assert.match(repository, /EXPEDIENT_CONFIGURATION_HEADER_QUERY_FAILED/);
  assert.match(repository, /EXPEDIENT_CONFIGURATION_IDENTITY_FIELDS_QUERY_FAILED/);
  assert.match(repository, /EXPEDIENT_CONFIGURATION_SECONDARY_TYPES_QUERY_FAILED/);
});

test("campos de identidad incluyen exclusivamente estado_unico", () => {
  assert.match(repository, /ra_auto_registro_expediente_id_auto_registro=@autoRegistrationId AND estado_unico=1/);
  assert.match(repository, /P\("@autoRegistrationId", header\.AutoRegistrationId\)/);
  assert.match(repository, /SafeIdentifier\(field\.NombreCampo\)/);
  assert.match(repository, /fields\.Count = 0 Then Return Nothing/);
  assert.match(models, /Class CampoIdentidadExpedienteImportacion[\s\S]*NombreCampo[\s\S]*Obligatorio/);
});

test("modo multiple carga tipologias secundarias sin concatenar el tramite", () => {
  assert.match(repository, /UtilExpedienteRelacionado=1/);
  assert.match(repository, /SELECT DISTINCT tipo_doc_series_Id_Tipo_Doc_Series/);
  assert.doesNotMatch(repository, /IdTramite\s*&|procedureId\s*&/);
});

test("repositorio moderno esta registrado una sola vez y legacy permanece intacto", () => {
  const include = 'Compile Include="Infrastructure\\Repositories\\Workflow\\ImportarServicioWeb\\MySqlImportExpedientConfigurationRepository.vb"';
  assert.equal(project.split(include).length - 1, 1);
  assert.doesNotMatch(repository, /SolicitaEstructuraTramite|SolicitaCamposUnicosAutoRegistroExpediente|SolicitaListaTiposExpedienteSegundarioSii/);
});

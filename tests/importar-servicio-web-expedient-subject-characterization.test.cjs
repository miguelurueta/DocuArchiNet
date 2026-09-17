const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const legacySource = fs.readFileSync(
  path.join(root, "Integracionccv/ClassConsultaExpedienteSII.vb"),
  "utf8",
);
const modernSteps = fs.readFileSync(
  path.join(root, "Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb"),
  "utf8",
);
const modernEndpoint = fs.readFileSync(
  path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"),
  "utf8",
);
const modernResolver = fs.readFileSync(
  path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiExpedientSubjectResolver.vb"),
  "utf8",
);

function vbFunctionBody(source, name, nextName) {
  const start = source.indexOf(`Function ${name}`);
  assert.notEqual(start, -1, `${name} no fue localizada`);
  const end = source.indexOf(`Function ${nextName}`, start);
  assert.notEqual(end, -1, `${nextName} no fue localizada`);
  return source.slice(start, end);
}

const subjectResolver = vbFunctionBody(
  legacySource,
  "SolicitaEstructuraExpedienteSII",
  "ConsultaEexpedienteProponente",
);

test("caracteriza entradas explicitas y ausencia de sesion en el resolvedor legacy", () => {
  assert.match(subjectResolver, /ByVal Matricula As String/);
  assert.match(subjectResolver, /ByVal Proponente As String/);
  assert.match(subjectResolver, /ByVal Gabinete As String/);
  assert.match(subjectResolver, /ByRef StruSiiCahcheInscripcion As StruSiiCahcheInscripcion/);
  assert.doesNotMatch(subjectResolver, /HttpContext|Session(?:\.Item)?/);
});

test("caracteriza ramas y datos devueltos para MERCANTIL ESAL y RUP", () => {
  assert.match(subjectResolver, /Case "MERCANTIL"[\s\S]*ConsultaExpedienteMercantilEsal\(Matricula/);
  assert.match(subjectResolver, /Case "ESAL"[\s\S]*Matricula = Matricula\.Replace\("9000", ""\)[\s\S]*"S0" & Matricula/);
  assert.match(subjectResolver, /Case "RUP"[\s\S]*ConsultaEexpedienteProponente\(Val\(Proponente\)/);
  for (const field of [
    "NitIdentificacion",
    "Rsocial",
    "Matricula",
    "NombrePropietario",
    "Identificacionpro",
    "MatriculaPropietario",
  ]) {
    assert.match(subjectResolver, new RegExp(`StruSiiCahcheInscripcion\\.${field} =`), field);
  }
  assert.match(subjectResolver, /Case Else[\s\S]*No se pudo homologar el gabinete/);
});

test("el borde moderno detecta contexto de tarea discrepante y el resolvedor usa contexto derivado en servidor", () => {
  assert.match(modernEndpoint, /request\.TaskId <> trustedTaskId[\s\S]*TASK_CONTEXT_MISMATCH/);
  assert.match(
    modernSteps,
    /SolicitaEstructuraExpedienteSII\([\s\S]*item\.MetadatosSii\.Matricula[\s\S]*item\.MetadatosSii\.Proponente[\s\S]*metadata\.NombreGabinete/,
  );
  assert.doesNotMatch(subjectResolver, /ID_TAREA_SELECCIONDA|WF_RUTAWORKFLOW|Id_Ruta_Workflow/);
});

test("el adaptador moderno materializa la identidad con la funcion legacy preservada y datos SII autoritativos", () => {
  assert.match(modernResolver, /SolicitaDatosAutoRegistro\(configuracion\.IdAutoRegistro/);
  assert.match(modernResolver, /SolicitaDatosFuncionAutoRegistro\(dataFunction, authoritative/);
  assert.match(modernResolver, /\.MATRICULA_SII = inscripcion\.Matricula/);
  assert.match(modernResolver, /\.NIT_SII = inscripcion\.IdentificacionSujeto/);
  assert.match(modernResolver, /\.RSOCIAL_SII = inscripcion\.RazonSocial/);
  assert.match(modernResolver, /configuracion\.CamposIdentidad\(index\)\.Valor =/);
  assert.match(modernResolver, /SolicitaDatosGestionCamposAutoRegistro\(/);
  assert.match(modernResolver, /Case "CODIGO_SERIE_TRD"\s*:\s*fields\(index\)\.valor_campo_expediente = seriesId/);
  assert.match(modernResolver, /Case "CODIGO_AREA_TRD"/);
  assert.match(modernResolver, /Case "CODIGO_SUB_SERIE_TRD"/);
  assert.match(modernResolver, /Case "ID_FONDO"/);
  assert.match(modernResolver, /Case "id_instrumento"/);
  assert.doesNotMatch(modernResolver, /HttpContext|Session(?:\.Item)?/);
});

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");
const interfaces = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb", "utf8");

test("modelo conserva agregado de inscripcion y plan logico", () => {
  for (const symbol of ["InscripcionImportacion", "DestinoLogicoExpedienteImportacion", "PlanExpedienteImportacion", "RolExpedienteImportacion"]) {
    assert.match(models, new RegExp(`(?:Class|Enum) ${symbol}`));
  }
  assert.match(models, /Class PlanExpedienteImportacion[\s\S]*Property Codigo As String/);
  for (const field of ["ClaveInscripcion", "Libro", "Registro", "Matricula", "Proponente", "IdentificacionSujeto", "RazonSocial", "MatriculaPropietario", "IdExpediente", "RolExpediente", "ClientItemIds"]) {
    assert.match(models, new RegExp(`Property ${field} As`));
  }
});

test("documento relacionado representa destino y cada postcondicion", () => {
  for (const field of ["IdTarea", "IdImagen", "NombreGabinete", "RadicadoSii", "IdExpedienteEsperado", "EstadoDestino", "EstadoRelacion", "EstadoCache", "EstadoIndiceGabinete", "EstadoIndiceSql", "EstadoIndiceXml", "EstadoReconciliacion"]) {
    assert.match(models, new RegExp(`Property ${field} As`));
  }
  for (const state of ["Pendiente", "Confirmado", "Ausente", "Conflicto", "ResultadoIncierto", "Fallido"]) {
    assert.match(models, new RegExp(`\\n    ${state}\\r?\\n`));
  }
});

test("cache e indice SQL XML tienen contratos separados", () => {
  assert.match(models, /Class EntradaCacheVinculoDocumentoImportacion/);
  assert.match(models, /Class EvidenciaIndiceElectronicoImportacion/);
  assert.match(models, /Property SqlConfirmado As Boolean/);
  assert.match(models, /Property XmlConfirmado As Boolean/);
  assert.match(interfaces, /Interface IImportDocumentLinkCacheRepository/);
  assert.match(interfaces, /Interface IImportDocumentIndexUpdater/);
  assert.match(interfaces, /Interface IImportElectronicIndexVerifier/);
});

test("todos los puertos de expediente reciben contexto confiable", () => {
  const ports = interfaces.slice(interfaces.indexOf("Public Interface IImportExpedientConfigurationRepository"));
  const functions = ports.match(/Function [\s\S]*?(?=\n    Function |\nEnd Interface)/g) || [];
  assert.ok(functions.length >= 12);
  for (const signature of functions) {
    assert.match(signature, /contexto As ContextoImportacionServicio/);
  }
  assert.doesNotMatch(ports, /HttpContext|Session|RequestDto|ResponseDto|ClsssStructureVinculaDocumento|CStruSii/);
});

test("resultado seguro no expone detalles fisicos o excepciones", () => {
  const start = models.indexOf("Class ResultadoEfectoExpedienteImportacion");
  const end = models.indexOf("Public Class ContextoImportacionServicio", start);
  const result = models.slice(start, end);
  assert.match(result, /Estado As EstadoEfectoExpedienteImportacion/);
  assert.match(result, /Codigo As String/);
  assert.match(result, /MensajeVisible As String/);
  assert.doesNotMatch(result, /Sql|Ruta|Exception|StackTrace|Session/i);
});

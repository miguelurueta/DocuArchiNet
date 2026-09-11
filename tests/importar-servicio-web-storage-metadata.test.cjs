const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const dto = fs.readFileSync(path.join(root, "DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb"), "utf8");
const service = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb"), "utf8");
const repository = fs.readFileSync(path.join(root, "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportStorageMetadataRepository.vb"), "utf8");
const documentTypes = fs.readFileSync(path.join(root, "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentTypeResolver.vb"), "utf8");
const composition = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const persistence = fs.readFileSync(path.join(root, "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb"), "utf8");
const steps = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb"), "utf8");
const adapter = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb"), "utf8");

test("el llamador aporta solo radicado y no metadatos de infraestructura", () => {
  const start = dto.indexOf("Public Class CreateImportIntentRequestDto");
  const body = dto.slice(start, dto.indexOf("End Class", start));
  assert.match(body, /Property Radicado As String/);
  assert.doesNotMatch(body, /NombreGabinete|NombreRutaWorkflow|NombreClaseFormatoDocumento|RutaArchivo/);
});

test("radicado es obligatorio, participa en idempotencia y se persiste parametrizado", () => {
  assert.match(service, /String\.IsNullOrWhiteSpace\(request\.Radicado\)/);
  assert.match(service, /Field\(intent\.ContextoOriginal\.Radicado\)/);
  assert.match(persistence, /radicado[^"]*@radicado/);
  assert.match(persistence, /P\("@radicado",c\.Radicado\)/);
});

test("servidor deriva ruta, gabinete y clase documental", () => {
  assert.match(repository, /SELECT Nombre_Ruta FROM rutas_workflow WHERE ID_RUTA=@idRuta/);
  assert.match(repository, /SafeIdentifier\(routeName\)/);
  assert.match(repository, /configuracion_gabinete/);
  assert.match(repository, /DOCUMENTO ELECTRONICO/);
  assert.doesNotMatch(repository, /ra_dig_tipos_docum_lista_chequeo/);
  assert.match(documentTypes, /FROM ra_dig_tipos_docum_lista_chequeo rdt/);
  assert.match(documentTypes, /tipo_doc_entrante_id_Tipo_Doc_Entrante=@procedureId/);
  assert.match(documentTypes, /tipo_doc_series_Id_Tipo_Doc_Series=@documentTypeId/);
  assert.match(documentTypes, /LIMIT 2/);
  assert.match(documentTypes, /DOCUMENT_TYPE_NOT_ALLOWED_FOR_PROCEDURE/);
  assert.match(documentTypes, /DOCUMENT_TYPE_MAPPING_AMBIGUOUS/);
  assert.match(documentTypes, /DOCUMENT_TYPE_NAME_MISMATCH/);
  assert.match(steps, /IdTipoListaChequeo = documentType\.IdTipoListaChequeo/);
  assert.match(steps, /DescripcionTipo = documentType\.NombreTipoDocumental/);
  assert.doesNotMatch(steps, /IdTipoListaChequeo = If\(item\.IdTipoDocumental/);
  assert.doesNotMatch(steps, /DescripcionTipo = If\(item\.NombreArchivo/);
  assert.match(persistence, /document_type_name/);
  assert.match(persistence, /NombreTipoDocumental/);
  assert.doesNotMatch(repository, /HttpContext|Session\.Item/);
  assert.doesNotMatch(documentTypes, /HttpContext|Session\.Item/);
  assert.match(composition, /New RadicacionModuleConnectionFactory\(session\.CadenaConexionRadicacion\)/);
});

test("matriz SII concuerda con mercantil, rup y esal del almacenamiento historico", () => {
  for (const field of ["CODBARRAS", "ENLASE", "MATRICULA", "RAZONSOCIAL", "NITCEDULA", "LIBRO", "INSCRIPCION", "RECIBOCAJA", "FECHAINSCRIP", "FECHAREGISTR", "ACTO", "DESCRIPCIONA", "DESCRIACTO"]) {
    assert.match(steps, new RegExp(`"${field}"`), field);
  }
  assert.match(steps, /String\.Equals\(cabinet, "RUP"[\s\S]*sii\.Proponente/);
  assert.match(steps, /SolicitaReciboCodigoBarrasSII/);
  assert.match(steps, /String\.IsNullOrWhiteSpace\(item\.MetadatosSii\.RazonSocial\)[\s\S]*SolicitaEstructuraExpedienteSII/);
  assert.match(steps, /SII_SUBJECT_METADATA_UNAVAILABLE/);
  assert.match(steps, /item\.MetadatosSii\.RazonSocial = subject\.Rsocial/);
  assert.match(steps, /item\.MetadatosSii\.NitCedula = subject\.NitIdentificacion/);
  assert.match(steps, /command\.Radicado = receipt/);
  assert.match(steps, /BuildSiiFields\(metadata\.NombreGabinete, taskBarcode, receipt/);
  assert.match(steps, /String\.IsNullOrWhiteSpace\(taskBarcode\)/);
  assert.match(steps, /isMercantil[\s\S]*"DESCRIACTO", "DESCRIPCIONA"/);
  assert.match(steps, /TryParseExact[\s\S]*"yyyyMMdd"[\s\S]*"yyyy-MM-dd"/);
  assert.match(adapter, /NombreCampoGabinete = campo\.Nombre/);
  assert.match(adapter, /ValorCampoGabinete = If\(campo\.Valor/);
});

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const models = fs.readFileSync('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb', 'utf8');
const dtos = fs.readFileSync('DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb', 'utf8');

const block = (source, start, end) => source.slice(source.indexOf(start), source.indexOf(end, source.indexOf(start)));

test('la intención conserva capacidad como parte del contexto inmutable', () => {
  const context = block(models, 'Public NotInheritable Class ContextoIntencionImportacion', 'Public NotInheritable Class MetadatosAlmacenamientoImportacion');
  assert.match(context, /Public Property ProviderId As String/);
  assert.match(context, /Public Property Capability As String/);
  assert.match(context, /Public Property Radicado As String/);
});

test('el resultado interno representa evidencia física y recuperación explícitas', () => {
  const snapshot = block(models, 'Public Class SnapshotItemReconciliacionImportacion', 'Public Class ResultadoFaseImportacion');
  const phase = block(models, 'Public Class ResultadoFaseImportacion', 'Public Class TransicionImportacion');
  for (const source of [snapshot, phase]) {
    assert.match(source, /Public Property EvidenciaFisicaConfirmada As Boolean/);
    assert.match(source, /Public Property Recuperable As Boolean/);
  }
});

test('el contrato público agrega evidencia sin retirar campos 1.0', () => {
  const result = block(dtos, '<Serializable()> Public Class ImportItemResultDto', '<Serializable()> Public Class PreflightImportResponseDto');
  for (const legacyField of ['ExternalKey', 'Status', 'DocumentId', 'PersistenceKnown', 'Retryable', 'TaskId']) {
    assert.match(result, new RegExp(`Public Property ${legacyField} As`));
  }
  assert.match(result, /Public Property EvidenceStatus As String/);
  assert.match(result, /Public Property RecoveryAllowed As Boolean/);
});
test('el catálogo solo predetermina una tipología autorizada inequívoca', () => {
  const catalog = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentTypeCatalogRepository.vb', 'utf8');
  const presentation = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemPresentationService.vb', 'utf8');
  const documentType = block(dtos, '<Serializable()> Public Class ImportDocumentTypeDto', '<Serializable()> Public Class ImportItemMetadataDto');
  assert.match(catalog, /If result\.Count = 1 Then result\(0\)\.Predeterminado = True/);
  assert.doesNotMatch(catalog, /Contains\("CONSTANCIA"|Contains\("INSCRIPCION"/i);
  assert.match(documentType, /Public Property IsDefault As Boolean/);
  assert.match(presentation, /\.IsDefault=item\.Predeterminado/);
});
test('preflight e intención enlazan capacidad ENLASE con la huella', () => {
  const preflight = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb', 'utf8');
  const intent = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb', 'utf8');
  assert.match(preflight, /CAPABILITY_NOT_SUPPORTED/);
  assert.match(preflight, /Fingerprint\(contexto, request\.Capability, request\.Items, configuration\)/);
  assert.match(preflight, /If\(capability, String\.Empty\)\.Trim\(\)\.ToUpperInvariant\(\)/);
  assert.match(intent, /\.Capability = contexto\.Capability, \.Items = request\.Items/);
  assert.match(intent, /\.Capability = context\.Capability/);
  assert.match(intent, /Field\(intent\.ContextoOriginal\.Capability\.ToUpperInvariant\(\)\)/);
});
test('repositorio persiste capacidad e identidad externa sin alterar unicidad del lote', () => {
  const repo = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb', 'utf8');
  const migration = fs.readFileSync('Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-81-preparacion-persistencia-reconciliacion/Sql/001-add-intent-capability.sql', 'utf8');
  assert.match(repo, /provider_id,capability,radicado/);
  assert.match(repo, /@providerId,@capability,@radicado/);
  assert.match(repo, /\.Capability=Convert\.ToString\(reader\("capability"\)\)/);
  assert.match(repo, /P\("@capability",c\.Capability\)/);
  assert.match(repo, /provider_id,external_key,target_task_id,document_type_id,document_type_name/);
  assert.match(migration, /information_schema\.COLUMNS/);
  assert.match(migration, /ADD COLUMN capability VARCHAR\(80\) NOT NULL DEFAULT ''/);
});

test('idempotencia ENLASE usa identidad opaca y rechaza URL temporal', () => {
  const intent = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb', 'utf8');
  assert.match(intent, /Field\(value\.IdentidadExterna\.ExternalKey\)/);
  assert.match(intent, /IsHttpLocation\(item\.ExternalKey\)/);
  assert.match(intent, /Uri\.UriSchemeHttp/);
  assert.doesNotMatch(intent, /Field\([^\r\n]*Url/i);
});

test('ejecución adquiere guard y revalida dentro del lock antes de efectos', () => {
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const execute = block(orchestrator, 'Public Function Execute', 'Private Shared Function AllItemsStored');
  const lock = execute.indexOf('_guard.Adquirir(contexto, "execute|"');
  const reload = execute.indexOf('_repository.Obtener(contexto, request.IntentId)');
  const context = execute.indexOf('_validator.Validar(contexto, intent.ContextoOriginal)');
  const capability = execute.indexOf('intent.ContextoOriginal.Capability');
  const effect = execute.indexOf('For Each stepItem In _steps');
  assert.ok(lock >= 0 && lock < reload && reload < context && context < capability && capability < effect);
  assert.match(execute, /Using leaseToDispose As IDisposable = executionLease[\s\S]*End Using/);
  assert.match(web, /executionRelatedCoordinator, New MySqlImportIntentConcurrencyGuard\(connections, executor\)/);
});
test('contexto servidor y validador fijan capacidad antes y dentro del lock', () => {
  const models = fs.readFileSync('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb', 'utf8');
  const validator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ValidadorContextoImportacion.vb', 'utf8');
  const preflight = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb', 'utf8');
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  assert.match(models, /Optional ByVal capability As String = Nothing/);
  assert.match(web, /result\.Contexto\.NombreRutaWorkflow, request\.Capability\)/);
  assert.match(preflight, /CAPABILITY_CONTEXT_MISMATCH/);
  assert.match(validator, /contexto\.Capability, persistido\.Capability/);
});
test('la referencia SII para descargar anexos se reconstruye en servidor y queda en el contexto inmutable', () => {
  const endpoint = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const intent = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb', 'utf8');
  const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb', 'utf8');
  const request = block(dtos, '<Serializable()> Public Class CreateImportIntentRequestDto', '<Serializable()> Public Class CreateImportIntentResponseDto');
  assert.match(request, /Public Property ProviderReference As String/);
  assert.match(endpoint, /request\.ProviderReference = trustedBarcode[\s\S]*\.Intents\.Crear\(context, request\)/);
  assert.match(intent, /\.ProviderReference = If\(request\.ProviderReference, String\.Empty\)\.Trim\(\)/);
  assert.match(intent, /Field\(intent\.ContextoOriginal\.ProviderReference\)/);
  assert.match(repository, /provider_reference/);
  assert.match(repository, /@providerReference/);
});

test('descarga ENLASE reconsulta el anexo opaco y aplica el transporte seguro antes de persistir', () => {
  const provider = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb', 'utf8');
  const client = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb', 'utf8');
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  const annexDownload = block(client, 'Public Async Function DownloadAnnexAsync', 'Private Function DownloadSelectedAsync');
  assert.match(provider, /If String\.Equals\(capability, AnnexesEnlaseCapability[\s\S]*DownloadAnnexAsync/);
  assert.match(annexDownload, /ValidateAnnexContext\(providerReference, correlationId\)/);
  assert.match(annexDownload, /QueryRadicadoAsync\(barcode/);
  assert.match(annexDownload, /SiiEnlaseAnnexContractMapper\.Resolve\(source, externalKey\.Trim\(\)\)/);
  assert.match(annexDownload, /DownloadSelectedAsync\(resolved/);
  assert.match(annexDownload, /_resolvedMetadata\(CacheKey\(externalKey, correlationId\)\) = resolved\.Metadata/);
  assert.match(steps, /ContextoOriginal\.ProviderReference[\s\S]*ContextoOriginal\.Capability\)\)\.GetAwaiter\(\)\.GetResult\(\)/);
});
test('adaptador ENLASE invoca una vez la función legacy con el archivo moderno preparado', () => {
  const adapter = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb', 'utf8');
  const legacy = fs.readFileSync('workflow/ClassAlmacenamiento.vb', 'utf8');
  const project = fs.readFileSync('GestionDocumental-Docuarchi.net.vbproj', 'utf8');
  assert.equal((adapter.match(/\.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII\(/g) || []).length, 1);
  assert.doesNotMatch(adapter, /WebService_integracion_sii|HttpClient|DownloadFileViaRestAPI|AlmacenaDocumentoTareaWorkflow/);
  assert.match(adapter, /ContextoSesionCoincide\(comando\)/);
  assert.match(adapter, /ID_TAREA_SELECCIONDA_ENLACE/);
  assert.match(adapter, /comando\.RutaArchivo\)/);
  assert.match(legacy, /Optional ByVal RutaArchivoPreparada As String = Nothing/);
  assert.match(legacy, /If Not String\.IsNullOrWhiteSpace\(RutaArchivoPreparada\)[\s\S]*File\.Exists\(ArchivoDonwload\)[\s\S]*Else[\s\S]*DownloadFileViaRestAPI/);
  assert.match(project, /LegacyEnlaseImportDocumentStorageAdapter\.vb/);
});
test('mapping legacy ENLASE conserva ID como evidencia pendiente y sanea salidas', () => {
  const adapter = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb', 'utf8');
  const yes = block(adapter, 'If String.Equals(respuesta, "YES"', 'End If');
  assert.match(yes, /\.Exitoso = True/);
  assert.match(yes, /\.PersistenciaConocida = False/);
  assert.match(yes, /\.IdDocumento = idImagen/);
  assert.match(yes, /\.EvidenciaFisicaConfirmada = False/);
  assert.match(yes, /\.Reintentable = False/);
  assert.match(adapter, /Private Shared Function MapearRechazo/);
  assert.match(adapter, /ENLASE_DOCUMENT_TYPE_REQUIRED/);
  assert.match(adapter, /ENLASE_STORAGE_FILE_UNAVAILABLE/);
  assert.match(adapter, /ENLASE_STORAGE_UNCERTAIN/);
  assert.match(adapter, /se requiere reconciliación/);
  assert.doesNotMatch(adapter, /MensajeVisible\s*=\s*respuesta|MensajeVisible\s*=\s*ex\.Message/);
  assert.match(adapter, /Dim idImagen As Integer = 0[\s\S]*Catch[\s\S]*Return FalloIncierto\(idImagen\)/);
  assert.match(adapter, /If idDocumento > 0 Then Return FalloIncierto\(idDocumento\)/);
  assert.match(adapter, /\.IdDocumento = If\(idDocumento > 0/);
});
test('composición ENLASE usa almacenamiento propio y excluye efectos de constancias', () => {
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const intent = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb', 'utf8');
  const config = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/EnlaseImportEffectConfigurationRepository.vb', 'utf8');
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  const compose = block(web, 'Private Shared Function Compose', 'Private Shared Function CreatePresentation');
  assert.match(compose, /isEnlase = String\.Equals\(trustedContext\.Capability/);
  assert.match(compose, /If\(isEnlase,[\s\S]*New EnlaseImportEffectConfigurationRepository/);
  assert.match(compose, /If\(isEnlase,[\s\S]*New LegacyEnlaseImportDocumentStorageAdapter/);
  assert.match(compose, /executionExpedientCoordinator As ImportExpedientCoordinator = If\(isEnlase, Nothing, expedientCoordinator\)/);
  assert.match(compose, /executionRelatedCoordinator As ImportRelatedDocumentCoordinator = If\(isEnlase, Nothing, relatedCoordinator\)/);
  assert.match(compose, /If isEnlase Then[\s\S]*VerifyStoredImportExecutionStep[\s\S]*FaseImportacionServicio\.Completada[\s\S]*Else[\s\S]*FaseImportacionServicio\.CacheActualizado/);
  assert.match(config, /ExpedientMode = ModoExpedienteImportacion\.SinExpediente/);
  assert.match(config, /AutomaticCreationEnabled = False/);
  assert.match(intent, /Not String\.Equals\(context\.Capability, SiiImportProvider\.AnnexesEnlaseCapability[\s\S]*_inscriptions\.Resolver/);
  assert.match(intent, /EstadoRelacion = If\(isEnlase, EstadoEfectoExpedienteImportacion\.NoAplica/);
  assert.doesNotMatch(compose, /AsignarTarea|CerrarTarea|Terminar_Tarea_Workflow/i);
  assert.match(orchestrator, /\.DocumentId = If\(confirmed, item\.IdDocumento, Nothing\)/);
  assert.match(orchestrator, /PendingVerification/);
});
test('reconciliación correlaciona contexto, capacidad, identidad externa y relación autorizada', () => {
  const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb', 'utf8');
  assert.match(repository, /intent\.provider_id=@contextProvider AND intent\.capability=@capability/);
  assert.match(repository, /P\("@contextProvider", contexto\.ProviderId\)/);
  assert.match(repository, /P\("@capability", If\(contexto\.Capability, String\.Empty\)\.Trim\(\)\)/);
  assert.match(repository, /item\.provider_id=@providerId AND item\.external_key=@externalKey/);
  assert.match(repository, /FROM logdocuarchi WHERE id_tran=@documentId[\s\S]*ID_TAREA_WF=@taskId/);
  assert.match(repository, /\.Capability=Convert\.ToString\(reader\("capability"\)\)/);
  assert.match(repository, /\.ProviderReference=Convert\.ToString\(reader\("provider_reference"\)\)/);
});
test('confirmación ENLASE exige relación lógica única y existencia física explícita', () => {
  const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb', 'utf8');
  const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
  const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
  assert.match(repository, /SELECT ID_TAREA_WF,GABINETE FROM logdocuarchi/);
  assert.match(repository, /DocumentoFisicoExiste\(connection, relation\.CabinetName, item\.IdDocumento\.Value\)/);
  assert.match(repository, /Regex\.IsMatch\(value, "\^\[A-Za-z\]\[A-Za-z0-9_\]\{0,63\}\$"\)/);
  assert.match(repository, /SELECT ID FROM `" & cabinetName\.Trim\(\) & "` WHERE ID=@physicalDocumentId LIMIT 2/);
  assert.match(repository, /Return count = 1/);
  const classify = block(mapper, 'Private Shared Function Classify', 'Private Shared Function VisibleStatus');
  const physical = classify.indexOf('Not item.EvidenciaFisicaConfirmada');
  const confirmed = classify.indexOf('ConsistenciaDocumentoImportacion.Confirmado');
  assert.ok(physical >= 0 && physical < confirmed);
  assert.match(service, /requiresExpedientEvidence[\s\S]*AnnexesEnlaseCapability[\s\S]*If requiresExpedientEvidence Then ApplyExpedientEvidence/);
});

test('registro lógico sin archivo se proyecta recuperable sin exponer documento', () => {
  const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
  assert.match(mapper, /Case ConsistenciaDocumentoImportacion\.RecursoFisicoAusente/);
  assert.match(mapper, /dto\.Status="Recuperable"/);
  assert.match(mapper, /dto\.EvidenceStatus="Missing"/);
  assert.match(mapper, /dto\.RecoveryAllowed=True/);
  assert.match(mapper, /If dto\.Status<>"Disponible" Then dto\.DocumentId=Nothing/);
});
test('ejecución ENLASE verifica evidencia antes de completar y no reintenta incertidumbre', () => {
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  const machine = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb', 'utf8');
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const verify = block(steps, 'Public NotInheritable Class VerifyStoredImportExecutionStep', 'Public NotInheritable Class CompleteImportExecutionStep');
  assert.match(verify, /_repository\.ObtenerItem\(contexto, intencion\.Id/);
  assert.match(verify, /CantidadDocumentos <> 1/);
  assert.match(verify, /CantidadRelaciones <> 1/);
  assert.match(verify, /EvidenciaFisicaConfirmada/);
  assert.match(verify, /DOCUMENT_PHYSICAL_RESOURCE_MISSING/);
  assert.match(verify, /\.Recuperable = True/);
  assert.match(verify, /ENLASE_STORAGE_RELATION_INCONSISTENT/);
  assert.match(verify, /\.PersistenciaConocida = False[\s\S]*\.Reintentable = False/);
  assert.match(machine, /DocumentoAlmacenado, FaseImportacionServicio\.CacheActualizado, FaseImportacionServicio\.Reconciliada/);
  assert.match(web, /steps\.Add\(New VerifyStoredImportExecutionStep\(reconciliationRepository\)\)[\s\S]*FaseImportacionServicio\.Completada/);
});

test('listado ENLASE oculta documento válido y rehabilita únicamente el recurso físico ausente', () => {
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const status = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportItemStatusRepository.vb', 'utf8');
  assert.match(web, /presentation\.EnrichItems\(importContext,request\.ProviderId,response\)/);
  assert.doesNotMatch(web, /If Not SiiImportProvider\.IsAnnexRequest\(request\) Then presentation\.EnrichItems/);
  assert.match(web, /Catch ex As MySql\.Data\.MySqlClient\.MySqlException When ex\.Number = 1054[\s\S]*FailureQuery\(request, "IMPORT_SCHEMA_MIGRATION_REQUIRED"\)[\s\S]*Catch ex As InvalidOperationException/);
  assert.match(status, /i\.capability=@capability/);
  assert.match(status, /ResolverGabinetes\(contexto,rows\)/);
  assert.match(status, /SELECT GABINETE FROM logdocuarchi/);
  assert.match(status, /state\.Confirmado = state\.Confirmado OrElse physicallyPresent/);
  assert.match(status, /verifiedMissing\.Add\(row\.Key\)/);
  assert.match(status, /If pair\.Value\.Confirmado OrElse verifiedMissing\.Contains\(pair\.Key\) Then pair\.Value\.TieneNovedad=False/);
});
test('preflight exige confirmación explícita para reimportar y permite recuperación cuando el recurso físico ya no existe', () => {
  const preflight = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb', 'utf8');
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  assert.match(preflight, /_status\.ObtenerLote\(contexto, contexto\.ProviderId, keys\)/);
  assert.match(preflight, /state\.Confirmado[\s\S]*explicitReimports\.Contains\(key\)[\s\S]*DOCUMENT_REIMPORT_CONFIRMATION_REQUIRED/);
  assert.match(preflight, /DOCUMENT_REIMPORT_EXPLICIT/);
  assert.doesNotMatch(preflight, /DOCUMENT_ALREADY_IMPORTED/);
  assert.doesNotMatch(preflight, /state\.TieneAntecedente[\s\S]*DOCUMENT_REIMPORT_CONFIRMATION_REQUIRED/);
  assert.match(preflight, /ExpedientMode = ModoExpedienteImportacion\.GestionarExpediente AndAlso configuration\.IdentityFields\.Count = 0/);
  assert.match(web, /New ServicioPreflightImportacion\(validator, documentTypes, effectConfiguration, New ImportEffectPlanBuilder\(\), itemStatus, documentTypeCatalog\)/);
});
test('máquina y agregación distinguen omisión idempotente, recuperable e incierto', () => {
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
  const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
  const execute = block(orchestrator, 'Public Function Execute', 'Private Shared Function AllItemsCompleted');
  const completed = execute.indexOf('If AllItemsCompleted(intent) Then');
  const effects = execute.indexOf('For Each stepItem In _steps');
  assert.ok(completed >= 0 && completed < effects);
  assert.match(execute, /AllItemsCompleted\(intent\)[\s\S]*response\.Accepted = True[\s\S]*Return response/);
  assert.match(service, /If recoverable=items\.Count Then Return "Recuperable"/);
  assert.match(mapper, /IMPORT_RESULT_UNCERTAIN[\s\S]*dto\.Retryable=False/);
  assert.match(mapper, /DOCUMENT_PHYSICAL_RESOURCE_MISSING/);
  assert.match(mapper, /DOCUMENT_RELATION_DUPLICATED/);
});
test('matriz DOC-81 cubre respuesta perdida, evidencia, ausencia física, duplicidad, parcial y filtro por elemento', () => {
  const adapter = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb', 'utf8');
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
  const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  const verify = block(steps, 'Public NotInheritable Class VerifyStoredImportExecutionStep', 'Public NotInheritable Class CompleteImportExecutionStep');
  assert.match(adapter, /MapearRechazo\(respuesta, idImagen\)/);
  assert.match(adapter, /Catch[\s\S]*FalloIncierto\(idImagen\)/);
  assert.match(orchestrator, /If fallo\.IdDocumento\.HasValue Then item\.IdDocumento = fallo\.IdDocumento[\s\S]*DesdeFallo/);
  assert.match(verify, /CantidadDocumentos <> 1 OrElse evidence\.CantidadRelaciones <> 1/);
  assert.match(verify, /Not evidence\.EvidenciaFisicaConfirmada[\s\S]*DOCUMENT_PHYSICAL_RESOURCE_MISSING/);
  assert.match(mapper, /CantidadRelaciones>1 Then Return ConsistenciaDocumentoImportacion\.RelacionDuplicada/);
  assert.match(service, /If request IsNot Nothing AndAlso Not String\.IsNullOrWhiteSpace\(request\.ExternalKey\)[\s\S]*ObtenerItem\(context,intentId,context\.ProviderId,request\.ExternalKey\)/);
  assert.match(service, /Return "Parcial"/);
});
test('DOC-81 conserva la ruta de constancias y el contrato ASMX legacy sin asignar ni cerrar tarea', () => {
  const web = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
  const asmx = fs.readFileSync('webservice/WebService_integracion_sii.asmx.vb', 'utf8');
  const storage = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');
  const enlase = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb', 'utf8');
  const compose = block(web, 'Private Shared Function Compose', 'Private Shared Function CreatePresentation');
  const oldMethod = block(asmx, 'Public Function SeviceGuardaDocumentoAnexoSII', 'Public Function ServiceSolicitaArchivosAnexosrelacionadosRadicadoSII');
  assert.match(compose, /Dim storage As IImportDocumentStoragePort = If\(isEnlase,[\s\S]*New LegacyEnlaseImportDocumentStorageAdapter\(\),[\s\S]*New LegacyImportDocumentStorageAdapter\(\)\)/);
  assert.match(storage, /\.AlmacenaDocumentoTareaWorkflow\(/);
  assert.equal((oldMethod.match(/PreAlmacenaDocumentoAnexosEnlaceIntegracionSII\(/g) || []).length, 1);
  assert.doesNotMatch(oldMethod, /RutaArchivoPreparada/);
  assert.doesNotMatch(compose + storage + enlase, /Terminar_Tarea_Workflow|AsignarTarea|CerrarTarea/i);
});
test('la migración DOC-81 exige autorización y una cuenta ALTER sin exponer secretos', () => {
  const script = fs.readFileSync('tools/e2e/scripts/apply-doc81-intent-schema-interactive.ps1', 'utf8');
  const migration = fs.readFileSync('Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-81-preparacion-persistencia-reconciliacion/Sql/001-add-intent-capability.sql', 'utf8');

  assert.match(script, /Read-Host .*Escriba SI/);
  assert.match(script, /Read-Host 'Contraseña MySQL' -AsSecureString/);
  assert.match(script, /information_schema\.columns[\s\S]*column_name='capability'/);
  assert.match(script, /ALTER TABLE workflow_import_intent ADD COLUMN capability VARCHAR\(80\) NOT NULL DEFAULT '' AFTER provider_id/);
  assert.match(script, /ALTER TABLE workflow_import_intent ADD COLUMN provider_reference VARCHAR\(80\) NOT NULL DEFAULT '' AFTER radicado/);
  assert.match(script, /DOC81_SCHEMA_VERDICT=PASS/);
  assert.doesNotMatch(script, /Write-Host[^\r\n]*(password|contraseña|connectionString)/i);
  assert.match(migration, /ADD COLUMN capability VARCHAR\(80\) NOT NULL DEFAULT '' AFTER provider_id/);
  assert.match(migration, /ADD COLUMN provider_reference VARCHAR\(80\) NOT NULL DEFAULT '' AFTER radicado/);
});
test('preparación ENLASE no exige ni construye campos exclusivos de constancias', () => {
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  const store = block(steps, 'Public NotInheritable Class StoreImportExecutionStep', 'Public NotInheritable Class VerifyStoredImportExecutionStep');
  assert.match(store, /Dim isEnlase = String\.Equals\(intencion\.ContextoOriginal\.Capability,[\s\S]*AnnexesEnlaseCapability/);
  assert.match(store, /If Not isEnlase AndAlso String\.IsNullOrWhiteSpace\(item\.MetadatosSii\.RazonSocial\)/);
  assert.match(store, /command\.Campos = If\(isEnlase,[\s\S]*New List\(Of CampoAlmacenamientoImportacion\)\(\),[\s\S]*BuildSiiFields/);
  assert.match(store, /Digits\(If\(sii\.Libro, String\.Empty\)\.Replace/);
});
test('persistencia diagnostica etapa y categoría MySQL sin exponer detalles', () => {
  const repo = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb', 'utf8');
  assert.match(repo, /Dim failureStage As String = "HEADER"/);
  for (const stage of ['REQUIREMENT','INSCRIPTION','ITEM','COMMIT']) assert.match(repo, new RegExp('failureStage = "' + stage + '"'));
  assert.match(repo, /Catch ex As MySqlException/);
  assert.match(repo, /"INTENT_PERSISTENCE_" & failureStage & "_" & PersistenceReason\(ex\)/);
  assert.match(repo, /Case 1048, 1364 : Return "REQUIRED_VALUE"/);
  assert.match(repo, /Case 1265, 1406 : Return "VALUE_LENGTH"/);
  assert.match(repo, /Case 1452 : Return "FOREIGN_KEY"/);
  assert.doesNotMatch(repo, /MensajeVisible\s*=\s*ex\.Message/);
});
test('ENLASE persiste un nombre técnico acotado y no la descripción del proveedor', () => {
  const intent = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb', 'utf8');
  const preflight = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb', 'utf8');
  const adapter = fs.readFileSync('tools/e2e/scripts/adapters/importar-servicio-web-e2e-adapter.cjs', 'utf8');
  assert.match(intent, /NombreArchivo = If\(isEnlase, EnlaseFileName\(item\.ExternalKey, item\.ContentType\), item\.FileName\)/);
  assert.match(intent, /Return "anexo-" & Hash\(If\(externalKey, String\.Empty\)\.Trim\(\)\)\.Substring\(0, 24\) & extension/);
  assert.match(preflight, /item\.ExternalKey\.Trim\(\)\.Length > 500/);
  assert.match(preflight, /Not String\.Equals\(contexto\.Capability,[\s\S]*AnnexesEnlaseCapability[\s\S]*item\.FileName[\s\S]*Length > 500/);
  assert.match(adapter, /item\.FileName = 'documento-sii\.pdf'/);
});

test('proyección ENLASE transporta un DTO tipado y nunca expone dato_lista', () => {
  const adapter = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb', 'utf8');
  const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  const projection = block(dtos, '<Serializable()> Public Class ImportEnlaseDocumentProjectionDto', '<Serializable()> Public Class PreflightImportResponseDto');
  for (const field of ['CabinetName','DocumentId','Radicado','StorageType','DocumentName','TaskId','SignatureStatus','IconClass']) {
    assert.match(projection, new RegExp(`Public Property ${field} As`));
  }
  assert.match(adapter, /CrearProyeccion\(comando, imagen, idImagen\)/);
  assert.match(adapter, /String\.IsNullOrWhiteSpace\(Convert\.ToString\(imagen\.DBT\)\), imagen\.extension/);
  assert.doesNotMatch(adapter, /String\.IsNullOrWhiteSpace\(Convert\.ToString\(imagen\.DBT\)\), imagen\.tipodocumental/);
  assert.match(adapter, /\.ProyeccionDocumentoEnlase = proyeccion/);
  assert.match(steps, /item\.ProyeccionDocumentoEnlase = resultado\.ProyeccionDocumentoEnlase/);
  assert.match(orchestrator, /\.EnlaseProjection = If\(confirmed, MapEnlaseProjection\(item\), Nothing\)/);
  assert.doesNotMatch(dtos, /dato_lista/i);
  assert.doesNotMatch(orchestrator, /dato_lista/i);
});
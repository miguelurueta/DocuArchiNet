const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const root=path.resolve(__dirname,'..');
const read=(p)=>fs.readFileSync(path.join(root,p),'utf8');
test('DOC-71 modela modo autoritativo y NoAplica de forma persistible',()=>{
  const model=read('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb');
  const repository=read('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb');
  assert.match(model,/Enum ModoExpedienteImportacion[\s\S]*SinExpediente[\s\S]*GestionarExpediente/);
  assert.match(model,/Enum EstadoEfectoExpedienteImportacion[\s\S]*NoAplica/);
  assert.match(repository,/Modo = If\(header\.CreateEnabled, ModoExpedienteImportacion\.GestionarExpediente, ModoExpedienteImportacion\.SinExpediente\)/);
});
test('DOC-71 fingerprint incorpora el modo y mantiene índices documentales planeados',()=>{
  const builder=read('Services/Workflow/ImportarServicioWeb/ImportEffectPlanBuilder.vb');
  const preflight=read('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb');
  assert.match(builder,/Return ExpedientMode\.ToString\(\)/);
  assert.match(preflight,/configuration\.CanonicalValue\(\)/);
  assert.match(builder,/code = "DOCUMENT_STORAGE" OrElse code = "DOCUMENT_INDEXES"/);
});
test('DOC-71 sin expediente actualiza cada documento por ID y ENLASE sin tocar ID_EXPEDIENTE',()=>{
  const coordinator=read('Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb');
  const gateway=read('Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiDocumentIndexPhysicalGateway.vb');
  assert.match(coordinator,/For Each document In plan\.Documentos/);
  assert.match(coordinator,/mode = ModoExpedienteImportacion\.SinExpediente[\s\S]*_indices\.Actualizar/);
  assert.match(gateway,/ActualizarCamposSinExpediente/);
  assert.match(gateway,/WHERE ID\s*=\s*@imageId AND ENLASE\s*=\s*@radicado/);
  const noExpedientMethod=gateway.slice(gateway.indexOf('Public Function ActualizarCamposSinExpediente'),gateway.indexOf('Public Function LeerCampos'));
  assert.doesNotMatch(noExpedientMethod,/SET ID_EXPEDIENTE/);
});
test('DOC-71 bifurca antes de caché y persiste un plan sin ExpedientId',()=>{
  const coordinator=read('Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb');
  const repository=read('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb');
  assert.match(coordinator,/Modo = ModoExpedienteImportacion\.GestionarExpediente Then[\s\S]*_cache\.Obtener/);
  assert.match(repository,/plan\.Modo = ModoExpedienteImportacion\.GestionarExpediente AndAlso Not inscription\.IdExpediente\.HasValue/);
  assert.match(repository,/plan\.Modo = ModoExpedienteImportacion\.SinExpediente Then[\s\S]*EstadoRelacion = EstadoEfectoExpedienteImportacion\.NoAplica/);
});
test('DOC-71 resuelve sujeto SII antes de omitir los efectos físicos de expediente',()=>{
  const coordinator=read('Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb');
  const steps=read('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb');
  const reconciliation=read('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb');
  const subjectCall=coordinator.indexOf('Dim subject = _subjects.Resolver');
  const noExpedientBranch=coordinator.indexOf('configuration.Modo = ModoExpedienteImportacion.SinExpediente');
  assert.ok(subjectCall >= 0 && subjectCall < noExpedientBranch);
  assert.match(steps,/EstadoRelacion <> EstadoEfectoExpedienteImportacion\.NoAplica Then[\s\S]*SolicitaEstructuraExpedienteSII/);
  assert.doesNotMatch(steps,/EstadoRelacion = EstadoEfectoExpedienteImportacion\.NoAplica Then[\s\S]{0,160}SolicitaEstructuraExpedienteSII/);
  assert.match(reconciliation,/EstadoDestino <> EstadoEfectoExpedienteImportacion\.NoAplica AndAlso[\s\S]*cached_expedient_id/);
});
test('DOC-71 proyecta NoAplica sin convertirlo en expediente no resuelto',()=>{
  const mapper=read('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb');
  assert.match(mapper,/ExpedientStatus = If\(withoutExpedient, "NoAplica"/);
  assert.match(mapper,/RelationStatus = If\(withoutExpedient, "NoAplica"/);
  assert.match(mapper,/EstadoDestino = EstadoEfectoExpedienteImportacion\.NoAplica Then[\s\S]*Return ImportExpedientResultCodes\.Confirmed/);
});
test('DOC-71 consulta expediente SII y permite identidad parcial sin escrituras vacías',()=>{
  const steps=read('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb');
  const adapter=read('Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb');
  const coordinator=read('Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb');
  const subjectResolver=read('Infrastructure/Workflow/ImportarServicioWeb/Expedients/ModernSiiExpedientSubjectResolver.vb');
  const siiClient=read('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb');
  assert.match(subjectResolver,/ModoExpedienteImportacion\.SinExpediente Then Return Confirmed\(\)/);
  assert.match(subjectResolver,/MergeValue\(inscripcion\.IdentificacionSujeto, subject\.Identificacion\)/);
  assert.match(subjectResolver,/MergeValue\(inscripcion\.RazonSocial, subject\.RazonSocial\)/);
  assert.doesNotMatch(siiClient,/String\.IsNullOrWhiteSpace\(identification\) OrElse String\.IsNullOrWhiteSpace\(name\).*SII_SUBJECT_INCOMPLETE/);
  assert.match(steps,/RazonSocial\) AndAlso[\s\S]*EstadoRelacion <> EstadoEfectoExpedienteImportacion\.NoAplica Then[\s\S]*SolicitaEstructuraExpedienteSII/);
  assert.doesNotMatch(steps,/EstadoRelacion = EstadoEfectoExpedienteImportacion\.NoAplica Then[\s\S]{0,160}SII_STORAGE_METADATA_UNAVAILABLE/);
  assert.match(steps,/AddOptionalField\(fields, "RAZONSOCIAL"/);
  assert.match(steps,/AddOptionalField\(fields, "NITCEDULA"/);
  assert.match(adapter,/AddIfPresent\(fields, "NITCEDULA"/);
  assert.match(adapter,/AddIfPresent\(fields, "RAZONSOCIAL"/);
  assert.match(adapter,/fields\.Count = 0 Then Return Result\(EstadoEfectoExpedienteImportacion\.NoAplica, "DOCUMENT_INDEX_FIELDS_NOT_PROVIDED"/);
  assert.match(coordinator,/If documentIndex\.Estado <> EstadoEfectoExpedienteImportacion\.Confirmado Then/);
  assert.doesNotMatch(coordinator,/documentIndex\.Estado <> EstadoEfectoExpedienteImportacion\.NoAplica/);
});

test('DOC-71 propaga metadatos descargados solo hacia campos vacíos de la inscripción',()=>{
  const orchestrator=read('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb');
  assert.match(orchestrator,/MergeDownloadedSiiMetadata\(intent, expedientPlan\)[\s\S]*_relatedDocumentCoordinator\.Procesar/);
  assert.match(orchestrator,/String\.IsNullOrWhiteSpace\(inscription\.Matricula\)[\s\S]*metadata\.Matricula/);
  assert.match(orchestrator,/String\.IsNullOrWhiteSpace\(inscription\.IdentificacionSujeto\)[\s\S]*metadata\.NitCedula/);
  assert.match(orchestrator,/String\.IsNullOrWhiteSpace\(inscription\.RazonSocial\)[\s\S]*metadata\.RazonSocial/);
});
test('DOC-71 repetición sin expediente confirma por postlectura aunque UPDATE no cambie filas',()=>{
  const gateway=read('Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiDocumentIndexPhysicalGateway.vb');
  const method=gateway.slice(gateway.indexOf('Public Function ActualizarCamposSinExpediente'),gateway.indexOf('Public Function LeerCampos'));
  assert.match(method,/affected < 0 OrElse affected > 1/);
  assert.match(method,/SELECT[\s\S]*WHERE ID=@imageId AND ENLASE=@radicado/);
  assert.doesNotMatch(method,/affected <> 1/);
});

# 09 — Flujo completo con funciones implementadas

Este diagrama presenta el recorrido de `DOC-56` usando las clases y funciones efectivamente implementadas.

```text
┌──────────────────────────────────────────────────────────────┐
│ FRONTEND WORKFLOW                                            │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ WebServiceImportarServicioWebModern                          │
│                                                              │
│ ResolveCapabilities(request)                                 │
│ QueryItems(request)                                          │
│ GetPreview(request)                                          │
│ PreflightImport(request)                                     │
│ CreateImportIntent(request)                                  │
│ ExecuteImportIntent(request)                                 │
│ GetImportIntent(request)                                     │
│ ReconcileImportIntent(request)                               │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ SEGURIDAD Y CONTEXTO                                         │
│                                                              │
│ WebServiceImportarServicioWebModern.FeatureEnabled()         │
│ WebServiceImportarServicioWebModern.ValidRequest(request)    │
│ WebServiceImportarServicioWebModern.TryBuildImportContext()  │
│ WebServiceImportarServicioWebModern.Compose()                │
│ ValidadorContextoImportacion.Validar(contexto)               │
│ MySqlSiiImportAuthorizationRepository                        │
│   → usuario, grupo, tarea, ruta, trámite y permiso            │
└──────────────────────────────┬───────────────────────────────┘
                               │
                     ┌─────────┴─────────┐
                     │ ¿Contexto válido? │
                     └─────────┬─────────┘
                         NO    │    SÍ
                          ▼    │     ▼
              ┌───────────────────┐  ┌─────────────────────────┐
              │ Respuesta segura  │  │ CONSULTAR DOCUMENTOS SII│
              │ sin persistencia  │  └────────────┬────────────┘
              └───────────────────┘               ▼
┌──────────────────────────────────────────────────────────────┐
│ CONSULTA SII                                                 │
│                                                              │
│ WebServiceImportarServicioWebModern.QueryItems()             │
│ SiiImportProvider.QueryItemsAsync()                          │
│ SiiExternalImportProviderClient.QueryItemsAsync()            │
│ SiiExternalImportProviderClient.QuerySealAsync()             │
│ SiiExternalImportProviderClient.PostAsync()                  │
│ SiiExternalImportProviderClient.SendAsync()                  │
│ SiiExternalImportProviderClient.ObserveAsync()               │
│ SiiImportContractMapper.MapQuery()                           │
│ SiiImportContractMapper.BuildExternalKey()                   │
│ SiiImportContractMapper.DisplayName()                        │
└──────────────────────────────┬───────────────────────────────┘
                               │
                  ExternalItemDto[] + ExternalKey
                         sin token ni URL
                               │
             ┌─────────────────┴─────────────────┐
             ▼                                   ▼
┌───────────────────────────────┐  ┌───────────────────────────┐
│ PREVIEW                       │  │ PREFLIGHT                 │
│                               │  │                           │
│ GetPreview()                  │  │ PreflightImport()         │
│ SiiImportProvider.            │  │ ServicioPreflightImporta- │
│   GetPreviewAsync()           │  │ cion.Preflight()          │
│ SiiExternalImportProvider-    │  │ ServicioPreflightImporta- │
│ Client.ResolveImageAsync()    │  │ cion.Fingerprint()        │
│ QuerySealAsync()              │  │ MySqlImportDocumentType-  │
│ DownloadSelectedAsync()       │  │ Resolver.Resolver()       │
│ IsAllowedDownloadHost()       │  │                           │
│ SiiImportContractMapper.      │  │ Devuelve IsValid,         │
│   MapPreview()                │  │ Requirements, Commands y  │
│ SiiPreviewResponseFactory.    │  │ ContextFingerprint        │
│   Create()                    │  │                           │
│                               │  │                           │
│ Descriptor sin persistencia  │  │                           │
└───────────────────────────────┘  └─────────────┬─────────────┘
                                                 ▼
┌──────────────────────────────────────────────────────────────┐
│ CREACIÓN IDPOTENTE DE LA INTENCIÓN                           │
│                                                              │
│ WebServiceImportarServicioWebModern.CreateImportIntent()     │
│ ServicioIntencionImportacion.Crear()                         │
│ MySqlImportIntentConcurrencyGuard.Adquirir()                 │
│   → SELECT GET_LOCK() / SELECT RELEASE_LOCK()                 │
│ MySqlImportIntentRepository.CrearOReutilizar()               │
│   → IntentId + VersionToken o IDEMPOTENCY_CONFLICT            │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ EJECUCIÓN                                                    │
│                                                              │
│ WebServiceImportarServicioWebModern.ExecuteImportIntent()    │
│ ImportServiceOrchestrator.Execute()                          │
│ MySqlImportIntentRepository.Obtener()                        │
│ ImportIntentStateMachine.Intentar()                          │
│ MySqlImportIntentRepository.ActualizarTransicion()           │
│                                                              │
│ Estado: Creada → Validada                                    │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ DESCARGA                                                     │
│                                                              │
│ DownloadImportExecutionStep.Ejecutar()                       │
│ RegistroClientesProveedoresImportacion.Resolver()            │
│ SiiImportProvider.DownloadAsync()                            │
│ SiiImportProvider.ResolveStorageMetadataAsync()              │
│ SiiExternalImportProviderClient.DownloadAsync()              │
│ SiiExternalImportProviderClient.ResolveStorageMetadataAsync()│
│ SiiExternalImportProviderClient.ResolveImageAsync()          │
│ SiiExternalImportProviderClient.DownloadSelectedAsync()      │
│                                                              │
│ Estado: RecursoObtenido                                      │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ PREPARACIÓN                                                  │
│                                                              │
│ PrepareImportExecutionStep.Ejecutar()                        │
│ PrepareImportExecutionStep.ResolveTrustedExtension()         │
│   → archivo temporal confiable                               │
│   → estado ExpedientePreparado                               │
│                                                              │
│ PrepareImportIndicesExecutionStep.Ejecutar()                 │
│   → estado IndicesActualizados                               │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ METADATOS E ÍNDICES                                          │
│                                                              │
│ StoreImportExecutionStep.Ejecutar()                          │
│ StoreImportExecutionStep.BuildSiiFields()                    │
│ StoreImportExecutionStep.SiiDate()                           │
│ StoreImportExecutionStep.Digits()                            │
│ StoreImportExecutionStep.Limit()                             │
│ MySqlImportStorageMetadataRepository.Resolver()              │
│ MySqlImportDocumentTypeResolver.Resolver()                   │
│ Class_DAT_ADIC_TAR.SolicitaReciboCodigoBarrasSII()           │
│ Class_ra_sii_migra_imagenes.SolicitaEstructuraCamposSII()   │
└──────────────────────────────┬───────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ ALMACENAMIENTO                                               │
│                                                              │
│ LegacyImportDocumentStorageAdapter.Almacenar()               │
│                     │                                        │
│                     ▼                                        │
│ ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow()         │
│                                                              │
│ Una sola invocación moderna al almacenamiento legacy.        │
└──────────────────────────────┬───────────────────────────────┘
                               │
                    ┌──────────┴───────────┐
                    │ ¿YES + IdImagen > 0? │
                    └──────────┬───────────┘
                         NO    │    SÍ
                          ▼    │     ▼
              ┌──────────────────┐  DocumentoAlmacenado
              │ Fallo o resultado│          │
              │ incierto         │  CacheActualizado
              │                  │          │
              │ No reintentar    │      Completada
              │ automáticamente  │          │
              └─────────┬────────┘          │
                        └──────────┬─────────┘
                                   ▼
┌──────────────────────────────────────────────────────────────┐
│ RECONCILIACIÓN OBLIGATORIA                                   │
│                                                              │
│ ServicioReconciliacionImportacion.ProjectExecutionResult()  │
│ ServicioReconciliacionImportacion.GetImportIntent()         │
│ ServicioReconciliacionImportacion.ReconcileImportIntent()   │
│ MySqlImportReconciliationRepository.Obtener()               │
│ MySqlImportReconciliationRepository.ObtenerItem()           │
│ ImportItemResultMapper.Map()                                 │
│ ImportItemResultMapper.Classify()                            │
└──────────────────────────────┬───────────────────────────────┘
                               │
             ┌─────────────────┴──────────────────┐
             │ ¿Documento único, confirmado y     │
             │ relacionado con la tarea correcta? │
             └─────────────────┬──────────────────┘
                      NO       │       SÍ
                       ▼       │        ▼
┌──────────────────────────────┐  ┌────────────────────────────┐
│ RESULTADO PROTEGIDO          │  │ DOCUMENTO DISPONIBLE      │
│                              │  │                            │
│ ResultadoIncierto            │  │ TaskId                     │
│ RelacionDuplicada            │  │ DocumentId                 │
│ TareaDistinta                │  │ DocumentName               │
│ RelacionAusente              │  │ ContentType                │
│ NoConfirmado                 │  │ PersistenceKnown = true    │
│                              │  │ CorrelationId              │
│ Identidad documental = null  │  │                            │
└──────────────────────────────┘  └────────────────────────────┘
```

## Cadena funcional principal

```text
QueryItems()
  → SiiImportProvider.QueryItemsAsync()
  → SiiExternalImportProviderClient.QueryItemsAsync()
  → SiiImportContractMapper.MapQuery()
  → PreflightImport()
  → ServicioPreflightImportacion.Preflight()
  → MySqlImportDocumentTypeResolver.Resolver()
  → CreateImportIntent()
  → ServicioIntencionImportacion.Crear()
  → MySqlImportIntentConcurrencyGuard.Adquirir()
  → MySqlImportIntentRepository.CrearOReutilizar()
  → ExecuteImportIntent()
  → ImportServiceOrchestrator.Execute()
  → DownloadImportExecutionStep.Ejecutar()
  → PrepareImportExecutionStep.Ejecutar()
  → PrepareImportIndicesExecutionStep.Ejecutar()
  → StoreImportExecutionStep.Ejecutar()
  → LegacyImportDocumentStorageAdapter.Almacenar()
  → ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow()
  → ServicioReconciliacionImportacion.ProjectExecutionResult()
  → MySqlImportReconciliationRepository.Obtener()
  → ImportItemResultMapper.Map()
  → Documento disponible o resultado protegido
```

## Telemetría transversal

```text
SiiExternalImportProviderClient.ObserveAsync()
  → ExternalServiceAttemptTelemetry.Create()
  → ExternalServiceAttemptTelemetry.Registrar()
  → MySqlExternalServiceTelemetryRepository.Registrar()
  → ra_ser_intento_serviciointegracion

Consulta agregada:
MySqlExternalServiceTelemetryRepository.Consultar(
    providerId,
    operation,
    desdeUtc,
    hastaUtc
)
```

# 04 — Ejecución y almacenamiento

Fuentes: `ImportServiceOrchestrator.vb`, `ImportExecutionSteps.vb`, `LegacyImportDocumentStorageAdapter.vb`.

```mermaid
sequenceDiagram
    autonumber
    participant FE as Frontend
    participant WS as ASMX
    participant OR as ImportServiceOrchestrator
    participant IR as MySqlImportIntentRepository
    participant DL as DownloadImportExecutionStep
    participant SII as SiiImportProvider
    participant PP as PrepareImportExecutionStep
    participant PI as PrepareImportIndicesExecutionStep
    participant ST as StoreImportExecutionStep
    participant MD as MySqlImportStorageMetadataRepository
    participant DT as MySqlImportDocumentTypeResolver
    participant LA as LegacyImportDocumentStorageAdapter
    participant CL as ClassAlmacenamiento
    participant RC as ServicioReconciliacionImportacion

    FE->>WS: ExecuteImportIntent(IntentId, VersionToken, StopRequested=false)
    WS->>OR: Execute(context, request)
    OR->>IR: Obtener(context, IntentId)
    IR-->>OR: IntencionImportacionServicio
    OR->>OR: compara VersionToken y evalúa reintento/detención
    OR->>IR: transición Creada -> Validada
    OR->>DL: Ejecutar(context, intent, item)
    DL->>SII: DownloadAsync + ResolveStorageMetadataAsync
    DL-->>OR: bytes + MetadatosDocumentoSii
    OR->>IR: transición -> RecursoObtenido
    OR->>PP: escribe archivo temporal con extensión confiable
    OR->>IR: transición -> ExpedientePreparado
    OR->>PI: valida archivo preparado
    OR->>IR: transición -> IndicesActualizados
    OR->>ST: Ejecutar
    ST->>MD: Resolver(context): ruta, gabinete, clase
    ST->>DT: Resolver(context, IdTipoDocumental, NombreTipoDocumental)
    ST->>ST: SolicitaReciboCodigoBarrasSII
    ST->>ST: completa sujeto si falta y BuildLegacyIndexes
    ST->>LA: Almacenar(ComandoAlmacenamientoImportacion)
    LA->>CL: AlmacenaDocumentoTareaWorkflow(...)
    CL-->>LA: respuesta + IdImagen
    LA-->>ST: ResultadoFaseImportacion(IdDocumento)
    ST->>ST: elimina archivo temporal en Finally
    OR->>IR: transición -> DocumentoAlmacenado
    OR->>IR: transición -> CacheActualizado
    OR->>IR: transición -> Completada
    OR-->>WS: resultado interno
    WS->>RC: ProjectExecutionResult(context, execution)
    RC-->>FE: ExecuteImportIntentResponseDto confirmado o protegido
```

Cada llamada a `ActualizarTransicion` actualiza item e intención y agrega una fila de auditoría con control de versión.

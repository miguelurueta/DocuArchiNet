# 03 — Preflight, idempotencia e intención

Fuentes: `ServicioPreflightImportacion.vb`, `ServicioIntencionImportacion.vb`, `MySqlImportDocumentTypeResolver.vb`, `MySqlImportIntentRepository.vb`, `MySqlImportIntentConcurrencyGuard.vb`.

```mermaid
sequenceDiagram
    autonumber
    participant FE as Frontend
    participant WS as ASMX
    participant PF as ServicioPreflightImportacion
    participant AU as ValidadorContextoImportacion
    participant DT as MySqlImportDocumentTypeResolver
    participant RAD as Radicación
    participant IS as ServicioIntencionImportacion
    participant LG as MySqlImportIntentConcurrencyGuard
    participant IR as MySqlImportIntentRepository
    participant WDB as Workflow DB

    FE->>WS: PreflightImport(Items[])
    WS->>PF: Preflight(context, request)
    PF->>AU: Validar(context)
    AU-->>PF: permiso/tarea/ruta/trámite/proveedor
    loop cada ImportItemSelectionDto
        PF->>DT: Resolver(context, DocumentTypeId, DocumentTypeName)
        DT->>RAD: SELECT tipo_doc_series + ra_dig_tipos_docum_lista_chequeo por IdTramite
        RAD-->>DT: 0, 1 o múltiples coincidencias
        DT-->>PF: ResolucionTipoDocumentalImportacion
    end
    PF-->>FE: IsValid, Requirements, Commands, ContextFingerprint

    FE->>WS: CreateImportIntent(IdempotencyKey, Items, Requirements, ContextFingerprint, Radicado)
    WS->>IS: Crear(context, request)
    IS->>AU: Validar(context)
    IS->>LG: Adquirir(context, IdempotencyKey)
    LG->>WDB: SELECT GET_LOCK(lockName, 0)
    IS->>IS: calcula huella canónica SHA-256
    IS->>IR: CrearOReutilizar(context, intent)
    IR->>WDB: INSERT workflow_import_intent
    IR->>WDB: INSERT workflow_import_intent_requirement
    IR->>WDB: INSERT workflow_import_intent_item
    IR-->>IS: creada / reutilizada / IDEMPOTENCY_CONFLICT
    IS->>LG: Dispose lease
    LG->>WDB: SELECT RELEASE_LOCK(lockName)
    IS-->>FE: IntentId, Status, VersionToken, Reused
```

El ID de lista de chequeo resuelto durante preflight no cruza el contrato; se vuelve a resolver antes del almacenamiento.

# 07 — Persistencia y telemetría

Fuentes: migraciones SQL y repositorios `MySqlImportIntentRepository`, `MySqlImportReconciliationRepository`, `MySqlExternalServiceTelemetryRepository`.

```mermaid
erDiagram
    workflow_import_intent ||--o{ workflow_import_intent_requirement : contiene
    workflow_import_intent ||--o{ workflow_import_intent_item : contiene
    workflow_import_intent ||--o{ workflow_import_intent_transition : audita
    ra_ser_serviciointegracion ||--o{ ra_ser_intento_serviciointegracion : clasifica

    workflow_import_intent {
        varchar intent_id PK
        varchar idempotency_key
        varchar payload_hash
        varchar operation_id
        varchar correlation_id
        int user_id
        int group_id
        bigint task_id
        int route_id
        int procedure_id
        varchar provider_id
        varchar radicado
        varchar status
        varchar version_token
    }
    workflow_import_intent_item {
        varchar intent_id FK
        varchar client_item_id
        varchar provider_id
        varchar external_key
        bigint target_task_id
        int document_type_id
        varchar document_type_name
        varchar file_name
        varchar content_type
        varchar status
        bigint document_id
        boolean persistence_known
        boolean retryable
        varchar error_code
        varchar correlation_id
    }
    workflow_import_intent_transition {
        varchar intent_id FK
        varchar client_item_id
        varchar previous_status
        varchar new_status
        varchar previous_version
        varchar new_version
        datetime occurred_utc
        varchar correlation_id
        varchar result_code
    }
    ra_ser_serviciointegracion {
        int Id_ser_servicioIntegracion PK
        varchar NombreServicio UK
        datetime FechaRegistro
        int EstadoServicio
    }
    ra_ser_intento_serviciointegracion {
        bigint Id_ser_intentoServicioIntegracion PK
        int Id_ser_servicioIntegracion FK
        varchar NombreServicioSnapshot
        varchar Operacion
        boolean Exitoso
        varchar CodigoError
        varchar CategoriaError
        int EstadoHttp
        boolean Reintentable
        bigint TaskId
        varchar Radicado
        varchar CodigoBarras
        varchar ReferenciaProveedor
        varchar IntentId
        varchar ClientItemId
        varchar OperationId
        varchar CorrelationId
        datetime FechaInicioUtc
        datetime FechaFinUtc
        bigint DuracionMs
    }
```

La FK de telemetría es exclusivamente local a DocuArchi. `IntentId`, tarea, radicado, código de barras y referencia son snapshots opcionales, sin FK entre bases y sin exigir código de barras a otros proveedores.

# 06 — Reconciliación y contrato de salida

Fuentes: `ServicioReconciliacionImportacion.vb`, `MySqlImportReconciliationRepository.vb`, `ImportItemResultMapper.vb`.

```mermaid
sequenceDiagram
    autonumber
    participant WS as ASMX
    participant RS as ServicioReconciliacionImportacion
    participant RR as MySqlImportReconciliationRepository
    participant WF as Workflow DB
    participant DA as DocuArchi DB
    participant MP as ImportItemResultMapper
    participant FE as Frontend

    WS->>RS: ProjectExecutionResult / GetImportIntent / ReconcileImportIntent
    RS->>RR: Obtener(context, intentId) u ObtenerItem(..., externalKey)
    RR->>WF: SELECT workflow_import_intent + workflow_import_intent_item
    WF-->>RR: intención, versión, item, document_id, estado
    loop item con DocumentId
        RR->>DA: SELECT logdocuarchi por id_tran y módulo WORKFLOW
        RR->>DA: SELECT registro_producion_documental por documento/gabinete
        DA-->>RR: relaciones de tarea + nombre documental
    end
    RR-->>RS: SnapshotReconciliacionImportacion
    RS->>MP: Map(item, IdTareaOriginal)
    MP->>MP: Classify
    alt un documento y una relación, sin otra tarea
        MP-->>RS: Status Disponible + TaskId + DocumentId + DocumentName + ContentType
    else resultado incierto
        MP-->>RS: IMPORT_RESULT_UNCERTAIN sin identidad documental
    else relación ausente/duplicada/tarea distinta
        MP-->>RS: Inconsistente + código seguro sin identidad documental
    end
    RS->>RS: deduplica por TaskId:DocumentId y AggregateStatus
    RS-->>FE: respuesta pública
```

Clasificaciones exactas: `Confirmado`, `ResultadoIncierto`, `RelacionDuplicada`, `TareaDistinta`, `RelacionAusente` y `NoConfirmado`.

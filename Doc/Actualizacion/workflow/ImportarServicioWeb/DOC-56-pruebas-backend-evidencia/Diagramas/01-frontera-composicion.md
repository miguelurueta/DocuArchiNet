# 01 — Frontera ASMX y composición

Fuente: `webservice/WebServiceImportarServicioWebModern.asmx.vb`.

```mermaid
flowchart TB
    FE[Frontend Workflow] --> ASMX[WebServiceImportarServicioWebModern]
    ASMX --> G{FeatureEnabled}
    G -- false --> FD[Error FEATURE_DISABLED]
    G -- true --> V{ValidRequest}
    V -- false --> IR[Error INVALID_REQUEST]
    V -- true --> C{TryBuildImportContext}
    C --> SG[WorkflowPreviewSessionContextGate.AsegurarContexto]
    C --> ST[Session ID_TAREA_SELECCIONDA]
    C --> SP[Session DG_ID_TRAMITE o Classselecciotarea.Solicita_id_tipo_tramite_tarea_workflow]
    C -- false --> CF[Error de contexto seguro]
    C -- true --> CO[Compose]

    CO --> WF[WorkflowModuleConnectionFactory]
    CO --> DA[DocuarchiModuleConnectionFactory]
    CO --> RA[RadicacionModuleConnectionFactory]
    CO --> AU[ValidadorContextoImportacion + MySqlSiiImportAuthorizationRepository]
    CO --> IN[ServicioIntencionImportacion + MySqlImportIntentRepository + MySqlImportIntentConcurrencyGuard]
    CO --> OR[ImportServiceOrchestrator + ImportIntentStateMachine]
    CO --> RE[ServicioReconciliacionImportacion + MySqlImportReconciliationRepository + ImportItemResultMapper]
    CO --> PR[RegistroClientesProveedoresImportacion + SiiImportProvider]
    CO --> TE[MySqlExternalServiceTelemetryRepository]
```

Operaciones que atraviesan esta frontera: `ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`.

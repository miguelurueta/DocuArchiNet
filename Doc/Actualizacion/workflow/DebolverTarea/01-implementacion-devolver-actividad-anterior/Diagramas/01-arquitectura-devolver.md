# Componentes y fronteras de DOC-32

```mermaid
flowchart LR
    Cliente[Cliente futuro de etapa 02] -->|idTarea, término, cursor\no idTarea, idConector, token| ASMX[WebServiceWorkflowModern]
    ASMX --> Sesion[WorkflowPreviewSessionContextGate]
    ASMX --> Servicio[ServicioDevolverActividad]
    Servicio --> Repositorio[MySqlDevolverActividadRepository]
    Servicio --> Cursor[DevolverActividadCursorCodec]
    Servicio --> Guard[MySqlDevolverActividadConcurrencyGuard]
    Servicio --> Adaptador[WorkflowLegacyDevolverActividadExecutorAdapter]
    Servicio --> Auditoria[WorkflowLegacyAuditoriaAdapter]
    Repositorio --> MySQL[(Workflow MySQL)]
    Guard --> MySQL
    Adaptador --> Motor[ClassWorkflow.Terminar_Tarea_Workflow]
    Auditoria --> MySQL

    %% Contraste AA/AAA: texto oscuro explícito sobre fondos claros.
    classDef boundary fill:#e8f1fb,stroke:#174a7e,color:#102a43,stroke-width:2px;
    classDef external fill:#fff7e6,stroke:#8a4b08,color:#3b2405,stroke-width:2px;
    classDef datastore fill:#e9f7ef,stroke:#176b3a,color:#123b25,stroke-width:2px;
    class Cliente external;
    class MySQL datastore;
    class ASMX,Servicio,Repositorio,Cursor,Guard,Adaptador,Auditoria boundary;
```

El cliente no llega a MySQL ni al motor legacy. El ASMX reconstruye el contexto y el servicio decide el único punto mutante después de revalidar el destino.

# Arquitectura DOC-42

```mermaid
flowchart TB
  U[Usuario Workflow autenticado] --> A[ASMX WebServiceWorkflowNotesModern]
  A --> S[ServicioNotasWorkflow]
  S --> R[MySqlNotasWorkflowRepository]
  R --> N[(ANOTACION_TAREA)]
  R --> V[(workflow_notas_version)]
  R --> I[(workflow_notas_idempotencia)]
  R --> L[(wf_log_workflow)]
  S -. contexto y autorización .-> C[Contexto Workflow confiable]
  classDef actor fill:#fff7e6,stroke:#8a4b08,color:#3b2405,stroke-width:2px;
  classDef app fill:#e8f1fb,stroke:#174a7e,color:#102a43,stroke-width:2px;
  classDef data fill:#edf7ed,stroke:#2f6f3e,color:#183b20,stroke-width:2px;
  class U actor; class A,S,R,C app; class N,V,I,L data;
```

El repositorio calcula SHA-256 en .NET. Actualización y eliminación condicionan nota y ledger en una única sentencia dentro de la transacción; la auditoría se confirma o revierte junto con la mutación.

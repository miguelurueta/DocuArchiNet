# Arquitectura de componentes

```mermaid
flowchart TB
    Usuario[Usuario autorizado]
    UI[Webworkflow.aspx\nworkflow-group-send-ui.js\nworkflow-group-send-confirmation.js]
    Gate[WorkflowCentroTrabajoModernActive]
    ASMX[WebServiceWorkflowModern.asmx\nPreviewEnviarGrupo / EjecutarEnvioGrupo]
    Aplicacion[ServicioEnvioGrupoTarea\nValidadorEnvioGrupoTarea]
    Dominio[Modelos, DTOs e interfaces\nde Enviar a grupo]
    Repo[MySqlEnvioGrupoRepository\nConsultas SELECT]
    Requisitos[WorkflowLegacyEnvioGrupoRequisitosAdapter\nAprobaciones]
    Executor[WorkflowLegacyEnvioGrupoExecutorAdapter]
    Motor[Motor legacy\nTerminar_Tarea_Workflow]
    Auditoria[Auditoría sanitizada\nASMX_ENVIO_GRUPO]
    Continuar[Continuar flujo existente\nIdConector y adaptador sin cambios]

    Usuario --> UI
    Gate --> UI
    UI -->|gate activo| ASMX
    UI -->|gate inactivo| LegacyPostback[Postback Web Forms legacy]
    ASMX --> Aplicacion
    Aplicacion --> Dominio
    Aplicacion --> Repo
    Aplicacion --> Requisitos
    Aplicacion --> Executor
    Executor --> Motor
    Aplicacion --> Auditoria
    UI -. no modifica .-> Continuar
```

La infraestructura es la única capa que conoce consultas SQL y la llamada al motor legado. Los contratos de dominio y aplicación no exponen `Page`, `Session` ni `IdConector`.

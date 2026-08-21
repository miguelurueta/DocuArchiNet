# Arquitectura de componentes

```mermaid
flowchart TB
    Cliente[Cliente futuro etapa 02\nNo entregado en DOC-28]
    ASMX[WebServiceWorkflowModern.asmx\nPreviewEnviarUsuario / EjecutarEnvioUsuario]
    Contexto[WorkflowPreviewSessionContextGate\nCAMBIO_USUARIO fail-closed]
    Aplicacion[ServicioEnvioUsuarioTarea\nValidadorEnvioUsuarioTarea]
    Dominio[Modelos, DTOs e interfaces\nexclusivos de usuario]
    Repo[MySqlEnvioUsuarioRepository\nSELECT y cursor protegido]
    Lock[MySqlTransicionConcurrencyGuard\nGET_LOCK]
    Permiso[Autorización legacy\nCAMBIO_USUARIO]
    Requisitos[Requisitos legacy\nrespuesta YES]
    Executor[WorkflowLegacyEnvioUsuarioExecutorAdapter]
    Motor[ClassWorkflow\nTerminar_Tarea_Workflow]
    Auditoria[Auditoría sanitizada\nASMX_ENVIO_USUARIO]
    Continuar[Continuar flujo existente\nIdConector sin cambios]

    Cliente --> ASMX
    ASMX --> Contexto
    ASMX --> Aplicacion
    Aplicacion --> Dominio
    Aplicacion --> Repo
    Aplicacion --> Lock
    Aplicacion --> Permiso
    Aplicacion --> Requisitos
    Aplicacion --> Executor
    Executor --> Motor
    Aplicacion --> Auditoria
    ASMX -. no modifica .-> Continuar
```

La infraestructura es la única capa que conoce SQL, permisos legacy, requisitos y motor. Los contratos de dominio y ASMX no exponen `Page`, `Session` ni `IdConector`.

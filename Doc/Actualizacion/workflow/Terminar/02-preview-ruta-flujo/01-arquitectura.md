# Arquitectura

El preview es un recorrido paralelo de lectura. No llama ni compone el ejecutor legacy. El diagrama de capas renderizable está en [02-clases-y-componentes.md](Diagramas/02-clases-y-componentes.md).

## Responsabilidades

| Capa | Responsabilidad | No permitido |
| --- | --- | --- |
| Presentation | Valida la sesión autenticada y, desde Gestión, vuelve a validar la relación Workflow; resuelve snapshots Workflow y Docuarchi para el contexto seguro; devuelve DTO/errores seguros. | Recibir usuario, grupo, ruta, actividad o credenciales del cliente; SQL; excepción serializada. |
| Application | Gate, validación, selección FLUJO/RUTA y mapeo. | Terminación, eventos, correo, firma, cambio de estado. |
| Infrastructure/Repositories/Workflow | Consultas parametrizadas de tarea, flujo y ruta; el estado documental de ruta se lee en Docuarchi y los destinos en Workflow. | `Session`, `DataSet`, HTML, controles Web Forms o escritura. |
| Infrastructure/Shared/Data | Conexión, comandos, parámetros y lectores tipados. | Conocer Workflow o Presentation. |

`ServicioTransicionTarea` tiene una composición específica de preview con cinco dependencias: repositorios de tarea/flujo/ruta, gate y validador. Esa composición deja en `Nothing` el repositorio de conector y el ejecutor; por tanto el endpoint no puede completar una transición.

Las tablas y columnas configurables de ruta se validan como identificadores antes de interpolarse. Los valores funcionales se envían como parámetros MySQL.

`tipo_doc_entrante` reside en Docuarchi. El gate entrega su snapshot sin serializarlo y el ASMX crea `DocuarchiModuleConnectionFactory`; `MySqlTransicionRutaRepository` lo usa solo para `estado_ruta_open_close`. La tarea, configuración y destinos continúan usando `WorkflowModuleConnectionFactory`.

Para `FLUJO`, el repositorio enumera conectores salientes autorizados desde la actividad real. No usa `TIPO_RUTA_ABIERTA_CERRADA` ni `TIPO_ABIERTA_CERRADA_ACTIVIDAD`: son configuración de libertad de asignación, no disponibilidad de envío.

## Rutas de implementación

| Capa | Ruta real |
| --- | --- |
| Presentation | `webservice/WebServiceWorkflowModern.asmx(.vb)` y `webservice/WorkflowPreviewSessionContextGate.vb` |
| Application | `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` |
| Contratos/modelos | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`, `Modelo/Workflow/Terminar/WorkflowModernModels.vb`, `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` |
| Infrastructure Workflow | `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` |
| Infrastructure compartida | `Infrastructure/Shared/Data/ModuleDataContracts.vb`, `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb`, `Infrastructure/Shared/Data/ModuleConnectionFactory.vb`, `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` (`WorkflowModuleConnectionFactory` y `DocuarchiModuleConnectionFactory`) |
| Gate | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` |

## Alternativas descartadas

- Reusar `ClassWorkflow.Validar_enviar_actividad_por_conector_flujo_o_ruta(Page)`: depende de página, controles y sesión.
- Reusar `InicioWorkflow.InicializaSesionModuloWorkflow`: carga permisos, compila scripts y registra auditoría; el preview solo usa su consulta legacy de usuario/ruta/grupo para completar una sesión Gestión existente.
- Llamar `Terminar_Tarea_Workflow` o `Cambia_Estado`: escriben y pertenecen al envío legacy.
- Crear un repositorio genérico o devolver `DataSet`: debilita los límites del caso de uso y el contrato JSON.

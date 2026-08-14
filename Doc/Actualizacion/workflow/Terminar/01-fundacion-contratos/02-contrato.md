# Contratos

- Ticket: DOC-9
- Cambio OpenSpec: doc-9-contrato-terminar-tarea-workflow
- Clasificacion: cross_cutting

## DTOs de salida futura

Los contratos de este caso de uso residen en `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`. La fachada y el validador correspondientes residen en `Services/Workflow/Terminar/`; todas las rutas parten de la raíz del repositorio, conservan la responsabilidad de Application y se agrupan por el módulo `Workflow` y el caso de uso `Terminar`.

| DTO | Campos principales | Uso y protección |
|---|---|---|
| `PrevisualizacionTransicionDto` | `IdTarea`, `Origen`, `TipoDecision`, `Destinos`, `Requisitos`, `TokenVersion`, `Error` | Contrato de preview futuro; no retorna HTML ni `DataSet`. |
| `DestinoTransicionDto` | `Id`, `Nombre`, `Tipo`, `Orden` | Describe un conector permitido, sin identidad interna del usuario. |
| `RequisitoTransicionDto` | `Codigo`, `Descripcion`, `Obligatorio`, `Satisfecho` | Reporta estado funcional de un requisito. |
| `ResultadoTransicionDto` | `Exito`, `EstadoFinal`, `MensajeFuncional`, `CodigoBloqueo`, auditoría y reintento | Contrato de ejecución futura; no serializa excepciones o SQL. |
| `ErrorTransicionDto` | `Codigo`, `MensajeVisible`, `ReferenciaTrazabilidad` | Error estable y visible para UI. |
| `HabilitacionWorkflowModernDto` | `Estado`, `Codigo`, `MensajeFuncional`, `Activo` | Expone solo la decisión de habilitación. |

## Componentes Application

| Componente | Ruta | Entrada | Salida o efecto permitido |
|---|---|---|---|
| `ServicioTransicionTarea` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | `ContextoModuloWorkflow`, id de tarea o `SolicitudTransicionWorkflow` | Preview o resultado funcional; no conoce `Page`, `Session` ni SQL. |
| `ValidadorTransicionTarea` | `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | Contexto o solicitud | `ErrorTransicionDto` controlado cuando el contexto, tarea o conector no son válidos. |
| `EvaluadorHabilitacionWorkflowModern` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Puerto `IWorkflowModernFeatureGate` | `HabilitacionWorkflowModernDto`; trata una respuesta ausente como `inactivo`. |
| `ProveedorTransicionesFlujo` / `ProveedorTransicionesRuta` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Puerto de flujo o ruta y `TareaWorkflow` | Colección de `DestinoTransicionDto`, sin datos de interfaz. |
| `EjecutorTransicionTarea` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | `IWorkflowLegacyExecutor`, contexto y solicitud | Mapea el resultado del puerto; no invoca clases legacy directamente. |

## Interfaces de Domain

Las interfaces y modelos internos de Workflow están en `Modelo/Workflow/Terminar/`; la ruta física no altera su responsabilidad de Domain.

| Interfaz | Entrada | Responsabilidad |
|---|---|---|
| `ITareaWorkflowRepository` | contexto, id de tarea | Obtener estado tipado de una tarea accesible. |
| `ITransicionFlujoRepository` | contexto y tarea | Consultar destinos permitidos por flujo. |
| `ITransicionRutaRepository` | contexto y tarea | Consultar destinos permitidos por ruta. |
| `IConfiguracionConectorRepository` | contexto, tarea y conector | Revalidar que un conector sigue disponible. |
| `IAuditoriaTransicionRepository` | auditoría técnica tipada | Registrar el resultado sin exponerlo al navegador. |
| `IWorkflowModernFeatureGate` | contexto validado | Decidir la habilitación moderna en servidor. |
| `IWorkflowLegacyExecutor` | contexto y solicitud | Encapsular la futura llamada al motor legacy. |

## Infraestructura compartida

`ContextoModulo` es el contrato común de identidad de módulo, usuario, grupo y login. `ContextoModuloWorkflow` lo hereda y fija el módulo `WORKFLOW`. `IModuleConnectionFactory` recibe únicamente `ContextoModulo`; `ModuleConnectionFactory`, `AdoNetDataExecutor` y `DbTransactionFactory` están en `Infrastructure/Shared/Data/` y no conocen códigos o modelos Workflow.

Los repositorios de negocio se implementarán únicamente bajo `Infrastructure/Repositories/Workflow/`. Deberán recibir el contexto ya validado, usar `IDataExecutor` e `ITransactionFactory`, parametrizar consultas y devolver modelos tipados; no pueden leer `HttpContext.Current.Session`, devolver `DataSet` ni reutilizarse como `GenericRepository`.

## Habilitación

`Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` requiere el valor explícito `WorkflowCentroTrabajoModernActive=true|1|yes`. Los filtros opcionales son `WorkflowCentroTrabajoModernUsers`, `WorkflowCentroTrabajoModernGroups`, `WorkflowCentroTrabajoModernExcludedUsers` y `WorkflowCentroTrabajoModernExcludedGroups`. Si falta la clave base, es inválida o el perfil no coincide, el resultado es `inactivo`; una exclusión explícita devuelve `excluido`.

La configuración se resuelve únicamente en servidor. `ContextoModuloWorkflow` llega validado a la capa y los repositorios no tienen permiso de consultar `HttpContext.Current.Session`.

## Adaptador legacy

`Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb` implementa el contrato actual de `WorkflowLegacyExecutorAdapter` y devuelve:

```text
Exito = false
EstadoFinal = pendiente
CodigoBloqueo = WORKFLOW_MODERN_EXECUTION_PENDING
```

Esta respuesta hace segura la fundación: todavía no hay endpoint, no existe una `Page` reconstruida ni se ejecuta `Terminar_Tarea_Workflow`, `Cambia_Estado` o un evento dinámico.

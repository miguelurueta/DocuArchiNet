# Arquitectura

- Ticket: DOC-9
- Cambio OpenSpec: doc-9-contrato-terminar-tarea-workflow
- Clasificacion: cross_cutting

## Decisión

DOC-9 aplica una arquitectura limpia pragmática dentro del monolito Web Forms. Es una primera etapa de *Strangler Fig*: los contratos nuevos coexisten con el código actual, sin redirigir solicitudes ni reescribir reglas que el motor legacy ya resuelve.

```text
Futuro Presentation (ASMX / JavaScript)
                 |
                 v
Application: ServicioTransicionTarea / validadores / DTOs
                 |
                 v
Domain: modelos e interfaces de tarea, rutas, flujo y auditoría
                 |
                 v
Infrastructure: configuración, datos MySQL y adaptador legacy
                 |
                 v
ClassWorkflow.Terminar_Tarea_Workflow / Cambia_Estado (no invocados en DOC-9)
```

## Estructura física implementada

```text
DTOs/Workflow/Terminar/
  TransicionWorkflowDtos.vb
Services/Workflow/Terminar/
  ServicioTransicionTarea.vb
  ValidadorTransicionTarea.vb
Modelo/Workflow/Terminar/
  WorkflowModernModels.vb
  WorkflowModernInterfaces.vb
Domain/Shared/
  ContextoModulo.vb
Infrastructure/Shared/Data/
  ModuleDataContracts.vb
  ModuleConnectionFactory.vb
  AdoNetDataInfrastructure.vb
Infrastructure/Workflow/Terminar/
  ConfiguracionWorkflowModernFeatureGate.vb
  WorkflowLegacyExecutorAdapter.vb
Infrastructure/Repositories/Workflow/
  README.md
```

`Infrastructure/Repositories/Workflow/` es solo un límite de diseño en DOC-9: aún no contiene implementaciones MySQL ni SQL de negocio. La ubicación bajo la raíz evita mezclar las capas nuevas con la carpeta legacy `workflow/`.

## Separación de responsabilidades

| Clase o función | Ruta | Capa | Parámetros/DTO | Responsabilidad | Dependencia legacy permitida |
|---|---|---|---|---|---|
| `ContextoModulo` | `Domain/Shared/ContextoModulo.vb` | Domain compartido | módulo, usuario, grupo y login | Representar el contexto mínimo reusable de un módulo. | Ninguna. |
| `ContextoModuloWorkflow`, `TareaWorkflow` | `Modelo/Workflow/Terminar/WorkflowModernModels.vb` | Domain Workflow | contexto especializado y tarea tipados | Representar datos de Workflow sin ASP.NET. | Ninguna. |
| Puertos de repositorio y gate | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Domain Workflow | contexto, tarea, solicitud | Declarar necesidades de datos, auditoría, habilitación y ejecución. | Ninguna. |
| DTOs | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` | Application | preview, resultado, error, habilitación | Definir la futura respuesta serializable sin datos internos para el caso de uso Terminar. | Ninguna. |
| `ServicioTransicionTarea` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Application | contexto e identificadores validados | Orquestar gate, validación, repositorios y ejecutor del caso de uso Terminar. | Solo interfaz `IWorkflowLegacyExecutor`. |
| `ValidadorTransicionTarea` | `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | Application | contexto y solicitud | Bloquear contexto, tarea o conector inválidos con errores funcionales estables. | Ninguna. |
| `ProveedorTransicionesFlujo`, `ProveedorTransicionesRuta` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Application | contexto y tarea | Obtener y mapear destinos tipados desde los puertos de Workflow. | Ninguna. |
| `EvaluadorHabilitacionWorkflowModern`, `EjecutorTransicionTarea` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Application | contexto y solicitud | Mapear gate a DTO y delegar la ejecución exclusivamente al puerto legacy. | Solo interfaz `IWorkflowLegacyExecutor`. |
| `ConfiguracionWorkflowModernFeatureGate` | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | Infrastructure Workflow | `ContextoModuloWorkflow` | Resolver configuración por servidor con fallo cerrado. | Ninguna. |
| `IModuleConnectionFactory`, `ModuleConnectionFactory`, `IDataExecutor`, `ITransactionFactory` | `Infrastructure/Shared/Data/...` | Infrastructure compartida | `ContextoModulo`, conexión, parámetros | Resolver conexión autorizada y operaciones parametrizadas reutilizables, sin símbolos Workflow. | Ninguna. |
| Límite de repositorios Workflow | `Infrastructure/Repositories/Workflow/README.md` | Infrastructure Workflow | `ContextoModuloWorkflow` futuro | Reservar implementaciones específicas y parametrizadas, sin repositorio genérico ni SQL en DOC-9. | Ninguna. |
| `WorkflowLegacyExecutorAdapter` | `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb` | Infrastructure / límite legacy | contexto y solicitud | Ser el único sitio autorizado para integrar la transición legacy en una fase posterior. | `Terminar_Tarea_Workflow` y `Cambia_Estado`, aún no invocados. |

## Reutilización y límites

`Domain/Shared/ContextoModulo` e `Infrastructure/Shared/Data` no conocen entidades Workflow: Radicación, Expedientes y Gestión Documental podrán usar las mismas abstracciones de contexto, conexión, transacción, resultados y paginación. Cada módulo especializa el contexto común y conserva sus propios repositorios de negocio; no se creó un `GenericRepository`.

La carpeta `Infrastructure/Repositories/Workflow/` existe como límite de las futuras implementaciones tipadas. No contiene SQL en DOC-9 porque copiar las consultas legacy equivaldría a duplicar reglas antes de tener el preview de solo lectura.

Los contratos y servicios de Application se organizan desde la raíz primero por tipo técnico, después por módulo y por caso de uso: `DTOs/Workflow/Terminar` y `Services/Workflow/Terminar`. Esta convención permite que futuros módulos tengan sus propios DTOs y servicios sin quedar bajo la carpeta física de Workflow ni repetir el segmento `Services`.

## Alternativas descartadas

- Exponer `ClassWorkflow` desde JavaScript o un ASMX: filtra dependencias de `Page` y `Session` y omite validaciones del ciclo Web Forms.
- Migrar de una vez la transición: arriesga autorizaciones, firma, expediente, balanceo y eventos dinámicos.
- Implementar un repositorio genérico: diluye los límites de módulos y facilita consultas sin contexto autorizado.

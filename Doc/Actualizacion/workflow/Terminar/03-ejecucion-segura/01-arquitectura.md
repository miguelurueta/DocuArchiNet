# Arquitectura y responsabilidades

## Regla central

El navegador solicita; el servidor decide, valida y ejecuta. El `DestinoTransicionDto` del preview es informativo y nunca se usa como orden de ejecución.

## Capas

| Clase o función | Ruta | Capa | Parámetros/DTO | Responsabilidad | Dependencia legacy permitida |
| --- | --- | --- | --- | --- | --- |
| `EjecutarEnvioTarea` | `webservice/WebServiceWorkflowModern.asmx.vb` | Presentation | `idTarea`, `idConector`, `tokenVersion` → `ResultadoTransicionDto` | Compone dependencias y devuelve JSON controlado. | Ninguna llamada directa. |
| `AsegurarContextoEjecucion` | `webservice/WorkflowPreviewSessionContextGate.vb` | Presentation boundary | `HttpContext.Session` → `ContextoModuloWorkflow` | Reconstruye identidad Gestión→Workflow, permisos y ruta; preserva los scripts que el login ya cargó y solo los compila si faltan. | Consultas de permisos y, solo como recuperación, `CompilaScriptUsuario`; no llama el motor. |
| `ServicioTransicionTarea.Ejecutar` | `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Application | Contexto + `SolicitudTransicionWorkflow` | Ordena gate, validación, guard, relectura, resolución, requisitos, ejecución y auditoría. | Ninguna. |
| `ValidadorTransicionTarea` | `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | Application | Solicitud/contexto → `ErrorTransicionDto` | Rechaza valores mal formados con código estable. | Ninguna. |
| `MySqlTransicionEjecucionRepository.ResolverDestino` | `Infrastructure/Repositories/Workflow/MySqlTransicionEjecucionRepository.vb` | Infrastructure | Contexto, tarea, conector → `DestinoEjecucionWorkflow` | Reautoriza destino RUTA o FLUJO y obtiene los argumentos reales de ejecución. | Solo tablas Workflow, mediante `IModuleConnectionFactory`. |
| `WorkflowLegacyRequisitosAdapter.Evaluar` | `Infrastructure/Workflow/Terminar/WorkflowLegacyRequisitosAdapter.vb` | Infrastructure | Contexto, tarea, destino → requisitos | Reutiliza comprobación de respuesta y aprobación; oculta Session y textos internos. | Clases legacy de requisitos. |
| `MySqlTransicionConcurrencyGuard.Adquirir` | `Infrastructure/Workflow/Terminar/MySqlTransicionConcurrencyGuard.vb` | Infrastructure | Contexto, tarea, token → lease | Obtiene/libera `GET_LOCK` por tarea y versión, sin escribir estados. | MySQL solamente. |
| `WorkflowLegacyExecutorAdapter.Ejecutar` | `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb` | Legacy adapter | Contexto, tarea, destino → `ResultadoEjecucionWorkflow` | Único límite de ejecución efectiva y normalización inicial del retorno. | Única llamada nueva a `Terminar_Tarea_Workflow`. |
| `WorkflowLegacyAuditoriaAdapter.Registrar` | `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` | Infrastructure | `AuditoriaTransicion` | Escribe una traza adicional, sin propagar errores. | `InicioWorkflow.Insertando_Datos_Log`. |

## Secuencia de dependencias

El ASMX no conoce SQL ni llama a `ClassWorkflow`. El servicio depende de puertos (`ITareaWorkflowRepository`, `ITransicionEjecucionRepository`, `IRequisitosTransicionRepository`, `ITransicionConcurrencyGuard`, `IAuditoriaTransicionRepository` e `IWorkflowLegacyExecutor`). Infrastructure los implementa y el adaptador es el único que toca el motor.

Ver [componentes Mermaid](Diagramas/01-componentes.mmd).

## Decisiones y alternativas descartadas

| Decisión | Razón |
| --- | --- |
| Releer dentro del lock | El preview puede quedar vencido entre mostrar el destino y enviarlo. |
| `GET_LOCK` MySQL, no lock en memoria | Protege más de una instancia IIS sin abrir una segunda transacción de negocio. |
| `Page = Nothing` y actualización de interfaz en `0` | El ASMX no debe manipular controles Web Forms. |
| Preparación mínima de sesión | Se habilitan permisos y eventos sin ejecutar `InicializaSesionModuloWorkflow`, que registra y compone estado de página. |
| Activar por gate | El camino nuevo nace apagado y el camino Web Forms no cambia. |

Se descartó ejecutar desde JavaScript, ASMX o Application, reimplementar cambios de estado, crear repositorio genérico y usar el destino publicado en el preview.

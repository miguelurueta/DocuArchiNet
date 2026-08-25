# Arquitectura y componentes

DOC-32 introduce una capacidad de servidor aislada para devolver una tarea Workflow a una actividad anterior. Preview y ejecución pertenecen a la misma capacidad porque comparten contexto autenticado, semántica contextual de la arista y token de versión. No se entrega UI, no se activa un gate y no se cambia configuración de ambiente.

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`

| Capa | Componentes implementados o reutilizados | Responsabilidad |
| --- | --- | --- |
| Presentation | Sin entrega en DOC-32. | El navegador expresa intención con tarea, conector y token; no autoriza destino ni contexto. |
| ASMX | `WebServiceWorkflowModern`, `WorkflowPreviewSessionContextGate` | Reconstruye sesión autenticada, calcula permiso específico y compone dependencias exclusivas. |
| Application | `ServicioDevolverActividad` | Normaliza preview, coordina lock, relectura, resolución de destino, ejecución, resultado público y auditoría. |
| Domain | Modelos, DTOs, códigos y puertos bajo `Modelo/Workflow/Devolver` y `DTOs/Workflow/Devolver` | Mantiene contratos propios, sin `Page`, `Session`, HTML, SQL ni contratos de envío. |
| Infrastructure | `MySqlDevolverActividadRepository`, `DevolverActividadCursorCodec`, `MySqlDevolverActividadConcurrencyGuard`, `WorkflowLegacyDevolverActividadExecutorAdapter` | Resuelve aristas autorizadas, protege continuación, excluye ejecuciones concurrentes y concentra el único punto mutante. |

## Fronteras y compatibilidad

`PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, Enviar a usuario, Enviar a grupo, Usuario anterior y el guard tokenizado existente permanecen independientes. DOC-32 tampoco modifica páginas, postbacks, modales, `UpdatePanel`, `GridView` ni `WorkflowCentroTrabajoModernActive`.

La conexión a la base Workflow se obtiene dentro del contexto de sesión del módulo. Los nuevos componentes no exponen ni persisten cadenas de conexión, credenciales, cookies o detalles de infraestructura.

## Semántica contextual de `IdConector`

El tipo de contexto no llega del navegador: el repositorio lo reconstruye desde el snapshot activo de la tarea, asignado al usuario autenticado.

| Contexto | Identidad de `IdConector` | Arista aceptada |
| --- | --- | --- |
| Ruta | `actividades_disponibles_envio.ID_ACTIVIDADES_DISPONIBLES_ENVIO` | Ruta y origen coinciden; la actividad siguiente coincide con la actividad actual de la tarea. |
| Flujo | Registro de conector entrante de `wf_registro_conectores_actividades_envio_flujo_trabajo` | Flujo y actividad destino actuales coinciden; el origen mantiene usuario o grupo vigente. |

El mismo valor numérico nunca cambia de semántica entre Ruta y Flujo. Cada consulta y resolución utiliza solamente la semántica que corresponde al snapshot reconstruido.

## Punto mutante y relevo

Solo `WorkflowLegacyDevolverActividadExecutorAdapter` llama a `ClassWorkflow.Terminar_Tarea_Workflow`. Recibe una tarea y destino que el servidor ya releyó y validó; nunca un DTO de cliente. La auditoría adicional se registra después de la respuesta del motor y no revierte una transición confirmada.

La etapa 02 debe consumir los DTOs ASMX y definir su experiencia accesible sin reproducir consultas, permisos, cursor, Ruta, Flujo ni la llamada legacy. Los límites están representados en [Componentes y fronteras](Diagramas/01-arquitectura-devolver.md) y [Preview y ejecución](Diagramas/02-preview-y-ejecucion.md).

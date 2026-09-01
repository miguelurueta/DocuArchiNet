# Arquitectura y decisiones aprobadas

DOC-42 mantiene las Notas dentro de Workflow y expone cuatro operaciones modernas: `CrearNota`, `ConsultarNota`, `ActualizarNota` y `EliminarNota`.

| Capa | Responsabilidad |
| --- | --- |
| ASMX | Reconstruye sesión y contexto Workflow; no acepta tarea desde sesión compartida. |
| Servicio | Valida autorización, tarea, contenido, UUID y versión. |
| Repositorio | Ejecuta preflight y mutaciones parametrizadas dentro de transacciones. |
| Persistencia | `ANOTACION_TAREA`, `wf_log_workflow`, `workflow_notas_idempotencia` y `workflow_notas_version`. |

El ETag SHA-256 se calcula en .NET sobre valores canónicos. `workflow_notas_version` conserva la versión vigente y permite que actualización y eliminación unan nota y ledger en una sola sentencia condicionada. No se usa `SHA2()` de MySQL ni se hace backfill automático de notas históricas.

No se modifican WebForms legacy, consumidores, gates, usuarios ni grupos.

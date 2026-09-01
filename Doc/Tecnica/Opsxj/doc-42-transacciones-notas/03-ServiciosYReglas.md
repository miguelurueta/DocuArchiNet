# TRANSACCIONES-NOTAS

- Ticket: DOC-42
- Cambio OpenSpec: doc-42-transacciones-notas
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

### Esquema confirmado

`ANOTACION_TAREA` está en InnoDB, conserva `Dato_Anotacion TEXT` con charset `utf8` y dispone de las claves operativas e históricas por tarea/estado/fecha/nota. `wf_log_workflow` está en InnoDB y dispone de `IX_wf_log_tarea_fecha`.

La idempotencia se persiste en `workflow_notas_idempotencia`, con unicidad `(Inicio_Tareas_Workflow_id_Tarea, Id_Usuario_Workflow, Client_Request_Id)` y expiración mediante `Fecha_Expiracion`. La implementación no debe asumir una columna `VERSION_ETAG` en esa tabla; el ETag se calcula desde la nota.

Documentar clases VB.NET/C#, reglas de negocio, dependencias y manejo de errores.

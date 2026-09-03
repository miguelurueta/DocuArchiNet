# RETIRO-LEGAZY-NOTA — Servicios y reglas

- Ticket: DOC-45
- Cambio OpenSpec: doc-45-retiro-legazy-nota
- Clasificacion: cross_cutting

## Servicios y reglas

- Se retira `Class_anotacion_tarea.Eliminar_nota_tarea_workflow`; se conserva `Eliminar_nota_service_workflow` por sus consumidores activos.
- `WebServiceWorkflowNotesModern.asmx` continúa recibiendo `idTarea` explícito.
- El listado devuelve `PuedeGestionar`, calculado con usuario autenticado y autor persistido.
- Actualización y eliminación validan tarea, actividad vigente, autor y versión en persistencia.
- Una mutación sobre nota ajena responde `NotOwner` sin modificar nota, versión ni auditoría de éxito.
- Idempotencia, concurrencia optimista y auditoría transaccional de DOC-42 permanecen vigentes.

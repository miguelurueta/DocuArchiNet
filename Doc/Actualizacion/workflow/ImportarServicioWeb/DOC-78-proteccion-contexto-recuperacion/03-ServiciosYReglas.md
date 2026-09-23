# Servicios y reglas

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Servicios y reglas

`importar-servicio-web-task-context-guard.js` captura un contexto inmutable, revalida la tarea visible y administra el bloqueo reversible. `importar-servicio-web-recovery.js` exige `IntentId` y consume exclusivamente `GetImportIntent`/`ReconcileImportIntent` por el cliente API moderno. Solo `TASK_CONTEXT_MISMATCH` y `PERSISTED_CONTEXT_MISMATCH` activan recuperación de conflicto. No se usa `localStorage`, polling ni reintento ciego.

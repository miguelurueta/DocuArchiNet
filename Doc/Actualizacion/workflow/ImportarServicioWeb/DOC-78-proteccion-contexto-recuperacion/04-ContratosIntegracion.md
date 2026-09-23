# Contratos de integración

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Contratos e integraciones

La UI captura `TaskId`, `ProviderId`, operación, identidades externas e inicio al preparar. Antes del primer efecto repite preflight y compara la tarea. El evento público `workflow:task-context-changed` es solo una señal para verificar; nunca sustituye el snapshot backend. Los documentos se proyectan únicamente cuando su `TaskId` coincide con la vista original.

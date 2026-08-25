# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`WorkflowReturnActivityUi` solicita y normaliza el preview sin inventar identidades de Ruta, Flujo o usuario. Mantiene búsqueda, cursor, páginas, cancelación, secuencia de solicitudes y selección en estado exclusivo. `WorkflowReturnActivityConfirmation` recibe la selección vigente y envía solo `idTarea`, `idConector` y `tokenVersion`.

Las reglas de negocio siguen en DOC-32: autenticación, permiso, tarea activa, destino entrante, token, cursor y lock se reconstruyen y revalidan en servidor. La UI trata los bloqueos como mensajes funcionales saneados; no expone excepciones, SQL, detalles de red, cookies, credenciales ni secretos.

Los módulos nuevos no invocan `ClassWorkflow` ni conexiones de base de datos. La presentación común recibe el `IdTarea` correlacionado para actualizar exactamente una representación después de un éxito.

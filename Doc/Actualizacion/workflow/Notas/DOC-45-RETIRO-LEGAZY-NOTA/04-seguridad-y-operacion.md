# Seguridad y operación

- La ruta moderna conserva `idTarea` explícito y no obtiene la tarea desde `Session("ID_TAREA_SELECCIONDA")`.
- La lectura de notas continúa disponible para usuarios autorizados sobre la tarea. `PuedeGestionar` se calcula en servidor con el autor persistido, el usuario autenticado y la actividad vigente; el cliente no recibe ni envía identidad para decidir propiedad.
- `UPDATE` y `DELETE` conservan sus condiciones atómicas por tarea, actividad, autor y versión. Una nota existente de otro autor responde `NotOwner` sin ejecutar auditoría de éxito.
- Las notas extensas se truncan únicamente en presentación y pueden abrirse como texto seguro en un diálogo de solo lectura con scroll interno.
- Las consultas de control E2E continúan limitadas a `SELECT` parametrizados según el runbook.
- No se habilita el gate durante pruebas estáticas o compilación.
- Estado comprobado: `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.

Toda futura corrida autenticada debe leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` y recibir autorización literal independiente.

D-07/RQ-07 quedó validado con una nota propiedad de otro usuario dentro de la misma tarea descartable autorizada. La prueba no creó ni modificó esa nota: verificó ausencia de acciones mutantes, rechazo directo `NotOwner` y versión intacta. El visor se validó con una nota propia extensa, creada temporalmente y eliminada por la misma corrida.

D-08/RQ-08 quedó validado mediante el modo `test:doc45:empty-notes`: la tarea comenzó sin notas, se creó y eliminó una nota temporal y se restauró `Nueva nota 0`. D-10/RQ-10 quedó validado seleccionando la tarea mediante la UI y operando el botón renderizado por el `UpdatePanel` sin recarga completa.

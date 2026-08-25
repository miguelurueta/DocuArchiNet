# VERIFICACION-TRANSVERSAL-DEVOLVER-TAREA

- Ticket: DOC-34
- Cambio OpenSpec: doc-34-verificacion-transversal-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`ServicioDevolverActividad` valida contexto, permiso, tarea activa y cursor para preview. En ejecución adquiere exclusión por tarea y relee dentro del lease permiso, snapshot, token y conector antes de usar el adaptador. La auditoría publica códigos funcionales y una referencia saneada; una falla posterior no revierte una transición exitosa.

`MySqlDevolverActividadRepository` resuelve Ruta y Flujo con consultas `SELECT` parametrizadas, conector entrante, filtro autorizado, orden, cursor y límite. `WorkflowLegacyDevolverActividadExecutorAdapter` es la frontera única hacia `Terminar_Tarea_Workflow`; no invoca tratamientos de respuestas ni controles Web Forms.

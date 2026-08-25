# BACKEND-ACTIVIDAD-ANTERIOR

- Ticket: DOC-32
- Cambio OpenSpec: doc-32-backend-actividad-anterior
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

- La capacidad exclusiva `Devolver` contiene modelos, DTOs, puertos, servicio, repositorio, cursor, guard y adaptador; no reutiliza contratos de envío ni de Usuario anterior.
- `PreviewDevolverActividad` construye el universo autorizado desde la tarea, usa únicamente `SELECT` parametrizados y después aplica término, límite, orden y cursor ligado a tarea, contexto y consulta.
- `EjecutarDevolverActividad` toma un lock por `IdTarea`, relee permiso, tarea, token, tipo Ruta/Flujo y conector entrante, y reconstruye el destino en servidor antes de invocar el motor.
- El adaptador `WorkflowLegacyDevolverActividadExecutorAdapter` es el único punto nuevo que invoca `Terminar_Tarea_Workflow`; desactiva actualización de interfaz y reasignaciones, preserva los eventos aprobados y evita recorridos legacy excluidos.
- Los fallos se normalizan a códigos públicos sin SQL, sesión, credenciales ni excepciones internas. La auditoría `ASMX_DEVOLVER_ACTIVIDAD` añade advertencia si falla después de confirmar la transición, sin revertirla.

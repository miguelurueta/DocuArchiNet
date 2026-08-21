# BACKEND-ENVIAR-USUARIO-WORKFLOW

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`ServicioEnvioUsuarioTarea` tiene un recorrido de preview y otro de ejecución. El preview valida contexto y permiso, consulta tarea y delega solo lecturas a `MySqlEnvioUsuarioRepository`.

La ejecución toma `MySqlTransicionConcurrencyGuard`, y dentro de su lease revalida permiso, tarea/token, ruta/flujo, destino usuario–actividad y requisitos de respuesta. `WorkflowLegacyEnvioUsuarioExecutorAdapter` es el único componente mutante y llama una vez a `Terminar_Tarea_Workflow` con `Page = Nothing` y conector cero.

`WorkflowLegacyEnvioUsuarioRequisitosAdapter` bloquea el resultado distinto de `YES` de `Verifica_respuesta_radicado_sin_respuesta`; no reasigna ni modifica respuesta. `WorkflowLegacyAuditoriaAdapter` registra el mecanismo `ASMX_ENVIO_USUARIO` y, si falla, la operación ya exitosa conserva su éxito con advertencia sanitizada.

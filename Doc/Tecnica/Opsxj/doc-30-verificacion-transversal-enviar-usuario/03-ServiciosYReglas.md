# Servicios y reglas — Verificación transversal de Enviar a usuario

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificacion: cross_cutting

## Servicios y reglas

`WebServiceWorkflowModern` compone `PreviewEnviarUsuario` y `EjecutarEnvioUsuario` con repositorio, servicio, validadores y adaptadores exclusivos. La inspección confirma permiso calculado en servidor, tarea activa, token de versión, destino usuario–actividad autorizado, requisitos de respuesta y serialización con `GET_LOCK`. El adaptador de ejecución llama el motor directo sin `After_envio_usuario_workflow` ni reasignación de respuesta; la auditoría identifica el mecanismo `ASMX_ENVIO_USUARIO` y conserva mensajes públicos sanitizados.

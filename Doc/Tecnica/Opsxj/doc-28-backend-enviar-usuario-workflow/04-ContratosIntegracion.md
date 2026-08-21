# BACKEND-ENVIAR-USUARIO-WORKFLOW

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Los endpoints ASMX autenticados por sesión son `PreviewEnviarUsuario(idTarea, consulta, cursor, tamanoPagina)` y `EjecutarEnvioUsuario(idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion)`.

ASMX no admite parámetros `Optional`: para preview los valores de negocio opcionales se envían como `null`/vacío/`0`. La respuesta contiene datos mínimos, códigos funcionales y nunca `IdConector`, `Session`, `Page`, SQL ni error técnico.

No hay cambios de esquema. La integración legacy se limita al permiso, validación de respuesta, llamada directa al motor y registro de auditoría; no usa handlers WebForms ni los flujos legacy excluidos.

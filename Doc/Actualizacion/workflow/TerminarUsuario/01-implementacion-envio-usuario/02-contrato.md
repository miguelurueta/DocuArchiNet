# Contratos, endpoints y códigos

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificación: cross_cutting

## Endpoints autenticados

| Endpoint | Payload JSON | Respuesta | Efecto |
| --- | --- | --- | --- |
| `PreviewEnviarUsuario` | `{ idTarea, consulta, cursor, tamanoPagina }` | `PrevisualizacionEnvioUsuarioDto` | Solo lectura paginada. |
| `EjecutarEnvioUsuario` | `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }` | `ResultadoEnvioUsuarioDto` | Ejecución directa controlada. |

ASMX no permite parámetros `Optional` en un `WebMethod`. El cliente de la futura etapa 02 debe enviar siempre los cuatro campos de preview: `consulta` y `cursor` admiten `null` o cadena vacía; `tamanoPagina: 0` se normaliza a 25.

Los contratos exclusivos de usuario usan `IdTarea`, `IdUsuarioWorkflowDestino`, `IdActividadDestino` y `TokenVersion` cuando aplica. No aceptan ni devuelven `IdConector`.

## Preview y datos expuestos

`PrevisualizacionEnvioUsuarioDto` publica contexto mínimo de tarea, `TokenVersion`, `TamanoPagina`, `TieneMas`, `CursorSiguiente` y destinos con identificador usuario–actividad, nombre, cargo y actividad. El cursor es opaco; se protege con `MachineKey` y no contiene una autorización reutilizable.

El filtro se limita a 100 caracteres, no acepta controles y la página se limita a 1..50. La lista se ordena establemente por nombre, usuario y actividad. Un cursor o filtro inválido devuelve un error funcional, nunca SQL o excepciones.

## Resultado de ejecución y códigos

`ResultadoEnvioUsuarioDto` contiene `Exito`, `EstadoFinal`, `CodigoBloqueo`, mensaje funcional, destino confirmado, token, requisitos, advertencias y referencia de auditoría. Los códigos relevantes son:

- `WORKFLOW_USER_SEND_FORBIDDEN`
- `WORKFLOW_USER_DESTINATION_INVALID`
- `WORKFLOW_USER_DESTINATION_UNAVAILABLE`
- `WORKFLOW_USER_SEARCH_TERM_INVALID`
- `WORKFLOW_USER_CURSOR_INVALID`
- `WORKFLOW_VERSION_INVALID` y `WORKFLOW_VERSION_CONFLICT`
- `WORKFLOW_TRANSITION_IN_PROGRESS`
- `WORKFLOW_REQUIREMENT_NOT_MET` cuando la política de respuesta no permite enviar.

Los errores se normalizan sin exponer `IdConector`, `Page`, `Session`, token completo, SQL, credenciales ni detalles de infraestructura.

# Contratos, endpoints y códigos

- Ticket: DOC-15
- Cambio OpenSpec: doc-15-base-enviar-grupo
- Clasificacion: cross_cutting

## Endpoints autenticados

| Endpoint | Payload | Respuesta | Efecto |
| --- | --- | --- | --- |
| `PreviewEnviarGrupo` | `{ idTarea }` | `PrevisualizacionEnvioGrupoDto` | Solo lectura. |
| `EjecutarEnvioGrupo` | `{ idTarea, idActividadDestino, tokenVersion }` | `ResultadoEnvioGrupoDto` | Ejecución directa controlada. |

Los contratos de grupo usan `IdTarea`, `IdActividadDestino` y `TokenVersion`; no contienen `IdConector`.

## Códigos funcionales relevantes

- `WORKFLOW_ROUTE_CHANGE_FORBIDDEN`
- `WORKFLOW_GROUP_DESTINATION_INVALID`
- `WORKFLOW_GROUP_DESTINATION_UNAVAILABLE`
- `WORKFLOW_APPROVAL_PENDING`
- `WORKFLOW_VERSION_INVALID` y `WORKFLOW_VERSION_CONFLICT`
- `WORKFLOW_TRANSITION_IN_PROGRESS`

Los errores se normalizan sin exponer SQL, Session, token, credenciales ni excepciones internas.

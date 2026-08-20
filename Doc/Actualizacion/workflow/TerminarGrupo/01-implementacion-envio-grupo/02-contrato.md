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

## DOC-26 — Búsqueda paginada

| Endpoint | Payload | Respuesta | Efecto |
| --- | --- | --- | --- |
| `BuscarDestinosEnvioGrupo` | `{ idTarea, termino, pagina, tamanoPagina }` | `BusquedaDestinosEnvioGrupoDto` | Solo lectura paginada. |

`PreviewEnviarGrupo` conserva el payload `{ idTarea }`, pero devuelve como máximo 25 destinos y añade `Pagina`, `TamanoPagina` y `TieneMas`. La búsqueda devuelve los mismos metadatos, `TokenVersion` y destinos con `IdActividadDestino`, nombre de actividad y resumen de grupo; nunca devuelve `IdConector`.

El término vacío restaura la primera página. Un término no vacío debe tener entre 2 y 80 caracteres. Página menor que uno se normaliza a uno y el tamaño se normaliza a 1..50, con 25 como valor inicial. `WORKFLOW_GROUP_SEARCH_TERM_INVALID` representa un término de longitud inválida.

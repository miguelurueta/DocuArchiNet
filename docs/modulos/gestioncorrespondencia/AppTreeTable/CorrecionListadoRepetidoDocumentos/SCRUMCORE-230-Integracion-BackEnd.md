# SCRUMCORE-230 — Integración FE-BE (BackEnd)

## 1) Source of truth: gabinete por tarea
**Endpoint**
- `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`

**Cliente FE**
- `src/modules/gestionCorrespondencia/services/solicitaGabineteRadicadoWorkflow.service.ts`
  - `getSolicitaGabinetePorTareaWorkflow(idTareaWf)`

**Campos consumidos por FE**
- `NombreGabinete` (string)
- `Radicado` (string)
- `EstadoExistenciaRadicado` (string; validación FE usa `"NO"` case-insensitive)
- `IdTareaWorkflow` (number; opcional)

**Reglas contractuales FE**
- El `Radicado` que se usa para filtrar documentos proviene únicamente de este endpoint.
- Si el `Radicado` está vacío (luego de `trim()`), el FE NO consulta el listado documental.
- Si `EstadoExistenciaRadicado` indica no-existencia (valor `"NO"`), el FE NO consulta el listado documental.

## 2) Listado documental: ListaDocumentosRadicados/query
**Endpoint**
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`

**Cliente FE**
- `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`
  - `queryListaDocumentosRadicados(payload)`

## 3) Payload FE (root query)
**Builder**
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
  - `buildListaDocumentosRadicadosRootQuery({ idTareaWf, nombreGabinete, radicado })`

**Campos relevantes enviados por FE**
- `NombreGabinete`: se toma del gabinete por tarea (si existe).
- `CampoRadicado`: siempre `"ENLASE"`.
- `Radicado`: se envía como `trim()` del radicado retornado por gabinete.
- `Search`: NO se usa como sustituto silencioso de `Radicado`.

## 4) Payload FE (children query / jerárquico)
**Builder**
- `buildListaDocumentosRadicadosChildrenQuery({ nombreGabinete, radicado, parentRowId, parentNodeType, level })`

Reglas:
- `radicado` se mantiene para no “desanclar” el árbol de la tarea actual.

## 5) Matriz FE-BE (decisiones)
| Caso | Gabinete (`Radicado`) | `EstadoExistenciaRadicado` | FE consulta `/query` | Resultado FE |
|---|---|---|---|---|
| A | vacío / whitespace | (cualquiera) | NO | error funcional: radicado obligatorio |
| B | válido | `"NO"` | NO | error funcional: radicado no existe |
| C | válido | distinto de `"NO"` | SÍ | listado filtrado por radicado |

## 6) No cambios contractuales
Este ticket:
- NO cambia endpoints.
- NO cambia DTOs del backend.
- Solo endurece validación y garantiza que el request incluya filtro real por radicado.


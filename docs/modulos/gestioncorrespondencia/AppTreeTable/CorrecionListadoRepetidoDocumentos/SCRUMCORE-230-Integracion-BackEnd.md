# SCRUMCORE-230 — IntegraciÃ³n FE-BE (BackEnd)

## Endpoint source of truth (gabinete por tarea)
- `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`

Campos usados por FE:
- `NombreGabinete`
- `Radicado`
- `EstadoExistenciaRadicado`

## Endpoint listado documentos
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`

Campos relevantes enviados por FE:
- `NombreGabinete` (si existe)
- `CampoRadicado = "ENLASE"`
- `Radicado = <trim>`

Reglas:
- FE NO ejecuta query si `Radicado` vacÃ­o.
- FE NO ejecuta query si `EstadoExistenciaRadicado` bloquea (ej. `"NO"`).


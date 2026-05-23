# SCRUMCORE-230 — ImplementaciÃ³n Detallada

## Archivos tocados
- `src/modules/gestionCorrespondencia/types/solicitaGabineteRadicadoWorkflow.types.ts`
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`

## Mapper (query)
El request root/children a `ListaDocumentosRadicados/query` incluye:
- `CampoRadicado = "ENLASE"`
- `Radicado = <trim>`

No se usa `Search` como fallback silencioso de `Radicado`.

## Hook (validaciÃ³n y anti-stale)
`load()`:
- resuelve gabinete por `idTareaWf`
- valida `Radicado` obligatorio (si falta -> error controlado y NO query)
- valida `EstadoExistenciaRadicado` (si `"NO"` -> error controlado y NO query)
- evita aplicar resultados stale mediante secuencia (`loadSeqRef`)

Al cambiar `idTareaWf`:
- limpia refs y estado (rows/columns/selecciÃ³n)


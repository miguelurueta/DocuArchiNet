# SCRUMCORE-230 — Metadata

- Ticket: `SCRUMCORE-230`
- Fecha: 2026-05-23
- Rama (feature): `feature/SCRUMCORE-230`
- PR: `https://github.com/miguelurueta/DocuArchiCore.react/pull/261`
- Merge commit (main): `1daa3a01d35a2503d1228f4302b8e04dd4977071` (2026-05-23)

## Resumen ejecutivo
- Se corrigió el bug de “listado documental repetido” asegurando filtro **obligatorio** por `Radicado`.
- Se implementó validación estricta de `Radicado` y `EstadoExistenciaRadicado` antes de consultar `ListaDocumentosRadicados/query`.
- Se implementó estrategia anti-stale ante cambios de tarea (concurrencia) y se evitó el “flash” de error por cancelación de carga.

## Artefactos (docs)
- `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/SCRUMCORE-230-Arquitectura.md`
- `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/SCRUMCORE-230-Implementacion-Detallada.md`
- `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/SCRUMCORE-230-Integracion-BackEnd.md`
- `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/SCRUMCORE-230-Pruebas.md`

## Commits (orden cronológico inverso)
- `05e846f` SCRUMCORE-230: add PR link to metadata
- `ceebfd6` SCRUMCORE-230: documentacion detallada del fix
- `c151c2b` SCRUMCORE-230: no mostrar error al cancelar load
- `219a9cf` SCRUMCORE-230: close remaining tasks (playwright + docs)
- `339ef7c` SCRUMCORE-230: update metadata commit list
- `ce5717c` SCRUMCORE-230: tests UI + docs enterprise
- `a178f7a` SCRUMCORE-230: filter documentos by radicado + anti-stale
- `e74da79` SCRUMCORE-230: align tasks with prompt
- `a58e9ad` SCRUMCORE-230: refine design/spec/tasks
- `abccdad` feat(SCRUMCORE-230): proposal inicial OpenSpec

## Confirmaciones (MUST)
- Backend NO modificado.
- Endpoints NO modificados.
- Contratos BE/DTOs NO alterados.
- `AppTable`/`AppTreeTable` NO modificados globalmente.
- Sin uso de `any` en código productivo.


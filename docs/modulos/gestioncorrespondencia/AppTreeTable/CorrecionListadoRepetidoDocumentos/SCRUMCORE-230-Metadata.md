# SCRUMCORE-230 — Metadata

- Ticket: `SCRUMCORE-230`
- Fecha: 2026-05-23
- Rama: `feature/SCRUMCORE-230`

## Resumen
- Filtrado estricto por `Radicado` (source of truth: gabinete por tarea).
- Validación `EstadoExistenciaRadicado` antes de consultar `ListaDocumentosRadicados/query`.
- Estrategia anti-stale al cambiar `idTareaWf` (limpieza + ignorar responses viejas).

## PR / Commits
- PR: (pendiente)
- Commits (orden cronológico inverso):
  - `ce5717c` SCRUMCORE-230: tests UI + docs enterprise
  - `a178f7a` SCRUMCORE-230: filter documentos by radicado + anti-stale
  - `e74da79` SCRUMCORE-230: align tasks with prompt
  - `a58e9ad` SCRUMCORE-230: refine design/spec/tasks
  - `abccdad` feat(SCRUMCORE-230): proposal inicial OpenSpec

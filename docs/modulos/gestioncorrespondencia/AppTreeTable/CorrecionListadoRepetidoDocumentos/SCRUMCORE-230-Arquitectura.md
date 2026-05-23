# SCRUMCORE-230 — Arquitectura (Filtrado por Radicado + Anti-stale)

## Objetivo
Corregir el bug donde `DocumentosWorkbench` puede mostrar el mismo set documental entre tareas distintas por ausencia de filtro efectivo por `Radicado`, y endurecer la validaciÃ³n del contexto antes de consultar documentos.

## Restricciones (MUST)
- No modificar backend, endpoints ni contratos.
- No romper `AppTreeTable`/`AppTable`.
- No romper selecciÃ³n mÃºltiple, `loadChildren` ni `ver_documento`.
- No usar `any`.

## Source of Truth
El `Radicado` vÃ¡lido se obtiene exclusivamente de:
- `getSolicitaGabinetePorTareaWorkflow(idTareaWf)`

## Anti-stale
Al cambiar `idTareaWf`:
- limpiar estado derivado del listado
- invalidar loads en vuelo
- evitar aplicar respuestas antiguas al estado actual


# SCRUMCORE-231 - Metadata

- Ticket: **SCRUMCORE-231**
- Tipo: Refactor de lifecycle / aislamiento de estado por task id
- Autor: Equipo Codex (operación guiada por usuario)
- Fecha: 2026-06-05
- Versión: 1.0.2

## Control de cambios
- 2026-06-05: Aplicación de `key`-based remount en ruta de detalle (`detailPanelKey` por `parsedId`).
- 2026-06-05: Pruebas de remount y estado local en `GestionCorrespondenciaRoute.spec.test.tsx`.
- 2026-06-05: Inclusión de navegación rápida consecutiva (`924 -> 925 -> 926`) para validar lifecycle.
- 2026-06-05: Limpieza de estado local entre tareas (`editor`-like state) en pruebas.
- 2026-06-05: Ejecución adicional de suite de tests de `gestionCorrespondencia` para hardening de regresión.
- 2026-06-05: Refinamiento de tasks OpenSpec y documentación enterprise.

## Referencias cruzadas
- SCRUMCORE-219 — Normalización idRespuestaRadicado
- SCRUMCORE-220 — Contexto transversal documental
- SCRUMCORE-221 — Documento tabla consume contexto gabinete
- `docs/modulos/gestioncorrespondencia/remount-gestion-respuesta-por-tarea/*`
- `openspec/changes/scrumcore-231-remount-gestion-respuesta-por-tarea/tasks.md`

## Estado de evidencia
- Pruebas automatizadas ejecutadas:
  - `GestionCorrespondenciaRoute.spec.test.tsx`: 1 archivo, 15 tests, 0 fallos.
  - `gestionCorrespondenciaTableRequestMapper.test.ts`: 1 archivo, varios casos.
  - `solicitaEstructuraRespuestaIdTarea.service.test.ts`: 1 archivo, casos de mapping/servicio.
  - `workflowInboxAutocomplete.service.test.ts`: 1 archivo, casos de servicio autocomplete.
- Lint ejecutado: `npx eslint` sobre archivos tocados sin hallazgos.
- Regresión transversal e interacción completa: **pendiente** (requiere QA/e2e en entorno completo).
- Build global: errores preexistentes en otros módulos fuera de este cambio en pruebas globales de lint (no atribuibles al cambio actual).

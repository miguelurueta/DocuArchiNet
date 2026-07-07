# SCRUMCORE-295 - Integracion Lista Documentos AppTreeTable

## Indice Enterprise

Este paquete documenta la implementacion frontend de `SCRUMCORE-295` para el listado de documentos radicados en `DocumentosWorkbench` con `AppTreeTable`.

La solucion final lista todos los documentos principales del radicado sin paginacion interactiva, usa `DocumentRelationScope=documentsOnly`, solicita el dataset completo con `EnablePagination=false` y aplica busqueda local deterministica sobre las filas recibidas.

## Documentos Del Paquete

| Documento | Proposito |
|---|---|
| [01-arquitectura.md](01-arquitectura.md) | Alcance, objetivos, no objetivos, decisiones y responsabilidades por archivo. |
| [02-contrato-api.md](02-contrato-api.md) | Endpoint, request, response, headers, claims, totales y compatibilidad del contrato. |
| [03-busqueda-lista-completa.md](03-busqueda-lista-completa.md) | Carga completa, busqueda local, normalizacion, contador filtrado y razones de diseno. |
| [04-ui-componentes.md](04-ui-componentes.md) | Integracion `DocumentosWorkbench`, `AppTableQueryWrapper`, `AppTreeTable`, CSS y estados UI. |
| [05-pruebas-validacion.md](05-pruebas-validacion.md) | Tests, lint, build blocker conocido, criterios de aceptacion y evidencia tecnica. |
| [06-diagramas.md](06-diagramas.md) | Diagramas Mermaid de componentes, carga inicial, busqueda y decision de total. |

## Decision Final Implementada

| Area | Decision |
|---|---|
| Scope base | `DocumentRelationScope=documentsOnly`. |
| Paginacion | `EnablePagination=false` en el listado documental del workbench. |
| Page | `Page=1` cuando la paginacion esta deshabilitada. |
| PageSize | Se conserva por compatibilidad DTO; no limita filas en este flujo. |
| Search backend | `Search=""` en modo full-list para evitar recortes incorrectos antes del filtro local. |
| Search UI | Filtro local sobre `RowId`, `Values` y `Meta`. |
| UI paginacion | Oculta con `AppTableQueryWrapper showPagination={false}`. |
| Compatibilidad wrapper | `showPagination` tiene default `true`. |
| Renderer | `AppTreeTable` sigue sin reglas de negocio documentales. |

## Rutas De Artefactos OpenSpec

- `openspec/changes/scrumcore-295-lista-documentos-apptretable/proposal.md`
- `openspec/changes/scrumcore-295-lista-documentos-apptretable/design.md`
- `openspec/changes/scrumcore-295-lista-documentos-apptretable/specs/lista-documentos-apptretable/spec.md`
- `openspec/changes/scrumcore-295-lista-documentos-apptretable/specs/lista-documentos-apptretable/jira-context.md`
- `openspec/changes/scrumcore-295-lista-documentos-apptretable/tasks.md`

## Estado De Validacion

- OpenSpec: `37/37` tareas completas.
- Vitest enfocado: `55 passed`, `1 skipped`.
- ESLint enfocado: OK.
- `git diff --check`: OK.
- `npm run build`: bloqueado por deuda preexistente fuera de `SCRUMCORE-295` en `GestionRespuestaUploadDocumental.tsx`.

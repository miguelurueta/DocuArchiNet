## Tasks

## 0. Definiciones (bloqueantes)

- [x] 0.1 Confirmar integración `SCRUM-205 ListaDocumentosRadicados`: request/response/actions según prompt
- [x] 0.2 Confirmar `SPEC ID` en tests: `[SPEC:APPTREETABLE-217]` (no cambiar)
- [x] 0.3 Confirmar estrategia de identificadores: `Rows[].RowId` -> `AppTreeTableRow.id` estable y único
- [x] 0.4 Confirmar precondición: `AppTreeTable` es wrapper de `AppTable` y expone `onSelectRow` / `onCellClicked` / `onActionTriggered` (sin hacks)

## 1. Guardrails (arquitectura + no-regresión)

- [x] 1.1 Validar que `DocumentosWorkbench` preserva layout visor + rail + responsive/overlay (sin cambios estructurales)
- [x] 1.2 Asegurar que `DocumentosWorkbench` NO consume Axios ni DTOs backend directamente (solo hook/adapters)
- [x] 1.3 Asegurar TypeScript estricto (sin `any`) y sin breaking changes en `AppTable`/`AppTreeTable`/`AppVisorEmbedPdf`/`AppCollapseRail`
- [x] 1.4 Checklist A11y/UI (enterprise):
  - foco visible (tab/shift+tab)
  - navegación por teclado en tabla/acciones
  - loading/error/empty perceptibles (ARIA/live si aplica)
  - sin focus traps (overlay/rail)

## 2. Service + Contratos SCRUM-205

- [x] 2.1 Tipos request/response/action DTO en `src/modules/gestionCorrespondencia/types/*` (sin `any`)
- [x] 2.2 Service `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`:
  - query jerárquica (root/children) con: `ViewMode`, `Page`, `PageSize`, `SortDir`, `ParentRowId`, `ParentNodeType`, `Level`, `IncludeConfig`, `EnablePagination`, `EnableColumnFilters`
  - action request con: `ActionId`, `RowId`, `NodeType`, `Payload.IdDocumento`, `Payload.NombreGabinete`
- [x] 2.3 Normalizar errores a resultados consumibles por UI: `ok:false` + mensaje en español; usar `errors[0].errorMessage` si existe
- [ ] 2.4 Validar shape real de `Config`/`Columns`/`ViewMode` (SCRUM-209) y ajustar adapters si el backend difiere
  - confirmar `ViewMode: hierarchical | flatDocuments`
  - confirmar que rutas/envelope siguen estables
  - confirmar columnas presentes en `flatDocuments` (no depender de legacy)
  - confirmar claims requeridos backend (`defaulalias`, `usuarioid`) y manejo UI de `400` (error controlado)

## 3. Adapters (DTO -> UI Models)

- [x] 3.1 Request mapper `gestionRespuestaDocumentosRequestMapper.ts` (contexto pantalla -> payload query)
- [x] 3.2 Response adapter `documentosWorkbenchResponseAdapter.ts`:
  - `RowId` -> `id`
  - `Values` -> `values`
  - `Meta.HasChildren` -> `hasChildren`
  - `Meta.{NodeType,ParentId,DocumentId,NombreGabinete}` -> `meta`
  - columnas backend-driven (prioridad): `Config` -> columnas tabla / `Columns` -> keys / fallback inferencia
- [x] 3.3 Action mapper `documentosWorkbenchActionMapper.ts`:
  - construir `Action request` desde `rowId + meta`
  - cubrir `ver_documento`
- [x] 3.3.1 SCRUM-209 flatDocuments (enterprise):
  - asegurar que el label principal se toma del valor provisto por backend (p.ej. `TIPODOCUMENTO`) sin recalcular `DOC {ID}`
  - evitar render de columnas irrelevantes/legacy en `flatDocuments` (preferir label + acciones)
- [x] 3.4 Acciones secundarias (enterprise, backend-driven):
  - soportar `onActionTriggered` para ActionIds genéricos (no solo `ver_documento`)
  - construir ActionRequest SCRUM-205 desde `meta` (RowId/NodeType/DocumentId/NombreGabinete)
  - reutilizar flujo Dynamic UI existente (`client_event`/mapeos) sin duplicar lógica
  - definir comportamiento de respuesta: refresh/reload nodo si aplica, o side-effects (p.ej. visor) sin acoplar a DTO
- [x] 3.5 Unit tests obligatorios (tag `[SPEC:APPTREETABLE-217]`): request mapper, response adapter, action mapper

## 4. Hook (orquestación) + Workbench wiring

- [x] 4.1 Hook `useGestionRespuestaDocumentosTable.ts`:
  - `load` / `loadChildren`
  - caching/estados
  - expone columnas backend-driven para `AppTreeTable`
- [x] 4.2 `DocumentosWorkbench.tsx`:
  - reemplazar hardcode
  - mantener layout visor izquierda + rail derecha + overlay intacto
  - wiring `AppTreeTable` con `load/loadChildren`
- [x] 4.3 Eventos primarios:
  - click/selección -> `ver_documento` (vía `onSelectRow`/`onCellClicked`)
- [x] 4.4 Eventos secundarios:
  - menú dinámico -> `onActionTriggered` -> ejecutar `ActionId` backend-driven
  - MUST NOT hardcodear acciones en `DocumentosWorkbench`
  - MUST NOT duplicar lógica Dynamic UI (reusar mappers/`client_event`)
  - MUST mantener selección y no romper scroll/layout
- [x] 4.5 Integración visor:
  - actualizar documento activo solo si `ver_documento` OK
  - fallback cuando no hay documento
- [x] 4.6 Estados UI (panel): loading/empty/error en español + retry visible
- [x] 4.7 Performance:
  - handlers memoizados
  - evitar recreación innecesaria de rows/columns
  - no jitter de scroll/layout; estabilidad visor/rail

## 7. Criterios enterprise (los 8 puntos del prompt)

- [x] 7.1 Precondición AppTreeTable: `onSelectRow` + `onCellClicked` + `onActionTriggered` + wrapper `AppTable` (bloqueante)
- [x] 7.2 Menú secundario dinámico backend-driven: render + trigger + ejecución ActionId + side-effects (sin hardcode, sin duplicar Dynamic UI)
- [x] 7.3 Visor PDF: `ver_documento` -> resolver `fileUrl` -> actualizar visor; en error no cambia activo; fallback sin documento
- [x] 7.4 Contrato SCRUM-205: request/response/action con campos obligatorios (query + meta + payload)
- [ ] 7.4.1 Compatibilidad SCRUM-209: `flatDocuments` simplificado + label backend + no columnas legacy asumidas
  - rutas `query/action/resolve` estables
  - envelope `AppResponses<T>` estable
  - header `Authorization` obligatorio y manejo de `400` por claims
- [x] 7.5 Clean Architecture: Workbench orquestador; hook/service/adapters; NO axios/DTO en UI
- [x] 7.6 Estados UX enterprise: loading/empty/error/retry en español; `errors[0].errorMessage` priorizado
- [x] 7.7 A11y/teclado: navegación keyboard-friendly + focus visible + acciones accesibles
- [x] 7.8 Performance: memoización, estabilidad visual, no re-render masivo/jitter, rail/visor estables

## 5. Tests & Calidad

- [x] 5.1 Unit tests (tag `[SPEC:APPTREETABLE-217]`) ejecutados
- [x] 5.2 Integration test `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (render + selección + visor + action triggered mock)
- [x] 5.3 Browser interaction (manual) documentado: click documento, expand/collapse, menú, retry, responsive, foco
- [ ] 5.4 E2E Playwright: carga real, `ver_documento` actualiza visor, menú secundario, responsive (bloqueado por env vars en entorno actual)
- [x] 5.5 Regresión: smoke de `AppTreeTable` y `AppTable` (tests relevantes + verificación mínima)
- [x] 5.6 Evidencia (comandos + salida) registrada en documentación

## 6. Documentación obligatoria

- [x] 6.1 `docs/modulos/gestioncorrespondencia/`:
  - `SCRUMCORE-217-Arquitectura.md`
  - `SCRUMCORE-217-Implementacion-Detallada.md`
  - `SCRUMCORE-217-Integracion-BackEnd.md`
  - `SCRUMCORE-217-Pruebas.md`
  - `SCRUMCORE-217-Metadata.md`
- [x] 6.2 Diagramas Mermaid requeridos + trazabilidad a código
- [x] 6.3 Mantener `proposal.md` / `design.md` / tasks alineados al prompt (guardrails + SCRUM-205 + UI wiring)

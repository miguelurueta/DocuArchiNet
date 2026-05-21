## Tasks

## 0. Definiciones (bloqueantes)

- [x] 0.1 Confirmar integración `SCRUM-205 ListaDocumentosRadicados`: request/response/actions (ver “Contrato backend obligatorio” del ticket)
- [x] 0.2 Confirmar `SPEC ID` en tests: `[SPEC:APPTREETABLE-217]` (no cambiar)
- [x] 0.3 Confirmar estrategia de identificadores: `RowId` backend -> `AppTreeTableRow.id` (string) estable y único a través del árbol

## 1. Baseline & Safety

- [x] 1.1 Revisar `DocumentosWorkbench` actual: layout visor + rail + responsive (no cambiar estructura)
- [x] 1.2 Identificar fuente actual del árbol/lista (vacía/hardcode) y puntos de wiring con visor
- [x] 1.3 Validar precondición: `AppTreeTable` expone y soporta eventos requeridos sin hacks (`onSelectRow`, `onCellClicked`, `onActionTriggered`) y render interno basado en `AppTable`
- [x] 1.4 Asegurar scope: cambios confinados a `src/modules/gestionCorrespondencia/**` (hooks/services/adapters/types/tests/docs) y NO romper `AppTable`, `AppTreeTable`, `AppVisorEmbedPdf`, `AppCollapseRail`

## 2. Backend Contracts (SCRUM-205) y Service

- [x] 2.1 Crear tipos request/response/action DTO en `src/modules/gestionCorrespondencia/types/*` (sin `any`)
- [x] 2.2 Implementar `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts` (Axios) para:
  - query jerárquica (root/children) con campos requeridos: `ViewMode`, `Page`, `PageSize`, `SortDir`, `ParentRowId`, `ParentNodeType`, `Level`, `IncludeConfig`, `EnablePagination`, `EnableColumnFilters`
  - action request con campos requeridos: `ActionId`, `RowId`, `NodeType`, `Payload.IdDocumento`, `Payload.NombreGabinete`
- [x] 2.3 Normalizar errores a resultados consumibles por UI (sin throw hacia el componente): `ok:false` + `message` en español; cuando exista `errors[0].errorMessage` usarlo
- [x] 2.4 Garantizar que `DocumentosWorkbench` NO consume Axios ni DTOs directamente (solo hook/adapters)

## 3. Adapters (SCRUM-205 -> UI Models)

- [x] 3.1 Implementar `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts` (contexto pantalla -> payload query SCRUM-205)
- [x] 3.2 Implementar `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`:
  - `Rows[].RowId` -> `AppTreeTableRow.id`
  - `Rows[].Values` -> `AppTreeTableRow.values`
  - `Rows[].Meta.HasChildren` -> `AppTreeTableRow.hasChildren`
  - `Rows[].Meta.*` relevante -> `AppTreeTableRow.meta` (incluye `NodeType`, `ParentId`, `DocumentId`, `NombreGabinete`)
  - `label` derivado: usar `Values` segun metadata/config (o fallback consistente si backend no define label)
- [x] 3.3 Implementar `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`:
  - `ActionId` -> callbacks UI (`ver_documento` y menu secundario) sin duplicar lógica Dynamic UI
  - construir `Action request` con `RowId/NodeType/Payload` desde `meta`
- [x] 3.4 Agregar pruebas unitarias obligatorias para request mapper, response adapter y action mapper con `[SPEC:APPTREETABLE-217]`

## 4. Integración `DocumentosWorkbench -> AppTreeTable`

- [x] 4.1 Crear `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts` para:
  - orchestration query SCRUM-205 (root + children)
  - exponer `load/loadChildren`, `columns/metadata`, `rowActions/menuActions` (backend-driven)
  - exponer handlers memoizados
- [x] 4.2 Reemplazar árbol estático en `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`:
  - mantener layout visor izquierda + rail derecha + overlay mobile intacto
  - conectar `AppTreeTable` con `load/loadChildren`
- [x] 4.3 Wiring de eventos:
  - click principal ejecuta `ver_documento` (vía `onSelectRow`/`onCellClicked` según corresponda)
  - menú secundario dinámico usa `onActionTriggered` y `client_event` existente (sin duplicar Dynamic UI)
- [x] 4.4 Integración visor:
  - actualizar documento activo solo si `ver_documento` OK
  - mapear `fileUrl`, `DocumentId`, `NombreGabinete`
  - mantener fallback cuando no hay documento activo
- [x] 4.5 Reglas de errores (panel):
  - loading visible, empty en español, error con retry visible; success=false muestra `errors[0].errorMessage`
- [x] 4.6 Performance/estabilidad:
  - no re-render completo del workbench
  - no recrear `rows` innecesariamente; handlers memoizados
  - no jitter de scroll/layout, no romper rail
- [x] 4.7 Validar compatibilidad con `SCRUM-205` y preservación de layout (visor + rail)

## 5. Tests & Evidencia

- [x] 5.1 Unit tests obligatorios (con `[SPEC:APPTREETABLE-217]`):
  - request mapper
  - response adapter (Rows -> TreeRows / `AppTreeTableRow`)
  - action mapper (incluye `ver_documento`)
- [x] 5.2 UI integration tests obligatorios en `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`:
  - render workbench
  - loading/error/empty
  - integración `AppTreeTable` (expand/collapse, selección)
  - menú dinámico / action triggered
  - integración visor (documento activo)
- [ ] 5.3 Browser interaction tests (manual checklist) documentados: click documento, expand/collapse, menú, retry error, responsive/focus
- [ ] 5.4 E2E (Playwright) obligatorios: carga real, `ver_documento` actualiza visor, menú secundario, responsive intacto
- [ ] 5.5 Regresión: asegurar `AppTreeTable` y `AppTable` siguen funcionando (tests existentes + smoke)
- [x] 5.6 Registrar evidencia de ejecución (comandos + salida relevante) en documentación del cambio

## 6. Documentación

- [x] 6.1 Crear carpeta `docs/modulos/gestioncorrespondencia/` (si no existe) y agregar:
  - `docs/modulos/gestioncorrespondencia/SCRUMCORE-217-Arquitectura.md`
  - `docs/modulos/gestioncorrespondencia/SCRUMCORE-217-Implementacion-Detallada.md`
  - `docs/modulos/gestioncorrespondencia/SCRUMCORE-217-Integracion-BackEnd.md`
  - `docs/modulos/gestioncorrespondencia/SCRUMCORE-217-Pruebas.md`
  - `docs/modulos/gestioncorrespondencia/SCRUMCORE-217-Metadata.md`
- [x] 6.2 Incluir diagramas Mermaid requeridos (class/sequence/state) y trazabilidad a código
- [x] 6.3 Mantener `proposal.md`/`design.md` alineados al contrato backend final y guardrails

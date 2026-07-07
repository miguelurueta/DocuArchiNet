## 1. Refinement

- [x] 1.1 Lock the request contract for `DocumentRelationScope`, `EnablePagination`, `Page`, and `PageSize` against the current backend query flow.
- [x] 1.2 Confirm the implementation stays localized to `gestionCorrespondencia` and does not alter global `AppTable` / `AppTreeTable` behavior.
- [x] 1.3 Keep the default behavior for omitted scope as `documentsOnly`.

## 2. Contract And Mapping

- [x] 2.1 Update `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts` so root and children queries inject the same default scope and pagination policy.
- [x] 2.2 Preserve the existing backend query keys already consumed by `listaDocumentosRadicados.service.ts`.
- [x] 2.3 Keep request-building behavior deterministic for refresh, tree loading, and scope changes.

## 3. Hook, Service, And Adapter

- [x] 3.1 Update `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts` to propagate backend validation states and preserve refresh/reset behavior.
- [x] 3.2 Update `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts` to preserve backend totals, rows, and table metadata without inventing totals client-side.
- [x] 3.3 Keep `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts` transport-only and contract compatible.
- [x] 3.4 Verify `DocumentosWorkbench` continues consuming the refined model without changing the current user flow.

## 4. UI Behavior

- [x] 4.1 Preserve page state when only `Page` changes.
- [x] 4.2 Reset `Page` to `1` when `DocumentRelationScope`, `Radicado`, `CampoRadicado`, `NombreGabinete`, or filter context changes.
- [x] 4.3 Keep validation errors visible without silent fallback to another scope.
- [x] 4.4 Confirm the current tree/list rendering path remains visually stable.

## 5. Tests

- [x] 5.1 Add or update `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts` for default scope handling and scope changes.
- [x] 5.2 Add or update `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` for total mapping and row metadata preservation.
- [x] 5.3 Add or update `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` for refresh flow, state reset, and no-regression behavior.
- [x] 5.4 Add or update `src/modules/gestionCorrespondencia/tests/gestionCorrespondenciaTableRequestMapper.test.ts` only if shared request mapping needs to stay aligned with the new contract.
- [x] 5.5 Run the affected `vitest` suite and capture the exact commands used.

## 6. Documentation And Publish

- [x] 6.1 Create or update the technical package under `docs/Architecture/GestionCorrrespondecia/Integracion-ListaDocumentos-AppTreeTable/`.
- [x] 6.2 Document the request contract, UI scope rules, totals behavior, validation behavior, and compatibility notes.
- [x] 6.3 Review the final diff for consistency between Jira, OpenSpec, code, and documentation before publish.

## 7. Interactive Pagination Hardening - Superseded By Full-List UX

- [x] 7.1 Extend the document-list request mapper so `Page` and `PageSize` are supplied by the domain query state instead of being fixed constants.
- [x] 7.2 Expose server-side pagination state from `useGestionRespuestaDocumentosTable` without adding business rules to `AppTable` or `AppTreeTable`.
- [x] 7.3 Wrap the workbench tree with the existing `AppTableQueryWrapper` so page navigation reuses shared UI controls.
- [x] 7.4 Keep `AppTreeTable` as a reusable renderer over `AppTable`; do not add document scope, radicado, gabinete, total, offset, or attachment rules to the component.
- [x] 7.5 Add focused regression tests for request `Page/PageSize`, page changes, scope reset, and workbench pagination delegation.

Note: this phase was implemented and then intentionally evolved after functional validation. The final SCRUMCORE-295 UX lists the complete document set and hides interactive pagination in `DocumentosWorkbench`.

## 8. Final Full-List Search UX Alignment

- [x] 8.1 Configure `useGestionRespuestaDocumentosTable` so the workbench document list sends `EnablePagination=false` for root, child, refresh, and mutation reload flows.
- [x] 8.2 Keep `Page=1` when pagination is disabled and preserve `PageSize` only for DTO compatibility.
- [x] 8.3 Add `showPagination?: boolean` to `AppTableQueryWrapper` with default `true` so existing consumers remain unaffected.
- [x] 8.4 Render `DocumentosWorkbench` with `showPagination={false}` and no wrapper refresh button.
- [x] 8.5 Compact the document-list search input with CSS scoped to `DocumentosWorkbench`.
- [x] 8.6 Implement deterministic local search over `RowId`, `Values`, and `Meta` when the complete dataset is loaded.
- [x] 8.7 Avoid backend `Search` in the full-list workbench flow so the API cannot hide rows before local filtering.
- [x] 8.8 Use filtered row count as total when local search is active.
- [x] 8.9 Add focused regression tests for hidden pagination, `showPagination`, complete-list requests, and local search.
- [x] 8.10 Update OpenSpec and enterprise documentation with SCRUM ID, final decisions, diagrams, validation commands, and known build blocker.
- [x] 8.11 Split the enterprise documentation package into index, architecture, API contract, full-list search, UI/components, validation, and diagrams documents.

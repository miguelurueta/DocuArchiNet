# Design

## Context

SCRUMCORE-295 formalizes the frontend contract for the document list rendered in `DocumentosWorkbench` / `AppTreeTable`. The current module already owns the query flow, row mapping, search state, row mapping, and refresh lifecycle, so the change stays inside `gestionCorrespondencia` instead of pushing document business rules into `AppTable` or `AppTreeTable`.

## Goals

- Make attachment scope explicit in the request contract.
- Preserve the existing tree/list behavior for current consumers.
- Keep the document list deterministic by loading the complete dataset for the main workbench list.
- Hide interactive pagination in this specific document-list surface.
- Use backend totals when no local search is active and use filtered row count when local search is active.
- Make search deterministic over the complete dataset received by the UI.
- Surface validation errors instead of masking them with a silent fallback.

## Non-Goals

- Do not redesign `AppTable` as a generic paging engine.
- Do not change unrelated consumer modules.
- Do not infer attachment membership from file name, extension, or label text.
- Do not introduce a new screen or new navigation surface.
- Do not rely on backend `Search` for the full-list workbench flow when that can hide expected rows before the UI receives the complete dataset.

## Decisions

1. Keep the change localized to `src/modules/gestionCorrespondencia`.
   - The request mapper, hook, service, and response adapter already form the natural seam.
   - `AppTable` and `AppTreeTable` remain reusable primitives unless a regression forces a shared fix.

2. Extend the query contract with explicit scope and pagination controls.
   - `DocumentRelationScope` becomes the way to request `documentsOnly`, `includeResponseAttachments`, or `responseAttachmentsOnly`.
   - The final `DocumentosWorkbench` document list sends `EnablePagination=false` for root loads and child loads.
   - `Page` is normalized to `1` when pagination is disabled; `PageSize` remains in the payload for DTO compatibility.
   - The mapper keeps compatible defaults for other call sites; the domain hook owns the document-list policy.
   - When a scope is not supplied, the default behavior remains `documentsOnly`.

3. Keep totals backend-aware and search-aware.
   - Prefer `meta.total`.
   - Support `meta.Total` for PascalCase backend metadata.
   - Fall back to `data.pagination.total` when `meta.total` is unavailable.
   - Support `data.Pagination.Total` for PascalCase pagination metadata.
   - When local search is active, use the filtered row count because the visible universe is intentionally narrowed by the UI.

4. Preserve the existing workbench layout and action contract.
   - `DocumentosWorkbench` keeps orchestrating the table and the viewer.
   - The action path must remain compatible with the current row-action model.
   - No extra coupling is added to `AppTreeTable`.

5. Make search deterministic in the full-list flow.
   - `AppTableQueryWrapper` owns the search input event.
   - `useGestionRespuestaDocumentosTable` stores `queryState.search`.
   - The hook loads all rows with backend `Search=""` when pagination is disabled.
   - The hook filters returned rows locally over `RowId`, `Values`, and `Meta`.
   - Search normalization removes accents and compares case-insensitively.

6. Make validation explicit.
   - Invalid scope or paging values should surface as functional validation errors.
   - The UI should not retry automatically with a different scope.
   - If `responseAttachmentsOnly` is exposed in this ticket, it follows the same validation path and defaults as the other scopes.

7. Keep shared wrapper compatibility.
   - `AppTableQueryWrapper` adds `showPagination?: boolean`.
   - The prop defaults to `true`; all existing consumers keep their current pagination controls.
   - `DocumentosWorkbench` is the specific consumer that passes `showPagination={false}`.

## Technical Approach

- `gestionRespuestaDocumentosRequestMapper` will become the single place that injects the default scope and pagination policy into root/children queries.
- `useGestionRespuestaDocumentosTable` will call the full-list load path for the initial workbench render, child loads, refreshes, and mutation reloads in the document-list surface.
- `listaDocumentosRadicados.service.ts` remains transport only.
- `documentosWorkbenchResponseAdapter.ts` will continue translating backend rows into `AppTreeTableRow` and table metadata, but should preserve any response pagination and total information exposed by the backend.
- `useGestionRespuestaDocumentosTable.ts` will coordinate loading, local search, refresh, state reset on task change, total resolution, and error propagation.
- `DocumentosWorkbench.tsx` will keep the presentation behavior stable, render a compact search input, and hide pagination controls in this surface.

## Risks / Trade-offs

- Root and child loads with `EnablePagination=false` can return larger payloads. This is accepted for the workbench document-list UX because the user explicitly requested all rows without pagination.
- If the backend omits `meta.total`, the UI must rely on `data.pagination.total`; if both are missing, the UI falls back to the visible row count.
- When local search is active, the counter intentionally reflects filtered rows rather than backend total.
- Keeping the default scope as `documentsOnly` protects compatibility, but it also means new consumers must opt in explicitly to attachment visibility.
- Backend-side `Search` remains available for future paginated flows; this workbench flow does not rely on it while pagination is disabled.

## Migration Plan

1. Update the request mapper to send the new controls.
2. Update the hook and adapter to preserve backend totals, validation states, and local search behavior.
3. Add tests for scope defaults, full-list loads, hidden pagination, local search, page resets, total mapping, and validation handling.
4. Refresh and split technical documentation under `docs/Architecture/GestionCorrrespondecia/Integracion-ListaDocumentos-AppTreeTable/`.
5. Publish the change once the spec and tasks are consistent with the implementation plan.

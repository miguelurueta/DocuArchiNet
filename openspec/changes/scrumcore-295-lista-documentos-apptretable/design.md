# Design

## Context

SCRUMCORE-295 formalizes the frontend contract for the document list rendered in `DocumentosWorkbench` / `AppTreeTable`. The current module already owns the query flow, row mapping, and refresh lifecycle, so the change should stay inside `gestionCorrespondencia` instead of pushing business rules into `AppTable`.

## Goals

- Make attachment scope explicit in the request contract.
- Preserve the existing tree/list behavior for current consumers.
- Keep pagination deterministic when the UI needs a full refresh.
- Use backend totals as the source of truth for counters and paging.
- Surface validation errors instead of masking them with a silent fallback.

## Non-Goals

- Do not redesign `AppTable` as a generic paging engine.
- Do not change unrelated consumer modules.
- Do not infer attachment membership from file name, extension, or label text.
- Do not introduce a new screen or new navigation surface.

## Decisions

1. Keep the change localized to `src/modules/gestionCorrespondencia`.
   - The request mapper, hook, service, and response adapter already form the natural seam.
   - `AppTable` and `AppTreeTable` remain reusable primitives unless a regression forces a shared fix.

2. Extend the query contract with explicit scope and pagination controls.
   - `DocumentRelationScope` becomes the way to request `documentsOnly`, `includeResponseAttachments`, or `responseAttachmentsOnly`.
   - `EnablePagination=true` is the default for the main list and tree load.
   - `EnablePagination=false` is reserved for explicit full refresh flows that must not lose rows outside the first page.
   - When a scope is not supplied, the default behavior remains `documentsOnly`.

3. Keep totals backend-driven.
   - Prefer `meta.total`.
   - Fall back to `data.pagination.total` when `meta.total` is unavailable.
   - Never derive totals from the visible page length.

4. Preserve the existing workbench layout and action contract.
   - `DocumentosWorkbench` keeps orchestrating the table and the viewer.
   - The action path must remain compatible with the current row-action model.
   - No extra coupling is added to `AppTreeTable`.

5. Make validation explicit.
   - Invalid scope or paging values should surface as functional validation errors.
   - The UI should not retry automatically with a different scope.
   - If `responseAttachmentsOnly` is exposed in this ticket, it follows the same validation path and defaults as the other scopes.

## Technical Approach

- `gestionRespuestaDocumentosRequestMapper` will become the single place that injects the default scope and pagination policy into root/children queries.
- `useGestionRespuestaDocumentosTable` will call the paginated load path for the initial workbench render and the explicit full-refresh path only after a mutation requires it.
- `listaDocumentosRadicados.service.ts` remains transport only.
- `documentosWorkbenchResponseAdapter.ts` will continue translating backend rows into `AppTreeTableRow` and table metadata, but should preserve any response pagination and total information exposed by the backend.
- `useGestionRespuestaDocumentosTable.ts` will coordinate loading, refresh, state reset on task change, and error propagation.
- `DocumentosWorkbench.tsx` will keep the presentation behavior stable and only consume the refined model.

## Risks / Trade-offs

- Full refreshes with `EnablePagination=false` can return larger payloads. That is acceptable only for the flows that require it.
- If the backend omits `meta.total`, the UI must rely on `data.pagination.total`; if both are missing, the UI falls back to the visible row count.
- Keeping the default scope as `documentsOnly` protects compatibility, but it also means new consumers must opt in explicitly to attachment visibility.

## Migration Plan

1. Update the request mapper to send the new controls.
2. Update the hook and adapter to preserve backend totals and validation states.
3. Add tests for scope defaults, page resets, total mapping, and validation handling.
4. Refresh technical documentation under `docs/Architecture/GestionCorrrespondecia/Integracion-ListaDocumentos-AppTreeTable/`.
5. Publish the change once the spec and tasks are consistent with the implementation plan.

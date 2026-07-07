## Why

SCRUMCORE-295 requires a frontend contract that can refresh the document list without losing rows to pagination and without mixing response attachments into the main document view.

Implementation note: after validating the UI flow, the workbench document list was finalized as a full-list experience. It requests the complete base-document dataset with `EnablePagination=false`, hides interactive pagination in this surface, and applies deterministic local search over the returned rows.

## What Changes

- Add an explicit `DocumentRelationScope` to control whether the query returns only documents, documents plus response attachments, or only response attachments.
- Use `EnablePagination=false` for the `DocumentosWorkbench` document list so the UI receives the complete row set for the selected scope.
- Hide interactive pagination in the document-list surface while preserving `AppTableQueryWrapper` pagination for existing consumers by default.
- Keep `Page=1` and `PageSize` in the request for DTO compatibility, but do not rely on them to limit rows when pagination is disabled.
- Treat `meta.total` / `meta.Total` as the preferred total and `data.pagination.total` / `data.Pagination.Total` as fallback when no local search is active.
- Apply local search over `RowId`, `Values`, and `Meta` after loading the complete dataset to avoid backend `Search` semantics hiding expected rows.
- Keep the existing `AppTreeTable` and `DocumentosWorkbench` integration stable and backward compatible when the scope is omitted.
- Surface validation errors from backend instead of silently forcing `documentsOnly`.

## Jira Details

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- Consumer: `DocumentosWorkbench` through `useGestionRespuestaDocumentosTable`
- Backend contract: `AppResponses<object>`
- Required request keys: `NombreGabinete`, `CampoRadicado`, `Radicado`
- Optional request controls: `DocumentRelationScope`, `EnablePagination`, `Page`, `PageSize`, `Search`, `SearchType`
- Main UI cases:
  - main document tree/list
  - refresh after storing a document or attachment
  - full related-documents view
  - attachments-only view

## Capabilities

- `lista-documentos-apptretable`: frontend contract and UI behavior for document list refresh, attachment scope, and pagination control in `AppTreeTable`.

## Impact

- Changes are localized to the document list flow in `gestionCorrespondencia`.
- Existing consumers remain compatible if they continue using the default scope and default `AppTableQueryWrapper` pagination behavior.
- The change reduces visual inconsistency, missing rows after refresh, and wrong totals in the UI.
- The final UX avoids page-size confusion by listing all rows and filtering locally.
- The implementation should ship with its technical package under `docs/Architecture/GestionCorrrespondecia/Integracion-ListaDocumentos-AppTreeTable/`.

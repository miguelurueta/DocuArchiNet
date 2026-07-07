## Why

SCRUMCORE-295 requires a frontend contract that can refresh the document list without losing rows to pagination and without mixing response attachments into the main document view.

## What Changes

- Add an explicit `DocumentRelationScope` to control whether the query returns only documents, documents plus response attachments, or only response attachments.
- Use `EnablePagination=true` for the main list and tree load, and `EnablePagination=false` only for explicit full refresh flows that must not drop rows outside the first page.
- Treat `meta.total` as the preferred total and `data.pagination.total` as fallback.
- Keep the existing `AppTreeTable` and `DocumentosWorkbench` integration stable and backward compatible when the scope is omitted.
- Surface validation errors from backend instead of silently forcing `documentsOnly`.

## Jira Details

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- Consumer: `DocumentosWorkbench` through `useGestionRespuestaDocumentosTable`
- Backend contract: `AppResponses<object>`
- Required request keys: `NombreGabinete`, `CampoRadicado`, `Radicado`
- Optional request controls: `DocumentRelationScope`, `EnablePagination`, `Page`, `PageSize`
- Main UI cases:
  - main document tree/list
  - refresh after storing a document or attachment
  - full related-documents view
  - attachments-only view

## Capabilities

- `lista-documentos-apptretable`: frontend contract and UI behavior for document list refresh, attachment scope, and pagination control in `AppTreeTable`.

## Impact

- Changes are localized to the document list flow in `gestionCorrespondencia`.
- Existing consumers remain compatible if they continue using the default scope.
- The change reduces visual inconsistency, missing rows after refresh, and wrong totals in the UI.
- The implementation should ship with its technical package under `docs/Architecture/GestionCorrrespondecia/Integracion-ListaDocumentos-AppTreeTable/`.

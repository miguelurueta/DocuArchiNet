# lista-documentos-apptretable Specification

## Purpose

Define the frontend contract and UI behavior for the document list rendered by `DocumentosWorkbench` through `AppTreeTable`, including document relation scope, full-list loading, local search, totals, validation behavior, delete warning handling, and backward compatibility for shared table primitives.

## Requirements

### Requirement: Query contract supports document scope and explicit full-list loading
The system SHALL allow the frontend consumer of `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query` to send a document-scope control and explicit pagination control without changing the endpoint route.

The request SHALL support:

- `DocumentRelationScope`
- `EnablePagination`
- `Page`
- `PageSize`
- the existing business keys `NombreGabinete`, `CampoRadicado`, and `Radicado`

The frontend SHALL keep the default behavior compatible with current consumers when `DocumentRelationScope` is omitted.

#### Scenario: Main list uses the default document scope
- **WHEN** `DocumentosWorkbench` loads the main document list
- **THEN** the request uses `documentsOnly` as the default scope
- **AND** the request sends `EnablePagination=false`
- **AND** the UI renders only base documents for the radicado

#### Scenario: Main list loads the complete row set
- **WHEN** the workbench document list loads or refreshes
- **THEN** the request sends `EnablePagination=false`
- **AND** the response is consumed as a complete dataset for the current scope
- **AND** pagination controls are not shown in the document-list surface

#### Scenario: Related-documents view includes attachments
- **WHEN** the user requests the full related-documents view
- **THEN** the request sends `DocumentRelationScope=includeResponseAttachments`
- **AND** the UI can render documents and response attachments in the same list

#### Scenario: Attachments-only view isolates attachment rows
- **WHEN** the consumer requests only response attachments
- **THEN** the request sends `DocumentRelationScope=responseAttachmentsOnly`
- **AND** the UI renders only rows related through `ra_anexos_respuesta`

### Requirement: Totals remain backend-aware and search-aware
The system SHALL use backend totals as the source of truth when no local search is active.

The frontend SHALL prefer `meta.total` or `meta.Total` and SHALL fall back to `data.pagination.total` or `data.Pagination.Total` if the meta total is not available.

The frontend SHALL use the filtered row count when local search is active because the visible dataset has been narrowed by the UI.

#### Scenario: Backend returns a total for a paginated response
- **WHEN** the backend returns `meta.total`
- **THEN** the UI uses that value for the counter when no local search is active

#### Scenario: Backend omits meta total
- **WHEN** `meta.total` is missing
- **THEN** the UI uses `data.pagination.total`
- **AND** does not infer the total from `rows.length` unless no backend total is available

#### Scenario: Local search changes visible total
- **WHEN** the user enters a search term in the workbench document list
- **THEN** the UI filters the complete row set locally
- **AND** the counter reflects the filtered rows

#### Scenario: Refresh must not hide a row outside a page
- **WHEN** the flow refreshes the document list
- **THEN** the UI keeps the returned full dataset for the chosen scope
- **AND** no row is hidden because of frontend pagination

### Requirement: Workbench search filters locally over complete rows
The system SHALL implement document-list search in the workbench over the complete set of rows returned by the backend when `EnablePagination=false`.

The frontend SHALL NOT depend on backend `Search` semantics for the full-list document workbench flow.

The local search SHALL include `RowId`, `Values`, and `Meta`.

The local search SHALL compare text case-insensitively and accent-insensitively.

#### Scenario: User searches visible document metadata
- **WHEN** the backend returns all rows for `documentsOnly`
- **AND** the user types a search term
- **THEN** the UI filters rows locally using `RowId`, `Values`, and `Meta`
- **AND** only matching rows remain visible

#### Scenario: Backend search would hide expected rows
- **WHEN** `EnablePagination=false`
- **THEN** the request sends backend `Search` as an empty string
- **AND** the UI applies the active search term after receiving the full row set

#### Scenario: User clears search
- **WHEN** the search input is cleared
- **THEN** the UI reloads or renders the complete unfiltered row set
- **AND** the counter returns to the backend total when available

### Requirement: Query context reset remains deterministic
The system SHALL preserve the active document query context while keeping `Page=1` for the full-list workbench flow.

The frontend SHALL keep:

- `NombreGabinete`
- `CampoRadicado`
- `Radicado`
- `DocumentRelationScope`
- filters
- ordering
- `PageSize` for DTO compatibility

The frontend SHALL reset `Page` to `1` when the scope or search context changes.

#### Scenario: Workbench list hides page navigation
- **WHEN** `DocumentosWorkbench` renders the document list
- **THEN** `AppTableQueryWrapper` is rendered with pagination hidden
- **AND** the UI does not expose next page, previous page, or page-size controls

#### Scenario: User changes scope
- **WHEN** the user changes `DocumentRelationScope`
- **THEN** the UI resets `Page` to `1`
- **AND** the new request is built from the current filter context

#### Scenario: User changes search
- **WHEN** the user changes the document-list search term
- **THEN** the UI resets `Page` to `1`
- **AND** the next full-list load applies the search locally

### Requirement: Validation errors are surfaced without silent fallback
The system SHALL surface backend validation failures as functional errors.

The frontend SHALL NOT silently retry the request with `documentsOnly` or any other fallback scope when the backend returns validation errors.

The UI SHALL show field-level errors when the backend provides them and SHALL keep the current user context intact.

#### Scenario: Invalid scope is rejected
- **WHEN** the backend returns a validation error for `DocumentRelationScope`
- **THEN** the UI shows the validation message
- **AND** the UI does not retry with a different scope

#### Scenario: Invalid paging values are rejected
- **WHEN** the backend rejects `Page` or `PageSize`
- **THEN** the UI shows the error
- **AND** the user can correct the current filter state

### Requirement: Delete restrictions are communicated as toast warnings only
The system SHALL present delete feature restrictions in the workbench as a transient warning toast.

The frontend SHALL NOT render a persistent inline banner, alert, or fallback panel for delete restrictions when the backend responds with a warning-style failure.

The message shown to the user SHALL be the functional backend message normalized by the UI copy layer.

#### Scenario: Delete is disabled by backend feature flag
- **WHEN** the user triggers `eliminar_item` and the backend responds with a warning-style 400
- **THEN** the UI shows a warning toast
- **AND** the toast message communicates that the delete functionality is not currently available
- **AND** no inline delete notice remains on the screen

### Requirement: Existing consumers remain backward compatible
The system SHALL preserve the current behavior for consumers that do not send the new scope control.

#### Scenario: Legacy consumer omits scope
- **WHEN** an existing consumer does not send `DocumentRelationScope`
- **THEN** the backend and frontend continue using the default document-only behavior

#### Scenario: Other table consumers remain unaffected
- **WHEN** other modules consume `AppTable` or `AppTreeTable`
- **THEN** the change in this ticket does not alter their default query or rendering behavior

#### Scenario: Shared query wrapper keeps pagination by default
- **WHEN** another consumer renders `AppTableQueryWrapper` without `showPagination={false}`
- **THEN** pagination controls remain visible
- **AND** the consumer behavior remains unchanged

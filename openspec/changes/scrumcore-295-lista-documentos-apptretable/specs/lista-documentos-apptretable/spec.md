## ADDED Requirements

### Requirement: Query contract supports document scope and explicit pagination
The system SHALL allow the frontend consumer of `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query` to send a document-scope control and pagination control without changing the endpoint route.

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
- **AND** the UI renders only base documents for the radicado

#### Scenario: Full refresh uses pagination disabled
- **WHEN** the user refreshes a flow that must not lose rows outside the first page
- **THEN** the request sends `EnablePagination=false`
- **AND** the response is consumed as a complete dataset for the current scope

#### Scenario: Related-documents view includes attachments
- **WHEN** the user requests the full related-documents view
- **THEN** the request sends `DocumentRelationScope=includeResponseAttachments`
- **AND** the UI can render documents and response attachments in the same list

#### Scenario: Attachments-only view isolates attachment rows
- **WHEN** the consumer requests only response attachments
- **THEN** the request sends `DocumentRelationScope=responseAttachmentsOnly`
- **AND** the UI renders only rows related through `ra_anexos_respuesta`

### Requirement: Totals and paging remain backend-driven
The system SHALL use backend totals as the source of truth for the table counter and page model.

The frontend SHALL prefer `meta.total` and SHALL fall back to `data.pagination.total` if the meta total is not available.

The frontend SHALL NOT derive total rows from the visible page length when the backend provides a total value.

#### Scenario: Backend returns a total for a paginated response
- **WHEN** the backend returns `meta.total`
- **THEN** the UI uses that value for the counter and paging state

#### Scenario: Backend omits meta total
- **WHEN** `meta.total` is missing
- **THEN** the UI uses `data.pagination.total`
- **AND** does not infer the total from `rows.length` unless no backend total is available

#### Scenario: Refresh after storing must not hide a new row
- **WHEN** the flow refreshes after storing a document or attachment
- **THEN** the UI keeps the returned full dataset for the chosen scope
- **AND** the row can appear even if it was not in the first page previously

### Requirement: Scope changes preserve the active query context
The system SHALL preserve the active query context when the user changes page.

The frontend SHALL keep:

- `NombreGabinete`
- `CampoRadicado`
- `Radicado`
- `DocumentRelationScope`
- filters
- ordering
- `PageSize`

The frontend SHALL reset `Page` to `1` when the scope or search context changes.

#### Scenario: User changes page
- **WHEN** the user requests the next page
- **THEN** the request keeps the same scope and filters
- **AND** only `Page` changes

#### Scenario: User changes scope
- **WHEN** the user changes `DocumentRelationScope`
- **THEN** the UI resets `Page` to `1`
- **AND** the new request is built from the current filter context

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

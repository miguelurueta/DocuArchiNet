## ADDED Requirements

### Requirement: Delete persisted document from document workbench
The system SHALL allow deleting a persisted document from the Gestion Respuesta document rail using the StorageEngine delete contract.

#### Scenario: Delete from an eligible row
- **WHEN** the user triggers `eliminar_item` on a row whose metadata allows delete
- **AND** the row resolves to a persisted document identifier
- **THEN** the frontend sends `DELETE /api/gestor-documental/eliminar-documento/{idAlmacen:long}`
- **AND** the request includes `nombreGabinete`
- **AND** the request uses `sourceModule=WORKFLOW` for this screen
- **AND** the request preserves `X-Request-Id` or the client request id when available
- **AND** the delete outcome is handled as a persisted document deletion

#### Scenario: Delete is blocked by backend policy
- **WHEN** the backend rejects the delete with a business or authorization error
- **THEN** the UI shows the backend message using the configured precedence
- **AND** the request id is preserved for diagnostics when available
- **AND** the document list remains consistent after the failed attempt

#### Scenario: Row is not deletable
- **WHEN** a row does not expose `CanDelete=true`
- **THEN** the workbench must not present the delete action as available from that row state
- **AND** the backend remains the final authority if the action is invoked through a fallback path

### Requirement: Preserve document list and active viewer state
The system SHALL keep the document rail and the active viewer in a consistent state after delete.

#### Scenario: Active document is deleted
- **WHEN** the deleted row is the active row in the viewer
- **THEN** the viewer clears the removed document
- **AND** the document list refreshes
- **AND** the active row reference is cleared

#### Scenario: Deleted row is not the active row
- **WHEN** the deleted row is not currently active
- **THEN** only the document list is refreshed
- **AND** the current viewer document remains untouched

### Requirement: Resolve backend delete errors with user-facing precedence
The system SHALL surface delete failures using the backend error envelope with a stable precedence order.

#### Scenario: Error envelope contains UserMessage
- **WHEN** the delete response includes `errors[0].UserMessage`
- **THEN** the UI uses that text as the primary user-facing message

#### Scenario: Error envelope lacks UserMessage
- **WHEN** the delete response does not include `errors[0].UserMessage`
- **AND** `errors[0].Message` is present
- **THEN** the UI uses `errors[0].Message` as the fallback message

#### Scenario: Only technical message is available
- **WHEN** the response has no `errors[]`
- **AND** `message` is present
- **THEN** the UI uses `message` only as technical fallback
- **AND** the UI still avoids rendering raw paths, SQL, or stack traces as primary text

#### Scenario: Error severity follows contract
- **WHEN** the backend returns a validation, business, unauthorized, not found, conflict, or technical delete failure
- **THEN** the UI maps severity using the prompt contract
- **AND** 400 validation and 400 business are warnings
- **AND** 401 is a warning for session/authentication
- **AND** 403 is an error
- **AND** 404 is an error
- **AND** 409 business is a warning
- **AND** 500 and unexpected failures are errors

### Requirement: Keep the current workbench action contract compatible
The system SHALL preserve the current row-action contract used by `DocumentosWorkbench` and `useGestionRespuestaDocumentosTable`.

#### Scenario: Backend still emits generic `eliminar_item`
- **WHEN** the document table emits `eliminar_item`
- **THEN** the frontend routes the action through the existing workbench action flow
- **AND** the implementation remains compatible with the current list query and row model

#### Scenario: Legacy rows omit `CanDelete`
- **WHEN** a row does not include delete metadata
- **THEN** the current action flow still works
- **AND** the backend remains the source of truth for the final authorization decision

### Requirement: Preserve supportability and security of error output
The system SHALL not leak raw technical details into primary delete UX.

#### Scenario: Backend returns sanitized technical details
- **WHEN** the backend returns `message`, `errors[0].Message`, or a technical failure payload
- **THEN** the UI does not render paths, SQL, stack traces, bearer tokens, cookies, or connection strings as primary user text
- **AND** the request id remains the preferred support correlation token

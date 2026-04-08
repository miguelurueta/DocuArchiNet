## MODIFIED Requirements

### Requirement: Gestion Correspondencia toolbar search
The system SHALL render `AppInputSearch` inside `AppToolbar.actionContent` in `GestionCorrespondencia` and keep the table wrapper search disabled to avoid duplicate search controls.

#### Scenario: Search is rendered in toolbar
- **WHEN** `GestionCorrespondencia` renders with a table query state
- **THEN** the toolbar MUST include exactly one visible `AppInputSearch` with accessible name `Buscar tareas workflow`

#### Scenario: Search uses a controlled visible value
- **WHEN** the toolbar search renders with autocomplete enabled
- **THEN** its visible value MUST be controlled by the page composition while preserving the applied table search value through `table.queryState.search`

#### Scenario: Typing updates autocomplete text without applying table search
- **WHEN** the user types in the toolbar search
- **THEN** the page MUST update the autocomplete search text through `useWorkflowInboxAutocomplete.setSearchText(value)` and MUST NOT call backend services directly from the page

#### Scenario: Manual search applies table query
- **WHEN** the user confirms a free text search with Enter or the search icon
- **THEN** the page MUST call `table.onQueryChange({ search: value })` and the request mapper MUST continue applying the simple search mapping contract

#### Scenario: Search clear updates query state
- **WHEN** the user clears the toolbar search
- **THEN** the page MUST clear autocomplete suggestions and update the search through `table.onQueryChange({ search: "" })` without adding a parallel reset flow in the page

#### Scenario: Wrapper search remains disabled
- **WHEN** the toolbar search is present in `GestionCorrespondencia`
- **THEN** `AppTableQueryWrapper` MUST be rendered with `showSearch={false}`

#### Scenario: Existing toolbar actions remain available
- **WHEN** the toolbar search is present
- **THEN** the existing refresh and contextual response actions MUST remain rendered and usable

#### Scenario: Export and pagination remain delegated
- **WHEN** the toolbar search is present
- **THEN** export and pagination MUST remain delegated to the existing `AppTableQueryWrapper` and related table controls without changing their public contracts

#### Scenario: Toolbar search styling remains local
- **WHEN** `GestionCorrespondencia` applies layout styles to the toolbar search
- **THEN** the styles MUST be scoped to the module CSS and MUST NOT alter the internal semantics, focus behavior, states, or accessibility of `AppInputSearch`

## ADDED Requirements

### Requirement: Gestion Correspondencia workflow inbox autocomplete
The system SHALL provide a frontend autocomplete layer for Workflow Inbox suggestions through a domain hook and service while keeping `AppInputSearch` presentational.

#### Scenario: Autocomplete hook does not query below minLength
- **WHEN** `useWorkflowInboxAutocomplete` receives search text shorter than its configured `minLength`
- **THEN** it MUST NOT call the backend service and MUST expose an empty `items` array

#### Scenario: Autocomplete hook queries with search and limit
- **WHEN** `useWorkflowInboxAutocomplete` receives search text that satisfies `minLength`
- **THEN** it MUST call the autocomplete service with the current `search` text and the configured `limit`

#### Scenario: Autocomplete hook exposes loading and items
- **WHEN** a suggestion request is in progress and then resolves
- **THEN** the hook MUST expose `loading = true` during the request and MUST expose mapped `items` with only `value` and optional `label` after success

#### Scenario: Autocomplete hook handles errors without breaking table search
- **WHEN** the autocomplete service rejects
- **THEN** the hook MUST expose an `error`, MUST NOT throw to the component tree, and MUST keep free text table search usable

#### Scenario: Autocomplete hook ignores obsolete responses
- **WHEN** an older request resolves after a newer request has already been issued
- **THEN** the hook MUST NOT let the older response overwrite the current `items`

#### Scenario: Autocomplete debounce is centralized in the hook
- **WHEN** `GestionCorrespondencia` integrates autocomplete with `AppInputSearch`
- **THEN** suggestion debounce MUST live in `useWorkflowInboxAutocomplete` and `AppInputSearch` MUST be configured without its own typing debounce for this flow

#### Scenario: AppInputSearch receives presentational autocomplete inputs
- **WHEN** `GestionCorrespondencia` renders the toolbar search with autocomplete
- **THEN** it MUST pass only `options`, `loading`, value, and callbacks to `AppInputSearch`, and MUST NOT pass endpoint URLs, DTOs, or service functions

#### Scenario: Selecting suggestion applies table search
- **WHEN** the user selects an autocomplete suggestion
- **THEN** `AppInputSearch` MUST emit the selected value and `GestionCorrespondencia` MUST apply it through `table.onQueryChange({ search: selectedValue })`

#### Scenario: Free text search works without suggestions
- **WHEN** there are no autocomplete suggestions and the user confirms free text search
- **THEN** `GestionCorrespondencia` MUST still apply the table search through `table.onQueryChange({ search: value })`

#### Scenario: Autocomplete service adapts backend response
- **WHEN** the backend autocomplete response includes fields beyond `value` and `label`
- **THEN** `workflowInboxAutocomplete.service` MUST adapt the response to the hook contract without leaking backend-specific fields into `AppInputSearch.options`

#### Scenario: Autocomplete does not alter AppTable contracts
- **WHEN** autocomplete is enabled in `GestionCorrespondencia`
- **THEN** export, pagination, selection, `AppTableQueryWrapper`, and `AppTable` public contracts MUST remain unchanged

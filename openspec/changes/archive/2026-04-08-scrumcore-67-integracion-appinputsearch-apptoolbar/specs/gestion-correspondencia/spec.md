## MODIFIED Requirements

### Requirement: Gestion Correspondencia toolbar search
The system SHALL render `AppInputSearch` inside `AppToolbar.actionContent` in `GestionCorrespondencia` and keep the table wrapper search disabled to avoid duplicate search controls.

#### Scenario: Search is rendered in toolbar
- **WHEN** `GestionCorrespondencia` renders with a table query state
- **THEN** the toolbar MUST include exactly one visible `AppInputSearch` with accessible name `Buscar tareas workflow`

#### Scenario: Search uses query state
- **WHEN** the toolbar search renders
- **THEN** its value MUST come from `table.queryState.search`

#### Scenario: Search changes update query state
- **WHEN** the user types in the toolbar search
- **THEN** the page MUST call `table.onQueryChange({ search: value })` and MUST NOT call backend services directly

#### Scenario: Search clear updates query state
- **WHEN** the user clears the toolbar search
- **THEN** the page MUST update the search through `table.onQueryChange({ search: "" })` without adding a parallel reset flow in the page

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

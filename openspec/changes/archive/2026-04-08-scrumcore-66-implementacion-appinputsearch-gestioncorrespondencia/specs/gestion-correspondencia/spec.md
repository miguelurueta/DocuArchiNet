## ADDED Requirements

### Requirement: Gestion Correspondencia toolbar search
The system SHALL render `AppInputSearch` inside `AppToolbar.actionContent` in `GestionCorrespondencia` and keep the table wrapper search disabled to avoid duplicate search controls.

#### Scenario: Search is rendered in toolbar
- **WHEN** `GestionCorrespondencia` renders with a table query state
- **THEN** the toolbar MUST include one visible `AppInputSearch` with accessible name `Buscar tareas workflow`

#### Scenario: Search uses query state
- **WHEN** the toolbar search renders
- **THEN** its value MUST come from `table.queryState.search`

#### Scenario: Search changes update query state
- **WHEN** the user types in the toolbar search
- **THEN** the page MUST call `table.onQueryChange({ search: value })` and MUST NOT call backend services directly

#### Scenario: Wrapper search remains disabled
- **WHEN** the toolbar search is present in `GestionCorrespondencia`
- **THEN** `AppTableQueryWrapper` MUST be rendered with `showSearch={false}`

#### Scenario: Existing toolbar actions remain available
- **WHEN** the toolbar search is added
- **THEN** the existing refresh and contextual response actions MUST remain rendered and usable

### Requirement: Gestion Correspondencia search request mapping
The system SHALL map effective simple search text from `GestionCorrespondencia` to `SearchType = 2` in the module request mapper without changing shared `AppTable` request mapping.

#### Scenario: Effective simple search uses LIKE search type
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives input with `search` whose trimmed length is greater than zero and no advanced search override
- **THEN** the mapped request MUST include the trimmed `Search` value and `SearchType = 2`

#### Scenario: Empty search does not force LIKE
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives empty or whitespace-only `search`
- **THEN** the mapped request MUST NOT force `SearchType = 2`

#### Scenario: Advanced search type is preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives `searchType = 3`
- **THEN** the mapped request MUST preserve `SearchType = 3`

#### Scenario: Pagination and filters are preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` maps search input with page, page size, sort, include config, and structured filters
- **THEN** the mapped request MUST preserve `Page`, `PageSize`, `SortField`, `SortDir`, `IncludeConfig`, and `StructuredFilters`

#### Scenario: Shared mapper remains generic
- **WHEN** other tables use the shared dynamic UI request mapper
- **THEN** they MUST NOT receive `SearchType = 2` automatically because of this Gestion Correspondencia behavior

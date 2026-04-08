## MODIFIED Requirements

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

#### Scenario: Explicit non-advanced search type without text is preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives an explicit `searchType` different from `3` and no effective search text
- **THEN** the mapped request MUST preserve the explicit `SearchType` instead of forcing `SearchType = 2`

#### Scenario: Pagination and filters are preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` maps search input with page, page size, sort, include config, and structured filters
- **THEN** the mapped request MUST preserve `Page`, `PageSize`, `SortField`, `SortDir`, `IncludeConfig`, and `StructuredFilters`

#### Scenario: Shared mapper remains generic
- **WHEN** other tables use the shared dynamic UI request mapper
- **THEN** they MUST NOT receive `SearchType = 2` automatically because of this Gestion Correspondencia behavior

#### Scenario: All matching rows reuse module mapper
- **WHEN** `GestionCorrespondencia` requests all matching rows after a simple search
- **THEN** the request MUST be built through `mapGestionCorrespondenciaTableRequest` and MUST preserve the active `Search` and `SearchType`

#### Scenario: Backend export reuses module mapper
- **WHEN** `GestionCorrespondencia` requests a backend export after a simple search
- **THEN** the export request MUST be built through `mapGestionCorrespondenciaTableRequest` and MUST preserve the active `Search` and `SearchType`

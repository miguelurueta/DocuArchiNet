## ADDED Requirements

### Requirement: AppTableExport exports current page rows

The system MUST allow exporting the currently visible page using the rows exposed by the datasource.

#### Scenario: current page export uses visible rows
- **GIVEN** an `AppTableExportDataSource` with `getCurrentPageRows`
- **WHEN** the user triggers export for `currentPage`
- **THEN** the exported dataset MUST use the rows returned by `getCurrentPageRows`
- **AND** MUST use only data-bearing columns that are valid for export

### Requirement: AppTableExport exports selected rows only when selection is available

The system MUST allow exporting selected rows only when the datasource exposes selected rows.

#### Scenario: selected rows export uses current selection
- **GIVEN** an `AppTableExportDataSource` with `getSelectedRows`
- **WHEN** the user triggers export for `selectedRows`
- **THEN** the exported dataset MUST use the rows returned by `getSelectedRows`

#### Scenario: selected rows option is not actionable without selection
- **GIVEN** an `AppTableExportDataSource` without `getSelectedRows` or with an empty selection result
- **WHEN** the export actions are rendered
- **THEN** the `selectedRows` action MUST be hidden or disabled with clear semantics

### Requirement: Export mode availability is datasource-driven

The system MUST resolve available export actions from datasource capabilities instead of screen-specific logic.

#### Scenario: only supported local modes are exposed
- **GIVEN** a datasource that only supports `currentPage`
- **WHEN** the export actions are resolved
- **THEN** only `currentPage` MUST be available

#### Scenario: current page and selected rows are both supported
- **GIVEN** a datasource that supports `currentPage` and `selectedRows`
- **WHEN** the export actions are resolved
- **THEN** both local modes MUST be available

### Requirement: Export loading is independent from table loading

The system MUST keep export progress separate from table data loading states.

#### Scenario: export does not activate table loading states
- **GIVEN** the table is visible and an export starts
- **WHEN** the export operation takes time
- **THEN** the table content MUST remain visible
- **AND** the table skeleton MUST NOT be activated by export loading

### Requirement: This phase only exposes local export modes

The system MUST keep this phase limited to local export modes and exclude server-side export scopes.

#### Scenario: server-side modes are not exposed in this phase
- **GIVEN** the export UI implemented for this ticket
- **WHEN** the user opens the available actions
- **THEN** `allLoaded` MUST NOT be exposed by this phase
- **AND** `allMatching` MUST NOT be exposed by this phase

### Requirement: Exported data excludes purely visual action columns

The system MUST avoid exporting purely visual action columns as if they were business data.

#### Scenario: action columns are excluded from exported dataset
- **GIVEN** a table configuration that includes action-only columns
- **WHEN** the export payload is generated
- **THEN** the exported dataset MUST exclude columns without data value semantics

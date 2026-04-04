## ADDED Requirements

### Requirement: AppTable separates row click, row selection, and cell focus

`AppTable` MUST support row click callbacks, single or multiple row selection, and configurable cell focus behavior as distinct concerns.

#### Scenario: Row click remains available

- **GIVEN** an `AppTable` configured with `onRowClicked`
- **WHEN** the user clicks a row
- **THEN** the callback is invoked with the row data

#### Scenario: Single selection remains supported

- **GIVEN** an `AppTable` configured with `rowSelection="single"`
- **WHEN** the user selects a row
- **THEN** only one row remains selected

#### Scenario: Multiple selection remains supported

- **GIVEN** an `AppTable` configured with `rowSelection="multiple"`
- **WHEN** the user selects rows
- **THEN** multiple rows can remain selected

### Requirement: AppTable suppresses cell focus by default for list screens

`AppTable` MUST allow configuring AG Grid cell focus behavior and SHOULD default to suppressing cell focus for list-style screens.

#### Scenario: Cell focus is suppressed by default

- **GIVEN** an `AppTable` without explicit cell focus configuration
- **WHEN** the user clicks a row
- **THEN** the row may be selected
- **AND** no cell remains visually focused by default

#### Scenario: Cell focus can be enabled explicitly

- **GIVEN** an `AppTable` configured with cell focus enabled
- **WHEN** the user clicks a cell
- **THEN** AG Grid default cell focus behavior remains available

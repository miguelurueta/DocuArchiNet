## ADDED Requirements

### Requirement: AppTable provides a shared readable typography system

`AppTable` MUST provide a shared typography baseline for row content and headers in both table and card presentations.

#### Scenario: Grid rows use the shared readable typography

- **GIVEN** an `AppTable` rendered in table mode
- **WHEN** rows are displayed
- **THEN** row content uses the shared font family, size, weight, and line-height

#### Scenario: Grid headers remain visually consistent

- **GIVEN** an `AppTable` rendered in table mode
- **WHEN** headers are displayed
- **THEN** headers use a consistent typography style aligned with row content

### Requirement: Card presentation keeps the same typography direction

`AppTable` MUST keep typography consistency when rendered as cards.

#### Scenario: Cards use the same typography direction as the grid

- **GIVEN** an `AppTable` rendered in cards mode
- **WHEN** card labels and values are displayed
- **THEN** typography remains visually consistent with the table presentation

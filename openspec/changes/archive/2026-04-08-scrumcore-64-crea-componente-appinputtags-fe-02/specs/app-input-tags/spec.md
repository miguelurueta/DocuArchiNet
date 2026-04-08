## ADDED Requirements

### Requirement: Generic autocomplete contract
The system SHALL allow `AppInputTags` to consume autocomplete data from any parent hook that maps external API responses into a generic option contract with `value`, `label`, and optional `meta`, without coupling the component to endpoints, DTOs, or services.

#### Scenario: Parent hook provides normalized options
- **WHEN** a parent hook maps an API response into options shaped as `{ value: "123", label: "Ana Perez", meta: { source: "usuarios" } }`
- **THEN** `AppInputTags` renders the option using `label` and preserves the component as a presentational UI consumer of normalized data

#### Scenario: Component does not know endpoint details
- **WHEN** `AppInputTags` receives `options`, `loading`, and `onSearch`
- **THEN** it does not construct HTTP payloads, choose endpoints, or import domain hooks

#### Scenario: Search delegates query handling to the parent
- **WHEN** the user types a query that satisfies `minLength`
- **THEN** `AppInputTags` calls `onSearch(query)` and leaves all API lookup behavior to the parent

### Requirement: Secondary actions composition
The system SHALL allow `AppInputTags` to compose secondary actions through component slots or `AppDropdown`-compatible actions while keeping domain behavior outside the reusable component.

#### Scenario: Parent renders a secondary action slot
- **WHEN** a consumer provides a toolbar or action slot to `AppInputTags`
- **THEN** the component renders the action area without interpreting domain-specific behavior

#### Scenario: Remove all remains accessible
- **WHEN** the component exposes a remove-all action alongside autocomplete controls
- **THEN** the action remains keyboard reachable and exposes an accessible name equivalent to "Eliminar todos"

#### Scenario: Secondary actions do not block autocomplete
- **WHEN** the parent renders secondary actions and `loading` is true
- **THEN** the input remains editable unless `disabled` or `selectDisabled` is true

## MODIFIED Requirements

### Requirement: Autocomplete options and loading
The system SHALL render autocomplete suggestions from immutable `options` compatible with `{ value: string; label: string; meta?: unknown }` and SHALL show `loading` without blocking input editing or manual search events.

#### Scenario: Options render as suggestions
- **WHEN** `options` contains `{ label: "Ana Perez", value: "ana" }`
- **THEN** the autocomplete presents `Ana Perez` as a selectable suggestion for value `ana`

#### Scenario: Option metadata remains UI-neutral
- **WHEN** `options` contains metadata such as `{ label: "Ana Perez", value: "ana", meta: { id: 7 } }`
- **THEN** `AppInputTags` keeps rendering the suggestion from `label` and `value` without requiring domain-specific metadata logic

#### Scenario: Empty options keep input usable
- **WHEN** `options` is empty
- **THEN** the input remains editable and manual tag confirmation remains available

#### Scenario: Loading keeps input editable
- **WHEN** `loading` is true
- **THEN** the component shows a loading indicator and keeps the input editable unless disabled

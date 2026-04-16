# Spec Delta

## New Requirements

### Requirement: AppCheckboxCheckAll reusable

The shared UI layer MUST provide an `AppCheckboxCheckAll` API on top of the
existing `AppCheckbox` family for total and partial selection flows.

#### Scenario: Render controlled check all

- **WHEN** a consumer renders `AppCheckboxCheckAll` with `value`, `onChange`
  and `options`
- **THEN** the component renders a master checkbox tied to the same controlled
  selection state
- **AND** the contract does not own business state internally

### Requirement: Indeterminate state for partial selection

The master checkbox MUST expose `indeterminate` when the selection is partial.

#### Scenario: Partial selection

- **GIVEN** at least one option is selected
- **AND** not all options are selected
- **WHEN** the check all control renders
- **THEN** the master checkbox shows `indeterminate`

#### Scenario: Full selection

- **GIVEN** all options are selected
- **WHEN** the check all control renders
- **THEN** the master checkbox is checked
- **AND** `indeterminate` is not active

### Requirement: Check all toggles full selection

The reusable check all flow MUST support selecting and clearing all options.

#### Scenario: Select all

- **GIVEN** not all options are selected
- **WHEN** the user activates the master checkbox
- **THEN** `onChange` receives the full list of option values

#### Scenario: Clear all

- **GIVEN** all options are selected
- **WHEN** the user deactivates the master checkbox
- **THEN** `onChange` receives an empty list

### Requirement: Shared behavior between Group and CheckAll

`AppCheckboxCheckAll` MUST reuse the same selection semantics as
`AppCheckboxGroup`, either through the group composition or a shared internal
helper.

#### Scenario: Consistent state derivation

- **WHEN** selection state is derived for `checked`, `indeterminate`,
  `select all`, or `clear all`
- **THEN** the behavior is based on one shared implementation path
- **AND** logic is not duplicated in divergent ways

### Requirement: Advanced documentation and examples

The component family MUST include documentation aligned with the real API and
examples for advanced usage.

#### Scenario: README coverage

- **WHEN** frontend teams consult the shared component documentation
- **THEN** they find examples for:
  - single checkbox
  - checkbox group
  - check all
  - integration with `Form.Item`, `name`, and `rules`

### Requirement: Advanced behavior validation

The component family MUST include tests for the advanced selection contract.

#### Scenario: Check all tests

- **WHEN** the test suite runs for `AppCheckbox`
- **THEN** it validates:
  - select all behavior
  - clear all behavior
  - indeterminate state
  - disabled behavior
  - documentation examples aligned with the exposed API

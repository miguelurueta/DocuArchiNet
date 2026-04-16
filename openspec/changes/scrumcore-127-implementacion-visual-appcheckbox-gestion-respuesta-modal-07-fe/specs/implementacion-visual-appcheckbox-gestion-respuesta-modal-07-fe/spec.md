# Spec Delta

## New Requirements

### Requirement: GestionDocumentoModal uses AppCheckbox in the top decision block

The `GestionDocumentoModal` UI MUST render the top boolean decision block using
shared `AppCheckbox` controls instead of `AppInput` with `type="checkbox"`.

#### Scenario: Modal renders shared checkboxes

- **WHEN** the modal opens
- **THEN** the top block renders three `AppCheckbox` controls
- **AND** the labels match the current modal copy
- **AND** `AppInput type="checkbox"` is not used for that block anymore

### Requirement: Modal keeps local checkbox state

The checkbox migration MUST preserve the existing local visual state behavior of
the modal.

#### Scenario: Toggle local boolean decisions

- **WHEN** the user clicks any checkbox in the top block
- **THEN** the corresponding local state updates
- **AND** the visual checked state reflects that update
- **AND** no backend or business side effect is triggered

### Requirement: Checkbox migration must not break the modal layout

The modal MUST preserve its current visual structure after the checkbox
migration.

#### Scenario: Stable modal composition

- **WHEN** the modal renders after the migration
- **THEN** the checkbox block remains above the selects and info box
- **AND** the selects, info box, tags input, and actions keep their position
- **AND** long checkbox labels wrap without causing horizontal overflow

### Requirement: Accessibility remains intact

The migrated checkbox block MUST preserve accessible interaction inside the
modal.

#### Scenario: Accessible checkbox interaction

- **WHEN** the modal is navigated with keyboard or assistive technology
- **THEN** each checkbox exposes the correct accessible role and label
- **AND** the modal keeps its focus flow and close behavior

### Requirement: Shared component consumption only

The module implementation MUST consume the checkbox from the shared UI layer and
must not introduce local checkbox wrappers for this flow.

#### Scenario: Shared checkbox integration

- **WHEN** the modal implementation is reviewed
- **THEN** the top decision block imports and uses `AppCheckbox`
- **AND** no duplicate checkbox wrapper is created inside the module

### Requirement: Modal test coverage for the migration

The modal test suite MUST validate the checkbox migration.

#### Scenario: Modal tests

- **WHEN** the related tests run
- **THEN** they validate:
  - modal open and close behavior
  - rendering of the three `AppCheckbox` labels
  - local checked state interaction
  - preservation of the rest of the modal content

## ADDED Requirements

### Requirement: AppAppsteps01Fe renders a reusable step sequence
The system SHALL provide a reusable UI component `AppAppsteps01Fe` in `src/app/Components/UI/AppAppsteps01Fe/` that renders a typed list of steps and a visible active step state.

#### Scenario: Render configured steps
- **WHEN** a consumer passes a valid list of steps with labels and one active step
- **THEN** the component renders all configured step labels in order and highlights the active step

### Requirement: AppAppsteps01Fe exposes controlled navigation hooks
The component MUST support controlled step changes through explicit props and callbacks so the host form can enforce business validation before moving between steps.

#### Scenario: Consumer changes active step
- **WHEN** the consumer updates the active step prop after a user interaction
- **THEN** the component reflects the new active step without losing the configured sequence

#### Scenario: Disabled step cannot be activated
- **WHEN** a step is marked as disabled in the provided model and the user tries to select it
- **THEN** the component keeps the current active step and does not emit a successful step-change outcome

### Requirement: AppAppsteps01Fe includes baseline accessibility semantics
The component SHALL expose baseline accessibility semantics for step state so keyboard and assistive technology users can identify current progress.

#### Scenario: Active step is announced semantically
- **WHEN** the component renders an active step
- **THEN** the DOM includes semantic attributes or roles that identify that step as the current progress state

### Requirement: AppAppsteps01Fe is exported in the shared UI barrel
The system SHALL export the component in `src/app/Components/UI/index.ts` to allow standard imports from the shared UI layer.

#### Scenario: Consumer imports component from UI barrel
- **WHEN** a module imports `AppAppsteps01Fe` from `src/app/Components/UI`
- **THEN** the import resolves successfully without requiring deep relative paths

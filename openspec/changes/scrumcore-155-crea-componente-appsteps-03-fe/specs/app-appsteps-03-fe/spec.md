## ADDED Requirements

### Requirement: AppSteps maintains mandatory behavioral test coverage across variants
The system MUST maintain an automated test suite for `AppSteps` that covers base navigation, form validation guards, progress rendering, timeline behavior, and controlled/uncontrolled usage.

#### Scenario: Base flow renders and changes enabled steps
- **WHEN** `AppSteps` is rendered with base items and the user interacts with an enabled step
- **THEN** the step change callback is executed and the new active step is reflected in the UI

#### Scenario: Disabled step blocks navigation
- **WHEN** a user attempts to select a step marked as disabled
- **THEN** navigation is blocked and the current step remains unchanged

#### Scenario: Form variant blocks and allows based on validateStep
- **WHEN** `variant="form"` is used with `validateStep` returning `false` or `true` (sync or async)
- **THEN** step navigation is blocked for `false` and allowed for `true`

#### Scenario: Progress and timeline variants expose their required signals
- **WHEN** `variant="progress"` receives `progressPercent` and `variant="timeline"` receives `timestamp` metadata
- **THEN** progress percentage and timeline timestamps are rendered, and timeline remains vertically oriented

### Requirement: AppSteps integrates in a real consumer module without duplicated step logic
The system SHALL integrate `AppSteps` in at least one production module so that the consumer orchestrates `items`, `current`, `onChange`, and `validateStep` without reimplementing internal step navigation logic.

#### Scenario: Consumer module adopts AppSteps as flow shell
- **WHEN** the target module renders its staged workflow in the real UI
- **THEN** the flow is rendered via `AppSteps` instead of custom duplicated step navigation markup/logic

#### Scenario: Consumer keeps validation ownership outside AppSteps
- **WHEN** form/business rules are required before advancing to another step
- **THEN** the consumer provides validation through `validateStep` or equivalent orchestration props while `AppSteps` remains domain-agnostic

### Requirement: Integration behavior is verifiable in module-level tests
The system MUST include module-level tests that validate `AppSteps` wiring in the integrated consumer to prevent regressions between shared UI and business module behavior.

#### Scenario: Integrated module test validates wiring contract
- **WHEN** the module-level test renders the consumer containing `AppSteps`
- **THEN** the test verifies observable behavior for active step rendering and step transitions according to supplied props

#### Scenario: Integrated module test guards against duplicated navigation behavior
- **WHEN** the integrated consumer is exercised in tests
- **THEN** navigation behavior is driven by `AppSteps` contract and no parallel custom navigation engine is required in the consumer

### Requirement: AppSteps usage documentation includes real integration example
The system SHALL document a real usage example of `AppSteps` in the integrated module, including selected variant and orchestration responsibilities.

#### Scenario: Documentation references integrated module and contract boundaries
- **WHEN** developers consult AppSteps usage documentation
- **THEN** they can identify a real module example and understand that domain validation stays in the consumer while step UI orchestration stays in `AppSteps`

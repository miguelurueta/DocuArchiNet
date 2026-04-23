## ADDED Requirements

### Requirement: AppLoadingState delays rendering to avoid flicker
The system SHALL provide a reusable `AppLoadingState` UI component that manages its own delay logic and avoids rendering a loading indicator until `loading` has remained `true` for at least `delayMs`.

#### Scenario: Loading turns true but finishes before delayMs
- **WHEN** `AppLoadingState` receives `loading=true` and then `loading=false` before `delayMs` elapses
- **THEN** the loading UI SHALL NOT be rendered at any point

#### Scenario: Loading remains true beyond delayMs
- **WHEN** `AppLoadingState` receives `loading=true` and `loading` remains `true` for at least `delayMs`
- **THEN** the loading UI SHALL be rendered after the delay elapses

#### Scenario: Loading turns false after being visible
- **WHEN** `AppLoadingState` is visible and then receives `loading=false`
- **THEN** the loading UI SHALL be hidden

### Requirement: AppLoadingState is inline and accessible
`AppLoadingState` MUST render as an inline (non full-screen) small card/loading state and MUST be accessible.

#### Scenario: Accessibility attributes are present while visible
- **WHEN** `AppLoadingState` is rendered in loading state (visible)
- **THEN** it SHALL expose `role="status"` and `aria-live="polite"`

#### Scenario: Component does not block the whole screen
- **WHEN** `AppLoadingState` is rendered inside a panel or page section
- **THEN** it SHALL NOT behave as a global full-screen blocker

### Requirement: AppLoadingState does not leak timers
`AppLoadingState` MUST clean up any internal timers when unmounted and when `loading` changes to prevent state updates after unmount.

#### Scenario: Unmount while a delay timer is pending
- **WHEN** `AppLoadingState` is unmounted while `loading=true` and a delay timer is pending
- **THEN** it SHALL clean up timers and MUST NOT attempt state updates after unmount

#### Scenario: Loading toggles while a delay timer is pending
- **WHEN** `AppLoadingState` receives `loading=true` and then toggles to `loading=false` before `delayMs` elapses
- **THEN** it SHALL clean up the pending delay timer

### Requirement: Consumers do not implement delay logic
Consumers MUST NOT duplicate delay/timer logic in views. The delay and visibility control MUST reside exclusively within `AppLoadingState`.

#### Scenario: Consumer passes loading and delayMs only
- **WHEN** a view needs a delayed loading indicator
- **THEN** it SHALL use `AppLoadingState` and only provide `loading` and (optionally) `delayMs`, without implementing timers in the view

### Requirement: GestionCorrespondenciaRoute uses AppLoadingState for the detail panel loading
`GestionCorrespondenciaRoute` SHALL use `AppLoadingState` for its detail panel loading state and SHALL preserve the existing test id for the loading container.

#### Scenario: Detail panel shows AppLoadingState while detailState is loading
- **WHEN** the route determines `detailState === "loading"`
- **THEN** it SHALL render `AppLoadingState` for the panel

#### Scenario: Loading container keeps its test id
- **WHEN** the loading state is rendered in the detail panel
- **THEN** the container SHALL keep `data-testid="gestion-correspondencia-loading-state"`


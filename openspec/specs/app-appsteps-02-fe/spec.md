# app-appsteps-02-fe Specification

## Purpose
TBD - created by archiving change scrumcore-154-crea-componente-appsteps-02-fe. Update Purpose after archive.
## Requirements
### Requirement: AppSteps supports progress variant with external percentage
The system SHALL support `variant="progress"` in `AppSteps` and render a global progress indicator only when `progressPercent` is provided by the consumer.

#### Scenario: Progress variant renders external percentage
- **WHEN** a consumer renders `AppSteps` with `variant="progress"` and a numeric `progressPercent`
- **THEN** the component displays the progress indicator and keeps visual consistency with the steps state without calculating business progress internally

#### Scenario: Progress block is omitted without percentage
- **WHEN** a consumer renders `variant="progress"` without `progressPercent`
- **THEN** the component does not render the progress block and keeps step rendering stable

### Requirement: AppSteps supports timeline variant with timestamp metadata
The system SHALL support `variant="timeline"` with vertical layout and timestamp rendering per step while preserving existing item composition (`title`, `description`, `icon`).

#### Scenario: Timeline enforces vertical layout
- **WHEN** a consumer renders `AppSteps` with `variant="timeline"`
- **THEN** the component forces vertical orientation regardless of external direction preference

#### Scenario: Timeline renders timestamp per step
- **WHEN** step items include `timestamp` metadata under `variant="timeline"`
- **THEN** each step renders its timestamp with readable timeline-style separation

### Requirement: AppSteps applies responsive behavior across variants
The system SHALL provide responsive behavior so `default`, `form`, and `progress` variants can fallback to vertical orientation in constrained viewport/container widths while `timeline` remains vertical.

#### Scenario: Horizontal variants fallback on small widths
- **WHEN** a horizontal-capable variant is rendered in a narrow viewport
- **THEN** the component falls back to a readable vertical presentation without breaking step content

#### Scenario: Timeline remains vertical on all breakpoints
- **WHEN** `variant="timeline"` is rendered on desktop, tablet, or mobile
- **THEN** orientation remains vertical with consistent readability

### Requirement: AppSteps provides accessible step interaction and feedback
The component MUST expose accessible interaction and feedback semantics for advanced variants, including keyboard navigation, visible focus, and non-color-only status cues.

#### Scenario: Active step exposes semantic current state
- **WHEN** a step is active in any supported variant
- **THEN** the component marks it with `aria-current="step"` semantics

#### Scenario: Step states are understandable beyond color only
- **WHEN** a step enters process or error visual state
- **THEN** status remains distinguishable through semantic/structural cues and not solely through color differences


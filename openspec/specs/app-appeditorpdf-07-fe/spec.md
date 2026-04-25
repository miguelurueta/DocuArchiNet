# app-appeditorpdf-07-fe Specification

## Purpose
TBD - created by archiving change scrumcore-164-crea-componente-appeditorpdf-07-fe. Update Purpose after archive.
## Requirements
### Requirement: AppEditorPdf reusable contract
The shared UI layer MUST provide an `AppEditorPdf` reusable component with a typed API for document source, current page state, and visual guides configuration.

#### Scenario: Consumer renders AppEditorPdf with minimal contract
- **WHEN** a module renders `AppEditorPdf` with required props only
- **THEN** the component SHALL render the PDF viewport without runtime prop validation errors

### Requirement: Visual guides for page boundaries
`AppEditorPdf` MUST render visual guides that identify page boundaries and reading frame limits for the active page.

#### Scenario: Visual guides are shown on page render
- **WHEN** the component renders a valid page
- **THEN** the viewport SHALL show page-boundary guides aligned with the rendered page area

### Requirement: Visual metrics publication
`AppEditorPdf` MUST expose visual metrics of the current page through an optional callback contract.

#### Scenario: Metrics callback on relevant visual change
- **WHEN** page index, zoom, or document source changes
- **THEN** `AppEditorPdf` SHALL emit updated visual metrics through `onMetricsChange` when provided

### Requirement: Stable navigation behavior
`AppEditorPdf` MUST preserve consistent navigation behavior when users switch pages while guides and metrics are enabled.

#### Scenario: Page navigation updates guides and metrics
- **WHEN** the user navigates from one page to another
- **THEN** the component SHALL update visible guides for the target page
- **AND** the component SHALL recalculate and emit metrics for the target page when callback is configured

### Requirement: Testable behavior coverage for FE-07
The implementation MUST include automated tests that validate the reusable contract, visual guides, and metrics behavior defined for FE-07.

#### Scenario: Focused component test suite
- **WHEN** the focused `AppEditorPdf` tests run
- **THEN** the suite SHALL verify render contract, guide visibility, and metrics emission behavior without real network calls


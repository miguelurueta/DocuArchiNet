## Purpose
TBD - created by syncing change scrumcore-165-crea-componente-appeditorpdf-08-fe. Update Purpose after archive.

## Requirements
### Requirement: Page counter overlay for AppEditorPdf
`AppEditorPdf` MUST render a visible page-context counter in the format `Pagina X de Y` when the component is in visual pagination mode and total pages are available.

#### Scenario: Counter is rendered with valid page context
- **WHEN** `AppEditorPdf` has a valid current page and total page count
- **THEN** the component SHALL display `Pagina X de Y` using the active context values

### Requirement: Deterministic active page resolution
`AppEditorPdf` MUST resolve the active page using a deterministic priority strategy: cursor position first when focus/selection exists, and scroll position as fallback when cursor context is unavailable.

#### Scenario: Cursor context takes precedence over scroll
- **WHEN** both cursor-based context and scroll-based context are available
- **THEN** the component SHALL use the cursor-derived page as the active page for the counter

#### Scenario: Scroll fallback without cursor context
- **WHEN** cursor context is not available and scroll context is available
- **THEN** the component SHALL compute and display the active page from scroll context

### Requirement: Stable counter updates under frequent interactions
`AppEditorPdf` MUST avoid unstable counter flicker and excessive rerender churn during rapid scroll and selection changes.

#### Scenario: Rapid scroll updates keep stable counter behavior
- **WHEN** the user performs rapid sequential scroll events
- **THEN** the counter SHALL update with controlled frequency and SHALL avoid redundant state updates for unchanged page context

### Requirement: Backward-compatible context publication
`AppEditorPdf` MUST keep the FE-07 reusable behavior and SHALL expose page-context information through an optional callback-compatible contract for advanced consumers.

#### Scenario: Optional callback receives updated page context
- **WHEN** the active page context changes and an optional context callback is configured
- **THEN** the component SHALL emit the updated page context without requiring the callback for baseline rendering

### Requirement: Testable behavior coverage for FE-08
The implementation MUST include automated tests for page counter rendering, active page priority rules, and fallback behavior in the reusable `AppEditorPdf` contract.

#### Scenario: Focused FE-08 test suite validates counter and context
- **WHEN** the FE-08 focused component tests run
- **THEN** the suite SHALL verify counter format, cursor-priority resolution, scroll fallback, and stable update behavior without real network calls

## Purpose
TBD - created by syncing change scrumcore-168-crea-componente-appeditorpdf-10-fe. Update Purpose after archive.

## Requirements
### Requirement: Responsive toolbar contract for AppEditorPdf (FE-10)
`AppEditorPdf` MUST offer a responsive toolbar surface that adapts to narrow viewports without requiring a theme toggle control.

#### Scenario: Toolbar remains usable on narrow width
- **WHEN** `AppEditorPdf` is rendered in a narrow container (mobile/tablet width)
- **THEN** the toolbar SHALL adapt (wrap, collapse, or overflow) so primary actions remain reachable

### Requirement: No theme toggle exposed by default
`AppEditorPdf` MUST NOT expose a theme toggle action as part of the FE-10 responsive toolbar by default.

#### Scenario: Theme toggle is not rendered
- **WHEN** the consumer renders `AppEditorPdf` with default props
- **THEN** the toolbar SHALL NOT render any theme toggle control

### Requirement: Theme mode remains externally controllable
Even without a theme toggle, `AppEditorPdf` MUST preserve compatibility with theme configuration passed through to the underlying editor (e.g. `themeMode` / `defaultThemeMode` patterns used by `AppEditor`).

#### Scenario: Consumer sets dark theme without toggle
- **WHEN** the consumer configures the editor theme mode via props
- **THEN** the editor SHALL render with the configured theme while keeping the toolbar free of a toggle action

### Requirement: Composable toolbarActions integration
The responsive toolbar MUST remain composable with `toolbarActions` provided by consumers, preserving ordering rules and not breaking optional actions introduced in earlier capabilities (e.g. FE-09 page-break action).

#### Scenario: External actions coexist with built-in actions
- **WHEN** a consumer provides `toolbarActions` and enables optional built-in actions
- **THEN** `AppEditorPdf` SHALL compose the actions deterministically and remain responsive under narrow widths

### Requirement: Testable behavior coverage for FE-10
The implementation MUST include automated tests that verify: responsive toolbar behavior at narrow widths, absence of theme toggle by default, and backward-compatible theme mode configuration.

#### Scenario: FE-10 test suite validates responsive toolbar contract
- **WHEN** the FE-10 focused tests run
- **THEN** the suite SHALL confirm the toolbar adapts to narrow widths and that no theme toggle is rendered unless explicitly introduced by a consumer


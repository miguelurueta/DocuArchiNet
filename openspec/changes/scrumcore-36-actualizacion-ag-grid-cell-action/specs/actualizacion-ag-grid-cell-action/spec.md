## ADDED Requirements

### Requirement: AppTable renders dynamic cell actions inside action columns
The system SHALL render visible cell content for columns marked as dynamic action columns when the normalized AppTable model includes `isActionColumn` and action metadata. The visual rendering MUST occur inside the shared `AppTable` stack and MUST NOT require a module-specific grid implementation.

#### Scenario: Render action content in the acciones column
- **WHEN** a table column is marked as `isActionColumn` and includes one or more `AppGridCellAction` items
- **THEN** the final `AppTable` column definition MUST render visible action content in the cell
- **AND** the module consuming `AppTable` MUST NOT create a parallel renderer outside the shared component

### Requirement: AppTable action cells reuse the existing action layer
The system SHALL evaluate availability, build payloads and execute actions by reusing the shared dynamic action layer already implemented in `AppTable`. The action cell renderer MUST NOT duplicate guard, payload or execution logic.

#### Scenario: Execute an enabled action from the cell
- **WHEN** the user interacts with a rendered action that is visible and enabled
- **THEN** the renderer MUST evaluate availability using the shared guard
- **AND** the renderer MUST build the payload using the shared payload builder
- **AND** the renderer MUST execute the action through the shared action execution layer

### Requirement: Action visibility and enabled state follow the guard result
The system SHALL render dynamic actions according to the normalized availability result returned by the shared guard. Visibility and enabled state MUST be interpreted consistently across action cells.

#### Scenario: Hide an action when availability says it is not visible
- **WHEN** the shared availability evaluation returns `isVisible = false` for a cell action
- **THEN** the renderer MUST omit that action from the rendered output

#### Scenario: Disable an action when availability says it is visible but not enabled
- **WHEN** the shared availability evaluation returns `isVisible = true` and `isEnabled = false`
- **THEN** the renderer MUST show the action in a disabled state
- **AND** interacting with the disabled action MUST NOT trigger execution

### Requirement: Action rendering supports the minimum current presentation contract
The system SHALL support the minimum current visual contract required by the real backend payload used in `workflowInboxgestion`. At minimum, action cells MUST support `Presentation = icon_button` without breaking extensibility for future presentations.

#### Scenario: Render icon_button presentation
- **WHEN** a dynamic cell action resolves to `Presentation = icon_button`
- **THEN** the cell renderer MUST display an inline action affordance compatible with that presentation
- **AND** the renderer MUST preserve the backend action order without reordering in frontend

#### Scenario: Preserve rendering stability for unsupported presentations
- **WHEN** a dynamic cell action uses a presentation not yet supported visually
- **THEN** the renderer MUST keep the cell stable without throwing rendering errors
- **AND** the unsupported action MUST be ignored or represented by a neutral fallback

### Requirement: Action behavior classification MUST NOT execute final UI behaviors directly
The system SHALL classify dynamic action behavior using the shared behavior resolver, but action cells MUST NOT execute navigation, modal opening or download side effects directly as part of this phase.

#### Scenario: Handle client_event metadata without direct UI side effects
- **WHEN** a dynamic cell action resolves to `Behavior = client_event`
- **THEN** the renderer MUST keep execution inside the shared action layer flow
- **AND** the renderer MUST NOT directly navigate, open a modal or trigger a download from the cell renderer itself

### Requirement: Action cell rendering preserves compatibility with SCRUMCORE-35 integration
The system SHALL add visual rendering for cell actions without breaking the existing `GestionCorrespondencia` integration delivered in `SCRUMCORE-35`. The table, route loading flow and shared `AppTable` usage MUST continue working after the action renderer is added.

#### Scenario: GestionCorrespondencia keeps using AppTable after action rendering is added
- **WHEN** the `workflowInboxgestion` screen is rendered after the action cell phase is implemented
- **THEN** the `acciones` column MUST stop appearing empty
- **AND** the screen MUST still render through the shared `AppTable`
- **AND** the integration introduced in `SCRUMCORE-35` MUST remain intact

## ADDED Requirements

### Requirement: AppTable resolves menuItems against MenuActions before rendering AppDropdown
The system SHALL resolve `BehaviorConfig.menuItems` against the backend-provided `MenuActions` catalog before rendering dropdown items in dynamic action cells. The frontend MUST NOT hardcode menu definitions locally.

#### Scenario: Resolve menu ids to full actions
- **WHEN** a dynamic cell action contains `BehaviorConfig.menuItems`
- **THEN** the renderer MUST resolve each referenced id against `MenuActions` by exact `ActionId`
- **AND** the dropdown items MUST be built from the resolved actions instead of id humanization

### Requirement: MenuActions metadata is preserved through the shared AppTable pipeline
The system SHALL preserve `MenuActions` from the backend DTO through the shared dynamic AppTable pipeline until the action cell renderer can consume it.

#### Scenario: Preserve MenuActions from query to renderer
- **WHEN** a backend response includes `MenuActions`
- **THEN** the shared table query and normalization pipeline MUST keep that catalog available to the action cell renderer
- **AND** the consuming module MUST NOT implement its own menu catalog handling

### Requirement: Children are rendered as recursive dropdown submenus
The system SHALL support hierarchical menu actions by mapping backend `Children` to recursive `children` entries in `AppDropdownItem`.

#### Scenario: Render nested menu actions
- **WHEN** a resolved menu action includes one or more `Children`
- **THEN** the frontend MUST map those children recursively to dropdown submenu items
- **AND** the resulting submenu structure MUST be rendered through the shared `AppDropdown`

### Requirement: IsDivider maps to non-executable dropdown separators
The system SHALL map `IsDivider = true` to a visual divider entry in the shared dropdown contract. Divider items MUST NOT participate in action execution.

#### Scenario: Render divider item
- **WHEN** a resolved menu action has `IsDivider = true`
- **THEN** the frontend MUST render a dropdown divider
- **AND** the divider MUST NOT evaluate guards, build payloads or execute actions

### Requirement: Invalid or missing menu resolution does not break rendering
The system SHALL keep rendering stable when `MenuActions` is missing, empty or incomplete. Unresolved menu ids MUST be ignored without fatal UI errors.

#### Scenario: Ignore unresolved menu item ids
- **WHEN** a `menuItems` id does not exist in `MenuActions`
- **THEN** that item MUST be ignored
- **AND** the dropdown rendering MUST remain stable without throwing a fatal error

#### Scenario: Keep rendering stable when MenuActions is missing
- **WHEN** a response omits `MenuActions` or returns it empty
- **THEN** the renderer MUST preserve a stable fallback behavior
- **AND** the screen MUST NOT crash because of missing menu catalog data

### Requirement: Only resolved executable menu items may use the action layer
The system SHALL reuse the shared action layer only for resolved and executable dropdown items. Dividers and invalid items MUST never execute.

#### Scenario: Execute a valid resolved dropdown action
- **WHEN** a dropdown item resolves to a valid executable action
- **THEN** the renderer MUST evaluate availability, build payload and execute through the shared action layer

#### Scenario: Ignore execution for divider or invalid item
- **WHEN** a dropdown item is a divider or cannot be resolved to an executable action
- **THEN** the renderer MUST NOT call the shared action execution flow

### Requirement: AppDropdown compatibility is preserved for non-table consumers
The system SHALL extend the shared `AppDropdown` contract only as needed to support dynamic menu actions, while preserving compatibility with existing consumers outside AppTable.

#### Scenario: Existing AppDropdown consumers keep working
- **WHEN** the shared dropdown is extended to support divider items and recursive children
- **THEN** existing consumers outside AppTable MUST continue working without feature regressions

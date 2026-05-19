## ADDED Requirements

### Requirement: APP-APPTREETABLE-001 AppTreeTable provides backend-driven tree table rendering
The system SHALL provide a reusable `AppTreeTable` UI component that can render hierarchical data in a table-like layout, supporting a backend-driven integration mode.

#### Scenario: Render tree rows from provided data
- **WHEN** the consumer provides `rows` with hierarchical structure
- **THEN** the component renders the rows in a tree layout and shows expand/collapse controls for nodes with children

#### Scenario: Render tree rows from an async loader
- **WHEN** the consumer provides an async `load()` (or equivalent backend-driven data source)
- **THEN** the component requests data and renders the resulting tree rows

### Requirement: APP-APPTREETABLE-002 AppTreeTable supports a stable typed contract
The component MUST expose a typed contract (TypeScript) that is stable and reusable across modules.

#### Scenario: Consumer can provide typed rows and callbacks
- **WHEN** a consumer uses the component in TypeScript strict mode
- **THEN** `rows`, node identifiers, and callbacks are type-checked without using `any`

### Requirement: APP-APPTREETABLE-003 AppTreeTable isolates UI state and does not affect other components
The component MUST be self-contained and MUST NOT change global state, routes, or shared UI behavior outside its own render tree.

#### Scenario: Rendering AppTreeTable does not modify global navigation
- **WHEN** the component is mounted and interacted with
- **THEN** no navigation is triggered and no external layout elements are affected

### Requirement: APP-APPTREETABLE-004 AppTreeTable provides loading, empty, and error states
The component SHALL provide UI states for loading, empty results, and error conditions when using the backend-driven mode.

#### Scenario: Loader in progress shows loading state
- **WHEN** `load()` is pending
- **THEN** the component shows a loading indicator and disables interactions that require data

#### Scenario: Loader returns empty list shows empty state
- **WHEN** `load()` resolves with no rows
- **THEN** the component shows an empty state message

#### Scenario: Loader fails shows error state
- **WHEN** `load()` rejects or returns an error result
- **THEN** the component shows an error state and a retry affordance (if enabled by props)

### Requirement: APP-APPTREETABLE-005 AppTreeTable supports expand/collapse interactions
The component SHALL allow users to expand and collapse tree nodes, and MUST keep the expanded state consistent with the rendered rows.

#### Scenario: Expand node reveals children
- **WHEN** the user expands a node with children
- **THEN** the children become visible under the node

#### Scenario: Collapse node hides children
- **WHEN** the user collapses an expanded node
- **THEN** the children are hidden

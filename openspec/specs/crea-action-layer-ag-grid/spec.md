# crea-action-layer-ag-grid Specification

## Purpose
TBD - created by archiving change scrumcore-34-crea-action-layer-ag-grid. Update Purpose after archive.
## Requirements
### Requirement: Action layer stays inside the existing AppTable capability
The system SHALL implement the dynamic actions layer inside `src/app/Components/UI/AppTable/` so it continues the existing AppTable architecture established in Fase 1B and Fase 2. The system MUST NOT create this capability under `src/features/dynamic-ui-table/`.

#### Scenario: Add the action layer to the current dynamic table stack
- **WHEN** the action layer files are created for this phase
- **THEN** they MUST live under the existing `AppTable` root and preserve continuity with the current contracts, adapters, services, hooks and tests

### Requirement: Action execution reuses the normalized frontend action model
The system SHALL operate primarily on the normalized frontend action model produced by Fase 1B. The system MUST support `AppGridCellAction` as the principal action contract and MUST NOT reimplement the normalization logic already handled by `dynamicUiActionMapper.ts`.

#### Scenario: Consume an action already normalized by the dynamic table pipeline
- **WHEN** a consumer passes an action produced from `DynamicUiTableDto`
- **THEN** the action layer MUST be able to resolve behavior, presentation, payload and availability without requiring the raw backend DTO shape as its primary input

### Requirement: Action service executes backend actions without UI concerns
The system SHALL expose an HTTP service for dynamic actions that uses `clienteApi` and returns `ApiResponse<unknown>`. The service MUST support a default endpoint and an injected endpoint and MUST NOT transform the backend response into UI state.

#### Scenario: Execute an action against a bound endpoint
- **WHEN** a caller invokes the action service with a valid execution request
- **THEN** the service MUST send that request to the configured endpoint and return the backend contract unchanged

### Requirement: Action context includes claims and selection data
The system SHALL define an action context contract that supports `row`, `selectedRows`, `columnKey`, `tableId` and `userClaims`. The guard and payload builder MUST rely on that context instead of reading claims or selection state from domain-specific infrastructure.

#### Scenario: Evaluate an action that depends on claims and selected rows
- **WHEN** the action layer receives context with `selectedRows` and `userClaims`
- **THEN** it MUST be able to build payloads and evaluate availability without reaching into module-specific state

### Requirement: Payload builder is pure and respects precedence
The system SHALL provide a pure payload builder for action execution. It MUST build payload from derived row/selection fields, then request metadata, and finally manual payload overrides, returning a new object without mutating the action or the context.

#### Scenario: Manual payload overrides derived and metadata values
- **WHEN** the builder receives an action, context and manual payload with overlapping keys
- **THEN** the resulting payload MUST preserve the precedence order defined for this phase and MUST return a new object

### Requirement: Availability guard evaluates only safe frontend rules
The system SHALL provide an availability guard that evaluates `RequiredClaimsAny`, `RequiredClaimsAll`, `ClaimKey` and only those rules that are safe to interpret on the frontend. The guard MUST return `isVisible`, `isEnabled` and optional reasons, and MUST NOT invent results for ambiguous backend-only rules.

#### Scenario: Action contains an unsafe or ambiguous rule
- **WHEN** the guard receives an action with rules that cannot be safely resolved in frontend
- **THEN** it MUST avoid evaluating those rules as authoritative business logic and MUST document the limitation through its output or documentation

### Requirement: Behavior and presentation remain extensible string-based resolvers
The system SHALL resolve `behavior` and `presentation` metadata through extensible string-based resolvers. These resolvers MUST return `kind`, `rawValue`, `isKnown` and optional `config`, and MUST NOT execute navigation, modal rendering, downloads or any other UI side effect.

#### Scenario: Resolver receives a known behavior or presentation
- **WHEN** the resolver receives values such as `api_call`, `navigate`, `modal`, `client_event`, `button` or `icon_button`
- **THEN** it MUST classify the value and expose normalized metadata without rigid enums or side effects

#### Scenario: Resolver receives an unknown future value
- **WHEN** the resolver receives a behavior or presentation not explicitly known by the current frontend
- **THEN** it MUST preserve the raw value and report it as unknown instead of failing the contract

### Requirement: Actions hook only orchestrates mutations and reusable helpers
The system SHALL expose `useDynamicUiTableActions` as the only React Query layer for dynamic action execution. The hook MUST orchestrate the service, payload builder, guard and resolvers, and MUST return a reusable API with `executeAction`, helper functions, mutation state and last action result, without rendering UI or knowing domain behavior.

#### Scenario: Execute an action through the hook
- **WHEN** a consumer calls `executeAction` from the hook with a valid request
- **THEN** the hook MUST run the mutation, expose execution state and return a structured execution result without coupling to navigation, modals or domain-specific side effects


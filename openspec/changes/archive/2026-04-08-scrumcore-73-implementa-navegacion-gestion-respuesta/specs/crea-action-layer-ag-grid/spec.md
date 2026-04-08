## MODIFIED Requirements

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

### Requirement: Action renderer can notify reusable client events without domain coupling
The system SHALL allow the AppTable action layer to notify consumers when a visible `client_event` action is triggered, providing reusable action metadata without embedding module-specific navigation logic inside the shared renderer.

#### Scenario: Renderer emits client event metadata to the consumer
- **WHEN** a visible action with `behavior = "client_event"` is activated from the action cell
- **THEN** the action layer MUST be able to notify the consumer with at least `actionId`, `row`, and `columnKey`

#### Scenario: Shared action renderer does not hardcode navigation
- **WHEN** a `client_event` action is triggered from a row action cell
- **THEN** the shared action layer MUST NOT call `navigate`, build module URLs, or depend on a specific domain route

#### Scenario: Missing callback preserves current renderer behavior
- **WHEN** the consumer does not provide a reusable client-event callback
- **THEN** the action renderer MUST keep its current safe behavior and MUST NOT fail or assume a default navigation

#### Scenario: Api-call actions remain unaffected
- **WHEN** an action with `behavior = "api_call"` is triggered from the same action cell
- **THEN** the action layer MUST continue executing through the existing execution service and MUST NOT reroute that behavior through the client-event callback

#### Scenario: Action metadata uses the effective row model
- **WHEN** the action layer emits a reusable client event
- **THEN** the `row` payload MUST come from the effective row model already normalized for the table and MUST preserve the effective row identifier when available

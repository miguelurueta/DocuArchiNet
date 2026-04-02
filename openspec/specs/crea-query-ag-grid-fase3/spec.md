# crea-query-ag-grid-fase3 Specification

## Purpose
TBD - created by archiving change scrumcore-31-crea-query-ag-grid-fase3. Update Purpose after archive.
## Requirements
### Requirement: Generic query input for dynamic tables
The system SHALL define a transversal query contract for `AppTable` that accepts `tableId` as a required field and supports optional pagination, search, sorting, and configuration flags through `DynamicTableQueryInput`. The system MUST also define a `RequestMapper<TRequest>` that transforms that generic input into the concrete request expected by each backend endpoint.

#### Scenario: Build a domain request from the shared input
- **WHEN** a consumer provides `DynamicTableQueryInput` and a `RequestMapper<TRequest>`
- **THEN** the query layer MUST generate the concrete request object without adding domain-specific fields to the shared input contract

### Requirement: Service preserves the backend response contract
The system SHALL expose a reusable service function `getDynamicTable<TRequest>(request: TRequest)` that sends the request through `clienteApi` and resolves with `ApiResponse<DynamicUiTableDto | null>`. The service MUST preserve the backend response shape and MUST NOT map rows, columns, actions, or UI state.

#### Scenario: Return the backend payload unchanged
- **WHEN** the service receives a successful HTTP response containing `ApiResponse<DynamicUiTableDto | null>`
- **THEN** it MUST resolve with that same response contract so the hook can perform the UI adaptation

### Requirement: Hook adapts dynamic table responses to the shared AppTable model
The system SHALL expose `useDynamicUiTableQuery` as the only layer that uses React Query for this capability. The hook MUST accept `input`, `requestMapper`, and `queryFn`, MUST use the query key `["dynamic-ui-table", input.tableId, input.page, input.pageSize, input.search, input.sortField, input.sortDirection, input.includeConfig]`, and MUST transform `DynamicUiTableDto` into `rows` and `columns` using the existing phase 1B adapters.

#### Scenario: Produce AppTable-ready data from a dynamic response
- **WHEN** the hook receives a successful response with `data` containing table configuration and rows
- **THEN** it MUST return normalized `rows`, `columns`, `total`, `pagination`, `loading`, `error`, `isEmpty`, and `refetch` values in the shared `AppDataTableAgGrid` model without modifying the base `AppTable` component

### Requirement: This phase stops before visual ColDef adaptation
The system SHALL keep this capability at the query and shared-model level. It MUST NOT convert `AppGridColumn[]` to `ColDef[]` or flatten `AppGridRow[]` into the final row shape required by the visual `AppTable` component during this phase.

#### Scenario: Consumer needs the grid base visual contract
- **WHEN** a consumer requires `ColDef<T>[]` and plain row objects to render `AppTable`
- **THEN** that final visual adaptation MUST be resolved outside this phase without changing the behavior defined for the query hook

### Requirement: Null data is treated as an empty successful state
The system SHALL treat `success = true` with `data = null` as a valid empty result. In that case the hook MUST NOT expose an error and MUST return empty rows and columns, `total = 0`, and `isEmpty = true`.

#### Scenario: Successful query with no table payload
- **WHEN** the backend responds with `success = true` and `data = null`
- **THEN** the hook MUST represent the result as an empty state instead of a failed query

### Requirement: Query errors are surfaced consistently
The system SHALL surface both backend business failures (`success = false`) and transport failures as `Error | null` from the hook. The hook MUST follow the existing project error handling patterns and MUST keep React Query concerns encapsulated so consumers do not need to interpret Axios or backend internals.

#### Scenario: Backend indicates a failed operation
- **WHEN** the query resolves with `success = false`
- **THEN** the hook MUST expose an `Error` and MUST NOT report the result as a successful empty state

#### Scenario: Transport failure during the query
- **WHEN** the query function rejects because of an HTTP or network error
- **THEN** the hook MUST expose that failure through `error` and keep the output contract stable for the consumer


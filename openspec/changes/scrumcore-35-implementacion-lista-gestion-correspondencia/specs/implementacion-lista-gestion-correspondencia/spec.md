## ADDED Requirements

### Requirement: GestionCorrespondencia consumes workflowInboxgestion through the existing dynamic AppTable stack
The system SHALL integrate the `workflowInboxgestion` backend endpoint into the `gestionCorrespondencia` module by reusing the existing dynamic AppTable query infrastructure. The module MUST consume `POST /api/workflowInboxgestion/inboxgestion` through reusable hooks and services, and MUST NOT hardcode transport logic directly inside `GestionCorrespondencia.tsx`.

#### Scenario: Initial route load requests the inbox data
- **WHEN** the user enters `/dashboard/gestion-correspondencia`
- **THEN** the module MUST trigger the query for `workflowInboxgestion`
- **AND** the screen MUST resolve its state from the dynamic AppTable query pipeline rather than local mocks

### Requirement: AppTable remains the final visual renderer
The system SHALL continue using `AppTable` as the final rendered table in `gestionCorrespondencia`. The implementation MUST NOT create a parallel grid component inside the module and MUST NOT render AG Grid directly from the feature page.

#### Scenario: Render the inbox after a successful response
- **WHEN** the query returns data successfully
- **THEN** the screen MUST pass final `rows` and `columns` into `AppTable`
- **AND** the table visible in the page MUST still be the shared `AppTable` component

### Requirement: Final AppGrid to AppTable adaptation is reusable and lives in AppTable
The system SHALL provide reusable adapters that transform `AppGridColumn[]` to `ColDef<T>[]` and `AppGridRow[]` to flat `rowData` suitable for `AppTable`. These adapters MUST live under the existing `AppTable/adapters` folder because they are generic component integration concerns, not feature-specific logic.

#### Scenario: Convert the dynamic intermediate model to AppTable props
- **WHEN** `workflowInboxgestion` data is normalized by the dynamic AppTable query layer
- **THEN** the system MUST transform that intermediate model into the visual contract expected by `AppTable`
- **AND** the conversion MUST remain reusable for future modules

### Requirement: Initial screen loading uses the application Skeleton pattern
The system SHALL show a first-load `Skeleton` screen for `gestionCorrespondencia` using the same application pattern already used by route-level loading pages. The initial loading state MUST NOT rely only on the internal AG Grid overlay.

#### Scenario: First load is pending
- **WHEN** the inbox query is still loading for the first time
- **THEN** the route-level wrapper MUST render a screen skeleton for toolbar, filters, pagination summary and table area
- **AND** the page MUST NOT render mocked data as a fallback

### Requirement: Route-level loading and error handling preserve the module routing pattern
The system SHALL handle `loading`, `error` and `success` through a route-level wrapper compatible with the existing `Outlet + Drawer` structure in `gestionCorrespondencia`. The route integration MUST preserve the drawer navigation behavior already implemented by the module.

#### Scenario: The inbox query fails
- **WHEN** the inbox query returns an error
- **THEN** the route-level wrapper MUST render a stable error state
- **AND** the route structure MUST remain compatible with the contextual drawer behavior

#### Scenario: The user opens the contextual response drawer after data integration
- **WHEN** the user navigates to `/dashboard/gestion-correspondencia/respuesta`
- **THEN** the drawer MUST continue opening over the main screen
- **AND** closing it MUST continue returning to `/dashboard/gestion-correspondencia`

### Requirement: GestionCorrespondencia page becomes presentational instead of mock-backed
The system SHALL refactor `GestionCorrespondencia.tsx` so it no longer owns hardcoded mock rows and columns. The page MUST receive or derive prepared props from the module integration layer and stay focused on visual composition and local UI controls.

#### Scenario: Replace current mock data with integrated table state
- **WHEN** the integration is complete
- **THEN** `GestionCorrespondencia.tsx` MUST stop using static rows and columns defined in the component body
- **AND** the visible totals and controls MUST reflect the real query result

### Requirement: Refresh and pagination controls integrate with query state
The system SHALL wire the current screen controls to the real query state returned by the module integration layer. At minimum, the implementation MUST support refresh and page size synchronization against backend pagination metadata.

#### Scenario: The user clicks Actualizar
- **WHEN** the user clicks the `Actualizar` action in the toolbar
- **THEN** the module MUST trigger a `refetch` of the inbox query

#### Scenario: The backend returns pagination metadata
- **WHEN** the inbox response includes `Page`, `PageSize` and `Total`
- **THEN** the screen MUST reflect the backend total count
- **AND** the page size control MUST stay synchronized with the query state

### Requirement: Dynamic actions metadata is preserved without overpromising final UI execution
The system SHALL preserve dynamic action metadata coming from the inbox response so the screen remains compatible with the dynamic AppTable pipeline. This phase MUST NOT require a complete visual execution layer for actions inside `GestionCorrespondencia` unless that rendering support is explicitly added to `AppTable`.

#### Scenario: The inbox response includes CellActions metadata
- **WHEN** the backend returns dynamic actions metadata for the `acciones` column
- **THEN** the integration MUST preserve compatibility with the dynamic AppTable model
- **AND** the screen MUST NOT invent unsupported visual behavior beyond the currently available shared table capabilities

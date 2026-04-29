# app-appvisorpdf-01-fe (Delta Spec)

## ADDED Requirements

### Requirement: AppVisorPdf SHALL be implemented as a shared UI component
El sistema SHALL implementar el visor PDF como un componente UI reutilizable con nombre
can\u00f3nico `AppVisorPdf` y ubicaci\u00f3n obligatoria bajo `src/app/Components/UI/AppVisorPdf/`.

#### Scenario: Shared component location is enforced
- **WHEN** the component is created for `SCRUMCORE-190`
- **THEN** it SHALL be placed in `src/app/Components/UI/AppVisorPdf/`

#### Scenario: Module-specific placement is rejected
- **WHEN** implementation attempts to place the core viewer in `src/modules/...`
- **THEN** the change SHALL be considered non-compliant with this capability

### Requirement: AppVisorPdf SHALL remain domain-agnostic
El sistema SHALL mantener `AppVisorPdf` desacoplado de l\u00f3gica de negocio de m\u00f3dulos
como `gestionCorrespondencia`, integrando reglas externas solo v\u00eda props/callbacks.

#### Scenario: Domain logic stays in consumer module
- **WHEN** a consumer module integrates `AppVisorPdf`
- **THEN** business decisions SHALL remain in the module layer and not in shared UI

#### Scenario: Integration uses explicit contracts
- **WHEN** a consumer module connects to `AppVisorPdf`
- **THEN** it SHALL do so through typed props and explicit callbacks

### Requirement: AppVisorPdf SHALL provide a stable viewing UX baseline
El sistema SHALL preservar una UX base estable para visualizaci\u00f3n: render predecible,
sin parpadeos (flicker) inducidos por re-render y con navegaci\u00f3n consistente.

#### Scenario: Rendering remains stable while interacting
- **WHEN** the user performs supported interactions (zoom, scroll, page navigation)
- **THEN** the UI SHALL remain stable without unexpected flicker or layout jumps

#### Scenario: Loading states are explicit
- **WHEN** the PDF source is being resolved/loaded
- **THEN** the component SHALL surface an explicit loading state instead of silent blank UI

### Requirement: AppVisorPdf SHALL define a testable integration contract
El sistema SHALL definir un contrato de integraci\u00f3n comprobable que cubra:
fuente de documento (URL/blob/bytes), eventos de error, y hooks de extensi\u00f3n para
integraci\u00f3n con m\u00f3dulos consumidores.

#### Scenario: Consumer can observe load failures
- **WHEN** the PDF cannot be loaded or parsed
- **THEN** the consumer SHALL receive an explicit error notification callback

#### Scenario: Consumer can control key parameters
- **WHEN** the consumer provides configuration (e.g., initial page, zoom, read-only affordances)
- **THEN** `AppVisorPdf` SHALL honor those parameters deterministically


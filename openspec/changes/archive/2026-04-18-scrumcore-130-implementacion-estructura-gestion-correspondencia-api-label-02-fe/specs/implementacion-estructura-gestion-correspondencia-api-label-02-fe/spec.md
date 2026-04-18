# Spec Delta

## New Requirements

### Requirement: GestionRespuestaMainTabContent must consume estructura-respuesta by idTareaWf

The `GestionRespuestaMainTabContent` view MUST request
`solicita-estructura-respuesta-id-tarea` when the main management tab loads,
using `idTareaWf` as the query parameter.

#### Scenario: Request on component load

- **WHEN** `GestionRespuestaMainTabContent` mounts with a valid `idTareaWf`
- **THEN** the frontend performs the API request for
  `/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea`
- **AND** the request uses `idTareaWf` as the only filter input

### Requirement: The response must be exposed as estrucTuraRespuesta

The frontend MUST normalize the response into a reusable UI variable named
`estrucTuraRespuesta`.

#### Scenario: Reusable UI structure

- **WHEN** the API returns data successfully
- **THEN** the component exposes a reusable structure named
  `estrucTuraRespuesta`
- **AND** that structure contains, at minimum:
  - `Radicado`
  - `Destinatario`
  - `TramiteDocumento`

### Requirement: GestionRespuestaInfoHeader metadata must use estrucTuraRespuesta

The static metadata currently rendered by `GestionRespuestaInfoHeader` MUST be
replaced by values from `estrucTuraRespuesta`.

#### Scenario: Dynamic header metadata

- **WHEN** `estrucTuraRespuesta` is available
- **THEN** the header renders:
  - `Radicado` from `estrucTuraRespuesta.Radicado`
  - `Remitente` from `estrucTuraRespuesta.Destinatario`
  - `Trámite` from `estrucTuraRespuesta.TramiteDocumento`

### Requirement: Empty and error states must not break the header

The management tab MUST handle successful empty responses and controlled errors
without breaking the view.

#### Scenario: Empty result

- **GIVEN** the API returns `success = true` and `data = []`
- **WHEN** the component resolves its UI state
- **THEN** the header renders safe fallback values
- **AND** the component remains usable

#### Scenario: Controlled error

- **GIVEN** the API returns `success = false`
- **WHEN** the component resolves its UI state
- **THEN** the header renders safe fallback values
- **AND** the component does not assume valid structure data

### Requirement: Response semantics must not rely on message

The frontend MUST not use the backend `message` property for control flow.

#### Scenario: Logic source of truth

- **WHEN** the response is evaluated
- **THEN** frontend logic depends on:
  - `success`
  - `data`
  - `data.length`
- **AND** does not branch on `message`

### Requirement: Module-layer abstraction must be preserved

The implementation MUST follow the module architecture with explicit types,
service, normalization, and reusable state orchestration.

#### Scenario: Layered implementation

- **WHEN** the feature is implemented
- **THEN** the module contains:
  - typed response contracts
  - a service for the endpoint
  - a normalization path toward `estrucTuraRespuesta`
  - a reusable hook or equivalent orchestration consumed by
    `GestionRespuestaMainTabContent`

### Requirement: Test coverage must validate the integration

The relevant tests MUST validate the API consumption contract and the header
replacement behavior.

#### Scenario: Integration tests

- **WHEN** the related test suite runs
- **THEN** it validates:
  - request orchestration with `idTareaWf`
  - creation of `estrucTuraRespuesta`
  - replacement of static header metadata
  - fallback behavior for empty and error cases

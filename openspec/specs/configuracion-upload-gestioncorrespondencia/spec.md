# Configuracion Upload Gestion Correspondencia

## Purpose

Define how Gestion Correspondencia obtains and applies backend-driven upload configuration for Gestion Respuesta attachments, including allowed extensions, maximum file size, fail-closed behavior, and the preservation of the existing documental upload workflow.

## Requirements

### Requirement: Gestion Correspondencia shall load upload configuration for CORRESPO from backend
Gestion Correspondencia SHALL obtain the allowed file extensions and maximum upload size for Gestion Respuesta attachments from `/api/gestor-documental/configuracion-upload` using `nameProceso=CORRESPO`.

#### Scenario: Request upload configuration with CORRESPO process
- **GIVEN** Gestion Respuesta initializes the documental upload loader
- **WHEN** the upload configuration is requested
- **THEN** the frontend SHALL call `/api/gestor-documental/configuracion-upload`
- **AND** the request SHALL include `nameProceso=CORRESPO`
- **AND** the request SHALL be made through `clienteApi` from a service layer, not directly from a React component

#### Scenario: Apply backend extensions and size as final upload rules
- **GIVEN** the backend returns an active CORRESPO configuration row
- **WHEN** the row contains `ExtensionUpload` and `LengUpload`
- **THEN** `ExtensionUpload` SHALL be converted into `accept` and `allowedExtensions`
- **AND** `LengUpload` SHALL be converted into `maxSizeBytes`
- **AND** those values SHALL be passed to the existing documental upload flow

#### Scenario: Fail closed when configuration is not usable
- **GIVEN** the backend response has `success=false`, `data=[]`, no usable extensions, or `LengUpload <= 0`
- **WHEN** Gestion Respuesta tries to enable file selection
- **THEN** the upload flow SHALL remain disabled
- **AND** the user SHALL see a controlled functional error state with retry capability

### Requirement: Upload configuration service shall normalize backend response variants
The upload configuration service SHALL normalize backend payloads in PascalCase and camelCase without introducing `any`.

#### Scenario: Normalize PascalCase response
- **GIVEN** the backend row contains `ExtensionUpload`, `LengUpload`, `NameProceso`, and `EstadoProceso`
- **WHEN** the service maps the row
- **THEN** the returned config SHALL use `accept`, `allowedExtensions`, `maxSizeBytes`, and `nameProceso`

#### Scenario: Normalize camelCase response
- **GIVEN** the backend row contains `extensionUpload`, `lengUpload`, `nameProceso`, and `estadoProceso`
- **WHEN** the service maps the row
- **THEN** the returned config SHALL be equivalent to the PascalCase mapping

#### Scenario: Select the active row
- **GIVEN** the backend returns multiple rows
- **WHEN** at least one row has `EstadoProceso === 1` or `estadoProceso === 1`
- **THEN** the service SHALL use the first active row

#### Scenario: Fall back to first row when no active row exists
- **GIVEN** the backend returns rows but none are active
- **WHEN** the service maps the response
- **THEN** the service MAY use the first row
- **AND** it SHALL still validate extensions and maximum size before returning config

#### Scenario: Normalize extension list
- **GIVEN** `ExtensionUpload` is `.PDF, DOC, .docx, ,PDF`
- **WHEN** extensions are normalized
- **THEN** the result SHALL be `[ ".pdf", ".doc", ".docx" ]`
- **AND** `accept` SHALL be `.pdf,.doc,.docx`

### Requirement: Gestion Respuesta documental upload shall preserve existing workflow behavior
The SCRUMCORE-287 change SHALL only replace the final source of upload file rules and SHALL preserve the existing Gestion Respuesta documental upload workflow.

#### Scenario: Preserve AppUploadDocumental integration
- **GIVEN** Gestion Respuesta opens the upload documental modal
- **WHEN** `loadGestionRespuestaUploadConfig` resolves
- **THEN** `AppUploadDocumental` SHALL continue receiving its config through the existing `loadConfig` contract
- **AND** `AppUploadBatchView` SHALL continue receiving `accept` and `maxSizeBytes` through `AppUploadDocumental`

#### Scenario: Preserve documental process flags
- **GIVEN** the backend returns a valid CORRESPO upload configuration
- **WHEN** the Gestion Respuesta loader builds `UploadDocumentalConfig`
- **THEN** `multiple`, `requiereTipologia`, `requiereFechaCarga`, `fechaCargaObligatoria`, and `validationMode` SHALL keep the Gestion Respuesta behavior already implemented before SCRUMCORE-287

#### Scenario: Keep typologies out of scope
- **GIVEN** SCRUMCORE-284 already handles workflow typologies
- **WHEN** SCRUMCORE-287 is implemented
- **THEN** it SHALL NOT add typology requests, dropdowns, metadata by file, or `renderMetadata` behavior

#### Scenario: Keep storage out of scope
- **GIVEN** the upload by chunks and final storage flow already exists
- **WHEN** SCRUMCORE-287 is implemented
- **THEN** it SHALL NOT change `init -> chunks -> status -> complete -> almacenamiento`
- **AND** it SHALL NOT modify backend endpoints or storage payloads

### Requirement: Hook shall expose reusable upload configuration state
A typed hook SHALL expose reusable Gestion Correspondencia state for loading, empty, error, and retry when a surface needs direct upload configuration control.

#### Scenario: Load configuration on mount
- **GIVEN** `useConfiguracionUploadCorrespondencia` is enabled
- **WHEN** the hook mounts
- **THEN** it SHALL request the upload configuration once
- **AND** expose `loading`, `config`, `error`, `empty`, and `reload`

#### Scenario: Do not load when disabled
- **GIVEN** `enabled=false`
- **WHEN** the hook mounts
- **THEN** it SHALL NOT call the configuration service

#### Scenario: Ignore stale responses
- **GIVEN** a configuration request is in flight
- **WHEN** the hook reloads or unmounts
- **THEN** stale responses SHALL NOT overwrite the latest hook state
- **AND** the request SHOULD use `AbortController` when available

### Requirement: Tests and documentation shall cover the CORRESPO upload configuration contract
The change SHALL include focused tests and enterprise documentation for the upload configuration integration.

#### Scenario: Service tests cover normalization and validation
- **WHEN** service tests run
- **THEN** they SHALL cover request params, PascalCase mapping, camelCase mapping, extension normalization, active row selection, invalid response handling, and `AbortSignal`

#### Scenario: Hook tests cover state transitions
- **WHEN** hook tests run
- **THEN** they SHALL cover loading, success, empty, error, disabled mode, retry, abort, and stale response handling

#### Scenario: Integration tests cover existing upload loader behavior
- **WHEN** Gestion Respuesta upload tests run
- **THEN** they SHALL verify the backend-derived `accept` and `maxSizeBytes` reach the existing upload flow
- **AND** they SHALL verify file add/remove behavior remains intact

#### Scenario: Architecture documentation is updated
- **WHEN** implementation is complete
- **THEN** documentation SHALL be created under `docs/Architecture/GestionCorrrespondecia/Integracion-AppUploadDocumental/`
- **AND** it SHALL include SCRUMCORE-287 metadata, endpoint consumed, contract mapping, UI states, testing evidence, restrictions, and known limits

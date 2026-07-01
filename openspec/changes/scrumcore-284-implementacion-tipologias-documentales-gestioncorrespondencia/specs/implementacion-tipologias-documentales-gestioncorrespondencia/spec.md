## ADDED Requirements

### Requirement: Gestion Respuesta shall propagate workflow route context for attachment typologies
The system SHALL expose `idRutaWf` alongside `idTareaWf` through Gestion Respuesta and the shared Gestion Respuesta documentos context.

#### Scenario: Page receives workflow route id
- **GIVEN** `GestionRespuesta` is rendered with `idRutaWf`
- **WHEN** it creates `GestionRespuestaDocumentosProvider`
- **THEN** the provider receives `idRutaWf`
- **AND** `useGestionRespuestaDocumentos()` exposes `idRutaWf`

#### Scenario: Missing route id blocks workflow typology loading
- **GIVEN** `idTareaWf` exists
- **AND** `idRutaWf` is missing, zero, non-numeric, or not positive
- **WHEN** Gestion Respuesta attachment typologies are requested
- **THEN** the frontend does not call `/api/gestor-documental/tipologias-documentales`
- **AND** the upload flow does not use hardcoded fallback typologies
- **AND** storing a file requiring typology is blocked with a functional message

### Requirement: The frontend shall load workflow document typologies from the confirmed backend endpoint
The system SHALL load document typologies for Gestion Respuesta attachments from `GET /api/gestor-documental/tipologias-documentales` using workflow context.

#### Scenario: Request workflow typologies with required params
- **GIVEN** `idTareaWf` is a positive number
- **AND** `idRutaWf` is a positive number
- **WHEN** the typology service loads options
- **THEN** it calls `GET /api/gestor-documental/tipologias-documentales`
- **AND** sends query param `Contexto=WORKFLOW`
- **AND** sends query param `IdTareaWf={idTareaWf}`
- **AND** sends query param `IdRutaWf={idRutaWf}`
- **AND** does not send `IdTipoTramite`

#### Scenario: Normalize backend typology rows
- **GIVEN** backend returns `success=true`
- **AND** `data` contains rows with `Id` and `Descripcion`
- **WHEN** the service normalizes the response
- **THEN** each row becomes an option with `value`, `label`, `idTipoDocumento`, and `nombreTipoDocumento`
- **AND** `value` equals `Id`
- **AND** `label` equals `Descripcion`
- **AND** `idTipoDocumento` equals `Id`
- **AND** `nombreTipoDocumento` equals `Descripcion`

#### Scenario: Empty catalog is valid but blocks required typology storage
- **GIVEN** backend returns `success=true`
- **AND** `data=[]`
- **WHEN** typologies are loaded
- **THEN** the service returns an empty option list
- **AND** the UI exposes an empty catalog state
- **AND** the user cannot store files while typology is required and no option is available

#### Scenario: Functional backend failure is surfaced
- **GIVEN** backend returns `success=false`
- **WHEN** typologies are loaded
- **THEN** the service throws a functional error
- **AND** the message prefers `errors[0].UserMessage` when available
- **AND** falls back to `message`
- **AND** finally falls back to a generic typology loading error

### Requirement: The typology loading hook shall be abortable, retryable, and anti-stale
The system SHALL provide a Gestion Correspondencia hook for workflow typologies with stable loading state and safe cancellation.

#### Scenario: Do not call API without complete workflow context
- **GIVEN** the hook is enabled
- **AND** `idTareaWf` or `idRutaWf` is missing or invalid
- **WHEN** the hook renders
- **THEN** it does not call the typology service
- **AND** it returns no options
- **AND** it does not report a backend error

#### Scenario: Load options with valid context
- **GIVEN** the hook is enabled
- **AND** `idTareaWf` and `idRutaWf` are positive numbers
- **WHEN** the hook renders
- **THEN** it exposes `loading=true` while the request is pending
- **AND** exposes normalized options after success
- **AND** exposes `empty=true` only when the successful option list is empty

#### Scenario: Retry after failure
- **GIVEN** the hook request fails
- **WHEN** `reload()` is called
- **THEN** the hook calls the service again with the same workflow ids
- **AND** updates `options`, `loading`, `error`, and `empty` from the latest result

#### Scenario: Ignore stale response after workflow context changes
- **GIVEN** a request for task/ruta A is in flight
- **WHEN** the hook receives task/ruta B before A resolves
- **THEN** the result from A is ignored
- **AND** only the result from B can update the hook state

### Requirement: Gestion Respuesta upload shall use real workflow typologies and preserve per-file metadata
The system SHALL replace the seed/hardcoded typology loader used by `GestionRespuestaUploadDocumental` with a backend-backed workflow loader.

#### Scenario: AppUploadDocumental receives workflow route in context
- **GIVEN** `GestionRespuestaUploadDocumental` renders with valid provider context
- **WHEN** it creates `UploadDocumentalContext`
- **THEN** it includes `idTareaWorkflow`
- **AND** it includes `idRutaWorkflow`
- **AND** its `loadTiposDocumentales` loader can resolve typologies from the workflow endpoint

#### Scenario: No hardcoded typology fallback
- **GIVEN** backend typology loading fails
- **WHEN** the user opens the upload modal
- **THEN** the UI does not fall back to `Comprobante De Egreso` or any other hardcoded typology
- **AND** the user sees a functional loading/catalog error

#### Scenario: File-specific typology remains independent
- **GIVEN** multiple files are queued in `AppUploadDocumental`
- **WHEN** the user selects a typology for one file
- **THEN** only that file metadata changes
- **AND** each final storage request keeps using the selected file's own `Trd`

### Requirement: Shared UI components shall remain domain-agnostic
The system SHALL keep workflow typology logic inside `gestionCorrespondencia` and not inside shared UI components.

#### Scenario: AppUploadBatchView remains generic
- **WHEN** the implementation is reviewed
- **THEN** `AppUploadBatchView` does not import Gestion Correspondencia services, hooks, or typology types
- **AND** it does not call `clienteApi`
- **AND** it does not know about `Contexto=WORKFLOW`

#### Scenario: AppInputSelect remains the dropdown component
- **WHEN** typology metadata is rendered for files
- **THEN** the implementation uses the existing select path from `AppUploadDocumental` / `AppInputSelect`
- **AND** it does not introduce a custom select component for workflow typologies

### Requirement: Documentation and tests shall cover workflow typology integration
The system SHALL document and test the Gestion Respuesta workflow typology integration.

#### Scenario: Documentation exists
- **WHEN** SCRUMCORE-284 is ready for review
- **THEN** `docs/Architecture/GestionCorrrespondecia/17-FE-Tipologias-Documentales-Adjuntos-Workflow.md` exists
- **AND** it documents endpoint, params, response, `WORKFLOW + IdTareaWf + IdRutaWf`, no frontend `IdTipoTramite`, per-file metadata, error states, and tests executed

#### Scenario: Tests cover service, hook, and integration
- **WHEN** test evidence is collected
- **THEN** service tests cover params, no `IdTipoTramite`, normalization, empty, failures, invalid ids, and abort signal
- **AND** hook tests cover no-call without ids, loading, empty, error, reload, and stale response handling
- **AND** integration tests cover `idRutaWf` propagation and `GestionRespuestaUploadDocumental` using the workflow-backed typology loader


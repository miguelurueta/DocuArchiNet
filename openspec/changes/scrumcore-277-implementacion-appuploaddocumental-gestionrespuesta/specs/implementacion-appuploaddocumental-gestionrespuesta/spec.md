## ADDED Requirements

### Requirement: GestionRespuesta shall render the documental upload adapter in the Gestion tab
The system SHALL replace the current simple attachment upload surface in the Gestion tab with a GestionRespuesta-specific adapter that composes `AppUploadDocumental`.

#### Scenario: Render adapter with valid context
- **GIVEN** `GestionRespuesta` is opened with workflow task context
- **AND** `GestionRespuestaDocumentosProvider` resolves `nombreGabinete`
- **WHEN** the Gestion tab is rendered
- **THEN** the user sees the documental upload experience powered by `AppUploadDocumental`
- **AND** file selection remains disabled until required config and tipology loaders resolve

#### Scenario: Block storage without response context
- **GIVEN** `idRespuestaRadicado` is missing, empty, non-numeric, or not positive
- **WHEN** the user tries to store a selected file
- **THEN** storage is blocked with a functional error
- **AND** no temporary upload request is sent

### Requirement: The integration shall build the official workflow attachment storage contract
The system SHALL build the final storage request for each file using the SCRUM-250 official PascalCase backend contract.

#### Scenario: Build final request for one file
- **GIVEN** a file named `soporte-respuesta.pdf`
- **AND** `nombreGabinete` is `CORRESPO`
- **AND** `idRespuestaRadicado` is `672`
- **AND** the user selected tipology `43 / Comprobante De Egreso`
- **WHEN** the file is stored
- **THEN** the final `/api/gestor-documental/almacenamiento` request contains `NombreGabinete`, `RutaTemporalId`, `NombreDocumento`, `RequestId`, `Documentos`, `Trd`, `CabinetIndexSeed`, `AnexoRespuesta`, and `NumeroPaginasDeclaradas` in PascalCase
- **AND** `Documentos[0].ArchivoTemporalId` comes from temporary upload init
- **AND** `AnexoRespuesta.NombreArchivo` equals `file.name` without local path or subdirectories
- **AND** `CabinetIndexSeed.ProviderKey` is `RADICACION`
- **AND** `CabinetIndexSeed.Payload.ModoResolucion` is `RespuestaRadicado`

#### Scenario: Store multiple files sequentially
- **GIVEN** multiple valid files are queued
- **AND** each file can have independent tipology metadata
- **WHEN** the user chooses guardar todos
- **THEN** files are processed sequentially
- **AND** each file creates exactly one final storage POST
- **AND** each final POST uses the current file tipology in `Trd`

### Requirement: The storage lifecycle shall validate status before complete
The system SHALL execute `init -> chunks -> status -> complete -> almacenar` for each file.

#### Scenario: Successful temporary lifecycle
- **GIVEN** init returns `RutaTemporalId`, `ArchivoTemporalId`, and `ChunkSizeBytes`
- **WHEN** chunks are uploaded
- **THEN** each chunk request sends raw bytes
- **AND** each chunk request sends `Content-Type: application/octet-stream`
- **AND** each chunk request sends `X-Total-Chunks`
- **AND** `chunkIndex` starts at `0`
- **AND** the system calls status before complete
- **AND** complete is called only when `ChunksPendientes` is empty

#### Scenario: Pending chunks block completion
- **GIVEN** status returns one or more values in `ChunksPendientes`
- **WHEN** the upload flow reaches status validation
- **THEN** complete is not called
- **AND** final storage is not called
- **AND** the file is marked with a retryable error

### Requirement: The workflow attachment response shall be normalized and validated
The system SHALL normalize the nested PascalCase workflow-anexo backend response into typed frontend data while preserving the raw backend result.

#### Scenario: Confirm created attachment
- **GIVEN** final storage returns `success === true`
- **AND** `data.Documento.IdAlmacen` is valid
- **AND** `data.Documento.IdRegistroProduccionDocumental` is valid
- **AND** `data.Documento.NombreArchivoFinal` is non-empty
- **AND** `data.AnexoRespuesta.Created === true`
- **WHEN** the response is processed
- **THEN** the file is marked as stored
- **AND** the normalized result includes document and anexo data
- **AND** `rawBackendResult` is preserved

#### Scenario: Backend reports functional failure
- **GIVEN** final storage returns `success === false`
- **WHEN** the response is processed
- **THEN** the file is not marked as stored
- **AND** the error message uses `errors[0].UserMessage` when available
- **AND** falls back to `message`
- **AND** finally falls back to `Error almacenando anexo`

### Requirement: GestionRespuesta shall refresh the Documentos tab through the shared provider
The system SHALL centralize document reload coordination in `GestionRespuestaDocumentosProvider`.

#### Scenario: Refresh after confirmed anexo creation
- **GIVEN** final storage confirms `AnexoRespuesta.Created === true`
- **WHEN** `GestionRespuestaUploadDocumental` handles the stored callback
- **THEN** it calls `refreshDocumentos()`
- **AND** `documentosRefreshKey` changes
- **AND** `DocumentosWorkbench` reloads its list from backend

#### Scenario: Do not insert document rows manually
- **GIVEN** a file was stored successfully
- **WHEN** the Gestion tab refreshes the document list
- **THEN** `DocumentosWorkbench` obtains rows from its backend list service
- **AND** the implementation does not manually append a synthetic row as the source of truth

### Requirement: The implementation shall preserve enterprise constraints
The system SHALL avoid legacy runtime dependencies and unsafe contract shortcuts.

#### Scenario: Forbidden legacy paths are not used
- **WHEN** the implementation is reviewed
- **THEN** it does not use jQuery
- **AND** it does not use WebForms
- **AND** it does not call `.ashx`
- **AND** it does not use `XMLHttpRequest`
- **AND** it does not use legacy `FormData` upload for chunks
- **AND** it does not call global legacy callbacks
- **AND** it does not update DOM manually

#### Scenario: Type safety is preserved
- **WHEN** the changed TypeScript files are reviewed
- **THEN** no new `any` is introduced
- **AND** unknown backend fields use `unknown` plus runtime guards
- **AND** backend contract errors are surfaced as typed errors

### Requirement: Documentation and tests shall cover the integration
The system SHALL document and test the GestionRespuesta upload integration.

#### Scenario: Enterprise documentation exists
- **WHEN** the change is ready for review
- **THEN** `docs/Architecture/AppUploadDocumental/SCRUMCORE-277-Integracion-GestionRespuesta-Anexos.md` exists
- **AND** it documents objective, official contract source, end-to-end flow, FE/BE field matrix, PascalCase/camelCase mapping, tipology per file, Workbench refresh, error policy, and executed tests

#### Scenario: Affected behavior is tested
- **WHEN** test evidence is collected
- **THEN** mapper tests cover final request construction and response normalization
- **AND** service tests cover status-before-complete and blocked failure paths
- **AND** component/integration tests cover storing a file and refreshing the Documentos tab

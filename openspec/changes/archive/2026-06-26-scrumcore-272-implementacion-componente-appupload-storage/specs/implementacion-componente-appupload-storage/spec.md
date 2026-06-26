## ADDED Requirements

### Requirement: Storage Module Scaffold
The system SHALL provide a UI-independent almacenamiento documental storage module under `src/modules/almacenamientoDocumental/`.

#### Scenario: Module files exist
- **WHEN** SCRUMCORE-272 is implemented
- **THEN** `src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts` exists
- **AND** `src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts` exists
- **AND** `src/modules/almacenamientoDocumental/utils/storageFile.utils.ts` exists
- **AND** focused tests exist for the service and utils.

#### Scenario: Module has no UI dependency
- **WHEN** the storage module is reviewed
- **THEN** it does not import React components, `AppUpload`, `AppUploadDocumental`, `AppProgressBatch`, DOM APIs for rendering, jQuery, WebForms, `.ashx`, or `XMLHttpRequest`.

### Requirement: Strict Storage Contracts
The system SHALL model the storage API with strict TypeScript contracts and runtime guards.

#### Scenario: Public DTO types are available
- **WHEN** consumers import storage types
- **THEN** they can use `StorageUploadInitRequest`, `StorageUploadInitResponse`, `StorageUploadStatusResponse`, `StorageUploadCompleteResponse`, `StorageUploadCancelResponse`, `DocumentoEntrada`, `AlmacenarDocumentoRequest`, `AlmacenarDocumentoResponse`, `UploadStorageProgress`, `UploadOneDocumentInput`, and `UploadOneDocumentResult`.

#### Scenario: Unknown data is explicit
- **WHEN** backend fields are not modeled
- **THEN** they are represented as `unknown`
- **AND** the implementation does not introduce new `any` types.

#### Scenario: Response guards reject invalid contracts
- **WHEN** init, status, complete, cancel, or final storage responses miss required fields
- **THEN** the service throws a typed storage contract error
- **AND** callers do not receive partially trusted data.

### Requirement: Endpoint Builders
The system SHALL expose or use safe endpoint builders for the almacenamiento documental API.

#### Scenario: Endpoints match the ticket contract
- **WHEN** service functions call backend
- **THEN** they use `/api/gestor-documental/almacenamiento/upload-temporal/init`
- **AND** `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- **AND** `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- **AND** `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- **AND** `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
- **AND** `/api/gestor-documental/almacenamiento`.

#### Scenario: Path identifiers are encoded
- **WHEN** an endpoint includes `rutaTemporalId` or `archivoTemporalId`
- **THEN** those values are encoded with `encodeURIComponent`.

### Requirement: ClienteApi Integration
The system SHALL use the project Axios client for every backend call.

#### Scenario: Requests use clienteApi
- **WHEN** init, chunk, status, complete, cancel, or store are executed
- **THEN** the implementation uses `clienteApi`
- **AND** it passes `AbortSignal` through Axios config when provided.

#### Scenario: No legacy transport is used
- **WHEN** the service is reviewed
- **THEN** it does not use `XMLHttpRequest`, `fetch` directly, `.ashx`, or legacy `FormData` upload.

### Requirement: Chunk Utilities
The system SHALL provide utility functions for file metadata and chunk planning.

#### Scenario: Extension is normalized
- **WHEN** a file name has an extension
- **THEN** the utility returns a lowercase extension with leading dot
- **AND** names without extension return an empty string or controlled validation result.

#### Scenario: Chunk count is calculated
- **WHEN** file size and chunk size are positive
- **THEN** total chunks equals `Math.ceil(size / chunkSize)`
- **AND** empty or invalid sizes are rejected by validation.

#### Scenario: Chunk slices are bounded
- **WHEN** a chunk is requested
- **THEN** the returned `Blob` uses `Blob.slice(start, end)` within file bounds.

### Requirement: Temporary Upload Lifecycle
The system SHALL implement init, chunk, status, complete, and cancel operations.

#### Scenario: Init validates response
- **WHEN** `initTemporaryUpload` succeeds
- **THEN** it returns non-empty `rutaTemporalId`, non-empty `archivoTemporalId`, positive `chunkSizeBytes`, and status/estado data when available.

#### Scenario: Chunk upload sends binary body
- **WHEN** `uploadTemporaryChunk` is called
- **THEN** it sends the `Blob` as request body
- **AND** it sets `Content-Type: application/octet-stream`
- **AND** it sets `X-Total-Chunks` to the total chunk count.

#### Scenario: Status is exposed
- **WHEN** a caller needs upload status
- **THEN** `getTemporaryUploadStatus` calls the status endpoint and validates the response contract.

#### Scenario: Complete validates confirmation
- **WHEN** `completeTemporaryUpload` succeeds
- **THEN** it validates that backend confirmed completion or returned a valid completion contract.

#### Scenario: Cancel is best-effort callable
- **WHEN** temporal identifiers are available and cancellation is requested
- **THEN** `cancelTemporaryUpload` calls the delete endpoint with `AbortSignal` support.

### Requirement: Final Storage Request
The system SHALL implement final document storage after successful temporary upload completion.

#### Scenario: Store calls final endpoint
- **WHEN** `almacenarDocumento` is called with a valid request
- **THEN** it posts to `/api/gestor-documental/almacenamiento`
- **AND** it validates `idAlmacen`, `idRegistroProduccionDocumental`, `nombreArchivoFinal`, and `requestId`.

#### Scenario: Raw backend result is preserved
- **WHEN** backend returns additional fields not modeled in `AlmacenarDocumentoResponse`
- **THEN** `uploadAndStoreOneDocument` preserves the original backend result as `rawBackendResult`.

### Requirement: Upload And Store Orchestrator
The system SHALL provide `uploadAndStoreOneDocument` to execute the complete storage lifecycle for one file.

#### Scenario: Happy path executes in order
- **WHEN** `uploadAndStoreOneDocument` is called with valid input
- **THEN** it calls init, uploads all chunks, calls complete, and calls final storage in that order.

#### Scenario: Backend chunk size is authoritative
- **WHEN** init returns a valid `chunkSizeBytes`
- **THEN** the orchestrator recalculates total chunks using that backend value before uploading chunks.

#### Scenario: Progress is reported
- **WHEN** phases advance
- **THEN** `onProgress` receives `initializing`, `uploading`, `completing`, and `storing` phases with percent values bounded from 0 to 100.

#### Scenario: Final request contains one document
- **WHEN** final storage is called by `uploadAndStoreOneDocument`
- **THEN** the request contains one `DocumentoEntrada` for the current file
- **AND** it includes `rutaTemporalId` from the temporal upload.

### Requirement: Failure Ordering
The system SHALL stop the lifecycle at the failing phase.

#### Scenario: Init fails
- **WHEN** init fails
- **THEN** no chunks, complete, status, or final storage calls are made.

#### Scenario: Chunk fails
- **WHEN** a chunk upload fails
- **THEN** remaining chunks are not uploaded
- **AND** complete and final storage are not called.

#### Scenario: Complete fails
- **WHEN** complete fails
- **THEN** final storage is not called.

#### Scenario: Store fails
- **WHEN** final storage fails
- **THEN** the error includes the store phase and enough context for callers to mark the file as failed.

### Requirement: Cancellation
The system SHALL support `AbortSignal` cancellation and temporary upload cleanup.

#### Scenario: Abort before init completes
- **WHEN** the operation is aborted before temporal ids are available
- **THEN** the service throws `storage_aborted`
- **AND** it does not call cancel endpoint.

#### Scenario: Abort after init
- **WHEN** the operation is aborted after temporal ids are available
- **THEN** the service attempts `cancelTemporaryUpload`
- **AND** it does not call complete or final storage after abort.

#### Scenario: Cancel failure is explicit
- **WHEN** cancel endpoint fails during abort cleanup
- **THEN** the failure is exposed as a typed cancel error or documented warning result
- **AND** the document is not reported as stored.

### Requirement: Error Model
The system SHALL expose typed storage errors.

#### Scenario: Error codes identify phase
- **WHEN** an error is thrown by the storage service
- **THEN** it includes one of `storage_contract_error`, `storage_init_error`, `storage_chunk_error`, `storage_status_error`, `storage_complete_error`, `storage_cancel_error`, `storage_store_error`, or `storage_aborted`.

#### Scenario: Request id is preserved
- **WHEN** backend envelope includes a request id in data, meta, or errors
- **THEN** the thrown error or result preserves that request id when available.

### Requirement: Tests and Documentation
The system SHALL include focused verification for storage utils and service behavior.

#### Scenario: Utility tests cover file and chunks
- **WHEN** utility tests run
- **THEN** they cover extension normalization, chunk count, invalid sizes, request id generation, and chunk slicing.

#### Scenario: Service tests cover lifecycle
- **WHEN** service tests run
- **THEN** they cover init, chunk headers/body, status, complete, cancel, store, happy path orchestration, backend chunk size recalculation, raw result preservation, abort, and phase-specific failures.

#### Scenario: OpenSpec validates
- **WHEN** `npx.cmd openspec validate scrumcore-272-implementacion-componente-appupload-storage --strict` runs
- **THEN** the change validates successfully.

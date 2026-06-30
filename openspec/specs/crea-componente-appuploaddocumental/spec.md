# crea-componente-appuploaddocumental Specification

## Purpose
TBD - created by archiving change scrumcore-271-crea-componente-appuploaddocumental. Update Purpose after archive.
## Requirements
### Requirement: AppUploadDocumental Module
The system SHALL provide `AppUploadDocumental` as a UI-independent documental upload specialization under `src/modules/almacenamientoDocumental/components/AppUploadDocumental/`.

#### Scenario: Required files exist
- **WHEN** SCRUMCORE-271 is implemented
- **THEN** `AppUploadDocumental.tsx`, `AppUploadDocumental.types.ts`, `AppUploadDocumental.module.css`, `README.md`, `index.ts`, `hooks/useAppUploadDocumentalState.ts`, and `hooks/useAppUploadDocumentalActions.ts` exist
- **AND** focused tests exist for the component, state hook, mapper, and suggestion utility.

#### Scenario: Component uses reusable UI building blocks
- **WHEN** the component renders
- **THEN** it composes `AppUploadBatchView`
- **AND** file selection is delegated to `AppUpload` through that view
- **AND** batch processing is delegated to `AppProgressBatch`
- **AND** it does not render legacy HTML tables or Bootstrap/WebForms UI.

### Requirement: Loader-Based Configuration
The system SHALL load upload configuration and documental types before enabling storage actions.

#### Scenario: Config and types load before file selection
- **WHEN** `AppUploadDocumental` mounts with valid `proceso` and `context.nombreGabinete`
- **THEN** it calls `loadConfig` with `proceso`, `context`, and `modoDocumento`
- **AND** it calls `loadTiposDocumentales` with `proceso` and `context`
- **AND** file selection remains disabled until required loaders finish successfully.

#### Scenario: Loader failure is fail-safe
- **WHEN** config loading fails
- **THEN** the component disables selection and store actions
- **AND** displays a functional error state
- **AND** calls `onError` with the original error as `unknown`.

#### Scenario: No endpoint is invented
- **WHEN** no canonical config or type endpoint exists in the repo
- **THEN** the component relies on required loaders
- **AND** no direct `clienteApi` call is made from React.

### Requirement: File Queue and Validation
The system SHALL maintain a documental queue with independent state and metadata per file.

#### Scenario: Selected files are normalized
- **WHEN** files are selected or dropped
- **THEN** each file receives a stable `uid`, normalized extension, size, state, and metadata object
- **AND** selected files appear in `AppUploadBatchView`.

#### Scenario: Reject mode blocks invalid files
- **WHEN** `validationMode` is `reject` and a file violates extension or max size rules
- **THEN** the file is not added to the queue
- **AND** a controlled error is exposed.

#### Scenario: Queue-with-error mode keeps invalid files visible
- **WHEN** `validationMode` is `queue-with-error` and a file violates extension or max size rules
- **THEN** the file is added with state `error`
- **AND** the file cannot be stored until the error is resolved or removed.

#### Scenario: Metadata is isolated by file
- **WHEN** a user changes type, date, or selection for one file
- **THEN** only that file metadata changes
- **AND** other queued files keep their metadata.

### Requirement: Typology Per File
The system SHALL support independent documental typology per file.

#### Scenario: Typology is required
- **WHEN** config or props require typology
- **AND** a file has no valid `idTipoDocumento`
- **THEN** storing that file is blocked
- **AND** the row shows an actionable metadata error.

#### Scenario: Manual typology is preserved
- **WHEN** a user manually selects a typology
- **AND** automatic suggestion later recomputes
- **THEN** the manual selection is not overwritten.

#### Scenario: Final request uses current file typology
- **WHEN** a file is stored
- **THEN** the final request includes `trd.idTipoDocumento` and `trd.nombreTipoDocumento` for that file only.

### Requirement: Typology Suggestion Utility
The system SHALL provide a pure utility to suggest typology from file name.

#### Scenario: Suggestion computes best match
- **WHEN** `autoSuggestTipologia` is enabled and typology options are available
- **THEN** the utility normalizes file name and option names
- **AND** removes extension and non-alphanumeric separators
- **AND** tokenizes values
- **AND** ignores tokens below the configured minimum length
- **AND** returns the best option only when score meets the threshold.

#### Scenario: Suggestion is non-blocking
- **WHEN** no option reaches the threshold
- **THEN** no typology is selected automatically
- **AND** the user can still select a typology manually.

### Requirement: Documental Date Per File
The system SHALL support a per-file documental date when required by config or props.

#### Scenario: Date field appears when required
- **WHEN** `requiereFechaCarga` is true
- **THEN** each file row renders a date input in metadata
- **AND** the value is stored in file metadata as `fechaCarga`.

#### Scenario: Date validation blocks invalid store
- **WHEN** date is required or present
- **THEN** it must be a real `yyyy-MM-dd` date
- **AND** its year must not be in the future
- **AND** invalid values block storing only the affected file.

### Requirement: Individual Store
The system SHALL support storing one file when `allowSingleFileStore` is enabled.

#### Scenario: Single store processes only selected file
- **WHEN** the user triggers store on one row
- **THEN** only that file is validated and processed
- **AND** the same storage client and mapper are used
- **AND** `onStored` is emitted on success
- **AND** `onBatchComplete` is not emitted unless explicitly documented as policy.

### Requirement: Batch Store
The system SHALL support storing all valid files sequentially.

#### Scenario: Batch uses AppProgressBatch
- **WHEN** the user triggers store all
- **THEN** `AppProgressBatch` opens for valid queued files
- **AND** each batch item represents one file
- **AND** `processItem` updates current label, phase, and progress.

#### Scenario: Batch processes one file at a time
- **WHEN** multiple files are stored
- **THEN** files are processed sequentially
- **AND** each file executes `init -> chunks -> complete -> almacenar`
- **AND** each file creates one final storage request.

#### Scenario: Batch summary is emitted
- **WHEN** batch finishes
- **THEN** `onBatchComplete` receives total, stored, failed, skipped, cancelled, and stored results.

### Requirement: Storage Client Integration
The system SHALL use the existing storage client for upload and final storage.

#### Scenario: No direct HTTP in component
- **WHEN** implementation is reviewed
- **THEN** `AppUploadDocumental` and its hooks do not import `clienteApi`
- **AND** they call `uploadAndStoreOneDocument` for storage.

#### Scenario: Storage progress maps to UI phases
- **WHEN** storage client reports `initializing`, `uploading`, `completing`, or `storing`
- **THEN** the corresponding file row state/progress is updated
- **AND** `AppProgressBatch` phase is updated during batch processing.

### Requirement: Interface Registration Events
The system SHALL replace legacy `funcion_name` dispatch with typed interface registration events.

#### Scenario: Mapper builds known events
- **WHEN** storage result, raw backend result, context, metadata, process, and mode contain enough information
- **THEN** `buildUploadDocumentalInterfaceRegistration` returns typed `UploadDocumentalInterfaceRegistration[]`.

#### Scenario: Mapper falls back safely
- **WHEN** backend shape has useful data that cannot be safely normalized
- **THEN** the mapper returns `{ kind: "raw", raw }`
- **AND** it does not concatenate fields with `|`
- **AND** it does not call global legacy functions.

#### Scenario: Stored callback includes interface events
- **WHEN** a file stores successfully
- **THEN** `onStored` receives `AlmacenarDocumentoStoredResult`
- **AND** the result includes `interfaceRegistration` when mapper returns events
- **AND** `onInterfaceRegistration` receives the same events when provided.

### Requirement: Cancellation, Retry, and Anti-Stale
The system SHALL support cancellation, retry, and stale-result protection.

#### Scenario: Cancel aborts active storage
- **WHEN** a user cancels active upload
- **THEN** the active `AbortController` is aborted
- **AND** storage client cleanup is attempted when temporal ids exist
- **AND** the file is not marked as stored.

#### Scenario: Retry restarts failed file
- **WHEN** a failed file is retried
- **THEN** the current metadata is revalidated
- **AND** a new storage lifecycle starts from init
- **AND** old temporal ids are not reused.

#### Scenario: Stale results are ignored
- **WHEN** `proceso`, `context.nombreGabinete`, `modoDocumento`, or component mount lifecycle changes during an operation
- **THEN** pending operations are aborted or ignored
- **AND** `onStored` is not emitted for obsolete context.

### Requirement: Preview and Object URL Lifecycle
The system SHALL provide preview without leaking object URLs.

#### Scenario: Preview opens selected file
- **WHEN** a user selects a file row or preview action
- **THEN** the preview area shows PDF/image/fallback according to available file type support.

#### Scenario: Object URLs are cleaned up
- **WHEN** a file is removed, all files are cleared, or component unmounts
- **THEN** any object URL created by the component is revoked.

### Requirement: Security and Legacy Exclusions
The system SHALL avoid legacy transports and unsafe runtime behavior.

#### Scenario: Forbidden dependencies are absent
- **WHEN** source code under `src/modules/almacenamientoDocumental/components/AppUploadDocumental` is reviewed
- **THEN** it does not use jQuery, WebForms, Bootstrap manual DOM APIs, `.ashx`, `XMLHttpRequest`, legacy `FormData`, direct `fetch`, direct `clienteApi`, callbacks by string, or new `any`.

#### Scenario: Sensitive data is not logged
- **WHEN** upload errors occur
- **THEN** the component does not log file bytes, tokens, or full sensitive payloads
- **AND** user-visible errors are controlled messages.

### Requirement: Documentation and Verification
The system SHALL include enterprise documentation and focused verification.

#### Scenario: README documents public contract
- **WHEN** implementation is complete
- **THEN** `README.md` documents objective, props, embedded usage, modal usage, required loaders, upload flow, FE/BE matrix, typology policy, date policy, interface registration contract, errors/retry, and known limits.

#### Scenario: Tests cover critical behavior
- **WHEN** focused tests run
- **THEN** they cover config/type loading, validation modes, per-file metadata, typology suggestion/manual override, date validation, individual store, batch store, mapper variants, callbacks, cancellation, retry, and stale-result protection.

#### Scenario: Browser workflow is verified
- **WHEN** publish readiness is evaluated
- **THEN** browser, manual, or Playwright evidence covers selecting five files, previewing PDF, changing typology and date per file, removing one file, clearing all files, storing one file, storing all files, invalid extension, invalid size, and retry after simulated error
- **OR** the unavailable browser environment is documented as an explicit verification debt.


## ADDED Requirements

### Requirement: Shared AppUploadBatchView component

The system SHALL provide a shared `AppUploadBatchView` component under `src/app/Components/UI/AppUploadBatchView` for rendering reusable batch file upload experiences without business-domain coupling.

#### Scenario: Public file structure is created
- **WHEN** the component is implemented
- **THEN** the folder `src/app/Components/UI/AppUploadBatchView/` contains `AppUploadBatchView.tsx`, `AppUploadBatchView.types.ts`, `AppUploadBatchView.module.css`, `AppUploadBatchView.test.tsx`, `README.md`, and `index.ts`
- **AND** `src/app/Components/UI/index.ts` exports the component API.

#### Scenario: Component remains domain-agnostic
- **WHEN** the component is reviewed
- **THEN** it does not import documental services, storage clients, `clienteApi`, workflow modules, TRD types, gabinete/radicado concepts, or backend endpoints
- **AND** it does not use jQuery, Bootstrap manual, WebForms APIs, HTML strings, global callbacks, or new `any` types.

### Requirement: Controlled generic contract

The system SHALL expose a typed generic contract for files, states, summary, callbacks and render slots.

#### Scenario: File states are generic and complete
- **WHEN** consumers import `AppUploadBatchFileState`
- **THEN** the type includes `queued`, `validating`, `ready`, `uploading`, `completing`, `storing`, `done`, `warning`, `error`, `cancelled`, and `removed`.

#### Scenario: Metadata is generic
- **WHEN** consumers use `AppUploadBatchFileItem<TMetadata>`
- **THEN** `metadata` is typed as `TMetadata`
- **AND** the default metadata type is `unknown`, not `any`.

#### Scenario: View is controlled by props
- **WHEN** `files`, `selectedUid`, `summary`, `disabled`, `loading`, or `can*` props change
- **THEN** the rendered list, preview, toolbar and actions reflect the provided props
- **AND** the component does not become the source of truth for file metadata, validation, upload progress or backend results.

### Requirement: AppUpload composition

The system SHALL compose the existing `AppUpload` component for file selection without replacing or modifying `AppUpload`.

#### Scenario: Files are selected through AppUpload
- **WHEN** a user selects or drops files through the upload selector
- **THEN** `AppUploadBatchView` emits `onFilesSelected(files)` with the selected `File[]`
- **AND** the view does not perform business validation beyond forwarding selector constraints such as `accept`, `maxSize`, `multiple`, `drag`, and `disabled`.

#### Scenario: AppUpload contract remains intact
- **WHEN** the change is reviewed
- **THEN** `src/app/Components/UI/AppUpload/AppUpload.tsx` is not changed to support this ticket unless a separately justified compatibility fix is required.

### Requirement: Batch workbench layout

The system SHALL render an enterprise workbench layout with header, toolbar, upload selector, file list, active preview and footer.

#### Scenario: Empty state is rendered
- **WHEN** `files` is empty
- **THEN** the view displays `emptyMessage` or a default empty message
- **AND** global actions respect `canAddFiles`, `canSaveAll`, `canClearAll`, `disabled`, and `loading`.

#### Scenario: Files are rendered compactly
- **WHEN** `files` contains items
- **THEN** each item shows name, formatted size, state, optional progress, optional phase label, optional warning and optional error
- **AND** long names are visually constrained while preserving the full name via `title` or equivalent accessible text.

#### Scenario: Active file is visible
- **WHEN** `selectedUid` matches a file
- **THEN** that row is marked as active
- **AND** the active indication does not rely only on color.

#### Scenario: Responsive layout is stable
- **WHEN** the viewport is desktop-sized
- **THEN** the list and preview may render in two columns
- **WHEN** the viewport is mobile-sized
- **THEN** the preview and list stack without overlapping controls or overflowing text.

### Requirement: File actions

The system SHALL expose global and per-file actions through callbacks while respecting enablement props.

#### Scenario: Global actions emit callbacks
- **WHEN** the user activates guardar todos
- **THEN** `onSaveAll` is called only if saving all is enabled
- **WHEN** the user activates limpiar todos
- **THEN** `onClearAll` is called only if clearing is enabled.

#### Scenario: Per-file actions emit callbacks
- **WHEN** the user selects or previews a file
- **THEN** `onSelectFile` or `onPreviewFile` is called with that file `uid`
- **WHEN** the user removes a file
- **THEN** `onRemoveFile` is called with that file `uid`
- **WHEN** `canSaveOne=true` and the user saves one file
- **THEN** `onSaveFile` is called with that file `uid`.

#### Scenario: Disabled states are respected
- **WHEN** `disabled`, `loading`, or `item.disabled` is true
- **THEN** unavailable actions are disabled or omitted consistently
- **AND** icon-only buttons provide `aria-label`.

### Requirement: Render slots

The system SHALL allow consumers to inject metadata, custom preview, custom filename and footer content without leaking domain logic into the shared component.

#### Scenario: Metadata slot is rendered
- **WHEN** `renderMetadata` is provided
- **THEN** it is called with `{ item, disabled }` for each relevant file row
- **AND** the returned React node is rendered in the row.

#### Scenario: Custom preview overrides default preview
- **WHEN** `renderPreview` is provided and a file is selected
- **THEN** the custom preview is rendered with the selected item, optional preview URL, and `onClose`.

#### Scenario: Footer extra is rendered
- **WHEN** `renderFooterExtra` is provided
- **THEN** it receives the current summary and renders additional consumer-owned content.

### Requirement: Preview behavior

The system SHALL provide safe default preview behavior for selected files and allow custom preview injection.

#### Scenario: PDF preview is rendered
- **WHEN** the selected file is a PDF and no custom preview exists
- **THEN** the default preview uses an `iframe` or `object` backed by a local object URL or `previewUrl`.

#### Scenario: Image preview is rendered
- **WHEN** the selected file is an image and no custom preview exists
- **THEN** the default preview uses an `img` with accessible alt text.

#### Scenario: Fallback preview is rendered
- **WHEN** the selected file is neither PDF nor image
- **THEN** the default preview shows a fallback with file name, extension and formatted size.

#### Scenario: Object URLs are cleaned up
- **WHEN** the selected file changes or the component unmounts
- **THEN** object URLs created by the component are revoked.

### Requirement: Accessibility and UX

The system SHALL provide baseline accessible semantics for the batch view.

#### Scenario: Summary is announced
- **WHEN** summary values change
- **THEN** the summary region uses polite live-region semantics or equivalent accessible text.

#### Scenario: Errors are visible
- **WHEN** a file has `error` or `warning`
- **THEN** the message is rendered near the file row
- **AND** the state is not conveyed only by color.

#### Scenario: Keyboard access is available
- **WHEN** a user navigates through file rows and action buttons by keyboard
- **THEN** focus remains visible and actions are reachable.

### Requirement: Documentation and tests

The system SHALL document and test `AppUploadBatchView` as a shared UI component.

#### Scenario: README documents usage
- **WHEN** the README is reviewed
- **THEN** it explains objective, props, file states, slots, default preview, custom preview, accessibility, limitations and relationship with `AppUploadDocumental`.

#### Scenario: Unit and integration tests cover behavior
- **WHEN** the test suite for `AppUploadBatchView` runs
- **THEN** it covers empty state, summary, file rendering, active row, callbacks, enablement props, slots, errors/warnings, preview variants and object URL cleanup.

# app-appeditorpdf-06-fe Specification

## Purpose
TBD - created by archiving change scrumcore-163-crea-componente-appeditorpdf-06-fe. Update Purpose after archive.
## Requirements
### Requirement: AppEditorPdf SHALL provide visual pagination baseline by default

The system SHALL configure `AppEditorPdf` with a visual pagination baseline so
consumers render a document-like surface without explicitly passing pagination
props.

#### Scenario: Default visual mode contract

- **WHEN** a consumer renders `AppEditorPdf` without pagination props
- **THEN** the wrapper SHALL forward `paginationMode="visual"`
- **AND** it SHALL forward `pageFormat="A4"` and `pageOrientation="portrait"`
- **AND** it SHALL forward default margins `{ top: 96, right: 72, bottom: 96, left: 72 }`

### Requirement: AppEditorPdf SHALL keep explicit pagination overrides

The system SHALL preserve consumer control over pagination options when explicit
values are provided.

#### Scenario: Override pagination mode

- **WHEN** a consumer sets `paginationMode="none"` on `AppEditorPdf`
- **THEN** the wrapper SHALL forward `paginationMode="none"` without forcing visual mode

#### Scenario: Partial margins override

- **WHEN** a consumer provides partial `pageMargins`
- **THEN** the wrapper SHALL merge provided margins over baseline defaults
- **AND** non-provided margin sides SHALL keep default values

### Requirement: AppEditorPdf SHALL preserve existing accessibility and API contract

The system SHALL keep previous wrapper behavior for controlled/uncontrolled API
and accessible naming order while introducing pagination defaults.

#### Scenario: Accessibility contract unchanged

- **WHEN** consumer omits explicit `aria-label`
- **THEN** `AppEditorPdf` SHALL continue resolving accessible name by existing rules
- **AND** pagination defaults SHALL not alter this behavior


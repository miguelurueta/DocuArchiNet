## ADDED Requirements

### Requirement: Image horizontal alignment control for AppEditorPdf (FE-11)
`AppEditorPdf` MUST provide a reusable way to align an image horizontally (left, center, right) as part of the editing experience.

#### Scenario: User aligns an image to the center
- **WHEN** the user selects an image and chooses alignment `center`
- **THEN** the editor SHALL apply horizontal alignment to the selected image

### Requirement: Persistent alignment serialization contract
Image alignment MUST be persisted in the document as a stable attribute using `data-align` with values `left`, `center`, or `right`, and MUST survive HTML round-trip.

#### Scenario: Alignment survives HTML round-trip
- **WHEN** content with an aligned image is serialized to HTML and loaded again
- **THEN** the image SHALL keep the same `data-align` value and visual alignment

### Requirement: Alignment changes must preserve image width
Changing alignment MUST NOT drop or override the persisted image width (e.g. `data-width`) when present.

#### Scenario: Aligning keeps data-width untouched
- **WHEN** an image has `data-width` and the user changes alignment
- **THEN** the resulting image SHALL keep the existing `data-width` value

### Requirement: Compatibility with AppEditorPdf visual pagination
Image alignment MUST remain compatible with `AppEditorPdf` visual pagination mode, without breaking layout calculations or page-context behavior.

#### Scenario: Visual pagination remains stable with aligned images
- **WHEN** the document contains images aligned left/center/right in visual pagination mode
- **THEN** the editor SHALL keep stable pagination metrics and rendered layout without errors

### Requirement: Testable behavior coverage for FE-11
The implementation MUST include automated tests that validate alignment application, persistence, width preservation, and compatibility with `AppEditorPdf` usage.

#### Scenario: FE-11 test suite validates image alignment contract
- **WHEN** the FE-11 focused tests run
- **THEN** the suite SHALL verify `data-align` persistence, width preservation, and basic integration in `AppEditorPdf`


## ADDED Requirements
### Requirement: Manual Deskew Action
El modulo reusable de digitalizacion documental SHALL allow users to run Deskew manually after a page has already been captured.

#### Scenario: Deskew active page
- **GIVEN** the workspace has at least one captured page
- **AND** no multi-page thumbnail selection is active
- **WHEN** the user clicks the Deskew action beside rotate left and rotate right
- **THEN** the system applies Deskew only to the active page
- **AND** refreshes the preview, thumbnail list, organizer state, and page navigation from the updated scanner pages

#### Scenario: Deskew selected pages
- **GIVEN** the workspace has multiple selected pages
- **WHEN** the user clicks the Deskew action from the preview toolbar or organizer toolbar
- **THEN** the system applies Deskew to every selected page in visual order
- **AND** does not process unselected pages

#### Scenario: Deskew processing feedback
- **GIVEN** a manual Deskew operation is running
- **WHEN** the page is being corrected
- **THEN** the workspace shows the corporate overlay with the message "Corrigiendo inclinacion"
- **AND** keeps scanner controls disabled until the operation finishes

#### Scenario: Already aligned page
- **GIVEN** the selected page does not require correction
- **WHEN** Deskew completes without modifying the image
- **THEN** the system keeps the current page
- **AND** does not show a functional error

### Requirement: Existing Deskew Engine Reuse
The manual Deskew action SHALL reuse the same Deskew integration used by automatic image processing during capture.

#### Scenario: Native Deskew method is available
- **GIVEN** the Dynamsoft runtime exposes a Deskew-compatible native method
- **WHEN** manual Deskew runs on a captured page
- **THEN** the scanner client invokes the same Deskew feature registry used by automatic processing
- **AND** rebuilds the affected page metadata from the scanner buffer

#### Scenario: Native Deskew method is unavailable
- **GIVEN** the Dynamsoft runtime does not expose a Deskew-compatible native method
- **WHEN** manual Deskew is requested
- **THEN** the scanner client treats the operation as unsupported
- **AND** returns the current pages without throwing a functional error

### Requirement: Manual Deskew Compatibility
Manual Deskew SHALL operate on pages present in the scanner page collection regardless of how they were added.

#### Scenario: Supported page sources
- **GIVEN** pages were added by new scan, imported image/PDF flow, duplication, insertion, replacement, or append capture
- **WHEN** the page exists in the scanner page collection
- **THEN** manual Deskew can be requested for that page id through the scanner client contract
- **AND** generated PDF state is invalidated so the next PDF reflects the corrected pages

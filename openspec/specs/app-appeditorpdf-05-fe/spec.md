# app-appeditorpdf-05-fe Specification

## Purpose
TBD - created by archiving change scrumcore-162-crea-componente-appeditorpdf-05-fe. Update Purpose after archive.
## Requirements
### Requirement: GestionRespuesta SHALL expose AppEditorPdf as full-surface editor area

The system SHALL keep `AppEditorPdf` as the only dominant editing surface in
`GestionRespuestaMainTabContent`, without an extra visual shell from
`GestionRespuestaEditorContainer`.

#### Scenario: Neutral editor container shell

- **GIVEN** the workbench is rendered in `GestionRespuestaMainTabContent`
- **WHEN** `GestionRespuestaEditorContainer` wraps the editor
- **THEN** it SHALL behave as a neutral wrapper (no header/inner surface shell)
- **AND** it SHALL preserve accessibility labeling for the main editor zone

#### Scenario: Full-surface editor occupancy

- **GIVEN** `AppEditorPdf` is mounted in the principal column of the workbench
- **WHEN** the panel is rendered with default desktop layout
- **THEN** `AppEditorPdf` SHALL occupy the available height/width of the editor
  area without extra padding/border wrappers from the module container

### Requirement: Integration flow SHALL remain behaviorally stable after full-surface adjustment

The system SHALL preserve the current behavior of steps, send gating, toolbar
actions, and right tools panel while applying the full-surface integration
adjustment.

#### Scenario: Existing behavior remains stable

- **GIVEN** a user interacts with steps, toolbar and panel controls
- **WHEN** the full-surface integration is active
- **THEN** step transitions and send-gating SHALL keep the same behavior as in
  `04-FE`
- **AND** right panel collapse/expand SHALL remain operational


## ADDED Requirements

### Requirement: GestionRespuesta SHALL integrate AppEditorPdf as main editor surface
The system SHALL integrate `AppEditorPdf` in `GestionRespuestaMainTabContent`
as the main editable surface of the workbench, replacing direct usage of
`AppEditor` in that consumer.

#### Scenario: Main surface uses AppEditorPdf
- **WHEN** `GestionRespuestaMainTabContent` renders the editor region
- **THEN** the component SHALL mount `AppEditorPdf` as the principal editor surface

#### Scenario: Visual pagination setup is preserved
- **WHEN** `AppEditorPdf` is rendered in the workbench
- **THEN** existing pagination visual props SHALL remain configured without functional regression

### Requirement: Save interaction contract SHALL remain compatible in consumer flow
The system SHALL preserve save-state behavior in the consumer by using the
`AppEditorPdf` companion APIs without changing the user-facing save flow.

#### Scenario: Save action remains available in toolbar
- **WHEN** user edits content in `GestionRespuestaMainTabContent`
- **THEN** save action control SHALL remain present and operable in toolbar actions

#### Scenario: Save status remains based on current/saved values
- **WHEN** content changes and save is triggered
- **THEN** save-state transitions SHALL follow existing current/saved value logic

### Requirement: Integration change SHALL keep consumer behavior stable
The system SHALL keep existing module behavior stable for steps, panel tools,
attachments, and modal interactions while migrating editor integration.

#### Scenario: Existing workbench interactions remain stable
- **WHEN** user interacts with steps, side panel and attachments
- **THEN** behavior SHALL remain consistent with previous baseline expectations

#### Scenario: Tests reflect AppEditorPdf integration
- **WHEN** module tests run after migration
- **THEN** assertions SHALL validate AppEditorPdf integration without losing prior coverage

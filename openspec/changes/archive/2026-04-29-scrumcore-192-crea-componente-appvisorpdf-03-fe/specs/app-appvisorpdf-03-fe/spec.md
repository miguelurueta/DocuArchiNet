# app-appvisorpdf-03-fe (Delta Spec)

## ADDED Requirements

### Requirement: AppVisorPdf annotations SHALL be implemented via a dedicated AnnotateEngine
El sistema SHALL implementar anotaciones usando un engine separado `AnnotateEngine` y SHALL evitar
l\u00f3gica de Fabric dentro de componentes UI.

#### Scenario: Engine is separated from UI
- **WHEN** the user annotates a page
- **THEN** UI components SHALL delegate to `AnnotateEngine` methods and SHALL not contain direct Fabric implementation logic

#### Scenario: Engine lifecycle is explicit
- **WHEN** a page becomes visible or hidden due to virtualization
- **THEN** the system SHALL call `attach(pageNumber, overlayCanvas)` and `detach(pageNumber)` accordingly

### Requirement: AppVisorPdf SHALL support core annotation tools
El sistema SHALL soportar herramientas de anotaci\u00f3n: `freehand`, `text`, `rect`, `arrow`, `select`
alineadas con `AppVisorPdfTool`.

#### Scenario: Tool changes are supported
- **WHEN** the consumer sets a new tool
- **THEN** the engine SHALL update behavior via `setTool(tool)` deterministically

#### Scenario: Tool operates on active page overlay
- **WHEN** the active page overlay is attached
- **THEN** the selected tool SHALL create or manipulate objects on that page overlay only

### Requirement: AppVisorPdf SHALL provide undo/redo
El sistema SHALL soportar `undo()` y `redo()` con resultados consistentes.

#### Scenario: Undo reverts latest change
- **WHEN** user performs an action and then calls `undo()`
- **THEN** the latest action SHALL be reverted in the overlay state

#### Scenario: Redo reapplies reverted change
- **WHEN** user calls `redo()` after an undo
- **THEN** the reverted change SHALL be reapplied

### Requirement: AppVisorPdf SHALL serialize annotations to a versioned payload
El sistema SHALL serializar anotaciones a `VisorPdfAnnotationsPayloadV1` con `version: 1`,
`fingerprint?` y `pages[]` con `pageNumber` y `objects`.

#### Scenario: Serialize includes page and objects
- **WHEN** annotations exist on a page
- **THEN** `serialize()` SHALL include that page in `pages[]` with its objects payload

#### Scenario: Serialization is deterministic
- **WHEN** serialize is called multiple times without changes
- **THEN** the payload SHALL be stable and equivalent

### Requirement: AppVisorPdf SHALL restore annotations safely and be forward compatible
El sistema SHALL restaurar anotaciones desde un payload y SHALL ignorar objetos desconocidos de forma segura.

#### Scenario: Restore rehydrates objects
- **WHEN** `restore(payload)` is called with known objects
- **THEN** the engine SHALL rehydrate the objects onto their respective page overlays

#### Scenario: Unknown objects are ignored safely
- **WHEN** payload includes unknown or unsupported objects
- **THEN** restore SHALL not crash and SHALL ignore those objects while preserving supported ones


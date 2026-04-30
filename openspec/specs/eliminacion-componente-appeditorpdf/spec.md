# eliminacion-componente-appeditorpdf Specification

## Purpose
Definir la eliminación completa del componente `AppEditorPdf` y la limpieza de referencias en el proyecto.

## Requirements

### Requirement: AppEditorPdf component SHALL be removed from shared UI
El sistema SHALL eliminar `src/app/Components/UI/AppEditorPdf/` y SHALL not ship el componente.

#### Scenario: Component folder removed
- **WHEN** the repository is built
- **THEN** there SHALL be no `AppEditorPdf` folder nor imports referencing it

### Requirement: Barrels SHALL not export AppEditorPdf
El sistema SHALL remover exportaciones públicas del componente.

#### Scenario: UI index has no export
- **WHEN** importing from `src/app/Components/UI`
- **THEN** `AppEditorPdf` SHALL not be exported

### Requirement: Consumers SHALL not reference AppEditorPdf or helpers
El sistema SHALL eliminar referencias en `src/modules/**` y en particular en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`.

#### Scenario: No remaining references
- **WHEN** running a global search for `AppEditorPdf`
- **THEN** it SHALL return zero matches under `src/`

### Requirement: Repository tests SHALL remain green
El sistema SHALL actualizar o eliminar tests afectados y SHALL keep `npm.cmd test` passing.


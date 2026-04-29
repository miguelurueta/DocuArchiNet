# Spec: app-appeditorpdf-20-fe

## Capability

`app-appeditorpdf-20-fe`

Estandarizar la implementacion visual del tab **Documentos** dentro de `GestionRespuesta` (gestionCorrespondencia), utilizando componentes shared existentes (ej. `AppEditorPdf` / UI base) y evitando duplicacion de logica o componentes.

## Out Of Scope

- Nuevas features no requeridas por el tab.
- Refactors amplios de navegacion o layout global del modulo.
- Cambios al engine `AppEditor` no necesarios para visual.

## Requirements

### Visual Consistency

- El tab Documentos debe mantener consistencia con el look&feel del workbench (tipografia, spacing, acciones).
- Debe respetar layout responsive (desktop/tablet/mobile).

### Shared UI

- Priorizar componentes shared existentes antes de crear uno nuevo.
- Si se usa `AppEditorPdf`, su configuracion debe vivir en el consumidor (modulo), sin modificar el componente shared salvo que el contrato lo requiera.

### Stability

- Cambios visuales no deben romper:
  - scroll del panel
  - foco/teclado
  - comportamiento del editor principal

## Acceptance Criteria

1. Tab Documentos renderiza UI alineada al diseño esperado sin duplicar componentes shared.
2. No introduce flicker ni regresiones de navegacion dentro de GestionRespuesta.
3. Responsive: el tab se comporta correctamente en 1024px y 768px.

## Tests

- UI integration tests del tab Documentos (render y acciones base).


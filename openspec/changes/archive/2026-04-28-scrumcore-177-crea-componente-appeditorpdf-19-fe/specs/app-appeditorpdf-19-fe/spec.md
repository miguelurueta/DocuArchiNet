# Spec: app-appeditorpdf-19-fe

## Capability

`app-appeditorpdf-19-fe`

Ajustes de listas y margenes en `AppEditorPdf`, y remocion de wrappers innecesarios que afectan layout/estabilidad visual.

## Out Of Scope

- Cambios al engine `AppEditor` (este ticket se limita a `AppEditorPdf`).
- Cambios funcionales no relacionados a listas/margenes/layout.

## Requirements

### Lists

- Listas (ul/ol) deben respetar:
  - margenes de pagina (pageMargins)
  - consistencia visual entre hoja y area de lectura
- Evitar overflow lateral por indentacion excesiva.

### Margins

- Margenes efectivos deben aplicarse de forma consistente en:
  - contenido del editor
  - guias/overlays visuales

### Wrapper Removal

- Remover wrappers que:
  - dupliquen padding/margenes
  - introduzcan scroll anidado
  - causen reflow/repaint innecesario

## Acceptance Criteria

1. Listas no se desbordan horizontalmente en A4 portrait con margenes default.
2. Cambiar `pageMargins` actualiza layout sin flicker.
3. Eliminacion de wrapper no cambia el contrato publico pero reduce complejidad DOM.

## Tests

- Integration: render de contenido con listas y margenes.
- Snapshot/DOM assertions: no hay wrapper extra (si aplica).


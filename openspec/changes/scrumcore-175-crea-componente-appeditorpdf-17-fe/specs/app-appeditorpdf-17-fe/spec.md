# Spec: app-appeditorpdf-17-fe

## Capability

`app-appeditorpdf-17-fe`

Extiende `AppEditorPdf` para exponer un boton Guardar y un contrato de estado dirty (cambios pendientes), de forma reusable y sin acoplarse a modulos consumidores.

## Out Of Scope

- Persistencia backend real (solo contrato de UI/estado).
- Logica de negocio de modulos.
- Cambios de paginacion estructural.

## Requirements

### Dirty State

- Debe existir una forma tipada de representar:
  - `savedValue` vs `currentValue`
  - `isDirty` (derivado)
  - `saveStatus` (idle/saving/saved/error o equivalente)
- Debe soportar uso controlado y no controlado del editor.

### Save Action (UI)

- `AppEditorPdf` debe permitir componer un action "Guardar" en su toolbar (o exponer `AppEditorPdfSaveAction` equivalente).
- El action debe poder renderizarse:
  - icon-only
  - con texto "Guardar" (segun consumidor)
- Debe ser accesible (`aria-label` cuando sea icon-only).

### UX / Stability

- Guardar no debe:
  - perder foco/seleccion
  - reiniciar el editor
  - producir flicker

## Acceptance Criteria

1. Cuando `currentValue !== savedValue`, `isDirty` es true y el UI lo refleja (status).
2. Al ejecutar Guardar (callback), `savedValue` se actualiza y `isDirty` vuelve a false.
3. El action es accesible (nombre de boton correcto).

## Tests

- Unit: derivacion de `isDirty` y transiciones de `saveStatus`.
- Integration UI: render de action y callback de guardado.


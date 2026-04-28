# Design: SCRUMCORE-175 (AppEditorPdf - Guardar + Dirty State)

## Goal

Incorporar un contrato reusable de "dirty state" y una accion de Guardar en `AppEditorPdf`, manteniendo el engine compartido y sin acoplarse a modulos.

## Proposed Approach

- Reusar la infraestructura existente del editor shared:
  - `useAppEditorSaveState` y `AppEditorSaveAction` (si ya existen) mediante re-export en `AppEditorPdf`.
- `AppEditorPdf` no implementa persistencia; solo expone hooks/acciones y permite componer toolbar.

## State Model

- `currentValue`: valor actual del editor (controlado o interno).
- `savedValue`: snapshot guardado.
- `isDirty = currentValue !== savedValue` (normalizado).
- `saveStatus`: estado derivado para UI.

## UX Considerations

- Guardar no debe tomar foco del editor permanentemente.
- El action debe ser icon-only seguro (requiere `aria-label`).

## Non-Goals

- Manejo de errores del backend.
- Auto-save.


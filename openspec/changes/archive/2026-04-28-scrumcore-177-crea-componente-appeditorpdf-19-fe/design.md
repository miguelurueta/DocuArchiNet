# Design: SCRUMCORE-177 (AppEditorPdf - Listas, Margenes y Wrapper)

## Goal

Normalizar el layout de `AppEditorPdf` para que listas y contenido respeten margenes de pagina, y eliminar wrappers redundantes que empeoran la experiencia (scroll anidado, padding duplicado, etc.).

## Approach

- Revisar DOM actual de `AppEditorPdf`:
  - identificar wrappers que solo agregan layout (sin logica).
- Consolidar reglas CSS del wrapper:
  - un solo responsable de padding/margenes
  - evitar doble inset (shell + editor + guides)

## List Handling

- Controlar indentacion de listas via CSS en el scope de `AppEditorPdf`:
  - clamp max indent
  - asegurar que `contentWidth` sea limite visual

## Non-Goals

- Cambiar estilos globales del editor shared.
- Reescritura de schema de contenido.


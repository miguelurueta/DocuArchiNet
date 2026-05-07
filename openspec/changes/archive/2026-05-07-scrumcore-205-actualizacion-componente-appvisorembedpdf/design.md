## Context

SCRUMCORE-205 pide implementar thumbnails colapsables en `AppVisorEmbedPdf` usando plugin oficial EmbedPDF, manteniendo:
- arquitectura modular existente
- encapsulación (Workbench/consumers no conocen EmbedPDF)
- performance (virtualización/RenderLayer) y estabilidad de hooks

Actualmente `AppVisorEmbedPdf` ya incluye: document manager, viewport, scroll, render, zoom + toolbar (SCRUMCORE-204).

## Goals

- Agregar thumbnails colapsables **solo visual/layout** (UI tipo visor profesional).
- Usar exclusivamente `@embedpdf/plugin-thumbnail` (sin lógica custom).
- Reutilizar el toolbar existente agregando un botón `[☰ Thumbnails]` (o equivalente).
- Mantener render principal (Viewport/Scroller/RenderLayer) sin regresiones.

## Non-goals

- Search, annotations, rotate, print/download (salvo que el ticket lo exija explícitamente).
- Sidebars complejos, tabs, panel managers.
- Navegación manual de páginas (si el plugin oficial la maneja, se deja nativo).
- Crear `AppPdfThumbnailPanel.tsx` o wrappers innecesarios.

## Risks

- Rerenders masivos si el panel de thumbnails no está memoizado/aislado.
- Hook order/context issues si hooks del plugin se ejecutan fuera del provider `<EmbedPDF>`.

## Open questions (para tasks)

- ¿El panel es izquierdo o derecho?
- ¿Collapsed por defecto o abierto?
- Confirmar icono/texto del botón en toolbar (ej. `☰ Thumbnails`).

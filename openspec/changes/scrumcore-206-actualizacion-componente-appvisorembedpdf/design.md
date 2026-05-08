## Context

SCRUMCORE-206 pide extender `AppVisorEmbedPdf` implementando el plugin oficial de rotación (Rotate) de EmbedPDF, manteniendo:
- arquitectura modular desacoplada
- encapsulación (Workbench no conoce plugins EmbedPDF)
- performance (virtualización/RenderLayer) y estabilidad de hooks

Actualmente `AppVisorEmbedPdf` ya incluye: Pdfium engine, scroll + render + viewport, zoom + toolbar, thumbnails colapsables.

## Goals

- Agregar Rotate Plugin oficial de EmbedPDF (`@embedpdf/plugin-rotate`).
- Extender toolbar existente para exponer acciones mínimas de rotación (según prompt del ticket).
- Mantener todas las reglas enterprise: no lógica custom de render/engine/plugins, solo integración y UI.

## Non-goals

- Search, annotations, print/download, password, toolbar compleja fuera del alcance del prompt.

## Risks

- Rerenders masivos si la toolbar no está memoizada/handlers inestables.
- “Rules of Hooks” si hooks del plugin se ejecutan fuera del provider `<EmbedPDF>`.

## Open questions (para tasks)

- ¿Rotación soportada: 90° derecha/izquierda, reset a 0°, o ambos?
- ¿Rotación por documento o por página (según plugin)?

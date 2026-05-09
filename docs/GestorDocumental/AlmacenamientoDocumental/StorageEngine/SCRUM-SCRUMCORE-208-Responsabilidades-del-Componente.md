# SCRUMCORE-208 — Responsabilidades del Componente

## Qué encapsula `AppVisorEmbedPdf`
- Integración con EmbedPDF/Pdfium.
- Registro de plugins oficiales y su pipeline de render.
- Estados UI (loading/error/empty) del visor.
- UI presentacional del visor (toolbar/overlay) sin exponer lógica al Workbench.

## Paginación (este ticket)
- **Fuente de verdad**: `useScroll(documentId)` del plugin oficial.
- Renderiza un overlay de navegación (prev/indicador/next).
- Expone navegación únicamente a través de `scroll.provides` (guard clauses si no existe).

## Qué NO debe hacer
- No calcula página visible manualmente.
- No usa listeners DOM u observers custom para paginación.
- No mueve estado/lógica al `DocumentosWorkbench`.


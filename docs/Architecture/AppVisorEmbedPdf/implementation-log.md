# Implementation Log — AppVisorEmbedPdf

## Capa 01 (SCRUMCORE-197) — Scaffold + contratos

- Se crea `src/app/Components/UI/AppVisorEmbedPdf/` con export público.
- Se definen tipos y contratos:
  - `AppPdfSource`, `AppPdfLoadState`, `AppPdfCapabilities`
  - `AppPdfViewerApi`
  - `EmbedPdfEngine` (`load/renderPage/destroy`)
- Se documenta el módulo y se referencia la arquitectura.

## Capa 02A/02B (SCRUMCORE-198) — EmbedPDF nativo (core visual + hardening mínimo)

- Se implementa `AppVisorEmbedPdf` como wrapper nativo de EmbedPDF:
  - Engine PDFium vía `usePdfiumEngine()`
  - Plugins nativos (DocumentManager/Viewport/Scroll/Render/Zoom/InteractionManager) registrados como `PluginBatchRegistrations`
  - Toolbar mínima con `zoomIn/zoomOut/fit-width` usando hooks nativos del plugin zoom
- Se integra en `DocumentosWorkbench.tsx` bajo feature flag `VITE_ENABLE_EMBEDPDF`.


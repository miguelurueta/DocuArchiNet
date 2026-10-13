# DOC-100 — Planificador de viewport

## Objetivo y alcance

Corregir la planificación cliente del visor PDF para evitar renderizados duplicados u obsoletos y hacer honesta la evidencia de rendimiento. El cambio se limita al visor JavaScript y al runner E2E existente; no modifica backend, handler, WebMethod, iTextSharp, persistencia ni guardado de grafos.

## Componentes y dependencias

- Visor: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`.
- Coordinación preservada sin cambios: `pdfjs/pdf_anotate_view/script.js`.
- Runner y pruebas: `tools/e2e/`.
- Dependencias existentes: PDF.js, jQuery, DOM, `IntersectionObserver`, `requestAnimationFrame` y Playwright.
- Adaptadores/hook equivalentes: callbacks `onDocumentReady`, `onNewPage`, `onPageRender`, `onZoomChange` y `onActivePageChanged`.

## Documentos

1. `01-Arquitectura.md`
2. `02-FlujoIntegracion.md`
3. `03-ContratoUploadYMapping.md`
4. `04-EstadosErroresYAntiregresion.md`
5. `05-PruebasEvidencia.md`
6. `06-Diagramas.md`
7. `07-Metadata.md`

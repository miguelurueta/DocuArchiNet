# PLANIFICADOR-VIEWPORT

- Ticket: DOC-100
- Cambio OpenSpec: doc-100-planificador-viewport
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Evitar renders duplicados u obsoletos mediante una cola secuencial por página/generación, cancelación de `RenderTask`, viewport acotado y evidencia E2E honesta y saneada.

## Alcance y compatibilidad

El cambio afecta `pdf_viewer_anotate.js` y `tools/e2e`; `script.js`, backend y persistencia permanecen sin cambios. Conserva APIs, IDs DOM, página activa, scroll, zoom, rotación y anotaciones. La reversa consiste en revertir estos archivos y no requiere migración. El paquete canónico está en `Doc/Actualizacion/VisorPdf/Implementacion/DOC-100/`.

## Why

El visor PDF de Workflow libera la cola antes de que termine el render, admite trabajos duplicados u obsoletos y recorre todas las páginas durante scroll y zoom. La línea base autorizada confirmó 3.548 ms bloqueantes en scroll, 435 ms en zoom y un error JavaScript por iteración que el runner actual cuenta pero aun declara como `PASSED`.

## What Changes

- Clasificar errores del navegador mediante códigos y metadatos saneados, diferenciando frame principal, visor, worker y recursos opcionales.
- Emitir estados finales `PASSED`, `PASSED_WITH_OBSERVATIONS` o `FAILED` según hitos funcionales y errores clasificados.
- Convertir la carga/render de páginas en una cola secuencial, deduplicada y esperable.
- Identificar trabajos por página y generación de zoom/rotación, cancelar tareas obsoletas e impedir commits tardíos.
- Mantener un conjunto acotado de páginas visibles/adyacentes y coalescer scroll y zoom.
- Preservar contratos públicos, página activa, posición, anotaciones, rotación y comportamiento de error por página.
- Añadir pruebas Node focales, política de sanitización y evidencia E2E de lectura cuando exista autorización.

## Capabilities

### New Capabilities
- `planificador-viewport`: planificación observable, deduplicada, cancelable y acotada del render PDF.

### Modified Capabilities

- Ninguna especificación principal existente cambia; el visor legacy conserva sus contratos públicos.

## Impact

- Producción: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js` y, solo si la coordinación existente lo exige, `pdfjs/pdf_anotate_view/script.js`.
- Automatización: `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs` y pruebas Node bajo `tools/e2e/tests/`.
- Documentación: exploración vigente y paquete técnico de DOC-100.
- Fuera de alcance: handler, WebMethod, iTextSharp, persistencia, streaming, dependencias y guardado de grafos.


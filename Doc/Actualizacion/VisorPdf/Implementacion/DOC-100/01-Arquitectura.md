# Arquitectura

El visor conserva `PDFjsViewer` como contenedor de presentación y coordinación. El planificador vive en la misma clase porque necesita el PDF activo, las páginas, los callbacks y las capas de anotación existentes.

## Decisiones

- Un trabajo se identifica como `pageNumber:generation` y se registra en `_renderJobs`.
- `_loadingTask` mantiene un trabajador secuencial, espera `getPage` y `_renderPage`, y libera el estado en `finally`.
- `_renderTasks` conserva la tarea cancelable de PDF.js por página.
- Zoom o reinicialización incrementan `_renderGeneration`; una terminación antigua no instala canvas.
- `IntersectionObserver` mantiene intersecciones; el fallback geométrico solo se usa si el API no está disponible o aún no emitió.
- Scroll y zoom programan una reconciliación por frame.

Se descartó crear otro módulo o framework porque habría duplicado el estado legacy y alterado contratos globales. También se descartaron esperas fijas y renderizar todas las páginas.

## Responsabilidades

| Función | Ruta | Ubicación | Parámetros | Responsabilidad |
| --- | --- | --- | --- | --- |
| `_advanceRenderGeneration` | `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js` | `PDFjsViewer` | ninguno | Invalida cola y cancela renders vigentes. |
| `_scheduleVisiblePages` | misma | `PDFjsViewer` | `forceRedraw` | Coalesce reconciliaciones por frame. |
| `_setupViewportObserver` | misma | `PDFjsViewer` | ninguno | Configura observación acotada del viewport. |
| `loadPages` | misma | `PDFjsViewer` | páginas | Valida, identifica y deduplica trabajos. |
| `_loadingTask` | misma | `PDFjsViewer` | ninguno | Ejecuta secuencialmente y continúa tras error. |
| `_renderPage` | misma | `PDFjsViewer` | página PDF.js, número, trabajo | Retiene `RenderTask` y protege el commit. |
| `classifyBrowserFailure` | `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs` | runner | evento normalizado | Produce error saneado y estable. |
| `deriveResult` | misma | runner | hitos y errores | Deriva el estado final real. |

<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-100 corrige el planificador cliente del visor PDF y la interpretación de su evidencia E2E. La línea base autoritativa usa un PDF de 34 páginas y 33.589.755 bytes: primer canvas 2.902 ms en frío, mediana 878 ms, scroll 3.902 ms con 3.548 ms en tareas largas, zoom 779 ms con 435 ms bloqueantes y un `pageerror` por iteración.

El código vigente concentra la responsabilidad en:

- `PDFjsViewer.setZoom`, `_setScrollListener`, `_visiblePages`, `loadPages`, `_loadingTask`, `_cleanPage`, `_renderPage` y `rotate` en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`.
- `load_pdf_viwer_anotation` y los handlers de toolbar en `pdfjs/pdf_anotate_view/script.js`.
- `installPerformanceProbe`, `captureViewerMetrics`, `measureScroll`, `measureZoom`, `runMeasurement` y `main` en el runner E2E.
- Los contratos de política en `tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs`.

## Goals / Non-Goals

**Goals**

- Mantener un único render efectivo por página y generación.
- Esperar el fin real del render antes de liberar el trabajador secuencial.
- Cancelar trabajo innecesario y descartar resultados de generaciones antiguas.
- Acotar el trabajo de viewport y coalescer ráfagas de scroll/zoom.
- Clasificar errores sin persistir información documental o de sesión.
- Diferenciar una corrida limpia, una medición válida con observaciones y un fallo funcional.

**Non-Goals**

- Cambiar transporte HTTP, backend, sesión ASP.NET, iTextSharp o persistencia.
- Cambiar IDs DOM, firmas públicas o mecanismos de grafos, firma e impresión.
- Introducir frameworks, workers adicionales o un arnés E2E paralelo.
- Ejecutar E2E autenticada sin autorización vigente.

## Decisions

### D-01 — Clasificación saneada en la frontera E2E

El runner convertirá `error`, `unhandledrejection`, `pageerror`, respuestas HTTP y fallos de recursos en registros con campos permitidos: `code`, `source`, `type`, `fileBase` y `line`. `source` será uno de `main_frame`, `viewer`, `worker` u `optional_resource`. Se descartarán mensaje libre, stack completo, URL, query, rutas, tarea, usuario, documento, cookies y contenido.

Los errores requeridos del visor/worker/frame o un hito funcional ausente son fallo. Un recurso explícitamente opcional puede producir observación si visualización, scroll y zoom completan.

### D-02 — Estado final derivado, no constante

Una función pura derivará:

- `PASSED`: hitos completos y cero errores clasificados.
- `PASSED_WITH_OBSERVATIONS`: hitos completos y solo observaciones opcionales conocidas.
- `FAILED`: hito funcional incompleto, error requerido, error no clasificado o violación de sanitización.

`main` imprimirá el estado derivado y devolverá código distinto de cero para `FAILED`.

### D-03 — Cola por página y generación

Cada trabajo tendrá identidad `pageNumber:generation` y estado:

```text
idle -> queued -> rendering -> rendered
                 |              |
                 +-> cancelled <-+
```

La cola deduplicará identidades, descartará páginas inválidas y tendrá un único trabajador. `_loadingTask` esperará `getPage` y la promesa retornada por `_renderPage`; liberará `_loading` en `finally` y continuará después de error o cancelación.

### D-04 — Cancelación y commit protegido

El visor conservará el `RenderTask` efectivo por página. Zoom y rotación incrementarán la generación. Una tarea que salga del conjunto necesario o pertenezca a una generación anterior se cancelará cuando PDF.js lo permita. La finalización solo reemplazará el canvas si identidad y generación siguen vigentes.

### D-05 — Viewport acotado y eventos coalescidos

Un `IntersectionObserver` con raíz en el contenedor mantendrá las páginas intersectadas y su razón visible. La página activa será la de mayor intersección y la carga incluirá `extraPagesToLoad` adyacentes. Si el API no existe, se conservará un fallback geométrico coalescido.

Scroll se agrupará por `requestAnimationFrame`. Zoom/rotación invalidarán una sola generación y programarán una única reconciliación visible; no se usarán esperas fijas.

### D-06 — Compatibilidad visual y pública

Se conservarán `setZoom`, `loadPages`, `scrollToPage`, `rotate`, callbacks, IDs DOM y el símbolo léxico `let pdfViewer`. La reconciliación preservará página activa, scroll proporcional, overlays/anotaciones y rotación. El fallo de una página mostrará su estado de error sin bloquear las demás.

### D-07 — Verificación y entrega

Las pruebas Node usarán la infraestructura existente con dobles controlados de PDF.js/DOM. Se ejecutarán validación sintáctica y `test:workflow:pdf-viewer-performance:policy`. La E2E real será solo de lectura, reutilizará el runner oficial y se ejecutará únicamente con autorización; de lo contrario se documentará el bloqueo sin afirmar éxito remoto.

## Risks / Trade-offs

- `IntersectionObserver` entrega callbacks asíncronos; el fallback mantiene compatibilidad, pero debe evitar reintroducir el escaneo por evento.
- Cancelar PDF.js produce rechazos esperados que deben clasificarse como cancelación, no como error funcional.
- Conservar overlays durante zoom exige separar el commit del canvas de la capa de anotación.
- El error original solo puede declararse corregido después de una reproducción E2E autorizada.

## Migration Plan

1. Añadir primero contratos de clasificación y planificación con dobles.
2. Implementar estados del runner sin alterar el recorrido autenticado.
3. Introducir cola/generaciones y luego viewport/coalescencia.
4. Ejecutar pruebas focales y validación sintáctica.
5. Ejecutar E2E autorizada contra la misma línea base o registrar bloqueo.
6. Revertir los archivos JavaScript y del runner del cambio si aparece regresión; no requiere migración de datos ni configuración.

## Open Questions

- El código exacto y archivo propietario del `pageerror` se determinarán en la primera reproducción autorizada. Si el propietario está fuera del planificador, DOC-100 solo dejará clasificación y trazabilidad, sin corregir otro componente.

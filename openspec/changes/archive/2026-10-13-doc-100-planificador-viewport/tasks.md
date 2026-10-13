<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Observabilidad E2E

- [x] 1.1 [S] Añadir contratos unitarios para códigos, orígenes y campos permitidos de error. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs`. Origen: D-01, RQ-01. Verificación: `node --test tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs` cubre frame, visor, worker, recurso opcional y categoría desconocida.
- [x] 1.2 [M] Implementar clasificación y sanitización de `error`, `unhandledrejection`, `pageerror`, HTTP y recursos. Área/archivos: `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs`. Origen: D-01, RQ-01. Verificación: la prueba focal demuestra ausencia de URL, query, ruta, tarea, documento, credenciales, mensajes y stacks.
- [x] 1.3 [S] Añadir contratos para `PASSED`, `PASSED_WITH_OBSERVATIONS` y `FAILED`. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs`. Origen: D-02, RQ-02. Verificación: cada combinación de hitos y categorías produce un único estado esperado.
- [x] 1.4 [S] Sustituir el resultado constante por estado derivado y código de salida coherente. Área/archivos: `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs`. Origen: D-02, RQ-02. Verificación: una corrida con errores nunca imprime `WORKFLOW_PDF_PERF_RESULT=PASSED`.

## 2. Cola y generaciones de render

- [x] 2.1 [M] Crear pruebas focales del planificador con dobles de PDF.js/DOM para cola vacía, duplicados, orden, `getPage` fallido y render fallido. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-scheduler.test.cjs`, `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-03, RQ-03. Verificación: cada identidad página/generación inicia como máximo un render y la cola continúa después de error.
- [x] 2.2 [M] Incorporar identidad, estados y deduplicación de trabajo por página/generación. Área/archivos: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-03, RQ-03. Verificación: las pruebas focales cubren `idle -> queued -> rendering -> rendered/cancelled`.
- [x] 2.3 [M] Hacer que `_loadingTask` espere `getPage` y `_renderPage`, libere en `finally` y procese el siguiente trabajo. Área/archivos: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-03, RQ-03. Verificación: no hay concurrencia efectiva del trabajador y un rechazo no bloquea páginas posteriores.
- [x] 2.4 [M] Añadir pruebas de cancelación por viewport, zoom/rotación rápida y finalización tardía. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-scheduler.test.cjs`. Origen: D-04, RQ-04. Verificación: un trabajo obsoleto no reemplaza el canvas vigente y la cancelación esperada no se reporta como fallo.
- [x] 2.5 [M] Conservar `RenderTask`, incrementar generación e impedir commits obsoletos. Área/archivos: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-04, RQ-04. Verificación: las pruebas focales observan cancelación y exactamente un canvas vigente por página.

## 3. Viewport e interacción

- [x] 3.1 [M] Añadir pruebas del conjunto visible/adyacente, página activa, fallback y coalescencia. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-scheduler.test.cjs`. Origen: D-05, RQ-05. Verificación: ráfagas de scroll/zoom provocan máximo una reconciliación por frame y no recorren todas las páginas en la ruta con observer.
- [x] 3.2 [M] Mantener intersecciones y página activa mediante `IntersectionObserver` con fallback geométrico coalescido. Área/archivos: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-05, RQ-05. Verificación: solo visibles y `extraPagesToLoad` entran a la cola y el fallback conserva funcionalidad.
- [x] 3.3 [S] Coalescer scroll, zoom y rotación y eliminar la rama redundante de `estate_anotate`. Área/archivos: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`. Origen: D-05, RQ-05. Verificación: las pruebas observan una reconciliación por frame sin espera fija.
- [x] 3.4 [M] Añadir regresiones de página activa, scroll proporcional, overlays, anotaciones, rotación y aislamiento de error por página. Área/archivos: `tools/e2e/tests/workflow-pdf-viewer-scheduler.test.cjs`, `pdfjs/pdf_anotate_view/script.js`. Origen: D-06, RQ-06. Verificación: las firmas públicas e IDs DOM permanecen y el fallo de una página no detiene las demás.
- [x] 3.5 [S] Ajustar únicamente la coordinación UI necesaria sin cambiar contratos públicos. Área/archivos: `pdfjs/pdf_anotate_view/script.js`. Origen: D-06, RQ-06. Verificación: zoom, rotación, navegación, firma e impresión continúan invocando las firmas existentes; si no se requiere cambio, registrar evidencia y no editar el archivo.

## 4. Validación y evidencia

- [x] 4.1 [S] Ejecutar validación sintáctica y pruebas Node focales/política. Área/archivos: JavaScript modificado y `tools/e2e`. Origen: D-07, RQ-07. Verificación: `node --check` y `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy` terminan con código 0.
- [x] 4.2 [M] Ejecutar tres iteraciones E2E de lectura con el PDF autorizado y comparar carga, scroll, zoom, renders y errores contra la línea base. Área/archivos: runner y artefacto saneado de `tools/e2e`. Origen: D-01, RQ-01; D-07, RQ-07. Verificación: autorización vigente, evidencia sin datos sensibles y estado final coherente; si no se autoriza, registrar bloqueo sin afirmar corrección real.

  Evidencia final del 2026-10-13: segunda corrida con hitos completos y evidencia saneada; bloqueo de scroll -21,8 % y bloqueo de zoom -76,1 % frente a línea base. El estado `FAILED` es coherente por `pdf.worker.entry.min.js`, propietario de DOC-101.
- [x] 4.3 [M] Actualizar exploración y paquete técnico DOC-100 con arquitectura, contratos, estados, pruebas, métricas, comandos, riesgos y rollback. Área/archivos: `Doc/Actualizacion/VisorPdf/Exploracion/01-diagnostico-rendimiento-visor-pdf-grafos.md`, `Doc/Tecnica/Opsxj/doc-100-planificador-viewport/`. Origen: D-07, RQ-07. Verificación: documentación sin marcadores, consistente con el diff y evidencia realmente ejecutada.

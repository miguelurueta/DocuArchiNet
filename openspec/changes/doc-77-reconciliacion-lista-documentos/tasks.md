<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Reconciliación autoritativa

- [x] 1.1 Crear `importar-servicio-web-reconciliation.js` con recuperación/reconciliación mediante la API moderna y sin polling. Origen: D-01, RQ-01.
- [x] 1.2 Implementar el mapeo conservador de estados backend, incluidos timeout, ausencia e incierto. Origen: D-02, RQ-02.

## 2. Lista documental y aislamiento

- [x] 2.1 Crear `importar-servicio-web-document-list-adapter.js` con validación estricta de tarea y documento autorizado. Origen: D-03, RQ-03.
- [x] 2.2 Implementar recorrido único, lote final y deduplicación por `DocumentId`. Origen: D-04, RQ-04.
- [x] 2.3 Encapsular la proyección sobre la lista existente y el fallback de refresco completo, sin interpretar `dato_lista`. Origen: D-05, RQ-05.
- [x] 2.4 Integrar “Ver documento importado”, conservación de filtros/scroll, limpieza de selección, foco y reapertura persistida. Origen: D-06, RQ-06.
- [x] 2.5 Registrar módulos nuevos en el `.vbproj` y reutilizar estilos del feature. Origen: D-05, RQ-05; también D-06, RQ-06.

## 3. Pruebas y evidencia

- [x] 3.1 Crear `Tests/importar-servicio-web-reconciliation-ui.test.cjs` para estados, reconciliación y reapertura. Origen: D-01, RQ-01; también D-02, RQ-02 y D-06, RQ-06.
- [x] 3.2 Crear `Tests/importar-servicio-web-document-list-adapter.test.cjs` para lote, deduplicación, fallback y acción de visualización. Origen: D-04, RQ-04; también D-05, RQ-05.
- [x] 3.3 Crear `Tests/importar-servicio-web-task-isolation.test.cjs` para cambio de tarea y rechazo de documentos no autorizados. Origen: D-03, RQ-03.
- [x] 3.4 Ejecutar pruebas focales, suite afectada y build MSBuild; registrar evidencia saneada. Origen: D-01, RQ-01; cubre D-02, D-03, D-04, D-05, D-06 y RQ-02, RQ-03, RQ-04, RQ-05, RQ-06.

## 4. Documentación y cierre

- [x] 4.1 Crear exclusivamente `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-77-reconciliacion-lista-documentos/` con arquitectura, mapping, aislamiento, deduplicación, pruebas y diagramas. Origen: D-05, RQ-05; también D-06, RQ-06.
- [x] 4.2 Ejecutar validación OpenSpec y E2E real autorizado del flujo completo, incluida reapertura y tarea incorrecta. Origen: D-03, RQ-03; también D-06, RQ-06.

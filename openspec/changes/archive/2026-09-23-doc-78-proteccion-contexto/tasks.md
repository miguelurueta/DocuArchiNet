<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Guard de contexto

- [x] 1.1 Crear `importar-servicio-web-task-context-guard.js` con captura inmutable, comparación de tarea y bloqueo reversible. Origen: D-01, RQ-01; también D-03, RQ-03.
- [x] 1.2 Integrar el preflight fresco inmediatamente antes del primer efecto y rechazar contexto divergente sin ejecutar. Origen: D-02, RQ-02.
- [x] 1.3 Declarar aditivamente en `workflow/Webworkflow.aspx` los controles incompatibles y mensajes de escritura, sin reemplazar handlers legacy. Origen: D-03, RQ-03.

## 2. Conflicto y recuperación

- [x] 2.1 Crear `importar-servicio-web-recovery.js` usando `GetImportIntent`/`ReconcileImportIntent` y solo `IntentId` autoritativo. Origen: D-05, RQ-05.
- [x] 2.2 Integrar los conflictos normativos, conservando resultados y deteniendo pendientes. Origen: D-04, RQ-04.
- [x] 2.3 Integrar detección de cambio de tarea/pestaña como señal de verificación y mantener aislamiento documental. Origen: D-06, RQ-06.
- [x] 2.4 Registrar módulos en el `.vbproj` y conservar el gate moderno. Origen: D-03, RQ-03; también D-05, RQ-05.

## 3. Pruebas y evidencia

- [x] 3.1 Crear `Tests/importar-servicio-web-task-context-guard.test.cjs`. Origen: D-01, RQ-01; también D-02, RQ-02 y D-03, RQ-03.
- [x] 3.2 Crear `Tests/importar-servicio-web-recovery.test.cjs`. Origen: D-04, RQ-04; también D-05, RQ-05.
- [x] 3.3 Crear `Tests/importar-servicio-web-multi-tab-context.test.cjs`. Origen: D-06, RQ-06.
- [x] 3.4 Ejecutar pruebas focales, regresión y MSBuild; registrar evidencia saneada. Origen: D-01, RQ-01; cubre D-02, D-03, D-04, D-05, D-06 y RQ-02, RQ-03, RQ-04, RQ-05, RQ-06.

## 4. Documentación y cierre

- [x] 4.1 Crear exclusivamente `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-78-proteccion-contexto-recuperacion/` con arquitectura, matriz, recuperación, estados, pruebas y diagramas. Origen: D-03, RQ-03; también D-04, D-05, D-06 y RQ-04, RQ-05, RQ-06.
- [x] 4.2 Ejecutar validación OpenSpec y E2E real autorizado; verificar restauración del gate. Origen: D-05, RQ-05; también D-06, RQ-06.

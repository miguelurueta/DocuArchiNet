<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## 1. Validación local y gate

- [x] 1.1 [S] Crear la suite de arquitectura UI que compruebe composición única y ausencia de infraestructura paralela. Área/archivos: `tests/importar-servicio-web-ui-architecture.test.cjs`. Origen: D-01, RQ-01; también D-03, RQ-03. Verificación: `node --test tests/importar-servicio-web-ui-architecture.test.cjs`.
- [x] 1.2 [M] Crear la suite del gate completo para las ocho operaciones y audiencias. Área/archivos: `tests/importar-servicio-web-gate.test.cjs`, servicio moderno y `web.config`. Origen: D-02, RQ-02. Verificación: prueba verde y corte anterior a dependencias/efectos.
- [x] 1.3 [S] Crear la suite de regresión UI legacy y doble handler. Área/archivos: `tests/importar-servicio-web-legacy-ui-regression.test.cjs`, `workflow/Webworkflow.aspx(.vb)`. Origen: D-03, RQ-03; también D-06, RQ-06. Verificación: gate apagado preserva legacy y gate activo expone una sola entrada.
- [x] 1.4 [S] Crear la suite de invariancia UI/almacenamiento. Área/archivos: `tests/importar-servicio-web-storage-invariance-ui.test.cjs`, `workflow/ClassAlmacenamiento.vb`, adaptador moderno. Origen: D-01, RQ-01; también D-06, RQ-06. Verificación: huellas legacy e invocación única permanecen válidas.
- [x] 1.5 [M] Crear el validador frontend determinista que componga suites existentes y nuevas. Área/archivos: `tools/validation/Verify-ImportarServicioWebFrontend.ps1`. Origen: D-01, RQ-01. Verificación: comando local sin red retorna cero y propaga fallos.

## 2. Alternancia y contratos de ejecución

- [x] 2.1 [M] Declarar el árbol visual legacy y aplicar ocultamiento inicial reversible bajo el gate sin eliminar controles ni handlers. Área/archivos: `workflow/Webworkflow.aspx(.vb)`, estilos/módulos existentes. Origen: D-03, RQ-03; también D-06, RQ-06. Verificación: suites de gate activo/apagado y revisión de markup.
- [x] 2.2 [M] Cubrir una sola ejecución por intención, espera global y proyección final sin duplicados. Área/archivos: pruebas del UI, progreso y reconciliación existentes. Origen: D-04, RQ-04. Verificación: conteo de `ExecuteImportIntent`, estados pendientes y claves documentales únicas.

## 3. E2E y evidencia

- [x] 3.1 [S] Extender solo la suite E2E compartida con las aserciones estructurales faltantes, sin crear escenarios o perfiles paralelos. Área/archivos: `tools/e2e/tests/importar-servicio-web-modern.spec.cjs`. Origen: D-05, RQ-05. Verificación: suite local del arnés pasa sin autenticación.
- [x] 3.2 [M] Ejecutar suites focales, regresión completa y compilación, registrando resultados reales. Área/archivos: tests Importar Servicio Web y solución WebForms. Origen: D-01, RQ-01; cubre D-02, D-03, D-04 y RQ-02, RQ-03, RQ-04. Verificación: conteos y errores documentados.
- [x] 3.3 [M] Ejecutar E2E real únicamente tras autorización explícita o registrar bloqueo verificable. Área/archivos: `tools/e2e`, perfil runtime autorizado y evidencia saneada. Origen: D-05, RQ-05. Verificación: controles esperados, resultado real y gate restaurado.

## 4. Inventario, documentación y cierre

- [x] 4.1 [M] Inventariar controles, postbacks, handlers y ASMX legacy, clasificando referencias y criterios de retiro futuro. Área/archivos: superficies legacy y documentación DOC-79. Origen: D-06, RQ-06. Verificación: matriz completa sin declarar eliminaciones dentro del cambio.
- [x] 4.2 [M] Crear el paquete técnico y diagramas exclusivamente en la ruta canónica DOC-79. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-79-pruebas-gate-transicion-legacy/`. Origen: D-07, RQ-07; también D-05, RQ-05 y D-06, RQ-06. Verificación: índice, arquitectura, pruebas, inventario, rollback y evidencia saneada presentes; `docs/` ausente.
- [x] 4.3 [S] Validar estrictamente OpenSpec y OPSXJ con evidencia ligada al SHA final. Área/archivos: cambio `doc-79-pruebas-retiro-gate` y gobierno OPSXJ. Origen: D-07, RQ-07; cubre D-01 a D-06 y RQ-01 a RQ-06. Verificación: `openspec validate --strict` y `opsxj:validate` exitosos.

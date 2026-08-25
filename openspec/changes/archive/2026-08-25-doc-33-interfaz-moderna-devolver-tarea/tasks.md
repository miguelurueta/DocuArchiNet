<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## 1. Refinamiento aprobado

- [x] 1.1 [S] Consolidar el alcance UI DOC-33 contra Jira, DOC-32, exploración y código actual. Área/archivos: `openspec/changes/doc-33-interfaz-moderna-devolver-tarea/refinement.md`. Origen: D-01, RQ-01. Verificación: `opsxj:refine` reconoce estado aprobado sin marcadores.
- [x] 1.2 [M] Convertir decisiones y requisitos en design, spec y tareas trazables. Área/archivos: `design.md`, `specs/interfaz-moderna-devolver-tarea/spec.md`, `tasks.md`. Origen: D-01, RQ-01. Verificación: `openspec validate doc-33-interfaz-moderna-devolver-tarea --strict`.

## 2. Presentación y preview

- [x] 2.1 [M] Sustituir el enlace `D-TASK-ANT` por trigger, modal, región de estado y mensaje de éxito exclusivos. Área/archivos: `workflow/Webworkflow.aspx`. Origen: D-02, RQ-01. Verificación: prueba CJS encuentra el markup exclusivo y ausencia de `onclick` legacy.
- [x] 2.2 [M] Registrar los assets y bootstrap de devolución sin consultar el feature gate ni compartir bootstrap de otras operaciones. Área/archivos: `workflow/Webworkflow.aspx.vb`. Origen: D-01, RQ-01. Verificación: prueba CJS verifica `data-workflow-return-activity-*` y scripts exclusivos.
- [x] 2.3 [M] Implementar normalización y solicitud ASMX del preview con payload mínimo y JSON autorizado. Área/archivos: `js/workflow/workflow-return-activity-ui.js`. Origen: D-03, RQ-02. Verificación: prueba de VM inspecciona URL, payload y normalización sin datos de otras transiciones.
- [x] 2.4 [M] Implementar búsqueda, debounce, cursor, paginación, cancelación, descarte obsoleto e invalidación de selección en el estado propio del modal. Área/archivos: `js/workflow/workflow-return-activity-ui.js`. Origen: D-03, RQ-02. Verificación: prueba de VM cubre búsqueda, página y respuesta obsoleta sin ejecución.
- [x] 2.5 [M] Incorporar renderizado accesible, foco, trampa, Escape, cancelación y vista responsive de destinos. Área/archivos: `js/workflow/workflow-return-activity-ui.js`, `Styles/workflow-transition-modern.css`, `js/workflow/centro-trabajo-visual.js`. Origen: D-05, RQ-04. Verificación: pruebas estáticas cubren atributos ARIA, teclado y reglas responsive.

## 3. Confirmación, resultado y retiro legacy

- [x] 3.1 [M] Crear la confirmación exclusiva que consume selección vigente y ejecuta solo tarea, conector y token. Área/archivos: `js/workflow/workflow-return-activity-confirmation.js`. Origen: D-04, RQ-03. Verificación: prueba de VM confirma payload, bloqueo, error y doble envío.
- [x] 3.2 [S] Integrar éxito de devolución con `WorkflowTransitionPagePresentation` sin actualizar otra tarea. Área/archivos: `js/workflow/workflow-return-activity-confirmation.js`, `js/workflow/workflow-transition-page-presentation.js`. Origen: D-05, RQ-04. Verificación: prueba de VM verifica `applySuccess` con la tarea elegida.
- [x] 3.3 [M] Retirar botón, diseñador, handler y callback de postback de actividad anterior sin tocar Usuario anterior. Área/archivos: `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.designer.vb`, `workflow/Webworkflow.aspx.vb`, `js/workflow/Webworkflow.js`. Origen: D-06, RQ-05. Verificación: búsqueda focal no encuentra la ruta legacy y sí conserva el trigger de Usuario anterior.

## 4. Pruebas, documentación y cierre técnico

- [x] 4.1 [M] Añadir pruebas CJS del preview, selección, paginación, accesibilidad y aislamiento de módulos. Área/archivos: `tests/workflow-return-activity-ui.test.cjs`. Origen: D-07, RQ-02. Verificación: `node --test tests/workflow-return-activity-ui.test.cjs`.
- [x] 4.2 [M] Añadir pruebas CJS de confirmación, ejecución, bloqueo, timeout, cancelación y éxito puntual. Área/archivos: `tests/workflow-return-activity-confirmation.test.cjs`. Origen: D-07, RQ-03. Verificación: `node --test tests/workflow-return-activity-confirmation.test.cjs`.
- [x] 4.3 [S] Extender regresión estática de backend/UI para ausencia de postback legacy e independencia con otras transiciones. Área/archivos: `tests/workflow-return-activity.test.cjs`. Origen: D-06, RQ-05. Verificación: `node --test tests/workflow-return-activity.test.cjs`.
- [x] 4.4 [M] Actualizar paquete técnico, diagramas y documentos OPSXJ con selectores, contratos, accesibilidad, no regresión y limitación E2E. Área/archivos: `Doc/Actualizacion/workflow/DebolverTarea/01-implementacion-devolver-actividad-anterior/`, `Doc/Tecnica/Opsxj/doc-33-interfaz-moderna-devolver-tarea/`. Origen: D-07, RQ-06. Verificación: revisión de enlaces y `opsxj:validate`.
- [x] 4.5 [M] Ejecutar pruebas focales, validación OpenSpec y MSBuild; registrar evidencia sin ejecutar E2E autenticada no autorizada. Área/archivos: `tests/`, `openspec/changes/doc-33-interfaz-moderna-devolver-tarea/`, `.opsxj/evidence/`. Origen: D-07, RQ-06. Verificación: `node --test ...`, `openspec validate --strict` y `msbuild ... /clp:ErrorsOnly`.
- [x] 4.6 [L] Crear corrida E2E DOC-33 reutilizable de interfaz para preview y devolución real con perfiles no sensibles, ODBC de solo lectura, recursos descartables separados y evidencia saneada. Área/archivos: `tools/e2e/`, `Doc/Actualizacion/workflow/DebolverTarea/02-interfaz-moderna-devolver-actividad-anterior/`. Origen: D-07, RQ-06. Verificación: pruebas CJS de orquestación y E2E autorizada de preview/devolución aprobadas.
- [x] 4.7 [M] Añadir E2E que retiene la respuesta de ejecución y prueba bloqueo de confirmación, cierre, Escape, modal y `beforeunload` hasta liberar el backend. Área/archivos: `js/workflow/workflow-return-activity-*.js`, `tests/`, `tools/e2e/tests/doc33-return-activity-ui.spec.cjs`. Origen: D-08, RQ-07. Verificación: CJS focal y E2E autorizada de bloqueo UI aprobadas con recurso independiente.

<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Refinamiento

- [x] 1.1 [M] Consolidar el alcance contra Jira, Prompts 05–08, DOC-83, commit `0e8199c8` y fronteras actuales. Área/archivos: `refinement.md`, código y documentación referenciada. Origen: D-01, RQ-01. Verificación: matriz D/RQ sin marcadores pendientes.
- [x] 1.2 [M] Formalizar decisiones, requisitos, riesgos, rollback y separación ENLASE/Workflow. Área/archivos: `design.md`, `specs/fix-sellos-sii/spec.md`. Origen: D-06, RQ-06. Verificación: `openspec validate doc-84-fix-sellos-sii --strict`.

## 2. Contrato y backend

- [x] 2.1 [M] Agregar el modelo interno y DTO público exclusivos de proyección Workflow. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`, `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`. Origen: D-01, RQ-01. Verificación: prueba estructural confirma tipos distintos de ENLASE y ocho campos.
- [x] 2.2 [M] Construir la proyección Workflow cerrada desde `idImagen`, `imagen` y comando tras almacenamiento exitoso. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb`. Origen: D-01, RQ-01. Verificación: pruebas cubren campos, fallback `DBT`→`extension` y rechazo de faltantes.
- [x] 2.3 [M] Propagar `WorkflowProjection` por fase, ítem, orquestador y DTO solo para estados confirmados. Área/archivos: `ImportExecutionSteps.vb`, `ImportServiceOrchestrator.vb`. Origen: D-02, RQ-02. Verificación: prueba de mapeo confirma documento/tarea y ausencia en estados no confirmados.
- [x] 2.4 [M] Preservar la proyección Workflow en la reconciliación inmediata con coincidencia estricta. Área/archivos: `ServicioReconciliacionImportacion.vb`. Origen: D-02, RQ-02. Verificación: pruebas positivas y negativas para capacidad, identidad, documento, tarea y campos obligatorios.

## 3. Frontera JavaScript e integración

- [x] 3.1 [M] Crear el appender Workflow con validación de ocho campos, escape, destino `wf`, deduplicación y comprobación posterior. Área/archivos: `js/workflow/importar-servicio-web/importar-servicio-web-workflow-document-list-adapter.js`. Origen: D-03, RQ-03. Verificación: prueba Node cubre inserción, duplicado, lote, delimitador, HTML y tarea distinta.
- [x] 3.2 [S] Mapear `WorkflowProjection` en progreso y reconciliación, preservándola al reemplazar snapshots. Área/archivos: `importar-servicio-web-progress-adapter.js`, `importar-servicio-web-reconciliation.js`. Origen: D-02, RQ-02. Verificación: pruebas de ambos adaptadores conservan ocho campos y no cruzan ENLASE.
- [x] 3.3 [M] Transportar ambas proyecciones sin mezclarlas y despachar el appender correcto por capacidad. Área/archivos: `importar-servicio-web-document-list-adapter.js`, `importar-servicio-web-ui.js`. Origen: D-03, RQ-03. Verificación: pruebas negativas impiden Workflow→`rad` y ENLASE→`wf`.
- [x] 3.4 [M] Eliminar el fallback de recarga del cierre moderno y mantener el modal abierto si la fila no se comprueba. Área/archivos: `importar-servicio-web-ui.js`. Origen: D-04, RQ-04. Verificación: prueba falla ante `Button_actualiza_trevie_seleccion`, `PageRequestManager`, postback o recarga y confirma cierre solo tras proyección.
- [x] 3.5 [S] Registrar el módulo Workflow y renovar versiones públicas de scripts afectados. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`, `workflow/Webworkflow.aspx.vb`. Origen: D-05, RQ-05. Verificación: prueba comprueba orden de carga y versiones DOC-84 nuevas.
- [x] 3.6 [S] Predeterminar una única tipología equivalente a Constancia de Inscripción antes del fallback obligatorio, tolerando variantes ortográficas menores y renovando caché. Área/archivos: `importar-servicio-web-preparation.js`, `Webworkflow.aspx.vb`. Origen: D-07, RQ-07. Verificación: pruebas cubren prioridad, variante `Contancia de Inscrpcion` y ambigüedad cerrada.

## 4. Pruebas y evidencia

- [x] 4.1 [M] Agregar pruebas backend del contrato y preservación Workflow sin alterar fixtures ENLASE. Área/archivos: `tests/importar-servicio-web-workflow-projection.test.cjs`, pruebas de ejecución/reconciliación. Origen: D-01, RQ-01. Verificación: suite focal backend PASS.
- [x] 4.2 [M] Agregar pruebas frontend de inserción, operabilidad contractual, deduplicación, tarea y ausencia de recarga. Área/archivos: pruebas de lista, UI, progreso y reconciliación. Origen: D-03, RQ-03. Verificación: suite focal frontend PASS.
- [x] 4.3 [M] Ejecutar regresión ENLASE y controles DOC-81/DOC-83 sin relajar aserciones. Área/archivos: `tests/importar-servicio-web-enlase-*`, runner DOC-83. Origen: D-06, RQ-06. Verificación: comandos determinísticos PASS; ninguna E2E autenticada ejecutada.
- [x] 4.4 [M] Ejecutar compilación y validaciones OpenSpec/OPSXJ, registrando cualquier fallo preexistente por separado. Área/archivos: solución, `openspec/changes/doc-84-fix-sellos-sii`. Origen: D-06, RQ-06. Verificación: build y `openspec validate --strict` PASS.
- [x] 4.5 [M] Ejecutar en Playwright local el appender Workflow contra `insert_row_documento_relacionado` real y comprobar `id_wf`, `idd_wf`, tipología, icono y las seis acciones sin red, autenticación ni recarga. Área/archivos: `tools/e2e/tests/doc84-workflow-document-row.spec.cjs`, `tools/e2e/package.json`. Origen: D-03, RQ-03. Verificación: `npm.cmd --prefix tools/e2e run test:doc84:workflow-row` PASS.

## 5. Documentación y cierre

- [x] 5.1 [M] Documentar causa, contratos, comparación ENLASE/Workflow, archivos y evidencia saneada. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-84-correccion-proyeccion-sellos-workflow/`, `Doc/Tecnica/Opsxj/doc-84-fix-sellos-sii/`. Origen: D-06, RQ-06. Verificación: contrato documental OPSXJ sin plantillas ni checklist abierto.
- [x] 5.2 [S] Registrar la aceptación E2E como no ejecutada salvo autorización explícita y conservar las restricciones del runbook. Área/archivos: documentación DOC-84. Origen: D-06, RQ-06. Verificación: evidencia distingue pruebas locales de E2E bloqueada y no contiene secretos.

<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-78-proteccion-contexto

## Fuente y alcance

- Ticket: `DOC-78` — PROTECCION-CONTEXTO
- Cambio OpenSpec: `doc-78-proteccion-contexto`
- Fuente Jira: `specs/proteccion-contexto/jira-context.md`
- Perfil tecnológico: `legacy-webforms-vb` con módulos JavaScript aislados y pruebas Node `node:test`.

## Contexto inspeccionado

- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`: `requestContext`, `prepareCurrent`, `executeCreatedIntent`, `close` e integración del modal.
- `js/workflow/importar-servicio-web/importar-servicio-web-intent-client.js`: separación actual entre preflight y creación de intención.
- `js/workflow/importar-servicio-web/importar-servicio-web-api.js`: operaciones modernas `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`.
- `js/workflow/importar-servicio-web/importar-servicio-web-reconciliation.js` y `importar-servicio-web-document-list-adapter.js`: recuperación y aislamiento por tarea ya disponibles.
- `workflow/Webworkflow.aspx`: tarea visible, modal de importación y acciones Workflow incompatibles.
- Compatibilidad a preservar: handlers globales, endpoints/ASMX, almacenamiento legacy, `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` y lista documental existente.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Capturar un contexto inmutable al preparar la intención y compararlo con la tarea visible antes de cada frontera relevante. | `importar-servicio-web-ui.js:requestContext`, `importar-servicio-web-preparation.js:normalize` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Ejecutar un preflight fresco inmediatamente antes del primer efecto; un contexto cambiado impide enviar la ejecución. | `importar-servicio-web-intent-client.js:preflight/confirm`, `importar-servicio-web-ui.js:executeCreatedIntent` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Publicar un guard aislado que bloquee controles declarados mediante atributos durante escritura, sin reemplazar handlers globales. | `workflow/Webworkflow.aspx`, `importar-servicio-web-ui.js:initialize` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Tratar únicamente `TASK_CONTEXT_MISMATCH` y `PERSISTED_CONTEXT_MISMATCH` como conflictos normativos, detener pendientes y reconciliar. | `importar-servicio-web-api.js`, `importar-servicio-web-reconciliation.js:complete` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Recuperar solo desde un `IntentId` autoritativo provisto por página/backend mediante API moderna; no usar `localStorage`, polling ni reintento ciego. | `importar-servicio-web-api.js:GetImportIntent/ReconcileImportIntent` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Detectar cambio de tarea en la misma pestaña y mediante señal multi-pestaña no autoritativa; nunca proyectar documentos sobre otra tarea. | `importar-servicio-web-document-list-adapter.js:currentTaskId/isAuthorized`, `Webworkflow.aspx:Hidden_id_tarea` | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La operación conserva tarea e identidad de intención originales. | WHEN cambia la tarea visible THEN el guard declara conflicto antes de continuar. | No sustituir la autoridad backend con estado cliente. |
| RQ-02 | El último preflight válido precede al primer efecto. | WHEN el contexto cambia entre preparación y ejecución THEN no se llama `ExecuteImportIntent`. | Conservar cancelación sin efectos durante preparación. |
| RQ-03 | Selección/búsqueda de tareas y acciones Workflow incompatibles quedan bloqueadas durante escritura. | WHEN inicia ejecución THEN controles declarados quedan deshabilitados y se restauran al finalizar. | Integración aditiva; no sobrescribir eventos legacy. |
| RQ-04 | Los conflictos normativos detienen pendientes y conservan resultados previos. | WHEN backend responde mismatch THEN se muestra conflicto y se consulta snapshot autoritativo. | No inventar `TASK_CONTEXT_CHANGED`. |
| RQ-05 | Recarga, cierre forzado o pérdida de conexión recuperan estado verificable. | WHEN existe `IntentId` autoritativo THEN se consulta/reconcilia sin reejecutar. | Sin `localStorage`, buscador cliente ni polling. |
| RQ-06 | Solo la tarea original recibe actualización documental. | WHEN otra pestaña o la vista cambia de tarea THEN no se sincroniza la lista y se exige recuperación. | `beforeunload` es advertencia, no garantía. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` aparece en design, spec y al menos una tarea.
2. Cada tarea conserva `Origen: D-XX, RQ-XX`.
3. El backend continúa como única autoridad de intención, tarea y ejecución.
4. El guard frontend es una defensa UX adicional, no una garantía de integridad.

## Resultado del refinamiento

- Estado: aprobado para sincronización OpenSpec e implementación posterior.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-78 --sync`.

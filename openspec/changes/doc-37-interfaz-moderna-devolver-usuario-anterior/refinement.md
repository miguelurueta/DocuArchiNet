<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-37-interfaz-moderna-devolver-usuario-anterior

## Fuente y alcance

- Ticket: `DOC-37` — INTERFAZ-MODERNA-DEVOLVER-USUARIO-ANTERIOR.
- Cambio OpenSpec: `doc-37-interfaz-moderna-devolver-usuario-anterior`.
- Fuente funcional: `specs/interfaz-moderna-devolver-usuario-anterior/jira-context.md`.
- Tecnología identificada: ASP.NET Web Forms, VB.NET y JavaScript sin framework.
- Predecesor verificado: DOC-36 está archivado, sus 23 tareas están completas y Jira está en estado Listo.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx` contiene el enlace de Usuario anterior con `D-TWU-ANT`, el control oculto `Button_tool_devolver_a_usuario` y el menú de Devolver; `js/workflow/Webworkflow.js` convierte ese código en confirmación nativa y postback.
- `workflow/Webworkflow.aspx.vb` registra sin feature gate la presentación de devolución de actividad y envío a usuario; `WorkflowTransitionModernActive` solo protege otras operaciones. `WorkflowTransitionPagePresentation` ya actualiza fila, visor, contador, listado y scroll de forma localizada.
- `js/workflow/workflow-return-activity-ui.js`, su integración de confirmación y `ConfirmationDialog.js` aportan el patrón accesible de modal, foco, trampa de foco, Escape, cancelación y bloqueo mientras una ejecución está en curso.
- `WebServiceWorkflowModern.asmx.vb` publica `PreviewDevolverUsuarioAnterior(idTarea)` y `EjecutarDevolverUsuarioAnterior(idTarea, tokenVersion)`. DOC-36 documenta que el preview devuelve solo actividad, usuario histórico y token opaco; el servidor revalida autorización, historial y concurrencia.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La presentación de Usuario anterior se registra para cada contexto Workflow válido y no consulta `WorkflowCentroTrabajoModernActive`. | `workflow/Webworkflow.aspx.vb`, `ConfigureWorkflowTransitionModernPresentation` y registro sin gate de `RegisterWorkflowReturnActivityModernPresentation`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El trigger exclusivo sustituye `D-TWU-ANT`, `Button_tool_devolver_a_usuario` y su handler; la devolución de actividad conserva su ruta actual. | `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/Webworkflow.aspx.designer.vb`, `js/workflow/Webworkflow.js`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El navegador envía únicamente `idTarea` al preview y `idTarea` más `tokenVersion` a la ejecución; usuario y actividad se muestran solo desde la respuesta del servidor. | `webservice/WebServiceWorkflowModern.asmx.vb`, `DTOs/Workflow/DevolverUsuarioAnterior/DevolverUsuarioAnteriorDtos.vb`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | El adaptador tiene IDs, eventos, estado y requests propios; reutiliza el diálogo compartido y protege foco, Escape, cancelación, doble clic y cierre mientras la ejecución está en curso. | `js/java_general/ConfirmationDialog.js`, `js/workflow/workflow-return-activity-ui.js`. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Un éxito solo actualiza la tarea afectada y la presentación moderna; bloqueos, conflictos y fallas de transporte conservan la bandeja y ofrecen recuperación controlada. | `js/workflow/workflow-transition-page-presentation.js` y códigos públicos DOC-36. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | DOC-37 reutiliza el runner, la sesión efímera, controles ODBC `SELECT` y reservas locales de DOC-36 con perfil propio no sensible y dos tareas UI aisladas. Exige una etapa por invocación para respetar la única tarea seleccionada de Workflow; actividad y destino proceden del preview vigente y no del perfil. | `tools/e2e/scripts/support/workflow-e2e-orchestrator.cjs`, adaptadores DOC-33/DOC-36 y perfiles de ejemplo. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | El workflow de CI fija OpenSpec 1.7.0 para que la validación completa del repositorio sea reproducible y no dependa de contratos introducidos por una versión más reciente. | `.github/workflows/opsxj-validation.yml` y validación completa de 30 especificaciones con OpenSpec 1.7.0. | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El comando moderno está disponible cuando el menú Workflow es válido aunque el gate de otras operaciones esté apagado. | Cuando se registra la página, entonces el bootstrap propio activa el trigger sin leer el feature gate. | No se altera la política de las otras operaciones modernas. |
| RQ-02 | No existe una ruta alcanzable del comando Usuario anterior hacia `D-TWU-ANT`, el botón oculto o un postback. | Cuando se pulsa Usuario anterior, entonces se abre solo el modal propio. | Devolver a actividad anterior conserva selector, modal y handler propios. |
| RQ-03 | Preview y ejecución usan exclusivamente los endpoints DOC-36 con payload mínimo. | Cuando el preview es elegible, entonces el modal muestra un único usuario y actividad del servidor; cuando no lo es, muestra el bloqueo recibido. | No se envían destino, historial, ruta, flujo, conector ni autorización desde el cliente. |
| RQ-04 | El flujo es accesible y no abandona una ejecución en curso. | Cuando se usa teclado, Escape, cancelación, doble clic o timeout, entonces foco y estado quedan definidos y no se duplica la solicitud. | No se crea modal paralelo ni estado compartido con otras operaciones. |
| RQ-05 | El éxito refresca solo la tarea afectada; los errores no mutan la bandeja. | Cuando la ejecución es exitosa, entonces se actualizan fila, visor, contador, listado y scroll; cuando se bloquea o falla, entonces la tarea permanece visible. | Se preservan contratos y eventos de los demás comandos Workflow. |
| RQ-06 | DOC-37 puede validarse en el mismo entorno de pruebas controlado por DOC-36 sin heredar secretos ni reutilizar una tarea mutante. | El perfil no sensible separa `uiExecution` y `uiLock`; el runner exige autorización y una etapa por invocación, y la prueba verifica el modal exclusivo y los endpoints DOC-36. | No se provisiona un ambiente; una E2E autenticada usa solo tareas descartables autorizadas. |
| RQ-07 | La validación CI usa una versión explícita de OpenSpec compatible con las especificaciones existentes. | Cuando OPSXJ valida todas las especificaciones, entonces OpenSpec 1.7.0 completa sin fallos introducidos por una versión más reciente. | Se conserva la cobertura completa; no se excluyen cambios ni especificaciones base. |

## Resultado del refinamiento

La etapa completó la arquitectura de pruebas. La validación cubrió contratos exclusivos, ausencia de ruta heredada, accesibilidad, actualización localizada, el runner DOC-37 y una CI reproducible. Con autorización explícita, las etapas E2E de preview, ejecución y bloqueo UI se completaron en GESTOR sin persistir secretos ni respuestas.

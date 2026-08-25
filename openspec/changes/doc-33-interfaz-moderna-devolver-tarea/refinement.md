<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-33-interfaz-moderna-devolver-tarea

## Fuente y alcance

- Ticket: `DOC-33` — INTERFAZ-MODERNA-DEVOLVER-TAREA.
- Cambio OpenSpec: `doc-33-interfaz-moderna-devolver-tarea`.
- Fuente Jira: `specs/interfaz-moderna-devolver-tarea/jira-context.md` y el prompt `02-ui-moderna-oficial.md`.
- Perfil tecnológico: `legacy-webforms-vb` con JavaScript sin bundler y pruebas CJS con `node:test`.

DOC-32 ya entrega `PreviewDevolverActividad` y `EjecutarDevolverActividad`. DOC-33 solo reemplaza la presentación de **Devolver → Elegir actividad anterior**: no modifica autorización, Ruta/Flujo, lock, auditoría ni el motor legacy de servidor.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx` publica el enlace `D-TASK-ANT`, el botón oculto `Button_tool_devolver_a_actividades_anterior` y los controles Web Forms que sostienen su postback.
- `workflow/Webworkflow.aspx.vb` enlaza ese botón a `ClassWorkflow.Activa_devolver_actividades_anteriores`, recorrido que DOC-33 debe retirar solo para esta acción.
- `webservice/WebServiceWorkflowModern.asmx.vb`, `ServicioDevolverActividad` y `DevolverActividadDtos.vb` ya exponen y protegen preview, paginación, token, conector, revalidación y ejecución.
- `workflow-user-send-ui.js`, `workflow-user-send-confirmation.js`, `ConfirmationDialog.js`, `workflow-transition-page-presentation.js` y `workflow-transition-modern.css` demuestran el patrón moderno reutilizable de modal, confirmación y actualización puntual.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Registrar la presentación de devolución por contexto Workflow válido, sin consultar `WorkflowCentroTrabajoModernActive` ni alterar el gate de otras acciones. | `ConfigureWorkflowTransitionModernPresentation` y el bootstrap de Envío a usuario en `workflow/Webworkflow.aspx.vb`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Sustituir `D-TASK-ANT` por un botón y módulos JavaScript exclusivos; sus selectores, eventos y estado no se comparten con envío, grupos ni Usuario anterior. | Enlace legacy de `workflow/Webworkflow.aspx:648` y módulos `workflow-user-send-*.js` como referencia aislada. | D-02 | RQ-01, RQ-05 | Origen: D-02, RQ-01; D-02, RQ-05 |
| D-03 | Consumir el preview existente con solo tarea, término, cursor y tamaño; aplicar mínimo dos caracteres, debounce, páginas, cancelación y descarte de respuesta obsoleta. | Contrato DOC-32 y `workflow-user-send-ui.js`. | D-03 | RQ-02 | Origen: D-03, RQ-02 |
| D-04 | Ejecutar únicamente tarea, `IdConector` y token provenientes del preview vigente mediante confirmación accesible y estado de envío exclusivo. | `EjecutarDevolverActividad` y `ConfirmationDialog.js`. | D-04 | RQ-03 | Origen: D-04, RQ-03 |
| D-05 | Tras éxito, actualizar solo la tarea afectada con `WorkflowTransitionPagePresentation`; bloqueo, error, timeout o cancelación no inician otra transición ni alteran otras acciones. | `workflow-transition-page-presentation.js` y resultados públicos DOC-32. | D-05 | RQ-04 | Origen: D-05, RQ-04 |
| D-06 | Retirar el enlace, botón, handler y listener de postback legacy de actividad anterior; preservar de forma comprobable Usuario anterior, Continuar flujo, Enviar a usuario y Enviar a grupo. | `inicializa_tipo_adjunto_documento`, `Button_tool_devolver_a_actividades_anterior_Click` y el callback de `Webworkflow.aspx`. | D-06 | RQ-05 | Origen: D-06, RQ-05 |
| D-07 | Cubrir bootstrap, aislamiento, búsqueda, paginación, respuesta obsoleta, selección, accesibilidad, ejecución y presentación con CJS; compilar y documentar sin ejecutar E2E autenticada. | `tests/workflow-user-send-*.test.cjs`, `tests/workflow-return-activity.test.cjs` y paquete técnico DOC-32. | D-07 | RQ-06 | Origen: D-07, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La acción moderna exclusiva abre sin postback ni gate para una tarea Workflow seleccionada. | WHEN se activa el trigger THEN no invoca `inicializa_tipo_adjunto_documento`, botones ocultos ni selectores de otras transiciones. | Usuario anterior conserva su trigger y ruta. |
| RQ-02 | El modal muestra únicamente destinos y contexto autorizados del preview vigente. | WHEN cambia búsqueda, cursor o tarea THEN invalida selección anterior, descarta respuestas obsoletas y no envía transición. | `IdConector` permanece opaco y no se transforma entre Ruta y Flujo. |
| RQ-03 | La confirmación ejecuta el payload mínimo vigente y bloquea doble confirmación o cierre durante el envío. | WHEN el servidor bloquea, vence token o falla la red THEN restaura un estado seguro para cancelar o reintentar. | El servidor continúa siendo la única autoridad. |
| RQ-04 | Éxito retira/actualiza la tarea puntual, contador, visor y scroll; los estados no exitosos conservan la bandeja. | WHEN el resultado es éxito, bloqueo, error, timeout o cancelación THEN el foco y mensajes quedan accesibles y correlacionados. | No modifica estado de otras operaciones. |
| RQ-05 | No queda ruta Web Forms alcanzable desde la acción moderna. | WHEN se inspecciona el markup, code-behind y scripts THEN no hay `D-TASK-ANT`, botón, handler ni listener legacy de actividad anterior. | Se prueba que las demás rutas conservan sus contratos. |
| RQ-06 | Existe evidencia reproducible de UI, compilación y documentación; no se ejecuta E2E real sin autorización. | WHEN corren pruebas focales y MSBuild THEN cubren los contratos nuevos y registran la limitación de QA. | No se cambia ambiente ni se persisten secretos. |

## Resultado del refinamiento

Estado: `approved`. Las decisiones, requisitos y tareas se basan en los contratos backend ya aprobados y limitan DOC-33 a la interfaz moderna exclusiva.

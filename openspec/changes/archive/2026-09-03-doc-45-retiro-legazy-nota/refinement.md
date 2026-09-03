<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-45-retiro-legazy-nota

## Fuente y alcance

- Ticket: `DOC-45` — RETIRO-LEGAZY-NOTA
- Cambio OpenSpec: `doc-45-retiro-legazy-nota`
- Fuente Jira: `specs/retiro-legazy-nota/jira-context.md`
- Perfil tecnológico: ASP.NET Web Forms/VB.NET, JavaScript legacy, ASMX y Playwright/Node bajo `tools/e2e`.
- Alcance: retirar el consumidor visual legacy de Notas dentro del Centro de Trabajo Workflow después de demostrar equivalencia moderna, además de eliminar piezas sin referencias verificadas.
- Fuera de alcance: tablas o datos, módulos de radicación/gestión de correspondencia, semántica de borrado y rutas legacy que todavía tengan consumidores.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb` y `js/workflow/Webworkflow.js`: contienen el fallback legacy todavía referenciado y el consumidor moderno estabilizado por DOC-44.
- `webservice/WebServiceWorkflow.asmx.vb`: mantiene los endpoints legacy `Service_*_nota_tarea_workflow`, consumidos por scripts de Workflow y por otros módulos; no son candidatos de retiro global en DOC-45.
- `webservice/WebServiceWorkflowNotesModern.asmx.vb`: contrato moderno con tarea explícita, autorización, versión, idempotencia y auditoría incorporado por DOC-42.
- `workflow/Class_anotacion_tarea.vb`: contiene dos rutinas de eliminación. `Eliminar_nota_service_workflow` tiene consumidor en `WebServiceWorkflow.asmx.vb`; la búsqueda estática solo encuentra la definición de `Eliminar_nota_tarea_workflow`, por lo que esta última es candidata a retiro sujeto a validación final.
- `workflow/WebFormAnotacion.aspx(.vb)` permanece incluido en el proyecto y usa `Class_anotacion_tarea`; no se elimina sin demostrar ausencia de navegación, enlaces directos y uso operativo.
- `tools/e2e/tests/notes-workflow.spec.cjs`, `doc43-notes-ui.spec.cjs`, `doc44-workflow-notes.spec.cjs` y sus pruebas de política ya cubren contrato moderno, CRUD, conflicto, tarea explícita y exclusión de doble presentación.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Retirar únicamente `Class_anotacion_tarea.Eliminar_nota_tarea_workflow` si una búsqueda final confirma cero consumidores; conservar `Eliminar_nota_service_workflow`. | `workflow/Class_anotacion_tarea.vb:157`, `workflow/Class_anotacion_tarea.vb:366`, `webservice/WebServiceWorkflow.asmx.vb:899` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | No retirar en este cambio los endpoints `Service_*_nota_tarea_workflow`, `WebFormAnotacion` ni activos compartidos con radicación o gestión de correspondencia mientras existan referencias vivas. | `js/workflow/Webworkflow.js:3383-3552`, `js/radicacion/WebFormRadicacionEntrante.js:533-702`, `js/gestion_correspondencia/WebForm_interface_gestion_tramite.js:1763-1932`, `GestionDocumental-Docuarchi.net.vbproj:8640` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Reutilizar exclusivamente el arnés y la autenticación de `tools/e2e`; la evidencia real exige autorización explícita y nunca se sustituye por mocks. | `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/tests/support/authenticated-workflow-session.cjs`, `tools/e2e/package.json` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener rollback reversible por Git, sin conservar una doble presentación activa, y finalizar toda corrida con `WorkflowCentroTrabajoModernActive=false` y audiencias vacías. | `tools/e2e/scripts/run-doc44-workflow-notes-interactive.cjs`, `tools/e2e/AGENT-RUNBOOK.md` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Retirar de `Webworkflow` el botón, controles, handlers y llamadas JavaScript legacy de Notas; conservar los contratos y páginas compartidos mientras Radicación o Correspondencia los consuman. La UI moderna será la única presentación para un contexto Workflow válido. | `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/Classselecciotarea.vb`, `js/workflow/Webworkflow.js`, `workflow/WorkflowModernPresentationBootstrap.vb` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Integrar la validación E2E real como parte inseparable de DOC-45, reutilizando el ejecutor de Notas existente, sin login, arnés, configuración o `.env` paralelos y sin modificar innecesariamente el gate. | `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/tests/support/authenticated-workflow-session.cjs`, `tools/e2e/tests/doc44-workflow-notes.spec.cjs`, `tools/e2e/scripts/run-doc44-workflow-notes-interactive.cjs` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Permitir lectura completa de notas ajenas, pero reservar edición y eliminación al propietario mediante una capacidad calculada en servidor y defensa atómica en persistencia. | Modelos/DTO, servicio/repositorio moderno, `Webworkflow.aspx`, cliente y E2E oficial de Notas | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Convertir el estado `0 notas` en una acción explícita `Nueva nota` que abra directamente el editor, conservando contador y accesibilidad. | `workflow/Webworkflow.aspx`, `js/workflow/Webworkflow.js`, políticas y E2E oficial | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Preservar la señal cromática y el glifo de las acciones dinámicas de la tabla de tareas y de los controles mostrar/ocultar índice bajo la presentación moderna. | estilos de tareas y Centro de Trabajo, `workflow/Webworkflow.aspx`, política DOC-45 | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Mantener operativo el acceso moderno de Notas después de que un postback parcial reemplace la barra de tareas, mediante delegación de eventos y sincronización con `endRequest`. | `js/workflow/Webworkflow.js`, E2E oficial y políticas | D-10 | RQ-10 | Origen: D-10, RQ-10 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La rutina duplicada sin consumidores deja de compilarse sin alterar la ruta de borrado legacy activa ni el contrato moderno. | WHEN se repite el inventario estático y se ejecutan pruebas focales THEN no existe referencia a `Eliminar_nota_tarea_workflow` y continúan presentes `Eliminar_nota_service_workflow` y `EliminarNota`. | Un consumidor dinámico no detectable obliga a conservar la rutina; rollback: restaurar la función atómica. |
| RQ-02 | Ningún archivo, endpoint o control con referencias vivas se elimina. | WHEN se revisa el diff THEN los endpoints `Service_*`, `WebFormAnotacion` y los consumidores externos permanecen sin cambios salvo documentación/pruebas justificadas. | Evita regresiones fuera de Workflow y retiros globales no autorizados. |
| RQ-03 | La regresión demuestra autorización, tarea explícita, lectura no mutante, CRUD, idempotencia, versión/conflicto, auditoría y aislamiento usando el arnés común. | WHEN exista autorización literal THEN las suites aplicables se ejecutan en `tools/e2e` y generan solo evidencia saneada; sin autorización quedan bloqueadas explícitamente. | No se guardan secretos, contenidos de notas, cookies ni cadenas de conexión. |
| RQ-04 | El Centro de Trabajo conserva una sola operación por acción y un rollback operativo seguro. | WHEN finaliza cualquier corrida, incluso por error, THEN el gate queda `false`, usuarios/grupos vacíos y el retiro puede revertirse atómicamente desde Git. | Una restauración fallida bloquea el cierre y requiere corrección antes de continuar. |
| RQ-05 | La experiencia moderna es la única superficie de Notas dentro de `Webworkflow` para un contexto válido. | WHEN se selecciona una tarea operable THEN aparece el panel moderno y no existen `Panel_Buttonanotacion`, `ImageButtonanotacion*`, handlers ni invocaciones legacy de Notas en el consumidor Workflow. | Los endpoints y páginas compartidos permanecen para no romper Radicación ni Correspondencia. |
| RQ-06 | Código, pruebas focales, build, E2E real autorizada y evidencia saneada forman una única unidad de entrega. | WHEN se valida DOC-45 THEN la E2E oficial reutilizada demuestra panel moderno visible, controles legacy inexistentes, CRUD real, lecturas negativas, una mutación por acción y configuración segura al cierre. | Sin autorización o datos descartables, DOC-45 permanece abierto; no se aceptan mocks ni evidencia anterior a la implementación corregida. |
| RQ-07 | Cualquier usuario autorizado puede leer completa una nota de la tarea, pero solo su autor puede editarla o eliminarla. | WHEN se lista una nota ajena extensa THEN aparece lectura ampliada y no aparecen acciones mutantes; llamadas directas de actualización/eliminación responden `NotOwner` sin cambios de nota, versión o auditoría. | La UI no deduce propiedad ni sustituye la validación del backend; la E2E exige una nota ajena de la misma tarea y autorización real. |
| RQ-08 | Una tarea sin notas ofrece creación visible e inmediata. | WHEN el contador autorizado es cero THEN el acceso muestra `Nueva nota 0`, su nombre accesible indica creación y el primer clic enfoca el editor. | La E2E requiere una tarea descartable realmente vacía y restaura el estado vacío al finalizar. |
| RQ-09 | Los controles no afectados por el retiro de Notas conservan su identidad visual. | WHEN se muestra la lista de tareas o el índice THEN Ver documentos, Detalle, Retomar y mostrar/ocultar índice conservan fondo y glifo con contraste. | Las reglas se limitan a identificadores existentes y no cambian eventos, visibilidad ni autorización. |
| RQ-10 | El acceso moderno sobrevive a la navegación asíncrona de Web Forms. | WHEN la selección de tarea reemplaza la barra mediante `UpdatePanel` THEN el nuevo botón de Notas abre el modal y usa el identificador actual sin recarga completa. | La escucha delegada no escribe la tarea ni duplica mutaciones; `endRequest` solo refresca lectura autorizada. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` se desarrolla en `design.md`, se refleja en `spec.md` y se vincula desde `tasks.md` con `Origen: D-XX, RQ-XX`.
2. Las tareas de inventario, retiro, pruebas, documentación y rollback conservan su origen explícito.
3. No se ejecutan E2E autenticadas ni se activa el gate sin las autorizaciones exigidas por `AGENTS.md` y el runbook.
4. Una referencia viva o una brecha de evidencia convierte el retiro correspondiente en bloqueo, no en una eliminación asumida.
5. La E2E anterior al retiro ampliado no constituye evidencia final; debe repetirse sobre el código corregido con autorización expresa.

## Resultado del refinamiento

- Estado: aprobado para sincronizar decisiones en design, spec y tasks.
- Criterio de implementación: retiro atómico de la rutina duplicada y del consumidor legacy exclusivo del Centro de Trabajo; se conservan únicamente las superficies compartidas con consumidores externos comprobados.
- Siguiente comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-45 --sync`.

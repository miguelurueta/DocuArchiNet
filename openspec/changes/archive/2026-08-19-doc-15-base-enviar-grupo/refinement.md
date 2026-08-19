<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - DOC-15: base Enviar a grupo

## Fuente y alcance

- Ticket: `DOC-15` — BASE-ENVIAR-GRUPO.
- Plataforma: ASP.NET Web Forms .NET Framework, VB.NET, MySQL y ASMX.
- Superficie: exclusivamente el comando **Enviar a grupo** de `workflow/Webworkflow.aspx`.
- Fuera de alcance: `Enviar a usuario`, reasignación de respuesta, los contratos `PreviewEnviarTarea` y `EjecutarEnvioTarea`, `ServicioTransicionTarea`, conectores ficticios, una segunda configuración de gate y activar cualquier ambiente.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx.vb` maneja el camino legacy con `ImageButtonEnviaActividad_Click` y `Button_tool_enviar_actividad_Click`; valida `Cambio_Ruta`, ruta y flujo antes de listar actividades y bloquea aprobaciones pendientes antes de terminar la tarea.
- `workflow/Class_Listado_Actividades_workflow.vb` resuelve actividades de `LISTADO_ACTIVIDADES_WORKFLOW` por ruta para el modal legacy. No debe exponerse ni reutilizar controles `GridView`, `UpdatePanel` o `Session` desde ASMX.
- `webservice/WebServiceWorkflowModern.asmx.vb`, `WorkflowPreviewSessionContextGate`, `ServicioTransicionTarea`, `MySqlTransicionConcurrencyGuard` y `WorkflowLegacyExecutorAdapter` aportan el límite moderno existente, pero su contrato de transición exige `IdConector > 0` y no representa envío directo a actividad.
- `ConfiguracionWorkflowModernFeatureGate.vb` y `WorkflowModernPresentationBootstrap.vb` son la única evaluación de habilitación. El modo piloto y el modo oficial ya están definidos por la configuración existente; esta entrega no crea ni modifica claves de configuración.
- `ClassWorkflow.Terminar_Tarea_Workflow` conserva el motor de estado, eventos y correo. La variante de grupo debe llegar a él mediante un adaptador de envío directo, sin `Page`, sin handler Web Forms, sin conector y con los identificadores de flujo que usa el camino legacy.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Enviar a grupo es una operación hermana de la transición por conector: recibe `IdTarea`, `IdActividadDestino` y `TokenVersion`, y nunca fabrica ni acepta `IdConector`. | `workflow/Webworkflow.aspx.vb`; `Modelo/Workflow/Terminar/WorkflowModernModels.vb`; `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | D-01 | RQ-01 | 2.1, 2.2, 4.1; Origen: D-01, RQ-01 |
| D-02 | Preview y ejecución usan el mismo contexto autenticado y `IWorkflowModernFeatureGate`; una llamada directa fuera del alcance queda bloqueada y la UI conserva el postback legacy. | `webservice/WebServiceWorkflowModern.asmx.vb`; `workflow/WorkflowModernPresentationBootstrap.vb`; `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | D-02 | RQ-01, RQ-05 | 3.3, 4.6, 5.1; Origen: D-02, RQ-01 |
| D-03 | El preview de grupo consulta solo actividades autorizadas de la ruta, no escribe estado, auditoría ni eventos, y no usa el DTO de preview como autorización de ejecución. | `workflow/Class_Listado_Actividades_workflow.vb`; `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` | D-03 | RQ-02 | 3.1, 3.2, 6.1; Origen: D-03, RQ-02 |
| D-04 | Dentro de `GET_LOCK` se releen tarea y token y se revalidan `Cambio_Ruta`, tarea activa, ruta, flujo/actividad de flujo y pertenencia del destino a la ruta. | `Services/Workflow/Terminar/ServicioTransicionTarea.vb`; `Infrastructure/Workflow/Terminar/MySqlTransicionConcurrencyGuard.vb`; `workflow/Webworkflow.aspx.vb` | D-04 | RQ-03 | 4.2, 4.5, 6.2; Origen: D-04, RQ-03 |
| D-05 | La regla específica de grupo bloquea solicitudes de aprobación sin decisión y conserva la ausencia de validación de respuesta radicada; cualquier endurecimiento de respuesta requiere decisión funcional posterior. | `workflow/Webworkflow.aspx.vb`; `Doc/Actualizacion/workflow/TerminarGrupo/00-exploracion-arquitectura-envio-grupo.md` | D-05 | RQ-04 | 4.3, 6.2; Origen: D-05, RQ-04 |
| D-06 | Solo un adaptador de envío directo puede invocar `ClassWorkflow.Terminar_Tarea_Workflow`; usa destino autorizado, conector cero, flujo cero y actualización de interfaz desactivada. | `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb`; `workflow/Webworkflow.aspx.vb` | D-06 | RQ-03, RQ-04 | 4.4, 6.2; Origen: D-06, RQ-03 |
| D-07 | La interfaz reutiliza bootstrap, confirmación, accesibilidad y actualización visual existentes, sin exponer SQL, motor legacy o controles Web Forms a JavaScript y sin cambiar Continuar flujo. | `workflow/Webworkflow.aspx`; `workflow/Webworkflow.aspx.vb`; `tests/workflow-transition-*.test.cjs` | D-07 | RQ-05 | 5.1, 5.2, 5.3, 6.3; Origen: D-07, RQ-05 |
| D-08 | Los resultados y la auditoría adicional son sanitizados y distinguibles por `ASMX_ENVIO_GRUPO`; una advertencia posterior no revierte una transición confirmada. | `Services/Workflow/Terminar/ServicioTransicionTarea.vb`; `Modelo/Workflow/Terminar/WorkflowModernModels.vb` | D-08 | RQ-06 | 4.5, 6.2; Origen: D-08, RQ-06 |
| D-09 | La entrega se verifica con pruebas focales, compilación, QA manual, documentación y rollback fail-closed. No ejecuta E2E autenticado, carga ni activación de gate sin autorización explícita. | `AGENTS.md`; `Doc/Actualizacion/workflow/TerminarGrupo/prompts/08-pruebas-verificacion.md`; `Doc/Actualizacion/workflow/TerminarGrupo/prompts/09-liberacion-activacion-controlada.md` | D-09 | RQ-07 | 6.4, 6.5, 7.1, 7.2, 7.3; Origen: D-09, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario de aceptación | Riesgo y compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El ASMX existente expone preview y ejecución de grupo con contrato directo, contexto autenticado y gate fail-closed. | Con gate inactivo, sesión inválida o permiso no autorizado, devuelve bloqueo funcional y no alcanza el motor. | No cambia `PreviewEnviarTarea`, `EjecutarEnvioTarea` ni `IdConector`. |
| RQ-02 | El preview devuelve únicamente destinos de actividad válidos de la ruta y un token de versión. | Al consultar, no se escriben tarea, estado, auditoría ni eventos; sin permiso, ruta cerrada o sin destinos devuelve un error público seguro. | El listado legacy por `GridView` permanece disponible con gate inactivo. |
| RQ-03 | La ejecución revalida estado y destino dentro de un lock y termina como máximo una vez. | Token vencido, destino retirado, tarea inactiva, ruta/flujo/actividad cerrados o dos solicitudes concurrentes no producen una segunda transición. | No se crea una transacción de estado alternativa. |
| RQ-04 | Los requisitos propios de grupo conservan solicitudes de aprobación sin decisión y el motor legacy. | Una aprobación sin decisión bloquea; éxito usa `Terminar_Tarea_Workflow` sin conector ni reasignación de respuesta. | No se introduce la validación de respuesta radicada sin aprobación funcional. |
| RQ-05 | La experiencia moderna es progresiva y reversible. | Con gate inactivo el botón y modal legacy conservan el postback; Continuar flujo mantiene endpoints, payload y destinos por conector. | No se crean claves ni precedencias nuevas de configuración. |
| RQ-06 | El contrato público y la auditoría no filtran detalles internos. | Éxito, bloqueo o advertencia contienen código, texto funcional y referencia sanitizada; SQL, Session, tokens, credenciales y excepciones no aparecen. | Una advertencia de correo/evento posterior no deshace éxito confirmado. |
| RQ-07 | El cambio queda verificable y con rollback documentado. | Build, pruebas focales y QA manual registran evidencia; cualquier prueba mutante solo ocurre con ambiente, cuentas y tareas descartables autorizadas. | Desactivar el gate existente devuelve nuevos intentos al camino legacy sin migración ni reversión de transiciones ya confirmadas. |

## Resultado del refinamiento

Estado: aprobado para implementación. La decisión de habilitación se limita al gate existente y su configuración ya definida; esta etapa no la modifica. Cada tarea abierta conserva su origen y su verificación principal.

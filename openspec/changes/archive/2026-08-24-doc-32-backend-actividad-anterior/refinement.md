<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-32-backend-actividad-anterior

## Fuente y alcance

- Ticket: `DOC-32` — BACKEND-ACTIVIDAD-ANTERIOR
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnologico: `legacy-webforms-vb`; el corte se limita a VB.NET, ASMX y MySQL existentes.

La persona responsable confirma el alcance backend de DOC-32: no se crea UI, gate ni configuracion; Ruta y Flujo se resuelven desde la tarea y la mutacion queda encapsulada en un adaptador exclusivo.

## Contexto inspeccionado

- `webservice/WebServiceWorkflowModern.asmx.vb` concentra los endpoints ASMX modernos y compone dependencias sin invocar el motor de Workflow.
- `workflow/ClassWorkflow.vb` contiene `Terminar_Tarea_Workflow`; `Activa_devolver_actividades_anteriores` y `Enviar_actividad_por_conector_flujo_de_trabajo_anterior` dependen de controles Web Forms y quedan fuera del recorrido nuevo.
- `Infrastructure/Workflow/Terminar/MySqlTransicionConcurrencyGuard.vb` nombra locks con tarea y token; DOC-32 requiere un guard separado cuyo nombre depende solo de `IdTarea`.
- `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` ilustra las consultas parametrizadas existentes, pero sus conectores son salientes y no se reutilizan para devolución.
- `WorkflowLegacyExecutorAdapter` demuestra la llamada segura con `Page = Nothing` y sin actualización de interfaz; DOC-32 usa otro adaptador para preservar la semántica entrante.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Crear contratos, DTOs, servicio, repositorios y adaptador exclusivos de devolución; no reutilizar contratos de envío ni Usuario anterior. | `Modelo/Workflow/Devolver`, `DTOs/Workflow/Devolver`, `Services/Workflow/Devolver`, `Infrastructure/Workflow/Devolver` nuevos; `ServicioTransicionTarea` no se modifica. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Resolver permiso, tipo Ruta/Flujo e identidad de conector exclusivamente desde la tarea y el contexto autenticado; fallar cerrado. | `WorkflowPreviewSessionContextGate.vb`, repositorios específicos de devolución y estado de tarea. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El preview ejecuta solo SELECT parametrizados, aplica filtro y paginación sobre el universo autorizado y usa cursor opaco ligado a tarea, contexto, tipo, término y orden. | `AdoNetDataExecutor`, repositorio y codec de cursor de devolución nuevos. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La ejecución adquiere un lock exclusivo por tarea, independiente de token, y dentro del lock relee permiso, tarea, token, contexto y conector entrante. | Guard exclusivo de devolución, servicio de ejecución y repositorio de resolución final. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Solo el adaptador exclusivo invoca una vez `Terminar_Tarea_Workflow` con `Page = Nothing`, interfaz legacy desactivada, eventos dinámicos activos y notificación de asignación configurada por el destino. | `ClassWorkflow.Terminar_Tarea_Workflow`; nuevo `WorkflowLegacyDevolverActividadExecutorAdapter`. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | El adaptador y los componentes nuevos no construyen ni invocan componentes de respuestas; la auditoría sanitizada usa el mecanismo `ASMX_DEVOLVER_ACTIVIDAD` y una falla de auditoría solo agrega advertencia. | `WorkflowLegacyAuditoriaAdapter.vb`, servicio y pruebas focales. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Preservar los endpoints, contratos, guard y recorridos de Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior; documentar y probar el aislamiento. | Endpoints existentes, `MySqlTransicionConcurrencyGuard.vb`, pruebas CJS y documentación técnica DOC-32. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Incorporar E2E DOC-32 protegida que reutiliza el helper de sesión existente, controles MySQL de lectura, ejecución real autorizada, carrera de dos solicitudes y métricas de latencia con presupuesto por ambiente. | `tools/e2e/tests/support/authenticated-workflow-session.cjs`, nueva suite DOC-32 y scripts de configuración y concurrencia. | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Existen contratos y endpoints exclusivos `PreviewDevolverActividad` y `EjecutarDevolverActividad`. | WHEN se inspeccionan los contratos THEN no exponen `Page`, `Session`, HTML, SQL ni tipos de envío existentes. | Las capacidades actuales conservan sus archivos y firmas. |
| RQ-02 | Solo un usuario con permiso específico y tarea accesible obtiene o ejecuta una devolución. | WHEN un cliente publica actividad, usuario, grupo, Ruta, Flujo o destino THEN el servidor los ignora y resuelve su propio contexto. | Una inconsistencia devuelve código público estable sin fuga. |
| RQ-03 | El preview devuelve únicamente aristas entrantes autorizadas y una página estable. | WHEN el cursor, término o contexto no coincide THEN se bloquea sin escribir tarea, auditoría, eventos ni datos de negocio. | Ruta y Flujo conservan identidades de conector aisladas. |
| RQ-04 | Dos solicitudes, aun con tokens diferentes, no devuelven dos veces la misma tarea. | WHEN el lock está ocupado, token vencido o conector es inválido THEN se bloquea antes del motor. | El guard genérico tokenizado no cambia. |
| RQ-05 | Una devolución válida conserva eventos dinámicos y notificación de asignación, sin actualizar controles Web Forms. | WHEN el adaptador ejecuta THEN llama una vez al motor con `Page = Nothing`, interfaz `0`, eventos `1`, notificación de asignación según destino y reasignaciones `0`. | El perfil se prueba contra Ruta y Flujo. |
| RQ-06 | Éxito, bloqueo, error reintentable y advertencias se normalizan y auditan sin datos sensibles. | WHEN la auditoría falla después del motor THEN la transición confirmada se conserva y la respuesta incluye advertencia. | No se permite tratamiento de respuestas en componentes nuevos. |
| RQ-07 | La entrega tiene pruebas y documentación reproducibles sin activar UI, feature flag ni ambiente. | WHEN se ejecutan las verificaciones locales THEN se demuestra aislamiento, SQL de lectura y compatibilidad. | El gate `WorkflowCentroTrabajoModernActive` no cambia. |
| RQ-08 | La entrega ejecuta E2E real autorizada con evidencia saneada de preview, transición, auditoría, carrera y latencias. | WHEN se suministran ambiente, cuenta Workflow, dos tareas descartables, SELECT de control y presupuestos de rendimiento THEN una transición real y una carrera de dos solicitudes cumplen sus resultados esperados. | Secretos, cookies, destinos, tokens y cadenas de conexión no se guardan ni imprimen. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: aprobado para planificación e implementación del backend.
- Comando de sincronización: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-32 --sync`.

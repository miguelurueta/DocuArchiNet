<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-41-lectura-listado-contador

## Fuente y alcance

- Ticket: `DOC-41` — LECTURA-LISTADO-CONTADOR.
- Cambio OpenSpec: `doc-41-lectura-listado-contador`.
- Fuente funcional: `specs/lectura-listado-contador/jira-context.md`.
- Tecnología afectada: ASP.NET Web Forms con VB.NET y MySQL.

El cambio habilita únicamente el camino moderno de lectura de notas: listado, consulta de contenido y contador para una tarea autorizada. No incorpora creación, edición, eliminación, interfaz, migración de consumidores ni cambios al gate de Centro de Trabajo.

## Contexto inspeccionado

- `Modelo/Workflow/Notas/NotasWorkflowModels.vb` y `NotasWorkflowInterfaces.vb` ya definen contratos internos con `IdTarea` explícito y puertos sin `Session` ni controles WebForms.
- `Services/Workflow/Notas/ServicioNotasWorkflow.vb` valida permiso, tarea activa y ruta mediante `ITareaWorkflowRepository`, pero las lecturas todavía delegan a un repositorio fail-closed.
- `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` contiene el punto de extensión de persistencia y ya recibe `IModuleConnectionFactory` e `IDataExecutor`.
- `workflow/Class_anotacion_tarea.vb` y `webservice/WebServiceWorkflow.asmx.vb` conservan las consultas legacy; combinan SQL concatenado, `SELECT *` para el contador y el uso de `ID_TAREA_SELECCIONDA`. Se preservan sin modificación.
- `webservice/WorkflowPreviewSessionContextGate.vb` resuelve `PuedeInteractuarAnotaciones` desde la sesión autenticada; `WebServiceWorkflowNotesModern.asmx` es el límite ASMX especializado que reutiliza el arnés E2E de Notas.
- `tests/workflow-notes-contracts.test.cjs` y `tools/e2e/tests/notes-workflow-policy.test.cjs` son las bases de verificación local y de política E2E reutilizable.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Exponer listado, contenido y contador solo en `WebServiceWorkflowNotesModern.asmx`; cada operación recibe `idTarea` en su DTO y se limita a respuestas de lectura. | `WebServiceWorkflowNotesModern.asmx.vb`; `DTOs/Workflow/Notas/NotasWorkflowDtos.vb`. | D-01 | RQ-01, RQ-03, RQ-04, RQ-05 | Origen: D-01, RQ-01 |
| D-02 | Resolver permiso y tarea desde el contexto autenticado con `AsegurarContextoNotas` y `MySqlTareaWorkflowRepository.ObtenerTarea`; no usar la tarea seleccionada en sesión ni datos de identidad del navegador. | `WorkflowPreviewSessionContextGate.AsegurarContextoNotas`; `MySqlWorkflowPreviewRepositories.vb`. | D-02 | RQ-01, RQ-03 | Origen: D-02, RQ-01 |
| D-03 | Usar el único orden expuesto en esta fase: `FECHA_ANOTACION DESC, ID_ANOTACION DESC`; paginar con tamaño 25 por defecto y máximo 50, y proteger el cursor con `MachineKey` ligado a tarea, actor, grupo, ruta, versión y límite de la página. | `DevolverActividadCursorCodec.vb`; patrones 25/50 en `ServicioDevolverActividad.vb`. | D-03 | RQ-02 | Origen: D-03, RQ-02 |
| D-04 | Implementar las tres lecturas en `MySqlNotasWorkflowRepository` con parámetros `MySqlParameter`; listado y contador aplican la misma visibilidad operativa, el contador usa `COUNT(*)` y el contenido filtra a la vez por `ID_ANOTACION` e `INICIO_TAREAS_WORKFLOW_ID_TAREA`. | `Class_anotacion_tarea.vb`; `ModuleDataContracts.vb`; `MySqlNotasWorkflowRepository.vb`. | D-04 | RQ-02, RQ-03, RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener el histórico moderno deshabilitado: las consultas operativas filtran `ANOTACION_TAREA.ESTADO_TAREA = 1`; tarea inaccesible, nota ajena y cursor inválido devuelven una respuesta funcional sin filas, conteo ni cursor de otro contexto. | Filtros operativos de `Class_anotacion_tarea.vb`; códigos de `NotasWorkflowModels.vb`. | D-05 | RQ-03, RQ-04 | Origen: D-05, RQ-03 |
| D-06 | No modificar endpoints legacy, controles WebForms ni `WorkflowCentroTrabajoModernActive`; probar el comportamiento con fakes y reutilizar exclusivamente `tools/e2e` para la validación autorizada, dejando evidencia saneada y el bloqueo explícito si no hay autorización. | `tests/WorkflowReturnUserPreviousBehaviorTests.cs`; `tools/e2e/AGENT-RUNBOOK.md`; `tools/e2e/tests/notes-workflow-policy.test.cjs`. | D-06 | RQ-05, RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Un actor autenticado con permiso de notas consulta únicamente una tarea activa asignada a su ruta mediante `idTarea` explícito. | WHEN el endpoint recibe una tarea autorizada THEN devuelve el resultado de lectura sin consultar `ID_TAREA_SELECCIONDA`. | Las páginas y ASMX legacy siguen sin cambios. |
| RQ-02 | El listado usa orden determinista y página acotada; un cursor no puede reutilizarse entre tareas, actores, grupos, rutas o versiones de tarea. | WHEN el cursor es válido para el mismo contexto THEN continúa desde el último registro; WHEN no lo es THEN no devuelve datos ni cursor. | Evita fugas entre contextos y lecturas sin límite. |
| RQ-03 | El contenido solo se entrega si la nota pertenece a la tarea autorizada; una nota de otra tarea no revela existencia ni contenido. | WHEN `idNota` no pertenece a `idTarea` THEN la respuesta funcional no contiene nota ni metadatos de la nota ajena. | Conserva el contrato de aislamiento incluso ante enumeración de identificadores. |
| RQ-04 | El contador usa `COUNT(*)` parametrizado con la misma política del listado y no expone histórico moderno. | WHEN se cuenta una tarea autorizada THEN el resultado coincide con el listado operativo y no materializa filas. | No se interpreta `ESTADO_TAREA` como autorización para histórico. |
| RQ-05 | Las tres operaciones modernas son de solo lectura y no modifican tarea, estado, auditoría ni gates. | WHEN se invoca listado, contenido o contador THEN no se ejecutan sentencias de escritura ni se altera el estado de la tarea. | No hay cambio de consumidor ni de feature flag. |
| RQ-06 | Las pruebas cubren autorización, orden, cursor, aislamiento y contador; la E2E reutiliza la infraestructura aprobada y no se ejecuta sin autorización de ambiente y datos. | WHEN no hay autorización E2E THEN se registra el bloqueo sin simular ni fabricar evidencia. | No se exponen secretos, cookies ni cadenas de conexión. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` se desarrolla en `design.md`, se refleja en escenarios de `spec.md` y se vincula a una tarea con su requisito `RQ-XX`.
2. Cada tarea con checkbox conserva un único origen principal y una verificación observable.
3. La implementación de lectura no introduce lógica de negocio en `.aspx` o code-behind, ni adapta `Class_anotacion_tarea` como repositorio moderno.
4. La E2E autenticada requiere leer previamente `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`, además de autorización explícita para ambiente, cuentas y tareas descartables.

## Resultado del refinamiento

- Estado: aprobado mediante la confirmación del responsable del cambio.
- Alcance de histórico: bloqueado para esta fase; una política de negocio explícita será precondición de una fase posterior.
- Siguiente paso: implementar las tareas en orden y mantener la evidencia de pruebas saneada.

<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-44-estabilizacion-workflow

## Fuente y alcance

- Ticket: `DOC-44` — ESTABILIZACION-WORKFLOW
- Cambio OpenSpec: `doc-44-estabilizacion-workflow`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnologico: ASP.NET Web Forms/VB.NET, JavaScript y ASMX existentes; no agregar dependencias ni ampliar cambios de producto fuera de `workflow/`, su configuración de entrega, pruebas E2E y documentación DOC-44. El ASMX compartido se valida y reutiliza sin alterar consumidores ajenos.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx(.vb)`: panel moderno, botón/modal/GridView legacy, tarea seleccionada y bootstrap condicionado.
- `js/workflow/Webworkflow.js`: adaptador único `WorkflowNotesModern`, serialización JSON, estados, cambio de tarea y CRUD.
- `webservice/WebServiceWorkflowNotesModern.asmx.vb`: listar, consultar, contar, crear, actualizar y eliminar con `idTarea` explícito.
- `tools/e2e/tests/doc43-notes-ui-*.cjs` y `tools/e2e/AGENT-RUNBOOK.md`: regresión reutilizable, autorización TTY y evidencia saneada.
- Compatibilidad preservada: con el gate apagado permanece disponible el botón/modal/GridView legacy; la ruta moderna no se inicializa ni ejecuta en paralelo.
- Deuda para fase 06: handlers `ImageButtonanotacion(_)_Click`, `GridView_lista_notas`, `Class_anotacion_tarea` y funciones JS legacy continúan presentes hasta demostrar referencias cero.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Estabilizar solo el consumidor de Notas de `Webworkflow`; se permiten su configuración, pruebas y documentación, mientras el ASMX compartido solo se valida/reutiliza y los demás consumidores y el retiro legacy quedan fuera. | `workflow/Webworkflow.aspx`: `Panel_notas_modernas`, `Panel_content_anotacion`, `GridView_lista_notas`; `Web.config`; `tools/e2e`; paquete técnico DOC-44. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Mantener `WorkflowNotesModern` como único cliente moderno, con JSON real, `idTarea` explícito y resultados funcionales del ASMX. | `js/workflow/Webworkflow.js`: `WorkflowNotesModern`; `webservice/WebServiceWorkflowNotesModern.asmx.vb`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Hacer mutuamente excluyentes los caminos moderno y legacy; el rollback es gate `false` con audiencias vacías, sin cambios de datos. | `workflow/Webworkflow.aspx.vb`: `ConfigureWorkflowNotesModernPresentation`; `Web.config`: claves `WorkflowCentroTrabajoModern*`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Integrar una regresión E2E exclusiva de Workflow y cubrir autorización, tarea/nota cruzada, conflicto, contenido, cursor, rollback y doble operación reutilizando `tools/e2e`; las corridas reales requieren autorización explícita y restauración del gate. | `tools/e2e/tests/doc43-notes-ui-policy.test.cjs`, `doc43-notes-ui.spec.cjs`, runners y `AGENT-RUNBOOK.md`. | D-04 | RQ-04 | Origen: D-04, RQ-04 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El cambio modifica únicamente el consumidor Centro de Trabajo Workflow y su configuración de entrega; valida/reutiliza el ASMX compartido y produce pruebas y documentación sin intervenir otros consumidores. | WHEN se inventarían referencias de Notas THEN solo se consideran `workflow/`, configuración, utilidades E2E y documentación DOC-44; los contratos compartidos se inspeccionan únicamente en la frontera utilizada. | Evita alterar otros consumidores; el retiro queda para fase 06. |
| RQ-02 | Listar, consultar, contar y mutar notas usan el contrato moderno único con tarea explícita y autorización backend. | WHEN la UI moderna opera una nota THEN cada solicitud incluye `idTarea` y no obtiene identidad desde sesión mutable. | Previene tarea/nota cruzada, autorización visual y CRUD duplicado. |
| RQ-03 | Gate y fallback nunca habilitan dos canales simultáneos y permiten rollback sin mutar datos. | WHEN el gate está `false` THEN funciona legacy y no se inicializa moderno; WHEN está activo para contexto autorizado THEN se oculta el disparador legacy. | El valor entregado permanece `false`, usuarios y grupos vacíos. |
| RQ-04 | Existe una regresión E2E exclusiva de Workflow y produce evidencia saneada y verificable de seguridad, conflicto, contenido, cursor y reversibilidad. | WHEN se integra la prueba THEN reutiliza infraestructura existente; WHEN existe autorización explícita THEN se ejecuta sobre tarea descartable; de lo contrario se registra bloqueo sin simulación. | Secretos efímeros, controles de datos solo `SELECT` y gate restaurado. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: aprobado. Alcance, decisiones, compatibilidad, evidencia y deuda legacy fueron contrastados con el código del consumidor Workflow.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- <ISSUE-KEY> --sync`.

<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-9-contrato-terminar-tarea-workflow

## Fuente y alcance

- Ticket: `DOC-9` — CONTRATO-TERMINAR-TAREA-WORKFLOW.
- Cambio OpenSpec: `doc-9-contrato-terminar-tarea-workflow`.
- Fuente funcional: `specs/contrato-terminar-tarea-workflow/jira-context.md`.
- Perfil técnico observado: ASP.NET Web Forms, VB.NET y MySQL; no hay consumidor frontend React ni contrato `AppResponses<T>` dentro del alcance.

DOC-9 corresponde al Prompt 01. Crea contratos paralelos de Workflow y componentes compartidos de raíz para la modernización gradual, y conserva la ejecución de tareas en el camino legacy. Los prompts 02 a 06 consumirán esta fundación en cambios separados.

## Contexto inspeccionado

- Entrada vigente: `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- Decision existente de flujo/ruta: `ClassWorkflow.Validar_enviar_actividad_por_conector_flujo_o_ruta`.
- Nucleo legacy: `ClassWorkflow.Terminar_Tarea_Workflow` y `ClassWorkflow.Cambia_Estado`.
- Base implementada: `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar` y `Services/Workflow/Terminar`, más `Domain/Shared`, `Infrastructure/Shared/Data`, `Infrastructure/Workflow/Terminar` y `Infrastructure/Repositories/Workflow/` como límite de repositorios futuros.
- Evidencia automatizada: `tools/validation/Verify-Doc9Foundation.ps1` y compilacion MSBuild Debug.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La fundacion no modifica ni sustituye el flujo WebForms vigente. | Contratos nuevos de raíz y ausencia de cambios funcionales en el camino legacy. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Los contratos son tipados y Domain/Application no dependen de WebForms. | `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar`, `Services/Workflow/Terminar` y verificacion focal. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El unico límite nuevo reservado para el motor legacy es inerte durante DOC-9. | `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La habilitación moderna es de servidor e inicia fail-closed. | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb`. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | `Infrastructure/Shared/Data` es reutilizable y no conoce Workflow; los puertos de repositorio siguen siendo propios de Workflow. | `Domain/Shared/ContextoModulo.vb`, `Infrastructure/Shared/Data/*` y `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | La evidencia automatizada no sustituye la QA manual del flujo vigente. | `tools/validation/Verify-Doc9Foundation.ps1` y tarea 3.3. | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El WebForms actual conserva la terminacion de tareas. | La compilacion incorpora la fundacion sin cambiar el camino de ejecucion actual. | Preserva autorizacion, firma, expediente y eventos dinamicos legacy. |
| RQ-02 | Las nuevas capas no exponen Page, Session, HTML, SQL o excepciones internas. | La verificacion focal rechaza dependencias WebForms en Domain/Application. | Evita que Presentation futura absorba reglas de negocio. |
| RQ-03 | La composicion accidental no termina una tarea. | El adapter devuelve `WORKFLOW_MODERN_EXECUTION_PENDING`. | Evita una migracion parcial sin Page, Session, piloto ni rollback. |
| RQ-04 | Sin habilitacion explicita no se activa la experiencia moderna. | El gate devuelve `inactivo` y codigo funcional no sensible. | Reduce activacion accidental y conserva rollout gradual. |
| RQ-05 | La infraestructura compartida no acopla modulos ni mezcla dominios. | Los puertos Workflow reciben contexto validado y usan contratos tipados. | Permite reutilizar datos comunes sin `GenericRepository`. |
| RQ-06 | La evidencia distingue lo ejecutado de lo que requiere ambiente web. | Build y verificacion focal se registran; QA manual se completa con pasos reproducibles. | Evita declarar regresion validada sin evidencia observable. |

## Resultado del refinamiento

La trazabilidad D-01 a D-06 se refleja en `design.md`, `spec.md` y `tasks.md`. DOC-9 mantiene abierta exclusivamente la QA manual 3.3; el cierre queda bloqueado hasta registrar esa evidencia.

<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-9 construye exclusivamente la fundacion paralela de contratos para una futura modernizacion de terminacion de tareas Workflow. El comportamiento vigente permanece en `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`; esta entrega no crea endpoints, JavaScript, modal, SQL funcional ni una transicion efectiva.

## Goals / Non-Goals

**Goals**

- Declarar contratos tipados, DTOs y limites de capas desde la raíz por tipo técnico, módulo y caso de uso.
- Preparar una habilitacion moderna segura y una frontera unica hacia el motor legacy.
- Dejar infraestructura de datos reutilizable sin mezclar reglas propias de Workflow.

**Non-Goals**

- D-01 / RQ-01: modificar el flujo WebForms actual, `Terminar_Tarea_Workflow` o `Cambia_Estado`.
- D-03 / RQ-03: ejecutar una tarea legacy desde la nueva capa.
- D-04 / RQ-04: habilitar usuarios reales ni reemplazar el piloto visual existente.
- D-05 / RQ-05: crear `GenericRepository`, SQL de negocio o repositorios Workflow concretos.

## Decisions

### D-01 — Fundación paralela sin efectos sobre el flujo vigente

La fundación moderna, organizada por tipo técnico desde la raíz, coexiste con la implementación existente. La entrega no modifica `Webworkflow.aspx`, su code-behind, `ClassWorkflow.Terminar_Tarea_Workflow` ni `ClassWorkflow.Cambia_Estado`; esto preserva autorizacion, firma, expediente, balanceo y eventos dinamicos del camino actual. RQ-01 define la evidencia de esta separacion.

### D-02 — Capas y contratos tipados sin dependencias WebForms

Domain contiene modelos y puertos; Application contiene DTOs, validacion y orquestacion; Infrastructure implementa configuracion, datos y adaptadores. Presentation futura solo podra consumir DTOs. Físicamente, los componentes se agrupan desde la raíz por tipo técnico, módulo y caso de uso: `Modelo/Workflow/Terminar/` contiene modelos y puertos de Domain; `DTOs/Workflow/Terminar/` contiene DTOs; `Services/Workflow/Terminar/` contiene la fachada, proveedores y validador; e `Infrastructure/Repositories/Workflow/` delimita repositorios futuros de negocio. Los tipos nuevos no dependen de `Page`, `Session`, `GridView`, `UpdatePanel` ni `ModalPopupExtender`. RQ-02 verifica los limites mediante inspeccion focal.

### D-03 — Adaptador legacy exclusivo e inerte en esta fase

`Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb` implementa `IWorkflowLegacyExecutor` y es la unica frontera nueva reservada para `Terminar_Tarea_Workflow` y `Cambia_Estado`. En DOC-9 retorna `WORKFLOW_MODERN_EXECUTION_PENDING`; no reconstruye `Page` o `Session`, no llama al motor y no duplica reglas legacy. RQ-03 impide una transicion accidental.

### D-04 — Habilitación de servidor fail-closed

`Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` implementa `IWorkflowModernFeatureGate` a partir de `ContextoModuloWorkflow`. Sin configuracion explicita `WorkflowCentroTrabajoModernActive`, con valor invalido o con usuario/grupo no autorizado, retorna `inactivo`; una exclusion retorna `excluido`. La bandera se mantiene independiente del piloto visual existente. RQ-04 protege la activacion gradual posterior.

### D-05 — Infraestructura compartida, repositorios por dominio

`Infrastructure/Shared/Data` desde la raíz declara conexión, ejecución parametrizada, transacción, paginación y resultado técnico reutilizables. `Domain/Shared/ContextoModulo` representa el contexto mínimo común; `ContextoModuloWorkflow` lo especializa en el dominio Workflow. `ModuleConnectionFactory`, `AdoNetDataExecutor` y `DbTransactionFactory` no contienen nombres, modelos ni códigos `WORKFLOW_*`. Domain declara contratos de repositorio exclusivos para Workflow y `Infrastructure/Repositories/Workflow/` reserva sus implementaciones. Los repositorios futuros recibirán `ContextoModuloWorkflow`, no leerán `HttpContext.Current.Session`, no devolverán `DataSet` ni HTML y no usarán un repositorio genérico. RQ-05 conserva reutilización sin acoplamiento de dominio.

### D-06 — Evidencia reproducible sin afirmar QA no ejecutado

La evidencia automatizada de DOC-9 consiste en compilacion MSBuild y `tools/validation/Verify-Doc9Foundation.ps1`. La verificacion manual del flujo WebForms se mantiene como tarea abierta hasta registrar ambiente, pasos y resultado; no se sustituye con una afirmacion documental. RQ-06 separa la evidencia observada de la validacion manual pendiente.

## Risks / Trade-offs

- La capa moderna no puede terminar tareas hasta una fase posterior que componga de forma segura el adaptador, pilotee usuarios y compare resultados contra el camino vigente.
- El fail-closed puede dejar usuarios fuera de la experiencia moderna si la configuracion no existe; es el comportamiento deseado para esta fundacion.
- La solucion WebForms no dispone de una suite aislada de pruebas unitarias; por ello se conserva compilacion y verificacion estatica focal, complementadas con QA manual reproducible.

## Migration Plan

1. D-01 a D-05: mantener esta fundacion sin conexiones de interfaz ni ejecucion legacy.
2. D-04: una fase posterior configura el piloto con el mismo contrato `IWorkflowModernFeatureGate`.
3. D-03: una fase posterior implementa preview y ejecucion con equivalencia, rollback y aprobacion antes de habilitar el adaptador.
4. D-06: registrar QA manual del flujo existente antes de cerrar DOC-9.

## Open Questions

No hay decisiones tecnicas abiertas dentro de la fundacion. La composicion de preview, endpoint y piloto pertenece a los cambios posteriores definidos por los prompts 02 a 06.

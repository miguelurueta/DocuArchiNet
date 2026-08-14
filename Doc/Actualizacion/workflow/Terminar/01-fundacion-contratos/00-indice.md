# Fundación paralela y contratos

- Ticket: DOC-9 — CONTRATO-TERMINAR-TAREA-WORKFLOW
- Cambio OpenSpec: doc-9-contrato-terminar-tarea-workflow
- Clasificacion: cross_cutting
- Fecha: 2026-08-13
- Estado: implementado; QA manual de regresión aceptada por usuario responsable
- Alcance: contratos internos, habilitación fail-closed y base reutilizable de acceso a datos para la modernización gradual del envío de tareas.

## Resumen de cambios

Se creó la fundación sin cambiar la interfaz, el code-behind ni la transición legacy. Los modelos, DTOs, servicios Application y componentes de Infrastructure se ubican desde la raíz por tipo técnico, módulo y caso de uso; `Infrastructure/Repositories/Workflow/` conserva el límite de repositorios futuros. `Domain/Shared` e `Infrastructure/Shared/Data` alojan el contexto y los componentes técnicos comunes sin acoplamiento a Workflow.

La estructura física oficial de DOC-9 es `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar`, `Services/Workflow/Terminar`, `Domain/Shared`, `Infrastructure/Shared/Data`, `Infrastructure/Workflow/Terminar` e `Infrastructure/Repositories/Workflow`. No queda código de la fundación bajo `workflow/modern/`.

No se agregaron ASMX, JavaScript, modal, consultas Workflow, SQL nuevo ni una transición moderna ejecutable. El adaptador devuelve `WORKFLOW_MODERN_EXECUTION_PENDING`; por lo tanto no puede enviar tareas mientras no exista un cambio posterior aprobado.

## Paquete documental

| Archivo | Contenido |
|---|---|
| `01-arquitectura.md` | Capas, dependencias y decisiones. |
| `02-contrato.md` | DTOs, interfaces y resultados funcionales. |
| `03-flujo-y-seguridad.md` | Límites legacy, habilitación, riesgos y reversa. |
| `04-pruebas-y-evidencia.md` | Compilación, verificaciones y QA manual registrada. |
| `Diagramas/fundacion-componentes.mmd` | Diagrama de componentes y fronteras. |

## Archivos relacionados

- `Modelo/Workflow/Terminar/WorkflowModernModels.vb`
- `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`
- `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`
- `Services/Workflow/Terminar/ServicioTransicionTarea.vb`
- `Services/Workflow/Terminar/ValidadorTransicionTarea.vb`
- `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb`
- `Domain/Shared/ContextoModulo.vb`
- `Infrastructure/Shared/Data/ModuleDataContracts.vb`
- `Infrastructure/Shared/Data/ModuleConnectionFactory.vb`
- `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb`
- `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb`
- `Infrastructure/Repositories/Workflow/README.md`
- `GestionDocumental-Docuarchi.net.vbproj`

## Comportamiento preservado

Se preservan sin modificación `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/ClassWorkflow.vb`, el envío por ruta y flujo, las verificaciones de autorización, firma, expediente y aprobaciones, los eventos `PRETERMINARACTIVIAD` / `TERMINARACTIVIDAD`, correo, trazabilidad y `Cambia_Estado`.

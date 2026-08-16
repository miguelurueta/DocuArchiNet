# DOC-11 — Ejecución segura de transición

Estado: implementación y pruebas automatizadas preparadas. La ejecución E2E mutante queda pendiente de una tarea y cuentas descartables autorizadas.

Fecha: 2026-08-15.

## Alcance

Se agrega `EjecutarEnvioTarea` al ASMX moderno existente. El navegador solo expresa intención mediante tarea, conector y versión; el servidor vuelve a resolver sesión, permisos, tarea, destino y requisitos antes de delegar al motor legacy.

No se modificaron `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb` ni `workflow/ClassWorkflow.vb`.

## Lectura del paquete

- [Arquitectura](01-arquitectura.md): capas, inventario, decisiones y límite legacy.
- [Contrato](02-contrato.md): entrada, salida, códigos y compatibilidad ASMX.
- [Flujo y seguridad](03-flujo-y-seguridad.md): revalidación, concurrencia, piloto y reversa.
- [Pruebas y evidencia](04-pruebas-y-evidencia.md): build, pruebas reutilizables y QA manual.
- [Diagramas](Diagramas/): componentes, secuencia, concurrencia y estados en Mermaid.

## Archivos principales

| Área | Archivos |
| --- | --- |
| Punto de entrada | `webservice/WebServiceWorkflowModern.asmx.vb`, `webservice/WorkflowPreviewSessionContextGate.vb` |
| Application | `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `ValidadorTransicionTarea.vb` |
| Domain | `Modelo/Workflow/Terminar/WorkflowModernModels.vb`, `WorkflowModernInterfaces.vb` |
| Infrastructure | `Infrastructure/Repositories/Workflow/MySqlTransicionEjecucionRepository.vb`, `Infrastructure/Workflow/Terminar/*.vb` |
| Contrato | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` |
| Pruebas E2E | `tools/e2e/tests/doc11-execution.spec.cjs`, `tools/e2e/scripts/run-doc11-concurrency.cjs` |

## Resultado

El único nuevo llamador de `ClassWorkflow.Terminar_Tarea_Workflow` es `WorkflowLegacyExecutorAdapter`. `Cambia_Estado`, `PRETERMINARACTIVIAD`, `TERMINARACTIVIDAD`, correo y trazabilidad base continúan dentro del motor legacy.

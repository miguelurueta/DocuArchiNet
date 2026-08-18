# DOC-13 — Confirmación especializada y envío asíncrono

| Campo | Valor |
| --- | --- |
| Ticket | `DOC-13` — CONFIRMACION-ESPECIALIZADA |
| Fecha | 2026-08-16 |
| Estado | Implementado; pendiente de compilación y QA manual final |
| Alcance | Presentation JavaScript/CSS, integración con contratos ASMX existentes y actualización visual correlacionada |
| Rollback | Desactivar `WorkflowCentroTrabajoModernActive`; la página deja de registrar los assets modernos y conserva el flujo legacy |

## Alcance entregado

1. `ConfirmationDialog` reutilizable y accesible, sin dependencia de Workflow, Web Forms, Session, controles ocultos o texto del caso de uso.
2. Adaptador de Workflow que recibe el destino seleccionado, invoca solo `EjecutarEnvioTarea(idTarea, idConector, tokenVersion)` y normaliza su resultado.
3. Callback de página basado en atributos `data-workflow-*` que actualiza la fila, contexto, visor, contador y mensaje solo tras éxito correlacionado.
4. Pruebas focales de selección, diálogo, adaptador y callback de página.

## Archivos relacionados

| Área | Rutas |
| --- | --- |
| Componente | `js/java_general/ConfirmationDialog.js`, `Styles/confirmation-dialog.css` |
| Adaptadores | `js/workflow/workflow-transition-confirmation-integration.js`, `js/workflow/workflow-transition-page-presentation.js`, `js/workflow/workflow-transition-ui.js` |
| Host Web Forms | `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/ClassListandoTareas.vb` |
| Contrato existente | `webservice/WebServiceWorkflowModern.asmx.vb`, `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` |
| Pruebas | `tests/workflow-transition-ui.test.cjs`, `tests/confirmation-dialog.test.cjs`, `tests/workflow-transition-confirmation-integration.test.cjs`, `tests/workflow-transition-page-presentation.test.cjs` |

## Contenido del paquete

- [Arquitectura](01-arquitectura.md)
- [Contrato](02-contrato.md)
- [Flujo y seguridad](03-flujo-y-seguridad.md)
- [Pruebas y evidencia](04-pruebas-y-evidencia.md)
- [Diagramas](Diagramas/)

## Límites explícitos

DOC-13 no modifica `Terminar_Tarea_Workflow`, `Cambia_Estado`, autorización, requisitos de negocio, repositorios, SQL, Session, DTOs ni endpoints. El cliente no infiere trámite, actividad de origen legible, requisitos o advertencias cuando el preview no los publica.

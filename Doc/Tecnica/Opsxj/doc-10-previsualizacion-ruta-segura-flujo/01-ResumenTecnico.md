# PREVISUALIZACION-RUTA-SEGURA-FLUJO

- Ticket: DOC-10
- Cambio OpenSpec: doc-10-previsualizacion-ruta-segura-flujo
- Clasificacion: cross_cutting (Transversal)
## Objetivo

El endpoint paralelo previsualiza destinos de una tarea Workflow mediante JSON de solo lectura, sin modificar el flujo Web Forms ni su terminación legacy.

## Alcance y compatibilidad

- [x] Se agregan el ASMX paralelo `webservice/WebServiceWorkflowModern.asmx`, su code-behind y capas de lectura tipadas. `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb` no cambian.
- [x] El preview solo lee; no termina, cambia estado, envía correo ni invoca el motor legacy. La reversa es apagar `WorkflowCentroTrabajoModernActive` y vaciar sus listas de alcance.

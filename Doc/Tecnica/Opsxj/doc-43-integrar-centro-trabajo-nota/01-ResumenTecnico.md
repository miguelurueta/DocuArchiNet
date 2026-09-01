# INTEGRAR-CENTRO-TRABAJO-NOTA

- Ticket: DOC-43
- Cambio OpenSpec: doc-43-integrar-centro-trabajo-nota
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Modernizar el consumidor de Notas del Centro de Trabajo con los contratos ASMX aprobados, manteniendo el flujo Web Forms legacy como reversa cuando el gate esté inactivo.

## Alcance y compatibilidad

### Inventario inicial

- Consumidor Web Forms: `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- Cliente existente: `js/workflow/Webworkflow.js`.
- Contratos modernos disponibles: `ListarNotas`, `ContarNotas`, `CrearNota`, `ActualizarNota` y `EliminarNota` en `webservice/WebServiceWorkflowNotesModern.asmx.vb`.
- Repositorio moderno: `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb`.
- Referencia visual: `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-ui-notas-workflow-moderno.html`.
- Compatibilidad: conservar GridView, postbacks y eventos legacy; no activar el gate durante el desarrollo.

- [x] Páginas, controles, servicios y scripts afectados identificados en el inventario.
- [x] Comportamiento preservado: modal, GridView, postbacks y eventos legacy. Reversa: mantener `WorkflowCentroTrabajoModernActive=false`.

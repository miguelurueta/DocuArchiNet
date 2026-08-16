# LISTA-MODERNA-DESTINO

- Ticket: DOC-12
- Cambio OpenSpec: doc-12-lista-moderna-destino
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Entregar una lista moderna de destinos para Continuar flujo que consulte solo `PreviewEnviarTarea`. La vista es una mejora progresiva: el servidor publica el bootstrap únicamente cuando `IWorkflowModernFeatureGate` habilita al contexto autenticado que también protege el ASMX.

## Alcance y compatibilidad

- Afecta `workflow/Webworkflow.aspx`, el nuevo bootstrap de Presentation, `js/workflow/workflow-transition-ui.js` y `Styles/workflow-transition-modern.css`.
- Se conservan sin cambio funcional el enlace original, `ImageButtonterminar`, `GridView_envia_flujo` y el modal Web Forms legacy mientras el bootstrap esté ausente o sea `false`.
- No se altera Application, Domain, Infrastructure, el motor de transición, correo, auditoría ni el endpoint de ejecución.
- El rollback consiste en desactivar `WorkflowCentroTrabajoModernActive` para el piloto: la siguiente carga vuelve al recorrido legacy sin migración de datos.

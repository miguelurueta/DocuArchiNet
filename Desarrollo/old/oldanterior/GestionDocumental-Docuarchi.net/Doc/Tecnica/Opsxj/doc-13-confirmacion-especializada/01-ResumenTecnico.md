# CONFIRMACION-ESPECIALIZADA

- Ticket: DOC-13
- Cambio OpenSpec: doc-13-confirmacion-especializada
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Incorporar una confirmación reutilizable antes de enviar una tarea desde el flujo moderno. El diálogo separa la presentación de la transición: muestra el destino ya seleccionado, confirma la acción y comunica resultados sin sustituir las reglas del servidor.

Los componentes principales son `ConfirmationDialog.js`, el adaptador `workflow-transition-confirmation-integration.js`, la presentación de página y el registro condicional de activos en `Webworkflow.aspx.vb`.

## Alcance y compatibilidad

- La página afectada es `workflow/Webworkflow.aspx`; el punto de entrada es la selección de destino entregada por `workflow-transition-ui.js`.
- El adaptador conserva el contrato ASMX existente y llama a `WebServiceWorkflowModern.asmx/EjecutarEnvioTarea` con identificador de tarea, conector y versión.
- No se cambian decisiones de autorización, requisitos ni reglas de asignación: permanecen en la capa de servidor existente.
- El piloto queda protegido por el gate de presentación. Con el gate desactivado no se cargan los activos nuevos y se mantiene el comportamiento legacy.
- La reversa consiste en desactivar el gate y retirar los activos de presentación; no requiere migración de datos ni modificación de la tarea.

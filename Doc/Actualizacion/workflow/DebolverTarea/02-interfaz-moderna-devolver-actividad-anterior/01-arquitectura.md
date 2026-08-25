# Arquitectura y componentes

DOC-33 incorpora una presentación oficial propia para la acción **Elegir actividad anterior** dentro de `workflow/Webworkflow.aspx`. La integración se registra siempre que la página cargue sus recursos de transición: no depende del gate histórico de Centro de Trabajo y no comparte el estado de Continuar flujo, Enviar a usuario o Enviar a grupo.

| Capa | Componentes | Responsabilidad |
| --- | --- | --- |
| Markup Web Forms | `workflow-return-activity-trigger`, `workflow-return-activity-modern-modal`, vista de tabla y tarjetas | Expone el disparador, el diálogo accesible y una representación responsive de destinos. |
| Bootstrap | `RegisterWorkflowReturnActivityModernPresentation` y `RegisterWorkflowReturnActivityModernBootstrap` | Registra assets exclusivos y publica solo `IdTarea` de la tarea visual actual. |
| Preview UI | `workflow-return-activity-ui.js` | Consulta, normaliza, busca, pagina y selecciona una actividad anterior autorizada. |
| Confirmación | `workflow-return-activity-confirmation.js`, `ConfirmationDialog.js` | Confirma la selección vigente, previene doble envío y ejecuta el contrato mínimo. |
| Presentación común | `WorkflowTransitionPagePresentation`, `centro-trabajo-visual.js`, `workflow-transition-modern.css` | Actualiza únicamente la tarea afectada, conserva contadores y adapta tabla/tarjetas. |
| Servidor existente | `PreviewDevolverActividad` y `EjecutarDevolverActividad` de DOC-32 | Reconstruye autorización, cursor, destino y concurrencia; el navegador no reproduce reglas de negocio. |

La relación entre límites se ilustra en [arquitectura de interfaz](Diagramas/01-arquitectura-ui.md) y la secuencia completa se detalla en [preview y ejecución](Diagramas/02-secuencia-preview-ejecucion.md).

## Fronteras preservadas

El módulo nuevo no llama `ClassWorkflow`, no accede a SQL ni usa `Page`, sesiones, cookies o credenciales. La única mutación permanece detrás de la ejecución DOC-32. El resultado se integra mediante la presentación compartida con el identificador de tarea correlacionado, por lo que una respuesta nunca refresca arbitrariamente la lista de otras tareas.

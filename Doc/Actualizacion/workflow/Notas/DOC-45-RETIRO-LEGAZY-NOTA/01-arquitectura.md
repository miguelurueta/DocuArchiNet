# Arquitectura

## Frontera funcional

`Webworkflow` presenta Notas exclusivamente mediante `WorkflowNotesModern`. El acceso visible muestra el contador y abre un diálogo superpuesto; cuando el total es cero cambia a `Nueva nota 0` y abre directamente el editor. La lista, editor, visor completo y confirmador de eliminación viven dentro de la misma superficie, con tamaño estable, scroll interno y restitución de foco.

El cliente siempre obtiene la tarea desde `Hidden_id_tarea_selecionada`. Sus eventos se delegan en `document` porque el `UpdatePanel` puede reemplazar la barra. Después de cada postback parcial, `PageRequestManager.endRequest` relee la tarea explícita y sincroniza el contador mediante una consulta autorizada de solo lectura.

## Backend y seguridad

`WebServiceWorkflowNotesModern.asmx` conserva los contratos transaccionales de DOC-42. Modelo, DTO y repositorio exponen `PuedeGestionar`, calculado con el autor persistido y el usuario autenticado. `UPDATE` y `DELETE` validan atómicamente tarea, actividad vigente, autor y versión; una mutación ajena responde `NotOwner` sin auditoría de éxito.

## Compatibilidad conservada

Los endpoints `Service_*_nota_tarea_workflow`, `Eliminar_nota_service_workflow` y `WebFormAnotacion.aspx` permanecen porque conservan consumidores en Radicación o Correspondencia. El consumidor antiguo de `Webworkflow`, sus controles, handlers y llamadas JavaScript se retiran. No existe doble presentación ni doble escritura.

No se agregó DDL ni migración de datos. El gate permanece en su estado seguro: `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos. La vista integral está en [el diagrama de arquitectura](Diagramas/01-arquitectura.md).

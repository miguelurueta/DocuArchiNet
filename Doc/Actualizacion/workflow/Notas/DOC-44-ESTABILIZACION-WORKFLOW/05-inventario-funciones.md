# Inventario de funciones y deuda

| Canal | Símbolos | Estado DOC-44 |
| --- | --- | --- |
| Moderno | `WorkflowNotesModern`, `Panel_notas_modernas`, `ConfigureWorkflowNotesModernPresentation` | Estabilizado y cubierto por política. |
| Contexto | `Hidden_id_tarea_selecionada` | Identidad explícita entregada al cliente. |
| Legacy | `ImageButtonanotacion(_)_Click`, `GridView_lista_notas`, `ModalPopupExtender_edition_content_anotacion` | Preservado para rollback. |
| Legacy JS | `Event_note_workflow`, `Button_Show_Guardar`, funciones de filas GridView | Preservado; no invocado por cliente moderno. |
| Legacy dominio | `Class_anotacion_tarea` | Preservado sin cambios. |

## Criterio fase 06

Solo se podrá retirar legacy cuando búsqueda estática, telemetría y regresión autorizada demuestren cero referencias/ejecuciones de sus handlers, controles, funciones y clase, y exista decisión de negocio para retirar el fallback.

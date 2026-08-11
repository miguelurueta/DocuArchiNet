# Contrato de estado de selección

## Tarea

| Estado / campo | Fuente y formato | Actualización | Regla |
| --- | --- | --- | --- |
| Tarea canónica | `Session("ID_TAREA_SELECCIONDA")`; ID de tarea, `0` o `-1` | `ButtonSeleccionGrupo_Click` y handlers de negocio | Autoridad posterior al postback. |
| Tarea candidata | `Hidden_id_tarea_sel`; ID de tarea, inicialmente `-1` | JavaScript de selección antes del clic puente | Entrada al postback; no autoridad final. |
| Espejo seleccionado | `Hidden_id_tarea_selecionada`; ID, `0` o `-1` | Code-behind tras leer/actualizar sesión | Permite que JS decida habilitación/renderizado; no sustituye sesión. |

**Invariante:** después de seleccionar una tarea y completar el postback, la sesión y los hidden de tarea que el handler sincroniza deben representar el mismo ID. Si no hay tarea, se deben preservar los centinelas `0` o `-1` definidos por el flujo existente.

## Documento

| Contexto / campo | Fuente y formato | Actualización | Regla |
| --- | --- | --- | --- |
| Documento activo | `hiden_seleccion_documento_wf`; descriptor delimitado por `\|` | Evento JS `vis_doc_selecion_wf` | Alimenta el handler de visor; no asumir que es solo un ID. |
| Identificador de fila activa | `hiden_seleccion_documento_id_wf` | Mismo evento JS | Identifica la fila y el resaltado actual. |
| Selección masiva | checkbox de `GridView_list_documento_relacion_wf` y flujos de acción | Interacción de grilla / handlers específicos | Independiente del documento activo. |

**Invariantes:** abrir un documento actualiza descriptor e ID de fila antes de pulsar `Button_selecion_treview_documento`; marcar checkbox no puede alterar por sí solo el documento abierto ni el visor. Los cambios visuales pueden pintar estado, pero no escribir estos valores salvo por el mecanismo existente.


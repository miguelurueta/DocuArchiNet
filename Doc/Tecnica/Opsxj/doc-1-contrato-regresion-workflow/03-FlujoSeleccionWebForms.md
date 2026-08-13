# Flujo de selección WebForms

## Tarea

```text
Fila de tarea
  -> JavaScript escribe Hidden_id_tarea_sel
  -> clic ButtonSeleccionGrupo
  -> ButtonSeleccionGrupo_Click (servidor)
  -> Session("ID_TAREA_SELECCIONDA")
  -> Hidden_id_tarea_selecionada / Hidden_id_tarea_sel sincronizados
  -> UpdatePanel_general_variable y paneles dependientes re-renderizados
```

## Documento, visor e índice

```text
Fila de documento / evento vis_doc_selecion_wf
  -> JS escribe hiden_seleccion_documento_wf y ..._id_wf
  -> clic Button_selecion_treview_documento
  -> Button_selecion_treview_documento_Click (servidor)
  -> Visualiza_documento_workflow_visor
  -> UpdatePanelindice + UpdatePanelVisor + iframe del visor
```

## Límites de actualización parcial

| Área | Panel o contenedor | Comprobación tras postback |
| --- | --- | --- |
| Cabecera | `UpdatePanel_menu_cab` | Menú, acciones y sus eventos siguen disponibles. |
| Selección/lista documental | `UpdatePanelseleccion` | Hidden inputs y filas conservan el estado previsto. |
| Índice | `UpdatePanelindice` dentro de `#contenido_indice` | El índice corresponde al documento activo. |
| Visor | `UpdatePanelVisor` dentro de `#contenido_imagen` | El iframe/documento mostrado corresponde al descriptor. |
| Filtros y etiquetas | `UpdatePanelseleccionfiltro`, paneles de selección auxiliares | No se pierden los handlers necesarios ni se alteran permisos. |

La página registra comportamiento de AJAX mediante `PageRequestManager` y `Sys.Application`. Cualquier capa visual futura debe inicializarse de forma idempotente después de los reemplazos parciales; no debe añadir listeners duplicados por cada `endRequest`.


# Contrato de controles críticos

## Fuentes inspeccionadas

- `workflow/Webworkflow.aspx`: estructura, IDs WebForms, `UpdatePanel` y botones ocultos.
- `workflow/Webworkflow.aspx.vb`: handlers, sesión y validaciones de negocio/permisos.
- `js/workflow/Webworkflow.js`: selección en cliente, clics puente y cálculo de dimensiones.

La fuente de autorización efectiva es servidor/code-behind y sus clases de negocio; el cliente únicamente dispara eventos y refleja estado. Un cambio visual no debe tomar decisiones de permiso.

| Grupo | Control o selector | Disparador / propietario técnico | Postback y riesgo de regresión |
| --- | --- | --- | --- |
| Marco | `#div_content_general_wf` | `Webworkflow.aspx`; dimensiones en `Webworkflow.js` | No mover; su alto condiciona tareas, índice y visor. |
| Menú | `UpdatePanel_menu_cab`, `#menucab` | página y ciclo ASP.NET AJAX | Parcial; listeners/estilos deben sobrevivir al reemplazo. |
| Tareas | `Panelactividad`, `GridView2` | script de lista y `ButtonSeleccionGrupo_Click` | Selección cambia contexto de servidor. |
| Seleccionar tarea | `Hidden_id_tarea_sel`, `ButtonSeleccionGrupo` | `prevent_lista_tareas()` → code-behind | Parcial; no confundir candidato con tarea consolidada. |
| Contexto de tarea | `Hidden_id_tarea_selecionada` | sesión `ID_TAREA_SELECCIONDA` en code-behind | Espejo de cliente; `0`/`-1` representan ausencia/no selección. |
| Documentos | `UpdatePanelseleccion`, `GridView_list_documento_relacion_wf` | página/code-behind | Parcial; no romper IDs de filas ni checkbox. |
| Abrir documento | `hiden_seleccion_documento_wf`, `hiden_seleccion_documento_id_wf`, `Button_selecion_treview_documento` | evento `vis_doc_selecion_wf` en JS → handler VB | Parcial; el descriptor alimenta visor e índice. |
| Índice | `#contenido_indice`, `UpdatePanelindice`, `Panel_indice` | handler de selección documental | Parcial; se muestra/oculta con reglas de ancho heredadas. |
| Visor | `#contenido_imagen`, `UpdatePanelVisor`, `#ifrm_visor_` | `Visualiza_documento_workflow_visor` | Parcial; no cambiar ruta, iframe ni su tamaño calculado. |
| Acciones de tarea | `ImageButtonpendiente`, `ImageButtonanotacion`, `ImageButtonterminar`, `ImageButtonEnviarUsuario`, `ImageButtonEnviaActividad` | handlers VB y validaciones de sesión | Postback de negocio; comprobar cuenta con y sin permiso. |
| Devolver / clasificar | `Button_tool_devolver_a_usuario`, `Button_tool_devolver_a_actividades_anterior`, `Button_clasficar_documento` | handlers VB | Postback de negocio; conservar `onclick`, nombre e ID. |

## Invariantes de cambio visual

- No renombrar ni duplicar IDs, hidden inputs, botones puente o `UpdatePanel`.
- No reemplazar `onclick` ni convertir acciones de servidor en navegación de cliente.
- No usar visibilidad visual como sustituto de una validación de permiso de servidor.
- Revalidar las acciones tras cada actualización parcial del panel que las contiene.


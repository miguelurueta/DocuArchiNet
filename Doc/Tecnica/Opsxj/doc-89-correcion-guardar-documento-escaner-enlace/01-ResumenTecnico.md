# DOC-89 - Corrección del progreso al guardar desde escáner en Enlace

- Ticket: DOC-89
- Cambio OpenSpec: doc-89-correcion-guardar-documento-escaner-enlace
- Clasificacion: cross_cutting

## Objetivo

Evitar que el progreso del almacenamiento digitalizado cubra de blanco toda la interfaz de Enlace, preservando el async postback, una sola persistencia y el componente de escáner compartido.

## Diagnóstico confirmado

El video de reproducción muestra que la página no navega ni hace postback completo. `WebFormEscan.aspx` termina la carga Dynamsoft, `activa_document_save()` pulsa `window.parent.ButtonAlmacenar` y Microsoft AJAX ejecuta un async postback de `Webworkflow.aspx`.

La pantalla blanca era la composición accidental de dos responsabilidades sobre `#progres_bar`:

- `posicion_update_pogres_modal()` agregaba `.overlay_`, ancho/alto completos y posición `(0,0)`.
- `.ctw-loading-indicator` aportaba el fondo blanco de la tarjeta moderna.

La interfaz permanecía detrás del nodo blanco hasta `endRequest`; no era destruida ni reconstruida.

## Corrección implementada

`workflow/Webworkflow.aspx` separa `ButtonAlmacenar` de `Button_guardar_desicion`. El primero usa un indicador fijo de 200 px, centrado y sin `.overlay_`. Antes de modificar estilos captura el estado inline del indicador y del spinner; en `finally` lo restaura de forma idempotente y oculta el progreso.

No se modificaron `WebFormEscan*`, Dynamsoft, `online_demo_operation.js`, `ClassAlmacenamiento`, `ButtonAlmacenar_Click`, estilos globales ni la proyección incremental del nodo.

## Alcance y compatibilidad

La única superficie productiva editada es el manejo cliente de `ButtonAlmacenar` en `workflow/Webworkflow.aspx`. La compatibilidad queda protegida mediante pruebas sobre `Button_guardar_desicion`, el UpdatePanel, la persistencia/proyección y las ramas `Hidden21` del escáner compartido.

## Reversa

Revertir únicamente las funciones `mostrar_progreso_almacenamiento_digitalizado` y `limpiar_progreso_almacenamiento_digitalizado`, y restaurar la condición previa de `InitializeRequest`/`CheckStatus`. No existe migración de datos ni cambio de almacenamiento que revertir.

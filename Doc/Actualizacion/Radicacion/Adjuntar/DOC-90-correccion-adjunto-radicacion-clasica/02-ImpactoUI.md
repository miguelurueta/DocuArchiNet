# DOC-90 — Impacto UI

## Superficie modificada

La única pantalla modificada es `radicador/WebFormRadicacionEntrante.aspx`. Su función `ActivaAdjuntarDocumentoRadicacion` conserva modal, tipologías, preview, selección múltiple y `funcion_name = "insert_row_documento_relacionado"`; cambia `evento_adjunta` a `ADJUNTARADICACION_CLASICA`.

También se corrigió el estado inicial de sus pestañas. El número de tareas pendientes es informativo y no equivale a una asignación. Cuando `Hidden_radicado_seleccion` está vacío en la carga inicial:

- `Recepción y radicación` queda activa y habilitada.
- `Envío y soporte documental` queda inactiva y deshabilitada.
- Sus acciones de adjuntar, eliminar y firmar documentos no quedan visibles ni accesibles.
- La campana `Pendientes`, su total y la apertura de la lista permanecen visibles en el encabezado común porque son el mecanismo para seleccionar la tarea, no una acción documental.

Después de que `CheckStatus` recibe `Hidden_result_boton_tool = "YES"`, `asig_radicado_tab()` habilita y selecciona Soporte directamente. No vuelve a rechazar la transición leyendo un oculto anterior al postback. Al iniciar un radicado nuevo o terminar la tarea actual, la pantalla vuelve a Recepción y bloquea Soporte, aunque todavía existan tareas pendientes. La selección usa operaciones idempotentes.

Para conservar la sincronización Web Forms, `Hidden_radicado_seleccion` forma parte de `UpdatePanel_boton_tool`. La campana, `Label_numero_item` y `Hidden_numero_rad_pend` están dentro de `UpdatePanel_pendientes_radicacion`, visible y actualizado en cada postback parcial.

La referencia se versionó como:

```text
../js/radicacion/WebFormRadicacionEntrante.js?v=20261007-doc90-3
```

## Proyección

`generic_control/FileUploadHandler.js` no fue modificado. La rama `funcion_name == "insert_row_documento_relacionado"` construye la fila con gabinete, imagen, radicado, tipología, tarea y firma, y ejecuta una sola vez `insert_row_documento_relacionado(...)`. Esta rama no depende del nombre del evento y no ejecuta postback, `DataBind` ni recarga.

## Superficies preservadas

- Radicación Simplificada conserva su evento, pantalla, recurso y proyección DOC-85.
- No se cambiaron estilos, foco, hover ni responsive; se sincronizaron `aria-selected` y `aria-disabled` con el estado real.
- No se agregaron globales JavaScript ni estado compartido nuevo.

La validación visual autenticada está automatizada en `tools/e2e/tests/doc90-radicacion-classic-attachment.spec.cjs`: antes de seleccionar comprueba Recepción activa, Soporte deshabilitado, el botón de adjuntar oculto y la campana `A1` visible; después selecciona el registro pendiente por `a_s_r_p_333`, comprueba Soporte activo, abre `a_load_file`, verifica una fila `id_rad` nueva y controla navegación/postback. Permanece pendiente de ejecución porque no se recibió autorización explícita de ambiente, cuenta, carga y recurso descartable.

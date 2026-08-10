# JIRA-06 — Documentos relacionados, visor e índice

## Prompt para Jira

**Rol:** Actúa como arquitecto UI senior especializado en tablas operativas, gestión documental, visores PDF y sincronización de selección documento–metadatos.

Consolida la experiencia de documentos relacionados dentro del centro de trabajo, preservando la relación documento activo → visor → índice/metadatos.

### Contrato visual obligatorio

La barra del visor aplica `.ctw-document-bar`; sus acciones usan `.ctw-btn` y el dropdown existente se adapta a `.ctw-menu`. La fila seleccionada aplica `.ctw-document-row--selected`, con fondo `#edf3ff` y borde interno izquierdo azul de 3 px, idéntico al HTML base. Estos estilos deben reutilizar tokens y nunca duplicar colores, alturas o sombras locales.

### Alcance

- Aplicar selección única de fila con borde izquierdo de contexto y sin bordes redondeados inconsistentes.
- Corregir ancho de columna checkbox, tamaño del control, separación izquierda y consistencia de hover/selección.
- Garantizar truncamiento real con `text-overflow: ellipsis`, `min-width: 0` y tooltip/nombre accesible para títulos largos.
- Alinear color de `th`, celda checkbox, hover y selección para una misma fila.
- Mantener menús de documento: eliminar, tipología, firma, versiones y reemplazar.

### Restricciones no negociables

- No cambiar atributos `id_wf`, `idd_wf`, `tip_event` ni IDs de controles.
- El evento de apertura debe conservar visor e índice actuales.
- El checkbox masivo conserva su función legacy.

### Entregables técnicos

1. `01-ContratoDocumentoActivo.md`.
2. `02-EspecificacionTablaDocumentos.md` con tamaños, truncamiento y estados.
3. Matriz de prueba de visor/índice/documentos.

### Criterios de aceptación

- Un título largo muestra puntos suspensivos visibles.
- La columna checkbox ocupa solo el ancho requerido.
- Seleccionar otro documento elimina de inmediato el estado anterior.
- Visor e índice operan sobre el documento activo correcto.

### Pruebas requeridas

- Títulos cortos/largos, documentos repetidos, menú abierto, firma, reemplazo y postback parcial.

### Reversión

Desactivar la hoja específica de documentos; no tocar eventos ni datos.

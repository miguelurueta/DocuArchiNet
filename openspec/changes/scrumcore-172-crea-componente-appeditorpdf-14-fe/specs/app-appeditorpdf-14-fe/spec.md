# Spec: app-appeditorpdf-14-fe

## Capability

`app-appeditorpdf-14-fe`

Extiende `AppEditorPdf` para soportar segmentacion visual por area util (content height) en modo `paginationMode="visual"`, sin modificar el HTML persistido del editor.

## Problem

La paginacion visual basada unicamente en altura total de pagina puede no representar de forma estable el area util real (altura disponible despues de margenes, guias y zoom). Se requiere segmentacion por area util para mantener cortes visuales consistentes y predecibles.

## Out Of Scope

- Paginacion estructural (no partir documento ni insertar marcadores persistidos).
- Export / PDF real.
- Logica de negocio de modulos consumidores.

## Requirements

### Effective Content Height

- Debe calcular `contentHeight` como `pageHeight - marginTop - marginBottom`.
- La segmentacion visual debe basarse en `contentHeight` (y `zoomLevel` cuando aplique).

### Visual Segmentation

- Debe renderizar separadores/espaciadores de hoja de manera visual, sin alterar el contenido persistido.
- Debe mantener scroll continuo (unico) y documento continuo.

### UX Stability

- No flicker, no salto de cursor, no perdida de seleccion durante:
  - escritura normal
  - pegado (paste)
  - carga de imagenes
  - resize

### Performance

- Evitar medicion de layout por keypress.
- Si se usan observers (ResizeObserver/scroll), deben ser acotados y debounced/throttled.

## Acceptance Criteria

1. `contentHeight` se calcula correctamente y se refleja en metricas publicadas.
2. La segmentacion visual sigue `contentHeight` y no depende de modificar HTML persistido.
3. Paste/typing no produce repaginacion agresiva con flicker.
4. Resize/zoom actualiza segmentacion sin romper foco/seleccion.

## Tests

- Unit: calculo de `contentHeight` y normalizacion de margenes/zoom.
- Integration UI: comportamiento estable en typing/paste/resize con paginacion visual.


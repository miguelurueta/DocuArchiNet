# Design: SCRUMCORE-174 (AppEditorPdf - Zoom Visual UI)

## Goal

Incorporar zoom visual a `AppEditorPdf` asegurando:

- Documento continuo (una instancia de editor).
- Paginacion visual consistente bajo zoom.
- UX estable (sin reset, sin perdida de seleccion, sin flicker).

## Proposed Approach

- `AppEditorPdf` actua como wrapper que:
  - calcula metricas (page/content sizes)
  - publica `zoomLevel` hacia el engine `AppEditor`
  - aplica estilos CSS/variables para overlays y guias
- `AppEditor` mantiene el engine y aplica zoom mediante variables CSS / layout controlado.

## Behavior Under Zoom

- `zoomLevel` afecta:
  - `pageStride` y calculo de pagina activa por scroll/cursor
  - overlays (page boundary / reading frame) para alinearse con el contenido
- Scheduling:
  - cambios de zoom disparan recomputo con debounce si hay slider.

## Risks

- Flicker por reflow/repaint:
  - Mitigar coalescing y evitar setState redundante.
- Scroll jump:
  - Capturar anchor y restaurar cuando se recalcula segmentacion.

## Non-Goals

- Controles UI complejos fuera del alcance del componente shared (se puede exponer API y que el consumidor renderice controles).


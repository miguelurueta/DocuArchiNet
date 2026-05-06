# SCRUMCORE-204 — Comportamiento del componente

## Estados

Se mantienen estados base del visor (engine loading / document loading / error / empty / success).

## Comportamiento de zoom

- Zoom In/Out cambia el zoom del documento activo usando `ZoomScope`.
- Reset vuelve a 100% (`requestZoom(1)`).
- `zoomLevel` mostrado en toolbar refleja `currentZoomLevel` del plugin.

## Anti-rerender

- Toolbar memoizada (`React.memo`).
- Handlers de zoom creados con `useCallback` para estabilidad.


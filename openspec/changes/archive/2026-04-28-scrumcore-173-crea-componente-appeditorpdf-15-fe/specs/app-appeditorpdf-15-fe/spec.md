# Spec: app-appeditorpdf-15-fe

## Capability

`app-appeditorpdf-15-fe`

Hardening del modo multi-hoja de `AppEditorPdf` (paginacion visual), enfocandose en estabilidad UX, consistencia de segmentacion y performance bajo operaciones costosas (paste, resize, imagenes).

## Out Of Scope

- Paginacion estructural (no modificar HTML persistido ni fragmentar el documento).
- Export/PDF real.
- Logica de dominio de modulos consumidores.

## Requirements

### Stability (No Flicker)

- Al navegar, escribir o pegar contenido:
  - no debe haber flicker del canvas/hojas
  - no debe perderse foco o seleccion
  - no deben existir saltos bruscos de scroll

### Robustness Under Heavy Ops

- Paste:
  - repaginacion debe ejecutarse de forma segura y predecible
  - no debe bloquear la escritura mas de lo estrictamente necesario
- Resize:
  - cambios de viewport deben actualizar segmentacion sin loops de layout
- Images:
  - al cargar/rehidratar imagenes, la segmentacion se revalida sin reiniciar el editor

### Performance

- No medicion de layout por keypress.
- Observers (ResizeObserver/scroll) acotados y debounced/throttled.
- Evitar trabajo redundante: no recalcular segmentacion si no cambia el estado relevante.

## Acceptance Criteria

1. Typing continuo no dispara repaginacion agresiva ni flicker.
2. Paste grande no rompe seleccion, y el resultado visual se estabiliza.
3. Resize no provoca loops de reflow ni perdida de foco.
4. Imagenes tardias (load/error) revalidan segmentacion sin bloquear escritura.

## Tests

- Integration UI: typing/paste/resize con paginacion visual.
- Regression: foco/seleccion preservados tras repaginacion.


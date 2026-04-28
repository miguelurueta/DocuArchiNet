# Spec: app-appeditorpdf-16-fe

## Capability

`app-appeditorpdf-16-fe`

Agregar zoom visual UI a `AppEditorPdf` para que el usuario pueda aumentar/disminuir el tamaño de la hoja/area de lectura, preservando el documento continuo y sin modificar el HTML persistido.

## Out Of Scope

- Zoom del browser o transformaciones fuera del componente.
- Export / PDF real.
- Cambios de paginacion estructural.
- Logica de dominio de modulos consumidores.

## Requirements

### Zoom Contract

- `AppEditorPdf` debe exponer `zoomLevel?: number` y `defaultZoomLevel?: number`.
- `zoomLevel` debe afectar el render visual:
  - escalado de hoja (page boundaries/reading frame)
  - calculo de pagina activa (si aplica)
  - calculo de segmentacion visual (page stride/content height)
- No debe alterar el HTML persistido del documento.

### UX Stability

- Cambiar zoom no debe:
  - perder foco/seleccion
  - hacer reset de editor
  - producir flicker (coalescing de recalculos)
- Debe mantener scroll continuo.

### Accessibility

- Controles de zoom (si el componente los expone) deben tener `aria-label` claros.
- El nivel actual debe ser anunciable (texto o aria-live si aplica).

### Performance

- Evitar recalculo en cascada por cada tick si hay slider/teclas.
- Preferir debounce/throttle para recalculos costosos.

## Acceptance Criteria

1. `zoomLevel` escala el render visual sin modificar el documento.
2. El editor conserva seleccion y foco al cambiar zoom.
3. La paginacion visual se mantiene consistente bajo zoom.
4. No hay flicker apreciable en cambios sucesivos de zoom.

## Tests

- Unit: normalizacion de `zoomLevel` y clamps (si aplica).
- Integration UI: cambio de zoom no rompe render ni callbacks.


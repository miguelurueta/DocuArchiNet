# Spec: app-appeditorpdf-18-fe

## Capability

`app-appeditorpdf-18-fe`

Auditoria tecnica y optimizacion de rerender para `AppEditorPdf` con el objetivo de reducir renders innecesarios, estabilizar props/callbacks y evitar work costoso repetido (especialmente en paginacion visual, zoom y guias).

## Out Of Scope

- Refactors de `AppEditor` (este ticket se enfoca en `AppEditorPdf`).
- Cambios funcionales visibles no requeridos por performance.
- Micro-optimizaciones sin medicion o sin criterio de aceptacion.

## Requirements

### Render Stability

- Cambios de props no relacionados al editor (ej. metadata externa) no deben causar:
  - recomposicion costosa de guias
  - re-creacion de callbacks sin necesidad
  - recalculo de metricas sin cambios relevantes

### Memoization & Contracts

- `AppEditorPdf` debe estabilizar:
  - objetos derivados (margenes, config de guias)
  - callbacks (`onPageContextChange`, `onMetricsChange`, etc.)
  - toolbar composition cuando sea posible

### Observability

- Exponer un mecanismo no-invasivo para diagnosticar rerenders:
  - optional debug flag o data-attributes (solo dev) o contadores internos para tests
  - sin contaminar UI productiva

## Acceptance Criteria

1. Re-render de `AppEditorPdf` con props equivalentes no dispara recomputos costosos (segun tests/bench).
2. `onMetricsChange` no se llama repetidamente cuando metricas no cambian.
3. Guias/overlays no recalculan estilos si `zoomLevel` y config no cambian.

## Tests

- Unit: ensures memoized outputs stay referentially stable when inputs unchanged.
- Integration: simulate rerenders with same props and assert callbacks not spammed.


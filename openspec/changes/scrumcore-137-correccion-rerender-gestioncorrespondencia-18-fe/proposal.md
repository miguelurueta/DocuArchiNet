## Why

El profiling de `SCRUMCORE-136` confirmo que `AppTableExport` seguia rerenderizando durante typing y clear en `GestionCorrespondencia`, incluso despues de estabilizar `dataSource` y otros callbacks. La evidencia apunto a props frecuentes todavia recreadas en el page component, especialmente arrays inline y la region `paginationActions`.

El ticket `SCRUMCORE-137` busca aislar esa frontera de render con el menor cambio posible para reducir recomposicion innecesaria del export sin alterar su comportamiento.

## What Changes

- Estabilizar `formats` y `enabledModes` de `AppTableExport`.
- Memoizar `paginationActions` para evitar recrear la region de exportacion en rerenders equivalentes.
- Confirmar con profiling automatizado que `AppTableExport` deja de rerenderizar durante typing y clear.
- Mantener intacto el contrato funcional de exportacion actual.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `gestion-correspondencia-export-boundary`: la frontera de render de exportacion recibe props mas estables desde `GestionCorrespondencia`.

## Impact

- Reduce rerenders innecesarios de `AppTableExport` durante typing y clear.
- No cambia modos visibles ni comportamiento de export backend/local.
- Usa un aislamiento de bajo riesgo sin introducir abstracciones innecesarias.

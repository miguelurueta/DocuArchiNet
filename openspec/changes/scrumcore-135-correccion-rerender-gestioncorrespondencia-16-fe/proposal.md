## Why

`GestionCorrespondencia` recrea objetos y callbacks derivados en cada render, especialmente el `dataSource` que consume `AppTableExport`. Eso invalida memoizaciones internas del export y recomputa props de bajo riesgo aunque no hayan cambiado datos reales.

El ticket `SCRUMCORE-135` busca estabilizar esas props derivadas en el page component sin cambiar la UX de busqueda, exportacion ni navegacion.

## What Changes

- Memoizar `dataSource` de `AppTableExport` y las funciones que lo alimentan.
- Estabilizar callbacks de navegacion y acciones de tabla con dependencias correctas.
- Reutilizar props derivadas de bajo riesgo como `responsivePresentation`.
- Agregar cobertura automatizada enfocada en estabilidad referencial del page component.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `gestion-correspondencia-page`: el page component expone props derivadas mas estables hacia `AppTableExport` y `AppTable`.

## Impact

- Reduce invalidaciones referenciales de `AppTableExport`.
- Mantiene intacta la UX actual de busqueda, refresh, exportacion y navegacion.
- Requiere validacion focal del page component y sus interacciones.

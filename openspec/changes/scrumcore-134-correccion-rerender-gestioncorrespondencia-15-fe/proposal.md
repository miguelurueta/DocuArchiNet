## Why

`useGestionCorrespondenciaTable` recrea `getAllMatchingRows` y `getBackendExportFile` en cada render. Eso impide estabilizar el `dataSource` que consume `AppTableExport` en `GestionCorrespondencia` y contribuye a rerenders evitables del flujo de exportacion.

El ticket `SCRUMCORE-134` busca estabilizar esos handlers sin perder acceso al estado efectivo mas reciente de filtros, busqueda, orden y total de filas.

## What Changes

- Estabilizar `getAllMatchingRows` y `getBackendExportFile` en `useGestionCorrespondenciaTable`.
- Usar referencias al estado efectivo mas reciente para evitar closures stale en exportacion y carga `allMatching`.
- Agregar cobertura automatizada que verifique estabilidad referencial y uso correcto del estado actualizado.
- Mantener intacto el contrato publico de `GestionCorrespondenciaTableResult`.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `gestion-correspondencia-table`: el hook expone handlers estables para export backend y carga de filas `allMatching`.

## Impact

- Reduce invalidaciones de props en `AppTableExport`.
- Preserva exportacion backend y calculo de `allMatching` con el estado mas reciente.
- Requiere validacion focal sobre `useGestionCorrespondenciaTable`.

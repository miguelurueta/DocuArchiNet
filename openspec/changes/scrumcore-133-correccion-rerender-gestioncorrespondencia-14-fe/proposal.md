## Why

`useDynamicUiTableQuery` expone `refetch` como un wrapper inline nuevo en cada render. Aunque conceptualmente representa la misma accion, esa inestabilidad referencial se propaga a consumidores como `useGestionCorrespondenciaTable` y a botones que reciben `refetch` como prop.

El ticket `SCRUMCORE-133` busca eliminar esa fuente innecesaria de invalidacion de props sin alterar la integracion con React Query ni el comportamiento observable de la recarga.

## What Changes

- Estabilizar el wrapper publico de `refetch` en `useDynamicUiTableQuery`.
- Mantener intacta la semantica actual de recarga delegando en `query.refetch()` de React Query.
- Agregar cobertura automatizada que valide estabilidad referencial y ejecucion correcta de la recarga.
- Verificar que consumidores como `useGestionCorrespondenciaTable` sigan funcionando sin regresion.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `dynamic-ui-table-query`: el hook expone un `refetch` estable para consumidores que dependan de igualdad referencial.

## Impact

- Reduce invalidaciones de props evitables en acciones UI que reciben `refetch`.
- No modifica `queryKey`, cache, retry ni cancelacion de React Query.
- Requiere validacion focal sobre `useDynamicUiTableQuery` y su consumidor principal en gestion correspondencia.

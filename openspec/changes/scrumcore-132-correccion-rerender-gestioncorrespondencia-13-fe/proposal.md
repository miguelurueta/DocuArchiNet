## Why

`GestionCorrespondencia` consume `useAppTableQueryState` para propagar cambios de busqueda, paginacion y ordenamiento. Hoy el hook devuelve `onQueryChange` como una funcion nueva en cada render, lo que rompe estabilidad referencial y dificulta memoizacion segura en consumidores y wrappers.

El ticket `SCRUMCORE-132` busca corregir esa fuente de rerenders evitables sin cambiar el contrato publico ni la semantica de actualizacion del estado de consulta.

## What Changes

- Estabilizar `onQueryChange` dentro de `useAppTableQueryState` usando una referencia memoizada segura.
- Mantener intacta la logica actual de merge del patch y reseteo de pagina definida por `updateAppTableQueryState`.
- Agregar cobertura automatizada que valide la estabilidad referencial del handler a traves de rerenders y updates de estado.
- Verificar que `useGestionCorrespondenciaTable` siga funcionando sin regresion en el flujo de query activo.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `app-table-query-state`: el hook de estado de consulta expone un `onQueryChange` estable para consumidores que dependan de igualdad referencial.

## Impact

- Reduce una fuente de rerenders evitables en consumidores como `GestionCorrespondencia`.
- No cambia el contrato publico de `useAppTableQueryState`.
- Requiere validacion focal de pruebas sobre `useAppTableQueryState` y `useGestionCorrespondenciaTable`.

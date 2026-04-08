## Why

INTEGRACION-APPINPUTSEARCH-APPTOOLBAR. PROMPT ARQUITECTONICO Ticket 02 FE

## What Changes

- Se consolida la integracion de `AppInputSearch` dentro de `AppToolbar.actionContent` en `GestionCorrespondencia`.
- Se valida que el buscador use `table.queryState.search` y `table.onQueryChange({ search })` como unico flujo de estado.
- Se mantiene `AppTableQueryWrapper` con `showSearch={false}` para evitar buscadores duplicados.
- Se preservan acciones existentes del toolbar, exportacion y paginacion.

## Capabilities

### New Capabilities
-

### Modified Capabilities
- `gestion-correspondencia`

## Impact

- Delta spec sobre `openspec/specs/gestion-correspondencia/spec.md`.
- Validacion focal de `GestionCorrespondencia` para garantizar un unico buscador visible en toolbar.
- Sin cambios esperados en contratos de `AppInputSearch`, `AppTable`, exportacion ni backend.

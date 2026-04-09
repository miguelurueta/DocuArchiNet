## Why

REFINAR-FOCO-VISUAL-APPTABLE. PROMPT ARQUITECTONICO Ticket 26 FE

## What Changes

- Se refinara la representacion visual del foco de celda en `AppTable` cuando `rowClickAffordance` este activo.
- Se mantendra el foco funcional requerido por AG Grid para soportar teclado y `Enter`.
- Se desacoplara visualmente el foco de celda de la semantica de seleccion de fila.
- Se preservaran sin cambios el contrato reusable del componente, la exclusion de columnas especiales y la accesibilidad de controles internos.

## Capabilities

### Modified Capabilities
- `crea-componente-table`: Refinar foco visual de celdas navegables en `AppTable` sin alterar el comportamiento funcional del grid.

## Impact

- Cambios en el shared component `AppTable` y sus estilos scoped.
- Actualizacion de specs y pruebas de `AppTable`.
- Sin cambios de contrato para modulos consumidores.

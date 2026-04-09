## Why

Adoptar en `GestionCorrespondencia` la affordance navegable reusable ya implementada en `AppTable`, eliminando la solucion CSS local del modulo y consolidando el patron shared para cursor, hover y teclado sin alterar la navegacion del dominio.

## What Changes

- Se reemplaza en `GestionCorrespondencia` el uso de `gridClassName={styles.navigableGrid}` por la prop reusable de `AppTable`.
- Se elimina el CSS local equivalente a cursor y affordance navegable.
- Se mantiene intacta la logica de navegacion actual del modulo.
- Se validan acciones, seleccion y teclado sobre la implementacion adoptada.

## Capabilities

### New Capabilities
- None.

### Modified Capabilities
- `gestion-correspondencia`: Adopta la affordance navegable reusable de `AppTable` y elimina la solucion local duplicada.

## Impact

- Cambios en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`.
- Cambios en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Ajustes de pruebas del modulo.

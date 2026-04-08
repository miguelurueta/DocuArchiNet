## Why

Implementar el autocomplete frontend para Workflow Inbox en Gestion Correspondencia, manteniendo `AppInputSearch` como componente presentacional y ubicando la conexion a backend en un hook y servicio desacoplados del componente UI.

## What Changes

- Crear un hook `useWorkflowInboxAutocomplete` en el modulo `gestionCorrespondencia`.
- Crear un servicio `workflowInboxAutocomplete.service.ts` para consultar sugerencias del backend.
- Integrar las sugerencias en el `AppInputSearch` de `GestionCorrespondencia` mediante `options` y `loading`.
- Separar el flujo de sugerencias del flujo de busqueda real de tabla.
- Mantener Enter, click en icono y seleccion de sugerencia como eventos que aplican `table.onQueryChange({ search })`.
- Evitar doble debounce entre `AppInputSearch` y el hook de autocomplete.

## Capabilities

### New Capabilities


### Modified Capabilities
- `gestion-correspondencia`: Agrega autocomplete frontend desacoplado para el buscador workflow existente en toolbar.

## Impact

- Modifica `src/modules/gestionCorrespondencia` con hook, servicio, tipos y pruebas.
- Integra `AppInputSearch` con sugerencias sin mover logica de endpoints a la pantalla.
- Mantiene `AppTableQueryWrapper showSearch={false}` y no cambia contratos de `AppTable`.

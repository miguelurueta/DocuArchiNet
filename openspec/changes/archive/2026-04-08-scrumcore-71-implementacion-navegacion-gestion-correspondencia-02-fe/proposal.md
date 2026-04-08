## Why

IMPLEMENTACION-NAVEGACION-GESTION-CORRESPONDENCIA-02-FE. PROMPT ARQUITECTÓNICO — Ajustar GestionRespuesta para cierre y retorno tipo Gmail

## What Changes

- Refinar la experiencia de `GestionRespuesta` dentro del shell persistente de `GestionCorrespondencia`.
- Hacer mas claro el flujo de cierre y retorno al listado sin volver al patron `Drawer`.
- Preservar la bandeja principal visible y la URL como fuente de verdad de la navegacion.
- Ajustar pruebas y documentacion del modulo para el nuevo comportamiento observable.

## Capabilities

### Modified Capabilities
- `gestion-correspondencia`: Refina el detalle secundario del shell persistente para que `GestionRespuesta` tenga un flujo de cierre y retorno mas claro, sin acoplarse al router ni reemplazar la bandeja principal.

## Impact

- Cambios en `src/modules/gestionCorrespondencia/pages`, `routes`, `style` y pruebas del modulo.
- Actualizacion de la spec principal `gestion-correspondencia`.
- Sin impacto esperado en `AppTable`, `AppToolbar`, `AppTableQueryWrapper` ni en el flujo de datos de la bandeja.

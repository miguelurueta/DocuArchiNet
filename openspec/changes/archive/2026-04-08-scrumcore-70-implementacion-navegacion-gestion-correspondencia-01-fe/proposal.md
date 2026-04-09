## Why

Implementar un shell de navegación tipo Gmail en `GestionCorrespondencia`, evolucionando el patrón actual basado en `Drawer` hacia una navegación persistente del módulo gobernada por routing.

## What Changes

- Reemplazar la experiencia secundaria modal del módulo por una región persistente de navegación/detalle.
- Mantener la URL como fuente de verdad para apertura, cierre y deep-linking.
- Preservar el listado principal de `GestionCorrespondencia` como región estable del shell.
- Ajustar pruebas y documentación del módulo al nuevo patrón de navegación.

## Capabilities

### New Capabilities


### Modified Capabilities
- `gestion-correspondencia`: Evoluciona la navegación secundaria desde `Drawer` contextual a shell persistente tipo Gmail.

## Impact

- Cambios en `src/modules/gestionCorrespondencia/routes`, `pages`, `style` y pruebas de routing.
- Actualización de la spec principal del módulo para reflejar el nuevo patrón de navegación.

## Why

IMPLEMENTACION-TOOLTIP-AFFORDANCE-APPTABLE. PROMPT ARQUITECTONICO Ticket 27 FE

## What Changes

- Se agregara un contrato reusable y opt-in en `AppTable` para mostrar una pista textual contextual sobre superficies navegables.
- La pista textual convivira con `rowClickAffordance` sin alterar la navegacion real ni el contrato de eventos.
- La implementacion cubrira grid y cards, excluyendo acciones, seleccion y controles interactivos internos.
- La estrategia de implementacion evitara un wrapper costoso por cada celda navegable en AG Grid.

## Capabilities

### Modified Capabilities
- `crea-componente-table`: Extender `AppTable` con tooltip opt-in para affordance navegable.

## Impact

- Cambios en `AppTable`, sus renderers y pruebas shared.
- Sin acoplamiento a modulos consumidores.
- Refinamiento del contrato reusable y de la experiencia UX en grid y cards.

## Context

`AppTable` ya soporta:

- renderer tabular con AG Grid
- renderer cards
- layout `content` y `fill`

Pero la tipografía visual de filas y headers no está suficientemente estandarizada. La experiencia actual se percibe menos refinada para escenarios tipo inbox.

## Decision

Estandarizar la tipografía de `AppTable` como una preocupación shared del componente, no como overrides por pantalla.

La implementación debe cubrir:

1. Tipografía de filas del grid
2. Tipografía de headers del grid
3. Tipografía de cards

## Visual direction

Referencia visual aproximada:

- filas: `13px` o `14px`, peso `400`, line-height compacta
- headers: mismo family, peso moderado, jerarquía limpia
- cards: consistente con las filas del grid

No se busca clonar Gmail literalmente; se busca un look más limpio, compacto y estable.

## Implementation notes

- La base compartida debe vivir en `AppTable.module.css` y/o configuración visual de AG Grid
- El renderer cards debe alinearse al mismo sistema tipográfico
- El cambio no debe depender de `GestionCorrespondencia`

## Compatibility

- No romper alturas de fila
- No romper cards
- No introducir una fuente distinta al sistema actual si ya existe una guía shared

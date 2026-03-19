## Why

Los campos de tipo seleccion ya se renderizan, pero actualmente no aseguran poblarse con las opciones declaradas en `ilist_row_drowlist`. Esto causa selects vacios o incompletos en radicacion, por lo que se requiere completar el llenado de opciones para cumplir el comportamiento esperado.

## What Changes

- Poblar dinamicamente los `<select>` de campos `SELECCION` con `ilist_row_drowlist`, incluyendo la opcion inicial "Seleccionar".
- Mantener atributos y metadatos existentes (required, disabled, title, tooltipAyuda) sin alterar la estructura actual.

## Capabilities

### New Capabilities

- `campos-plantilla-select-ilist`: Poblado de opciones de seleccion usando `ilist_row_drowlist` con opcion inicial estandar.

### Modified Capabilities

- `campos-dinamicos-plantilla`: Se amplian los requisitos para que los campos `SELECCION` siempre incluyan opciones desde `ilist_row_drowlist`.

## Impact

- Renderizado de campos dinamicos en radicacion.
- Tests de UI para campos `SELECCION`.

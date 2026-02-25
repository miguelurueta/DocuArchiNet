## Why

Los labels de los campos `AUTOCOMPLETE` pueden mostrarse con capitalizacion inconsistente, afectando la uniformidad visual. Se requiere asegurar el efecto de letra capital solo para estos campos.

## What Changes

- Aplicar capitalizacion a labels de campos `AUTOCOMPLETE` con `campo_tip = 1`.
- Mantener atributos existentes (data-ident, required, disabled, title, tooltipAyuda) sin alterar la estructura actual.

## Capabilities

### New Capabilities

- `labels-capitalize-autocomplete`: Capitalizacion de labels en campos dinamicos `AUTOCOMPLETE`.

### Modified Capabilities

- `campos-dinamicos-plantilla`: Se amplian los requisitos para que los labels de campos `AUTOCOMPLETE` se muestren con letra capital.

## Impact

- UI de radicacion (labels de campos dinamicos `AUTOCOMPLETE`).
- Estilos compartidos del formulario.
- Tests de UI para labels.

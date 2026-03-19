## Why

Los labels de los campos dinamicos pueden mostrarse con capitalizacion inconsistente, afectando la uniformidad visual. Se requiere aplicar el efecto de letra capital para los labels de campos `SELECCION` y `AUTOCOMPLETE` y mantener la experiencia coherente.

## What Changes

- Aplicar estilo de letra capital a los labels de campos `SELECCION` y `AUTOCOMPLETE` cuando `campo_tip = 1`.
- Mantener atributos existentes (data-ident, required, disabled, title, tooltipAyuda) y la estructura actual de render.

## Capabilities

### New Capabilities

- `labels-capitalize-campos-plantilla`: Capitalizacion de labels en campos dinamicos `SELECCION` y `AUTOCOMPLETE`.

### Modified Capabilities

- `campos-dinamicos-plantilla`: Se amplian los requisitos para que los labels de campos `SELECCION` y `AUTOCOMPLETE` se muestren con letra capital.

## Impact

- UI de radicacion (labels de campos dinamicos).
- Estilos compartidos del formulario.
- Tests de UI para labels.

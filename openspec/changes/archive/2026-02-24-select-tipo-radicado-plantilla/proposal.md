## Why

El select de TipoRadicado existe en la UI, pero no se alimenta desde `camposPlantilla`, lo que impide configurar sus opciones desde la plantilla. Se requiere poblarlo con `ilist_row_drowlist` cuando `name_campo = "TipoRadicado"`.

## What Changes

- Buscar en `camposPlantilla` el registro con `name_campo = "TipoRadicado"`.
- Poblar el `<select data-ident="pl-radicacion-spe-TipoRadicado">` con `ilist_row_drowlist` e incluir la opcion "Seleccionar".
- Mantener atributos existentes (required, title, tooltipAyuda) sin cambiar estructura.

## Capabilities

### New Capabilities

- `select-tipo-radicado-plantilla`: Poblado dinamico de opciones para el select TipoRadicado basado en `camposPlantilla`.

### Modified Capabilities

- `campos-dinamicos-plantilla`: Se amplian los requisitos para poblar el select TipoRadicado desde la plantilla.

## Impact

- UI de radicacion (select TipoRadicado).
- Tests de UI para la plantilla y el select.

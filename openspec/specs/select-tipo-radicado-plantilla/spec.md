## ADDED Requirements

### Requirement: Select TipoRadicado poblado desde plantilla
El sistema SHALL localizar en `camposPlantilla` el campo con `name_campo = "TipoRadicado"` y poblar el `<select data-ident="pl-radicacion-spe-TipoRadicado">` usando `ilist_row_drowlist`, incluyendo siempre la opcion inicial "Seleccionar".

#### Scenario: Select TipoRadicado con opciones de plantilla
- **WHEN** existe un campo `TipoRadicado` en `camposPlantilla`
- **THEN** el `<select>` incluye la opcion inicial "Seleccionar" y las opciones de `ilist_row_drowlist` con `value={idValue}` y texto `{Value}`

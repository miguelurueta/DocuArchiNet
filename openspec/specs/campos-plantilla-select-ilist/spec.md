## ADDED Requirements

### Requirement: Poblado de opciones en campos seleccion
El sistema SHALL poblar los `<select>` de campos `SELECCION` con las opciones de `ilist_row_drowlist`, incluyendo siempre la opcion inicial "Seleccionar".

#### Scenario: Select incluye opcion inicial y opciones de lista
- **WHEN** un campo `SELECCION` tiene `ilist_row_drowlist` disponible
- **THEN** el `<select>` renderiza primero la opcion con `value=null` y texto "Seleccionar", seguida por cada opcion de `ilist_row_drowlist`

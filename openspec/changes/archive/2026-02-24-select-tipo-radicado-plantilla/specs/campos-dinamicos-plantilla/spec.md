## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL renderizar controles específicos según `ComportamientoCampo` y propagar atributos de validación, accesibilidad y metadatos. En campos `SELECCION`, el `<select>` SHALL poblarse con `ilist_row_drowlist`, incluyendo siempre la opcion inicial "Seleccionar". Para `TipoRadicado`, el `<select data-ident="pl-radicacion-spe-TipoRadicado">` SHALL usar el registro de `camposPlantilla` con `name_campo = "TipoRadicado"` y poblarse con `ilist_row_drowlist`.

#### Scenario: Campo de tipo selección
- **WHEN** un campo tiene `ComportamientoCampo = "SELECCION"`
- **THEN** el componente renderiza un `<select>` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`, e incluye la opcion inicial "Seleccionar" junto con las opciones de `ilist_row_drowlist`

#### Scenario: Campo de tipo autocompletar
- **WHEN** un campo tiene `ComportamientoCampo = "AUTOCOMPLETE"`
- **THEN** el componente renderiza un `<input type="text">` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`

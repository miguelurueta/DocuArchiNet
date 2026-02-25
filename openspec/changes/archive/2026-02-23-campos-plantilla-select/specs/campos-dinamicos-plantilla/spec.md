## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL renderizar controles específicos según `ComportamientoCampo` y propagar atributos de validación, accesibilidad y metadatos, manteniendo la misma estructura y estilos usados por los campos `AUTOCOMPLETE`.

#### Scenario: Campo de tipo selección
- **WHEN** un campo tiene `ComportamientoCampo = "SELECCION"`
- **THEN** el componente renderiza un `<select>` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, `data-api-method` según `apiMethod`, `data-group` cuando aplique, y con la misma estructura y clases visuales que los campos `AUTOCOMPLETE`

#### Scenario: Campo de tipo autocompletar
- **WHEN** un campo tiene `ComportamientoCampo = "AUTOCOMPLETE"`
- **THEN** el componente renderiza un `<input type="text">` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`

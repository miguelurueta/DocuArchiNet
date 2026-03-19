## ADDED Requirements

### Requirement: Renderizado dinámico de campos tipo plantilla
El sistema SHALL renderizar dinámicamente los campos de `camposPlantilla` cuyo `campo_tip = 1` dentro de un contenedor `<Card data-ident="pl-radicacion-card-spe">`, aplicando los atributos de configuración definidos por la plantilla.

#### Scenario: Renderizado de campos filtrados por tipo
- **WHEN** `camposPlantilla` contiene campos con `campo_tip = 1`
- **THEN** el componente renderiza únicamente esos campos dentro del `Card` con `data-ident="pl-radicacion-card-spe"`

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL renderizar controles específicos según `ComportamientoCampo` y propagar atributos de validación, accesibilidad y metadatos.

#### Scenario: Campo de tipo selección
- **WHEN** un campo tiene `ComportamientoCampo = "SELECCION"`
- **THEN** el componente renderiza un `<select>` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`

#### Scenario: Campo de tipo autocompletar
- **WHEN** un campo tiene `ComportamientoCampo = "AUTOCOMPLETE"`
- **THEN** el componente renderiza un `<input type="text">` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`

### Requirement: Reglas de validación y accesibilidad
El sistema SHALL aplicar validaciones específicas (tipo/pattern) y atributos de accesibilidad apropiados, además de exponer eventos `onChange`, `onBlur` y `onFocus`.

#### Scenario: Campo de correo
- **WHEN** un campo tiene `control_tip_correo = 1`
- **THEN** el control se renderiza con `type="email"` y validación de correo (pattern o validación nativa)

#### Scenario: Accesibilidad y metadata
- **WHEN** se renderiza un campo dinámico
- **THEN** el control incluye `aria-label` o `aria-describedby`, un `label` con `aleas_campo`, `title` con `title_control`, y un `span.tooltip-ayuda` con `tooltipAyuda` (preparado para i18n)

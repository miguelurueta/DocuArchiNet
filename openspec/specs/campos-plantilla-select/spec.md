## ADDED Requirements

### Requirement: Renderizado de campos seleccion en plantilla
El sistema SHALL renderizar los campos con `ComportamientoCampo = "SELECCION"` y `campo_tip = 1` dentro del `<Card data-ident="pl-radicacion-card-spe">`, usando la misma estructura y estilos que los campos `AUTOCOMPLETE`.

#### Scenario: Campo seleccion usa estructura y estilos consistentes
- **WHEN** un campo tiene `ComportamientoCampo = "SELECCION"` y `campo_tip = 1`
- **THEN** el componente renderiza el control dentro del `Card` con la misma jerarquia de contenedor y clases CSS usadas por los campos `AUTOCOMPLETE`

### Requirement: Atributos declarativos y accesibilidad en campos seleccion
El sistema SHALL propagar atributos declarativos del campo seleccion y exponer eventos para integracion de logica adicional, incluyendo accesibilidad e i18n.

#### Scenario: Atributos y metadata en select
- **WHEN** se renderiza un campo `SELECCION`
- **THEN** el `<select>` incluye `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` segun `max_leng_campo`, `required` segun `obligatorio_campo`, `disabled` segun `disable_campo`, `data-api-method` segun `apiMethod`, `data-group` cuando aplique y atributos `aria-label` o `aria-describedby`

#### Scenario: Label, tooltip y eventos
- **WHEN** se renderiza un campo `SELECCION`
- **THEN** el `label` usa `aleas_campo`, el tooltip usa `title_control`, se muestra `span.tooltip-ayuda` con `tooltipAyuda` (preparado para i18n), y el control expone `onChange`, `onBlur` y `onFocus`

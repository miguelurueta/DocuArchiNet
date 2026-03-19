## ADDED Requirements

### Requirement: Render de campo FECHALIMITERESPUESTA con metadatos de plantilla
El sistema SHALL localizar en `camposPlantilla` el registro cuyo `name_campo = "FECHALIMITERESPUESTA"` y usar sus metadatos para renderizar el label del campo de fecha en radicación. El label SHALL usar `title_control` como atributo `title` y, si `tooltipAyuda` tiene valor, SHALL renderizar un icono con clase `tooltip-ayuda` junto al label.

#### Scenario: Label con title y tooltip para FECHALIMITERESPUESTA
- **WHEN** `camposPlantilla` contiene un campo con `name_campo = "FECHALIMITERESPUESTA"` y valores en `title_control` o `tooltipAyuda`
- **THEN** el label de "Fecha Límite Respuesta" usa `title_control` como `title` y renderiza un icono con clase `tooltip-ayuda` si `tooltipAyuda` no esta vacio

### Requirement: Conservacion de comportamiento del control de fecha
El sistema SHALL conservar el control existente de fecha para `FECHALIMITERESPUESTA` y mantener sus atributos funcionales y de accesibilidad declarativa cuando exista tooltip.

#### Scenario: El DatePicker mantiene comportamiento y accesibilidad
- **WHEN** se renderiza el campo de fecha con metadatos de plantilla
- **THEN** el formulario mantiene el `DatePicker` actual, conserva sus atributos declarativos existentes y expone `aria-describedby` asociado al tooltip cuando aplica

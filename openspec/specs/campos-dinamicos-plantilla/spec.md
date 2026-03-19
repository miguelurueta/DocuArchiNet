## ADDED Requirements

### Requirement: Renderizado dinámico de campos tipo plantilla
El sistema SHALL renderizar dinámicamente los campos de `camposPlantilla` cuyo `campo_tip = 1` dentro de un contenedor `<Card data-ident="pl-radicacion-card-spe">`, aplicando los atributos de configuración definidos por la plantilla.

#### Scenario: Renderizado de campos filtrados por tipo
- **WHEN** `camposPlantilla` contiene campos con `campo_tip = 1`
- **THEN** el componente renderiza únicamente esos campos dentro del `Card` con `data-ident="pl-radicacion-card-spe"`

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL renderizar controles específicos según `ComportamientoCampo` y propagar atributos de validación, accesibilidad y metadatos. En campos `SELECCION`, el `<select>` SHALL poblarse con `ilist_row_drowlist`, incluyendo siempre la opcion inicial "Seleccionar". Para `TipoRadicado`, el `<select data-ident="pl-radicacion-spe-TipoRadicado">` SHALL usar el registro de `camposPlantilla` con `name_campo = "TipoRadicado"` y poblarse con `ilist_row_drowlist`.

#### Scenario: Campo de tipo selección
- **WHEN** un campo tiene `ComportamientoCampo = "SELECCION"`
- **THEN** el componente renderiza un `<select>` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`, e incluye la opcion inicial "Seleccionar" junto con las opciones de `ilist_row_drowlist`

#### Scenario: Campo de tipo autocompletar
- **WHEN** un campo tiene `ComportamientoCampo = "AUTOCOMPLETE"`
- **THEN** el componente renderiza un `<input type="text">` con `data-ident="pl-radicacion-spe-{name_campo}"`, `maxLength` según `max_leng_campo`, `required` según `obligatorio_campo`, `disabled` según `disable_campo`, y `data-api-method` según `apiMethod`

### Requirement: Reglas de validación y accesibilidad
El sistema SHALL aplicar validaciones específicas (tipo/pattern) y atributos de accesibilidad apropiados, además de exponer eventos `onChange`, `onBlur` y `onFocus`. Los labels de campos `AUTOCOMPLETE` con `campo_tip = 1` SHALL mostrarse con efecto de letra capital.

#### Scenario: Campo de correo
- **WHEN** un campo tiene `control_tip_correo = 1`
- **THEN** el control se renderiza con `type="email"` y validación de correo (pattern o validación nativa)

#### Scenario: Accesibilidad y metadata
- **WHEN** se renderiza un campo dinámico
- **THEN** el control incluye `aria-label` o `aria-describedby`, un `label` con `aleas_campo` capitalizado, `title` con `title_control`, y un `span.tooltip-ayuda` con `tooltipAyuda` (preparado para i18n)

### Requirement: Render de campo Descripcion_Documento con metadatos de plantilla
El sistema SHALL localizar en `camposPlantilla` el registro cuyo `name_campo = "Descripcion_Documento"` y usar sus metadatos para renderizar el label y el control existente. El label SHALL usar `title_control` como atributo `title` y, si `tooltipAyuda` tiene valor, SHALL renderizar un icono con clase `tooltip-ayuda` junto al label. El control SHALL conservar `required` y `disabled` existentes.

#### Scenario: Label con title y tooltip para Descripcion_Documento
- **WHEN** `camposPlantilla` contiene un campo con `name_campo = "Descripcion_Documento"` y valores en `title_control` o `tooltipAyuda`
- **THEN** el label del campo usa `title_control` como `title` y renderiza un icono con clase `tooltip-ayuda` si `tooltipAyuda` no está vacío

#### Scenario: Conserva required y disabled
- **WHEN** el campo `Descripcion_Documento` está marcado como obligatorio o deshabilitado en la plantilla
- **THEN** el control respeta `required` y `disabled` existentes

### Requirement: Render de campo RE_flujo_trabajo con metadatos de plantilla
El sistema SHALL localizar en `camposPlantilla` el registro cuyo `name_campo = "RE_flujo_trabajo"` y usar sus metadatos para renderizar el label y el control existente. El label SHALL usar `title_control` como atributo `title` y, si `tooltipAyuda` tiene valor, SHALL renderizar un icono con clase `tooltip-ayuda` junto al label. El control SHALL conservar `required` y `disabled` existentes.

#### Scenario: Label con title y tooltip para RE_flujo_trabajo
- **WHEN** `camposPlantilla` contiene un campo con `name_campo = "RE_flujo_trabajo"` y valores en `title_control` o `tooltipAyuda`
- **THEN** el label del campo usa `title_control` como `title` y renderiza un icono con clase `tooltip-ayuda` si `tooltipAyuda` no está vacío

#### Scenario: Conserva required y disabled
- **WHEN** el campo `RE_flujo_trabajo` está marcado como obligatorio o deshabilitado en la plantilla
- **THEN** el control respeta `required` y `disabled` existentes

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

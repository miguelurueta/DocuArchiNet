## MODIFIED Requirements

### Requirement: Render de campo RE_flujo_trabajo con metadatos de plantilla
El sistema SHALL localizar en `camposPlantilla` el registro cuyo `name_campo = "RE_flujo_trabajo"` y usar sus metadatos para renderizar el label y el control existente. El label SHALL usar `title_control` como atributo `title` y, si `tooltipAyuda` tiene valor, SHALL renderizar un icono con clase `tooltip-ayuda` junto al label. El control SHALL conservar `required` y `disabled` existentes.

#### Scenario: Label con title y tooltip para RE_flujo_trabajo
- **WHEN** `camposPlantilla` contiene un campo con `name_campo = "RE_flujo_trabajo"` y valores en `title_control` o `tooltipAyuda`
- **THEN** el label del campo usa `title_control` como `title` y renderiza un icono con clase `tooltip-ayuda` si `tooltipAyuda` no está vacío

#### Scenario: Conserva required y disabled
- **WHEN** el campo `RE_flujo_trabajo` está marcado como obligatorio o deshabilitado en la plantilla
- **THEN** el control respeta `required` y `disabled` existentes

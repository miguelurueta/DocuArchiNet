## MODIFIED Requirements

### Requirement: Reglas de validación y accesibilidad
El sistema SHALL aplicar validaciones específicas (tipo/pattern) y atributos de accesibilidad apropiados, además de exponer eventos `onChange`, `onBlur` y `onFocus`. Los labels de campos `AUTOCOMPLETE` con `campo_tip = 1` SHALL mostrarse con efecto de letra capital.

#### Scenario: Accesibilidad y metadata
- **WHEN** se renderiza un campo dinámico
- **THEN** el control incluye `aria-label` o `aria-describedby`, un `label` con `aleas_campo` capitalizado, `title` con `title_control`, y un `span.tooltip-ayuda` con `tooltipAyuda` (preparado para i18n)

#### Scenario: Campo de correo
- **WHEN** un campo tiene `control_tip_correo = 1`
- **THEN** el control se renderiza con `type="email"` y validación de correo (pattern o validación nativa)

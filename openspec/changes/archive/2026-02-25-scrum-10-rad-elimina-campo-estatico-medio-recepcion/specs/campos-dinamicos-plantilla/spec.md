## MODIFIED Requirements

### Requirement: Renderizado dinámico de campos tipo plantilla
El sistema SHALL renderizar dinámicamente los campos de `camposPlantilla` cuyo `campo_tip = 1` dentro de un contenedor `<Card data-ident="pl-radicacion-card-spe">`, aplicando los atributos de configuración definidos por la plantilla. El formulario de radicación MUST evitar renderizar controles estáticos duplicados para los mismos datos cuando existan en metadata dinámica.

#### Scenario: Renderizado de campos filtrados por tipo
- **WHEN** `camposPlantilla` contiene campos con `campo_tip = 1`
- **THEN** el componente renderiza únicamente esos campos dentro del `Card` con `data-ident="pl-radicacion-card-spe"`

#### Scenario: Medio de recepción sin duplicidad estática
- **WHEN** la plantilla contiene el campo dinámico de medio de recepción (por ejemplo `MEDIORECEPCION`)
- **THEN** el formulario no renderiza el campo estático con `data-ident="pl-radicacion-spe-Medio-recep"` y conserva una sola fuente de captura para ese dato

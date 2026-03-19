## ADDED Requirements

### Requirement: Autocompletado dinámico de campos de plantilla
El sistema SHALL renderizar campos con `campo_tip = 1` y `ComportamientoCampo = "AUTOCOMPLETE"` dentro del `<Card data-ident="pl-radicacion-card-spe">`, usando un componente de autocompletado que consulte la API de plantillas.

#### Scenario: Renderiza solo campos de autocompletado
- **WHEN** `camposPlantilla` incluye campos con `campo_tip = 1` y `ComportamientoCampo = "AUTOCOMPLETE"`
- **THEN** el componente renderiza únicamente esos campos dentro del `Card` con `data-ident="pl-radicacion-card-spe"`

### Requirement: Consulta a API de autocompletado
El sistema SHALL consultar `/api/PlantillaRadicado/solicitaAutoCompleteCampos` enviando los parámetros requeridos por campo.

#### Scenario: Parámetros de consulta por campo
- **WHEN** el usuario escribe en un campo de autocompletado
- **THEN** el request incluye `{ TextoBuscado, defaultDbAlias, tbl_control, name_campo }` donde `tbl_control` proviene de `ComportamientoCampo` y `name_campo` corresponde a `name_campo` del campo actual

### Requirement: Estado de carga y manejo de errores
El sistema SHALL mostrar un estado de carga durante la consulta y mensajes de error amigables si la API falla, centralizando errores con Axios.

#### Scenario: Muestra loading
- **WHEN** la consulta de autocompletado está en progreso
- **THEN** el control muestra un indicador de carga

#### Scenario: Error de API
- **WHEN** la API responde con error o falla la red
- **THEN** se muestra un mensaje de error amigable y el error se reporta de forma centralizada

### Requirement: Accesibilidad y extensibilidad
El sistema SHALL incluir atributos de accesibilidad y permitir extensiones para nuevos endpoints o validaciones.

#### Scenario: Accesibilidad y metadata
- **WHEN** se renderiza un campo de autocompletado
- **THEN** el control incluye `aria-label`, `aria-describedby`, `data-ident`, `data-group` y soporte para `className` dinámico

## ADDED Requirements

### Requirement: Autocompletado dinámico de campos de plantilla
El sistema SHALL renderizar campos con `campo_tip = 1` y `ComportamientoCampo` configurado para autocompletado dentro del `<Card data-ident="pl-radicacion-card-spe">`, usando un componente reutilizable que consulta la API de plantillas. Cuando `name_campo = "ANEXOS_COR"`, el control SHALL renderizarse como autocompletado con `data-ident="pl-radicacion-spe-ANEXOS_COR"` y mantener `required`, `disabled`, `title` y `tooltipAyuda`.

#### Scenario: Renderiza campos de autocompletado incluyendo ANEXOS_COR
- **WHEN** `camposPlantilla` incluye un campo con `campo_tip = 1`, `ComportamientoCampo` configurado para autocompletado y `name_campo = "ANEXOS_COR"`
- **THEN** el componente renderiza un autocompletado con `data-ident="pl-radicacion-spe-ANEXOS_COR"` y respeta `required`, `disabled`, `title` y `tooltipAyuda`

### Requirement: Consulta a API de autocompletado
El sistema SHALL consultar `/api/PlantillaRadicado/solicitaAutoCompleteCampos` enviando `{ TextoBuscado, defaultDbAlias, tbl_control, name_campo }`, donde `tbl_control` proviene del campo `tbl_control` y `name_campo` del campo actual. Los valores `texValue` de la respuesta SHALL mostrarse como opciones en el autocompletado.

#### Scenario: Parámetros de consulta por campo
- **WHEN** el usuario escribe en el autocompletado de `ANEXOS_COR`
- **THEN** el request incluye `TextoBuscado` con el valor digitado, `defaultDbAlias` vacío, `tbl_control` derivado de `tbl_control` y `name_campo = "ANEXOS_COR"`

#### Scenario: Opciones mostradas desde la respuesta
- **WHEN** la API responde con `data` que incluye elementos con `texValue`
- **THEN** el autocompletado muestra cada `texValue` como opción seleccionable

### Requirement: Estado de carga y manejo de errores
El sistema SHALL mostrar un indicador de carga durante la consulta y manejar errores de forma centralizada vía Axios, mostrando un mensaje de error amigable.

#### Scenario: Muestra loading
- **WHEN** la consulta de autocompletado está en progreso
- **THEN** el control muestra un indicador de carga

#### Scenario: Error de API
- **WHEN** la API responde con error o falla la red
- **THEN** se muestra un mensaje de error amigable y el error se reporta de forma centralizada

### Requirement: Accesibilidad y extensibilidad
El sistema SHALL incluir atributos de accesibilidad y metadata para reutilizar el componente en otros campos.

#### Scenario: Accesibilidad y metadata
- **WHEN** se renderiza el autocompletado de `ANEXOS_COR`
- **THEN** el control incluye `aria-label`, `aria-describedby`, `data-ident`, `data-group` y soporte para `className` dinámico

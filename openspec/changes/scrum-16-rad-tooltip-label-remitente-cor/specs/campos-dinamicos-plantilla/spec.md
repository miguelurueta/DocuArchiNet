## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL resolver el campo de remitente desde `camposPlantilla` usando `name_campo = "REMITENTE_COR"` (comparacion normalizada). Con esa coincidencia, el control `data-ident="pl-radicacion-spe-REMITENTE_COR"` SHALL conservar su comportamiento tipo token y aplicar metadata declarativa de plantilla: `required`, `disabled`, `title_control` como atributo `title`, y `tooltipAyuda` en label mediante `span.tooltip-ayuda` con icono de informacion.

#### Scenario: Resuelve metadata de REMITENTE_COR por name_campo
- **WHEN** `camposPlantilla` contiene un elemento con `name_campo = "REMITENTE_COR"`
- **THEN** el control de remitente toma ese elemento como fuente de configuracion

#### Scenario: Aplica required y disabled desde metadata de plantilla en remitente
- **WHEN** el registro de `REMITENTE_COR` indica `obligatorio_campo` y `disable_campo`
- **THEN** el selector de remitente refleja esos valores en validacion y estado habilitado/deshabilitado

#### Scenario: Aplica title y tooltipAyuda en label de remitente
- **WHEN** el registro de `REMITENTE_COR` trae `title_control` y `tooltipAyuda`
- **THEN** el label del remitente incluye atributo `title` con `title_control` y muestra `span.tooltip-ayuda` con icono de ayuda usando `tooltipAyuda`

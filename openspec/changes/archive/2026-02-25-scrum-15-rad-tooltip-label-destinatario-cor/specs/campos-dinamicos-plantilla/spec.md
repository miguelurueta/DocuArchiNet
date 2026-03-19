## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL resolver el campo de destinatario desde `camposPlantilla` usando `name_campo = "Destinatario_Cor"` (comparación normalizada). Con esa coincidencia, el control `data-ident="pl-radicacion-spe-Destinatario_Cor"` SHALL conservar su comportamiento de selección y aplicar metadata declarativa de plantilla: `required`, `disabled`, `title_control` como atributo `title`, y `tooltipAyuda` en label mediante `span.tooltip-ayuda` con icono de información.

#### Scenario: Resuelve metadata de Destinatario_Cor por name_campo
- **WHEN** `camposPlantilla` contiene un elemento con `name_campo = "Destinatario_Cor"`
- **THEN** el control de destinatario toma ese elemento como fuente de configuración

#### Scenario: Aplica required y disabled desde metadata de plantilla
- **WHEN** el registro de `Destinatario_Cor` indica `obligatorio_campo` y `disable_campo`
- **THEN** el selector destinatario refleja esos valores en validación y estado habilitado/deshabilitado

#### Scenario: Aplica title y tooltipAyuda en label de destinatario
- **WHEN** el registro de `Destinatario_Cor` trae `title_control` y `tooltipAyuda`
- **THEN** el label del destinatario incluye atributo `title` con `title_control` y muestra `span.tooltip-ayuda` con icono de ayuda usando `tooltipAyuda`

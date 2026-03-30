# altura-dinamica-appcontent-gestion-correspondencia Specification

## Purpose
TBD - created by archiving change scrumcore-18-actualizacion-visual-ui-appcontent. Update Purpose after archive.
## Requirements
### Requirement: AppContent SHALL ocupar el espacio vertical restante debajo del toolbar
El sistema SHALL permitir que `AppContent` en `GestionCorrespondencia` ocupe exactamente el espacio restante del viewport despues de renderizar `AppToolbar`, sin calculos manuales de altura.

#### Scenario: Contenido pequeño no genera scroll extra
- **WHEN** la vista renderiza contenido corto que cabe dentro del espacio restante
- **THEN** `AppContent` SHALL ocupar el alto disponible sin generar scroll innecesario en la pagina completa

#### Scenario: Contenido largo usa el alto restante disponible
- **WHEN** la vista renderiza contenido largo debajo del toolbar
- **THEN** `AppContent` SHALL mantenerse dentro del espacio restante y no empujar el layout completo fuera del viewport

### Requirement: El scroll SHALL vivir dentro de AppContent y no en el body
El sistema SHALL limitar el scroll vertical al contenido interno de `AppContent`, manteniendo visible el toolbar y evitando scroll en el body o en el page wrapper.

#### Scenario: Scroll interno aparece cuando el contenido excede el espacio
- **WHEN** el contenido de `AppContent` supera el alto disponible
- **THEN** el usuario SHALL poder desplazarse dentro de `AppContent` mediante scroll interno vertical

#### Scenario: Toolbar permanece visible durante el scroll del contenido
- **WHEN** el usuario hace scroll sobre contenido largo de `AppContent`
- **THEN** `AppToolbar` SHALL permanecer visible y fuera de la region scrollable

### Requirement: El layout del modulo SHALL usar flexbox para resolver altura restante
El sistema SHALL estructurar `GestionCorrespondenciaLayout`, la pagina del modulo y `AppContent` como una cadena flex vertical compatible con `flex: 1` y `min-height: 0`.

#### Scenario: Layout permite expansion y contraccion del contenido
- **WHEN** la pagina se renderiza en desktop, tablet o mobile
- **THEN** el layout SHALL permitir que `AppContent` se expanda o contraiga segun el espacio restante sin romper la estructura vertical

### Requirement: El ajuste SHALL preservar funcionalidad existente del modulo
El sistema SHALL mantener sin cambios funcionales la navegacion, el drawer contextual y la composicion actual de acciones del toolbar.

#### Scenario: Ruta contextual sigue funcionando tras el ajuste
- **WHEN** el usuario navega a la subruta `respuesta`
- **THEN** la vista principal y el drawer contextual SHALL seguir funcionando como antes del cambio de layout


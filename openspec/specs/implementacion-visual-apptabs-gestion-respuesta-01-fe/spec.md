## Purpose

Definir el comportamiento visual y estructural de `GestionRespuesta` usando `AppTabs`,
manteniendo el boton "Volver a la bandeja" visible y preservando accesibilidad y responsive.

## Requirements

### Requirement: GestionRespuesta usa AppTabs como layout principal
El sistema SHALL renderizar `AppTabs` como contenedor principal en `GestionRespuesta.tsx`,
reemplazando el contenido actual por secciones basadas en tabs.

#### Scenario: Render de tabs en GestionRespuesta
- **WHEN** el usuario navega a la vista `GestionRespuesta`
- **THEN** la UI muestra `AppTabs` con las pestanas definidas por la vista

### Requirement: Boton Volver a la bandeja permanece visible
El sistema SHALL mantener el `AppButton` de "Volver a la bandeja" visible y fuera del contenido de tabs,
sin variar al cambiar de pestana.

#### Scenario: Boton visible al cambiar de tab
- **WHEN** el usuario cambia de pestana en `AppTabs`
- **THEN** el boton "Volver a la bandeja" sigue visible en la vista

### Requirement: Estructura de tabs respeta accesibilidad y responsive
El sistema SHALL preservar navegacion por teclado y layout responsive para `AppTabs` en `GestionRespuesta`.

#### Scenario: Navegacion accesible y responsive
- **WHEN** el usuario navega con teclado o en pantalla mobile
- **THEN** los tabs siguen accesibles y el layout no rompe el boton

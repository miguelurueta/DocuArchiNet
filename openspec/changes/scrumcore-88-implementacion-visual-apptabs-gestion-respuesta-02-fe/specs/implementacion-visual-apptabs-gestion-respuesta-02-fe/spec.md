## ADDED Requirements

### Requirement: Tabs con bordes superiores y laterales sin borde inferior
El sistema SHALL mostrar `AppTabs` con bordes visibles solo en la parte superior y en los laterales,
sin borde inferior, para evitar el corte visual con el contenido.

#### Scenario: Bordes visibles en tabs
- **WHEN** el usuario visualiza `GestionRespuesta`
- **THEN** cada tab muestra borde superior y laterales, sin borde inferior

### Requirement: Separacion minima entre tabs
El sistema SHALL aplicar separacion minima entre tabs para evitar que se vean pegados,
sin romper la alineacion del header.

#### Scenario: Separacion controlada
- **WHEN** el usuario observa la fila de tabs
- **THEN** la separacion entre tabs es minima y uniforme

### Requirement: Hover sutil sin colision visual
El sistema SHALL aplicar un hover sutil en tabs que no choque con el borde superior
ni incremente visualmente la separacion.

#### Scenario: Hover no invasivo
- **WHEN** el usuario pasa el cursor sobre un tab
- **THEN** el hover eleva levemente el tab sin generar colision con el borde superior

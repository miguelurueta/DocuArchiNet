## ADDED Requirements

### Requirement: Estructura modular para GestionCorrespondencia
El sistema SHALL organizar la UI de `GestionCorrespondencia` en componentes de modulo
reutilizables, manteniendo `GestionRespuesta.tsx` como orquestador de tabs.

#### Scenario: Composicion modular del layout
- **WHEN** el usuario navega a `GestionRespuesta`
- **THEN** la vista renderiza contenido del tab principal delegando en componentes dedicados

### Requirement: Estilos locales sin afectar globales
El sistema SHALL aplicar ajustes visuales mediante CSS Modules del modulo,
sin introducir estilos globales.

#### Scenario: Estilos locales aislados
- **WHEN** se aplican ajustes visuales en GestionCorrespondencia
- **THEN** los estilos no afectan otros modulos ni componentes shared

### Requirement: Tabs existentes se mantienen funcionales
El sistema SHALL preservar el comportamiento actual de tabs,
incluyendo navegacion y contenido del segundo tab.

#### Scenario: Segundo tab sin regresiones
- **WHEN** el usuario cambia al segundo tab
- **THEN** su contenido y comportamiento permanecen iguales

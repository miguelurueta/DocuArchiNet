## ADDED Requirements

### Requirement: Refinamiento visual consistente
El sistema SHALL alinear los estilos de `AppCollapseRail` con el panel de
herramientas de GestionRespuesta, manteniendo bordes, radios y sombras coherentes.

#### Scenario: Estilos consistentes
- **WHEN** el panel se renderiza en desktop o mobile
- **THEN** su apariencia coincide con los lineamientos visuales del workbench

### Requirement: Accesibilidad avanzada
El sistema SHALL mantener labels consistentes en toggles y rail, asegurando foco
visible y atributos ARIA completos.

#### Scenario: Toggle accesible
- **WHEN** el usuario navega con teclado o lector de pantalla
- **THEN** el toggle anuncia su estado y recibe foco visible

### Requirement: Responsive estable en tablet y mobile
El sistema SHALL garantizar comportamiento estable en tablet (colapsado por defecto)
y mobile (overlay), sin romper el layout principal.

#### Scenario: Breakpoints consistentes
- **WHEN** el viewport cambia entre tablet y mobile
- **THEN** el panel se adapta sin saltos de layout

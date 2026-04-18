## Purpose

Definir el comportamiento del componente reusable `AppCollapseRail` para paneles
laterales colapsables en la UI, con accesibilidad y soporte responsive.

## Requirements

### Requirement: Panel colapsable controlado externamente
El sistema SHALL exponer un componente `AppCollapseRail` con estado controlado por
propiedades `collapsed` y `onToggle`, sin manejar logica de negocio interna.

#### Scenario: Toggle controlado desde contenedor
- **WHEN** el contenedor invoca `onToggle` y actualiza `collapsed`
- **THEN** el panel refleja el nuevo estado sin manejar estado interno oculto

### Requirement: Rail visible cuando el panel esta colapsado
El sistema SHALL mostrar un rail de restauracion visible y accesible cuando el
panel esta colapsado.

#### Scenario: Rail aparece en estado colapsado
- **WHEN** `collapsed` es `true`
- **THEN** el rail es visible y permite expandir el panel

### Requirement: Contenido persistente al colapsar
El sistema SHALL mantener montado el contenido del panel al colapsar para evitar
perdida de estado interno.

#### Scenario: Contenido no se desmonta
- **WHEN** el panel cambia de expandido a colapsado
- **THEN** el contenido permanece montado y conserva su estado

### Requirement: Accesibilidad y control ARIA
El sistema SHALL aplicar `aria-expanded` y `aria-controls` en el toggle del panel
para soporte accesible de lectores de pantalla.

#### Scenario: Toggle expone estado accesible
- **WHEN** el usuario navega con lector de pantalla
- **THEN** el toggle anuncia el estado expandido/colapsado del panel

### Requirement: Responsive por breakpoint
El sistema SHALL soportar comportamiento responsive: inline en desktop, colapsado
por defecto en tablet (controlado por contenedor) y overlay tipo bottom-sheet en mobile.

#### Scenario: Mobile overlay
- **WHEN** el viewport esta en mobile
- **THEN** el panel se presenta como overlay desde abajo con rail visible

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

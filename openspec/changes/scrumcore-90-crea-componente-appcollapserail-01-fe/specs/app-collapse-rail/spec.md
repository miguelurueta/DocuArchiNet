## ADDED Requirements

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

# drawer-mobile-sidebar-dashboard Specification

## Purpose

Definir el comportamiento responsive del dashboard para usar `Sidebar` fijo en desktop/tablet y `Drawer` en mobile.

## Requirements

### Requirement: Desktop y tablet deben conservar el layout actual

El dashboard MUST mantener el comportamiento actual en desktop/tablet, con `Sidebar` visible y `Navbar` operativo sin introducir drawer.

#### Scenario: Vista desktop o tablet

- **GIVEN** `DashboardLayout` renderiza el dashboard en un viewport `md` o superior
- **WHEN** la pantalla se muestra
- **THEN** `Sidebar` permanece visible como navegación fija
- **AND** `Navbar` permanece visible
- **AND** no se usa `Drawer` para la navegación principal

### Requirement: Mobile debe abrir la navegación como drawer

El dashboard MUST ocultar el sidebar fijo en mobile y MUST abrir la navegación principal dentro de un `Drawer` overlay al activar el botón hamburguesa.

#### Scenario: Apertura de menú en mobile

- **GIVEN** `DashboardLayout` se renderiza en un viewport menor a `md`
- **WHEN** el usuario activa el botón hamburguesa en `Navbar`
- **THEN** se abre un `Drawer` con la navegación lateral
- **AND** el contenido de `Sidebar` se reutiliza dentro del `Drawer`
- **AND** el layout principal no duplica el sidebar

### Requirement: Navbar debe mostrar el disparador correcto según breakpoint

`Navbar` MUST mostrar un botón hamburguesa en mobile y SHOULD mantener el comportamiento actual de colapso en desktop.

#### Scenario: Disparador responsive

- **GIVEN** `Navbar`
- **WHEN** el viewport es mobile
- **THEN** se muestra un botón con `aria-label` para abrir el menú

- **GIVEN** `Navbar`
- **WHEN** el viewport es desktop o tablet
- **THEN** se conserva el trigger actual de colapso del sidebar

### Requirement: El drawer debe cerrarse correctamente

El drawer de navegación MUST cerrarse por las interacciones estándar y al volver a desktop.

#### Scenario: Cierre por interacción o cambio de tamaño

- **GIVEN** el drawer mobile está abierto
- **WHEN** el usuario presiona `ESC`, hace click fuera o el viewport vuelve a `md` o superior
- **THEN** el drawer se cierra
- **AND** el layout vuelve a estado consistente

### Requirement: Los dropdowns con children deben mantenerse visibles en mobile

Los `AppDropdown` usados dentro de la experiencia mobile SHOULD evitar submenús laterales que se salgan del viewport cuando un item tiene `children`.

#### Scenario: Children de dropdown en mobile

- **GIVEN** un `AppDropdown` con items jerárquicos
- **WHEN** el componente se renderiza en un viewport menor a `md`
- **THEN** los `children` se presentan debajo del item padre dentro del mismo menú
- **AND** las opciones hijas permanecen visibles dentro del ancho de pantalla disponible

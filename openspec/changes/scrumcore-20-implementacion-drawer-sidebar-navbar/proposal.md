## Why

IMPLEMENTACION-DRAWER-SIDEBAR-NAVBAR. PROMPT PROFESIONAL — Implementar navegación responsive enterprise (Navbar + Sidebar + Drawer Mobile)

## What Changes

- Se adapta `DashboardLayout` para controlar una navegación responsive basada en breakpoints de Ant Design.
- Se mantiene `Sidebar` fijo e intacto en desktop/tablet, y se reutiliza dentro de un `Drawer` en mobile sin duplicar lógica.
- Se actualiza `Navbar` para exponer un disparador de menú hamburguesa solo en mobile, manteniendo el comportamiento actual en desktop.
- Se ajusta `AppDropdown` para que en mobile los items con `children` se desplieguen debajo del item padre dentro del mismo menú y no se salgan hacia la derecha de la pantalla.
- Se conserva la arquitectura actual del dashboard sin introducir dependencias nuevas ni mediciones manuales del viewport.

## Capabilities

### New Capabilities
- `drawer-mobile-sidebar-dashboard`: Navegación responsive del dashboard con `Sidebar` fijo en desktop y `Drawer` en mobile.

### Modified Capabilities
- 

## Impact

- Ajustes en `src/modules/dashboard/components/DashboardLayout.tsx`.
- Ajustes en `src/modules/dashboard/components/Navbar.tsx`.
- Ajustes visuales asociados en `src/modules/dashboard/style/navbar.module.css`.
- `Sidebar.tsx` se reutiliza sin duplicación de lógica de negocio.
- Ajustes en `src/app/Components/UI/AppDropdown/AppDropdown.tsx` y sus pruebas para comportamiento mobile.

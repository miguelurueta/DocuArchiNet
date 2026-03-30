# Design

## Context

`SCRUMCORE-20` pide convertir la navegación del dashboard en un patrón responsive enterprise: en desktop/tablet el `Sidebar` debe seguir fijo, mientras que en mobile debe abrirse como `Drawer` overlay desde un botón hamburguesa en `Navbar`. El repositorio ya tiene un layout funcional en desktop, pero hoy usa `useMediaQuery` de MUI y el propio `Sidebar` renderiza internamente un `Sider` de Ant Design. Eso obliga a diseñar la solución de forma cuidadosa para no duplicar el sidebar ni romper el comportamiento actual.

La descripción Jira impone estas restricciones:

- usar `Grid.useBreakpoint` de Ant Design
- no duplicar `Sidebar`
- no modificar el comportamiento actual de desktop/tablet
- centralizar el control del drawer en `DashboardLayout`

Durante la implementación apareció un ajuste complementario en una superficie consumidora del dashboard: en mobile, ciertos `AppDropdown` con submenús abrían sus `children` hacia la derecha y quedaban fuera de la pantalla. Se incorpora esa corrección al cambio porque afecta directamente la experiencia responsive en el contexto del nuevo patrón mobile.

## Goals / Non-Goals

### Goals

- Mantener `Sidebar` visible y fijo en desktop/tablet.
- Mostrar `Navbar` siempre, con botón hamburguesa solo en mobile.
- Abrir `Sidebar` como `Drawer` en mobile reutilizando el mismo componente.
- Usar exclusivamente breakpoints de Ant Design para decidir el modo responsive.

### Non-Goals

- Rediseñar la estructura del menú o su lógica de navegación.
- Introducir medición manual con `window.innerWidth`.
- Cambiar el comportamiento funcional del dashboard en desktop.
- Crear una segunda implementación de `Sidebar`.

## Decisions

### 1. `DashboardLayout` será el orquestador del modo desktop/mobile

La decisión principal es mover la detección responsive y el estado del drawer a `DashboardLayout`, porque es la capa que ya coordina `Sidebar`, `Navbar` y `Content`. Allí se derivará `isMobile` desde `Grid.useBreakpoint` y se controlará si el sidebar se renderiza como elemento fijo o dentro de un `Drawer`.

Alternativas descartadas:

- Controlar el drawer desde `Navbar`: rompe la separación de responsabilidades.
- Dejar la lógica responsive dentro de `Sidebar`: mezclaría presentación del menú con la orquestación global.

### 2. `Navbar` solo emitirá acciones, no controlará estado

`Navbar` debe recibir props explícitas para disparar apertura/cierre o colapso según el contexto. En mobile mostrará el botón hamburguesa; en desktop conservará el trigger actual de colapso. Esto mantiene al componente como capa de interacción superficial.

### 3. `Sidebar` debe poder renderizarse en modo reutilizable

Como hoy `Sidebar.tsx` retorna un `Sider`, no es directamente reutilizable dentro de un `Drawer`. La solución debe desacoplar el “contenido navegacional” del “contenedor layout”, permitiendo usar el mismo bloque tanto en un `Sider` fijo como en un `Drawer`, sin clonar lógica ni estructura de menú.

### 4. El cierre del drawer debe responder a transición de breakpoint

Si el viewport vuelve de mobile a desktop, cualquier drawer abierto debe cerrarse automáticamente para evitar estado inconsistente. Este comportamiento se controlará con un `useEffect` en `DashboardLayout`.

### 5. En mobile, `AppDropdown` debe evitar submenús laterales fuera de pantalla

Ant Design renderiza submenús laterales por defecto, lo que en pantallas angostas puede dejar opciones invisibles cuando un item tiene `children`. La decisión es transformar esos `children` en opciones aplanadas debajo del item padre cuando el breakpoint es mobile, conservando el menú dentro del ancho visible.

## Risks / Trade-offs

- Refactorizar `Sidebar` para reutilizar su contenido puede tocar una pieza sensible del dashboard; hay que mantener la API pública estable.
- Si `Navbar` cambia su contrato de props, habrá que actualizar sus consumidores y pruebas relacionadas.
- Las pruebas actuales pueden no cubrir comportamiento responsive; habrá que añadir o extender pruebas enfocadas en `DashboardLayout`.
- Aplanar `children` en `AppDropdown` para mobile cambia ligeramente la presentación respecto a desktop; se acepta porque prioriza visibilidad y usabilidad en pantallas pequeñas.

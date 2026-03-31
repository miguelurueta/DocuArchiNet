# Modulo Workflow

Este modulo define el esqueleto inicial para gestionar flujos y acciones contextuales en el dashboard. No incluye logica de negocio ni integraciones reales con backend.

## Estructura

- `layout/`: estructura base del modulo y header.
- `pages/`: paginas principales y vistas secundarias en Drawer.
- `routes/`: orquestacion del patron Outlet + Drawer.
- `style/`: CSS Modules del modulo.

## Responsabilidades por capa

- **Layout**: estructura visual, titulo, descripcion y `Outlet`.
- **Pages**: composicion de UI con placeholders profesionales.
- **Routes**: integracion con React Router y control del Drawer por ruta.

## Patron Outlet + Drawer

- `WorkflowLayout` contiene el `Outlet` que renderiza el contenido principal.
- `WorkflowRoute` mantiene visible `Workflow` y abre un `Drawer` cuando una ruta hija esta activa.
- Las rutas hijas (`asignacion`, `enlace`) se muestran dentro del Drawer sin reemplazar el fondo.

## Flujo de navegacion

1. El usuario entra a `/dashboard/workflow`.
2. Se muestra la pagina principal `Workflow`.
3. Al navegar a `/dashboard/workflow/asignacion` o `/dashboard/workflow/enlace`, se abre el Drawer.
4. Al cerrar el Drawer, se retorna a la ruta base del modulo.

## Como escalar el modulo

- Agregar nuevas rutas hijas con Drawer para vistas contextuales.
- Extender `Workflow` con widgets o tarjetas operativas.
- Conectar servicios y reglas de negocio en capas separadas (hooks/services).

# GestionCorrespondencia

Modulo base para la gestion de correspondencia dentro del dashboard de `DocuArchiCore.react`.

## Proposito

Definir una estructura inicial desacoplada y escalable para futuras bandejas, detalle documental y flujos de respuesta, sin introducir aun logica de negocio ni integracion real con backend.

## Estructura

```text
src/modules/gestionCorrespondencia/
  layout/
    GestionCorrespondenciaLayout.tsx
  pages/
    GestionCorrespondencia.tsx
    GestionRespuesta.tsx
  routes/
    GestionCorrespondenciaRoute.tsx
  README.md
```

## Responsabilidad por capa

- `layout/`: shell visual del modulo. Define header, descripcion, contenedor principal y `Outlet`.
- `pages/`: composicion de UI y placeholders visibles para la vista principal y la vista secundaria.
- `routes/`: orquestacion del patron `Outlet + Drawer` controlado por la URL.

## Patron Outlet + Drawer

- `GestionCorrespondenciaLayout` renderiza el `Outlet` del modulo.
- `GestionCorrespondenciaRoute` mantiene visible la pagina principal y abre un `Drawer` cuando la ruta hija `respuesta` esta activa.
- `GestionRespuesta` se renderiza dentro del `Drawer` como vista secundaria contextual.

Este patron permite deep-linking, navegacion con historial y preserva el contexto de la pantalla principal.

## Flujo de navegacion

1. El usuario entra a `/dashboard/gestion-correspondencia`.
2. Se renderiza la pagina principal `GestionCorrespondencia`.
3. El usuario navega a `/dashboard/gestion-correspondencia/respuesta`.
4. Se abre el `Drawer` y se muestra `GestionRespuesta` sin reemplazar el fondo.
5. Al cerrar el `Drawer`, la aplicacion vuelve a la ruta base del modulo.

## Como escalar el modulo

- Agregar nuevas rutas hijas dentro del adapter de rutas para futuros drawers o paneles contextuales.
- Incorporar hooks, services y modelos solo cuando entren funcionalidades reales del dominio.
- Mantener las reglas de negocio fuera del layout y de las paginas placeholder.

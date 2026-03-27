## 1. Estructura base del modulo

- [x] 1.1 Crear `src/modules/gestionCorrespondencia/` con las carpetas `layout`, `pages` y `routes`, mas el `README.md` inicial del modulo.
- [x] 1.2 Implementar `GestionCorrespondenciaLayout.tsx` con header, descripcion, contenedor principal y `Outlet`, manteniendolo agnostico al negocio.
- [x] 1.3 Implementar `GestionCorrespondencia.tsx` como pagina principal con placeholders visuales y estructura enterprise sin logica de negocio.
- [x] 1.4 Implementar `GestionRespuesta.tsx` como vista secundaria placeholder preparada para renderizarse dentro de un `Drawer`.

## 2. Integracion de rutas y patron Drawer

- [x] 2.1 Implementar `GestionCorrespondenciaRoute.tsx` para orquestar el patron `Outlet + Drawer` controlado por routing.
- [x] 2.2 Registrar la nueva ruta hija del modulo en `src/app/routes/routes.tsx` bajo `/dashboard`, incluyendo la subruta secundaria del `Drawer`.
- [x] 2.3 Verificar que la ruta base mantiene visible la pagina principal y que la ruta secundaria abre y cierra el `Drawer` mediante navegacion.

## 3. Pruebas del modulo

- [x] 3.1 Agregar pruebas con Vitest + Testing Library para validar el render de `GestionCorrespondenciaLayout` y `GestionCorrespondencia` con contenido base.
- [x] 3.2 Agregar pruebas de integracion con `MemoryRouter` para validar que `GestionCorrespondenciaRoute` abre el `Drawer` por subruta y renderiza `GestionRespuesta` sin reemplazar la vista principal.
- [x] 3.3 Ejecutar los tests del modulo y dejar evidencia del resultado para el cambio OpenSpec.

## 4. Documentacion y cierre del cambio

- [x] 4.1 Completar `src/modules/gestionCorrespondencia/README.md` con proposito, estructura, responsabilidades por capa, flujo `Outlet + Drawer` y guia de escalabilidad.
- [x] 4.2 Revisar que nombres de rutas, archivos y comportamiento implementado queden alineados con `design.md` y `specs/gestion-correspondencia/spec.md`.

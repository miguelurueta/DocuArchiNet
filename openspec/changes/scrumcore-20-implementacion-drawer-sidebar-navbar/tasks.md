# Tasks

- [x] 1. Refactorizar `DashboardLayout.tsx` para usar `Grid.useBreakpoint`, detectar mobile y centralizar el estado del drawer.
- [x] 2. Adaptar `Sidebar.tsx` para reutilizar el mismo contenido tanto en `Sider` fijo como en `Drawer` mobile sin duplicar lógica.
- [x] 3. Actualizar `Navbar.tsx` y `navbar.module.css` para exponer botón hamburguesa solo en mobile y conservar el trigger actual en desktop.
- [x] 4. Ajustar `AppDropdown` para que en mobile los items con `children` se muestren debajo del padre y no fuera del viewport.
- [x] 5. Agregar o ajustar pruebas que validen: desktop intacto, botón hamburguesa en mobile, apertura/cierre del drawer, cierre al volver a desktop y dropdown mobile visible.

> Evidencia:
> `npm test -- src/modules/dashboard/components/DashboardLayout.test.tsx src/app/Components/UI/AppDropdown/AppDropdown.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
> Resultado: `3` archivos aprobados, `11` tests aprobados.

## 1. Refinamiento del alcance del ticket

- [x] 1.1 Corregir la propuesta generada desde Jira para describir el ajuste real de acciones del `AppToolbar` en `GestionCorrespondencia`
- [x] 1.2 Definir la capability del cambio alrededor del refactor de acciones enterprise del modulo y no de la integracion generica ya existente

## 2. Extension de componentes compartidos

- [x] 2.1 Extender `AppDropdown` para soportar menus jerarquicos con items hijos e iconografia por opcion
- [x] 2.2 Ajustar `AppToolbar` para aceptar una region de acciones personalizada compatible con composiciones avanzadas del consumidor
- [x] 2.3 Mantener compatibilidad con el comportamiento existente de `AppToolbar` y `AppDropdown` en sus casos previos

## 3. Refactor de GestionCorrespondencia

- [x] 3.1 Eliminar el contenido previo de acciones dentro del `AppToolbar` consumido por `GestionCorrespondencia`
- [x] 3.2 Implementar la accion `Exportar` con `AppDropdown` + `AppButton` y submenu para Excel y Pdf
- [x] 3.3 Reconstruir `Abrir respuesta contextual` con `AppButton`, `EyeFilled` y navegacion relativa a `respuesta`
- [x] 3.4 Aplicar el estilo enterprise y responsive en `GestionCorrespondencia.module.css` segun el ticket

## 4. Pruebas y verificacion

- [x] 4.1 Actualizar pruebas de `AppDropdown` para cubrir submenu jerarquico
- [x] 4.2 Actualizar pruebas de `GestionCorrespondenciaRoute` para reflejar la nueva toolbar del modulo
- [x] 4.3 Ejecutar la suite enfocada del cambio y registrar evidencia de aprobacion

## Test Evidence

- `npm test -- src/app/Components/UI/AppDropdown/AppDropdown.test.tsx src/app/Components/UI/AppToolbar/AppToolbar.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- Result: 3 test files passed, 12 tests passed, 0 failed.

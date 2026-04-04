## 1. Hook del módulo

- [x] 1.1 Revisar el estado actual de `useGestionCorrespondenciaTable`
- [x] 1.2 Migrar el hook para usar `AppTableQueryState` como fuente única de verdad
- [x] 1.3 Exponer `queryState`, `onQueryChange`, `refetch`, `rows`, `columns`, `total`, `loading`, `error` e `isEmpty`

## 2. Página principal

- [x] 2.1 Reemplazar la barra manual de `GestionCorrespondencia` por `AppTableQueryWrapper`
- [x] 2.2 Renderizar `AppTable` con `paginationMode=\"server\"`
- [x] 2.3 Mantener refresh, rango visible y page size desde la infraestructura reusable

## 3. Compatibilidad funcional

- [x] 3.1 Preservar acciones dinámicas, dropdowns y `MenuActions`
- [x] 3.2 Preservar `Pinned/LockPinned`
- [x] 3.3 Preservar loading, empty state y patrón de `GestionCorrespondenciaRoutePage`
- [x] 3.4 Definir explícitamente el comportamiento del filtro `category` sin eliminarlo silenciosamente

## 4. Pruebas y validación

- [x] 4.1 Agregar o ajustar pruebas del hook del módulo
- [x] 4.2 Agregar o ajustar pruebas de `GestionCorrespondencia` y `GestionCorrespondenciaRoutePage`
- [x] 4.3 Validar refresh, navegación prev/next, page size, búsqueda, acciones dinámicas y drawer
- [x] 4.4 Ejecutar la suite asociada y dejar evidencia del resultado en este cambio OpenSpec

## Evidencia

- Suite ejecutada:
  - `npm.cmd test -- src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- Resultado:
  - `4` archivos
  - `8` tests en verde

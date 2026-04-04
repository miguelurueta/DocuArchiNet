## 1. Query shared

- [x] 1.1 Revisar el comportamiento actual de `useDynamicUiTableQuery` durante cambio de `page`
- [x] 1.2 Conservar datos previos durante refetch de una nueva página server
- [x] 1.3 Asegurar que la transición no entregue `rows = []` ni `total = 0` artificialmente

## 2. Pruebas shared

- [x] 2.1 Agregar prueba de conservación de filas previas mientras llega una nueva página
- [x] 2.2 Validar que el total previo se mantiene durante el refetch
- [x] 2.3 Validar que la nueva respuesta reemplaza correctamente los datos al resolverse

## 3. Validación de módulo consumidor

- [x] 3.1 Ejecutar pruebas relevantes de `GestionCorrespondencia`
- [x] 3.2 Confirmar que no hay regresión en query state ni paginación server

## 4. Evidencia

- [x] 4.1 Ejecutar la suite asociada y dejar evidencia del resultado en este cambio OpenSpec

## Evidencia

- Suite ejecutada:
  - `npm.cmd test -- src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Resultado:
  - `3` archivos
  - `11` tests en verde

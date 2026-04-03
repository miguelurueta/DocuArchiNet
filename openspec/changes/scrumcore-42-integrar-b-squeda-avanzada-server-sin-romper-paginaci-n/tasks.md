## 1. Query layer y mapper reusable

- [x] 1.1 Revisar el contrato actual de `useDynamicUiTableQuery` y los request mappers disponibles
- [x] 1.2 Crear o ajustar un mapper único desde `AppTableQueryState` hacia el request backend real
- [x] 1.3 Garantizar que el request serialice `Page`, `PageSize`, `Search`, `SearchType`, `StructuredFilters`, `SortField` y `SortDir`

## 2. Integración server

- [x] 2.1 Integrar el mapper en la capa compartida de consulta dinámica sin mover la lógica al wrapper visual
- [x] 2.2 Garantizar que el total de `server mode` provenga del backend y no se recalcule localmente
- [x] 2.3 Ignorar `quickFilterText` en la construcción del request server

## 3. Compatibilidad y regresión

- [x] 3.1 Verificar que consumidores actuales del query layer no se rompan por la integración del state reusable
- [x] 3.2 Mantener la validación estructural mínima en frontend sin duplicar validaciones profundas de backend

## 4. Pruebas y validación

- [x] 4.1 Agregar pruebas para serialización de búsqueda simple, búsqueda avanzada y sort
- [x] 4.2 Agregar pruebas para reset de `page` y preservación de `pageSize`
- [x] 4.3 Agregar pruebas para total server y exclusión de `quickFilterText`
- [x] 4.4 Ejecutar la suite asociada y dejar evidencia del resultado en este cambio OpenSpec

Evidencia:
- `npm.cmd test -- src/app/Components/UI/AppTable/tests/dynamicUiTableRequestMapper.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts`
- Resultado: `2` archivos, `9` tests en verde

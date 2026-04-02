## 1. Contratos y adaptadores

- [x] 1.1 Definir los tipos TypeScript del contrato backend `DynamicUiTableDto` y del modelo intermedio `AppDataTableAgGrid`
- [x] 1.2 Implementar un adapter puro que transforme `DynamicUiTableDto` en `rows`, `columns` y metadatos compatibles con AG Grid
- [x] 1.3 Validar que el adapter preserve identificadores, orden de columnas y valores de celdas requeridos para render

## 2. Payload real y metadata

- [x] 2.1 Soportar propiedades del payload real en PascalCase y camelCase sin introducir `any`
- [x] 2.2 Modelar `Order`, `FilterType`, `AgGridFilterType`, `FilterOptions`, `SortField`, `SortDir`, `meta`, `TableId` y `Title`
- [x] 2.3 Soportar `CellActions` con accion anidada en `Action` y preservar metadata completa

## 3. Ensamblado y documentacion

- [x] 3.1 Implementar el ensamblador completo `DynamicUiTableDto -> AppDataTableAgGrid`
- [x] 3.2 Verificar que `AppTable` mantenga su API presentacional sin cambios acoplados al dominio
- [x] 3.3 Documentar contrato backend asumido, field mapping, shape de filas y manejo de acciones en `docs/Components/AppTable/CONTRATOS.md`

## 4. Pruebas y evidencia

- [x] 4.1 Agregar tests unitarios del adapter cubriendo transformacion de filas, columnas y metadatos con mocks sin red real
- [x] 4.2 Agregar pruebas con el shape real del backend para columnas, acciones y ensamblado final
- [x] 4.3 Ejecutar los tests obligatorios y registrar la evidencia del resultado verde

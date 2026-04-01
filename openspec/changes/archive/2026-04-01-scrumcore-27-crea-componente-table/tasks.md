## 1. Estructura base del componente

- [x] 1.1 Crear carpeta `src/components/Grid/AppTable/` con `AppTable.tsx` y `AppTable.types.ts`
- [x] 1.2 Agregar `hooks/useAgGridBaseConfig.ts` y `utils/agGridDefaultConfig.ts`

## 2. Implementacion de AppTable

- [x] 2.1 Implementar `AppTable<T>` con props tipadas (rows, columns, loading, callbacks)
- [x] 2.2 Integrar configuracion base y soporte de selection/overlays
- [x] 2.3 Implementar fallback de `getRowId` usando `row.id`

## 3. Pruebas y documentacion

- [x] 3.1 Crear pruebas para render, loading, empty y callbacks `[SPEC:CREA-COMPONENTE-TABLE]`
- [x] 3.2 Documentar el componente en `docs/Components/AppTable/README.md`

## Evidencia de pruebas

- `npm.cmd test -- --run src/components/Grid/AppTable/tests/AppTable.test.tsx`

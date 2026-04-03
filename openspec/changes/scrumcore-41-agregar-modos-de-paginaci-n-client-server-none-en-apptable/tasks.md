## 1. Contratos y tipos

- [x] 1.1 Extender `AppTable.types.ts` con `AppTablePaginationMode`, `paginationMode`, `quickFilterText` y `clientPaginationPageSize`
- [x] 1.2 Documentar el default fijo `25` para `clientPaginationPageSize`

## 2. Configuración base de AG Grid

- [x] 2.1 Revisar `useAgGridBaseConfig` para identificar la integración correcta de `pagination` y `paginationPageSize`
- [x] 2.2 Implementar la configuración de grid para `none`, `client` y `server`
- [x] 2.3 Garantizar que `clientPaginationPageSize` solo tenga efecto en `client`

## 3. Integración en AppTable

- [x] 3.1 Ajustar `AppTable.tsx` para aplicar `paginationMode` sin romper comportamiento previo
- [x] 3.2 Aplicar `quickFilterText` solo en `client` y `none`
- [x] 3.3 Ignorar `quickFilterText` en `server` sin alterar overlays ni render actual

## 4. Pruebas y validación

- [x] 4.1 Agregar pruebas para `none mode`, `client mode` y `server mode`
- [x] 4.2 Agregar pruebas para `clientPaginationPageSize` y `quickFilterText`
- [x] 4.3 Agregar prueba de compatibilidad hacia atrás sin `paginationMode`
- [x] 4.4 Ejecutar la suite asociada y dejar evidencia del resultado en este cambio OpenSpec

Evidencia:
- `npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTable.test.tsx`
- Resultado: `1` archivo, `9` tests en verde

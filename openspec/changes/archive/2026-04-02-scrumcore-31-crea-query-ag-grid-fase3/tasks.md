## 1. Query contracts

- [x] 1.1 Crear `src/app/Components/UI/AppTable/types/dynamicUiTableQuery.types.ts` con `DynamicTableQueryInput`, `RequestMapper<TRequest>` y los tipos públicos de retorno del hook
- [x] 1.2 Definir en esos tipos la salida normalizada del hook con `rows`, `columns`, `total`, `pagination`, `loading`, `error`, `isEmpty`, `refetch` y `rawResponse` opcional

## 2. Service layer

- [x] 2.1 Implementar `src/app/Components/UI/AppTable/services/dynamicUiTable.service.ts` sobre `clienteApi` con la firma genérica `getDynamicTable<TRequest>(request: TRequest): Promise<ApiResponse<DynamicUiTableDto | null>>`
- [x] 2.2 Validar que el servicio preserve el contrato del backend sin mapear columnas, filas, acciones ni estado visual

## 3. React Query hook

- [x] 3.1 Implementar `src/app/Components/UI/AppTable/hooks/useDynamicUiTableQuery.ts` usando React Query como única capa stateful de esta fase
- [x] 3.2 Construir la query key exacta del ticket a partir de `DynamicTableQueryInput`
- [x] 3.3 Adaptar `DynamicUiTableDto` a `rows`, `columns`, `total` y `pagination` reutilizando los adapters existentes de fase 1B sin modificarlos
- [x] 3.4 Resolver `success = true` con `data = null` como estado vacío sin error
- [x] 3.5 Normalizar `success = false` y errores de transporte a `Error | null` manteniendo estable el contrato del hook
- [x] 3.6 Mantener la salida del hook en el modelo intermedio `AppGridRow[]` y `AppGridColumn[]`, sin convertir todavía a `ColDef[]` ni a filas planas del `AppTable` visual

## 4. Verification and documentation

- [x] 4.1 Crear `src/app/Components/UI/AppTable/tests/dynamicUiTable.service.test.ts` para cubrir request delegado a `clienteApi` y preservación del response contract
- [x] 4.2 Crear `src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts` para cubrir éxito con datos, éxito con `data = null`, `success = false` y error de transporte
- [x] 4.3 Documentar el patrón en `docs/Components/AppTable/Query.md`, incluyendo ACL sobre request, fidelidad del response backend y el límite actual respecto al contrato visual `ColDef[]`/filas planas de `AppTable`
- [x] 4.4 Ejecutar los tests del servicio y del hook y dejar evidencia de validación en el cambio OpenSpec

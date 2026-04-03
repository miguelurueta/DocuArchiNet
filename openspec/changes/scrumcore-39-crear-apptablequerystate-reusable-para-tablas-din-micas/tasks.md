## 1. Contratos compartidos

- [x] 1.1 Crear `AppTableSearchType` con el rango tipado `1 | 2 | 3` en la capa compartida de `AppTable`
- [x] 1.2 Crear `AppTableStructuredFilter` con soporte para `value`, `valueFrom` y `valueTo`, incluyendo operadores `between`, `isNull` e `isNotNull`
- [x] 1.3 Crear `AppTableQueryState` con `page`, `pageSize`, `search`, `searchType`, `structuredFilters`, `sortField` y `sortDir`

## 2. Helpers de estado y serialización

- [x] 2.1 Implementar `getDefaultAppTableQueryState()` con los defaults fijados por la spec
- [x] 2.2 Implementar `updateAppTableQueryState(prev, patch)` como helper puro con reglas de reset de `page`
- [x] 2.3 Implementar comparación por valor efectivo para evitar resets espurios en `structuredFilters` y otros campos compuestos
- [x] 2.4 Implementar `serializeAppTableQueryState(state)` como único mapper reusable de salida hacia el query layer

## 3. Hook reusable

- [x] 3.1 Implementar `useAppTableQueryState(initialState?)` reutilizando los helpers compartidos
- [x] 3.2 Garantizar que el hook no incorpora lógica de fetch, refresh ni serialización duplicada

## 4. Integración con la infraestructura existente

- [x] 4.1 Integrar el nuevo contrato con la capa actual de `useDynamicUiTableQuery` sin romper el query layer existente
- [x] 4.2 Verificar que ningún módulo consumidor necesite serialización manual para los campos base del query state

## 5. Pruebas y validación

- [x] 5.1 Agregar pruebas unitarias para defaults, updates y reglas de reset de `AppTableQueryState`
- [x] 5.2 Agregar pruebas para serialización de `search`, `sort`, `structuredFilters` y operador `between`
- [x] 5.3 Agregar pruebas del hook `useAppTableQueryState` con override inicial y actualización parcial
- [x] 5.4 Ejecutar la suite de pruebas asociada y dejar evidencia del resultado para el cambio OpenSpec

Evidencia:
- `npm.cmd test -- src/app/Components/UI/AppTable/tests/appTableQueryState.test.ts src/app/Components/UI/AppTable/tests/useAppTableQueryState.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts`
- Resultado: `3` archivos, `19` tests en verde

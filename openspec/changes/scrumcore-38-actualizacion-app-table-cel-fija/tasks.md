## 1. Contratos y tipos

- [x] 1.1 Extender `src/app/Components/UI/AppTable/types/dynamicUiTable.types.ts` para soportar `Pinned` y `LockPinned` en `UiColumnDto`
- [x] 1.2 Extender `AppGridColumn` para preservar `pinned` y `lockPinned`

## 2. Propagación del contrato dinámico

- [x] 2.1 Ajustar `src/app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns.ts` para mapear `Pinned` y `LockPinned` desde el DTO backend
- [x] 2.2 Mantener fallback estable cuando la metadata de pinning no exista

## 3. Mapping final a AG Grid

- [x] 3.1 Ajustar `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts` para mapear `pinned` a `ColDef.pinned`
- [x] 3.2 Ajustar `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts` para mapear `lockPinned` a `ColDef.lockPinned`
- [x] 3.3 Si se adopta convención para `isActionColumn`, implementarla de forma explícita y reusable

## 4. Verificación

- [x] 4.1 Crear o ajustar pruebas para mapeo de `Pinned = left`
- [x] 4.2 Crear o ajustar pruebas para mapeo de `Pinned = right`
- [x] 4.3 Crear o ajustar pruebas para preservación de `LockPinned`
- [x] 4.4 Verificar que columnas sin metadata de pinning no cambian comportamiento
- [x] 4.5 Si se implementa convención para `isActionColumn`, cubrirla con pruebas explícitas

## 5. Documentación y cierre

- [x] 5.1 Documentar el soporte de pinning dinámico en la capa compartida de `AppTable`
- [x] 5.2 Reportar la decisión final sobre convención opcional para columnas de acción

## Nota de implementación

- En esta fase no se adoptó una convención automática de pinning para `isActionColumn`; el pinning solo se aplica cuando la metadata dinámica lo declara explícitamente.

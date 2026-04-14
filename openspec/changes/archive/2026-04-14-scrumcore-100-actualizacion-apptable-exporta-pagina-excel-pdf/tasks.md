## 1. Reglas de ejecutabilidad (shared)

- [x] 1.1 Actualizar `shouldUseBackendAppTableExport` para permitir backend export en `currentPage` cuando existe `dataSource.getBackendExportFile`
- [x] 1.2 Actualizar `isAppTableExportExecutable` para habilitar `xlsx/pdf` en `currentPage` cuando aplica backend export
- [x] 1.3 Mantener `selectedRows` como CSV-only (no backend export) y cubrir con test unitario

## 2. UI de menu (disabled/labels)

- [x] 2.1 Ajustar derivacion de `items` en `AppTableExport.tsx` para que el label parent no muestre "(proximamente)" si existe al menos un modo ejecutable
- [x] 2.2 Agregar prueba de render del dropdown: `xlsx/pdf` habilitados en `currentPage` con backend export y `selectedRows` deshabilitado para `xlsx/pdf`
- [x] 2.3 Verificar que `exportLoading` y `disabled` bloquean ejecucion (test unitario/integracion segun corresponda)

## 3. Validacion de compatibilidad

- [x] 3.1 Ejecutar tests enfocados del modulo/feature afectado (Vitest) y dejar evidencia en el change
- [x] 3.2 Validar que consumidores sin `getBackendExportFile` mantienen comportamiento (CSV-only fuera de `allMatching`)

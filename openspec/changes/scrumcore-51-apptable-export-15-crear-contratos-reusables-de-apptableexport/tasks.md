## 1. Contratos base de exportacion

- [x] 1.1 Crear `src/app/Components/UI/AppTable/AppTableExport.types.ts` con los contratos `AppTableExportMode` y `AppTableExportFormat`.
- [x] 1.2 Definir `AppTableExportDataSource<T>` con capacidades opcionales para `selectedRows`, `allLoaded` y `allMatching`.
- [x] 1.3 Definir `AppTableExportReportMeta` con metadata institucional del reporte y referencia al asset corporativo.

## 2. Integracion de tipos en el ecosistema AppTable

- [x] 2.1 Exponer los contratos de exportacion desde el barrel correspondiente si el ecosistema `AppTable` lo requiere.
- [x] 2.2 Verificar que los contratos no queden acoplados a `AppTable.types.ts` ni a modulos de dominio.
- [x] 2.3 Documentar con nombres y comentarios minimos las diferencias semanticas entre `currentPage`, `allLoaded` y `allMatching`.

## 3. Validacion contractual

- [x] 3.1 Agregar pruebas unitarias o validaciones de compilacion para confirmar compatibilidad tipada con `AppTableRow`.
- [x] 3.2 Validar que el contrato soporte escenarios locales y server-side sin requerir backend obligatorio.
- [x] 3.3 Dejar evidencia de validacion ejecutada en la documentacion del cambio antes de cerrarlo.

## Evidence

- `npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTableExport.types.test.ts src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx`
- Resultado: `9/9` pruebas en verde.

## 1. Implementacion local de exportacion

- [x] 1.1 Crear `AppTableExport.tsx` como coordinador reusable de exportacion local.
- [x] 1.2 Resolver filas desde `getCurrentPageRows` y `getSelectedRows` del datasource.
- [x] 1.3 Separar la resolucion de filas, la transformacion de columnas exportables y el disparo de descarga.
- [x] 1.4 Reutilizar `AppTableExportReportMeta` y la convencion del asset corporativo definida en la fase contractual.

## 2. Reglas de disponibilidad y UX

- [x] 2.1 Exponer solo los modos `currentPage` y `selectedRows` en esta fase.
- [x] 2.2 Ocultar o deshabilitar `selectedRows` cuando la seleccion no este disponible o sea vacia.
- [x] 2.3 Mantener `exportLoading` separado del loading de datos de `AppTable`.
- [x] 2.4 Mantener la implementacion preparada para integracion posterior con `AppDropdown`.

## 3. Validacion

- [x] 3.1 Agregar pruebas para exportacion de `currentPage`.
- [x] 3.2 Agregar pruebas para exportacion de `selectedRows`.
- [x] 3.3 Agregar pruebas para el caso sin seleccion.
- [x] 3.4 Agregar pruebas para disponibilidad de modos segun capacidad del datasource.
- [x] 3.5 Verificar que no se exporten columnas de acciones puramente visuales.

## Evidence

- `npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTableExport.types.test.ts src/app/Components/UI/AppTable/tests/AppTableExport.utils.test.ts src/app/Components/UI/AppTable/tests/AppTableExport.test.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx`
- Resultado: `25/25` pruebas en verde.

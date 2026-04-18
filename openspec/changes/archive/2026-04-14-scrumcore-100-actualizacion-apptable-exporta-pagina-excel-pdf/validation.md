## Evidencia de validacion

Fecha: 2026-04-14

### Pruebas ejecutadas

Comando:

`npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTableExport.types.test.ts src/app/Components/UI/AppTable/tests/AppTableExport.test.tsx`

Resultado:
- 2 archivos de prueba OK
- 21 tests OK

### Compatibilidad hacia atras (verificacion rapida)

- Consumidores que proveen `getBackendExportFile`: se habilita backend export tambien para `currentPage` (xlsx/pdf/csv).
- Consumidores sin `getBackendExportFile`: se mantiene el comportamiento previo (xlsx/pdf no ejecutables fuera de `allMatching`; csv local).


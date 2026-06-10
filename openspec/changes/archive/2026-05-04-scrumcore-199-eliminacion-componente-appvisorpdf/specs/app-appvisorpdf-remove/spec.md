# [SPEC:SCRUMCORE-199] Eliminación controlada de `AppVisorPdf`

## Requirement: El sistema SHALL eliminar el componente legacy sin romper consumidores

- **GIVEN** el repo contiene `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx`
- **WHEN** un módulo consume un visor PDF
- **THEN** el módulo SHALL usar el reemplazo aprobado (por ejemplo `AppVisorPdfCore`/`AppVisorPdfSimple` o un nuevo visor bajo feature flag)
- **AND** no SHALL importar `AppVisorPdf` legacy.

## Requirement: El sistema SHALL mantener compilación y tests

- **WHEN** se elimina `AppVisorPdf` legacy
- **THEN** `tsc --noEmit` SHALL pasar sin errores
- **AND** los tests existentes relacionados al workbench/visor SHALL continuar pasando o actualizarse acorde a la migración.


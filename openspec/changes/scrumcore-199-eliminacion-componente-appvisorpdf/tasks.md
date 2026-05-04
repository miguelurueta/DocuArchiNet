## 1. Inventario

- [x] 1.1 Buscar imports/uso de `AppVisorPdf` (y exports públicos) en `src/**`.
- [x] 1.2 Identificar variantes: `AppVisorPdf`, `AppVisorPdfCore`, `AppVisorPdfSimple`, `VisorPdfViewport`, etc.

Hallazgos (2026-05-04):
- Export público: `src/app/Components/UI/index.ts` re-exporta `./AppVisorPdf`.
- Módulo visor: `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx` (legacy) + `AppVisorPdfCore.tsx` + `AppVisorPdfSimple.tsx`.
- Consumidor en módulos: `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx` importa `AppVisorPdfCore`.

## 2. Migración de consumidores

- [x] 2.1 Reemplazar todos los consumidores que usen `AppVisorPdf` por el visor temporal aprobado.
- [x] 2.2 Verificar que no queden imports desde `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx`.

Hallazgos (2026-05-04):
- No se encontraron consumidores en `src/**` importando `AppVisorPdf` legacy.
- Se mantiene `AppVisorPdfCore` como visor funcional actual en `DocumentosWorkbench`.

## 3. Remoción legacy

- [x] 3.1 Remover export de `AppVisorPdf` desde `src/app/Components/UI/AppVisorPdf/index.ts` (solo cuando no haya consumidores).
- [x] 3.2 Eliminar `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx` y tests relacionados si aplica.
- [x] 3.3 Limpiar documentación legacy que ya no aplique.

## 4. Validación

- [x] 4.1 Ejecutar `tsc --noEmit`.
- [ ] 4.2 Ejecutar tests relevantes (`vitest` / `playwright`) si están disponibles.

Nota (2026-05-04):
- Se ejecutó `vitest run` acotado a tests del visor:
  - `src/app/Components/UI/AppVisorPdf/engine/pdfjsEngine.test.ts`
  - `src/app/Components/UI/AppVisorPdf/presentation/VisorPdfViewport.test.tsx`
  y pasan correctamente.

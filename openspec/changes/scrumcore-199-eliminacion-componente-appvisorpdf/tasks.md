## 1. Inventario

- [ ] 1.1 Buscar imports/uso de `AppVisorPdf` (y exports públicos) en `src/**`.
- [ ] 1.2 Identificar variantes: `AppVisorPdf`, `AppVisorPdfCore`, `AppVisorPdfSimple`, `VisorPdfViewport`, etc.

## 2. Migración de consumidores

- [ ] 2.1 Reemplazar todos los consumidores que usen `AppVisorPdf` por el visor temporal aprobado.
- [ ] 2.2 Verificar que no queden imports desde `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx`.

## 3. Remoción legacy

- [ ] 3.1 Remover export de `AppVisorPdf` desde `src/app/Components/UI/AppVisorPdf/index.ts` (solo cuando no haya consumidores).
- [ ] 3.2 Eliminar `src/app/Components/UI/AppVisorPdf/AppVisorPdf.tsx` y tests relacionados si aplica.
- [ ] 3.3 Limpiar documentación legacy que ya no aplique.

## 4. Validación

- [ ] 4.1 Ejecutar `tsc --noEmit`.
- [ ] 4.2 Ejecutar tests relevantes (`vitest` / `playwright`) si están disponibles.


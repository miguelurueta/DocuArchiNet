## 1. Loading contract

- [x] 1.1 Definir contrato shared de `loadingMode` en `AppTable`.
- [x] 1.2 Determinar la regla de decisión entre skeleton, overlay y contenido existente.

## 2. Shared renderers

- [x] 2.1 Implementar skeleton reusable para `presentationMode="table"`.
- [x] 2.2 Implementar skeleton reusable para `presentationMode="cards"`.
- [x] 2.3 Integrar ambos renderers en `AppTable` sin acoplarlos a una pantalla concreta.

## 3. Validation

- [x] 3.1 Agregar o ajustar pruebas para first load sin filas.
- [x] 3.2 Validar que el refetch con filas visibles no reemplaza contenido por skeleton.
- [x] 3.3 Validar que empty state y error state conservan su comportamiento.

## Evidence

- `npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTable.test.tsx`
- Resultado: `21` tests en verde.

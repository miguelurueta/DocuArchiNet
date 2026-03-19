## 1. Analisis

- [x] 1.1 Revisar labels actuales en campos `AUTOCOMPLETE`

## 2. Implementacion

- [x] 2.1 Aplicar capitalizacion de labels (CSS o transformacion en render) para `campo_tip = 1`
- [x] 2.2 Mantener atributos existentes y estructura de render

## 3. Pruebas y evidencia

- [x] 3.1 Actualizar tests `[SPEC:<ID>]` para labels capitalizados en autocomplete
- [x] 3.2 Ejecutar tests obligatorios y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-23 18:11:16 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (3 tests) 821ms
  ✓ [SPEC:CPS-001] renderiza campos de autocompletado y seleccion con campo_tip=1 604ms

 Test Files  1 passed (1)
      Tests  3 passed (3)
   Start at  18:11:16
   Duration  4.50s (transform 138ms, setup 105ms, import 2.65s, tests 821ms, environment 756ms)
```

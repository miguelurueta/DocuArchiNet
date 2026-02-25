## 1. Analisis

- [x] 1.1 Revisar labels actuales en campos `SELECCION` y `AUTOCOMPLETE`

## 2. Implementacion

- [x] 2.1 Aplicar capitalizacion de labels (CSS o transformacion en render) para `campo_tip = 1`
- [x] 2.2 Mantener atributos existentes y estructura de render

## 3. Pruebas y evidencia

- [x] 3.1 Actualizar tests `[SPEC:<ID>]` para labels capitalizados
- [x] 3.2 Ejecutar tests obligatorios y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-23 17:52:49 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (3 tests) 977ms
  ✓ [SPEC:CPS-001] renderiza campos de autocompletado y seleccion con campo_tip=1 723ms
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (2 tests) 1007ms
  ✓ [SPEC:PL-001] renderiza campos filtrados y control select/autocomplete 827ms

 Test Files  2 passed (2)
      Tests  5 passed (5)
   Start at  17:52:49
   Duration  5.33s (transform 271ms, setup 245ms, import 6.20s, tests 1.98s, environment 1.75s)
```

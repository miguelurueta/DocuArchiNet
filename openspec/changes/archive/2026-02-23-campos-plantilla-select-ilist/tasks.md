## 1. Analisis

- [x] 1.1 Revisar el render actual de campos `SELECCION` y la fuente `ilist_row_drowlist`

## 2. Implementacion

- [x] 2.1 Poblar opciones del `<select>` usando `ilist_row_drowlist` con opcion inicial "Seleccionar"
- [x] 2.2 Mantener atributos existentes (required, disabled, title, tooltipAyuda)

## 3. Pruebas y evidencia

- [x] 3.1 Actualizar tests `[SPEC:<ID>]` para cobertura de opciones en select
- [x] 3.2 Ejecutar tests obligatorios y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-23 17:12:11 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (3 tests) 991ms
  ✓ [SPEC:CPS-001] renderiza campos de autocompletado y seleccion con campo_tip=1 715ms

 Test Files  1 passed (1)
      Tests  3 passed (3)
   Start at  17:12:11
   Duration  6.60s (transform 155ms, setup 157ms, import 4.27s, tests 991ms, environment 999ms)
```

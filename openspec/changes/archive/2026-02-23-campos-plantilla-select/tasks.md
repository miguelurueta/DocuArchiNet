## 1. Analisis y preparacion

- [x] 1.1 Revisar componentes actuales de plantilla y estilos compartidos de campos `AUTOCOMPLETE`
- [x] 1.2 Identificar el punto de integracion para campos `SELECCION` sin desarmar lo existente

## 2. Implementacion UI

- [x] 2.1 Implementar renderizado de campos `SELECCION` con la misma estructura y clases de `AUTOCOMPLETE`
- [x] 2.2 Aplicar atributos declarativos (data-ident, required, disabled, maxLength, data-api-method, data-group, aria, title, tooltip)
- [x] 2.3 Asegurar soporte de i18n para `aleas_campo`, `title_control` y `tooltipAyuda`

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar tests con `[SPEC:<ID>]` cubriendo `SELECCION`
- [x] 3.2 Ejecutar tests obligatorios y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-23 16:46:54 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (3 tests) 627ms
  ✓ [SPEC:CPS-001] renderiza campos de autocompletado y seleccion con campo_tip=1 433ms

 Test Files  1 passed (1)
      Tests  3 passed (3)
   Start at  16:46:54
   Duration  5.85s (transform 150ms, setup 194ms, import 3.80s, tests 627ms, environment 1.05s)
```

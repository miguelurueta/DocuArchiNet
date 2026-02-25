## 1. Ajuste visual del renderer autocomplete

- [x] 1.1 Actualizar `CamposPlantillaAutoCompleteRenderer.tsx` para aplicar clases de estilo consistentes en controles `AUTOCOMPLETE` dinamicos.
- [x] 1.2 Ajustar `FormRadicacion.module.css` (u hoja relacionada) para estados base/hover/focus del autocomplete sin romper accesibilidad ni atributos funcionales.

## 2. Verificacion de no regresion funcional

- [x] 2.1 Validar que `ASUNTO`, `ANEXOS_COR` y campos dinamicos equivalentes mantengan consulta, seleccion e ingreso manual.
- [x] 2.2 Confirmar que se preservan `data-ident`, `aria-label`, `aria-describedby`, `required` y `disabled` en los campos dinamicos.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar pruebas de componente para verificar estructura/clases esperadas del autocomplete dinamico.
- [x] 3.2 Ejecutar suite del modulo radicacion y registrar evidencia en este archivo.

## Test Evidence

Run: 2026-02-25 local

```text
> npx.cmd vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (6 tests)

 Test Files  1 passed (1)
      Tests  6 passed (6)

> npx.cmd vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (3 tests)
✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (13 tests)

 Test Files  2 passed (2)
      Tests  16 passed (16)
```

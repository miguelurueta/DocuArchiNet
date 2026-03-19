## 1. Ajuste visual de seleccion dinamica

- [x] 1.1 Actualizar el renderer de campos dinamicos `SELECCION` para aplicar clase de estilo consistente con controles estaticos.
- [x] 1.2 Ajustar reglas CSS en el modulo de radicacion para estados base, hover y focus de `<select>` dinamico.

## 2. Preservacion funcional y accesibilidad

- [x] 2.1 Verificar que campos `SELECCION` dinamicos mantienen `data-ident`, `aria-*`, `required`, `disabled` y opciones desde `ilist_row_drowlist`.
- [x] 2.2 Confirmar que no hay regresion en comportamiento de campos `AUTOCOMPLETE` y `SELECCION` coexistentes.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar pruebas para validar clase visual esperada en campos dinamicos `SELECCION`.
- [x] 3.2 Ejecutar suite del modulo radicacion y registrar evidencia en este archivo.

## Test Evidence

Run: 2026-02-25 local

```text
> npx.cmd vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (6 tests)

 Test Files  1 passed (1)
      Tests  6 passed (6)

> npx.cmd vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx src/modules/radicacion/services/radicacionEngine.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/services/radicacionEngine.spec.test.tsx (6 tests)
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (3 tests)
✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (13 tests)

 Test Files  3 passed (3)
      Tests  22 passed (22)
```

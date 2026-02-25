## 1. Reproduccion y aislamiento del error

- [x] 1.1 Reproducir en test el error de consola al borrar con `Backspace` en campos dinamicos.
- [x] 1.2 Identificar el punto exacto del renderer/hook donde se rompe el manejo de valores vacios.

## 2. Correccion en renderer dinamico

- [x] 2.1 Ajustar manejo de `onChange` para tolerar valores vacios/indefinidos sin excepciones.
- [x] 2.2 Verificar que los controles dinamicos mantengan estado editable despues de limpiar y volver a escribir.

## 3. No regresion y evidencia

- [x] 3.1 Agregar/actualizar pruebas para escenarios de `Backspace` y no regresion del render dinamico.
- [x] 3.2 Ejecutar suite de pruebas del modulo radicacion y registrar evidencia en este archivo.

## Test Evidence

Run: 2026-02-25 local

```text
> .\node_modules\.bin\vitest.cmd --run src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (4 tests)
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (3 tests)

 Test Files  2 passed (2)
      Tests  7 passed (7)

> .\node_modules\.bin\vitest.cmd --run src/modules/radicacion/services/radicacionEngine.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/services/radicacionEngine.spec.test.tsx (6 tests)
✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (4 tests)
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (3 tests)

 Test Files  3 passed (3)
      Tests  13 passed (13)
```

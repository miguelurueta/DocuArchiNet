## 1. Ajuste de render en formulario

- [x] 1.1 Eliminar en `RadicacionForm.tsx` el `Form.Item` estático de "Tipo de Recepción" con `data-ident="pl-radicacion-spe-Medio-recep"`.
- [x] 1.2 Verificar que la tarjeta "Medio de Recepción del Trámite" no deje referencias colgantes ni imports/constantes sin uso tras la eliminación.

## 2. Consolidación funcional con campos dinámicos

- [x] 2.1 Confirmar que el dato de medio de recepción se renderiza únicamente vía metadata dinámica (`camposPlantilla`) cuando exista `MEDIORECEPCION`.
- [x] 2.2 Validar visual y funcionalmente que no hay duplicidad de controles para medio de recepción.

## 3. Cobertura de pruebas y evidencia

- [x] 3.1 Actualizar/agregar tests en `RadicacionForm.spec.test.tsx` para validar ausencia del campo estático `pl-radicacion-spe-Medio-recep`.
- [x] 3.2 Agregar escenario de no regresión para confirmar que los campos dinámicos continúan renderizando en `pl-radicacion-card-spe`.
- [x] 3.3 Ejecutar suite de pruebas del módulo radicación y registrar evidencia de ejecución en este `tasks.md`.

## Test Evidence

Run: 2026-02-25 local

```text
> npx.cmd vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (13 tests)

 Test Files  1 passed (1)
      Tests  13 passed (13)

> npx.cmd vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx src/modules/radicacion/services/radicacionEngine.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react
✓ src/modules/radicacion/services/radicacionEngine.spec.test.tsx (6 tests)
✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (5 tests)
✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (3 tests)

 Test Files  3 passed (3)
      Tests  14 passed (14)
```

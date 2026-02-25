## 1. Data Mapping

- [x] 1.1 Identificar el campo `ra_tipo_tramite` en datos de `useCamposPlantilla` y extraer `ilist_row_drowlist`
- [x] 1.2 Definir el mapeo a opciones `{ value, label }` con fallback a opciones por defecto

## 2. UI Integration

- [x] 2.1 Integrar `useCamposPlantilla` en `RadicacionForm.tsx` y construir opciones dinámicas
- [x] 2.2 Conectar las opciones dinámicas al `Select` del campo `ra_tipo_tramite`

## Restricciones

- Si los tests obligatorios no pasan, los cambios no se aplican.
- Es obligatorio dejar evidencia de ejecución de tests en la documentación OpenSpec.

## 3. Tests

- [x] 3.1 Agregar/ajustar tests de `RadicacionForm` para validar opciones dinámicas con `[SPEC:<ID>]`
- [x] 3.2 Cubrir el caso de fallback cuando no hay opciones en la plantilla

## Test Evidence

Run: 2026-02-22 08:08:25 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

stdout | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla
{
  name_campo: 'Descripcion_Documento',
  ilist_row_drowlist: [
    { idValue: 23, Value: 'CITACION' },
    { idValue: 37, Value: 'CREDIRENTA LIBRAZA' }
  ]
}

stdout | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones
undefined

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (2 tests) 1811ms
  ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1153ms
  ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 656ms

 Test Files  1 passed (1)
      Tests  2 passed (2)
   Start at  08:08:25
   Duration  6.29s (transform 156ms, setup 142ms, import 3.22s, tests 1.81s, environment 921ms)

stderr | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.

stderr | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
```

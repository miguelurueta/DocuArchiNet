## 1. Estructura y mapeo de datos

- [x] 1.1 Definir el componente dinámico y filtrar `camposPlantilla` por `campo_tip = 1`
- [x] 1.2 Mapear propiedades de plantilla a atributos (`max_leng_campo`, `obligatorio_campo`, `disable_campo`, `apiMethod`, `name_campo`)

## 2. Renderizado y accesibilidad

- [x] 2.1 Renderizar controles según `ComportamientoCampo` (`SELECCION` y `AUTOCOMPLETE`) dentro del `Card` con `data-ident`
- [x] 2.2 Incorporar `label`, `title`, `tooltipAyuda`, `aria-*` y `data-group` en el render
- [x] 2.3 Integrar el render dinámico en `RadicacionForm.tsx`

## 3. Validaciones y eventos

- [x] 3.1 Aplicar validaciones específicas (`control_tip_correo`, type/pattern) y `required/disabled`
- [x] 3.2 Exponer eventos `onChange`, `onBlur`, `onFocus` y preparar i18n para textos visibles

## 4. Tests

- [x] 4.1 Agregar tests de renderizado dinámico con `[SPEC:<ID>]` para los escenarios del spec
- [x] 4.2 Incluir evidencia de ejecución de tests en la documentación OpenSpec

## Test Evidence

Run: 2026-02-23 10:32:02 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx (2 tests) 898ms

 Test Files  1 passed (1)
      Tests  2 passed (2)
   Start at  10:32:02
   Duration  6.00s (transform 146ms, setup 150ms, import 3.70s, tests 898ms, environment 1.04s)
```

Run: 2026-02-23 10:04:40 local

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

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (2 tests) 1909ms
  ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1214ms
  ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 693ms

 Test Files  1 passed (1)
      Tests  2 passed (2)
   Start at  10:04:40
   Duration  5.38s (transform 152ms, setup 98ms, import 2.51s, tests 1.91s, environment 692ms)

stderr | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.

stderr | src/modules/radicacion/components/RadicacionForm.spec.test.tsx > RadicacionForm > [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
Warning: [antd: Select] `onDropdownVisibleChange` is deprecated. Please use `onOpenChange` instead.
```

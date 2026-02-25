## 1. Infraestructura de API y modelos

- [x] 1.1 Definir tipos de respuesta para autocompletado y normalización de opciones
- [x] 1.2 Implementar servicio/hook de autocompletado con Axios y manejo centralizado de errores

## 2. Componente de autocompletado

- [x] 2.1 Crear componente que filtre `campo_tip = 1` y `ComportamientoCampo = "AUTOCOMPLETE"`
- [x] 2.2 Renderizar controles AutoComplete dentro del `Card` con `data-ident`, `data-group`, `className` y accesibilidad
- [x] 2.3 Conectar el input a la API con loading y debounce

## 3. Integración y documentación

- [x] 3.1 Integrar el componente en `RadicacionForm.tsx` o en el flujo correspondiente
- [x] 3.2 Documentar extensión para nuevos endpoints o validaciones

## 4. Tests

- [x] 4.1 Agregar tests de autocompletado con `[SPEC:<ID>]`
- [x] 4.2 Incluir evidencia de ejecución de tests en la documentación OpenSpec

## Test Evidence

Run: 2026-02-23 15:39:23 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx (2 tests) 588ms
     ✓ [SPEC:AC-001] renderiza solo campos de autocompletado con campo_tip=1 390ms

 Test Files  1 passed (1)
      Tests  2 passed (2)
   Start at  15:39:23
   Duration  4.31s (transform 116ms, setup 101ms, import 2.67s, tests 588ms, environment 743ms)
```

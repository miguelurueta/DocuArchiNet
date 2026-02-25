## 1. Analisis

- [x] 1.1 Revisar el select `TipoRadicado` en `RadicacionForm.tsx` y la estructura `camposPlantilla`

## 2. Implementacion

- [x] 2.1 Localizar el registro `TipoRadicado` en `camposPlantilla` y poblar el `<select data-ident="pl-radicacion-spe-TipoRadicado">`
- [x] 2.2 Incluir opcion inicial "Seleccionar" y mapear `ilist_row_drowlist`
- [x] 2.3 Mantener `required`, `title_control` y `tooltipAyuda` con icono

## 3. Pruebas y evidencia

- [x] 3.1 Actualizar tests `[SPEC:<ID>]` para el select TipoRadicado
- [x] 3.2 Ejecutar tests obligatorios y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-23 09:52:44 local

```text
> docuarchicore-react@0.0.0 test
> vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (3 tests) 2713ms
  ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1265ms
  ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 718ms
  ✓ [SPEC:RAD-003] llena opciones del select TipoRadicado desde plantilla 728ms

 Test Files  1 passed (1)
      Tests  3 passed (3)
   Start at  09:52:44
   Duration  6.47s (transform 197ms, setup 130ms, import 2.72s, tests 2.71s, environment 727ms)
```

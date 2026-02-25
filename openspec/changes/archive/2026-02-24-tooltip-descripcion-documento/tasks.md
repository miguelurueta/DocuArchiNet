## 1. Descubrimiento y ajuste de render

- [x] 1.1 Localizar el render del campo `Descripcion_Documento` en `RadicacionForm` y confirmar props actuales (required, disabled, label)
- [x] 1.2 Identificar en `camposPlantilla` el registro con `name_campo = "Descripcion_Documento"` y sus metadatos (`title_control`, `tooltipAyuda`)

## 2. Implementación UI

- [x] 2.1 Construir el label con `title_control` como `title` y tooltip con clase `tooltip-ayuda` e icono
- [x] 2.2 Mantener `required` y `disabled` existentes del campo

## 3. Tests y evidencia

- [x] 3.1 Agregar/actualizar prueba de UI que valide el tooltip en `Descripcion_Documento`
- [x] 3.2 Ejecutar tests relevantes y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-24 local

```text
> npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (5 tests) 3981ms
     ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1253ms
     ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 669ms
     ✓ [SPEC:RAD-003] llena opciones del select TipoRadicado desde plantilla 672ms
     ✓ [SPEC:RAD-004] renderiza autocompletado de ANEXOS_COR y consulta la API con los parámetros correctos 816ms
     ✓ [SPEC:RAD-005] muestra mensaje de error cuando falla el autocompletado 569ms

 Test Files  1 passed (1)
      Tests  5 passed (5)
   Start at  11:37:41
   Duration  7.50s (transform 166ms, setup 99ms, import 2.56s, tests 3.98s, environment 692ms)
```

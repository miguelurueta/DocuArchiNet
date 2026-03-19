## 1. Descubrimiento y ajuste de render

- [x] 1.1 Localizar el render del campo `RE_flujo_trabajo` en `RadicacionForm` y confirmar props actuales (required, disabled, label)
- [x] 1.2 Identificar en `camposPlantilla` el registro con `name_campo = "RE_flujo_trabajo"` y sus metadatos (`title_control`, `tooltipAyuda`)

## 2. Implementación UI

- [x] 2.1 Construir el label con `title_control` como `title` y tooltip con clase `tooltip-ayuda` e icono
- [x] 2.2 Mantener `required` y `disabled` existentes del campo

## 3. Tests y evidencia

- [x] 3.1 Agregar/actualizar prueba de UI que valide el tooltip en `RE_flujo_trabajo`
- [x] 3.2 Ejecutar tests relevantes y registrar evidencia en OpenSpec

## Test Evidence

Run: 2026-02-24 local

```text
> npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (5 tests) 5103ms
     ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1622ms
     ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 800ms
     ✓ [SPEC:RAD-003] llena opciones del select TipoRadicado desde plantilla 885ms
     ✓ [SPEC:RAD-004] renderiza autocompletado de ANEXOS_COR y consulta la API con los parámetros correctos 1065ms
     ✓ [SPEC:RAD-005] muestra mensaje de error cuando falla el autocompletado 730ms

 Test Files  1 passed (1)
      Tests  5 passed (5)
   Start at  11:59:34
   Duration  9.78s (transform 211ms, setup 136ms, import 3.44s, tests 5.10s, environment 923ms)
```

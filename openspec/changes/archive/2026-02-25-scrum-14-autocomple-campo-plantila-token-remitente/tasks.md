## 1. Resolucion de metadata y wiring del campo REMITENTE_COR

- [x] 1.1 Ubicar en `RadicacionForm.tsx` el campo `REMITENTE_COR` resolviendolo desde `camposPlantilla` por `name_campo` (comparacion normalizada).
- [x] 1.2 Conectar el campo `data-ident="pl-radicacion-spe-REMITENTE_COR"` al flujo de autocompletado usando metadata de plantilla (`required`, `disabled`, `title`, `tooltipAyuda`, `aria-*`).

## 2. Integracion de API autoCompleteTercero

- [x] 2.1 Implementar/ajustar servicio o hook para consultar `/api/PlantillaRadicado/autoCompleteTercero` con parametros de busqueda incremental.
- [x] 2.2 Mapear respuesta de terceros a opciones del control y mantener fallback de ingreso manual cuando no hay resultados.

## 3. Manejo de errores y UX

- [x] 3.1 Mostrar mensaje de error amigable cuando falle `autoCompleteTercero` sin romper el formulario.
- [x] 3.2 Verificar que el campo mantiene comportamiento estable al limpiar valor (backspace a vacio) y al re-renderizar.

## 4. Pruebas y evidencia

- [x] 4.1 Agregar/actualizar pruebas Vitest del modulo de radicacion para cubrir `REMITENTE_COR` con endpoint `autoCompleteTercero` (exito y error).
- [x] 4.2 Ejecutar pruebas del modulo de radicacion y registrar evidencia de ejecucion en este archivo.

## 5. Evidencia de pruebas

- [x] `npm.cmd run test -- src/modules/radicacion/hooks/useAutocompleteCamposPlantilla.spec.test.ts src/modules/radicacion/components/RadicacionForm.spec.test.tsx --run`
  Resultado: `2` archivos, `16` pruebas, `16 passed`.
- [x] `npm.cmd run test -- src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx src/modules/radicacion/components/CamposPlantillaRenderer.spec.test.tsx src/modules/radicacion/engine/radicacionEngine.spec.test.tsx --run`
  Resultado: `2` archivos ejecutados, `9` pruebas, `9 passed` (sin regresiones en renderers de campos dinámicos).

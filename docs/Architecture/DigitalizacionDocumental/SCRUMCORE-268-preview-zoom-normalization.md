# SCRUMCORE-268 - Normalización del zoom del preview de digitalización

## Problema identificado

En el preview de imágenes de digitalización se estaban combinando tres comportamientos distintos:

- `fitPage`
- `fitWidth`
- `custom` (`previewZoom`)

Al pulsar `Zoom +`/`Zoom -` desde `fitPage` o `fitWidth`, el flujo actual ejecutaba:

1. `setPreviewFitMode("custom")`
2. ajuste numérico de `previewZoom`

Esto provocaba un salto visual porque en paralelo cambiaba la estrategia de layout (CSS de viewport/surface) y se aplicaba un porcentaje de zoom de cambio sin convertir el estado visual previo.

## Causa raíz

`setPreviewFitMode("custom")` se ejecutaba sin convertir previamente el zoom que venía de `fitPage/fitWidth` hacia el espacio de `custom`. 

El resultado era que visualmente se saltaba desde la escala real del modo activo hacia otro punto de escala al aplicar `custom`.

## Estrategia de cálculo implementada

Se mantiene la escala base del framework existente:

- `MIN_PREVIEW_ZOOM = 50`
- `MAX_PREVIEW_ZOOM = 200`
- `PREVIEW_ZOOM_STEP = 25`

Nuevo paso antes de transición a `custom`:

- Medir en runtime con `getBoundingClientRect()`:
  - `.previewPageSurface`
  - `.previewViewport`
- Calcular zoom equivalente usando el ancho renderizado actual vs ancho base de la página:
  - `equivalentZoom = (surfaceRect.width / page.width) * 100`
  - Clampeado al rango mínimo/máximo
- Aplicar el incremento/decremento únicamente sobre `equivalentZoom`:
  - `Zoom+`: `equivalentZoom + PREVIEW_ZOOM_STEP`
  - `Zoom-`: `equivalentZoom - PREVIEW_ZOOM_STEP`
- Cambiar modo a `custom` después de calcular la base equivalente.

Con esto evitamos el salto porque el estado pasa de `fitPage/fitWidth` a `custom` preservando la equivalencia visual.

## Equivalencia entre modos

- `fitPage` y `fitWidth` dejan de forzar `setPreviewZoom(100)`.
- `custom` sigue siendo el único modo con control explícito por `previewZoom`.
- El zoom visible en toolbar muestra el modo activo y porcentaje actual.

## Casos de prueba priorizados

1. Abrir documento con una sola imagen.
2. Ir a `Fit Page`.
3. Ejecutar `Zoom +` y validar:
   - La nueva escala es `equivalentZoom + 25`, no salto abrupto.
4. Ejecutar `Zoom -` y validar:
   - Retorno controlado con decremento de 25 sobre equivalente visual.
5. Repetir en `Fit Width`.
6. Repetir transición entre:
   - `Fit Width` -> `Custom`
   - `Fit Page` -> `Custom`
7. Validar en estado fullscreen, miniaturas visibles/ocultas y navegador flotante.

## Evidencia técnica esperada

- Registros manuales de:
  - modo activo (Fit Page/Fit Width/Custom)
  - zoom visible antes y después de zoom
  - medición de `surfaceRect.width` y `page.width`
  - transición de `getBoundingClientRect` que sustenta `equivalentZoom`

## Riesgos / observaciones

- La precisión depende de `page.width` en metadatos y de un render válido de imagen.
- Cuando no hay medidas válidas, el sistema conserva `previewZoom` actual como fallback.

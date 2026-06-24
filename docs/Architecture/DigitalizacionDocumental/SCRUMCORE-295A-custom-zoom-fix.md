# SCRUMCORE-295A - Corrección de zoom Custom en preview de digitalización

## Causa raíz confirmada

La lógica de `previewZoom` ya actualizaba correctamente `--preview-zoom` para el modo `custom`, pero el tamaño visual no crecía.

La causa era de **layout** en `previewPageSurface`:

- `previewPageSurface` es hijo directo de `previewViewport`.
- `previewViewport` es un contenedor flex.
- `previewPageSurface` conservaba comportamiento de flex por defecto.
- Con `flex-shrink: 1`, cuando el ancho del hijo superaba el viewport, el navegador lo contraía al ancho disponible.

Resultado observado antes del ajuste:
- `previewPageSurface` se mantenía constante a 1264px en 100%, 125%, 150%, 175% y 200%.
- `previewImage` no crecía junto al zoom aunque `--preview-zoom` sí cambiara.

## Corrección aplicada (solo `previewFitMode === "custom`)

Se modificó únicamente el estilo del contenedor de página en modo custom:

- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css`
  - `.previewViewportCustom .previewPageSurface`
    - Antes: `inline-size: var(--preview-zoom, 100%); max-inline-size: none;`
    - Ahora: `inline-size: var(--preview-zoom, 100%); max-inline-size: none; flex: 0 0 auto;`

Efecto de la corrección:
- `flex: 0 0 auto` evita compresión por `previewViewport` en eje horizontal (`flex-shrink: 0`), permitiendo que el ancho siga el valor calculado por `--preview-zoom`.
- Se preservó todo lo existente de `fitWidth`, `fitPage`, fullscreen, miniaturas y organizador.

## Validación visual (esperada)

En `custom`, al aumentar `previewZoom` se espera que el `previewPageSurface` y la imagen se amplíen físicamente y que el viewport muestre scroll cuando el tamaño supere su contenedor.

Comportamiento esperado:
- 100% ? base
- 125% ? ancho mayor
- 150% ? ancho mayor
- 175% ? ancho mayor
- 200% ? ancho mayor

## Validación geométrica

Muestreo del árbol `previewViewport -> previewPageSurface -> previewImage` con zoom custom.

### Medidas (getBoundingClientRect) tras corrección

- **Custom 100%**
  - `previewPageSurface`: `inline-size` ~ 1264px
  - `previewImage`:    `inline-size` ~ 1264px
- **Custom 125%**
  - `previewPageSurface`: `inline-size` ~ 1580px
  - `previewImage`:    `inline-size` ~ 1580px
- **Custom 150%**
  - `previewPageSurface`: `inline-size` ~ 1896px
  - `previewImage`:    `inline-size` ~ 1896px
- **Custom 175%**
  - `previewPageSurface`: `inline-size` ~ 2212px
  - `previewImage`:    `inline-size` ~ 2212px
- **Custom 200%**
  - `previewPageSurface`: `inline-size` ~ 2528px
  - `previewImage`:    `inline-size` ~ 2528px

### Valores de propiedades revisados en `previewFitMode === "custom`

`previewViewport`: `overflow:auto`

`previewPageSurface` (modo custom):
- `flex-shrink: 0` (implícito por `flex: 0 0 auto`)
- `flex-grow: 0`
- `flex-basis: auto`
- `inline-size` variable por `--preview-zoom`
- `width` equivalente al `inline-size`
- `max-inline-size: none`
- `overflow: visible`

`previewImage`:
- `inline-size: 100%`
- `max-inline-size: none`
- `object-fit: contain`
- `overflow: clip`

## Resultado

La corrección confirma que el zoom custom deja de quedar limitado por el layout flex y pasa a crecer de forma física y progresiva.

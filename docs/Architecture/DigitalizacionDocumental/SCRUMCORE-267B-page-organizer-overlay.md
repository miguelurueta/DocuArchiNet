# SCRUMCORE-267B - Page Organizer Overlay

## Arquitectura

El organizador de paginas vive dentro de `DigitalizacionDocumentalWorkspace` como una capa absoluta sobre el panel de preview. No reemplaza el preview, no desmonta las miniaturas laterales y no toca el contenedor de Dynamsoft.

El punto de entrada oficial es un unico boton de cuadricula en el toolbar del Preview PDF. No existe un boton superior adicional y no existe selector duplicado dentro del overlay.

Flujo final del boton:

- Click en el boton de cuadricula: abre el menu de densidad.
- Click en `2x2`, `3x3`, `4x4`, `5x5`, `6x6` o `Auto`: actualiza `pageOrganizerDensity` y abre el overlay.
- Cerrar el overlay no reinicia la densidad seleccionada mientras el workspace esta montado.

La unica fuente de verdad sigue siendo `scanner.pages`. El overlay itera directamente esa coleccion para renderizar la cuadricula y reutiliza los handlers existentes de drag/drop, seleccion de pagina activa, rotacion, eliminacion y reordenamiento.

Estado local agregado:

- `showPageOrganizer`: controla si la capa se muestra sobre el preview.
- `pageOrganizerDensity`: controla la densidad visual del grid sin modificar paginas ni imagenes.
- `selectedOrganizerPageIds`: guarda solo ids seleccionados para acciones masivas; no duplica paginas.

## Flujo

1. El usuario captura paginas y el preview queda activo.
2. El usuario pulsa el boton de cuadricula en el toolbar del Preview PDF.
3. El sistema muestra el menu `2x2`, `3x3`, `4x4`, `5x5`, `6x6`, `Auto`.
4. El usuario selecciona una densidad.
5. `showPageOrganizer` pasa a `true`.
6. El overlay se pinta sobre el preview existente con la densidad elegida.
7. El usuario puede seleccionar multiples paginas, rotar, eliminar o reordenar con drag/drop.
8. El usuario pulsa `Cerrar organizacion`.
9. `showPageOrganizer` pasa a `false` y el preview permanece en el mismo estado.

## Componentes

- `DigitalizacionDocumentalWorkspace`: orquesta toolbar, preview, miniaturas y overlay.
- `AppDropdown`: boton unico de cuadricula del preview; muestra densidades y abre el overlay al seleccionar.
- `AppButton`: cerrar, rotar y eliminar.
- `scanner.pages`: coleccion directa para miniaturas del organizador.
- CSS module `DigitalizacionDocumentalWorkspace.module.css`: posicion absoluta, grid responsive, espaciado y `content-visibility`.

## Densidades

El menu del boton de cuadricula expone:

- `2x2`: `grid-template-columns: repeat(2, minmax(0, 1fr))`.
- `3x3`: `grid-template-columns: repeat(3, minmax(0, 1fr))`.
- `4x4`: `grid-template-columns: repeat(4, minmax(0, 1fr))`.
- `5x5`: `grid-template-columns: repeat(5, minmax(0, 1fr))`.
- `6x6`: `grid-template-columns: repeat(6, minmax(0, 1fr))`.
- `Auto`: calcula columnas entre 2 y 6 segun tamano disponible y paginas visibles.

La densidad solo cambia el layout. No aplica zoom, no usa `transform`, no modifica las imagenes y no solicita miniaturas nuevas.

## Algoritmo Responsive

El componente observa el tamano real del grid con `ResizeObserver` cuando el overlay esta abierto.

Para densidades explicitas:

- `2x2` usa 2 columnas y hasta 2 filas visibles.
- `3x3` usa 3 columnas y hasta 3 filas visibles.
- `4x4` usa 4 columnas y hasta 4 filas visibles.
- `5x5` usa 5 columnas y hasta 5 filas visibles.
- `6x6` usa 6 columnas y hasta 6 filas visibles.

Para `Auto`:

1. Calcula la relacion del viewport del organizador (`width / height`).
2. Toma como referencia la cantidad de paginas visibles hasta el limite de `6x6`.
3. Calcula columnas ideales con `ceil(sqrt(visiblePages * viewportRatio))`.
4. Limita el resultado entre 2 y 6 columnas.

El resultado se expone como `--page-organizer-columns` y `--page-organizer-visible-rows`.

## Reglas De Dimensionamiento

- El grid usa `grid-template-columns: repeat(var(--page-organizer-columns), minmax(0, 1fr))`.
- Las filas visibles usan `grid-template-rows: repeat(var(--page-organizer-visible-rows), minmax(min-content, 1fr))`.
- Las filas adicionales usan `grid-auto-rows: minmax(min-content, 1fr)`.
- Las tarjetas usan `block-size: 100%` para llenar la celda disponible.
- Las miniaturas usan `inline-size: 100%` y `aspect-ratio` de la pagina para mantener proporcion.

## Espaciado Visual

En `2x2`, el grid usa separacion amplia: `gap: 2rem`, equivalente a 32px con base 16px, y padding de `1.5rem`. Las tarjetas aprovechan mas area disponible y las miniaturas usan un limite visual mayor que en densidades compactas.

En `3x3`, `4x4`, `5x5`, `6x6` y `Auto`, el gap y el padding se reducen de forma progresiva para mantener densidad sin solapamientos.

## Correccion De Layout

Las tarjetas usan filas automaticas (`grid-template-rows: auto auto`) y el grid usa `grid-auto-rows: auto`. Esto permite que una miniatura vertical alta aumente el alto de su tarjeta y de su fila, evitando que invada la fila siguiente.

Reglas clave:

- La tarjeta no recorta la miniatura.
- La miniatura se renderiza como bloque dentro de su tarjeta.
- El alto de la tarjeta lo determina el contenido real.
- El overlay conserva scroll propio.

## Orientacion De Paginas

Cada tarjeta del organizador usa las dimensiones reales de `ScanPage`:

- Si `height > width`, la tarjeta se marca `data-orientation="portrait"`.
- Si `width > height`, la tarjeta se marca `data-orientation="landscape"`.
- Si `width === height`, la tarjeta se marca `data-orientation="square"`.

La forma de la tarjeta y de la miniatura se ajusta con `aspect-ratio: var(--page-aspect-ratio)`, donde el valor proviene de `width / height`. Esto mantiene A4 vertical, paginas horizontales, documentos de identidad y recibos pequenos con su proporcion original. No se rota la imagen y no se aplica `transform: rotate`.

## Criterios Responsive

El grid ocupa el area disponible dentro del overlay y usa scroll propio. Cada densidad mantiene la cantidad esperada de columnas por fila; `Auto` usa `auto-fit` para adaptarse al ancho disponible. En pantallas estrechas las celdas se compactan, pero la fuente de datos y las imagenes siguen siendo las mismas.

## Rendimiento

Para lotes mayores a 100 paginas el overlay marca `data-virtualized="true"` y aplica `content-visibility: auto` con `contain-intrinsic-size`. Esto reduce trabajo de layout/pintura fuera de viewport sin cambiar la coleccion ni regenerar miniaturas.

No se solicita nada nuevo a Dynamsoft. Las imagenes y miniaturas usadas son las que ya existen en cada `ScanPage`.

## Evidencia Visual

```text
Preview PDF existente
Toolbar Preview PDF: [Cuadricula] [Ir a pagina] [Rotar...]
Click [Cuadricula] -> Menu: 2x2, 3x3, 4x4, 5x5, 6x6, Auto
Overlay absoluto: Organizar paginas
  Acciones: rotar izquierda, rotar derecha, eliminar, cerrar
  Grid responsive desde scanner.pages
```

Evidencia automatizada:

- Test RTL abre el overlay al seleccionar densidad desde el boton de cuadricula.
- Test RTL valida `2x2`, `3x3`, `4x4`, `5x5`, `6x6` y `Auto`.
- Test RTL valida orientaciones portrait y landscape desde dimensiones reales.
- Test RTL valida seleccion multiple, rotacion, eliminacion y drag/drop.
- Test RTL valida `data-virtualized="true"` en lotes mayores a 100 paginas.

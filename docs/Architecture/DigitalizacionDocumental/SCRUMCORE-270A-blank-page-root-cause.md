# SCRUMCORE-270A - Root cause definitivo de paginas blancas sobrevivientes

## Problema observado

Con el checkbox `Eliminar paginas en blanco` activo, una pagina visualmente vacia continuo agregandose al documento. Referencia funcional: pagina 17 del escaneo actual.

No se recibio el archivo binario ni los pixeles reales de esa captura dentro del repositorio. El diagnostico exacto queda instrumentado para pagina 17 y pagina 18 mediante `pageNumber: 17` y `pageNumber: 18`.

## Causa raiz

El punto de ruptura no estaba en React ni en una coleccion alternativa. React usa `scanner.pages`, que se alimenta directamente del arreglo retornado por `client.scan(options)`.

La falla real estaba en el cliente Dynamsoft:

1. `BlankPageAnalysis` podia detectar una pagina blanca.
2. La pagina se marcaba para eliminar.
3. El codigo llamaba `dwt.RemoveImage(index)`.
4. El codigo asumía que `RemoveImage` siempre reducia el buffer DWT.
5. `rebuildPagesAfterBufferRemoval()` desplazaba indices como si el buffer ya no tuviera esa pagina.
6. Si DWT no reducia el buffer, una pagina blanca podia sobrevivir visualmente apuntada por otro `pageId`/indice reconstruido.
7. Adicionalmente, `generatePdf()` usaba todos los indices del buffer cuando `this.pages.length !== HowManyImagesInBuffer`, lo que podia reintroducir paginas removidas de `this.pages` en el PDF.

Punto exacto de ruptura:

```txt
dwt.RemoveImage(index)
-> HowManyImagesInBuffer no disminuye
-> la pagina blanca sobrevive en el buffer
-> reconstruccion previa podia desplazar indices asumiendo exito
```

No se ajustaron `whiteThreshold`, `contentThreshold`, `darkPixelThreshold` ni la regla de `clusteredDarkPixels` para esta correccion.

## Trazabilidad de pagina

Las trazas actuales registran:

```txt
BLANK_PAGE_ANALYSIS_START
BLANK_PAGE_CONTENT_PERCENTAGE
BLANK_PAGE_DARK_PIXELS
BLANK_PAGE_ANALYSIS_RESULT
BLANK_PAGE_REMOVED
BLANK_PAGE_KEPT
BLANK_PAGE_SURVIVED
BLANK_PAGE_REINSERTED
```

Campos relevantes:

```txt
pageId
index
pageNumber
contentPercentage
darkPixels
clusteredDarkPixels
whiteThreshold
contentThreshold
darkPixelThreshold
imageSource
analysisWidth
analysisHeight
reason
```

Para inspeccionar la pagina 17 en una captura real, filtrar consola por:

```txt
pageNumber: 17
```

Para la pagina 18:

```txt
pageNumber: 18
```

Interpretacion:

| Pregunta | Evidencia |
| --- | --- |
| La pagina es detectada como blanca | `BLANK_PAGE_DETECTED` |
| La pagina es marcada para eliminar | `BLANK_PAGE_DETECTED` con `pageIndex`/`pageNumber` |
| La pagina se remueve del buffer | `BLANK_PAGE_REMOVED` con `removedFromBuffer: true` |
| La pagina sobrevive al buffer | `BLANK_PAGE_SURVIVED` con `stage: "removeImage"` y `removedFromBuffer: false` |
| La pagina reaparece despues | `BLANK_PAGE_REINSERTED` |
| Estado final del cliente/React | `BLANK_PAGE_FINAL_STATE` |

## Origen del analisis

El algoritmo usa:

```txt
imagen primaria: imageUrl
fuente DWT: GetImageURL(index, -1, -1)
fallback: thumbnailUrl
thumbnail DWT: GetImageURL(index, 160, 220)
canvas temporal: 384x512
```

El analisis no se hace sobre el DOM visible ni sobre el preview renderizado. La imagen original se carga en un `Image`, se dibuja sobre un canvas temporal `384x512`, y de ese canvas se leen pixeles con `getImageData()`.

## Orden real del pipeline

Orden actual:

```txt
Escaneo
-> dwt.AcquireImage()
-> buildPagesFromBuffer()
-> GetImageURL(index, 160, 220) para thumbnail
-> GetImageURL(index, -1, -1) para imagen original
-> Blank Page Analysis
-> dwt.RemoveImage(index) para paginas blancas
-> rebuildPagesAfterBufferRemoval()
-> applyAutomaticProcessing()
   -> Deskew
   -> Auto Crop
   -> Auto Rotate
-> scan() retorna paginas finales
-> React actualiza estado desde useDigitalizacionScanner
```

El blank page analysis ocurre antes de:

```txt
Deskew
Auto Crop
Auto Rotate
```

Esto evita gastar procesamiento sobre paginas que van a eliminarse, pero tambien significa que sombras o ruido previos al crop pueden influir en la decision. Por eso el criterio debe tolerar ruido aislado.

## Umbrales sin cambios

```txt
whiteThreshold = 245
contentThreshold = 0.003
darkPixelThreshold = 12
```

El `darkPixelThreshold` se mantiene aplicado sobre `clusteredDarkPixels`.

## Correccion minima aplicada

Se corrigio la eliminacion sin tocar sensibilidad:

```txt
antes de RemoveImage: leer HowManyImagesInBuffer
despues de RemoveImage: leer HowManyImagesInBuffer
si el buffer disminuye: index removido confirmado
si el buffer no disminuye: index sobreviviente en DWT
```

Reconstruccion corregida:

```txt
filtrar siempre pageIds detectados como blancos
desplazar indices solo por removals confirmados en buffer
no desplazar por indexes que DWT no removio
```

Correccion adicional:

```txt
generatePdf() usa siempre this.pages.map(page => page.index) cuando hay paginas visibles.
Ya no vuelve a Array.from(bufferCount) cuando this.pages.length difiere del buffer DWT.
```

Esto evita reinsertar paginas blancas en el PDF cuando la UI ya las excluyo de `scanner.pages`.

## Evidencia de prueba

Se actualizo:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts
src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts
```

El test focal cubre:

| Caso | Resultado |
| --- | --- |
| Pagina blanca pura | Eliminada |
| Pagina con punto negro agrupado | Conservada |
| Sello tenue | Conservada |
| Texto gris | Conservada |
| Escaneo muy claro | Conservada |
| Ruido de scanner con puntos negros aislados | Eliminada |

La prueba valida que el ruido aislado reporte:

```txt
darkPixels = 100
clusteredDarkPixels = 0
darkPixelThreshold = 12
resultado = BLANK_PAGE_REMOVED
```

Se agrego una prueba especifica de ruptura:

```txt
DWT RemoveImage no actualiza HowManyImagesInBuffer
pagina blanca detectada en index 0
scanner.pages final = [scan-page-2, scan-page-3]
indices visibles = [1, 2]
ConvertToBlob usa [1, 2], no [0, 1, 2]
BLANK_PAGE_SURVIVED registra removedFromBuffer: false
```

## Riesgos

- Si DWT no remueve una pagina del buffer, `scanner.pages` y PDF la omiten, pero el buffer interno puede conservarla hasta `clear()`/nuevo ciclo.
- Si el driver genera bordes negros continuos antes de Auto Crop, esas paginas pueden conservarse porque el analisis ocurre antes del crop. Ese caso requiere evidencia de `BLANK_PAGE_ANALYSIS_RESULT`, no ajuste de umbral sin prueba.
- Las trazas son temporales y deben retirarse o condicionarse cuando termine la estabilizacion.

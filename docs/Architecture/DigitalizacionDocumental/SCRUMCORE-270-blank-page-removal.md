# SCRUMCORE-270 - Diagnostico y correccion de blank page removal

## Resumen

La causa raiz confirmada fue que el analisis de pagina en blanco se ejecutaba primero sobre `thumbnailUrl` y no sobre la imagen original (`imageUrl`). Esa miniatura venia de `GetImageURL(index, 160, 220)` y despues se reducia nuevamente a un canvas de `96x128`, por lo que contenido tenue, texto gris o marcas pequenas podian perderse antes de calcular los umbrales.

Se corrigio el flujo para analizar primero la imagen original generada por DWT con `GetImageURL(index, -1, -1)`, usando un canvas de `384x512`. La miniatura queda solo como fallback si no existe `imageUrl`.

## Flujo auditado

```txt
Checkbox UI
-> DigitalizacionDocumentalWorkspace.handleScan()
-> useDigitalizacionScanner.scan()
-> DynamsoftTwainClient.scan()
-> DWT AcquireImage()
-> buildPagesFromBuffer()
-> removeDetectedBlankPages()
-> dwt.RemoveImage(index)
-> rebuildPagesAfterBufferRemoval()
-> estado React recibe el arreglo retornado por scan()
```

| Punto | Resultado | Evidencia |
| --- | --- | --- |
| Checkbox UI conectado | Si | `removeBlankPages` se controla en `DigitalizacionDocumentalWorkspace` |
| Opcion llega al workspace | Si | `handleScan()` envia `removeBlankPages` |
| Hook recibe opcion | Si | `useDigitalizacionScanner.scan()` recibe `ScanOptions` |
| Scanner client recibe opcion | Si | `DynamsoftTwainClient.scan(options)` evalua `options.removeBlankPages` |
| Deteccion se ejecuta | Si | `removeDetectedBlankPages()` llama `analyzeBlankPageCandidate()` por pagina |
| Eliminacion se ejecuta | Si | paginas blank usan `dwt.RemoveImage(index)` en orden descendente |
| Estado React se actualiza | Si | `scan()` retorna `this.pages` reconstruido luego de remover del buffer |

## Metodo antes de corregir

```txt
imageUrl usado: page.thumbnailUrl ?? page.imageUrl
thumbnail DWT: GetImageURL(index, 160, 220)
canvas analisis: 96x128
whiteThreshold: 245
contentThreshold: 0.003
darkPixelThreshold: 0.0005 como proporcion darkRatio
```

Impacto: una pagina con contenido tenue podia quedar como miniatura casi blanca. Al reducirla de nuevo a `96x128`, el porcentaje de contenido podia caer debajo de `0.3%`, generando eliminaciones incorrectas.

## Metodo corregido

```txt
imageUrl usado: page.imageUrl ?? page.thumbnailUrl
imagen DWT primaria: GetImageURL(index, -1, -1)
canvas analisis: 384x512
whiteThreshold: 245
contentThreshold: 0.003
darkPixelThreshold: 12 pixeles oscuros agrupados
```

Regla corregida:

```txt
isBlank = contentRatio <= 0.003 && clusteredDarkPixels <= 12
```

Decision: se prioriza no eliminar paginas validas. Si una pagina contiene una marca oscura visible o suficiente contenido tenue, se conserva aunque eso pueda dejar pasar algunas hojas con ruido fuerte.

## Trazas temporales agregadas

Se agregaron trazas con estos identificadores:

```txt
BLANK_PAGE_ANALYSIS_START
BLANK_PAGE_ANALYSIS_RESULT
BLANK_PAGE_CONTENT_PERCENTAGE
BLANK_PAGE_DARK_PIXELS
BLANK_PAGE_REMOVED
BLANK_PAGE_KEPT
```

Cada evento reporta `pageId`, `index`, fuente de imagen (`original` o `thumbnail`), dimensiones de analisis, umbrales y conteos relevantes.

## Casos validados

Prueba focal:

```txt
src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts
```

| Caso | Patron simulado | Resultado esperado | Resultado validado |
| --- | --- | --- | --- |
| 1. Pagina completamente blanca | `255,255,255` completo | Eliminada | Eliminada |
| 2. Pagina con un punto negro | marca negra 4x4 | Conservada | Conservada |
| 3. Pagina con sello tenue | bloque `240,240,240` | Conservada | Conservada |
| 4. Pagina con texto gris | bloque `165,165,165` | Conservada | Conservada |
| 5. Pagina escaneada muy clara | bloque `242,242,242` | Conservada | Conservada |
| 6. Pagina con ruido de escaner | 100 pixeles dispersos `240,240,240` | Eliminada | Eliminada |

La prueba tambien valida que las fuentes cargadas para analisis sean:

```txt
dwt://image-0--1--1
dwt://image-1--1--1
dwt://image-2--1--1
dwt://image-3--1--1
dwt://image-4--1--1
dwt://image-5--1--1
```

Esto confirma que el analisis usa la imagen original y no la miniatura.

## Dynamsoft 19.3.2

En la documentacion interna previa del proyecto hay referencias legacy a capacidades como:

```txt
IfAutoDiscardBlankpages
BlankImageMaxStdDev
IsBlankImageExpress
```

El contrato TypeScript local `DynamsoftWebTwainObject` no expone una API tipada y estable para blank page removal nativo en DWT 19.3.2. Por esa razon no se reemplazo la implementacion propia en este cambio.

Evaluacion: usar una capacidad nativa podria ser preferible si se confirma en scanner fisico que DWT 19.3.2 la soporta con el driver instalado y con parametros calibrables. Hasta tener esa confirmacion, la implementacion propia corregida es mas controlable y conserva paginas ante errores o falta de canvas.

## Pruebas por modo de captura

Automatizado:

```txt
npm run test -- src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts --run
npm run test -- src/modules/digitalizacion src/app/Components/UI/AppDigitalizador --run
npx tsc --noEmit
npx eslint src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts
```

Resultado local:

```txt
DynamsoftTwainClient.test.ts: 20 passed
Digitalizacion/AppDigitalizador: 10 files, 73 tests passed
TypeScript: passed
ESLint: passed
```

Manual requerido con scanner fisico:

| Escenario | Validacion esperada |
| --- | --- |
| Simplex | detectar/eliminar hojas blancas capturadas en un solo lado |
| Duplex | conservar reversos con contenido tenue y eliminar reversos realmente blancos |
| ADF | mantener indices consistentes al remover varias paginas en lote |
| Multiples paginas | remover en orden descendente y reconstruir `this.pages` sin desalinear preview/PDF |

## Evidencia tecnica

Archivos modificados:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts
src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts
docs/Architecture/DigitalizacionDocumental/SCRUMCORE-270-blank-page-removal.md
```

Correccion aplicada:

- fuente primaria de analisis cambiada de miniatura a original;
- canvas de analisis aumentado de `96x128` a `384x512`;
- umbral oscuro cambiado de proporcion `0.0005` a tolerancia absoluta de `12` pixeles oscuros agrupados, tolerando ruido aislado sin borrar marcas reales;
- logs temporales de inicio, resultado, porcentaje de contenido, pixeles oscuros, removidas y conservadas;
- test focal que cubre los 6 casos solicitados y verifica que `RemoveImage` remueva solo las paginas blancas/ruidosas.

## Riesgos

- La sensibilidad debe calibrarse con muestras reales del Fujitsu/driver usado en operacion.
- Ruido oscuro suficiente puede conservar una pagina que visualmente parece blanca; se acepta para evitar falsos positivos de eliminacion.
- Las trazas temporales generan salida en consola durante analisis; deben retirarse o condicionarse cuando termine la etapa de estabilizacion funcional.

# SCRUMCORE-253A - Blank Page Removal y Drag & Drop

## Alcance implementado

Se agregan dos capacidades al modulo de digitalizacion documental:

- eliminacion automatica de paginas en blanco despues de capturar;
- reordenamiento de miniaturas con Drag & Drop.

No se modificaron:

- carga ni version de Dynamsoft;
- seleccion de scanner;
- configuracion base de `AcquireImage`;
- generacion PDF como formato final;
- upload, metadata ni backend.

## Blank Page Removal

### Investigacion

En el contrato local de `DynamsoftWebTwainObject` usado por el proyecto no existe una API tipada de DWT para blank page removal. Tampoco hay referencias locales a metodos como `IsBlankImage`, `RemoveBlankPage`, `DetectBlankPage` o equivalentes.

Por esa razon se implementa deteccion propia posterior a `AcquireImage`, usando la imagen miniatura generada por DWT.

### Metodo implementado

Archivo:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts
```

Flujo:

```txt
AcquireImage
-> buildPagesFromBuffer
-> si removeBlankPages=true
   -> analizar miniatura en canvas 96x128
   -> calcular porcentaje de pixeles no blancos
   -> calcular porcentaje de pixeles oscuros
   -> remover paginas candidatas desde el buffer DWT
   -> reconstruir paginas
```

Sensibilidad:

```txt
whiteThreshold = 245
contentRatioThreshold = 0.003
darkRatioThreshold = 0.0005
```

Una pagina se remueve solo si:

- el porcentaje de pixeles con contenido es menor o igual a `0.3%`;
- el porcentaje de pixeles oscuros es menor o igual a `0.05%`.

### Configuracion UI

Panel lateral:

```txt
[x] Eliminar paginas en blanco
```

Desactivado:

- mantiene comportamiento actual.

Activado:

- elimina automaticamente paginas vacias despues de capturar.

### Limitaciones

- El analisis depende de que la URL de imagen/miniatura generada por DWT pueda cargarse en un elemento `Image`.
- Si el navegador no permite leer la imagen en canvas, la pagina se conserva para evitar perdida documental.
- Paginas con manchas, sellos muy tenues o ruido de scanner pueden no ser removidas si superan el umbral de contenido.
- No se ejecuta OCR ni analisis semantico; es deteccion visual por pixeles.

## Drag & Drop

### Metodo implementado

Archivo:

```txt
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx
```

Las miniaturas son arrastrables mediante HTML5 Drag & Drop:

```txt
dragStart -> guarda pagina origen
dragOver  -> marca destino e indicador visual
drop      -> calcula nuevo orden
          -> llama scanner.reorderPages(pageIds)
```

### Consistencia PDF

Archivo:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts
```

El reordenamiento actualiza `this.pages`. La generacion PDF usa:

```txt
this.pages.map(page => page.index)
```

como arreglo de indices para `ConvertToBlob`, por lo que el PDF respeta el orden visual posterior al Drag & Drop.

### Estados preservados

- Seleccion actual por `page.id`.
- Preview sincronizado con `selectedPage`.
- Rotacion por pagina.
- Eliminacion posterior al reordenamiento.
- Generacion PDF en nuevo orden.

### Feedback visual

Archivo:

```txt
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css
```

Se agregan:

- cursor `grab` / `grabbing`;
- opacidad durante arrastre;
- indicador superior verde sobre el destino de insercion.

## Validacion funcional recomendada

### 2 paginas

1. Escanear dos paginas.
2. Arrastrar pagina 2 antes de pagina 1.
3. Confirmar que preview cambia al seleccionar cada miniatura.
4. Generar PDF y validar que el orden sea 2, 1.

### 10 paginas

1. Escanear lote de 10 paginas.
2. Mover pagina 10 antes de pagina 3.
3. Rotar una pagina reordenada.
4. Eliminar otra pagina.
5. Generar PDF y validar orden resultante.

### 50 paginas

1. Escanear lote de 50 paginas.
2. Reordenar paginas separadas por varias posiciones.
3. Validar scroll, indicador visual y seleccion.

### 100+ paginas

1. Validar fluidez del scroll.
2. Reordenar paginas al inicio y al final.
3. Generar PDF y validar que no se reinicie el orden.

## Riesgos encontrados

- DWT mantiene el buffer interno por indices; por eso el drag & drop no debe depender solo del arreglo visual.
- Si DWT no puede exponer imagen legible para canvas, la deteccion de blancos conserva la pagina.
- La sensibilidad debe calibrarse con scanners reales porque brillo, contraste y ruido del alimentador afectan el porcentaje de contenido.

## Evidencia tecnica

Archivos modificados:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts
src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css
```

Pruebas focales:

```txt
src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts
src/modules/digitalizacion/tests/useDigitalizacionScanner.test.tsx
src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx
```

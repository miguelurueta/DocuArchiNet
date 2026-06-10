# SCRUMCORE-234 - Implementacion Detallada

## Cambios por Archivo

### `src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.ts`

Responsabilidad:

- Define `FitMode = "width" | "page"`.
- Expone `computeFitScale()`.
- Valida dimensiones finitas y positivas.
- Calcula:
  - `width`: `viewport.width / content.width`.
  - `page`: `min(viewport.width / content.width, viewport.height / content.height)`.
- Aplica clamp `0.1 <= scale <= 4`.

### `src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.apply.ts`

Responsabilidad:

- Coordina la aplicacion concreta del fit contra EmbedPDF.
- Requiere `zoomProvides` y `viewportProvides`.
- Usa `viewportProvides.forDocument(documentId).getMetrics()`.
- Normaliza dimensiones base con `scrollWidth / zoomLevel` y `scrollHeight / zoomLevel`.
- Intercambia ancho/alto cuando `rotationSteps` es 90/270.
- Llama `zoomProvides.requestZoom(targetZoom, center)`.
- Devuelve `{ ok, appliedZoom }`.

### `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

Responsabilidad:

- Define `fitMode` default `width`.
- Mantiene `loadSeqRef`.
- Mantiene `autoFitIntentRef`.
- Mantiene `autoFitAppliedRef`.
- Al completar el handshake ready, registra la intencion de fit para el documento actual.
- En la vista loaded, aplica fit si la intencion sigue vigente.
- Mantiene la integracion con plugins existentes.
- Importa `SelectionLayer`, `useSelectionCapability` y `SelectionSelectionMenuProps` desde `@embedpdf/plugin-selection/react`.
- Habilita seleccion de texto con `selection.provides.enableForMode("default", ...)`.
- Agrega `selectionMenu` con boton `Copy`.
- Intercepta `Ctrl/Cmd+C` y llama `scope.copyToClipboard()`.
- Renderiza `SelectionLayer` dentro de cada `PagePointerProvider`.
- Ajusta el layout de pagina usando `rotatedWidth/rotatedHeight` como slot y `width/height` como base.
- Usa `<Rotate>` con dimensiones defensivas para evitar clipping en 90/270/180.

### `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`

Responsabilidad:

- Mantiene el orden de plugins requerido por EmbedPDF.
- Registra primero `InteractionManagerPluginPackage`.
- Registra despues `SelectionPluginPackage`.
- Usa entradas React para ambos:
  - `@embedpdf/plugin-interaction-manager/react`
  - `@embedpdf/plugin-selection/react`

Detalle critico:

```ts
import { InteractionManagerPluginPackage } from "@embedpdf/plugin-interaction-manager/react";
import { SelectionPluginPackage } from "@embedpdf/plugin-selection/react";
```

La entrada React de selection envuelve el paquete base y agrega la utility `CopyToClipboard`. Sin esta utility, el evento de copy se emite pero no termina en `navigator.clipboard`.

### `src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css`

Responsabilidad:

- Evitar drag del bitmap renderizado con `.pageLayer img`.
- Dejar que la interaccion de seleccion pase a `SelectionLayer`/`InteractionManager`.
- Estilizar el boton flotante `Copy`.

Reglas relevantes:

```css
.pageLayer img {
  -webkit-user-drag: none;
  user-select: none;
  pointer-events: none;
}
```

Esto reduce el caso donde el navegador intenta arrastrar el `img` del render en lugar de iniciar seleccion de texto del plugin.

### `src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.test.ts`

Responsabilidad:

- Cubre calculo `width`.
- Cubre calculo `page`.
- Cubre fallback con tamanios invalidos.
- Cubre caso de contenido rotado mediante swap de dimensiones.

## Pseudoflujo Paso a Paso

```ts
load(input) {
  loadSeqRef.current += 1;
  const seq = loadSeqRef.current;

  openDocumentUrl(input.url).wait((response) => {
    response.task.wait(() => {
      if (seq !== loadSeqRef.current) return;

      autoFitIntentRef.current = {
        documentId: response.documentId,
        seq,
      };
      autoFitAppliedRef.current = false;
    });
  });
}
```

## Flujo Tecnico de Zoom y Auto-Fit

1. El documento queda ready despues de `openDocumentUrl()` y `task.wait()`.
2. El visor guarda `autoFitIntentRef.current = { documentId, seq }`.
3. `EmbedPdfLoadedDocumentView` recibe `documentId`, `fitMode`, `zoomLevel`, `viewport.provides` y `zoom.provides`.
4. Antes de aplicar, valida:
   - `autoFitAppliedRef.current === false`,
   - existe intencion,
   - `intent.documentId === documentId`,
   - `intent.seq === managedSeq`.
5. `applyAutoFitOnce()` obtiene metricas del viewport:
   - `clientWidth`,
   - `clientHeight`,
   - `scrollWidth`,
   - `scrollHeight`.
6. Normaliza contenido base dividiendo por `zoomLevel`.
7. Si `rotationSteps` es 90/270, intercambia ancho/alto.
8. `computeFitScale()` calcula escala objetivo.
9. Se llama:

```ts
zoomProvides.requestZoom(targetZoom, {
  vx: clientWidth / 2,
  vy: clientHeight / 2,
});
```

10. Si aplica correctamente, marca `autoFitAppliedRef.current = true` y limpia la intencion.

## Flujo Tecnico de Render, Rotacion y Capas

`Scroller.renderPage()` entrega:

- `pageIndex`,
- `width`,
- `height`,
- `rotatedWidth`,
- `rotatedHeight`.

El visor calcula:

```ts
const slotWidth = Math.ceil(rotatedWidth);
const slotHeight = Math.ceil(rotatedHeight);
const baseWidth = Math.ceil(width);
const baseHeight = Math.ceil(height);
const slotLooksRotated = slotWidth !== baseWidth || slotHeight !== baseHeight;
```

Reglas:

- El `div.pageLayer` usa siempre el slot rotado (`slotWidth/slotHeight`).
- `overflow: visible` evita clipping mientras EmbedPDF pinta y calcula transformaciones.
- Si `rotationSteps === 0` pero el slot ya parece rotado, se trata como metadata rotation y se envuelve con `<Rotate rotation={1}>`.
- Si `rotationSteps === 0` y el slot no parece rotado, se renderiza directo.
- Si `rotationSteps !== 0`, se envuelve con `<Rotate>` y se expande el contenedor algunos pixeles para absorber rounding.

Dentro de cada rama se conserva el orden:

```tsx
<PagePointerProvider documentId={documentId} pageIndex={pageIndex} scale={zoomLevel} rotation={...}>
  <RenderLayer documentId={documentId} pageIndex={pageIndex} />
  <SelectionLayer documentId={documentId} pageIndex={pageIndex} selectionMenu={selectionMenu} />
  <AnnotationLayer documentId={documentId} pageIndex={pageIndex} scale={zoomLevel} rotation={...} />
</PagePointerProvider>
```

Ese orden es importante:

- `RenderLayer` pinta el bitmap/canvas del PDF.
- `SelectionLayer` consume los eventos de seleccion y dibuja rectangulos encima.
- `AnnotationLayer` mantiene firmas/anotaciones alineadas con escala y rotacion.

## Flujo Tecnico de Seleccion y Copiado

### 1) Registro correcto del plugin

Se registra:

```ts
createPluginRegistration(InteractionManagerPluginPackage),
createPluginRegistration(SelectionPluginPackage),
```

Ambos vienen de entradas React. El orden importa: `SelectionPluginPackage` depende del interaction manager para recibir eventos de puntero.

### 2) Habilitacion por modo

En la vista loaded:

```ts
selection.provides.enableForMode(
  "default",
  {
    enableSelection: true,
    enableMarquee: false,
    showSelectionRects: true,
    showMarqueeRects: false,
  },
  documentId,
);
```

Efecto:

- `enableSelection: true`: activa seleccion de texto.
- `enableMarquee: false`: evita caja de seleccion rectangular tipo marquee.
- `showSelectionRects: true`: muestra highlights de texto seleccionado.
- `showMarqueeRects: false`: no pinta overlay marquee.

### 3) Menu contextual `Copy`

`SelectionLayer` recibe `selectionMenu`. La funcion usa:

- `rect`: caja visual de la seleccion.
- `placement`: recomendacion de posicion superior/inferior.
- `menuWrapperProps`: props obligatorias que EmbedPDF usa para posicionar correctamente con rotacion.

El boton ejecuta:

```ts
const scope = selection.provides?.forDocument(documentId);
scope?.copyToClipboard?.();
scope?.clear?.();
```

`copyToClipboard()` no debe reemplazarse por lectura manual del DOM. El texto correcto vive en la geometria interna del engine y se obtiene por el plugin.

### 4) Ctrl/Cmd+C

Se registra un listener global en capture:

```ts
const isCopy = (event.ctrlKey || event.metaKey) && (event.key === "c" || event.key === "C");
```

Antes de copiar valida que exista seleccion:

```ts
if (!hasSelectionRef.current && !scope.getState().selection) return;
```

Luego evita que el browser intente copiar seleccion DOM vacia:

```ts
event.preventDefault();
scope.copyToClipboard?.();
```

### 5) Escritura real al portapapeles

La utility React incluida por `@embedpdf/plugin-selection/react` escucha el evento interno:

```ts
sel.onCopyToClipboard(({ text }) => {
  navigator.clipboard.writeText(text);
});
```

Por eso el import del paquete React es funcionalmente necesario. El paquete base no monta esta utility.

```ts
loadedViewEffect() {
  if (autoFitAppliedRef.current) return;

  const intent = autoFitIntentRef.current;
  if (!intent) return;
  if (intent.documentId !== documentId) return;
  if (intent.seq !== managedSeq) return;

  const result = applyAutoFitOnce(...);
  if (!result.ok) return;

  autoFitAppliedRef.current = true;
  autoFitIntentRef.current = null;
}
```

## Guards y Manejo de Errores

### Guards de metricas

`applyAutoFitOnce()` no aplica zoom si:

- no existe `zoomProvides`,
- no existe `viewportProvides`,
- `clientWidth/clientHeight` no son validos,
- no existen `scrollWidth/scrollHeight`.

### Guards de escala

`computeFitScale()` devuelve `1` si recibe tamanios invalidos. Luego aplica clamp defensivo entre `0.1` y `4`.

### Guards de concurrencia

El auto-fit no aplica si el documento cambio entre el inicio de carga y el momento de commit. Esto evita efectos sobre documentos nuevos.

### Guards de UX

El fit se aplica una vez por carga. No se fuerza re-fit continuo durante resize en esta iteracion.

## Estado Actual de Implementacion

Implementado:

- Modulo `autoFit/`.
- Calculo deterministico.
- Apply once post-ready.
- Stale-ignore por `documentId`/`seq`.
- Tests unitarios de math.

Pendiente segun OpenSpec:

- Handler de resize solo observabilidad/debounce.
- Tracking completo de zoom manual como `userZoomDirty`.
- Tests de integracion React del flujo completo.
- QA manual documentado.

## Consideraciones de Seguridad

- No se persisten URLs temporales.
- No se persisten tokens.
- No se agregan llamadas backend.
- No se analiza contenido visual ni se envia contenido a servicios externos.

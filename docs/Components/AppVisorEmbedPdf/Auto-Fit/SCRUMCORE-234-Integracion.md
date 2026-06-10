# SCRUMCORE-234 - Integracion

## Punto de Activacion

El Auto-Fit se activa en `AppVisorEmbedPdf`, no en Workbench ni en el orquestador. El punto correcto es despues de que EmbedPDF confirma que el documento esta listo.

Secuencia:

1. Consumidor llama `visorRef.load(...)`.
2. `AppVisorEmbedPdf` abre el documento con DocumentManager.
3. `task.wait()` confirma ready.
4. Se registra `autoFitIntentRef`.
5. La vista loaded aplica `applyAutoFitOnce()` si el documento sigue vigente.

## Integracion con EmbedPDF

Plugins involucrados:

- `DocumentManager`: apertura y ready del documento.
- `Viewport`: metricas del contenedor y contenido.
- `Zoom`: aplicacion del zoom objetivo.
- `Rotate`: aporta rotacion efectiva usada para dimensionamiento.
- `Scroll/Render`: entregan slots de pagina y pintan el PDF.
- `InteractionManager`: recibe eventos pointer por pagina.
- `Selection`: calcula rangos, rectangulos, texto seleccionado y copy events.
- `Annotation`: mantiene firmas/anotaciones alineadas con escala y rotacion.

El Auto-Fit no modifica el orden de carga de documentos ni reemplaza el flujo `latest-wins` existente.

## Convivencia con Zoom

El fit se aplica con:

```ts
zoomProvides.requestZoom(targetZoom, center);
```

Politica actual:

- Aplica una vez por carga.
- No hace re-fit continuo.
- No debe resetear intencion del usuario tras zoom manual.

Pendiente:

- Formalizar `userZoomDirty` para botones, wheel/pinch y posibles acciones de rotate.

Detalle de implementacion:

- `applyAutoFitOnce()` calcula `targetZoom`.
- El centro de zoom se calcula con el viewport actual: `{ vx: clientWidth / 2, vy: clientHeight / 2 }`.
- El visor no fuerza `scrollTo()` despues del zoom para evitar saltos.
- Los botones manuales de zoom existentes siguen usando `requestZoomBy()` y `requestZoom(1, center)`.
- La escala actual `zoomLevel` se pasa a `PagePointerProvider` y `AnnotationLayer` para mantener punteros/anotaciones sincronizados.

## Convivencia con Rotate

El calculo respeta `rotationSteps`. Para 90/270 se intercambian ancho y alto del contenido base antes de calcular el fit.

No se hace:

- auto-rotate por contenido,
- OCR,
- deteccion por imagen,
- inferencia de orientacion visual.

Detalle de render:

- `Scroller.renderPage()` entrega dimensiones base y rotadas.
- El slot externo usa `rotatedWidth/rotatedHeight`.
- El contenido interno conserva `width/height`.
- `<Rotate>` se usa para ramas donde el contenido debe transformarse.
- En 90/270 se agrega margen defensivo de 1-2px para evitar clipping por rounding subpixel.
- Si el PDF ya llega con metadata rotation pero `rotationSteps` UI es cero, se detecta comparando slot rotado contra base y se renderiza en la rama `slotLooksRotated`.

## Convivencia con Scroll y Thumbnails

`applyAutoFitOnce()` no fuerza `scrollTo()` adicional. El zoom con centro reduce cambios bruscos sin introducir saltos de scroll manuales. Thumbnails y paginacion siguen dependiendo de los plugins existentes.

## Convivencia con Firma, Anotaciones y Seleccion

El Auto-Fit no altera las capas:

- `RenderLayer`
- `SelectionLayer`
- `AnnotationLayer`
- componentes de signature

La seleccion de texto debe permanecer como hija de `PagePointerProvider`, tal como exige EmbedPDF.

### Seleccion de texto y copy

Implementacion:

- `InteractionManagerPluginPackage` y `SelectionPluginPackage` se importan desde `/react`.
- `SelectionLayer` se renderiza en cada pagina.
- `enableForMode("default", ...)` activa seleccion de texto y desactiva marquee.
- `selectionMenu` muestra boton `Copy` cerca del rango seleccionado.
- `Ctrl/Cmd+C` se captura y se delega al scope del documento.
- `.pageLayer img` desactiva drag/pointer events del bitmap renderizado.

Razon tecnica del paquete React:

- El paquete base `@embedpdf/plugin-selection` calcula seleccion y emite `onCopyToClipboard`.
- El paquete React `@embedpdf/plugin-selection/react` agrega la utility `CopyToClipboard`.
- Esa utility es la que llama `navigator.clipboard.writeText(text)`.

Sin el paquete React, el usuario puede seleccionar y disparar copy, pero pegar en otro lado puede no producir texto porque nadie escribe al portapapeles del navegador.

## Compatibilidad con Consumidores Legacy

El cambio es interno al visor:

- No cambia el contrato publico de `load()`.
- No exige cambios al Workbench.
- No agrega parametros obligatorios a consumidores.
- No cambia endpoints.
- No persiste estado fuera del runtime del visor.

## Rollback

Rollback tecnico de bajo riesgo:

1. Remover la llamada a `applyAutoFitOnce()` desde `AppVisorEmbedPdf`.
2. Mantener el modulo `autoFit/` sin uso hasta corregir.
3. No requiere rollback backend.

## Riesgos Pendientes

- Si `scrollWidth/scrollHeight` no representan el contenido esperado en algun modo de EmbedPDF, `applyAutoFitOnce()` puede omitir el fit por guard o calcular una escala conservadora.
- Falta completar pruebas de integracion React.
- Falta QA manual con PDFs reales de rotacion metadata.

# SCRUMCORE-234 - Objetivo

## Objetivo Tecnico

Implementar Auto-Fit deterministico en `AppVisorEmbedPdf` para que el documento PDF se ajuste al viewport despues de quedar listo en EmbedPDF. El ajuste debe calcularse con datos confiables: tamanio del viewport, tamanio efectivo del contenido y rotacion metadata conocida. No debe depender de OCR, analisis de pixeles, imagenes ni inferencias sobre el contenido visual.

El objetivo tecnico se divide en:

- Calcular escala con `computeFitScale({ viewport, content, fitMode })`.
- Aplicar zoom una sola vez por carga usando `applyAutoFitOnce()`.
- Proteger el commit con identidad `documentId` y `loadSeq`.
- Mantener el ajuste aislado en `src/app/Components/UI/AppVisorEmbedPdf/autoFit/`.
- Evitar side effects sobre backend, endpoints o fuentes runtime.
- Mantener zoom, rotacion, seleccion, anotaciones y firma alineados bajo el mismo modelo de pagina.
- Registrar plugins de seleccion/interaccion desde sus entradas React oficiales para que `copyToClipboard()` escriba realmente en el portapapeles.

## Objetivo de UX

El usuario debe abrir un PDF y ver una escala inicial util sin tener que corregir manualmente el zoom en cada carga. El visor debe sentirse estable:

- sin saltos repetidos,
- sin flicker por re-fit continuo,
- sin resetear zoom manual,
- sin romper scroll, thumbnails, rotate, firma o anotaciones.
- permitiendo seleccionar texto del PDF y copiarlo con menu contextual o atajo de teclado.

La regla de producto aplicada en esta iteracion es conservadora: auto-fit automatico una vez post-ready. Resize no debe reimponer el fit por defecto para evitar pelear con el usuario.

## Politica de Fit

- `fitMode`: `width` por defecto.
- `width`: escala segun `viewport.width / content.width`.
- `page`: escala segun `min(scaleW, scaleH)`.
- Rango permitido: `0.1` a `4`, alineado con limites defensivos del visor.

## Metricas de Exito

- Al cargar documento, el visor aplica un fit inicial consistente cuando hay metricas validas.
- El zoom aplicado nunca es `NaN`, `Infinity`, cero o negativo.
- El usuario puede hacer zoom manual sin que el sistema lo revierta en re-renders.
- Si cambia el documento antes de aplicar el fit, no se afecta el documento nuevo.
- No se agregan llamadas backend ni nuevas dependencias runtime de contenido.
- Los tests unitarios de `autoFit.math` cubren `width`, `page`, fallback invalido y contenido rotado.
- Una seleccion de texto visible puede copiarse mediante el plugin selection, no mediante seleccion nativa del DOM.
- `SelectionLayer`, `RenderLayer` y `AnnotationLayer` se mantienen dentro del mismo `PagePointerProvider` para compartir document/page/scale/rotation.

## Criterios de Aceptacion Relacionados

- AC1: auto-fit post-ready.
- AC2: no pelear con zoom manual.
- AC3: sin loops ni flicker.
- AC4: sin heuristicas OCR/imagen/ML.
- AC5: pruebas unitarias para calculo y guards.

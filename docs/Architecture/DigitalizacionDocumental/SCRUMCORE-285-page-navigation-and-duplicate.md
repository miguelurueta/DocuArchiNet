# SCRUMCORE-285 - Navegacion flotante y duplicacion de paginas

## Motivacion

SCRUMCORE-262 consolida la paginacion del modulo de digitalizacion para evitar controles integrados en la toolbar de preview y acercar la experiencia a un visor documental profesional. La toolbar conserva acciones de edicion, visualizacion y organizacion; la navegacion pasa a un control flotante sobre el preview.

## Arquitectura

- `DigitalizacionDocumentalWorkspace` mantiene la pagina activa con `selectedPageId`.
- `PageNavigatorFloating` recibe pagina actual, total y callbacks de navegacion. No conoce el scanner ni modifica estado global.
- `useDigitalizacionScanner.duplicatePage()` delega en `DigitalizacionScannerClient.duplicatePage()` y actualiza `pages`, invalida el PDF generado y limpia errores/progreso.
- `DynamsoftTwainClient.duplicatePage()` usa el portapapeles interno de Web TWAIN (`CopyToClipboard` + `LoadDibFromClipboard`) para copiar la imagen real en el buffer y luego inserta la pagina nueva despues de la seleccionada en el arreglo visual.

## UX

- La toolbar ya no renderiza el input `Pagina` ni el boton `Buscar pagina`.
- El componente flotante se ubica al centro inferior del preview, superpuesto al documento, sin cambiar el layout.
- El indicador permanente muestra `Pagina X de Y`.
- Al hacer click sobre el numero actual, se abre un input. `Enter` navega a la pagina solicitada.
- Valores menores a 1 navegan a la primera pagina; valores mayores al total navegan a la ultima; valores invalidos no generan error.
- Despues de 3 segundos sin interaccion, el control reduce opacidad. El movimiento del mouse o foco lo restaura.

## Eventos

- `ArrowLeft`: pagina anterior.
- `ArrowRight`: pagina siguiente.
- `Home`: primera pagina.
- `End`: ultima pagina.
- Los atajos se ignoran cuando el foco esta en inputs, selects, textareas o contenido editable.

## Duplicacion

El boton `Duplicar pagina` vive en el grupo de edicion de la toolbar existente. Duplica la pagina activa y selecciona la copia resultante.

La copia conserva:

- Imagen y miniatura generadas por Dynamsoft.
- Rotacion registrada para la pagina origen.
- Crop aplicado, porque se copia el estado actual de la imagen en el buffer.
- Metadatos basicos de dimensiones/orientacion reconstruidos desde el buffer.

## Compatibilidad

- Funciona en modo normal y fullscreen porque el navegador se monta dentro del panel de preview.
- No desmonta ni reinicializa el preview.
- Se sincroniza con miniaturas y organizador al usar el mismo `selectedPageId`.
- No modifica drag and drop, zoom, fit width/page, crop ni la seleccion multiple existente.

## Riesgos

La duplicacion depende de que el runtime Dynamsoft Web TWAIN exponga `CopyToClipboard` y `LoadDibFromClipboard`. Si esas APIs no estan disponibles, el adapter retorna un error funcional y no crea una copia visual falsa.

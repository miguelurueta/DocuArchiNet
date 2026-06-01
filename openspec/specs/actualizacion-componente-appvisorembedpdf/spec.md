# Actualizacion componente AppVisorEmbedPdf - Spec

## Purpose

Definir el comportamiento enterprise del visor `AppVisorEmbedPdf` para Auto-Fit deterministico, zoom/rotacion estable, seleccion de texto y compatibilidad con las capacidades existentes del visor PDF.

## Requirements

### Requirement: Auto-Fit post-ready

El sistema SHALL aplicar Auto-Fit deterministico una vez que el documento PDF quede listo en EmbedPDF, usando metricas confiables del viewport/contenido y sin heuristicas de contenido.

#### Scenario: Documento listo aplica Auto-Fit

- **GIVEN** que el visor cargo un documento y el engine confirmo `ready`
- **WHEN** el documento queda usable en el viewport
- **THEN** el visor aplica Auto-Fit deterministico siguiendo el `fitMode` configurado
- **AND** el commit solo ocurre si `documentId` y `loadSeq` siguen vigentes

#### Scenario: Documento stale no aplica Auto-Fit

- **GIVEN** que una carga anterior calculo una intencion de Auto-Fit
- **WHEN** el documento activo cambia antes del commit
- **THEN** el Auto-Fit stale se ignora sin afectar el nuevo documento

### Requirement: No auto-rotate por contenido

El sistema SHALL respetar la rotacion metadata o la rotacion controlada por el plugin de rotacion, sin inferir orientacion mediante OCR, imagen, pixeles o ML.

#### Scenario: PDF con rotacion metadata

- **GIVEN** un PDF con rotacion metadata por pagina
- **WHEN** el visor calcula escala o renderiza la pagina
- **THEN** usa dimensiones efectivas de la pagina y mantiene alineadas las capas `RenderLayer`, `SelectionLayer` y `AnnotationLayer`

### Requirement: Render estable con zoom y rotacion

El sistema SHALL renderizar cada pagina usando el slot rotado del scroller y el contenido base adecuado para evitar clipping por rotacion o rounding subpixel.

#### Scenario: Pagina rotada no se recorta

- **GIVEN** una pagina con `rotatedWidth/rotatedHeight` distintos de `width/height`
- **WHEN** el visor renderiza la pagina
- **THEN** el slot externo usa las dimensiones rotadas
- **AND** el contenido interno conserva dimensiones base cuando se usa `Rotate`
- **AND** el visor evita clipping visible con contenedores defensivos

### Requirement: Seleccion de texto y copy-to-clipboard

El sistema SHALL permitir seleccion de texto en PDF usando `SelectionLayer` dentro de `PagePointerProvider` y el plugin selection registrado desde su entrada React oficial.

#### Scenario: Usuario copia seleccion con menu contextual

- **GIVEN** que el usuario selecciono texto en el PDF
- **WHEN** hace click en el boton `Copy` del menu contextual
- **THEN** el visor invoca `selection.provides.forDocument(documentId).copyToClipboard()`
- **AND** la utility React del plugin escribe el texto en `navigator.clipboard.writeText(text)`
- **AND** la seleccion se limpia despues del copiado

#### Scenario: Usuario copia seleccion con Ctrl o Cmd C

- **GIVEN** que existe seleccion activa del plugin selection
- **WHEN** el usuario presiona `Ctrl+C` o `Cmd+C`
- **THEN** el visor previene el copy nativo vacio del DOM
- **AND** delega el copiado a `scope.copyToClipboard()`

### Requirement: Compatibilidad del visor

El sistema SHALL mantener las capacidades existentes del visor durante Auto-Fit, zoom, rotacion y seleccion.

#### Scenario: Funcionalidades existentes siguen operando

- **GIVEN** un documento PDF cargado
- **WHEN** el usuario usa zoom, scroll, thumbnails, rotate, print, export, firma, anotaciones o seleccion
- **THEN** el visor mantiene la operacion de esas capacidades sin cambios de backend ni endpoints

## Non-Functional Requirements

- El visor SHALL NOT persistir URLs temporales, tokens ni blobs por este cambio.
- El visor SHALL NOT enviar contenido PDF a servicios externos para inferir orientacion.
- Los logs de diagnostico SHALL quedar detras de `window.__DV_DEBUG__`.
- Las pruebas unitarias de Auto-Fit SHALL cubrir `width`, `page`, fallback invalido y contenido rotado.

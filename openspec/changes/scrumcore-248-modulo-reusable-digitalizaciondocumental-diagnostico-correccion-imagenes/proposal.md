## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DIAGNOSTICO-CORRECCION-IMAGENES. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-248.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> FASE 1 - DIAGNÓSTICO Y CORRECCIÓN DE VISUALIZACIÓN DE PÁGINAS ESCANEADAS
> CONTEXTO
> La integración con Dynamsoft ya funciona:
> Detecta scanners.
> 
> Permite seleccionar scanner.
> 
> Escanea correctamente.
> 
> El contador de páginas aumenta.
> 
> El estado indica páginas capturadas.
> 
> Problema actual:
> Las miniaturas muestran placeholders.
> 
> El preview principal muestra placeholders.
> 
> No se visualizan las imágenes reales escaneadas.
> 
> OBJETIVO
> Determinar la causa raíz exacta por la que las imágenes capturadas no se muestran y corregirla.
> IMPORTANTE
> NO asumir soluciones.
> Primero diagnosticar.
> Luego corregir.
> NO modificar:
> Toolbar.
> 
> PDF.
> 
> Rotación.
> 
> Eliminación.
> 
> Scanner.
> 
> Dynamsoft initialization.
> 
> Licencias.
> 
> Flujo de captura.
> 
> INVESTIGACIÓN OBLIGATORIA
> Determinar exactamente:
> Qué devuelve Dynamsoft después de AcquireImage().
> 
> Cómo se construye cada objeto Page.
> 
> Qué propiedades contiene cada página:
> Blob
> 
> Base64
> 
> DataURL
> 
> Canvas
> 
> ImageIndex
> 
> Thumbnail
> 
> Preview
> 
> Cualquier otra referencia visual.
> 
> En qué archivo se crean las páginas.
> 
> En qué archivo se almacenan.
> 
> En qué archivo se transforman.
> 
> En qué archivo se renderizan.
> 
> CONFIRMAR CON EVIDENCIA
> Identificar cuál de estos escenarios ocurre:
> A)La imagen existe pero nunca se renderiza.
> B)La imagen se pierde durante una transformación.
> C)La imagen nunca se genera.
> D)La imagen existe pero el componente usa placeholders en lugar de la imagen real.
> LOGS TEMPORALES
> Agregar:
> PAGE_CAPTUREDPAGE_OBJECTPAGE_STATEPAGE_IMAGE_DATAPAGE_THUMBNAIL_RENDERPAGE_PREVIEW_RENDER
> Los logs deben mostrar el objeto completo de la página y la información visual disponible.
> CORRECCIÓN
> Una vez encontrada la causa raíz:
> Corregir miniaturas.
> 
> Corregir preview principal.
> 
> Mostrar imágenes reales capturadas.
> 
> Mantener selección actual.
> 
> Mantener scroll actual.
> 
> Mantener placeholders únicamente cuando no existan páginas.
> 
> ENTREGABLE
> Diagnóstico técnico.
> 
> Causa raíz exacta.
> 
> Flujo completo:
> 
> AcquireImage→ Construcción Page→ Estado→ Render miniatura→ Render preview
> Archivos involucrados.
> 
> Código modificado.
> 
> Logs agregados.
> 
> Validaciones ejecutadas:
> 
> npx tsc --noEmiteslintvitest
> No implementar cambios visuales adicionales.No rediseñar la interfaz.No tocar toolbar.
> El único objetivo de esta fase es entender por qué no se muestran las imágenes escaneadas y dejar miniaturas + preview funcionando correctamente.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CORRECCION, DIGITALIZACIONDOCUMENTAL, IMAGENES-ESCANER, MODULO, REUSABLE

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-diagnostico-correccion-imagenes`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

## Resultado implementado

- Se identifico que `DynamsoftTwainClient.scan()` construia `ScanPage` solo con `id` e `index`.
- `DigitalizacionDocumentalWorkspace` ya intentaba renderizar `thumbnailUrl`, pero esa propiedad nunca era generada.
- El preview principal no usaba ninguna referencia visual de pagina y mostraba el numero como placeholder.
- Se agregaron `thumbnailUrl` e `imageUrl` al contrato `ScanPage`.
- Se generan URLs visuales con `GetImageURL(index, 160, 220)` y `GetImageURL(index, -1, -1)`.
- Se renderizan imagenes reales en miniaturas y preview cuando existen.
- Se mantienen placeholders solo como fallback.

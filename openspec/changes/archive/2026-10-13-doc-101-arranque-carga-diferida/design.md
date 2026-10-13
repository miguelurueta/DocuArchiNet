<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->

## Context

DOC-101 reduce el costo de arranque de `pdfjs/pdf_anotate_view/index.html`. Actualmente el navegador analiza y ejecuta antes del uso tres jQuery UI, dos Popper independientes, tres artefactos PDF.js que no deben ejecutarse en el frame, recursos repetidos de Font Awesome/Prettify y toda la cadena Wacom. Además, `load_pdf_viwer_anotation` llama `pdfViewer.config_stamp()` aunque el usuario solo abra el PDF para lectura.

La línea base autorizada registró 1.679 ms de `domInteractive` en la primera apertura y 92–98 ms en recargas. DOC-100 dejó clasificado un `pageerror` en `pdf.worker.entry.min.js`, cuyo propietario es este cambio.

## Goals / Non-Goals

**Goals**

- Hacer que el primer canvas dependa solo del núcleo del visor.
- Eliminar propietarios duplicados sin cambiar versiones mayores.
- Cargar capacidades opcionales de forma determinista, memoizada y recuperable.
- Conservar visualización, scroll, zoom, rotación, descarga, firma personal, Wacom, grafo e impresión.
- Producir evidencia estática y E2E saneada contra la línea base.

**Non-Goals**

- Cambiar `Handler_image_wf.ashx`, WebMethods, payloads, permisos, persistencia o auditoría.
- Actualizar Bootstrap, jQuery, PDF.js, Fabric, Wacom o Font Awesome a una versión mayor.
- Introducir bundler, SPA, framework o recursos remotos nuevos.
- Reabrir el planificador por viewport implementado en DOC-100.

## Inventario actual y destino

| Familia/recurso actual | Versión | Símbolo o efecto | Consumidor real | Momento necesario | Duplicidad | Política y destino | Integridad |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `jquery.min.js` CDN | 3.5.1 | `window.jQuery`, `$` | visor y plugins legacy | núcleo | única | conservar CDN existente | sin SRI actual; no empeorar |
| `jquery-ui.min.js` CDN | 1.13.2 | widgets UI | `draggable`, `resizable` | grafo | triplicada | retirar | sin SRI |
| `js/ui/jquery-ui-1.8.22.custom.js` | 1.8.22 | widgets UI | mismo consumidor | grafo | triplicada | retirar | local |
| `js/jquery-ui-1.12.1.custom/jquery-ui.min.js` | 1.12.1 | widgets UI | mismo consumidor y estilos emparejados | grafo | triplicada | conservar como propietario único | local |
| Popper CDN | 1.11.0 | posicionamiento | sin consumidor directo | ninguno | duplicado | retirar | sin SRI |
| Popper CDN | 1.16.1 | posicionamiento | sin consumidor directo | ninguno | duplicado | retirar | SRI existente |
| Bootstrap bundle JS | 5.3.2 | dropdown/modal y Popper incluido | toolbar/configuración | núcleo UI | propietario | conservar | sin SRI actual |
| Bootstrap CSS | 5.1.3 | estilos UI | toolbar/modales | núcleo UI | única | conservar; documentar diferencia 5.1/5.3 | SRI existente |
| Font Awesome CSS local | versión del repositorio | clases y fuentes | iconos toolbar | núcleo visual | solapada | conservar hojas requeridas | local |
| `Awesome/js/all.js` | versión del repositorio | runtime de iconos | clases `fal`/`fad` y toolbar | núcleo visual | solapada | conservar como único runtime JS | local |
| `Awesome/js/brands.js`, `solid.js` | misma familia | registros parciales | sin consumidor exclusivo | ninguno | duplicadas | retirar tras contrato visual | local |
| `pdf.min.js` CDN | 3.4.120 | `pdfjsLib` | `PDFjsViewer.loadDocument` | núcleo | única | conservar | SRI existente |
| `pdf.worker.min.js` CDN | 3.4.120 | código de worker | PDF.js | al renderizar | mal cargado como script | retirar etiqueta y asignar a `workerSrc` | SRI de etiqueta actual no aplica al Worker |
| `pdf.sandbox.min.js` CDN | 3.4.120 | sandbox PDF.js | sin consumidor explícito del visor | no requerido hoy | carga indebida | retirar del frame; no añadir carga hasta existir consumidor | SRI existente |
| `pdf.worker.entry.min.js` CDN | 3.4.120 | entrada de bundler/worker | ninguno en el frame | nunca | carga indebida | retirar; elimina error DOC-100 | SRI existente |
| Fabric CDN | 4.3.0 | `window.fabric` | `_page_anotate`, `extend_fabric.js` | anotación que lo requiera | opcional anticipada | cargar bajo demanda | sin SRI actual |
| jsPDF CDN | 2.2.0 | `window.jspdf` | no se encontró consumidor | nunca | carga sin propietario | retirar; reintroducir solo con consumidor probado | sin SRI actual |
| Prettify RawGit loader | master | `PR` | `showPdfData` | diagnóstico | duplicado y remoto obsoleto | retirar | sin SRI |
| Prettify CDN | r298 | `PR.prettyPrint` | `showPdfData` | diagnóstico | duplicado | cargar bajo demanda como propietario único | sin SRI actual |
| `jquery.contextMenu.js/css` | local | plugin `contextMenu` | `context_menu` | grafo/firma | opcional anticipada | cargar con capacidad grafo | local |
| `jquery-ui-touch-punch` CDN | 0.2.3 | puente táctil jQuery UI | interacción táctil de grafo | grafo táctil | opcional anticipada | cargar con capacidad grafo | sin SRI actual |
| cadena `pdfjs/wacom/*` | local | SDK, USB, cifrado y diálogos | `captureFromSTU` | clic Wacom | `signature_sdk.js` y `wgssStuSdk.js` duplicados | quitar etiquetas iniciales y cargar una vez en orden | local; declarada en `.vbproj` |
| `printer_js/print.min.js/css` | local | `printJS` | `_PrinterPdf` | clic imprimir | opcional anticipada | cargar bajo demanda | local; declarada en `.vbproj` |
| `printer_js/dataURItoBlob.js` | local | conversión auxiliar | sin uso directo identificado en impresión actual | solo si la capacidad lo requiere | opcional anticipada | incluir únicamente en manifiesto probado | local |
| `service_configuracion_itex_stamp_user` | WebMethod existente | configuración de sello | `config_stamp()` | firma/grafo | llamada anticipada | invocar al activar firma/grafo, no al abrir | contrato existente |

## Decisions

### D-01 — Camino crítico explícito

El HTML conservará en el arranque únicamente estilos de layout/toolbar, jQuery 3.5.1, Bootstrap necesario para la toolbar, un propietario visual de Font Awesome, `pdf.min.js`, `pdf_viewer_anotate.js` y `script.js`. Las pruebas estáticas mantendrán una lista cerrada para impedir que capacidades opcionales regresen al camino crítico.

### D-02 — Frontera correcta de PDF.js

`pdf.min.js` seguirá siendo el único script PDF.js del frame. Antes del primer `getDocument`, `script.js` asignará `pdfjsLib.GlobalWorkerOptions.workerSrc` a la misma versión 3.4.120. Se retirarán las etiquetas de worker, sandbox y worker-entry. No se cambia la versión ni el planificador DOC-100.

### D-03 — Propietario único por familia

- jQuery UI: se conserva el recurso local 1.12.1 porque está emparejado con su CSS y cubre `draggable`/`resizable`; se retiran 1.13.2 y 1.8.22.
- Popper: Bootstrap 5.3.2 bundle será el único propietario; se retiran 1.11.0 y 1.16.1.
- Font Awesome: `all.js` será el único runtime JavaScript; se retiran `brands.js` y `solid.js` tras validar iconos `fal`, `fad`, `far` y dropdown.
- Prettify: se conserva r298 solo bajo demanda; se retira el loader RawGit.
- jsPDF: se retira porque no existe un consumidor en los scripts activos.

### D-04 — Cargador memoizado por capacidad

`script.js` será el propietario del registro `viewerCapabilities`. Cada entrada declarará CSS, scripts ordenados y símbolo final esperado. `loadViewerCapability(name)` devolverá la misma promesa durante cargas concurrentes, omitirá recursos ya presentes, resolverá solo al existir el símbolo esperado y eliminará su promesa fallida para permitir reintento.

No se crea un framework ni un segundo bootstrap. La API es interna; las firmas públicas de `PDFjsViewer` permanecen.

### D-05 — Puntos exactos de activación

| Capacidad | Activador | Recursos/operación diferida | Símbolo final |
| --- | --- | --- | --- |
| Wacom | `pdfTool_viwer_captureFromSTU` y menú contextual | cadena local Wacom sin duplicados | `captureFromSTU` |
| Impresión | `pdfTool_viwer_Printer` | `print.min.css`, `print.min.js`; auxiliar solo si se demuestra uso | `printJS` |
| Fabric | primera operación que invoque `_page_anotate`/`_page_fabric` | Fabric 4.3.0 y extensión realmente usada | `fabric.Canvas` |
| Prettify | `showPdfData` | Prettify r298 | `PR.prettyPrint` |
| Grafo | `pdfTool_parameter_grafo`, firma personal o menú contextual según el flujo | jQuery UI 1.12.1, contextMenu, touch-punch y WebMethod correspondiente | `$.fn.draggable`, `$.fn.contextMenu` |

La llamada incondicional `pdfViewer.config_stamp()` se elimina de `load_pdf_viwer_anotation`. La configuración se obtiene antes de la primera acción que la necesita y se reutiliza durante la sesión del visor.

### D-06 — Fallo localizado y reintento

Los handlers esperarán el cargador antes de invocar símbolos globales. Si falla, mostrarán un mensaje mediante el mecanismo visual existente, restaurarán el control para reintento y no modificarán `pdfViewer.pdf`, páginas, scroll ni zoom. El error seguirá llegando al runner saneado como recurso opcional; no habrá `catch` vacío.

### D-07 — Compatibilidad, seguridad y despliegue

No se añaden orígenes remotos ni upgrades mayores. Se mantienen los atributos SRI/crossorigin existentes en recursos que permanecen. Cualquier archivo local nuevo debe registrarse en `GestionDocumental-Docuarchi.net.vbproj`; si el cargador queda en `script.js`, el proyecto no necesita una entrada nueva. La discrepancia Bootstrap JS 5.3.2/CSS 5.1.3 se documenta como deuda y no se corrige sin pruebas de migración propias.

### D-08 — Evidencia y umbral

El runner existente añadirá conteos saneados de solicitudes del núcleo y de capacidades opcionales, sin guardar URL completa ni datos documentales. La aceptación exige hitos funcionales completos, ausencia del error `pdf.worker.entry.min.js`, cero solicitudes opcionales antes de activarlas y mejora de la primera apertura frente a 1.679 ms de `domInteractive`; las recargas se comparan contra 92–98 ms sin convertir esos valores en un umbral artificial.

## Risks / Trade-offs

- La elección de jQuery UI 1.12.1 reduce duplicidad, pero debe probar `draggable`, `resizable`, contextMenu y touch-punch.
- Retirar runtimes Font Awesome puede descubrir estilos implícitos; el contrato visual debe cubrir todos los prefijos usados.
- La primera activación de Wacom o impresión asumirá su costo de descarga; se mostrará estado de carga y los clics concurrentes compartirán promesa.
- `workerSrc` remoto conserva la disponibilidad del CDN existente; una migración a worker local queda fuera de alcance salvo que la evidencia demuestre que el archivo local desplegado es equivalente.
- Un recurso opcional puede fallar por red o CSP; el visor básico debe permanecer estable y permitir reintento.

## Migration Plan

1. Añadir contratos estáticos que describan el camino crítico y fallen con el HTML actual.
2. Corregir la frontera PDF.js y consolidar una familia por cambio verificable.
3. Incorporar el cargador interno y migrar una capacidad por vez: Wacom, impresión, Fabric/Prettify y grafo.
4. Ejecutar pruebas Node y validación sintáctica después de cada grupo.
5. Con autorización vigente, ejecutar la matriz E2E y tres iteraciones de lectura.
6. Documentar inventario antes/después, métricas y riesgos; revertir por grupo de capacidad si aparece regresión.

## Rollback

Los cambios se mantienen separados por familia. El rollback restaura únicamente las etiquetas o el activador de la capacidad afectada; no revierte el planificador DOC-100 ni cambia transporte/persistencia. Si falla el worker, se restaura temporalmente la configuración previa de PDF.js sin volver a ejecutar `pdf.worker.entry.min.js` en el frame.

## Open Questions

- Ninguna decisión de producto pendiente. La implementación debe confirmar mediante pruebas la compatibilidad de los propietarios seleccionados antes de retirar cada duplicado.

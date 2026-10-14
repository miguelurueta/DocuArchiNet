## Why

El visor enriquecido descarga hoy el PDF completo mediante `fetch -> Blob -> FileReader -> Uint8Array` antes de entregarlo a PDF.js. A la vez, `workflow/Handler_image_wf.ashx.vb` acepta una ruta física enviada por el navegador, materializa el archivo completo con `ClassRaEnvioCorrespondencia.ReadFile` y mantiene un bloqueo exclusivo de sesión. Este contrato aumenta tiempo a primera página, memoria y superficie de exposición de rutas.

## What Changes

- Introducir una referencia documental opaca, protegida, expirable y ligada a la sesión que emitió la URL.
- Resolver esa referencia en el servidor antes de abrir el archivo, sin aceptar una ruta física nueva como autoridad del flujo enriquecido.
- Convertir `Handler_image_wf.ashx` en un streamer por bloques con `GET`, `HEAD`, un único rango HTTP, validadores condicionales y errores públicos saneados.
- Permitir solicitudes Range concurrentes usando sesión de solo lectura y sin escribir estado desde el handler.
- Entregar a PDF.js la URL autorizada directamente, seleccionando rangos para PDF grandes linearizados y streaming secuencial para PDF no linearizados o pequeños, sin precarga completa en JavaScript.
- Conservar temporalmente `rut_image` como adaptador de compatibilidad para consumidores simples inventariados, sin otorgarle las capacidades nuevas de rango o caché autorizada.
- Añadir pruebas focales del contrato HTTP, regresión estática de consumidores, MSBuild y E2E autorizada con bytes transferidos y tiempo a primer canvas.
- Ejecutar una prueba de concepto local y descartable de linearización, sin reemplazar documentos ni cambiar el almacenamiento, para medir con el PDF.js existente los bytes requeridos hasta el primer canvas y delimitar una posible evolución posterior.

## Capabilities

### New Capabilities

- `streamer-http-rangos`: entrega privada y autorizada de documentos mediante streaming y rangos HTTP para el visor PDF enriquecido.

### Modified Capabilities

- Ninguna.

## Impact

- Handler y contrato HTTP: `workflow/Handler_image_wf.ashx.vb`.
- Referencia autorizada: nuevo componente VB.NET registrado en `GestionDocumental-Docuarchi.net.vbproj`.
- Emisión de URL enriquecida: `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb` y `Gestion/ClassGaProducionDocumental.vb`.
- Cliente PDF.js: `pdfjs/pdf_anotate_view/script.js`; `pdf_viewer_anotate.js` conserva una sola instancia y selecciona entre dos fuentes URL del mismo handler.
- Compatibilidad: consumidores simples del parámetro `rut_image`, descarga, impresión, firma, sello, aprobación, versiones y visores externos.
- Sin cambios de tablas, columnas, almacenamiento externo, CDN ni guardado de grafos.
- Prueba experimental: `tools/e2e/scripts/run-doc102-linearization-poc.cjs`, Ghostscript instalado en la estación y archivos exclusivamente temporales.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: HTTP, RANGOS, STREAMING

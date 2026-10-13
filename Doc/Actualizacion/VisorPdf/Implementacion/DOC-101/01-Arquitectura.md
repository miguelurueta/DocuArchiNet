# Arquitectura

El arranque queda dividido entre un núcleo mínimo y cinco capacidades opcionales. `script.js` es el único propietario del manifiesto y del cargador; `PDFjsViewer` conserva las operaciones de dominio y verifica también Wacom, impresión y Fabric para proteger llamadas legacy directas.

## Núcleo de arranque

El orden es jQuery 3.5.1, PDF.js 3.4.120, `pdf_viewer_anotate.js` y `script.js`. Bootstrap bundle mantiene dropdown y Popper. Font Awesome usa un solo runtime `all.js`. `pdf.min.js` es el único artefacto PDF.js ejecutado en el frame y `GlobalWorkerOptions.workerSrc` apunta al worker 3.4.120.

## Capacidades

| Nombre | Activador | Recursos | Condición de listo |
| --- | --- | --- | --- |
| `graph` | parámetros, firma o primer menú contextual | jQuery UI local 1.12.1, contextMenu y touch-punch opcional | `draggable`, `resizable`, `contextMenu` |
| `wacom` | firma de tableta con permiso | cadena local ordenada | `captureFromSTU` |
| `print` | impresión con permiso | Print.js CSS/JS | `printJS` |
| `fabric` | anotación basada en canvas | Fabric 4.3.0 | `fabric.Canvas` |
| `prettify` | detalle diagnóstico | Prettify r298 | `PR.prettyPrint` |

`loadViewerCapability(name)` conserva una promesa por capacidad, deduplica nodos por URL normalizada, inserta scripts en orden y borra solo la promesa fallida. La configuración de sello usa otra promesa memoizada y ya no se consulta al abrir un documento solo para lectura.

## Propietarios retirados

Se retiraron del arranque jQuery UI 1.13.2/1.8.22, Popper 1.11/1.16, Font Awesome `brands.js`/`solid.js`, worker/sandbox/worker-entry como scripts normales, jsPDF sin consumidor, el loader RawGit de Prettify y duplicados Wacom.

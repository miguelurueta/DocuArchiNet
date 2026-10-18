# DOC-105 — Impacto UI

- Ticket: DOC-105
- Cambio OpenSpec: doc-105-regresion-transversal
- Clasificacion: cross_cutting

Cambio: `doc-105-regresion-transversal`. Impacto: `cross_cutting`.

## Superficies UI

DOC-105 modifica HTML/JS/CSS del visor compartido: placeholders de contador, bloqueo durante preview, controles completos de zoom/rotación, espera single-flight de descarga/impresión, posición inicial y controles opacos del grafo. Producción reserva cabecera y área visible del modal. Automatiza recorridos existentes:

- `workflow/Webworkflow.aspx`: selección oficial de tarea y documento.
- `Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx`: documento relacionado en ventana nativa de solo lectura; la llamada existente anula el visor express y carga `Handler_image_wf.ashx`.
- `Gestion/WebFormProducionDocumental.aspx`: fila de `data_grid` y `ButtonVerDocumento`.
- `Gestion/WebFormGaVisorClasificacion.aspx`: iframe de clasificación/aprobación.
- Páginas de preview/descarga que navegan al handler.

## Validacion visual

El medidor observa primer canvas, scroll y zoom únicamente cuando PDF.js aplica. Para Correspondencia y handler simple esas interacciones son `NOT_APPLICABLE`; se verifican iframe, autorización y transporte. La validación manual de firma/tableta no se sustituye con simulación. Aceptaciones de videos y pruebas DOM se detallan en `Doc/Actualizacion/VisorPdf/Implementacion/DOC-105/03-PruebasLocales.md`. La E2E focal Workflow final pasa; esto no acredita guardado instrumentado ni impresión física. Prioridad de preview limitada a tres segundos con fallback; toolbar habilitada tras render principal.

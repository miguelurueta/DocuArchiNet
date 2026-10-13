# ARRANQUE-CARGA-DIFERIDA

- Ticket: DOC-101
- Cambio OpenSpec: doc-101-arranque-carga-diferida
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Reducir el trabajo previo al primer canvas. `index.html` conserva el núcleo; `script.js` incorpora un cargador memoizado para grafo, Wacom, impresión, Fabric y Prettify; `pdf_viewer_anotate.js` protege entradas legacy directas.

## Alcance y compatibilidad

- Afecta exclusivamente al iframe `pdfjs/pdf_anotate_view`, al runner de rendimiento y a sus pruebas.
- Preserva lectura, scroll, zoom, rotación, descarga, firmas e impresión.
- No cambia backend, WebMethods, payloads, sesión ni persistencia.
- El rollback está separado por worker, familia consolidada y manifiesto opcional.

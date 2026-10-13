# ARRANQUE-CARGA-DIFERIDA

- Ticket: DOC-101
- Cambio OpenSpec: doc-101-arranque-carga-diferida
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

No cambian handler, endpoint, payload, autenticación ni esquema. PDF.js 3.4.120 queda dividido correctamente: `pdf.min.js` corre en el frame y `pdf.worker.min.js` se declara en `GlobalWorkerOptions.workerSrc`.

El runner registra únicamente conteos `core`, `graph`, `wacom`, `print`, `fabric`, `prettify` y `other`; no persiste URL, query, tarea, documento, usuario o secretos.

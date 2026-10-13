# ARRANQUE-CARGA-DIFERIDA

- Ticket: DOC-101
- Cambio OpenSpec: doc-101-arranque-carga-diferida
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy`: 28/28, 2026-10-13.
- [x] `node --check` sobre los tres JavaScript modificados: aprobado, 2026-10-13.
- [x] E2E autenticada de rendimiento: `PASSED`, tres iteraciones, mediana 519 ms, cero fallos y cero solicitudes opcionales.
- [x] Matriz no persistente: `PASSED`; firma retirada sin guardar, Wacom fallido/reintentado, SDKs y lectura estables.

## QA/E2E WebForms

El runner automatizado cubrió lectura, scroll, zoom y tres iteraciones mediante consola interactiva, sin persistir credenciales. `domInteractive` inicial fue 389 ms frente a 1.679 ms y la mediana de primer canvas fue 519 ms frente a 878 ms. La matriz opcional no persistió cambios: el único fallo fue el aborto intencional de `q.js`; la recuperación del SDK Wacom pasó, pero no hubo prueba con tableta física.

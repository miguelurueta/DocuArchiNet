# LISTA-PREVIEW-INCRIPCIONES

- Ticket: DOC-69
- Cambio OpenSpec: doc-69-lista-preview-incripciones
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit`: el 2026-09-19, `node --test tests/importar-servicio-web-*.test.cjs` aprobó 355/355 y `npm.cmd --prefix tools/e2e run test:doc56:policy` aprobó 17/17.
- `build`: `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal` terminó con código 0; persisten advertencias heredadas de binding redirects.
- `manual_qa`: E2E real autorizada confirmó HEAD, una entrega entre dos GET concurrentes, rechazo de alteración/reutilización, expiración 404, siete controles SELECT sin mutación y restauración del gate.
- `persistencia`: prueba aislada MySQL 5.1 confirmó 16 columnas, 4 índices, cero FK y cero tablas temporales después del rollback.

## QA/E2E WebForms

Se reutilizó `tools/e2e` con el escenario `import-sii-read` y perfiles DOC-69. La telemetría SELECT confirmó exactamente una solicitud de token, una consulta de sello y una descarga de anexo; HEAD y GET no llamaron al proveedor. La evidencia pública conserva únicamente conteos y resultados, sin credenciales, descriptor, contenido ni datos personales. Esta validación demuestra el recurso descartable probado, no todos los navegadores ni topologías futuras de granja.

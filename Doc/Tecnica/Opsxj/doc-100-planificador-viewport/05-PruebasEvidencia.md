# PLANIFICADOR-VIEWPORT

- Ticket: DOC-100
- Cambio OpenSpec: doc-100-planificador-viewport
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

El 2026-10-13 se aprobaron 14/14 pruebas con `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy`, cuatro validaciones `node --check`, OpenSpec estricto y `git diff --check`.

## QA/E2E WebForms

Se ejecutaron dos E2E autorizadas con secretos ingresados únicamente por TTY. La segunda corrida confirmó bloqueo de scroll -21,8 % y bloqueo de zoom -76,1 % frente a línea base. El resultado sigue `FAILED` por `pdf.worker.entry.min.js`, propiedad de DOC-101, sin ocultar el error. Detalle: `Doc/Actualizacion/VisorPdf/Implementacion/DOC-100/05-PruebasEvidencia.md`.

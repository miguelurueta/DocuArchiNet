# PLANIFICADOR-VIEWPORT

- Ticket: DOC-100
- Cambio OpenSpec: doc-100-planificador-viewport
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

La superficie afectada es el canvas y los esqueletos de página del visor PDF. No cambian botones, IDs DOM, WebForms, modales, foco, estilos ni accesibilidad. Se preservan página activa, scroll proporcional, zoom, rotación, overlays y anotaciones.

## Validacion visual

Dos corridas reales de tres iteraciones completaron selección, primer canvas, scroll, zoom y recargas sobre 34 páginas. La segunda redujo 21,8 % el bloqueo de scroll y 76,1 % el de zoom. El resultado global siguió `FAILED` por una dependencia asignada a DOC-101, no por pérdida visual del planificador.

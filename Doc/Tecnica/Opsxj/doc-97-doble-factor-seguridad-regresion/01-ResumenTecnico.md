# DOBLE-FACTOR-SEGURIDAD-REGRESION

- Ticket: DOC-97
- Cambio OpenSpec: doc-97-doble-factor-seguridad-regresion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Auditar la seguridad y no regresión del Login 2FA entregado por DOC-91 a DOC-96 mediante una matriz transversal reproducible. No se reprodujo un defecto productivo; se añadieron pruebas, evidencia y CI.

## Alcance y compatibilidad

- [x] Superficies identificadas: `gestor.aspx*`, ASMX, servicios/repositorios 2FA, JavaScript, pruebas y CI.
- [x] Se preservan firmas, flujo OFF/NULL, `ClassCorreo` y recuperación. La reversa consiste en revertir pruebas/CI; no hubo cambio productivo ni DDL.

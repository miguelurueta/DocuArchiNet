# DOC-124 — 02-ImpactoUI

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

Fase04 añade recorrido de servicios ASHX/ASMX, sin pantalla o menú nuevo. La UI de fase06 consumirá ID previo, estados, muestras y consulta propia. No se extiende la interfaz de carpetas para cargar tablas.

## Validacion visual

La validación visual no aplica porque esta fase no añade pantalla. La E2E de servicios sí aplica: tools/e2e reutilizado aprobó1/1 del archivo Órdenes, con recepción/preparación y dos sesiones reales. La matriz completa sigue pendiente; el descubrimiento sintético se mantiene separado de la evidencia IIS/MySQL. No se presenta este recorrido como aceptación de una UI nueva o de todos los modos.

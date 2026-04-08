## Why

IMPLEMENTACION-NAVEGACION-GESTION-CORRESPONDENCIA-03-FE. PROMPT ARQUITECTÓNICO — Actualizar pruebas de regresión para navegación Gmail

## What Changes

- Se consolida el contrato de regresion de la navegacion tipo Gmail ya implementada en `gestion-correspondencia`.
- Se actualizan proposal, spec y pruebas para reflejar el shell real del modulo: bandeja principal montada, panel superpuesto, retorno visible y URL como fuente de verdad.
- Se elimina ambiguedad residual asociada al patron anterior de `Drawer` o modal.
- Se deja una base coherente para continuar con design, specs y tasks sin reabrir el rediseño del shell.

## Capabilities

### Modified Capabilities
- `gestion-correspondencia`: consolidar pruebas de regresion y lenguaje contractual del shell de navegacion Gmail ya implementado.

## Impact

- Ajustes en pruebas de routing y regresion del modulo `gestionCorrespondencia`.
- Sin cambios de arquitectura base ni nuevos endpoints.
- Riesgo reducido de regresion hacia `Drawer`, modal o reemplazo total de la bandeja principal.

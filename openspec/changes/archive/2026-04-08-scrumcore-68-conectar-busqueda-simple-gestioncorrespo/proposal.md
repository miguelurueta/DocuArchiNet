## Why

CONECTAR-BUSQUEDA-SIMPLE-GESTIONCORRESPO. PROMPT ARQUITECTONICO Ticket 03 FE

## What Changes

- Se consolida el contrato de busqueda simple de `GestionCorrespondencia`.
- Se garantiza que el mapper del modulo envie `SearchType = 2` cuando `search` tenga texto efectivo.
- Se preserva `SearchType = 3` para busqueda avanzada.
- Se evita tocar el mapper compartido de `AppTable`.
- Se valida que tabla, `getAllMatchingRows` y exportacion backend reutilicen el mapper del modulo.

## Capabilities

### New Capabilities
-

### Modified Capabilities
- `gestion-correspondencia`

## Impact

- Delta spec sobre `openspec/specs/gestion-correspondencia/spec.md`.
- Cobertura focal del mapper de request de `GestionCorrespondencia`.
- Sin cambios esperados en `AppInputSearch`, endpoint backend, autorizacion, claims ni mapper compartido de `AppTable`.

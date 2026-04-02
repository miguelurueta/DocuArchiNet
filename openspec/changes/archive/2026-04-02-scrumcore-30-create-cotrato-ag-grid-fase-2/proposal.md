## Why

La Fase 1A dejo `AppTable` como componente base reusable sobre AG Grid. Este ticket implementa la Fase 1B: una capa transversal de contratos y adaptacion para transformar el payload dinamico real del backend hacia estructuras internas de grid, sin introducir React, HTTP ni logica de dominio.

## What Changes

- Se modelan los contratos backend `DynamicUiTableDto`, `UiColumnDto`, `UiRowDto`, `UiActionDto`, `UiCellActionDto`, `Pagination` y `Sorting`.
- Se implementan adapters puros para columnas, filas, acciones y el ensamblado completo a `AppDataTableAgGrid`.
- Se documenta la estrategia de normalizacion y compatibilidad con el payload real del backend.
- Se agregan pruebas unitarias que cubren prioridades de field, columnas ocultas, orden, filtros, rows vacias, `Meta` y `CellActions.Action`.

## Capabilities

### New Capabilities
- `create-cotrato-ag-grid-fase-2`: capa transversal de contratos y adaptacion para Dynamic UI backend -> AppDataTableAgGrid.

### Modified Capabilities
- 

## Impact

- Nuevos tipos, adapters, tests y documentacion en `src/app/Components/UI/AppTable/`.
- `AppTable` permanece desacoplado y sin cambios de contrato publico.

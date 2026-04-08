## Why

CREA-COMPONENTE-APPINPUTTAGS-FE-02. # PROMPT ARQUITECTÓNICO  Ticket 02 FE

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-64.
- Se formaliza la propuesta OpenSpec para evolucionar `AppInputTags` con autocomplete generico y acciones secundarias.
- Se modifica la capability existente `app-input-tags` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
-

### Modified Capabilities
- `app-input-tags`: `AppInputTags` debe integrarse con contratos genericos de autocomplete, `loading`, `options`, `minLength`, `debounceMs` y slots/acciones desacopladas.

## Impact

- Evolucion del componente compartido en `src/app/Components/UI/AppInputTags/`.
- Posible integracion con `useAutocompleteCamposPlantilla` o cualquier hook que entregue opciones normalizadas.
- Nuevas pruebas de comportamiento para el contrato reusable del componente y sus consumidores.

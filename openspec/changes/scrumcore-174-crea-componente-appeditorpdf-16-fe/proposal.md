## Why

CREA-COMPONENTE-APPEDITORPDF-16-FE. # Ticket 16 FE - Zoom visual UI

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-174.
- Se formaliza la propuesta OpenSpec para implementar zoom visual en `AppEditorPdf` a partir del ticket Jira.
- Se define la capability `app-appeditorpdf-16-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appeditorpdf-16-fe`: Extension de `AppEditorPdf` para zoom visual UI (sin modificar HTML persistido).

### Modified Capabilities
- 

## Impact

- Cambios compartidos en `src/app/Components/UI/AppEditorPdf/` (sin acoplarse a `src/modules/...`).
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.

## Why

CREA-COMPONENTE-APPEDITORPDF-15-FE. # Ticket 15 FE - Hardening modo multi-hoja

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-173.
- Se formaliza la propuesta OpenSpec para hardening del modo multi-hoja en `AppEditorPdf` a partir del ticket Jira.
- Se define la capability `app-appeditorpdf-15-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appeditorpdf-15-fe`: Extension de `AppEditorPdf` para hardening del modo multi-hoja (estabilidad y performance).

### Modified Capabilities
- 

## Impact

- Cambios compartidos en `src/app/Components/UI/AppEditorPdf/` (sin acoplarse a `src/modules/...`).
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.

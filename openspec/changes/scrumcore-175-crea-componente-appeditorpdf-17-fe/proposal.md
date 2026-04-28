## Why

CREA-COMPONENTE-APPEDITORPDF-17-FE. # Ticket 17 FE - Boton Guardar y dirty state

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-175.
- Se formaliza la propuesta OpenSpec para agregar boton Guardar y dirty state en `AppEditorPdf` a partir del ticket Jira.
- Se define la capability `app-appeditorpdf-17-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appeditorpdf-17-fe`: Extension de `AppEditorPdf` para boton Guardar y estado dirty (sin acoplarse a modulos).

### Modified Capabilities
- 

## Impact

- Cambios compartidos en `src/app/Components/UI/AppEditorPdf/`.
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.

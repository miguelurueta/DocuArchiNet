## Why

CREA-COMPONENTE-APPEDITORPDF-19-FE. # Ticket 19 FE - Ajuste de listas, margenes y remocion de wrapper

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-177.
- Se formaliza la propuesta OpenSpec para ajustar listas, margenes y remover wrapper innecesario en `AppEditorPdf` a partir del ticket Jira.
- Se define la capability `app-appeditorpdf-19-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appeditorpdf-19-fe`: Extension de `AppEditorPdf` para ajustes de listas/margenes y simplificacion de wrapper.

### Modified Capabilities
- 

## Impact

- Cambios compartidos en `src/app/Components/UI/AppEditorPdf/`.
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.

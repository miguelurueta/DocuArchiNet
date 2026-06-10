## Why

CREA-COMPONENTE-APPEDITORPDF-20-FE. # Ticket 20 FE - Implementacion visual en GestionRespuesta (Tab Documentos)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-178.
- Se formaliza la propuesta OpenSpec para implementar visual de "Tab Documentos" en `GestionRespuesta` usando UI shared (sin duplicar componente) a partir del ticket Jira.
- Se define la capability `app-appeditorpdf-20-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appeditorpdf-20-fe`: Integracion/estandarizacion visual para Tab Documentos en GestionRespuesta (sin crear componente fuera de `AppEditorPdf` si no aplica).

### Modified Capabilities
- 

## Impact

- Ajustes de integracion visual en `src/modules/gestionCorrespondencia` (Tab Documentos) y/o reuso de `AppEditorPdf` donde corresponda.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.

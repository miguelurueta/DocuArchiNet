## Why

CREA-COMPONENTE-APPVISORPDF-04-FE. # Ticket 04 FE

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-193.
- Se formaliza la propuesta OpenSpec para implementar AppAppvisorpdf04Fe a partir del ticket Jira.
- Se define la capability `app-appvisorpdf-04-fe` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-appvisorpdf-04-fe`: Componente reusable AppAppvisorpdf04Fe para la capa UI compartida del proyecto.

### Modified Capabilities
- 

## Impact

- Cambios en `AppVisorPdf` (capability `app-appvisorpdf-04-fe`) dentro de `src/app/Components/UI/AppVisorPdf/`.
- NO se integra en m\u00f3dulos/pantallas consumidoras como parte de este ticket; solo se crea/expone funcionalidad reusable.
- Nuevas pruebas para el contrato reusable del componente/engine.

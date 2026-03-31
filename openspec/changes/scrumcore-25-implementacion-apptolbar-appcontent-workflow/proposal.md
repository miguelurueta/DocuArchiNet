## Why

IMPLEMENTACION-APPTOLBAR-APPCONTENT-WORKFLOW. PROMPT PROFESIONAL - Implementar AppToolbar + AppContent en el modulo Workflow con UI responsive y preparada para contenido complejo.

## What Changes

- Se actualiza `Workflow.tsx` para componer la UI con `AppToolbar` y `AppContent` en el orden obligatorio.
- Se ajusta el layout del modulo para mantener solo estructura y delegar la composicion a la pagina.
- Se agregan estilos responsivos en CSS Modules para toolbar y contenido.
- Se prepara el espacio para tablas (AG Grid / Ant Design) sin logica de negocio.

## Capabilities

### New Capabilities
- `implementacion-apptolbar-appcontent-workflow`: Composicion UI del modulo Workflow con toolbar superior, contenido scrollable y soporte responsive.

### Modified Capabilities
- 

## Impact

- Cambios en `src/modules/Workflow/pages/Workflow.tsx` y `src/modules/Workflow/style/Workflow.module.css`.
- Ajustes de estructura en `src/modules/Workflow/layout/WorkflowLayout.tsx` para respetar responsabilidades.

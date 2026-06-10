# SCRUMCORE-217 - Implementación Detallada

## Scope

Se implementa `DocumentosWorkbench` como consumidor backend-driven del contrato `SCRUM-205 ListaDocumentosRadicados`, usando `AppTreeTable` (wrapper sobre `AppTable`) y preservando el layout visor + rail.

## Capas y archivos

### Components
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
  - Mantiene layout existente (visor izquierda, rail derecha).
  - Wiring de `AppTreeTable` (`load/loadChildren/onSelectRow/onActionTriggered`).
  - Estado `activeFileUrl` para alimentar `AppVisorEmbedPdf`.

### Hooks
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
  - Orquesta `load` (root) y `loadChildren` (children).
  - Mantiene `latestRowRef` para resolver meta/values al ejecutar acciones.
  - Ejecuta `ver_documento` mediante action + resolve y retorna `fileUrl` al componente.
  - Expone `getTableColumns()` para columnas dinámicas (desde Config backend).

### Services
- `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`
  - `queryListaDocumentosRadicados` (endpoint query SCRUM-205).
  - `actionListaDocumentosRadicados` (endpoint action SCRUM-205).
  - `resolveDocumentoVisualizacion` (resolve de visor).

### Adapters
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
  - Construye request root y children (campos mínimos SCRUM-205).
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
  - Mapea `Rows[]` a `AppTreeTableRow` (id/label/values/meta/hasChildren).
  - Si existe `Config`, intenta convertirlo a columnas AG Grid (`ColDef[]`) usando adaptadores existentes de Dynamic UI.
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`
  - Construye request de action (`ActionId`, `RowId`, `NodeType`, `Payload.*`) para `ver_documento`.

### UI Wrapper
- `src/app/Components/UI/AppTreeTable/types.ts`
  - Extiende API pública con `onCellClicked`, `onActionTriggered` y `tableColumns` (override).
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
  - Reenvía `onCellClicked` y `onActionTriggered` desde `AppTable`.
  - Permite override `tableColumns` para columnas backend-driven (Dynamic UI).

## Wiring de eventos (antes/después)

Antes:
- `DocumentosWorkbench` renderizaba `AppTreeTable` sin integración real de SCRUM-205 ni actualización del visor.

Después:
- `load/loadChildren` consumen query SCRUM-205 (root/children).
- Click principal: `onSelectRow` → ejecuta `ver_documento` y actualiza `fileUrl` del visor.
- Menú secundario: `onActionTriggered` → permite acciones backend-driven (extensible).

## Consideraciones de errores

- `load/loadChildren` retornan `{ ok:false, message }` para que el wrapper muestre error y retry.
- Para `ver_documento`, si falla el action/resolve o falta metadata requerida, NO se cambia el documento activo.

## Próximos pasos

- Completar integración completa de acciones dinámicas (más allá de `ver_documento`) según metadata SCRUM-205.
- Añadir E2E Playwright y documentación de evidencia de ejecución.

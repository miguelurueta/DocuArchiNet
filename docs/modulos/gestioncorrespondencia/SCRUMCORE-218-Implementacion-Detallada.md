# SCRUMCORE-218 - Implementación Detallada

## Scope

Se implementa la normalización del contrato frontend para `DocumentosWorkbench` corrigiendo el render/disparo de acciones cuando el backend retorna `DynamicUiTableDto` en `data` directo o en `data.Config`.

## Capas y archivos

### Adapters
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
  - `pickDynamicUiTable` ahora acepta:
    - `data.Config` (legacy)
    - `data` directo con shape de tabla dinámica (columns/actions/tableId)
  - Mantiene inferencia existente de columnas y vista `flatDocuments`.

- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`
  - Amplía input compatible con `idDocumento?`.
  - Payload normalizado:
    - Prioriza `Payload.IdDocumento` cuando existe.
    - Usa `Payload.DocumentId` cuando `IdDocumento` no está.
    - Conserva `Payload.NombreGabinete` cuando viene.

### Hooks
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
  - `tableIdRef` se sincroniza en cada `load/loadChildren` con `model.tableId` o fallback `InboxListaDocumentosRadicado`.
  - `load()` asegura `columns` aún sin `model.columns` usando inferencia desde filas.
  - `buildActionContextFromRow` lee identificadores desde `Values` y `Meta`:
    - `IdDocumento` (prioritario)
    - `DocumentId` (fallback)
  - `performAction` usa siempre `TableId` efectivo + `RowId` + payload normalizado.

### Request mapper (sin cambios)
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
  - Se mantiene `IncludeConfig: true` en query root (compatibilidad contractual).

## Wiring de eventos (antes/después)

Antes:
- Si backend enviaba tabla en `data` directo, podía no mapearse configuración dinámica y no renderizar acciones de menú en `flatDocuments`.

Después:
- Se mapea configuración dinámica en ambos shapes.
- La columna de acciones mantiene `CellActions`/`MenuActions` (fallback `RowActions`).
- `onActionTriggered` recibe acciones renderizadas y ejecuta request con `TableId` correcto y payload consistente.

## Consideraciones de errores

- Se mantiene comportamiento de error actual en `load/loadChildren` y acción.
- No se altera manejo de servicios HTTP ni semántica de rutas.

## Compatibilidad

- Compatible con respuestas legacy (`data.Config`).
- Compatible con respuestas actuales (`data` directo).
- Sin cambios en backend.

# SCRUMCORE-221 - Integracion BackEnd

## Endpoint consultado indirectamente desde contexto

Este cambio NO introduce endpoints nuevos.

- `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
  - Resolución ejecutada por `solicitaGabineteRadicadoWorkflow.service.ts`.
  - Consultas realizadas por `GestionRespuestaDocumentosProvider` (SCRUMCORE-220), no por `useListaDocumentosRadicadosTreeTable`.

## Endpoints documentales utilizados por este hook

- `queryListaDocumentosRadicados` (POST/GET según implementación del service)
  - Sigue usando `NombreGabinete` resuelto por contexto.
- `actionListaDocumentosRadicados` con `ActionId = "ver_documento"`.
- `resolveDocumentoVisualizacion` para apertura de visor.

## Request/response

`queryListaDocumentosRadicados` conserva shape:

- `TableId`, `ViewMode`, `Page`, filtros y `NombreGabinete`.
- Reutiliza `response.data.Rows` con mapeo existente (`AppTreeTableRow`).

`actionListaDocumentosRadicados` conserva:

- `ActionId: "ver_documento"`.
- `Payload` con `NombreGabinete` + identificadores documentales.

## Integración FE-BE

1. `GestionRespuesta` provee `idTareaWf` y `radicado` desde flujo de estructura.
2. El provider de contexto resuelve `nombreGabinete`.
3. `useListaDocumentosRadicadosTreeTable` consume el contexto y manda `NombreGabinete` en query/actions.
4. Si falla resolución o no hay gabinete:
   - el hook responde error funcional controlado sin romper render ni cambiar endpoint.

## Fallback y retry

- Fallback:
  - sin `idTareaWf` válido: contexto reporta sin gabinete y no consulta documentos.
  - sin `NombreGabinete`: bloquea `load`, `loadChildren` y `onSelectRow`.
- Retry:
  - se delega al contexto: `reloadGabinete()` desde proveedor.
  - al refrescar contexto, el hook reintenta al siguiente `load/onSelectRow`.

## Compatibilidad y contratos

- No se alteró contrato de endpoints.
- No se cambió payload de acción ni estructura de request del documento.
- Se mantiene el contrato publicado del hook y servicios para consumidores existentes.

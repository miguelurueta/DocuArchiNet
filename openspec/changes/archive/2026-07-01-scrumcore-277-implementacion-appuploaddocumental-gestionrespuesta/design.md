## Context

SCRUMCORE-277 implements the real consumer integration for the reusable `AppUploadDocumental` built in the previous scrum. The consumer is `GestionRespuesta`, specifically the Gestion tab. The functional outcome is that a selected file becomes a stored response attachment related to `ra_anexos_respuesta`, indexed through the RADICACION provider, and visible from the Documentos tab after backend reload.

The official contract is `docs/Architecture/AppUploadDocumental/SCRUM-250-Integracion-Frontend-AnexosRespuesta.md` plus the Jira prompt for SCRUMCORE-277. The contract is considered real for this ticket; no external DTO path is required.

## Current State

- `GestionRespuesta.tsx` already wraps Gestion and Documentos tabs with `GestionRespuestaDocumentosProvider`.
- `GestionRespuestaMainTabContent.tsx` currently renders a simple `AppUpload` attachments area.
- `GestionRespuestaDocumentosContext.tsx` shares workflow/gabinete context and simple upload files, but has no document refresh signal.
- `DocumentosWorkbench.tsx` loads the document list through `useGestionRespuestaDocumentosTable(idTareaWf)` and opens documents through the existing embedded PDF viewer.
- `almacenamientoDocumentalUpload.service.ts` already owns the technical storage endpoints and typed errors, but SCRUMCORE-277 requires explicit workflow-anexo contract support, PascalCase mapping, and status-before-complete semantics.

## Goals

- Integrate `AppUploadDocumental` in the Gestion tab through a module adapter named `GestionRespuestaUploadDocumental`.
- Build one final `/api/gestor-documental/almacenamiento` request per file.
- Use per-file tipology metadata to build `Trd`.
- Build `AnexoRespuesta` and `CabinetIndexSeed` exactly from the official contract.
- Keep frontend domain types camelCase while mapping backend requests/responses to/from PascalCase explicitly.
- Validate the full temporary upload lifecycle, including `GET status` before `complete`.
- Refresh `DocumentosWorkbench` from backend after confirmed `AnexoRespuesta.Created === true`.
- Preserve `rawBackendResult` for specialized consumers and diagnostics without exposing sensitive payloads in UI errors.

## Non-Goals

- No backend changes.
- No legacy runtime integration.
- No invented loaders/endpoints for configuration or tipologies.
- No direct DOM/table manipulation in GestionRespuesta.
- No replacement of reusable upload primitives.

## Architecture

```txt
GestionRespuesta
  -> GestionRespuestaDocumentosProvider
     -> GestionRespuestaMainTabContent
        -> GestionRespuestaUploadDocumental
           -> AppUploadDocumental
              -> AppUploadBatchView
              -> AppUpload
              -> AppProgressBatch
           -> gestionRespuestaUploadDocumental.mapper
           -> gestionRespuestaUploadDocumental.service
           -> almacenamientoDocumentalUpload.service
     -> DocumentosWorkbench
        -> useGestionRespuestaDocumentosTable
        -> AppTreeTable
        -> AppVisorEmbedPdf
```

`GestionRespuestaUploadDocumental` is an adapter, not a new upload framework. It supplies context, loaders, request builders, and callbacks to `AppUploadDocumental`.

`GestionRespuestaDocumentosProvider` becomes the cross-tab coordination point by exposing:

```ts
documentosRefreshKey: number;
refreshDocumentos: () => void;
```

`DocumentosWorkbench` observes `documentosRefreshKey` and reloads the document list from backend. It must not insert the new row optimistically as the primary source.

## Contract Decisions

1. The backend contract uses PascalCase. Internal reusable frontend types may stay camelCase, but SCRUMCORE-277 must introduce explicit typed mappers.
2. `chunkIndex` is zero-based.
3. Chunks are raw bytes with `Content-Type: application/octet-stream` and `X-Total-Chunks`.
4. The lifecycle is `init -> chunks -> status -> complete -> almacenar`; `complete` is blocked when `ChunksPendientes` is not empty or received size is invalid when provided.
5. Multiple UI files are processed sequentially because each file can have a different tipology. Each file creates exactly one final storage POST.
6. `Trd.IdTipoDocumento` and `Trd.NombreTipoDocumento` come from the selected per-file tipology metadata.
7. `CabinetIndexSeed` uses `SourceModule = RADICACION`, `ProviderKey = RADICACION`, `Version = 1.0.0`, and `Payload.ModoResolucion = RespuestaRadicado`.
8. `AnexoRespuesta.NombreArchivo` is `file.name` only. Local paths and subdirectories are rejected/sanitized before request building.
9. Successful attachment creation requires `success === true`, valid document fields, and nested `data.AnexoRespuesta.Created === true`.
10. If the backend returns extra fields, preserve them as `rawBackendResult`; do not concatenate legacy pipe-delimited strings.

## Data Model Additions

Add typed frontend models without `any`:

- `AnexoRespuestaStorage`
- `CabinetIndexSeedStorage`
- `WorkflowAnexoStorageResult`
- backend PascalCase request/response DTO types for the mapper layer
- normalized camelCase result consumed by `GestionRespuestaUploadDocumental`

The storage response normalization should support the nested workflow-anexo response required by SCRUMCORE-277. A flat document response may remain supported for existing generic storage behavior, but it is not sufficient to confirm `ra_anexos_respuesta` creation.

## Error Handling

- Missing `nombreGabinete` or invalid `idRespuestaRadicado` blocks storage.
- Missing required tipology blocks the specific file.
- Init failure stops before chunks.
- Chunk failure stops before status/complete/store.
- Status with pending chunks stops before complete.
- Complete failure stops before final storage.
- Final storage failure does not mark the file as stored.
- `success === false` surfaces `errors[0].UserMessage ?? message ?? "Error almacenando anexo"`.
- Cancellation after a temporal upload exists attempts `DELETE upload-temporal`.
- Retry generates a new `RequestId` and does not reuse temporal identifiers.

## Refresh Strategy

When `GestionRespuestaUploadDocumental` receives a confirmed stored result with `AnexoRespuesta.Created === true`, it calls `refreshDocumentos()`. `DocumentosWorkbench` reacts by reloading through the existing backend list service. Once the backend returns the newly stored attachment in the list, the user can select it and use the existing embedded PDF workflow.

This avoids duplicated local state and keeps the backend as the authoritative source for document visibility and viewer resolution.

## Migration Plan

1. Audit existing GestionRespuesta hooks/services and storage client behavior.
2. Add workflow-anexo storage types and mapper tests.
3. Extend storage service to send PascalCase where required and validate status before complete.
4. Implement `gestionRespuestaUploadDocumental.mapper` and service wrapper.
5. Extend `GestionRespuestaDocumentosProvider` and hook return values with refresh semantics.
6. Implement `GestionRespuestaUploadDocumental`.
7. Replace the simple attachments surface in `GestionRespuestaMainTabContent`.
8. Make `DocumentosWorkbench` reload on `documentosRefreshKey`.
9. Add unit/integration tests and architecture documentation.
10. Run OpenSpec validation and affected test suites.

## Risks / Trade-offs

- Existing storage service currently accepts both camelCase/PascalCase response keys but sends camelCase requests. The workflow-anexo contract requires explicit PascalCase backend payloads.
- `idRespuestaRadicado` can arrive as string or number in context. The adapter must normalize a positive number and block invalid values.
- If no canonical config/tipology endpoint exists in the repo, the adapter must use loader wrappers or existing services without inventing endpoints.
- `DocumentosWorkbench` may need a reload key or hook dependency adjustment depending on how `AppTreeTable` triggers `load`.
- The nested workflow-anexo response is required for confirmed refresh. If backend returns only the flat document result, the document can be normalized but the attachment relation cannot be considered confirmed unless contract evidence says otherwise.

## Open Questions

- None blocking. Endpoint discovery for upload config/tipologies is implementation-time code audit; if absent, use required loaders/wrappers and document the limitation.

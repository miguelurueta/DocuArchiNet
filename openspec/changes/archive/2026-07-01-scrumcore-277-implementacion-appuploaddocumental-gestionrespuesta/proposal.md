## Why

GestionRespuesta currently exposes a simple attachment area in the Gestion tab. SCRUMCORE-277 must replace that surface with the reusable documental upload flow so users can upload one or more files as response attachments from workflow, store them through StorageEngineV2, and refresh the Documentos tab from the backend source of truth.

The official contract for this ticket is the frontend technical document preserved at:

`docs/Architecture/AppUploadDocumental/SCRUM-250-Integracion-Frontend-AnexosRespuesta.md`

No external backend DTO paths are required for this ticket. The contract supplied by Jira/SCRUM-250 is the implementation source of truth.

## What Changes

- Add a GestionRespuesta-specific adapter component that composes `AppUploadDocumental` inside `GestionRespuestaMainTabContent`.
- Map AppUploadDocumental file metadata into the StorageEngineV2 anexo workflow request:
  - `AnexoRespuesta`
  - `CabinetIndexSeed`
  - per-file `Trd`
  - single-document final storage request per file
- Extend the storage client/contracts where needed so the frontend can send PascalCase backend payloads and normalize PascalCase backend responses into typed frontend models.
- Enforce the official flow per file:
  `init -> chunks -> status -> complete -> almacenar`.
- Validate `ChunksPendientes` is empty before `complete` and validate `data.AnexoRespuesta.Created === true` before treating the attachment as created.
- Extend `GestionRespuestaDocumentosProvider` with a shared refresh signal so the Gestion tab can ask the Documentos tab to reload from backend after successful storage.
- Make `DocumentosWorkbench` react to that refresh signal without manually inserting rows.
- Add focused unit/integration tests and enterprise documentation for the GestionRespuesta integration.

## Capabilities

### New Capabilities

- `implementacion-appuploaddocumental-gestionrespuesta`: Integrates `AppUploadDocumental` with GestionRespuesta to store workflow response attachments through StorageEngineV2 and refresh the document workbench.

### Modified Capabilities

- `app-upload-documental`: Reused as the documental upload UI and orchestration surface.
- `almacenamiento-documental-upload`: Extended only where needed for workflow attachment contract mapping/status validation.
- `gestion-respuesta-documentos`: Extended with cross-tab document refresh semantics.

## Impact

- New component/adapter:
  - `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx`
- New mapper/service tests:
  - `src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.ts`
  - `src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.test.ts`
  - `src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts`
- Updated GestionRespuesta integration:
  - `GestionRespuestaMainTabContent.tsx`
  - `GestionRespuestaDocumentosContext.tsx`
  - `useGestionRespuestaDocumentos.ts`
  - `DocumentosWorkbench.tsx`
- Updated storage contracts/client if required:
  - `almacenamientoDocumental.types.ts`
  - `almacenamientoDocumentalUpload.service.ts`
  - related tests
- Documentation:
  - `docs/Architecture/AppUploadDocumental/SCRUMCORE-277-Integracion-GestionRespuesta-Anexos.md`

## Non-Goals

- Do not reimplement `AppUploadDocumental`, `AppUploadBatchView`, `AppUpload`, or `AppProgressBatch`.
- Do not copy legacy HTML, call global legacy functions, use jQuery, WebForms, `.ashx`, `XMLHttpRequest`, or legacy `FormData` upload.
- Do not invent configuration/tipology endpoints.
- Do not modify backend.
- Do not manually insert the stored document into `DocumentosWorkbench`; backend reload remains the source of truth.

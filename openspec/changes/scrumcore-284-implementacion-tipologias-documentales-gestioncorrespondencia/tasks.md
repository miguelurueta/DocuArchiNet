## 1. Refinement

- [x] 1.1 Review Jira prompt and current SCRUMCORE-277 implementation.
- [x] 1.2 Correct scope: keep `AppUploadDocumental`; replace seed typology loader with workflow backend catalog.
- [x] 1.3 Refine proposal/design/spec/tasks with concrete contracts, files, risks, and acceptance criteria.

## 2. Workflow Context Propagation

- [x] 2.1 Add `idRutaWf?: number` to `GestionRespuestaProps`.
- [x] 2.2 Pass `idRutaWf` from `GestionRespuesta` into `GestionRespuestaDocumentosProvider`.
- [x] 2.3 Add `idRutaWf?: number` to `GestionRespuestaDocumentosProvider` props and `GestionRespuestaDocumentosState`.
- [x] 2.4 Expose `idRutaWf` from `useGestionRespuestaDocumentos` fallback state.
- [x] 2.5 Add or update tests proving `idRutaWf` propagates page -> provider -> hook -> upload context.

## 3. Workflow Typology Service

- [x] 3.1 Create `src/modules/gestionCorrespondencia/types/tipologiasDocumentalesWorkflow.types.ts`.
- [x] 3.2 Create `src/modules/gestionCorrespondencia/services/tipologiasDocumentalesWorkflow.service.ts`.
- [x] 3.3 Implement `getTipologiasDocumentalesWorkflow(query, options)` using `clienteApi.get`.
- [x] 3.4 Validate `idTareaWf > 0` and `idRutaWf > 0` before request.
- [x] 3.5 Send params exactly: `Contexto=WORKFLOW`, `IdTareaWf`, `IdRutaWf`.
- [x] 3.6 Ensure `IdTipoTramite` is not sent.
- [x] 3.7 Normalize `{ Id, Descripcion }` to `{ value, label, idTipoDocumento, nombreTipoDocumento }`.
- [x] 3.8 Accept `success=true` with `data=[]`.
- [x] 3.9 Throw normalized functional errors for `success=false` and invalid shapes without introducing `any`.

## 4. Workflow Typology Hook

- [x] 4.1 Create `src/modules/gestionCorrespondencia/hooks/useTipologiasDocumentalesWorkflow.ts`.
- [x] 4.2 Load automatically only when enabled and both workflow ids are positive.
- [x] 4.3 Use `AbortController` and cleanup on unmount/context changes.
- [x] 4.4 Ignore stale responses when `idTareaWf` or `idRutaWf` changes.
- [x] 4.5 Expose `options`, `loading`, `error`, `empty`, and `reload`.
- [x] 4.6 Avoid duplicate backend calls per render.

## 5. Gestion Respuesta Upload Integration

- [x] 5.1 Update `GestionRespuestaUploadDocumental` to read `idRutaWf` from `useGestionRespuestaDocumentos`.
- [x] 5.2 Include `idRutaWorkflow` in `UploadDocumentalContext`.
- [x] 5.3 Replace the seed/hardcoded `loadGestionRespuestaTiposDocumentales` implementation.
- [x] 5.4 Make the Gestion Respuesta typology loader delegate to `getTipologiasDocumentalesWorkflow`.
- [x] 5.5 Do not fall back to hardcoded `Comprobante De Egreso` or any static typology.
- [x] 5.6 Block upload/store path when typology is required and `idRutaWf` is unavailable.
- [x] 5.7 Preserve existing SCRUMCORE-277 upload flow, modal UI, per-file metadata, mapper, storage options, and Workbench refresh.

## 6. Tests

- [x] 6.1 Add `tipologiasDocumentalesWorkflow.service.test.ts`.
- [x] 6.2 Cover service params, no `IdTipoTramite`, normalization, empty response, functional failure, invalid ids, invalid item shape, and `AbortSignal`.
- [x] 6.3 Add `useTipologiasDocumentalesWorkflow.test.tsx`.
- [x] 6.4 Cover no-call without ids, loading state, empty state, error state, reload, abort cleanup, and stale response ignore.
- [x] 6.5 Update `GestionRespuestaUploadDocumental.test.tsx`.
- [x] 6.6 Update `GestionRespuestaMainTabContent.test.tsx` or `GestionRespuesta.test.tsx` for `idRutaWf` propagation.
- [x] 6.7 Run focused suites for service, hook, upload adapter, main tab, provider and affected existing tests.

## 7. Documentation

- [x] 7.1 Create `docs/Architecture/GestionCorrrespondecia/17-FE-Tipologias-Documentales-Adjuntos-Workflow.md`.
- [x] 7.2 Document endpoint, params, response, no frontend `IdTipoTramite`, `idRutaWf` propagation, relation with SCRUMCORE-277, per-file metadata, error/empty/loading policy, and tests.
- [x] 7.3 Update SCRUMCORE-277 integration documentation only if the typology loader behavior description becomes stale.

## 8. Validation And Close

- [x] 8.1 Run `openspec.cmd validate scrumcore-284-implementacion-tipologias-documentales-gestioncorrespondencia --strict`.
- [x] 8.2 Confirm backend not modified.
- [x] 8.3 Confirm no new endpoint invented.
- [x] 8.4 Confirm no `IdTipoTramite` frontend resolution.
- [x] 8.5 Confirm no `any` introduced.
- [x] 8.6 Prepare commit/push/PR after implementation and verification.

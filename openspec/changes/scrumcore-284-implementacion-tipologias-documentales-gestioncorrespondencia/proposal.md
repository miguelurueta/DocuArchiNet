## Why

SCRUMCORE-277 integrated `AppUploadDocumental` into Gestion Respuesta and enabled per-file documental storage through StorageEngineV2. That implementation still uses a Gestion Respuesta adapter loader with a seed/hardcoded tipology option:

```ts
{ idTipoDocumento: 43, nombreTipoDocumento: "Comprobante De Egreso" }
```

SCRUMCORE-284 closes that enterprise gap by loading the real workflow document typologies from backend using the confirmed endpoint:

```text
GET /api/gestor-documental/tipologias-documentales
Contexto=WORKFLOW
IdTareaWf={idTareaWf}
IdRutaWf={idRutaWf}
```

The frontend must not resolve `IdTipoTramite`, must not send `IdTipoTramite=0`, and must not hardcode typologies. Backend SCRUM-304 resolves the workflow context from `IdTareaWf + IdRutaWf`.

## What Changes

- Propagate `idRutaWf` through Gestion Respuesta:
  - `GestionRespuestaProps`;
  - `GestionRespuestaDocumentosProvider`;
  - `GestionRespuestaDocumentosState`;
  - `useGestionRespuestaDocumentos`;
  - `GestionRespuestaUploadDocumental`.
- Add typed workflow typology contracts in Gestion Correspondencia.
- Add `tipologiasDocumentalesWorkflow.service.ts` using `clienteApi`.
- Add `useTipologiasDocumentalesWorkflow` with loading, empty, error, retry, abort and anti-stale behavior.
- Replace the seed/hardcoded `loadGestionRespuestaTiposDocumentales` with a workflow-backed loader.
- Preserve `AppUploadDocumental` and `AppUploadBatchView` as reusable/shared components without domain-specific typology logic.
- Keep per-file metadata selection inside `AppUploadDocumental` and feed it with the real backend catalog.
- Block storage when typologies are required but workflow context/catalog is unavailable.
- Add enterprise documentation for the workflow typology integration.
- Add focused service, hook and component tests.

## Current Context

After SCRUMCORE-277, the current UI no longer uses `AppUpload` directly in `GestionRespuestaMainTabContent`. It opens `GestionRespuestaUploadDocumentalModal`, which renders `GestionRespuestaUploadDocumental`, which composes `AppUploadDocumental`.

Therefore this ticket must not remigrate the UI to `AppUploadBatchView`. Instead, it must use the existing `AppUploadDocumental` loader extension point:

```ts
loadTiposDocumentales: (input: {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
}) => Promise<TipoDocumentalOption[]>;
```

## Capabilities

### New Capabilities

- `implementacion-tipologias-documentales-gestioncorrespondencia`: Workflow-backed documental typologies for Gestion Respuesta attachments.

### Modified Capabilities

- `gestion-correspondencia`: propagate `idRutaWf` and block upload when the required workflow typology catalog cannot be resolved.
- `implementacion-appuploaddocumental-gestionrespuesta`: replace seed typology loader with real WORKFLOW catalog while preserving the StorageEngineV2 upload flow.

## Impact

- No backend changes.
- No new endpoints invented.
- No changes to `AppUploadBatchView` domain responsibilities.
- No changes to the StorageEngineV2 upload contract.
- The main functional risk is missing `idRutaWf` in the caller route/context; the implementation must fail safe and not call the typology endpoint until both workflow ids are valid.


## Context

SCRUMCORE-284 implements real workflow typologies for Gestion Respuesta attachments.

The codebase state after SCRUMCORE-277 is:

- `GestionRespuestaMainTabContent` renders `GestionRespuestaUploadDocumentalModal`.
- `GestionRespuestaUploadDocumentalModal` opens a modal with `GestionRespuestaUploadDocumental`.
- `GestionRespuestaUploadDocumental` composes `AppUploadDocumental`.
- `AppUploadDocumental` already supports:
  - per-file metadata;
  - typology required validation;
  - `loadTiposDocumentales`;
  - `autoSuggestTipologia`;
  - single/batch storage;
  - StorageEngineV2 upload through `almacenamientoDocumentalUpload.service`.
- `gestionRespuestaUploadDocumental.service.ts` currently provides a seed typology list.

This ticket must harden the catalog source, not rebuild the upload UI.

## Backend Contract

Endpoint:

```text
GET /api/gestor-documental/tipologias-documentales
```

Required query for this flow:

```ts
{
  Contexto: "WORKFLOW",
  IdTareaWf: number,
  IdRutaWf: number
}
```

Expected response:

```ts
{
  success: boolean;
  message: string;
  data: Array<{
    Id: number;
    Descripcion: string;
  }>;
  meta?: {
    Status?: "success" | "empty" | "error" | string;
    RequestId?: string;
    Total?: number;
  };
  errors?: unknown[];
}
```

Rules:

- `Id` maps to `idTipoDocumento`.
- `Descripcion` maps to `nombreTipoDocumento`.
- `IdTipoTramite` is not resolved in frontend.
- `IdTipoTramite=0` must never be sent.
- No secondary frontend API call should be introduced to infer `IdTipoTramite`.

## Type Design

Add:

```text
src/modules/gestionCorrespondencia/types/tipologiasDocumentalesWorkflow.types.ts
```

Types:

```ts
export type TipologiaDocumentalWorkflowQuery = {
  idTareaWf: number;
  idRutaWf: number;
};

export type TipologiaDocumentalWorkflowDto = {
  Id: number;
  Descripcion: string;
};

export type TipologiaDocumentalWorkflowOption = {
  value: number;
  label: string;
  idTipoDocumento: number;
  nombreTipoDocumento: string;
};

export type TipologiasDocumentalesWorkflowResponse = {
  success: boolean;
  message: string;
  data: TipologiaDocumentalWorkflowDto[];
  meta?: {
    Status?: string;
    RequestId?: string;
    Total?: number;
  };
  errors?: unknown[];
};
```

The service returns `TipologiaDocumentalWorkflowOption[]`. The Gestion Respuesta adapter maps these to the existing `TipoDocumentalOption[]` expected by `AppUploadDocumental`.

No `any` should be introduced. Unknown backend error payloads must remain `unknown` and be read with guards.

## Service Design

Add:

```text
src/modules/gestionCorrespondencia/services/tipologiasDocumentalesWorkflow.service.ts
```

Public API:

```ts
export async function getTipologiasDocumentalesWorkflow(
  query: TipologiaDocumentalWorkflowQuery,
  options?: { signal?: AbortSignal },
): Promise<TipologiaDocumentalWorkflowOption[]>;
```

Rules:

- Validate `idTareaWf > 0`.
- Validate `idRutaWf > 0`.
- Use `clienteApi.get`.
- Endpoint: `/api/gestor-documental/tipologias-documentales`.
- Params:

```ts
{
  Contexto: "WORKFLOW",
  IdTareaWf: query.idTareaWf,
  IdRutaWf: query.idRutaWf,
}
```

- Do not include `IdTipoTramite`.
- Accept `success=true` with `data=[]`.
- Normalize `Id/Descripcion`.
- Throw a functional typed/service error when:
  - ids are invalid;
  - `success=false`;
  - response shape is invalid;
  - item id is not a positive number;
  - item description is empty.
- Prefer `errors[0].UserMessage`, then `message`, then a generic fallback for functional errors.

## Hook Design

Add:

```text
src/modules/gestionCorrespondencia/hooks/useTipologiasDocumentalesWorkflow.ts
```

Public API:

```ts
export function useTipologiasDocumentalesWorkflow(input: {
  idTareaWf?: number;
  idRutaWf?: number;
  enabled?: boolean;
}): {
  options: TipologiaDocumentalWorkflowOption[];
  loading: boolean;
  error?: string;
  empty: boolean;
  reload: () => Promise<void>;
};
```

Rules:

- Do not call backend until `enabled=true`, `idTareaWf > 0`, and `idRutaWf > 0`.
- Use `AbortController`.
- Abort in-flight request on unmount or context change.
- Ignore stale responses when task/ruta changes.
- Set `empty=true` only when request succeeds with no options.
- Keep `reload` stable and usable after failures.
- Avoid duplicate calls per render.

## Gestion Respuesta Integration

### Propagate `idRutaWf`

Update:

```text
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts
```

Expected minimal contract:

```ts
type GestionRespuestaProps = {
  idTareaWf?: number;
  idRutaWf?: number;
  radicado?: string;
  idRespuestaRadicado?: string | number;
};
```

Provider state:

```ts
type GestionRespuestaDocumentosState = {
  idTareaWf?: number;
  idRutaWf?: number;
  // existing fields...
};
```

### Replace Seed Loader

Update:

```text
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
```

Current seed loader must be removed from production behavior:

```ts
[{ idTipoDocumento: 43, nombreTipoDocumento: "Comprobante De Egreso" }]
```

`GestionRespuestaUploadDocumental` should use the workflow context to load real catalog data. Because `AppUploadDocumental` owns the loading call, the adapter can provide a loader that reads `context.idTareaWorkflow` and `context.idRutaWorkflow` and delegates to `getTipologiasDocumentalesWorkflow`.

Example shape:

```ts
export async function loadGestionRespuestaTiposDocumentales({ context }: GestionRespuestaTiposDocumentalesInput) {
  const options = await getTipologiasDocumentalesWorkflow({
    idTareaWf: requirePositive(context.idTareaWorkflow, "idTareaWf"),
    idRutaWf: requirePositive(context.idRutaWorkflow, "idRutaWf"),
  });

  return options.map((option) => ({
    idTipoDocumento: option.idTipoDocumento,
    nombreTipoDocumento: option.nombreTipoDocumento,
  }));
}
```

`GestionRespuestaUploadDocumental` must pass `idRutaWorkflow` into `UploadDocumentalContext`.

### Fail-Safe Behavior

If `idRutaWf` is missing:

- do not call `/api/gestor-documental/tipologias-documentales`;
- display a functional warning/error state indicating the workflow route is required to load typologies;
- do not allow storing files requiring typology.

If the typology endpoint fails:

- show a functional error through existing `AppUploadDocumental`/adapter error handling;
- allow retry by remount/reload path if implemented;
- do not fall back to hardcoded typologies.

If backend returns empty data:

- show an empty catalog state;
- block storage when typology is required.

## UI Boundaries

Do not modify `AppUploadBatchView` to know about workflow typologies. It remains a shared batch UI.

Do not create a new dropdown component. Use the existing `AppInputSelect` path already used by `AppUploadDocumental`.

Do not move documental domain logic into shared UI components.

## Testing Strategy

### Service Tests

```text
src/modules/gestionCorrespondencia/tests/tipologiasDocumentalesWorkflow.service.test.ts
```

Cover:

- sends `Contexto=WORKFLOW`, `IdTareaWf`, `IdRutaWf`;
- does not send `IdTipoTramite`;
- maps `Id/Descripcion` correctly;
- accepts `success=true` with `data=[]`;
- throws functional error for `success=false`;
- rejects invalid ids before request;
- rejects invalid item shape;
- respects `AbortSignal`.

### Hook Tests

```text
src/modules/gestionCorrespondencia/tests/useTipologiasDocumentalesWorkflow.test.tsx
```

Cover:

- no call without `idTareaWf`;
- no call without `idRutaWf`;
- loads options with valid ids;
- exposes `loading`;
- exposes `empty`;
- exposes `error`;
- `reload` retries;
- stale response is ignored when ids change;
- aborts in-flight request on unmount/change.

### Integration Tests

Update:

```text
src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx
src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx
```

Cover:

- `idRutaWf` propagates from page to provider/context;
- `GestionRespuestaUploadDocumental` includes `idRutaWorkflow` in `UploadDocumentalContext`;
- the Gestion Respuesta typology loader delegates to workflow service;
- missing `idRutaWf` blocks loader/storage path;
- hardcoded typology seed is no longer used in production loader.

## Documentation

Create:

```text
docs/Architecture/GestionCorrrespondecia/17-FE-Tipologias-Documentales-Adjuntos-Workflow.md
```

Must include:

- problem;
- endpoint consumed;
- request params;
- response shape;
- why `WORKFLOW + IdTareaWf + IdRutaWf`;
- why frontend does not resolve `IdTipoTramite`;
- relation with SCRUMCORE-277 `AppUploadDocumental`;
- metadata per file;
- error/empty/loading policy;
- tests executed.

## Risks / Trade-offs

- `idRutaWf` may not currently be available from all callers. This must be surfaced as a required context problem instead of silently using fallback typologies.
- Backend may return `success=true` with an empty catalog; this is not an error, but it blocks storage when typology is mandatory.
- `AppUploadDocumental` currently owns catalog loading through `loadTiposDocumentales`; introducing an external hook should not duplicate calls. Prefer service-backed loader unless the UI needs explicit retry controls outside `AppUploadDocumental`.


# SCRUMCORE-227 — AppVisorEmbedPdf.load() (Implementación detallada)

## Archivos tocados
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/types/AppVisorEmbedPdfProps.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/index.ts`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

## API pública
`AppVisorEmbedPdf` ahora expone ref:

```ts
type AppVisorEmbedPdfRef = {
  load(input: AppVisorLoadInput): Promise<AppVisorLoadResult>;
  reset(): void;
  cancelCurrentLoad(): void;
}
```

### Modo legacy vs managed
- Legacy: el consumidor pasa `fileUrl` como antes.
- Managed: el consumidor obtiene `visorRef` y llama `visorRef.current.load(input)`.

Regla: **solo** en managed se aplican `permissionsEffective` como gating de toolbar.

## Permisos y policy engine
### Endpoint consumido
`fetchMisPermisosVisorPdf({ codigoImpl })`:
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

### Mapping
`mapPermisosVisorPdfToEffectivePermissions(permissionsRaw)` centraliza el mapping:
- `pdf.signature.add` -> `allowSignaturePlacement`
- `pdf.signature.delete` -> `allowSignatureDelete`
- `pdf.signature.lock` -> `allowSignatureLockToggle`
- `pdf.annotation.edit` -> `allowAnnotationEdit`
- `pdf.export` -> `allowExport`
- `pdf.print` -> `allowPrint`

### Override por firma
`applySignedOverride()`:
- Si `isElectronicallySigned=true`:
  - `allowSignaturePlacement=false`
  - `allowSignatureDelete=false`
  - `allowSignatureLockToggle=false`
  - `allowAnnotationEdit=false`

Fail-safe:
- Si falla permisos o falta mapping: `failClosedEffectivePermissions()` (todo false) para edición.
- Visualización: no se bloquea por permisos (fail-open) mientras `url` sea válida.

## Concurrencia
- `loadAbortRef` cancela permisos en vuelo al iniciar un nuevo `load()`.
- `loadSeqRef` evita stale responses.

## Handshake (engine)
`EmbedPdfDocumentHost` reporta al `load()`:
- `ok=true` cuando `response.task.wait` resuelve.
- `ok=false` cuando falla `task.wait` o cuando `PdfErrorCode.Password` activa prompt.

Esto permite que `load()` tenga un resultado determinista sin acoplar consumidores al engine.

## Toolbar gating
`AppPdfToolbar` ahora acepta flags:
- `isSignatureDisabled`, `isDeleteSelectedSignatureDisabled`, `isSignatureLockToggleDisabled`,
  `isPrintDisabled`, `isExportDisabled`.

En legacy se mantiene abierto (`allow* = true`) para no romper consumidores previos.

## Integración (consumidor real actual)
`DocumentosWorkbench.tsx`:
- Mantiene `fileUrl` desde orquestador (blob).
- En PDFs, llama `visorRef.current.load({ url, nombre_modulo: 'gestioncorrespondencia', ... })`.
- Usa `documentosTable.getWorkbenchContext()` para obtener `radicado` cuando exista.


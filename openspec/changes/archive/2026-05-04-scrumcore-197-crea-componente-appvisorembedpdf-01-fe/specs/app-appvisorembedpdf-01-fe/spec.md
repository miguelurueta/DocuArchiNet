# [SPEC:SCRUMCORE-197] Crea componente `AppVisorEmbedPdf` (01-FE)

## Requirement: El sistema SHALL exponer `AppVisorEmbedPdf` como componente shared

- **THEN** el componente SHALL vivir en `src/app/Components/UI/AppVisorEmbedPdf/`
- **AND** SHALL tener export público en `src/app/Components/UI/AppVisorEmbedPdf/index.ts`
- **AND** SHALL ser agnóstico al dominio (no SHALL importar desde `src/modules/**`)

## Requirement: El sistema SHALL definir contratos mínimos para implementación incremental

- **THEN** SHALL existir `src/app/Components/UI/AppVisorEmbedPdf/domain/pdf.types.ts` con:
  - `AppPdfSource` (url/bytes)
  - `AppPdfLoadState`: `idle`, `loading`, `ready`, `password_required`, `error`
  - `AppPdfCapabilities`: flags para plugins dinámicos
- **AND** SHALL existir `src/app/Components/UI/AppVisorEmbedPdf/engine/embedPdfEngine.types.ts` con interfaz:
  - `load(source)`
  - `renderPage(request, canvas, signal?)`
  - `destroy()`
- **AND** SHALL existir `src/app/Components/UI/AppVisorEmbedPdf/domain/viewerApi.types.ts` con una API headless mínima:
  - `setSource(source|null)`
  - `setZoom(zoom)`
  - `setRotation(degrees)`
  - `openSidebar()` / `closeSidebar()`

## Requirement: El sistema SHALL documentar el componente y el plan incremental

- **THEN** SHALL existir `src/app/Components/UI/AppVisorEmbedPdf/README.md`
- **AND** SHALL registrarse el avance en `docs/Architecture/AppVisorEmbedPdf/implementation-log.md`


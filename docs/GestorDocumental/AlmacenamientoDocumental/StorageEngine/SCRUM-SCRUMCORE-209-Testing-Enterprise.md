# SCRUMCORE-209 — Testing Enterprise

## Unit / Integration (Vitest + RTL)
Archivo:
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

Estrategia:
- Se mockea `useDocumentManagerCapability()` para exponer:
  - `openDocumentUrl` (Task con `.wait(...)`)
  - `retryDocument` (Task con `.wait(...)`)
  - `onDocumentError` (hook de evento)
- Se mockea `DocumentContent` para simular estados `isLoaded/isError/isLoading`.

Escenarios mínimos:
- Renderiza prompt cuando el documento “falla” (simulación password required).
- No rompe el resto del render del visor.

## E2E (Playwright)
- Pendiente/optativo según pipeline del proyecto para PDFs protegidos reales.


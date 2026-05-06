# SCRUMCORE-204 — Testing enterprise

## Unit / RTL (Vitest)

Mínimo:
- Renderiza toolbar.
- Click `Zoom in/out/reset` invoca handlers/capability (mockeado).

Evidencias (local, 2026-05-06):
- `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` → `1 passed / 5 passed`

## E2E (Playwright)

Mínimo:
- Toolbar visible en ruta de test.
- Zoom cambia el % y reset vuelve a `100%`.
- Re-render estable (sin warnings Rules of Hooks).

Evidencias (local, 2026-05-06):
- `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfZoom.spec.ts playwright/appvisorEmbedPdfRerender.spec.ts` → `2 passed`

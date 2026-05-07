# SCRUMCORE-205 — Testing enterprise

## Unit / RTL (Vitest)

- Toggle open/close thumbnails.
- Click en thumbnail llama `scrollToPage` (capability oficial).

Evidencias (local, 2026-05-07):
- `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` → `1 passed / 6 passed`

## E2E (Playwright)

- Toggle thumbnails abre/cierra.
- Re-render estable (sin warnings Rules of Hooks).

Evidencias (local, 2026-05-06):
- `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfThumbnails.spec.ts` → `1 passed`

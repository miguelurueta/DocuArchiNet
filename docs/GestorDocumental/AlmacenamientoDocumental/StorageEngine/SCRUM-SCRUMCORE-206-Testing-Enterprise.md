# SCRUMCORE-206 — Testing enterprise (AppVisorEmbedPdf)

## Playwright (E2E)
- `playwright/appvisorEmbedPdfRotateZoomJump.spec.ts`
  - Valida guardrail: zoom disabled en rotación 90° (evita jump).
- `playwright/appvisorEmbedPdfRotate.spec.ts`
  - Rotación izquierda/derecha (sin reset en toolbar).
- `playwright/appvisorEmbedPdfZoom.spec.ts`
  - Zoom in/out/reset cuando `rotation === 0`.

## Consideraciones
- Pruebas que dependen de `/__playwright/embedpdf` requieren servidor `npm run dev` activo.

- Unit/RTL: clicks rotación invocan capability oficial (mock).
- E2E: rotate izquierda/derecha/reset sin warnings hooks.

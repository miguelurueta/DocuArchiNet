# SCRUMCORE-209 — Evidencias Técnicas

## Manual (local)
- Validación visual: prompt aparece cuando el PDF requiere password.
- Reintento con password inválida: no bloquea input y permite corregir.
- Password válida: el prompt se cierra y el PDF renderiza.

## Tests ejecutados (local)
- Vitest: `npm.cmd test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` (2026-05-12) → `12 passed`.

## CI/CD (pendiente / a completar)
- Adjuntar artefactos Playwright si el pipeline los genera:
  - `test-results/`
  - traces/videos/screenshots
- Adjuntar reporte de unit tests (Vitest) y coverage si aplica.

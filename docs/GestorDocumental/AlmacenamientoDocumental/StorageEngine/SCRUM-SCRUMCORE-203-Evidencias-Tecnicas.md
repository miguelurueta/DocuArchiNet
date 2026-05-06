# SCRUMCORE-203 — Evidencias técnicas

> Adjuntar aquí enlaces/rutas a evidencias generadas por CI/CD y/o ejecución local.

## Testing (Vitest)

- Output (local, 2026-05-06): `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` → `2 passed / 8 passed`
- Coverage: `TBD`

## Playwright (E2E)

- Report: `playwright-report/` (generado por Playwright; revisar en ejecución local/CI)
- Run (local, 2026-05-06): `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfRerender.spec.ts` → `1 passed`
- Traces: `TBD`
- Screenshots: `TBD`
- Video: `TBD`

## Logs / métricas

- Console logs relevantes: `TBD`
- Métricas performance/memoria: `TBD`

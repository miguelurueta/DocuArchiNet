# SCRUMCORE-230 — Pruebas

## Unit
- Mapper:
  - `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts`
  - Comando: `npm.cmd test -- --run src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts`
- Hook:
  - `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`
  - Comando: `npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`

## Integración UI (RTL)
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
- Comando: `npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## E2E (Playwright)
- `playwright/gestionCorrespondencia/documentosWorkbench.radicado230.spec.ts`
- Comando: `npx playwright test playwright/gestionCorrespondencia/documentosWorkbench.radicado230.spec.ts`
- Nota: el spec se ejecuta como E2E real-env y hace `skip` automático si faltan variables:
  - `PLAYWRIGHT_LOGIN_EMPRESA_ID`
  - `PLAYWRIGHT_LOGIN_MODULO_ID`
  - `PLAYWRIGHT_LOGIN_USER`
  - `PLAYWRIGHT_LOGIN_PASSWORD`
  - (opcional) `PLAYWRIGHT_API_URL`


# SCRUMCORE-217 - Pruebas

## Unitarias (obligatorias)

- `[SPEC:APPTREETABLE-217]` `gestionRespuestaDocumentosRequestMapper`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts`
- `[SPEC:APPTREETABLE-217]` `documentosWorkbenchResponseAdapter`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `[SPEC:APPTREETABLE-217]` `documentosWorkbenchActionMapper`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`

## Integración UI (obligatorias)

- `[SPEC:APPTREETABLE-217]` `DocumentosWorkbench`
  - Archivo: `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Cubre: render base, rail toggle, variante overlay, wiring selección/acción hacia visor (mock).

## Browser interaction (manual checklist)

- Click principal (fila/label) ejecuta `ver_documento` y actualiza visor.
- Expand/collapse no rompe layout ni selection.
- Menú secundario abre y dispara acción dinámica.
- Error en query muestra estado + retry.
- Responsive intacto: desktop, mobile overlay, tablet/iPad.
- Focus visible y navegación keyboard-friendly.

## E2E (Playwright)

Se agregó un smoke E2E:
- Archivo: `playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts`

Ejecución (2026-05-21):
- `npm run test:e2e -- playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts`
  - Resultado: FAIL por missing env var `PLAYWRIGHT_LOGIN_EMPRESA_ID` (configuración de entorno requerida).

## Regresión

- Verificar que `AppTreeTable` y `AppTable` siguen pasando sus suites existentes (smoke).

## Evidencia de ejecución

Comandos ejecutados (2026-05-21):
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (PASS)

Comandos ejecutados (2026-05-21) - validaciÃ³n estÃ¡tica:
- `npx tsc -p tsconfig.json --noEmit` (PASS)

Comandos ejecutados (2026-05-21) - regresiÃ³n acotada (ticket):
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx --pool=threads` (PASS)

Nota: en este entorno la ejecuciÃ³n de Vitest/Playwright puede fallar con `Error: spawn EPERM` al cargar `vite.config.ts` (esbuild/vite). En ese caso, ejecutar pruebas fuera del sandbox o con permisos adecuados.

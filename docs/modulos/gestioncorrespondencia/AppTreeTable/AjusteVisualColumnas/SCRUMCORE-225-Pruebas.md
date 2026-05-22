# SCRUMCORE-225 - Pruebas

## 1. Unitarias (Vitest)
**Ejecutadas:** sí (local) — `2026-05-22`.

Cobertura agregada:
- `[SPEC:APPTREETABLE-225-001]` selector/scoping/sizing preset:
  - `src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts`
- `[SPEC:APPTREETABLE-225-001]` integración adapter Workbench (Dynamic UI → Workbench model):
  - `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `[SPEC:APPTREETABLE-225-001]` wiring Workbench → AppTreeTable:
  - `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

Comandos (evidencia):
- `npm.cmd test -- --run src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
  - Resultado: `2 passed / 12 passed`
  - Validado preset: Documento `minWidth=60`, Acciones `minWidth=80`.
- `npm.cmd test -- --run src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Resultado: `3 passed / 22 passed`

## 2. Integración UI (Testing Library)
**Estado:** cubierta indirectamente por mocks/adapter tests (evita AG Grid real en JSDOM).

Pendiente opcional (si se requiere aumentar confianza):
- Validar render de headers usando mock de `AppTable`/`AppTreeTable` a nivel de DOM (sin AG Grid real).

## 3. Playwright E2E (regresión visual/funcional)
**Ejecutadas:** pendientes (requiere entorno real y variables `PLAYWRIGHT_*`).

Implementadas:
- `[SPEC:APPTREETABLE-225-001]` Workbench renderiza 2 headers visibles:
  - `playwright/gestionCorrespondencia/documentosWorkbench.columnas225.spec.ts`
- Placeholders `test.skip`:
  - click primario actualiza visor sin romper layout
  - acción secundaria no rompe selección/visor

Comando sugerido:
- `npm.cmd run test:e2e`

## 4. Regresión
- Confirmar que otros `tableId` no reciben el preset (scoping por `tableId`).
- Confirmar que acciones/selección no se alteran (solo presentation).

## 5. Go/No-Go
- Go si: unit tests pasan + Playwright (headers) pasa en entorno real.
- No-Go si: aparece scroll horizontal permanente en desktop o se pierden acciones/selección.

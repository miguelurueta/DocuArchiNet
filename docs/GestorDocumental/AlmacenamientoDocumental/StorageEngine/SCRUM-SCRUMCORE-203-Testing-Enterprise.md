# SCRUMCORE-203 — Testing enterprise

> Este documento debe reflejar lo ejecutado (CI/CD + reportes locales) y adjuntar evidencias.

## Unit testing (Vitest)

Objetivo:
- Validar hooks/utils/lógica interna estable.

Escenarios (mínimos):
- Render básico del componente sin crash.
- Comportamiento de demo PDF cuando `fileUrl` no existe.

Evidencias:
- Ejecutado local (2026-05-06): `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Resultado: `2 passed`, `8 passed`.

## Integration testing (React Testing Library)

Objetivo:
- Validar integración del componente en un consumer (ej. Workbench) sin filtrar `@embedpdf/*`.

Evidencias:
- `TBD`.

## E2E testing (Playwright) — REQUERIDO

Precondición:
- Playwright configurado en el repo.
- Browsers instalados (si falta): `npx playwright install`.

Escenarios (mínimos):
- Cargar una ruta que renderice `AppVisorEmbedPdf` con demo PDF.
- Validar que el visor renderiza contenido (por ejemplo, presencia del contenedor/role y ausencia de loader permanente).

### Re-render testing — REQUERIDO

Objetivo:
- Validar estabilidad de re-render de `AppVisorEmbedPdf`:
  - no loops
  - no warnings de Rules of Hooks
  - render estable ante re-mount o cambios de layout del contenedor

Estrategia:
- Test Playwright que:
  - navega a la pantalla con el visor
  - fuerza un re-render del árbol (p. ej. toggle UI del consumer o navegación ida/vuelta)
  - recolecta `page.on('console', ...)` y falla si detecta warnings/errores de hooks

Evidencias:
- Instalación browsers (2026-05-06): `npx playwright install`
- Ejecutado local (2026-05-06): `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfRerender.spec.ts`
  - Resultado: `1 passed` (chromium).

## Visual regression

- `TBD` (si el pipeline corporativo lo soporta, adjuntar screenshots).

## Performance testing

- `TBD` (si se mide: tiempos de carga, memoria, FPS).

## Accessibility testing

- Validar `aria-label`, roles y navegación básica.
- `TBD` (ideal: axe en pipeline).

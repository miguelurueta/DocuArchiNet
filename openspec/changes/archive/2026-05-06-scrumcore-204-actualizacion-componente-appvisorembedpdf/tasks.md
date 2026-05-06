# Tasks — SCRUMCORE-204 (Toolbar + Zoom Plugin)

## Refinamiento (antes de tocar código)
- [ ] Confirmar que el plugin a usar es `@embedpdf/plugin-zoom` y que se registrará con `createPluginRegistration(ZoomPluginPackage)`.
- [ ] Confirmar alcance UI: `Zoom In`, `Zoom Out`, `Reset` (sin fit-to-width/page).
- [ ] Confirmar estilo/ubicación toolbar (minimalista tipo viewer profesional, responsive, integrada al visor).

## Implementación (solo después de tu aprobación)
- [ ] Agregar dependencia `@embedpdf/plugin-zoom` (package.json) y registrar plugin oficialmente:
  - `createPluginRegistration(ZoomPluginPackage)` en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`.
- [ ] Crear toolbar presentacional y memoizada:
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.module.css`
  - Exportar `AppPdfToolbarProps` (tipado fuerte).
- [ ] Integrar toolbar en `AppVisorEmbedPdf` sin romper encapsulación:
  - La toolbar recibe `zoomLevel` + handlers; no conoce engine/plugins/workbench.
  - La lógica de zoom usa capability oficial del plugin (sin lógica manual).
- [ ] Performance:
  - Memoizar `AppPdfToolbar` con `React.memo` para evitar rerenders por scroll/virtualización.
- [ ] Actualizar tests mínimos obligatorios:
  - [ ] `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`: render toolbar + botones llaman handlers (sin crash, sin warnings React).
  - [ ] Playwright: agregar/actualizar escenario E2E para zoom + re-render estable (sin warnings hooks).
- [ ] Documentación enterprise obligatoria:
  - [ ] Actualizar `src/app/Components/UI/AppVisorEmbedPdf/README.md` (toolbar/zoom, non-goals, troubleshooting).
  - [ ] Generar/actualizar los 9 documentos en `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/` para `SCRUMCORE-204`, incluyendo Mermaid:
    - arquitectura
    - flujo zoom → viewport
    - secuencia render
    - responsabilidades
    - interacción toolbar ↔ viewport

## Validación (con tu aprobación antes de correr comandos)
- [ ] Pedir aprobación para correr `npm.cmd test` (focalizado).
- [ ] Pedir aprobación para correr `npm.cmd run test:e2e` (Playwright) y `npx playwright install` si falta.

## Entrega
- [ ] `opsxj:archive SCRUMCORE-204`
- [ ] Tras merge: `opsxj:close SCRUMCORE-204`

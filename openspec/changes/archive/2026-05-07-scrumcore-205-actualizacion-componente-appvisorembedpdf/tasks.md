# Tasks — SCRUMCORE-205 (Thumbnail colapsable, plugin oficial)

## Refinamiento (confirmado)
- [x] Plugin oficial: `@embedpdf/plugin-thumbnail` + `createPluginRegistration(ThumbnailPluginPackage)`.
- [x] Botón en toolbar: izquierda, solo icono + tooltip.
- [x] Panel thumbnails: izquierda.
- [x] Estado inicial: cerrado (`isThumbnailOpen = false`).
- [x] Estilo del botón: similar a botones existentes del toolbar.
- [x] No implementar lógica custom de thumbnails; usar solo plugin oficial.

## Implementación
- [x] Agregar dependencia `@embedpdf/plugin-thumbnail` en `package.json`.
- [x] Registrar `ThumbnailPluginPackage` en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`.
- [x] Actualizar `AppPdfToolbarProps` y `AppPdfToolbar` para incluir:
  - `onToggleThumbnails(): void`
  - `isThumbnailOpen: boolean`
- [x] Agregar botón toggle thumbnails en `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`.
- [x] Mantener `isThumbnailOpen` únicamente en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`.
- [x] Renderizar thumbnails directamente desde el plugin oficial en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` (sin `AppPdfThumbnailPanel.tsx`).
- [x] Layout colapsable solo visual (CSS Modules) sin wrappers innecesarios.
- [x] Auto-scroll nativo del plugin thumbnails configurado en el registro (`autoScroll: true`).
- [x] Click thumbnail navega usando capability oficial de scroll (`scrollToPage`) sin navegación “manual” fuera del plugin.
- [x] Highlight visual de página actual (solo CSS) basado en `useScroll(...).state.currentPage`.

## Testing
- [x] Actualizar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` para validar:
  - toggle open/close thumbnails
  - click thumbnail llama `scrollToPage` (capability oficial)
  - zoom sigue funcionando (smoke)
- [x] Agregar test Playwright: `playwright/appvisorEmbedPdfThumbnails.spec.ts` (toggle thumbnails + re-render estable, sin warnings hooks).

## Documentación enterprise
- [x] Generar/actualizar 9 archivos `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-205-*.md` con Mermaid obligatorios.
- [ ] `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-205-Metadata.md`: completar desde Jira (Sprint/Assignee/Estado) + Git/CI (branch/commit/run). (Git/fecha local ya completados; falta Jira)

## Validación (pedir aprobación antes de correr comandos)
- [x] Pedir aprobación para correr `npm.cmd test` focalizado.
- [x] Pedir aprobación para correr `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfThumbnails.spec.ts` (y `npx playwright install` si falta).

## Entrega
- [ ] `opsxj:archive SCRUMCORE-205`
- [ ] Tras merge: `opsxj:close SCRUMCORE-205`

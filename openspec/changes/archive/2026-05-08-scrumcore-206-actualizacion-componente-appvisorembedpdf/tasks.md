# Tasks — SCRUMCORE-206 (Rotate plugin oficial)

## Refinamiento (confírmame antes de implementar)
- [ ] Confirmar plugin oficial: `@embedpdf/plugin-rotate` + `RotatePluginPackage`.
- [ ] Confirmar acciones UI: rotar derecha, rotar izquierda, reset (¿cuáles van?).
- [ ] Confirmar ubicación de los botones en toolbar (izquierda/derecha).

## Implementación (NO ejecutar hasta tu aprobación)
- [ ] Agregar dependencia `@embedpdf/plugin-rotate` en `package.json`.
- [ ] Registrar `RotatePluginPackage` en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`.
- [ ] Extender `AppPdfToolbarProps`/`AppPdfToolbar` para disparar rotación (presentacional, memoizado, sin conocer EmbedPDF).
- [ ] Mantener estado/uso de rotate dentro de `AppVisorEmbedPdf.tsx` usando hooks/capabilities oficiales del plugin.
- [ ] Verificar que zoom + thumbnails siguen funcionando (sin wrappers extra).

## Testing
- [ ] Actualizar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` (click rotate invoca capability).
- [ ] Agregar test Playwright de rotate + re-render estable (sin warnings hooks).

## Documentación enterprise
- [ ] Generar/actualizar 9 docs `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-206-*.md` con Mermaid.
- [ ] `SCRUM-SCRUMCORE-206-Metadata.md` completar desde Jira (Sprint/Assignee/Estado) + Git/CI.

## Validación (pedir aprobación antes de correr comandos)
- [ ] Pedir aprobación para correr `npm.cmd test` focalizado.
- [ ] Pedir aprobación para correr `npm.cmd run test:e2e` (y `npx playwright install` si falta).

## Entrega
- [ ] `opsxj:archive SCRUMCORE-206`
- [ ] Tras merge: `opsxj:close SCRUMCORE-206`

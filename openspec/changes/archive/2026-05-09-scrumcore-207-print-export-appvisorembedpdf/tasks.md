# SCRUMCORE-207 — Tasks (Print + Export)

## Artefactos OpenSpec
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Implementación
- [x] Agregar dependencias `@embedpdf/plugin-print` y `@embedpdf/plugin-export` en `package.json`
- [x] Registrar `PrintPluginPackage` y `ExportPluginPackage` en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`
- [x] Integrar `usePrint(documentId)` y `useExport(documentId)` en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- [x] Extender `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx` con botones `Print`/`Export` (derecha)
- [x] Asegurar handlers con guard clause (no crash si `provides` es `null`)

## Testing (Vitest/RTL)
- [x] Mock `@embedpdf/plugin-print/react` y `@embedpdf/plugin-export/react` en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- [x] Test: toolbar renderiza botones Print/Export
- [x] Test: click ejecuta `provides.print()` / `provides.download()`
- [x] Test: no crashea cuando `provides` es `null`

## Documentación enterprise (obligatoria)
Ruta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- [x] `SCRUM-SCRUMCORE-207-Metadata.md` (Jira+Git+CI/CD)
- [x] `SCRUM-SCRUMCORE-207-Objetivo-General.md`
- [x] `SCRUM-SCRUMCORE-207-Responsabilidades-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-207-Arquitectura-Tecnica.md` (Mermaid)
- [x] `SCRUM-SCRUMCORE-207-Informacion-Tecnica-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-207-APIs-Utilizadas.md`
- [x] `SCRUM-SCRUMCORE-207-Comportamiento-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-207-Testing-Enterprise.md`
- [x] `SCRUM-SCRUMCORE-207-Evidencias-Tecnicas.md`

## Validación
- [x] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- [x] `npm.cmd run test:e2e -- playwright/appvisorEmbedPdfPrintExport.spec.ts`

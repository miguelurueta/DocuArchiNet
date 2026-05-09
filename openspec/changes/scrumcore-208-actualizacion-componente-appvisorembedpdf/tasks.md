# SCRUMCORE-208 — Tasks (Paginación nativa EmbedPDF)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Implementación
- [ ] Extender `AppPdfToolbarProps` con paginación (current/total + handlers)
- [ ] Render UI de paginación en `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
- [ ] Integrar `useScroll(documentId)` en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- [ ] Conectar handlers `onPreviousPage/onNextPage` usando `scroll.provides?.scrollToPreviousPage/scrollToNextPage`
- [ ] Ajustar CSS para layout responsive sin romper toolbar actual (CSS Modules)
- [ ] Guard clauses: no crash si `scroll.provides` es `null`

## Testing (Vitest/RTL)
- [ ] Mock `@embedpdf/plugin-scroll/react` (state + provides)
- [ ] Test: toolbar renderiza `Página X de Y`
- [ ] Test: click en anterior llama `scrollToPreviousPage`
- [ ] Test: click en siguiente llama `scrollToNextPage`
- [ ] Test: no crashea cuando `scroll.provides` es `null`

## Documentación enterprise (obligatoria)
Ruta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- [ ] `SCRUM-SCRUMCORE-208-Metadata.md` (Jira+Git+CI/CD)
- [ ] `SCRUM-SCRUMCORE-208-Objetivo-General.md`
- [ ] `SCRUM-SCRUMCORE-208-Responsabilidades-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-208-Arquitectura-Tecnica.md` (Mermaid)
- [ ] `SCRUM-SCRUMCORE-208-Informacion-Tecnica-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-208-APIs-Utilizadas.md`
- [ ] `SCRUM-SCRUMCORE-208-Comportamiento-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-208-Testing-Enterprise.md`
- [ ] `SCRUM-SCRUMCORE-208-Evidencias-Tecnicas.md`

## Validación
- [ ] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- [ ] (Opcional) Playwright smoke del visor (si aplica al flujo)


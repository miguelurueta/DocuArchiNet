# SCRUMCORE-208 — Tasks (Paginación nativa EmbedPDF)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Implementación
- [x] Extender `AppPdfToolbarProps` con paginación (current/total + handlers) (paginación quedó como overlay flotante)
- [x] Render UI de paginación (overlay) en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- [x] Integrar `useScroll(documentId)` en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- [x] Conectar handlers `onPreviousPage/onNextPage` usando `scroll.provides?.scrollToPreviousPage/scrollToNextPage`
- [x] Ajustar CSS para layout responsive sin romper toolbar actual (CSS Modules)
- [x] Guard clauses: no crash si `scroll.provides` es `null`

## Testing (Vitest/RTL)
- [x] Mock `@embedpdf/plugin-scroll/react` (state + provides)
- [x] Test: renderiza indicador `X/Y` (aria-label: `Página X de Y`)
- [x] Test: click en anterior llama `scrollToPreviousPage`
- [x] Test: click en siguiente llama `scrollToNextPage`
- [x] Test: no crashea cuando `scroll.provides` es `null`

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
- [x] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- [ ] (Opcional) Playwright smoke del visor (si aplica al flujo)

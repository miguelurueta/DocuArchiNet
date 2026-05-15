# SCRUMCORE-209 — Tasks (Password Protected via DocumentManager)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Implementación
- [x] Crear `presentation/AppPdfPasswordPrompt.tsx` (memoizado, desacoplado)
- [x] Crear `presentation/AppPdfPasswordPrompt.module.css` (CSS Modules)
- [x] Integrar prompt en `AppVisorEmbedPdf.tsx` (overlay) sin afectar visor
- [x] Manejar retry con `retryDocument(documentId, { password })` (DocumentManager oficial)
- [x] Detectar invalid password vía `onDocumentError` + `PdfErrorCode.Password` y reflejarlo en prompt
- [x] Guard clauses + cleanup (no race/stale states)

## Testing (Vitest/RTL)
- [x] Mock `useDocumentManagerCapability` para simular password-required/invalid
- [x] Test: muestra prompt cuando open requiere password
- [x] Test: submit llama `retryDocument` con `password`
- [x] Test: invalid password muestra estado de error (vía prompt)
- [x] Test: no rompe render del visor cuando el documento se desbloquea

## Documentación enterprise (obligatoria)
Ruta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- [x] `SCRUM-SCRUMCORE-209-Metadata.md`
- [x] `SCRUM-SCRUMCORE-209-Objetivo-General.md`
- [x] `SCRUM-SCRUMCORE-209-Responsabilidades-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-209-Arquitectura-Tecnica.md` (Mermaid)
- [x] `SCRUM-SCRUMCORE-209-Informacion-Tecnica-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-209-APIs-Utilizadas.md`
- [x] `SCRUM-SCRUMCORE-209-Comportamiento-del-Componente.md`
- [x] `SCRUM-SCRUMCORE-209-Testing-Enterprise.md`
- [x] `SCRUM-SCRUMCORE-209-Evidencias-Tecnicas.md`

## Validación
- [x] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

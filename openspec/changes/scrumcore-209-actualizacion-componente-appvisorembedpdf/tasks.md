# SCRUMCORE-209 — Tasks (Password Protected via DocumentManager)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Implementación
- [ ] Crear `presentation/AppPdfPasswordPrompt.tsx` (memoizado, desacoplado)
- [ ] Crear `presentation/AppPdfPasswordPrompt.module.css` (CSS Modules)
- [ ] Integrar prompt en `AppVisorEmbedPdf.tsx` (overlay) sin afectar visor
- [ ] Manejar retry con `openDocumentUrl({ url, password, autoActivate: true })`
- [ ] Detectar invalid password y reflejarlo en prompt
- [ ] Guard clauses + cleanup (no race/stale states)

## Testing (Vitest/RTL)
- [ ] Mock `useDocumentManagerCapability` para simular password-required/invalid
- [ ] Test: muestra prompt cuando open requiere password
- [ ] Test: submit llama `openDocumentUrl` con `password`
- [ ] Test: invalid password muestra estado de error
- [ ] Test: no rompe render del visor cuando el documento se desbloquea

## Documentación enterprise (obligatoria)
Ruta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- [ ] `SCRUM-SCRUMCORE-209-Metadata.md`
- [ ] `SCRUM-SCRUMCORE-209-Objetivo-General.md`
- [ ] `SCRUM-SCRUMCORE-209-Responsabilidades-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-209-Arquitectura-Tecnica.md` (Mermaid)
- [ ] `SCRUM-SCRUMCORE-209-Informacion-Tecnica-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-209-APIs-Utilizadas.md`
- [ ] `SCRUM-SCRUMCORE-209-Comportamiento-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-209-Testing-Enterprise.md`
- [ ] `SCRUM-SCRUMCORE-209-Evidencias-Tecnicas.md`

## Validación
- [ ] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`


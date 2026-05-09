# SCRUMCORE-209 — Tasks (Password Protected Plugin)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Dependencias / Registro
- [ ] Instalar `@embedpdf/plugin-password-protected`
- [ ] Registrar `PasswordProtectedPluginPackage` en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`

## UI Password Prompt (mínima)
- [ ] Crear `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx` (memoizado)
- [ ] Crear `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css` (CSS Modules)
- [ ] Integrar prompt en `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` sin afectar toolbar/viewport

## Flujo Password (solo plugin oficial)
- [ ] Consumir APIs oficiales del plugin (hook/state/provides) para:
  - detectar `password-required`
  - enviar password
  - detectar `invalid-password`
  - detectar `unlocked`
- [ ] Guard clauses + cleanup para evitar stale states

## Testing (Vitest/RTL)
- [ ] Mock plugin password (state + provides)
- [ ] Test: render prompt cuando password requerido
- [ ] Test: submit password llama action oficial
- [ ] Test: invalid password muestra error
- [ ] Test: unlock oculta prompt y render continúa

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
- [ ] (Opcional) Playwright smoke (si aplica al pipeline)


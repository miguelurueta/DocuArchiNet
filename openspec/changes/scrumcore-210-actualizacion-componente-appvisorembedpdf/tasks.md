# SCRUMCORE-210 — Tasks (Signature Plugin + Modal)

## Artefactos OpenSpec
- [x] `proposal.md`
- [x] `design.md`
- [x] `spec.md`
- [x] `tasks.md`

## Dependencias
- [ ] Instalar plugins oficiales: `@embedpdf/plugin-signature`, `@embedpdf/plugin-annotation`, `@embedpdf/plugin-interaction-manager`, `@embedpdf/plugin-selection`, `@embedpdf/plugin-history`
- [ ] Validar `package.json`/`package-lock.json`/tipado TS y compatibilidad con plugins existentes

## Plugins / Registro
- [ ] Extender `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts` para registrar:
  - `InteractionManagerPluginPackage`
  - `SelectionPluginPackage`
  - `HistoryPluginPackage`
  - `AnnotationPluginPackage`
  - `SignaturePluginPackage`
- [ ] Verificar que la registration no se recrea por render (mantener patrón estable actual)

## UI: Toolbar
- [ ] Actualizar `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx` para agregar botón Signature (icono + tooltip)
- [ ] Mantener toolbar desacoplada (no conocer engine/plugins) y memoizada
- [ ] Ubicar el botón Signature después del grupo Rotate (orden: Zoom → Rotate → Signature → …)

## UI: Modal
- [ ] Crear `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`
- [ ] Crear `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.module.css`
- [ ] Accesibilidad modal: `role="dialog"`, `aria-modal`, focus inicial, Escape cierra
- [ ] Modal desacoplado: NO recibe engine/plugins/documentId (solo callbacks/flags)

## Integración Signature (sin lógica custom)
- [ ] Integrar `useSignatureCapability()` en `AppVisorEmbedPdf.tsx` (encapsulado)
- [ ] Implementar `activateSignaturePlacement()` (oficial) al seleccionar firma
- [ ] Auto-close modal al activar placement
- [ ] Integrar `<AnnotationLayer />` en el pipeline (RenderLayer → (Selection Plugin) → AnnotationLayer)
- [ ] Upload: integrar `useSignatureUpload()` (oficial) para PNG/JPG/SVG

## Persistencia temporal
- [ ] Implementar `serializeEntries()` → `localStorage`
- [ ] Implementar `deserializeEntries()` desde `localStorage` al abrir documento
- [ ] Definir key estable por documento (sin exponer al Workbench): `appvisor:embedpdf:annotations:<documentId>`

## Cleanup / Estabilidad
- [ ] Cleanup al cerrar modal/cambiar documento: placement session/listeners/subscriptions (sin leaks)
- [ ] Validar que no rompe zoom/rotate/thumbnails/print/export/pagination/password

## Testing (Vitest/RTL)
- [ ] Actualizar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`:
  - modal open/close
  - render pads draw/type (mock)
  - upload (mock `useSignatureUpload`)
  - placement activation (mock `activateSignaturePlacement`)
  - annotation layer render (smoke)
  - localStorage serialize/deserialize
  - coexistencia plugins actuales (smoke)
- [ ] Asegurar mocks de `@embedpdf/plugin-signature/react` (sin WASM real)

## Documentación enterprise (obligatoria)
Ruta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- [ ] `SCRUM-SCRUMCORE-210-Metadata.md` (Jira+Git+CI/CD)
- [ ] `SCRUM-SCRUMCORE-210-Objetivo-General.md`
- [ ] `SCRUM-SCRUMCORE-210-Responsabilidades-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-210-Arquitectura-Tecnica.md` (Mermaid)
- [ ] `SCRUM-SCRUMCORE-210-Informacion-Tecnica-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-210-APIs-Utilizadas.md`
- [ ] `SCRUM-SCRUMCORE-210-Comportamiento-del-Componente.md`
- [ ] `SCRUM-SCRUMCORE-210-Testing-Enterprise.md`
- [ ] `SCRUM-SCRUMCORE-210-Evidencias-Tecnicas.md`

## Validación
- [ ] `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- [ ] (Opcional) Playwright smoke del visor (si aplica al flujo)

# SCRUMCORE-210 — Spec (AppVisorEmbedPdf · Signature Plugin + Modal)

## Scope
Implementar integración oficial de firma con EmbedPDF:
- Botón Signature en toolbar existente.
- Modal desacoplado para Draw/Type/Upload (APIs oficiales).
- Placement oficial con `useSignatureCapability()` + `activateSignaturePlacement()`.
- Render de firmas con `<AnnotationLayer />` en el pipeline actual.
- Persistencia temporal en `localStorage` usando `serializeEntries()` / `deserializeEntries()`.

## Out of scope (prohibido)
- Canvas custom / render manual / drag manual / coordenadas manuales.
- `pdf-lib`, `fabric.js`, `konva`.
- Viewers paralelos / duplicar toolbar / wrappers innecesarios.
- Backend/API custom para persistencia.

## Dependencias obligatorias
Instalar:
- `@embedpdf/plugin-signature`
- `@embedpdf/plugin-annotation`
- `@embedpdf/plugin-interaction-manager`
- `@embedpdf/plugin-selection`
- `@embedpdf/plugin-history`

## Plugins: registro obligatorio
Registrar mediante patrón existente `createPluginRegistration(...)` en `plugins/pluginRegistration.ts`:
- `InteractionManagerPluginPackage`
- `SelectionPluginPackage`
- `HistoryPluginPackage`
- `AnnotationPluginPackage`
- `SignaturePluginPackage`

## Estructura permitida
Agregar únicamente:
`src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`  
`src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.module.css`

No crear `viewers` paralelos ni nuevos managers.

## Toolbar API (extensión)
Actualizar `presentation/AppPdfToolbar.tsx` con un nuevo botón.

Prop mínima requerida:
```ts
interface AppPdfSignatureModalProps {
  isOpen: boolean;
  onClose(): void;
  /**
   * Dispara el flujo oficial de placement (se implementa en AppVisorEmbedPdf).
   * El modal NO debe conocer engine/plugins/documentId.
   */
  onStartPlacement(): void;
}
```

Toolbar (presentacional) debe exponer handler tipo:
```ts
onToggleSignatureModal(): void
```

## Requerimientos funcionales
### Abrir/cerrar modal
- Click Signature: abre modal.
- Cerrar: botón close y tecla Escape.
- Modal responsive.

### Draw Signature
- Usar exclusivamente `<SignatureDrawPad />`.

### Type Signature
- Usar exclusivamente `<SignatureTypePad />`.

### Upload Signature
- Usar exclusivamente `useSignatureUpload()`.
- Permitir: PNG/JPG/SVG.
- Sin crop custom.

### Placement oficial (obligatorio)
- Usar `useSignatureCapability()` y `activateSignaturePlacement()`.
- Flujo:
  1) usuario selecciona firma en modal
  2) modal se cierra automáticamente
  3) usuario hace click sobre el PDF
  4) EmbedPDF coloca firma

### Rendering oficial (obligatorio)
- Render de firmas vía `<AnnotationLayer />`.
- Integración en pipeline:
  `RenderLayer → (Selection Plugin) → AnnotationLayer`

### Persistencia temporal
- Guardar `serializeEntries()` en `localStorage` asociado al documento (clave definida en implementación).
- Cargar con `deserializeEntries()` al abrir documento.
 - Clave recomendada (encapsulada): `appvisor:embedpdf:annotations:<documentId>`

## Performance / estabilidad
- No recrear plugin registration por render (usar memo/const estable como hoy).
- Handlers estables donde aplique (no cascada de rerenders por scroll).
- Cleanup de sesiones de placement / listeners / subscripciones al cerrar modal o cambiar documento.

## Testing (obligatorio)
Actualizar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`:
- Abre/cierra modal.
- Render de pads (draw/type) sin crash (mock de componentes EmbedPDF si aplica).
- Upload: mock de `useSignatureUpload()` y validación de llamado.
- Placement: mock de `useSignatureCapability()` y validación de `activateSignaturePlacement()`.
- Render annotation: asegurar que `<AnnotationLayer />` está en el árbol cuando el documento está loaded.
- Persistencia: mock `localStorage` y validar serialize/deserialize.
- Coexistencia: no rompe zoom/rotate/thumbnails/print/export/pagination/password (smoke assertions).

Notas:
- Los tests deben mockear APIs de `@embedpdf/plugin-signature/react` (y capas de annotation/selection si aplica). No usar WASM real.

## Documentación enterprise (obligatoria)
Actualizar/crear bajo:
`docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`

Archivos obligatorios:
- `SCRUM-SCRUMCORE-210-Metadata.md`
- `SCRUM-SCRUMCORE-210-Objetivo-General.md`
- `SCRUM-SCRUMCORE-210-Responsabilidades-del-Componente.md`
- `SCRUM-SCRUMCORE-210-Arquitectura-Tecnica.md` (Mermaid)
- `SCRUM-SCRUMCORE-210-Informacion-Tecnica-del-Componente.md`
- `SCRUM-SCRUMCORE-210-APIs-Utilizadas.md`
- `SCRUM-SCRUMCORE-210-Comportamiento-del-Componente.md`
- `SCRUM-SCRUMCORE-210-Testing-Enterprise.md`
- `SCRUM-SCRUMCORE-210-Evidencias-Tecnicas.md`

Diagramas Mermaid obligatorios:
- Arquitectura
- Flujo placement
- Secuencia toolbar/modal/signature
- Estados modal

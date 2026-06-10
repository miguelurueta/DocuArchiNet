# SCRUMCORE-210 — Información Técnica del Componente

## Componente

- Nombre: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

## UI (presentación)

- Toolbar:
  - Ruta: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
  - `React.memo` para estabilidad de render.
- Modal firmas:
  - Ruta: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`
  - Tabs: `Dibujar firma`, `Subir firma`
  - Se removió el tab `Type`.
  - Reset UI:
    - Botón `Limpiar` solo cuando existe trazo.
    - Al usar firma, el modal se resetea para no conservar la firma anterior.
  - Upload:
    - Muestra nombre del archivo.
    - Botón cambia texto `Subir firma` / `Reemplazar firma`.
    - Permite quitar archivo con una `X`.

## Plugins / capabilities usados (relevantes a firma)

- Signature:
  - `@embedpdf/plugin-signature/react`
  - Placement nativo: `activateSignaturePlacement(entryId)`
- Annotation:
  - `@embedpdf/plugin-annotation/react`
  - Delete + commit para persistencia real en PDF.
- Export / Print:
  - `@embedpdf/plugin-export/react`
  - `@embedpdf/plugin-print/react`

## Estilos

- CSS Modules:
  - Modal: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.module.css`


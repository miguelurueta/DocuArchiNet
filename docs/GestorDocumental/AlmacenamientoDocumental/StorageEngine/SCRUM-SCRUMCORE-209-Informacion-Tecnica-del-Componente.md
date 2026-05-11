# SCRUMCORE-209 — Información Técnica del Componente

- Componente: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/`

## UI Password Prompt
- Archivo: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`
- Estilos: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css`
- API pública:
  - `isInvalidPassword?: boolean`
  - `isLoading?: boolean`
  - `onSubmit(password: string): void`
- Accesibilidad:
  - `role="dialog"` en el overlay
  - `aria-label` en input y botones
  - mensaje inválido con `role="alert"`

## Autofill hardening
- Se evita autofill con:
  - `autoComplete="new-password"`
  - `data-lpignore="true"`, `data-1p-ignore="true"`, `data-form-type="other"`

## Integración DocumentManager
- Apertura inicial: `provides.openDocumentUrl({ url, autoActivate: true })`
- Reintento: `provides.retryDocument(documentId, { password })`
- Detección password: `provides.onDocumentError(...)` + `PdfErrorCode.Password`
- Éxito/fallo real: `OpenDocumentResponse.task.wait(...)`


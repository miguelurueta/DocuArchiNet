# SCRUMCORE-209 — Información Técnica del Componente

## Componente
- Nombre: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- Prompt UI: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`

## Configuración demo (sin hardcode “externo”)
- Hook: `src/app/Components/UI/AppVisorEmbedPdf/hooks/useDemoPdfUrl.ts`
- Default local: `/demo/20260410DiagnosticoCCV_protected.pdf`
- Override por env: `VITE_EMBEDPDF_DEMO_PDF`

## Props públicas (sin cambios en esta fase)
- `fileUrl?: string`
- `className?: string`
- `style?: React.CSSProperties`

## UX / Accesibilidad del prompt
- Prompt como `role="dialog"` + `aria-label="Documento protegido"`.
- Input con `autoComplete="new-password"` y flags (`data-lpignore`, `data-1p-ignore`) para minimizar autofill de gestores.
- Toggle de visibilidad de contraseña (iconos Ant Design: `EyeOutlined` / `EyeInvisibleOutlined`).

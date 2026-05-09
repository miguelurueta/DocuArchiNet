# SCRUMCORE-209 — Design (Password Protected Plugin)

## Objetivo
Extender `AppVisorEmbedPdf` para soportar PDFs protegidos con contraseña usando **exclusivamente** el plugin oficial:
`@embedpdf/plugin-password-protected`.

## Principios (no negociables)
- Prohibido implementar lógica custom de password (decrypt manual, hacks Pdfium, parsers).
- Todo el flujo debe depender del plugin oficial y sus APIs/hook oficiales.
- Encapsulación total: consumidores (Workbench) no conocen estados ni APIs del plugin.
- Mantener virtualización/lazy rendering/viewport estable y el toolbar existente.

## UI mínima
Se agrega un prompt desacoplado y memoizado:
`presentation/AppPdfPasswordPrompt.tsx` (+ CSS module).

Ubicación: dentro del visor, en overlay centrado (no modal complejo).

## Flujo
1. Documento se intenta abrir (Document Manager).
2. Si el plugin indica `password-required`:
   - Se muestra `AppPdfPasswordPrompt`.
3. Usuario envía password:
   - Se llama a la acción oficial del plugin (unlock/submit).
4. Si password inválido:
   - Prompt muestra error y permite reintentar.
5. Si unlock correcto:
   - Prompt desaparece y continúa render normal (Viewport/Scroller/RenderLayer).

## Accesibilidad
- Input tipo password con `aria-label`.
- Botón submit con `aria-label` y estado disabled cuando `isSubmitting`.
- Focus management básico: al mostrar prompt, enfocar input.

## Anti-rerender
- `AppPdfToolbar` continúa memoizado (sin cambios de firma innecesarios).
- `AppPdfPasswordPrompt` se memoiza.
- Handlers del prompt con `useCallback`.


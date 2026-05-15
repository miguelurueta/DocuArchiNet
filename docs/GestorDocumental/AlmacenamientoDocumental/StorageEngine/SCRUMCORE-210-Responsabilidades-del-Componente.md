# SCRUMCORE-210 — Responsabilidades del Componente

## Responsabilidades principales

- Encapsular todo el pipeline EmbedPDF/Pdfium (engine + plugins + UI) para visualizar y operar PDFs.
- Implementar UI enterprise desacoplada (toolbar + modal) para firma electrónica *gráfica*.
- Gestionar estados UI de firma (modal abierto/cerrado, bloqueo UX, disponibilidad de acciones).
- Asegurar consistencia entre lo que se ve en pantalla y lo que se exporta/imprime (commit previo).

## Qué NO debe hacer

- No debe exponer APIs `@embedpdf/*` al Workbench/consumidores.
- No debe implementar firma digital criptográfica PKI/PAdES (eso requiere backend/PKI).
- No debe aplicar “hacks” por CSS para eliminar elementos del pipeline interno del engine.

## Estrategia de desacoplamiento

- `AppPdfToolbar` es presentacional y solo recibe props/handlers.
- `AppPdfSignatureModal` es UI (presentación) y emite acciones tipadas (`onStartPlacement`).
- Toda lógica EmbedPDF permanece en `AppVisorEmbedPdf.tsx` (hooks/capabilities).


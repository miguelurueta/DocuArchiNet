# SCRUMCORE-208 — Arquitectura Técnica

## Estructura / módulos impactados
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## Pipeline (alto nivel)
```mermaid
flowchart TD
  A[Pdfium Engine] --> B[EmbedPDF Host]
  B --> C[Scroll Plugin (@embedpdf/plugin-scroll)]
  C --> D[useScroll(documentId)]
  D --> E[Pagination Overlay (Prev/Indicator/Next)]
  C --> F[Scroller + Virtualization]
  F --> G[Viewport]
  G --> H[RenderLayer]
```

## Secuencia navegación
```mermaid
sequenceDiagram
  participant U as Usuario
  participant UI as Pagination Overlay
  participant SP as Scroll Plugin (provides)
  participant V as Viewport/Scroller

  U->>UI: click Anterior/Siguiente
  UI->>SP: scrollToPreviousPage()/scrollToNextPage()
  SP->>V: actualiza posición/estado interno
  V-->>UI: scroll.state.currentPage/totalPages
  UI-->>U: indicador X/Y actualizado
```

## Estados / guardrails
- Si `scroll.provides` es `null` o no expone métodos, los handlers hacen no-op (no crash).
- UI usa `scroll.state` como data source, sin duplicar estado de paginación.


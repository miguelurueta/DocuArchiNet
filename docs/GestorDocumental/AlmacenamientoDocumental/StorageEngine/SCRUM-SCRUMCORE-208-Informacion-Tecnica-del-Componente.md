# SCRUMCORE-208 — Información Técnica del Componente

- Componente: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/`
- Estrategia paginación:
  - Hook: `useScroll(documentId)` (plugin oficial)
  - Indicadores: `scroll.state.currentPage`, `scroll.state.totalPages`
  - Acciones: `scroll.provides?.scrollToPreviousPage()`, `scroll.provides?.scrollToNextPage()`
- UI:
  - Overlay flotante centrado inferior dentro del visor.
  - Iconos Ant Design para navegación.
  - Estilos: CSS Modules (`AppVisorEmbedPdf.module.css`)
- Compatibilidad:
  - No altera integración de Zoom/Rotate/Thumbnails/Print/Export.
  - Mantiene virtualización nativa (Scroller) y lazy rendering.


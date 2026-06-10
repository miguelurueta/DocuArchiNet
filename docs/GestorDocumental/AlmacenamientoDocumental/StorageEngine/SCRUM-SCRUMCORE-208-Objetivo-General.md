# SCRUMCORE-208 — Objetivo General

Extender el componente reusable `AppVisorEmbedPdf` agregando paginación **nativa** basada exclusivamente en el plugin oficial `@embedpdf/plugin-scroll`, sin introducir lógica custom (listeners/observers/cálculos viewport) y sin afectar funcionalidades existentes (zoom, rotate, thumbnails, print, export, virtualización).

Resultado esperado:
- Controles `Anterior / Indicador / Siguiente` visibles como overlay en el visor.
- Indicador basado en `scroll.state.currentPage` y `scroll.state.totalPages`.
- Navegación usando `scroll.provides?.scrollToPreviousPage()` y `scroll.provides?.scrollToNextPage()`.


# SCRUMCORE-208 — Comportamiento del Componente

## Lifecycle relevante
- Al cargar documento, `useScroll(documentId)` provee:
  - `currentPage` y `totalPages` (estado oficial).
- El overlay se re-renderiza cuando cambia el estado oficial del scroll plugin.

## Estados
- `success`: overlay visible y funcional si `provides` existe.
- `fallback`: si `provides` es `null`, botones no crashean (no-op) y el indicador continúa mostrando el estado disponible.

## Performance
- No se agregan listeners manuales de scroll para paginación.
- No se implementa tracking custom de página.
- Se preserva virtualización nativa del Scroller.


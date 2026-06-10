# SCRUMCORE-208 — Design (Paginación nativa EmbedPDF)

## Objetivo
Agregar controles de paginación enterprise al toolbar existente de `AppVisorEmbedPdf` usando **únicamente** capacidades oficiales del Scroll plugin de EmbedPDF.

## Principios (no negociables)
- Prohibido implementar lógica custom de paginación (listeners, observers, cálculos viewport, tracking manual).
- La fuente de verdad de paginación es `useScroll(documentId)`:
  - `scroll.state.currentPage`
  - `scroll.state.totalPages`
  - `scroll.provides?.scrollToNextPage()`
  - `scroll.provides?.scrollToPreviousPage()`
  - `scroll.provides?.scrollToPage(...)` (si aplica)
- Encapsulación: `DocumentosWorkbench` no conoce `useScroll` ni estados internos.
- Mantener performance: no introducir renders extra por scroll/virtualización.

## UI / Layout
Toolbar (existente) se extiende para incluir un bloque de paginación:

```
[ ← ]  Página X de Y  [ → ]    ... (Zoom/Rotate/Thumb/Print/Export existentes)
```

- En desktop: paginación visible en la misma fila.
- En mobile: paginación compacta (texto corto) sin romper el layout actual.

## Interacción
- Click `←`: ejecuta `scroll.provides?.scrollToPreviousPage()`.
- Click `→`: ejecuta `scroll.provides?.scrollToNextPage()`.
- Indicador renderiza `currentPage` y `totalPages` desde `scroll.state`.

## Accesibilidad
- Botones con `aria-label` claros: `"Página anterior"`, `"Página siguiente"`.
- Indicador con `aria-label`: `"Indicador de página"`.

## Anti-rerender
- `AppPdfToolbar` sigue memoizado con `React.memo`.
- Los handlers de paginación se estabilizan con `useCallback` (en `AppVisorEmbedPdf`) para minimizar renders del toolbar.


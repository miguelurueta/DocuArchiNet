# SCRUMCORE-208 — Spec (Paginación nativa EmbedPDF)

## Alcance
Extender `AppVisorEmbedPdf` para soportar paginación nativa usando `@embedpdf/plugin-scroll`.

## Requisitos funcionales
1. Debe renderizar en el toolbar:
   - Botón página anterior
   - Indicador `Página {currentPage} de {totalPages}`
   - Botón página siguiente
2. Debe usar únicamente:
   - `useScroll(documentId)`
   - `scroll.state.currentPage`
   - `scroll.state.totalPages`
   - `scroll.provides?.scrollToPreviousPage()`
   - `scroll.provides?.scrollToNextPage()`
3. Estado inicial:
   - Los botones deben tolerar `provides` faltante (guard clause).
4. No debe romper:
   - Zoom / Rotate / Thumbnails / Print / Export existentes
   - Virtualización (Scroller) y lazy rendering

## Requisitos de arquitectura
- `DocumentosWorkbench` permanece sin lógica de paginación.
- No se crea estado duplicado de paginación fuera del estado oficial del plugin.
- Se actualiza únicamente `AppVisorEmbedPdf` y `AppPdfToolbar` (presentational) + tests + docs.

## API obligatoria (toolbar)
Extender `AppPdfToolbarProps`:

```ts
export interface AppPdfToolbarProps {
  zoomLevel: number;

  currentPage: number;
  totalPages: number;
  onPreviousPage(): void;
  onNextPage(): void;

  onZoomIn(): void;
  onZoomOut(): void;
  onResetZoom(): void;

  onToggleThumbnails(): void;
  isThumbnailOpen: boolean;

  isZoomDisabled?: boolean;

  onRotateLeft(): void;
  onRotateRight(): void;

  onPrint(): void;
  onExport(): void;
}
```

## Testing mínimo (Vitest/RTL)
- Mock de `useScroll(...)`.
- Validar render del indicador `Página X de Y`.
- Validar click en `Página anterior/siguiente` ejecuta `scrollToPreviousPage/scrollToNextPage`.
- Validar que con `provides = null` no crashea.

## Documentación enterprise
Actualizar/crear los 9 docs en:
`docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/` para `SCRUMCORE-208`, incluyendo diagramas Mermaid del flujo de paginación.


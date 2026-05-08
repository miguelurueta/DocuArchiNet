# SCRUMCORE-206 — Comportamiento del componente (AppVisorEmbedPdf)

## Estados
- `loading engine`: loader engine.
- `loading document`: loader documento mientras `activeDocumentId` no existe / `DocumentContent` loading.
- `success`: visor renderiza PDF virtualizado.
- `error`: error state básico.
- `empty`: empty state básico.

## Zoom + Rotate (regla)
- Si `rotation === 0`: Zoom habilitado (zoom in/out/reset).
- Si `rotation !== 0`: Zoom deshabilitado (botones disabled con tooltip de estabilidad).

## Scroll-to-top (FAB)
- Overlay inferior derecho (tipo WhatsApp).
- Aparece solo cuando el usuario está suficientemente abajo (por `scrollTop`/`clientHeight`).
- Acción: `viewport.scrollTo({ x: 0, y: 0, behavior: "smooth" })`.

- Rotación se delega al plugin oficial `@embedpdf/plugin-rotate`.
- Toolbar dispara:
  - `rotateBackward()` (izquierda)
  - `rotateForward()` (derecha)
  - `setRotation(0)` (reset)

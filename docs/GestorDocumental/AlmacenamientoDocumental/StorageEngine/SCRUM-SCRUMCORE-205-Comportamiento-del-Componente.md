# SCRUMCORE-205 — Comportamiento del componente

- Click botón thumbnails alterna `isThumbnailOpen`.
- Render thumbnails condicional sin wrappers/componentes extra.
- Render principal del PDF se mantiene intacto.

## Auto-scroll (plugin oficial)

- El auto-scroll del panel de thumbnails se delega al plugin oficial `@embedpdf/plugin-thumbnail`.
- Configuración aplicada al registrar el plugin:
  - `autoScroll: true`
  - `scrollBehavior: "smooth"`

Esto permite que, al cambiar la página actual en el visor, el panel de thumbnails se desplace automáticamente hacia la miniatura correspondiente sin lógica custom.

## Página activa (highlight visual)

- La miniatura “activa” se resalta solo a nivel visual (CSS Modules).
- La página actual se obtiene del estado oficial del plugin scroll:
  - `useScroll(documentId).state.currentPage`

## Click thumbnail → navegación

- Al hacer click en una miniatura, la navegación se ejecuta usando la capability oficial de scroll:
  - `scroll.provides.scrollToPage({ pageNumber, behavior: "smooth", alignY: 0 })`
- No se implementa “navegación manual” fuera de las capabilities oficiales de EmbedPDF.

## Accesibilidad del panel

- El panel de thumbnails usa `aria-label="Panel thumbnails"` para que:
  - los tests E2E (Playwright) puedan localizarlo sin colisionar con el botón de toolbar (`aria-label="Abrir thumbnails"`).

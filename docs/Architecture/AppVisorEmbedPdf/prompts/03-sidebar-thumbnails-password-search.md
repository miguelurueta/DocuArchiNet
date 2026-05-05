# Prompt 03 — Sidebar + Thumbnails + Password + Search (plugins)

Objetivo: extender el visor con UX enterprise manteniendo toolbar limpia y plugins desacoplados.

## Alcance

- `AppPdfSidebar` responsive/collapsable (desktop/tablet/mobile).
- Thumbnails panel (collapsable) con cache.
- Password-protected:
  - detectar `password_required`
  - modal (`AppModal`) para solicitar contraseña
  - retry del open document con password
- Search:
  - UI desacoplada en panel propio
  - navegación de resultados

## Criterios de aceptación

- PDF con contraseña abre con flujo de modal.
- Thumbnails no bloquean render principal (lazy load).
- Search no re-renderiza viewport completo.


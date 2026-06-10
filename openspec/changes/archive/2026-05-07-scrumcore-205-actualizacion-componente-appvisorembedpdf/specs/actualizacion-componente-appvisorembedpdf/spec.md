# Spec — SCRUMCORE-205 (Thumbnail colapsable, plugin oficial) AppVisorEmbedPdf

## Objetivo

Extender `AppVisorEmbedPdf` para incluir thumbnails colapsables usando exclusivamente `@embedpdf/plugin-thumbnail` (plugin oficial), manteniendo encapsulación, performance y estabilidad de render.

## Alcance

Incluye:
- Botón en toolbar existente para abrir/cerrar thumbnails (estado `isThumbnailOpen` interno al visor).
- Render de thumbnails usando el componente oficial del plugin directamente en `AppVisorEmbedPdf.tsx` (sin wrappers).
- Comportamiento colapsable únicamente visual/layout.

No incluye:
- Search, annotations, rotate, print/download (por defecto).
- Sidebars complejos, tabs, panel managers.
- Lógica custom de thumbnails (render/cache/observers/virtualización/navegación manual).
- Crear `AppPdfThumbnailPanel.tsx`.

## Reglas de arquitectura

- Consumers no importan `@embedpdf/*`; todo queda encapsulado en `src/app/Components/UI/AppVisorEmbedPdf/`.
- Hooks/capabilities de plugins deben ejecutarse dentro de `<EmbedPDF>`.
- CSS Modules (o estrategia visual del proyecto, sin mezclar).
- No crear wrappers innecesarios.
- Memoización obligatoria: `React.memo(AppPdfToolbar)`; no memoizar componentes internos del plugin.

## UX mínima

- Toggle `[☰ Thumbnails]` (o icono equivalente) en toolbar actual.
- Click abre thumbnails; click nuevamente colapsa.
- Al cerrar thumbnails, el viewport ocupa el ancho completo.

## Testing mínimo

- Vitest/RTL: toggle open/close thumbnails + toolbar interaction + render condicional thumbnails.
- Re-render: toolbar no debe rerenderizar por scroll (validación mínima por test).
- Playwright: toggle thumbnails + re-render estable (sin warnings hooks).

## Documentación enterprise

Generar/actualizar los 9 documentos `SCRUM-SCRUMCORE-205-*.md` bajo:
- `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`

Incluir diagramas Mermaid:
- arquitectura
- flujo thumbnails open/close
- interacción toolbar → thumbnails
- responsabilidades y límites

## Criterios de aceptación

- Plugin oficial funcionando: `@embedpdf/plugin-thumbnail` registrado con `createPluginRegistration(ThumbnailPluginPackage)`.
- Botón de toolbar abre/cierra thumbnails correctamente.
- No existe lógica custom de thumbnails.
- Virtualización/render principal intactos.
- Sin warnings TS/React.
- Workbench no recibe lógica del plugin.

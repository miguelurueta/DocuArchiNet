# Spec — SCRUMCORE-206 (Rotate Plugin oficial) AppVisorEmbedPdf

## Objetivo

Extender `AppVisorEmbedPdf` incorporando rotación usando exclusivamente el plugin oficial de EmbedPDF (`@embedpdf/plugin-rotate`), manteniendo encapsulación y performance.

## Alcance

Incluye:
- Registrar `RotatePluginPackage` vía `createPluginRegistration(...)`.
- Exponer acciones de rotación en el toolbar existente (UI minimalista).

No incluye:
- Features no solicitadas (search/thumbnails extra/annotations/print/download/etc).

## Reglas de arquitectura

- Consumers no importan `@embedpdf/*`; todo queda dentro de `src/app/Components/UI/AppVisorEmbedPdf/`.
- Hooks/capabilities de plugins se ejecutan dentro de `<EmbedPDF>`.
- No lógica custom de rotación; usar el comportamiento nativo del plugin.
- Mantener toolbar memoizada (evitar rerenders por scroll/render).

## UX mínima

- Botón(es) en toolbar para rotar.
- Rotación refleja el estado actual (si aplica) sin romper scroll/virtualización/render.

## Testing mínimo

- Vitest/RTL: toolbar render + click rotate invoca capability oficial.
- Playwright: interacción rotate + re-render estable (sin warnings hooks).

## Documentación enterprise

Generar/actualizar 9 docs `SCRUM-SCRUMCORE-206-*.md` bajo:
- `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`

Incluir Mermaid:
- arquitectura
- flujo rotate → viewport/render
- estados básicos y responsabilidades

## Criterios de aceptación

- Rotate plugin oficial funcionando.
- Zoom/thumbnails/virtualización siguen funcionando.
- Sin warnings TS/React.
- Workbench permanece limpio.

## Context

SCRUMCORE-204 pide una actualización enterprise de `AppVisorEmbedPdf` incorporando:
- toolbar (UI)
- zoom (vía plugin/capability de EmbedPDF)

Actualmente `AppVisorEmbedPdf` (SCRUMCORE-201/202/203) solo incluye plugins base (document-manager, viewport, scroll, render) y explicitamente NO incluye zoom/toolbar.

## Goals

- Agregar toolbar enterprise **sin** romper encapsulación (consumers no importan `@embedpdf/*`).
- Incorporar zoom usando **exclusivamente** el plugin oficial `@embedpdf/plugin-zoom` (sin lógica custom).
- Mantener virtualización + lazy rendering (Scroll + RenderLayer) sin regresiones.
- Evitar regresiones de hooks/re-render (Rules of Hooks) y reducir rerenders con memoización.

## Non-goals

- rotate, search, thumbnails, annotations, signatures, password, print/download, sidebar, shortcuts avanzados.

## Key decisions (pendientes de confirmar en tasks)

1) **Dónde vive el estado de zoom**
   - Interno al visor, controlado por toolbar local.
2) **Qué comandos expone la toolbar**
   - Mínimo: zoom in, zoom out, reset.
3) **Arquitectura toolbar**
   - Presentacional y desacoplada (`AppPdfToolbar.tsx`) con `React.memo`.
4) **Accesibilidad**
   - Botones con `aria-label` y foco/teclado básico.

## Risks

- Riesgo de regresión “Rules of Hooks” si hooks de capabilities se mueven fuera del provider `<EmbedPDF>`.
- Riesgo de performance/re-render al cambiar zoom con PDFs grandes si no se memoiza la toolbar.

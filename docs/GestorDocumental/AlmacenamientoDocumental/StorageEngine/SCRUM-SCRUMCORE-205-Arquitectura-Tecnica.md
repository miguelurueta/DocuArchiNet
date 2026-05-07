# SCRUMCORE-205 — Arquitectura técnica

## Flujo

```mermaid
flowchart TD
  A[AppVisorEmbedPdf] --> B[EmbedPDF Provider]
  B --> C[ThumbnailPluginPackage]
  A --> D[AppPdfToolbar (memo)]
  D --> E[onToggleThumbnails]
  E --> A
  A --> F[ThumbnailsPane + ThumbImg]
  A --> G[Viewport + Scroller + RenderLayer]
```

## Integración oficial (sin lógica custom)

- Plugin thumbnails: `@embedpdf/plugin-thumbnail`
  - Registro: `createPluginRegistration(ThumbnailPluginPackage, { autoScroll: true, scrollBehavior: "smooth" })`
- Panel thumbnails: render directo desde el plugin:
  - `ThumbnailsPane` + `ThumbImg` (`@embedpdf/plugin-thumbnail/react`)
- Estado UI open/close:
  - `isThumbnailOpen` vive únicamente en `AppVisorEmbedPdf.tsx`

## Interacción toolbar → thumbnails (Mermaid)

```mermaid
sequenceDiagram
  participant U as User
  participant T as AppPdfToolbar
  participant V as AppVisorEmbedPdf
  participant P as Thumbnail Plugin

  U->>T: Click icono thumbnails
  T->>V: onToggleThumbnails()
  V->>V: isThumbnailOpen = !isThumbnailOpen
  V->>P: (render) ThumbnailsPane(documentId)
```

## Auto-scroll thumbnails (Mermaid)

```mermaid
sequenceDiagram
  participant S as Scroll Plugin
  participant P as Thumbnail Plugin
  participant UI as ThumbnailsPane

  S-->>P: current page cambia
  P-->>UI: onScrollTo({top, behavior})
  UI-->>UI: scrollTo(top, behavior)
```

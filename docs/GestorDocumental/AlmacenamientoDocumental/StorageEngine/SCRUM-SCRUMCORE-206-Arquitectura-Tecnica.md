# SCRUMCORE-206 — Arquitectura técnica (AppVisorEmbedPdf)

## Plugins (EmbedPDF)
- `DocumentManagerPluginPackage`
- `ViewportPluginPackage`
- `ScrollPluginPackage`
- `RenderPluginPackage`
- `ZoomPluginPackage`
- `ThumbnailPluginPackage`
- `RotatePluginPackage`

## Flujo de render (resumen)
```mermaid
flowchart TD
  A[AppVisorEmbedPdf] --> B[EmbedPDF engine + plugins]
  B --> C[DocumentManager openDocumentUrl]
  C --> D[Viewport]
  D --> E[Scroller (virtualización)]
  E --> F[RenderLayer por página]
  E --> G[Rotate wrapper por página cuando rotation != 0]
```

## Guardrails UX (estabilidad)
- El “jump” observado ocurre al combinar `Zoom +` con rotaciones 90°/270° (layout + transform).
- Se implementa un guardrail: deshabilitar zoom cuando `rotation !== 0`.

## UI overlay
- El botón flotante “Ir arriba” vive como overlay en `.main` (contenedor relativo) para no quedar por debajo del scroller.

```mermaid
flowchart TD
  A[AppVisorEmbedPdf] --> B[EmbedPDF Provider]
  B --> C[RotatePluginPackage]
  A --> D[AppPdfToolbar (memo)]
  D --> E[onRotateLeft/onRotateRight/onResetRotation]
  E --> A
  A --> F[Viewport + Scroller + RenderLayer]
```

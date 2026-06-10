# SCRUMCORE-205 — Responsabilidades del componente

- Registrar `ThumbnailPluginPackage` y renderizar thumbnails con componentes oficiales del plugin (sin lógica custom).
- Mantener `isThumbnailOpen` únicamente dentro de `AppVisorEmbedPdf.tsx`.
- Mantener Workbench/consumers sin conocimiento de thumbnails/plugins.


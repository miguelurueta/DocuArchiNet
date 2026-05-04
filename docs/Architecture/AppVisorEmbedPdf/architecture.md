# Arquitectura — `AppVisorEmbedPdf`

Documento base para el visor PDF modular del proyecto.

## Principios

- Headless: `AppPdfViewer` orquesta engine + estado + plugins (UI desacoplada).
- Engine: EmbedPDF **Core** (PDFium) en fases posteriores (no viewer monolítico).
- Plugins dinámicos por `capabilities` y configuración por operación.
- Performance: virtualización, render incremental (solo visibles + buffer), RenderQueue con prioridades.
- Capas (layers): `canvas`, `text`, `annotations`, `signatures`.
- Seguridad: validación de PDFs, límites por tamaño/páginas, sandbox de render, password flow.

## Roadmap

Ver `openspec/changes/scrumcore-197-crea-componente-appvisorembedpdf-01-fe/specs/app-appvisorembedpdf-01-fe/spec.md`
y los prompts posteriores (02A/02B/03/04/05).


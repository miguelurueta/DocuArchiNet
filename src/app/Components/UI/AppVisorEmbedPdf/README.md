# AppVisorEmbedPdf (01-FE)

Scaffold y contratos mínimos para un visor PDF modular (headless + plugins) que en fases posteriores usará **EmbedPDF Core (PDFium)** como engine.

Referencia de arquitectura: `docs/Architecture/AppVisorEmbedPdf/architecture.md`.

## Propósito

- Definir contratos estables para implementar incrementalmente un visor PDF enterprise.
- Mantener desacople: UI (toolbar/sidebar/viewport) no debe acoplarse al engine.
- Preparar `capabilities` para habilitar plugins dinámicos por operación/rol.

## No objetivos (esta fase)

- Integración real de EmbedPDF Core / PDFium / WASM / Workers.
- Implementación de UI del visor.
- Plugins (zoom/rotate/search/thumbnails/password/signatures/annotations/print/download).
- Integración en consumidores (`DocumentosWorkbench`).

## Export público

El módulo exporta **tipos** desde `src/app/Components/UI/AppVisorEmbedPdf/index.ts`.

## Roadmap incremental (prompts)

- 02A: core visual + plugins básicos (zoom/rotate/download) + integración Workbench (feature flag + fallback).
- 02B: hardening (RenderQueue, error taxonomy, lifecycle, guardrails, tests).
- 03: engine real (EmbedPDF Core/PDFium) + workers + cache.
- 04: sidebar + thumbnails + password + search.
- 05: signatures + annotations + print/download enterprise.

## Mapeo desde `AppVisorPdf`

- `AppVisorPdfInput` → `AppPdfSource`
- `createPdfjsEngine` → `EmbedPdfEngine` (adapter interface)
- Viewport/toolbar monolíticos → UI modular + plugins

## Nota

Usar **EmbedPDF Core**, no viewer monolítico.


# Design — SCRUMCORE-197: `AppVisorEmbedPdf` (01-FE)

## Objetivo de la fase 01-FE
Definir y crear el scaffold + contratos mínimos del componente shared `AppVisorEmbedPdf`, alineado a:
- `docs/Architecture/AppVisorEmbedPdf/architecture.md` (arquitectura objetivo)
- Prompt 01 (scaffold + contratos)

Esta fase 01-FE NO pretende entregar un visor funcional todavía; prepara la base para 02A/02B.

## Alcance (incluido)
- Estructura de módulo: `src/app/Components/UI/AppVisorEmbedPdf/`
- Contratos mínimos:
  - `domain/pdf.types.ts` (source/loadState/capabilities)
  - `domain/viewerApi.types.ts` (API headless mínima)
  - `engine/embedPdfEngine.types.ts` (adapter interface)
- Documentación mínima del componente: `README.md`
- Registro de implementación: `docs/Architecture/AppVisorEmbedPdf/implementation-log.md` (Capa 01)

## No alcance (excluido)
- Integración real EmbedPDF Core (PDFium) / WASM / workers.
- UI completa (toolbar/sidebar/viewport).
- Plugins (zoom/rotate/search/thumbnails/password/signatures/annotations/print/download).
- Integración en consumidores (`DocumentosWorkbench`).

## Decisiones
- **Headless-first:** separar contratos engine/UI para sostenibilidad.
- **Plugins dinámicos:** `capabilities` permitirá habilitar plugins por operación/rol.
- **Dominio agnóstico:** el módulo no importa nada desde `src/modules/**`.


# SCRUMCORE-203 — Responsabilidades del componente

## Componente

- Nombre: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/`

## Responsabilidades principales

- Inicializar el engine Pdfium (EmbedPDF) y mantenerlo encapsulado.
- Registrar plugins base (document manager, viewport, scroll, render).
- Abrir documento por `fileUrl` o demo local.
- Renderizar pipeline de visor (viewport/scroller/render layer).
- Exponer un API mínimo (props) sin filtrar dependencias internas.

## Qué encapsula

- Dependencias `@embedpdf/*`.
- Lógica de engine/plugins/providers.
- Manejo de estados del visor.
- Estrategia de virtualización/lazy rendering (EmbedPDF).

## Qué NO debe hacer

- No debe listar ni permitir seleccionar documentos (eso es responsabilidad del consumer si se requiere en otra UX).
- No debe implementar features fuera de alcance (zoom/rotate/toolbar/search/thumbnails/annotations/signatures/password/print/download).
- No debe depender del módulo consumidor (Workbench) para funcionar.

## Límites funcionales

- `fileUrl` debe ser accesible desde el navegador (paths válidos, CORS si aplica).
- PDFs extremadamente grandes pueden degradar performance (memoria/tiempo).

## Restricciones técnicas

- React + TypeScript.
- CSS Modules (o estrategia visual del proyecto; sin mezclar Tailwind/styled-components).
- Mantener reglas de hooks/context: hooks dependientes del provider se ejecutan dentro de `<EmbedPDF>`.

## Estrategia de desacoplamiento

- Consumers importan solo el componente: `AppVisorEmbedPdf`.
- Consumers no importan ni interactúan con plugins/engine directamente.

## Responsabilidades del consumidor

- Proveer `fileUrl` cuando se requiera un documento específico.
- Asegurar accesibilidad del recurso (routing local, proxy, CORS, auth fuera del visor).
- Definir layout/espacio del contenedor (alto/ancho) donde se monta el visor.

## Context

El ticket `SCRUMCORE-191` (02-FE) extiende el componente shared `AppVisorPdf` creado en 01-FE
para incorporar un **motor real de render PDF** basado en `pdfjs-dist`, con requisitos de
rendimiento para PDFs grandes (virtualizaci\u00f3n + cache).

Existe contexto de arquitectura en `docs/Architecture/AppVisorPdf/VisorPdf.md` y ya existe
la carpeta `src/app/Components/UI/AppVisorPdf/` con contratos UI (toolbar, estados, input).
Este ticket introduce la capa `engine/` y un viewport de presentaci\u00f3n que renderiza canvas
de forma incremental.

## Goals / Non-Goals

**Goals:**
- Implementar `PdfEngine` desacoplado de UI usando `pdfjs-dist`.
- Render incremental: **no** renderizar todas las p\u00e1ginas upfront; renderizar p\u00e1gina activa + buffer.
- Cache por `pageNumber|zoom` con pol\u00edtica LRU y l\u00edmites razonables.
- Manejo de cancelaci\u00f3n con `AbortSignal` para renders concurrentes/cambios de input.
- UI viewport (canvas + scrolling/virtualizaci\u00f3n) en `presentation/`.
- Mantener TypeScript estricto (sin `any`) y contratos testeables.

**Non-Goals:**
- Implementar anotaciones/fabric o text-layer completo si compromete el alcance.
- Thumbnails completos (solo si no complica; no obligatorios).
- Integrar `AppVisorPdf` en m\u00f3dulos/rutas del producto (el componente sigue siendo shared).

## Decisions

1) **Separaci\u00f3n Engine vs UI**
- **Decision:** encapsular pdf.js en `src/app/Components/UI/AppVisorPdf/engine/pdfjsEngine.ts`
  implementando el contrato `PdfEngine` (load/renderPage/destroy).
- **Why:** desacopla UI del motor, facilita tests (mock/spies) y permite reemplazar engine.
- **Alternatives:** consumir pdf.js directamente en componentes de presentaci\u00f3n.

2) **Virtualizaci\u00f3n por p\u00e1gina activa + buffer**
- **Decision:** el viewport renderiza la p\u00e1gina activa y un buffer configurable (default 1)
  alrededor, evitando render de p\u00e1ginas lejanas.
- **Why:** PDFs grandes deben mantenerse responsivos y sin freeze.
- **Alternatives:** render all pages upfront (anti-patr\u00f3n legacy) o render solo 1 p\u00e1gina sin prefetch.

3) **Cache LRU por `pageNumber|zoom`**
- **Decision:** cachear resultados/render targets por key `pageNumber|zoom` con evicci\u00f3n LRU,
  priorizando `maxCacheEntries` (default 12). Si la estimaci\u00f3n de bytes no es estable, se omite
  `maxCacheBytes` inicialmente.
- **Why:** el costo de render es alto; cache reduce latencia al navegar/zoom.
- **Alternatives:** cache sin l\u00edmites (riesgo de memoria) o sin cache (peor UX en PDFs grandes).

4) **Cancelaci\u00f3n y limpieza**
- **Decision:** `renderPage` acepta `AbortSignal`; el viewport aborta renders al cambiar input/zoom/p\u00e1gina,
  y `destroy()` aborta cualquier render en curso, limpia cache y libera `pdfDocument`.
- **Why:** evita trabajo innecesario y fugas de memoria/referencias.
- **Alternatives:** ignorar cancelaci\u00f3n y tolerar render race conditions.

5) **Worker pdf.js**
- **Decision:** configurar worker de pdf.js de forma local (bundle-friendly) para Vite/React,
  documentando troubleshooting (CORS/worker).
- **Why:** pdf.js requiere worker para buen rendimiento y evitar bloqueo main thread.
- **Alternatives:** worker CDN (fragilidad) o deshabilitar worker (no recomendado).

## Risks / Trade-offs

- **[Bundle size]** \u2192 pdfjs-dist aumenta bundle; mitigar con code-splitting del engine y revisar build.
- **[CORS/worker issues]** \u2192 documentar configuraci\u00f3n de worker y errores comunes.
- **[Memoria en PDFs grandes]** \u2192 l\u00edmites de cache + `destroy()` agresivo al cambiar input.
- **[Races en render]** \u2192 abort + ignorar resultados de renders obsoletos.

## Migration Plan

1) Agregar contrato `PdfEngine` y tipos `PdfLoadResult/PdfRenderRequest/PdfRenderResult`.
2) Implementar `engine/pdfjsEngine.ts` con `load()` + `renderPage()` y `AbortSignal`.
3) Crear `presentation/VisorPdfViewport.tsx` con canvas y virtualizaci\u00f3n (p\u00e1gina activa + buffer).
4) Conectar `AppVisorPdf` para usar engine + viewport, mostrando estados `loading/error` reales.
5) Implementar cache LRU y limpieza en cambios de input/zoom.
6) Agregar fixtures y tests obligatorios (load + render page1, zoom invalidation, virtualizaci\u00f3n no render pages lejanas).
7) Actualizar `README.md` con performance/troubleshooting/limitaciones.

## Open Questions

- \u00bfSe requiere text-layer accesible en esta fase o se prioriza canvas-only?
- \u00bfQu\u00e9 heur\u00edstica de buffer/prefetch es mejor para PDFs grandes (1 vs 2)?
- \u00bfNecesitamos `maxCacheBytes` real o basta `maxCacheEntries` en 02-FE?


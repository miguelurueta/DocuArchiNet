## 1. Engine contracts

- [x] 1.1 Crear tipos `PdfLoadResult`, `PdfRenderRequest`, `PdfRenderResult` y la interfaz `PdfEngine` en `src/app/Components/UI/AppVisorPdf/engine/`
- [x] 1.2 Agregar configuraci\u00f3n del worker de pdf.js (bundle-friendly) y documentar troubleshooting (CORS/worker)

## 2. pdf.js engine implementation

- [x] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/engine/pdfjsEngine.ts` con `load(input)` retornando `pageCount` y `fingerprint` (si aplica)
- [x] 2.2 Implementar `renderPage(req, canvas, signal?)` usando `AbortSignal` para cancelar renders obsoletos
- [x] 2.3 Implementar `destroy()` para abortar renders en curso, limpiar cache y liberar referencias del `pdfDocument`
- [x] 2.4 Manejar errores del engine con mensajes amigables (sin exponer stack traces en UI)

## 3. Cache & invalidation

- [x] 3.1 Implementar cache por `pageNumber|zoom` con pol\u00edtica LRU y l\u00edmite `maxCacheEntries` (default: 12)
- [x] 3.2 Limpiar cache completa cuando cambia `input` (nuevo PDF)
- [x] 3.3 Invalidar/gestionar cache al cambiar `zoom` (por key o limpieza selectiva seg\u00fan l\u00edmites)
- [x] 3.4 Definir l\u00edmites de cache `maxCacheBytes` (default 128 MB) o documentar por qu\u00e9 se usa solo `maxCacheEntries`

## 4. Viewport virtualization

- [x] 4.1 Crear `src/app/Components/UI/AppVisorPdf/presentation/VisorPdfViewport.tsx` (canvas + scrolling)
- [x] 4.2 Implementar virtualizaci\u00f3n: renderizar p\u00e1gina activa + buffer (default 1) y no renderizar p\u00e1ginas lejanas
- [x] 4.3 Conectar viewport con engine y estados reales `loading/error` en `AppVisorPdf`
- [x] 4.4 Asegurar que el render incremental no bloquee la UI en PDFs grandes (yield/abort/scheduling seg\u00fan aplique)

## 5. Tests

- [ ] 5.1 Agregar fixture PDF demo y test: `load()` + renderiza p\u00e1gina 1 a canvas (sin freeze)
- [x] 5.2 Test: cambiar `zoom` invalida cache/actualiza render (assert sobre llamadas a engine)
- [x] 5.3 Test de virtualizaci\u00f3n: al cambiar de p\u00e1gina, no renderiza p\u00e1ginas lejanas (assert sobre llamadas)

## 6. Documentation

- [x] 6.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con performance (virtualizaci\u00f3n + cache) y configuraci\u00f3n
- [x] 6.2 Documentar troubleshooting (worker, CORS, PDFs grandes) y limitaciones conocidas

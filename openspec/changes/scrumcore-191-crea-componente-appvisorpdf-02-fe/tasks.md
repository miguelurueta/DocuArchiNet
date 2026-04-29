## 1. Engine contracts

- [ ] 1.1 Crear tipos `PdfLoadResult`, `PdfRenderRequest`, `PdfRenderResult` y la interfaz `PdfEngine` en `src/app/Components/UI/AppVisorPdf/engine/`
- [ ] 1.2 Agregar configuraci\u00f3n del worker de pdf.js (bundle-friendly) y documentar troubleshooting (CORS/worker)

## 2. pdf.js engine implementation

- [ ] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/engine/pdfjsEngine.ts` con `load(input)` retornando `pageCount` y `fingerprint` (si aplica)
- [ ] 2.2 Implementar `renderPage(req, canvas, signal?)` usando `AbortSignal` para cancelar renders obsoletos
- [ ] 2.3 Implementar `destroy()` para abortar renders en curso, limpiar cache y liberar referencias del `pdfDocument`

## 3. Cache & invalidation

- [ ] 3.1 Implementar cache por `pageNumber|zoom` con pol\u00edtica LRU y l\u00edmite `maxCacheEntries` (default: 12)
- [ ] 3.2 Limpiar cache completa cuando cambia `input` (nuevo PDF)
- [ ] 3.3 Invalidar/gestionar cache al cambiar `zoom` (por key o limpieza selectiva seg\u00fan l\u00edmites)

## 4. Viewport virtualization

- [ ] 4.1 Crear `src/app/Components/UI/AppVisorPdf/presentation/VisorPdfViewport.tsx` (canvas + scrolling)
- [ ] 4.2 Implementar virtualizaci\u00f3n: renderizar p\u00e1gina activa + buffer (default 1) y no renderizar p\u00e1ginas lejanas
- [ ] 4.3 Conectar viewport con engine y estados reales `loading/error` en `AppVisorPdf`

## 5. Tests

- [ ] 5.1 Agregar fixture PDF demo y test: `load()` + renderiza p\u00e1gina 1 a canvas (sin freeze)
- [ ] 5.2 Test: cambiar `zoom` invalida cache/actualiza render (assert sobre llamadas a engine)
- [ ] 5.3 Test de virtualizaci\u00f3n: al cambiar de p\u00e1gina, no renderiza p\u00e1ginas lejanas (assert sobre llamadas)

## 6. Documentation

- [ ] 6.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con performance (virtualizaci\u00f3n + cache) y configuraci\u00f3n
- [ ] 6.2 Documentar troubleshooting (worker, CORS, PDFs grandes) y limitaciones conocidas


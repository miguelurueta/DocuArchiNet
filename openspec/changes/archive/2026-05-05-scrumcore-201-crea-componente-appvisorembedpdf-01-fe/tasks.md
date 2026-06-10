## 1. Alineación de naming y estructura

- [x] 1.1 Alinear naming entre `proposal.md`, spec y código (capability `app-appvisorembedpdf-01-fe` y componente `AppVisorEmbedPdf`)
- [x] 1.2 Definir path final del componente (ej: `src/app/Components/UI/AppVisorEmbedPdf/`) y actualizar el Impact del `proposal.md` si está incorrecto
- [x] 1.3 Definir API pública obligatoria `AppVisorEmbedPdfProps` (`fileUrl?`, `className?`, `style?`) y ubicar tipos en `types/`
- [x] 1.4 Crear estructura obligatoria de carpetas/archivos (hooks/engine/plugins/presentation/styles/types + `README.md`)

## 2. Dependencias y encapsulación del engine

- [x] 2.1 Reintroducir dependencias permitidas para 01-FE en `package.json` (`@embedpdf/core`, `@embedpdf/engines`, `@embedpdf/plugin-document-manager`, `@embedpdf/plugin-viewport`, `@embedpdf/plugin-scroll`, `@embedpdf/plugin-render`)
- [x] 2.2 Ejecutar `npm install` y verificar que `package-lock.json` queda actualizado
- [x] 2.3 Implementar inicialización de engine con `usePdfiumEngine()` dentro de `engine/` y exponer estado tipado (loading/error/engine)
- [x] 2.4 Encapsular EmbedPDF detrás de un adapter/export interno para facilitar mocks en tests (sin WASM/recursos nativos)

## 3. Plugins básicos (EmbedPDF)

- [x] 3.1 Implementar registro de plugins con `createPluginRegistration` (solo: document-manager + viewport + scroll + render)
- [x] 3.2 Configurar virtualización nativa con `Scroller` y lazy rendering nativo de EmbedPDF (sin implementar zoom/toolbar/etc.)
- [x] 3.3 Verificar explícitamente que NO se agregó wiring de features no permitidas (zoom/rotate/toolbar/search/thumbnails/annotations/signatures/password/print/download)

## 4. Implementación de `AppVisorEmbedPdf`

- [x] 4.1 Crear carpeta del componente con `AppVisorEmbedPdf.tsx` y export público en `index.ts`
- [x] 4.2 Implementar loader de engine (visible) mientras `usePdfiumEngine()` carga
- [x] 4.3 Implementar loader de documento (visible) mientras el PDF carga
- [x] 4.4 Implementar empty state: si `fileUrl` no existe, cargar un PDF demo configurable/local (sin hardcodear URLs externas en la lógica)
- [x] 4.5 Implementar error state básico (engine/document) sin crashes de React
- [x] 4.6 Implementar render básico con `EmbedPDF` + `DocumentContent` + `Viewport` + `Scroller` + `RenderLayer` y scroll vertical funcional
- [x] 4.7 Implementar estilos con CSS Modules (o estrategia del proyecto) en `styles/` (fondo tipo visor profesional + responsive)
- [x] 4.8 Agregar comentarios técnicos importantes en puntos críticos (engine init, plugin registration, virtualización/lazy rendering)

## 5. Tests (Vitest) para el spec

- [x] 5.1 Crear tests del componente con etiqueta `[SPEC:SCRUMCORE-201]` (o ID acordado) en el nombre del `describe`/test
- [x] 5.2 Mockear engine/plugins vía adapter para simular: loading engine, loading doc, carga exitosa, y error
- [x] 5.3 Validar API: `fileUrl` es opcional y activa fallback demo cuando no existe
- [x] 5.4 Validar arquitectura: un test/aseveración indirecta de que consumidores no requieren imports de `@embedpdf/*`

## 6. Validación y evidencia

- [x] 6.1 Ejecutar `npm run build` y asegurar “TS compila sin warnings” para lo introducido por el cambio
- [x] 6.2 Ejecutar `npm test` y corregir solo fallos relacionados con este cambio
- [x] 6.3 Registrar evidencia de comandos/resultados en `design.md` (sección “Validation Evidence” con fecha)

## 1. Setup

- [ ] 1.1 Crear carpeta `src/app/Components/UI/AppVisorEmbedPdf/` con export público (`index.ts`)
- [ ] 1.2 Definir contratos mínimos en `domain/pdf.types.ts`:
  - `AppPdfSource` (url/bytes)
  - `AppPdfLoadState` (`idle/loading/ready/password_required/error`)
  - `AppPdfCapabilities` (flags para plugins)
- [ ] 1.3 Definir interfaz del engine adapter en `engine/embedPdfEngine.types.ts`:
  - `load(source)` -> `{ pageCount; fingerprint?; capabilities? }`
  - `renderPage({ pageNumber; zoom; rotation? }, canvas, signal?)` -> `{ width; height }`
  - `destroy()`
- [ ] 1.4 Definir API headless mínima en `domain/viewerApi.types.ts`:
  - `setSource(source|null)`
  - `setZoom(zoom)`
  - `setRotation(degrees)`
  - `openSidebar()` / `closeSidebar()`
 - [ ] 1.5 Validar desacople de dominio: `AppVisorEmbedPdf` no importa desde `src/modules/**`

## 2. Documentación

- [ ] 2.1 Crear `src/app/Components/UI/AppVisorEmbedPdf/README.md`:
  - propósito y alcance de 01-FE
  - no-objetivos (engine real, UI completa, plugins)
  - referencia a `docs/Architecture/AppVisorEmbedPdf/architecture.md`
  - roadmap incremental con links a prompts (02A/02B/03/04/05)
  - mapeo desde `AppVisorPdf` (engine/viewport/toolbar)
  - nota explícita: “Usar EmbedPDF Core, no viewer monolítico”
- [ ] 2.2 Crear/actualizar `docs/Architecture/AppVisorEmbedPdf/implementation-log.md` con entrada “Capa 01”

## 3. Validación

- [ ] 3.1 Ejecutar `tsc --noEmit`

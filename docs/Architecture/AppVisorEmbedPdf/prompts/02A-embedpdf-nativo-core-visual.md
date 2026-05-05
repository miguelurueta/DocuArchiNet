# Prompt 02A/02B — EmbedPDF nativo (core visual + hardening) + plugins básicos + Workbench

Objetivo: implementar el primer incremento **usable en UI** del visor `AppVisorEmbedPdf` usando **EmbedPDF Core + PDFium + plugins nativos**, sin lógica propia (sin store/viewport/renderQueue custom), y con hardening mínimo enterprise (lifecycle + errores silenciosos).

Referencia: `docs/Architecture/AppVisorEmbedPdf/architecture.md`.

## Contexto

El visor previo (PDF.js / implementaciones custom) generó:
- cancelaciones ruidosas (“Rendering cancelled”),
- loops de render/scroll/zoom,
- acoplamiento entre UI, estado y engine.

Este prompt unifica 02A + 02B para asegurar una base estable:
- **Engine real**: PDFium (EmbedPDF) desde el día 1.
- **Plugins nativos**: Scroll/Render/Zoom/Viewport/DocumentManager/InteractionManager.
- **UI (Design System)**: toolbar compuesta con `AppButton`/`AppDropdown`, pero las acciones vienen 100% de capabilities/hooks nativos (nada de cálculos propios).
- **Integración segura**: feature flag en `DocumentosWorkbench.tsx`.
- **Hardening**: lifecycle + dedupe de “open” + manejo de errores sin ruido.

## Alcance (02A/02B)

### 1) Dependencias (nativas EmbedPDF)
Instalar:
- `@embedpdf/core`, `@embedpdf/engines`, `@embedpdf/pdfium`
- plugins:
  - `@embedpdf/plugin-document-manager`
  - `@embedpdf/plugin-viewport`
  - `@embedpdf/plugin-scroll`
  - `@embedpdf/plugin-render`
  - `@embedpdf/plugin-zoom`
  - `@embedpdf/plugin-interaction-manager`

### 2) Wrapper nativo (`AppVisorEmbedPdf`)
Implementar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` como composición **nativa**:

- Inicialización engine:
  - `const { engine, isLoading, error } = usePdfiumEngine()` (de `@embedpdf/engines/react`)
  - Mientras `isLoading || !engine`: mostrar skeleton/placeholder
  - Si `error`: mostrar mensaje de “No se pudo inicializar el visor”

- Montaje EmbedPDF:
  - Usar `<EmbedPDF engine={engine} plugins={plugins} />`
  - IMPORTANT: `plugins` debe seguir el contrato EmbedPDF:
    - `PluginBatchRegistrations` = `[{ package: PluginPackage, config? }, ...]`
    - No pasar “packages” sueltos.

- Abrir documento (DocumentManager nativo):
  - `useDocumentManagerCapability()`
  - URL:
    - `cap.openDocumentUrl({ url, name, autoActivate: true })`
  - Buffer:
    - `cap.openDocumentBuffer({ buffer, name, autoActivate: true })`
  - Dedupe: evitar reabrir el mismo documento en loop (ref con openKey por `source`).

- Viewport + Scroll + Render (nativos):
  - `Viewport` + `Scroller` (vertical) + `RenderLayer`
  - MVP visual: renderizar **al menos la página 0**:
    - `<RenderLayer documentId={docId} pageIndex={0} />`
  - Nota: para “todas las páginas”, no inventar virtualización propia en 02A; se hará cuando EmbedPDF exponga el modelo de páginas o plugin correspondiente.

- Zoom + Interaction (nativos):
  - Envolver el viewport con `ZoomGestureWrapper`.
  - Zoom desde hooks/capabilities:
    - `useZoom(docId)` o `useZoomCapability().provides.forDocument(docId)`
    - Invocar `zoomIn/zoomOut/requestZoom(...)` del plugin.

### 3) Toolbar (nativa por plugins, UI por Design System)
Toolbar mínima (no lógica propia):
- Botones (`AppButton`) que invocan:
  - `zoom.zoomIn()`
  - `zoom.zoomOut()`
  - opcional: `requestZoom("fit-width" | "fit-page")`

Regla de Hooks:
- No llamar hooks condicionalmente.
- Patrón recomendado:
  - `EmbedPdfToolbar` decide si hay `docId`
  - `EmbedPdfZoomToolbar` recibe `docId` y ahí sí usa `useZoom(docId)`.

### 4) Hardening mínimo (02B incluido)
- **Lifecycle/memoria**
  - Dedupe de openDocument
  - Si `source` se arma desde `Blob` (`URL.createObjectURL`), revocar en cleanup en el caller (Workbench) o en un wrapper dedicado.
- **Errores UX**
  - no exponer errores técnicos al usuario
  - fallback: “Abrir en nueva pestaña” si `source.kind === "url"`
- **Logging**
  - no dejar `console.log` persistente; solo en DEBUG/DEV y remover antes de merge.

## Reglas no negociables

- Nada de estado propio para zoom/scroll: todo vía plugin capability/hook.
- Nada de “renderPage” custom: render via `RenderLayer`.
- Feature flag obligatorio: `VITE_ENABLE_EMBEDPDF=true`.

## Entregables (archivos)

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/index.ts`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `docs/Architecture/AppVisorEmbedPdf/implementation-log.md` (Capa 02A/02B)

## Criterios de aceptación

- Con flag ON, se ve un PDF en UI (al menos página 1 renderizada).
- Zoom +/- funciona y es 100% nativo del plugin zoom.
- No hay errores ruidosos en consola por interacciones.
- `tsc --noEmit` pasa.

# Spec — SCRUMCORE-204 (Toolbar + Zoom) AppVisorEmbedPdf

## Objetivo

Actualizar `AppVisorEmbedPdf` para incorporar una toolbar enterprise desacoplada y controles de zoom usando el plugin oficial `@embedpdf/plugin-zoom`, manteniendo el componente encapsulado y reusable.

## Alcance

Incluye:
- UI de toolbar dentro de `AppVisorEmbedPdf`.
- Integración oficial de zoom: `createPluginRegistration(ZoomPluginPackage)`.

No incluye (por defecto):
- rotate, search, thumbnails, annotations, signatures, password, print/download.

## Reglas de arquitectura

- Consumers no importan `@embedpdf/*`; todo vive en `src/app/Components/UI/AppVisorEmbedPdf/`.
- Hooks/capabilities de EmbedPDF que dependen de provider deben ejecutarse dentro de `<EmbedPDF>`.
- CSS Modules (sin mezclar estrategias).
- No crear lógica custom manual de zoom; usar el comportamiento nativo del engine EmbedPDF.

## UX mínima esperada

- Toolbar visible y consistente con la UI del proyecto.
- Acciones mínimas:
  - Zoom in
  - Zoom out
  - Reset zoom (100%)
  - Mostrar `zoomLevel` actual.

## Toolbar API (obligatoria)

Crear componente presentacional memoizado:

```ts
export interface AppPdfToolbarProps {
  zoomLevel: number;
  onZoomIn(): void;
  onZoomOut(): void;
  onResetZoom(): void;
}
```

- Debe vivir en:
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.module.css`
- Debe estar memoizado explícitamente: `React.memo(...)`.
- No debe conocer engine, plugins, ni Workbench.

Accesibilidad:
- Botones con `aria-label`
- Soporte teclado básico (Enter/Space)

## Criterios de aceptación

- TS compila sin warnings.
- Zoom funciona sin romper virtualización/scroll.
- No hay warnings de React por orden de hooks.
- Toolbar desacoplada y memoizada.
- Tests actualizados o agregados:
  - Unit/RTL: render toolbar + interacción.
  - E2E Playwright: escenario zoom + re-render estable.
- Documentación enterprise actualizada bajo `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/` (ruta del repo en minúsculas) incluyendo diagramas Mermaid (arquitectura, flujo zoom, secuencia render, responsabilidades, interacción toolbar↔viewport).

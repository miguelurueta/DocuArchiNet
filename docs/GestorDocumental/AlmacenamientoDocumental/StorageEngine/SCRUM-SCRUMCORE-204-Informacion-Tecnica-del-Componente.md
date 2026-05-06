# SCRUMCORE-204 — Información técnica del componente

## Plugin oficial

- Paquete: `@embedpdf/plugin-zoom`
- Registro: `createPluginRegistration(ZoomPluginPackage)` en `plugins/pluginRegistration.ts`

## Toolbar API (obligatoria)

```ts
export interface AppPdfToolbarProps {
  zoomLevel: number;
  onZoomIn(): void;
  onZoomOut(): void;
  onResetZoom(): void;
}
```

## Memoización (obligatoria)

- `AppPdfToolbar` se memoiza con `React.memo` para evitar rerenders por scroll/virtualización.

## Reset zoom

- Implementado como `requestZoom(1)` (100%).


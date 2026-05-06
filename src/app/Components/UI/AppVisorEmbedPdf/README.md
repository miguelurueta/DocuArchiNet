# AppVisorEmbedPdf (01-FE)

Componente reusable enterprise para visualización de PDFs basado en EmbedPDF + Pdfium Engine.

## API

```tsx
<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />
```

Props públicas:
- `fileUrl?: string`
- `className?: string`
- `style?: React.CSSProperties`

## Notas de arquitectura

- Mantiene EmbedPDF encapsulado dentro del componente.
- Los módulos consumidores (ej. `DocumentosWorkbench`) no deben importar `@embedpdf/*`.
- El PDF demo se configura con `VITE_EMBEDPDF_DEMO_PDF` (ruta local/relativa) y por defecto usa un archivo en `public/demo/`.

## Toolbar + Zoom (SCRUMCORE-204)

El visor incorpora toolbar desacoplada (presentacional) para:
- Zoom In
- Zoom Out
- Reset zoom (100%)

La implementación usa el plugin oficial:
- `@embedpdf/plugin-zoom` registrado vía `createPluginRegistration(ZoomPluginPackage)`

Reglas:
- No se implementa zoom manual custom.
- La toolbar no conoce engine/plugins/workbench; solo recibe `zoomLevel` + handlers.
- `AppPdfToolbar` está memoizado (`React.memo`) para evitar rerenders por scroll/virtualización.


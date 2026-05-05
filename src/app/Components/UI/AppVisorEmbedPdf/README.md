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


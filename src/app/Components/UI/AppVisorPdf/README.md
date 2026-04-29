# AppVisorPdf

Componente UI reusable para visualizaci\u00f3n de documentos PDF (shell + toolbar + contratos).

## Performance (PDFs grandes)

- El engine usa render incremental (viewport virtualiza p\u00e1gina activa + buffer).
- Existe cache LRU por `pageNumber|zoom` con l\u00edmite por entradas (`maxCacheEntries`, default 12).
- `maxCacheBytes` (default 128MB en el ticket) no se estima con precisi\u00f3n en esta fase; se controla por cantidad de entradas y se recomienda monitorear memoria en PDFs grandes.

## Props (resumen)

- `input`: fuente del documento (`url` o `bytes`)
- `loading` / `error`: estados controlables por el consumidor
- `onRetry`: callback para reintento cuando `error` est\u00e1 presente
- `page` / `defaultPage` / `onPageChange`: control de paginaci\u00f3n (controlado/no controlado)
- `zoom` / `defaultZoom` / `onZoomChange`: control de zoom (controlado/no controlado)
- `tool` / `defaultTool` / `onToolChange`: control de herramienta seleccionada
- `onRequestSaveAnnotations`: solicitud para guardar anotaciones (desacoplado)
- `onRequestExport`: solicitud para exportar (desacoplado)

## Ejemplos

### Ejemplo con URL

```tsx
<AppVisorPdf
  input={{ kind: "url", url: "https://example.com/document.pdf" }}
  aria-label="Visor PDF"
/>
```

### Ejemplo con bytes

```tsx
<AppVisorPdf
  input={{ kind: "bytes", bytes: new Uint8Array([]), fileName: "documento.pdf" }}
  aria-label="Visor PDF"
/>
```

## Accesibilidad (a11y)

- Proveer `"aria-label"` para el visor.
- Mensajes de estado (empty/loading/error) deben exponerse con `role="status"` cuando aplique.
- La toolbar debe ser navegable por teclado y mantener focus visible.

## Troubleshooting

- **Worker pdf.js:** requiere configuraci\u00f3n local del worker para evitar bloquear el main thread.
- **CORS (URLs):** si usas `input.kind="url"`, la URL debe permitir lectura por el navegador (CORS) o debe ser servida desde el mismo origen.

## Limitaciones conocidas

- Render basado en canvas (text-layer/selecci\u00f3n no garantizados en esta etapa).
- Thumbnails no son parte del alcance del ticket 02-FE.

# AppVisorPdf

Componente UI reusable para visualizaci\u00f3n de documentos PDF (shell + toolbar + contratos).

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


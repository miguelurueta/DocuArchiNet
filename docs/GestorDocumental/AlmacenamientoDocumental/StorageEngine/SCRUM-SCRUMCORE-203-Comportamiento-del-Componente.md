# SCRUMCORE-203 — Comportamiento del componente

## Lifecycle (alto nivel)

1) Mount: inicializa engine Pdfium.
2) Engine ready: monta provider EmbedPDF con plugins.
3) Open document: abre `fileUrl` o demo.
4) Render success: pinta páginas con scroll/virtualización.
5) Unmount: cleanup (provider/plugins).

## Estados

- `loading (engine)`: mientras inicializa Pdfium.
- `loading (document)`: mientras se abre el PDF.
- `empty`: sin `fileUrl` y sin demo disponible.
- `error`: falla engine o falla apertura de documento.
- `success`: documento activo y renderizado.
- `fallback`: demo PDF cuando `fileUrl` no existe.

## Manejo de errores

- Debe mostrar error state básico y permitir diagnóstico (mensaje/log).
- Errores típicos:
  - `fileUrl` no accesible (404/403/CORS).
  - PDFs enormes (time/mem).
  - Hooks fuera del provider (no activa documento / warning Rules of Hooks).

## Responsive

- Se ajusta al contenedor del consumer.

## Re-render behavior (regla enterprise)

- No debe producir loops de render.
- No debe cambiar el orden de hooks entre renders.
- Cambios de `fileUrl` deben abrir el nuevo documento de forma controlada (sin duplicar efectos).

## Cleanup / unmount

- Debe liberar listeners/recursos del visor cuando se desmonta.

## Memoria / performance

- Virtualización reduce costo de render por páginas.
- PDFs extremadamente grandes pueden seguir impactando memoria/tiempo de parse/render.

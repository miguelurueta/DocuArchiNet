# SCRUMCORE-210 — Comportamiento del Componente

## Lifecycle y estados

- Engine loading: muestra loader de engine.
- Document loading: muestra loader de documento.
- Error/empty: muestra estados básicos (presentación).

## Firma (UI)

- El usuario abre el modal desde toolbar.
- Puede dibujar o subir firma.
- Al pulsar `Usar firma`:
  - se activa placement oficial del plugin.
  - el modal se resetea internamente (limpia estado).

## Borrado firma

- Solo se habilita si hay una firma seleccionada.
- El borrado intenta persistir en el PDF real vía `deleteAnnotation` y luego `commit()`.
- Se evita `purgeAnnotation` como mecanismo de borrado (solo es state/UI).

## Export / Print

- Antes de exportar/imprimir se fuerza `commit()` de anotaciones.
- Export descarga el buffer de `saveAsCopy` para minimizar inconsistencias de snapshot.

## Performance

- `Scroller` provee virtualización nativa.
- `AppPdfToolbar` memoizada para evitar rerenders por scroll.


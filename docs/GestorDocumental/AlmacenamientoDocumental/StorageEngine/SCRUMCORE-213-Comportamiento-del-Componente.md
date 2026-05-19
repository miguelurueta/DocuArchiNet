# SCRUMCORE-213 — Comportamiento del Componente

## Tab “Firma personal”

### Estado `ready`

Cuando la firma temporal está lista:

- Se renderiza un preview con `<img alt="Firma personal">` usando el `ObjectURL` (`blobUrl`).
- La UI **no** muestra:
  - `blobUrl` como texto (ni `blob:`)
  - `UrlTemporal`
- Se muestra un único botón: **“Usar firma”**.

### Apertura del modal (reset de tab)

Para mantener una UX consistente y evitar estados “pegados” entre aperturas:

- Cada vez que el modal se abre (`isOpen === true`), el tab activo se restablece a **“Dibujar firma”**.
- En la misma apertura se limpia el estado de “Firma personal” (`personal.clear()`), evitando que quede un tab activo con `ObjectURL` revocado o datos stale.

### Acción “Usar firma”

- Construye una `SignatureStampFieldDefinition` (tipo upload) con:
  - `previewDataUrl` (ObjectURL)
  - `imageData` (ArrayBuffer)
- Llama `onStartPlacement(stamp)` para iniciar placement oficial EmbedPDF.
- Resetea estado del modal y revoca `ObjectURL` (cleanup existente).

## Paginación (overlay)

### Indicador editable (ir a página)

En el overlay de paginación del visor:

- El indicador `current/total` se puede clicar para entrar en modo edición.
- En modo edición se muestra un input numérico.
- Al presionar `Enter` o al perder foco (`blur`), se navega a la página usando únicamente:
  - `scroll.provides.scrollToPage({ pageNumber, behavior: "smooth", alignY: 0 })`
- `Escape` cancela la edición sin navegar.

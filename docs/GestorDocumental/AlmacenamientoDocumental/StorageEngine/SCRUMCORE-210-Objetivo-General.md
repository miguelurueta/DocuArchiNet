# SCRUMCORE-210 — Objetivo General

Actualizar `AppVisorEmbedPdf` para incorporar flujo enterprise de **firma electrónica (gráfica)** y controles asociados en UI (modal, toolbar), manteniendo:

- Encapsulación total de EmbedPDF dentro de `AppVisorEmbedPdf`.
- Plugins oficiales EmbedPDF (sin lógica PDF manual).
- Virtualización nativa del visor.
- Tipado fuerte y separación de responsabilidades.

Resultado esperado:

- El usuario puede dibujar o subir una firma (imagen) desde un modal.
- El usuario puede colocar la firma en el PDF usando placement nativo del plugin.
- El usuario puede borrar la firma seleccionada desde toolbar.
- Export/Print reflejan el estado real del documento (sin anotaciones “fantasma”).
- El usuario puede bloquear/desbloquear firmas (guardrail UX) desde toolbar.


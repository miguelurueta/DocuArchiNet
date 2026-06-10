# SCRUMCORE-209 — Objetivo General

Extender el componente reusable `AppVisorEmbedPdf` para soportar PDFs protegidos con contraseña, **sin implementar desencriptación custom** y reutilizando capacidades oficiales del `DocumentManager` de EmbedPDF:

- Detectar “password required / invalid password” vía `onDocumentError` (`PdfErrorCode.Password`).
- Reintentar la carga vía `retryDocument(documentId, { password })`.
- Cerrar el estado de “Validando…” únicamente cuando el `OpenDocumentResponse.task` termina (éxito o falla).

Resultado esperado:
- Si el PDF requiere contraseña, se muestra un prompt desacoplado para ingresarla.
- Contraseña inválida permite reintentar sin bloquear el input (sin quedar “pegado” en “Validando…”).
- Contraseña válida desbloquea el PDF y cierra el prompt automáticamente.
- No se rompe el comportamiento existente del visor (virtualización, zoom, rotate, thumbnails, paginación, toolbar, etc.).

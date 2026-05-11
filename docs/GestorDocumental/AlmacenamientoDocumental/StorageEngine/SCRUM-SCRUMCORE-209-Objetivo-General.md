# SCRUMCORE-209 — Objetivo General

Extender el componente reusable `AppVisorEmbedPdf` para soportar PDFs protegidos con contraseña **sin plugins npm adicionales** y **sin lógica custom de desencriptación**, reutilizando únicamente capacidades oficiales del `DocumentManager` de EmbedPDF:

- Detección de error de password vía `onDocumentError` (código `PdfErrorCode.Password`).
- Reintento de carga vía `retryDocument(documentId, { password })`.
- Cierre del estado “Validando…” y del overlay al resolver el `task` interno de carga del documento (`OpenDocumentResponse.task`).

Resultado esperado:
- PDFs protegidos solicitan contraseña mediante un prompt desacoplado.
- Contraseña inválida permite reintento sin bloquear el input.
- Contraseña válida desbloquea el PDF y cierra el prompt automáticamente.
- No se rompen funcionalidades existentes (virtualización, zoom, rotate, thumbnails, paginación, print, export, toolbar).


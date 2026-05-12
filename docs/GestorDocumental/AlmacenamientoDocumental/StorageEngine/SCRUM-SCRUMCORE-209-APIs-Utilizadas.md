# SCRUMCORE-209 — APIs Utilizadas

En esta fase no se consumen endpoints HTTP propios del proyecto.

Integraciones relevantes (SDK EmbedPDF):
- `DocumentManager`:
  - `openDocumentUrl(...)`
  - `retryDocument(documentId, { password })`
  - `onDocumentError(...)` + `PdfErrorCode.Password`

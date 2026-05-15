# SCRUMCORE-210 — APIs Utilizadas

Este ticket no integra APIs backend para firma digital PKI/PAdES.

## URLs de PDF (fileUrl)

El visor consume PDFs desde:

- Rutas locales (`/demo/...`) servidas por `public/`.
- Endpoints HTTP que devuelven binario `application/pdf` (recomendado):
  - `GET /DocuArchiApi/api/...` → bytes PDF (no JSON).

## Export / Print

La exportación se realiza con APIs del plugin oficial de export (no endpoints HTTP):

- `exportApi.provides.saveAsCopy(documentId)` → buffer PDF materializado por engine.


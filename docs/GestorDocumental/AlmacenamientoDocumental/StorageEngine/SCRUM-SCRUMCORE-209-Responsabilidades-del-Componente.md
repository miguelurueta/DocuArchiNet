# SCRUMCORE-209 — Responsabilidades del Componente

## Responsabilidades principales (AppVisorEmbedPdf)
- Encapsular el flujo de apertura de documento (demo o `fileUrl`) usando EmbedPDF.
- Orquestar el `DocumentManager` para:
  - abrir documento (`openDocumentUrl`)
  - manejar error de password (`onDocumentError` + `PdfErrorCode.Password`)
  - reintentar (`retryDocument`) sin exponer lógica a `DocumentosWorkbench`.
- Mantener estados enterprise para password:
  - overlay/prompt
  - loading “Validando…”
  - invalid password (reintento)

## Qué NO debe hacer
- No debe implementar desencriptación/parseo de PDFs.
- No debe persistir contraseñas.
- No debe exponer engine/plugins/estados internos al Workbench.

## Responsabilidades del consumidor (Workbench/otros)
- Proveer `fileUrl` (opcional).
- No conocer ni manejar estados del password prompt.

# SCRUMCORE-209 — Responsabilidades del Componente

## Responsabilidades de `AppVisorEmbedPdf`
- Encapsular EmbedPDF/Pdfium y el `DocumentManager` dentro del componente.
- Orquestar el flujo de apertura del documento y reintentos de carga.
- Renderizar estados UX (loading/error/empty) y overlays enterprise del visor.
- Manejar el ciclo de vida del prompt de contraseña:
  - abrir/cerrar overlay
  - bloquear/desbloquear input durante validación
  - reflejar “contraseña inválida”

## Responsabilidades del `DocumentManager` (oficial EmbedPDF)
- Determinar si un PDF requiere password o si la password es incorrecta.
- Ejecutar la carga del documento con password/permissions usando el engine Pdfium.
- Emitir errores de documento (`onDocumentError`) y proveer `Task` de carga.

## Qué NO hace `AppVisorEmbedPdf`
- No desencripta PDFs manualmente.
- No usa `pdf.js` ni librerías externas de crypto.
- No implementa heurísticas DOM/listeners custom para detectar password.
- No expone detalles internos al `DocumentosWorkbench`.

## Responsabilidades del consumidor
- Consumir el visor vía props (`fileUrl`) sin manejar password plugin/engine.


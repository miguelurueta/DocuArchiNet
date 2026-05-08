# SCRUMCORE-207 — Responsabilidades del Componente

## AppVisorEmbedPdf

- Encapsula engine + plugins EmbedPDF.
- Expone acciones UI de Print/Export mediante callbacks.
- NO filtra detalles internos al Workbench.

## AppPdfToolbar

- Presentacional: dispara `onPrint()` y `onExport()`.
- No conoce engine/plugins.


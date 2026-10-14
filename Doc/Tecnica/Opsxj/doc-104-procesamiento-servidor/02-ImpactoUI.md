# PROCESAMIENTO-SERVIDOR — IMPACTO UI

## Superficie modificada

Solo cambia `PDFjsViewer._updatePage(i, document)` en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`:

- antes abría un PDF temporal de una página mediante `getPage(1)`;
- ahora abre el documento actualizado mediante transporte `stream` y solicita `getPage(i)`.

## Comportamiento preservado

- Un guardado produce un solo POST.
- El overlay permanece hasta commit y refresco confirmados.
- Error de red, contrato o refresco conserva el overlay y no reintenta automáticamente.
- Página activa, zoom y scroll no se reasignan.
- `committed_with_warning` refresca una vez y no duplica el pegado.

No cambian toolbar, navegación, accesibilidad, Wacom, impresión ni descarga. La validación visual real requiere PDF descartable y autorización E2E específica.

# TRANSACIONES-FRONTEND

- Ticket: DOC-103
- Cambio OpenSpec: doc-103-transaciones-frontend
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Corregir el falso éxito del pegado de grafo. La ruta anterior retornaba `"YES"` antes de `Image.onload`, no retornaba el AJAX y retiraba el overlay antes de confirmar servidor y refresco.

## Solución

`pdf_viewer_anotate.js` es el único propietario productivo. La operación ahora encadena validación → carga de imagen → POST ASMX → normalización → refresco parcial → retiro del overlay. Un registro `Map` aplica single-flight por documento, página y overlay; `finally` libera los controles.

Estados cerrados: `committed`, `committed_with_warning`, `rejected`, `invalid_response` y `uncertain`. Timeout, desconexión, aborto y 5xx no disparan reintento automático.

## Alcance y compatibilidad

- Modificados: JavaScript activo, pruebas Node y runner protegido en `tools/e2e`.
- Preservados: WebMethod, DTO, payload, iTextSharp, temporales, almacenamiento, firma, Wacom, impresión, descarga, permisos y `pdfannotate.js` inactivo.
- Refresco: una sola página mediante `_createLoadingSource(document, "stream")`; no reinicializa el PDF.
- Reversa: revertir los cambios DOC-103 de JavaScript y arnés. No hay migración ni cambio de datos.
- Límite DOC-104: atomicidad física, idempotencia o concurrencia entre pestañas/servidor.

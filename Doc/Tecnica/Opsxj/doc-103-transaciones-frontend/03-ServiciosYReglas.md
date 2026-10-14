# TRANSACIONES-FRONTEND

- Ticket: DOC-103
- Cambio OpenSpec: doc-103-transaciones-frontend
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

```text
Cliente / overlay
  -> event_drag_anotate valida contexto y obtiene clave single-flight
  -> save_anotate_firma espera Image.onload
  -> service_anotate_pdf_image envía un POST (timeout 30 s)
  -> WebService_itext.asmx conserva el contrato legacy
  -> iText modifica el PDF y devuelve IEnumerable(Of service_itex)
  -> normalizador cliente decide:
       committed -> refrescar página -> retirar overlay
       committed_with_warning -> refrescar -> retirar -> advertir
       rejected / invalid_response / uncertain -> conservar overlay
  -> finally libera clave y controles
```

No existe rollback cliente: después de enviar, una pérdida de conexión puede ser incierta. La UI obliga a recargar y verificar antes de reintentar. `pagehide` aborta cada transporte pendiente una sola vez. No hay retry automático.

`_updatePage` espera en orden la carga del PDF de una página, `getPage(1)`, `_renderPage` y `_add_anotate_page`. Si falla, la operación informa `REFRESH_FAILED` y conserva el overlay para que el usuario verifique el documento.

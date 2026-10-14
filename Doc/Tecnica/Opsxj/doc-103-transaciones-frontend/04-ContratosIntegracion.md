# TRANSACIONES-FRONTEND

- Ticket: DOC-103
- Cambio OpenSpec: doc-103-transaciones-frontend
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

- Endpoint preservado: `../../webservice/WebService_itext.asmx/service_anotate_pdf_image`.
- Método: POST JSON; el campo `anotate` conserva el arreglo legacy `Class_itext_anotate`.
- Respuesta válida: `data.d` debe ser colección no vacía. El primer elemento requiere `error_sistema === "YES"` y `url_salida` no vacía.
- `error_log === "YES"`: `committed`; otro valor: `committed_with_warning` porque el PDF ya pudo quedar modificado.
- HTTP 200 no equivale por sí solo a éxito.
- 401/403 y 404 son rechazos públicos; 5xx, status 0, timeout y aborto son resultados conservadores sin retry; parser o forma incompleta son `invalid_response`.

No cambia autenticación, autorización, esquema, WebMethod, DTO, iText, temporales ni almacenamiento. El runner real reutiliza `authenticated-workflow-session.cjs`, Playwright y controles ODBC existentes; no crea `.env`, login o proyecto paralelo.

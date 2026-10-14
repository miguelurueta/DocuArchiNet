# PROCESAMIENTO-SERVIDOR — CONTRATOS

## ASMX preservado

- Endpoint: `WebService_itext.asmx/service_anotate_pdf_image`.
- Método: POST JSON autenticado con sesión existente.
- Entrada: campo `anotate` con un arreglo de un `Class_itext_anotate`.
- Salida: colección `service_itex`.
- Commit: `error_sistema = "YES"` y `url_salida` no vacía.
- Advertencia posterior: `error_log <> "YES"`; no autoriza retry.

`url_salida` ya no apunta a `{usuario}.pdf`. Contiene la URL de lectura que el cliente ya poseía más `doc104_refresh=<GUID>` para invalidar caché; nunca se deriva de la ruta física.

## Concurrencia

- Mismo PDF: la segunda operación recibe `El documento está siendo actualizado` y no crea artefactos.
- PDFs distintos: locks y workspaces independientes.
- El lock de filesystem cubre procesos que comparten el almacenamiento; no pretende coordinar réplicas sin semántica de locking común.

## Errores y auditoría

Respuesta y `datos_add_grafo` excluyen ruta, URL documental completa, base64, excepción, cookie, token y cadena de conexión. La auditoría conserva página, operación, resultado, modo e identificadores funcionales ya autorizados.

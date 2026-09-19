# LISTA-PREVIEW-INCRIPCIONES

- Ticket: DOC-69
- Cambio OpenSpec: doc-69-lista-preview-incripciones
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

`GetPreview(GetPreviewRequestDto) As GetPreviewResponseDto` conserva metadatos públicos y devuelve `DescriptorId`; nunca devuelve URL, token, ruta o bytes. `workflow/ImportarServicioWebPreview.ashx?id={DescriptorId}` acepta GET y HEAD dentro de la misma sesión/tarea Workflow y detrás del gate moderno.

La respuesta válida contiene `Content-Type`, `Content-Length`, `Content-Disposition`, `Cache-Control: no-store, private`, `Pragma: no-cache`, `X-Content-Type-Options: nosniff` y `X-Frame-Options: SAMEORIGIN`. La tabla `workflow_import_preview_descriptor` reside exclusivamente en `workflowdocument`, usa `MEDIUMBLOB`, 16 columnas, 4 índices y cero llaves foráneas entre bases.

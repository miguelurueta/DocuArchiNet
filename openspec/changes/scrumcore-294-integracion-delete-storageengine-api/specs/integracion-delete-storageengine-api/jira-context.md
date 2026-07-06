# Jira Context - SCRUMCORE-294

## Summary

INTEGRACION-DELETE-STORAGEENGINE-API

## Description

> SCRUM-215 - Integracion Frontend Delete StorageEngine (Contrato vigente)
> Endpoint
> Metodo: DELETE
> 
> Ruta: /api/gestor-documental/eliminar-documento/{idAlmacen:long}
> 
> Query requerido:
> nombreGabinete
> 
> sourceModule (GD_PRODUCCION, WORKFLOW, DA_CONSULTA_LIBRE)
> 
> Claims requeridos
> defaulalias
> 
> usuarioid
> 
> RequestId
> Header recomendado: X-Request-Id.
> 
> Si no se envia, backend lo genera.
> 
> Siempre se retorna en meta.requestId.
> 
> Contrato de respuesta (backward-compatible)
> Se conserva la forma base:
> {
>   "success": false,
>   "message": "texto tecnico resumido",
>   "data": null,
>   "meta": {
>     "Total": 0,
>     "Page": 0,
>     "PageSize": 0,
>     "Status": "business",
>     "RequestId": "req-123",
>     "RetryAfterMs": null
>   },
>   "errors": [
>     {
>       "Type": "Business",
>       "Code": "DEL-BIZ-SHARED-ACTIVE",
>       "Field": "shared",
>       "Message": "Delete blocked by active shared relation",
>       "UserMessage": "El documento tiene relaciones compartidas activas"
>     }
>   ]
> }Reglas de lectura frontend
> message es tecnico resumido.
> 
> errors[].UserMessage es el texto recomendado para mostrar al usuario.
> 
> errors[].Message puede contener detalle tecnico seguro y sanitizado; no debe usarse como primera opcion de UX.
> 
> errors[].Code es identificador estable para UX/soporte.
> 
> meta.RequestId o meta.requestId debe mostrarse/copiarse en flujos de soporte cuando aplique.
> 
> Mapping HTTP y meta.status
> validation -> 400
> 
> forbidden -> 403
> 
> not_found -> 404
> 
> business -> 409
> 
> error -> 500
> 
> SCRUM-258 - Normalizacion de errores y contrato frontend
> SCRUM-258 no cambia el endpoint ni la forma base de AppResponses, pero si precisa la semantica de mensajes y confirma el contrato HTTP/MVC que consume el frontend.
> Contrato HTTP efectivo
> HTTP
> Resultado backend
> Lectura frontend
> 400 
> BadRequestObjectResult 
> Validacion o feature flag deshabilitado; mostrar warning controlado. 
> 404 
> NotFoundObjectResult 
> Documento/relacion no encontrada; mostrar mensaje funcional. 
> 403 
> ObjectResult con StatusCode = 403 
> Permisos/ownership; mostrar error de autorizacion. 
> 409 
> ObjectResult con StatusCode = 409 
> Bloqueo funcional; mostrar warning o bloqueo de negocio. 
> 500 
> ObjectResult con StatusCode = 500 
> Error tecnico; mostrar mensaje controlado y requestId. 
> Prioridad de mensaje para UI
> La UI debe resolver el texto visible en este orden:
> errors[0].UserMessage
> 
> errors[0].Message, solo como fallback y asumiendo que backend ya lo sanitizo
> 
> message, solo como fallback tecnico
> 
> Mensaje generico local si no existe envelope
> 
> No se debe mostrar message como texto principal cuando exista UserMessage.
> Campos tecnicos que no deben guiar UX primaria
> Campo
> Uso correcto
> message 
> Diagnostico resumido para logs/soporte; puede incluir code y requestId. 
> errors[].Message 
> Detalle tecnico seguro, sanitizado; fallback secundario. 
> errors[].Code 
> Seleccion de copy especifico, telemetria, soporte o reglas de UI. 
> meta.Status 
> Clasificacion general del error. 
> meta.RequestId 
> Correlacion con backend/logs. 
> Sanitizacion esperada desde backend
> El backend debe redactar rutas Windows/Linux/UNC, SQL, passwords, bearer tokens, cookies, connection strings, stack traces y saltos de linea. Aun asi, el frontend no debe inferir ni renderizar rutas fisicas, SQL ni stack traces.
> Causales clave de negocio
> DEL-BIZ-SHARED-ACTIVE
> 
> DEL-BIZ-PRODUCTION-RADICADO-ACTIVE
> 
> DELETE_FORBIDDEN_OWNER
> 
> DELETE_WORKFLOW_BLOCKED
> 
> DELETE_RADICADO_INVENTORY_BLOCKED
> 
> Nota para DELETE_WORKFLOW_BLOCKED
> Este codigo puede aparecer por tres causales funcionales:
> El documento tiene relacion real en dat_adic_tar{nombreRuta} y se intento eliminar desde un modulo distinto a WORKFLOW.
> 
> El documento es principal workflow y no existe sustituto valido para reemplazar ID_IMAGEN.
> 
> El documento es principal workflow con sustituto, pero el request no viene desde sourceModule=WORKFLOW.
> 
> Para UI, el mensaje debe venir de errors[0].UserMessage. No debe inferirse solo como "workflow activo".
> Causales físicas/compensación
> DEL-FS-COMPENSATION-PERSISTED
> 
> DEL-FS-COMPENSATION-FAILED
> 
> DEL-FS-XML-INDEX-RECONCILE (evidencia técnica de reconciliación de índice XML)
> 
> Política toast recomendada
> 400 validation y 400 business -> warning.
> 
> 401 -> warning (sesión/autenticación).
> 
> 403 -> error.
> 
> 409 business -> warning.
> 
> 500 error o sin respuesta backend -> error.
> 
> Matriz Front (Code -> Toast)
> Code
> HTTP
> meta.status
> Toast
> Mensaje a mostrar
> DELETE_RUNTIME_INVALID 
> 400 
> validation 
> warning 
> errors[0].UserMessage 
> DELETE_NOT_FOUND 
> 404 
> not_found 
> error 
> errors[0].UserMessage 
> DELETE_ALREADY_REMOVED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DEL-BIZ-SHARED-ACTIVE 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DEL-BIZ-PRODUCTION-RADICADO-ACTIVE 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_FORBIDDEN_OWNER 
> 403 
> forbidden 
> error 
> errors[0].UserMessage 
> DELETE_WORKFLOW_BLOCKED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_RADICADO_INVENTORY_BLOCKED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_EXPEDIENTE_BLOCKED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_SIGNED_DOCUMENT_BLOCKED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_SIGNED_VERSIONS_BLOCKED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_SNAPSHOT_INCONSISTENT 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_DB_CONCURRENCY_CONFLICT 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_DB_INCONSISTENT_STATE 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DELETE_DB_MUTATION_FAILED 
> 500 
> error 
> error 
> errors[0].UserMessage 
> DELETE_PHYSICAL_FAILED 
> 500 
> error 
> error 
> errors[0].UserMessage 
> DEL-FS-COMPENSATION-PERSISTED 
> 409 
> business 
> warning 
> errors[0].UserMessage 
> DEL-FS-COMPENSATION-FAILED 
> 500 
> error 
> error 
> errors[0].UserMessage 
> DEL-INT-UNEXPECTED 
> 500 
> error 
> error 
> errors[0].UserMessage si existe; si no, mensaje generico local 
> Fallback estricto recomendado
> Si existe errors[0].UserMessage, usar ese valor.
> 
> Si no existe UserMessage, usar errors[0].Message solo como fallback.
> 
> Si no existe errors[], usar message solo como fallback tecnico.
> 
> Si el fallback contiene rutas, SQL, stacktrace o tokens, reemplazar por mensaje generico local.
> 
> Nunca mostrar rutas físicas ni stacktrace aunque vengan en message.
> 
> SCRUM-251 - Impacto frontend
> SCRUM-251 no cambia el endpoint DELETE ni el contrato base AppResponses. La limpieza de ra_anexos_respuesta ocurre dentro de la fase transaccional DB del StorageEngine.
> Campos tecnicos adicionales
> Si el frontend o herramientas de soporte inspeccionan MutationReport, pueden aparecer campos adicionales:
> Campo
> Significado
> ResponseAttachmentRowsFound 
> Relaciones ra_anexos_respuesta encontradas para el documento 
> ResponseAttachmentRowsDeleted 
> Relaciones eliminadas dentro de la transaccion 
> ResponseAttachmentCleanupExecuted 
> Indica que el paso de cleanup fue evaluado 
> ResponseAttachmentCleanupFailed 
> Indica falla de cleanup antes del commit 
> RollbackExecuted 
> Indica rollback ejecutado en una falla transaccional reportada 
> RowsAffected.delete_response_attachments 
> Conteo de filas eliminadas en ra_anexos_respuesta 
> Nuevos codigos internos
> Codigo
> UX recomendada
> DELETE_RESPONSE_ATTACHMENT_NOT_FOUND 
> No mostrar error; condicion informativa cuando no habia relacion 
> DELETE_RESPONSE_ATTACHMENT_DUPLICATED 
> Warning operativo si se expone en soporte 
> DELETE_RESPONSE_ATTACHMENT_INCONSISTENT 
> Warning/error segun mapping backend 
> DELETE_RESPONSE_ATTACHMENT_CLEANUP_FAILED 
> Error transaccional; mostrar mensaje controlado de backend 
> La UI debe seguir usando errors[0].UserMessage cuando exista y no debe inferir rutas fisicas ni SQL desde message o errors[0].Message.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: API, DELETE, FRONTEND, INTEGRACION

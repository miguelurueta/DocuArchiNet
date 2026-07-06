## Why

LISTA-DOCUMENTOS-APPTRETABLE. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-295.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> Prompt Arquitectónico Frontend - Lista de Documentos Radicados / AppTreeTable
> Objetivo
>   Permitir que el frontend del listado de documentos radicados refresque y presente documentos relacionados sin perder filas  por paginación y sin mezclar anexos de respuesta cuando la pantalla requiere solo documentos principales.
>   El contrato debe permitir:
> DocumentRelationScope: controlar si el listado excluye anexos de respuesta, los incluye o devuelve solo anexos.
> 
> EnablePagination: pedir el conjunto completo cuando el flujo necesita refrescar después de almacenar un documento oanexo.
> 
> meta.total y data.pagination.total: reflejar el total real del filtro, no solo las filas visibles en una página.
> 
> Alcance UI
> Árbol/lista principal del radicado: mostrar documentos base sin anexos de respuesta.
> 
> Refresco posterior a almacenar documento/anexo: garantizar que el nuevo registro aparezca aunque quede fuera de laprimera página.
> 
> Vista de todos los relacionados: mostrar documentos y anexos en una sola tabla.
> 
> Vista exclusiva de anexos: mostrar solo filas relacionadas en ra_anexos_respuesta.
> 
> Búsqueda por radicado desde módulo externo: resolver por NombreGabinete, CampoRadicado y Radicado.
> 
>   No se crea una pantalla nueva. El frontend existente debe ajustar el payload del endpoint según el caso funcional.
> Endpoint Consumido
> Método: POST
> 
> Ruta: /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query
> 
> Controller: DocuArchi.Api/Controllers/GestorDocumental/Documentos/ListaDocumentosRadicados/ ListaDocumentosRadicadoController.cs
> 
> DTO request: MiApp.DTOs/DTOs/GestorDocumental/Documentos/ListaDocumentosRadicados/ListaDocumentosRadicadosDtos.cs
> 
> Service: MiApp.Services/Service/GestorDocumental/Documentos/ListaDocumentosRadicados/ListaDocumentosRadicadoService.cs
> 
> Repository: MiApp.Repository/Repositorio/GestorDocumental/Documentos/ListaDocumentosRadicados/ IListaDocumentosRadicadosRepository.cs
> 
> Envelope: AppResponses<object>
> 
> Autenticación: Bearer token
> 
> Claims requeridos: defaulalias, usuarioid
> 
> Reglas Funcionales
> El frontend debe enviar DocumentRelationScope explícitamente cuando el caso requiera anexos o filtrado específico.
> 
> EnablePagination=false debe usarse cuando se requiere refresco completo sin limitar por página.
> 
> La UI debe usar meta.total como fuente preferida y data.pagination.total como fallback.
> 
> Ante validación, no debe existir fallback silencioso a documentsOnly.
> 
> La UI no debe inferir anexos por nombre o extensión.
> 
> Al cambiar página, deben preservarse NombreGabinete, CampoRadicado, Radicado, DocumentRelationScope, filtros yordenamiento.
> 
> Valores de DocumentRelationScope
> documentsOnly: excluye anexos de respuesta.
> 
> includeResponseAttachments: incluye documentos y anexos.
> 
> responseAttachmentsOnly: devuelve solo anexos de respuesta.
> 
> Flujo de Consumo
> Carga inicial: documentsOnly, EnablePagination=true, Page=1, PageSize=25.
> 
> Árbol completo: documentsOnly, EnablePagination=false.
> 
> Refresco tras guardar: includeResponseAttachments, EnablePagination=false.
> 
> Vista solo anexos: responseAttachmentsOnly, EnablePagination=true.
> 
> Cambio de página: mantener scope y filtros.
> 
> Validación backend: mostrar error funcional sin fallback automático.
> 
> Request Base
> {
>   "ViewMode": "flatDocuments",
>   "DocumentRelationScope": "includeResponseAttachments",
>   "EnablePagination": false,
>   "CampoRadicado": "ENLASE",
>   "Radicado": "2200466700018",
>   "NombreGabinete": "CORRESPO",
>   "Page": 1,
>   "PageSize": 25
> }
> 
> ## Response Esperada
> 
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "tableId": "InboxListaDocumentosRadicado",
>     "columns": [],
>     "rows": [],
>     "pagination": {
>       "page": 1,
>       "pageSize": 25,
>       "total": 7
>     }
>   },
>   "meta": {
>     "status": "success",
>     "total": 7,
>     "page": 1,
>     "pageSize": 25
>   },
>   "errors": []
> }
> 
> ## Response de Validación
> 
> {
>   "success": false,
>   "message": "Validacion",
>   "data": null,
>   "meta": {
>     "status": "validation"
>   },
>   "errors": [
>     {
>       "field": "DocumentRelationScope",
>       "message": "DocumentRelationScope invalido"
>     }
>   ]
> }
> 
> ## Estados UI
> 
> - idle: listo para consultar.
> - loading: request en curso, bloquear recarga doble.
> - success: filas cargadas con total.
> - empty: consulta válida sin resultados.
> - validation: error de validación funcional.
> - unauthorized: sesión expirada.
> - forbidden: sin permisos.
> - error: fallo técnico o de red.
> 
> ## Mapeo UI
> 
> - Tabla/listado: data.rows
> - Columnas: data.columns
> - Paginador: data.pagination.page, pageSize, total
> - Contador total: meta.total, fallback data.pagination.total
> - Mensajes inline: errors[].field y errors[].message
> - Mensaje global: message, meta.status, HTTP status
> 
> ## Restricciones
> 
> - No calcular total, Limit, Offset ni lógica de anexos en frontend.
> - No loguear tokens, claims completos, SQL, contenido documental ni paths físicos.
> - No persistir payload completo con radicado salvo política aprobada.
> - El cambio debe ser compatible con consumidores actuales si omiten DocumentRelationScope.
> 
> ## Criterios de Aceptación
> - El refresco completo funciona sin perder registros por paginación.
> - El contador usa el total real del backend.
> - Cambiar scope reinicia Page=1.
> - No existe fallback silencioso ante validación.
> - La UI mantiene comportamiento estable y trazable.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: APPTREETABLE, IMPLEMENTACION, LISTADO

## Capabilities

### New Capabilities
- `lista-documentos-apptretable`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

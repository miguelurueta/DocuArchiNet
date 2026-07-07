# SCRUMCORE-295 - Contrato API

## Endpoint

| Propiedad | Valor |
|---|---|
| Metodo | `POST` |
| Ruta | `/api/GestorDocumental/Documentos/ListaDocumentosRadicados/query` |
| Envelope | `AppResponses<object>` |
| Autenticacion | Bearer token |
| Claims backend requeridos | `defaulalias`, `usuarioid` |
| Consumidor frontend | `useGestionRespuestaDocumentosTable` |

## Headers

| Header | Requerido | Nota |
|---|---|---|
| `Authorization: Bearer <token>` | Si | No loguear ni persistir. |
| `Content-Type: application/json` | Si | Body JSON. |

## Request Principal Implementado

```json
{
  "ViewMode": "flatDocuments",
  "Page": 1,
  "PageSize": 25,
  "SortDir": "ASC",
  "SortField": "ID",
  "ColumnMode": 2,
  "SearchType": 1,
  "Search": "",
  "StructuredFilters": [],
  "IncludeConfig": true,
  "EnablePagination": false,
  "EnableColumnFilters": false,
  "DocumentRelationScope": "documentsOnly",
  "NombreGabinete": "CORRESPO",
  "CampoRadicado": "ENLASE",
  "Radicado": "2500466700035",
  "TableId": "InboxListaDocumentosRadicado",
  "AplicaTrd": 0
}
```

## Reglas Del Request

- `DocumentRelationScope=documentsOnly` para el listado principal.
- `EnablePagination=false` para recibir el dataset completo.
- `Page=1` cuando la paginacion esta deshabilitada.
- `PageSize` se conserva por compatibilidad del DTO.
- `Search=""` en modo full-list para evitar recortes backend antes del filtro local.
- `SearchType` se conserva por compatibilidad.
- `NombreGabinete`, `CampoRadicado` y `Radicado` se preservan desde el contexto funcional.

## Valores De `DocumentRelationScope`

| Valor | Uso |
|---|---|
| `documentsOnly` | Lista principal sin anexos de respuesta. |
| `includeResponseAttachments` | Vista o flujo que requiere documentos y anexos. |
| `responseAttachmentsOnly` | Vista exclusiva de anexos de respuesta. |

## Response Soportado

El frontend soporta pagination y meta en camelCase y PascalCase:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "TableId": "InboxListaDocumentosRadicado",
    "Rows": [
      {
        "RowId": "9567",
        "Values": {
          "TIPODOCUMENTO": "Documento principal"
        },
        "Meta": {
          "NodeType": "documento",
          "HasChildren": false,
          "DocumentId": 9567,
          "NombreGabinete": "CORRESPO"
        }
      }
    ],
    "Pagination": {
      "Page": 1,
      "PageSize": 25,
      "Total": 7
    }
  },
  "meta": {
    "Total": 7,
    "status": "success"
  },
  "errors": []
}
```

## Resolucion De Totales

| Condicion | Fuente |
|---|---|
| Busqueda local activa | Total filtrado local. |
| `meta.total` existe | `meta.total`. |
| `meta.Total` existe | `meta.Total`. |
| `data.pagination.total` existe | `data.pagination.total`. |
| `data.Pagination.Total` existe | `data.Pagination.Total`. |
| No hay total backend | `rows.length`. |

## Validacion

- Errores de validacion se muestran sin fallback silencioso a otro scope.
- La UI no reintenta automaticamente con `documentsOnly` si backend rechaza un scope.
- Token ausente o claims faltantes se tratan como autenticacion/configuracion, no como lista vacia.

## Compatibilidad

- El mapper mantiene defaults compatibles.
- El hook documental es quien fuerza `EnablePagination=false`.
- Otros flujos pueden seguir usando `EnablePagination=true` si su caso de uso requiere grilla paginada.

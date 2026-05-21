# SCRUMCORE-217 - Integración BackEnd (SCRUM-205)

## Endpoint: Query (Listado jerárquico)

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- Service FE: `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`

### Request (campos obligatorios por contrato)

- `ViewMode` (`flatDocuments` | `hierarchical`)
- `Page`
- `PageSize`
- `SortDir` (`ASC` | `DESC`)
- `ParentRowId`
- `ParentNodeType`
- `Level`
- `IncludeConfig`
- `EnablePagination`
- `EnableColumnFilters`

Ejemplo (root):
```json
{
  "ViewMode": "flatDocuments",
  "Page": 1,
  "PageSize": 25,
  "SortDir": "ASC",
  "ParentRowId": null,
  "ParentNodeType": null,
  "Level": 1,
  "IncludeConfig": true,
  "EnablePagination": false,
  "EnableColumnFilters": false
}
```

Ejemplo (children):
```json
{
  "ViewMode": "hierarchical",
  "Page": 1,
  "PageSize": 25,
  "SortDir": "ASC",
  "ParentRowId": "ROW-1",
  "ParentNodeType": "folder",
  "Level": 2,
  "IncludeConfig": false,
  "EnablePagination": false,
  "EnableColumnFilters": false
}
```

### Response (campos obligatorios por contrato)

- `Rows[].RowId`
- `Rows[].Values`
- `Rows[].Meta.NodeType`
- `Rows[].Meta.ParentId`
- `Rows[].Meta.HasChildren`
- `Rows[].Meta.DocumentId`
- `Rows[].Meta.NombreGabinete`

Ejemplo:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Rows": [
      {
        "RowId": "ROW-1",
        "Values": {
          "ID": 1,
          "TIPODOCUMENTO": "DOC 1"
        },
        "Meta": {
          "NodeType": "documento",
          "ParentId": null,
          "HasChildren": false,
          "DocumentId": 99,
          "NombreGabinete": "GAB"
        }
      }
    ],
    "Config": null
  },
  "errors": null
}
```

## Endpoint: Action (ver_documento y acciones dinámicas)

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- Service FE: `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`

### Request (campos obligatorios por contrato)

- `ActionId`
- `RowId`
- `NodeType`
- `Payload.IdDocumento`
- `Payload.NombreGabinete`

Ejemplo (`ver_documento`):
```json
{
  "TableId": "InboxListaRadicados",
  "ViewMode": "flatDocuments",
  "ActionId": "ver_documento",
  "RowId": "ROW-1",
  "NodeType": "documento",
  "Payload": {
    "IdDocumento": 99,
    "NombreGabinete": "GAB"
  }
}
```

### Response

El backend retorna `DocumentResolveRequest` para proceder al resolve del visor.

Ejemplo:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "DocumentResolveRequest": {
      "NombreGabinete": "GAB",
      "IdDocumento": 99
    }
  }
}
```

## Endpoint: Resolve (Visor)

- Endpoint: `POST /api/gestor-documental/documentos/visualizacion/resolve`
- Service FE: `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`

### Request
```json
{
  "NombreGabinete": "GAB",
  "IdDocumento": 99
}
```

### Response (esperado por FE)

El FE espera obtener un `fileUrl` (o equivalente) para entregar a `AppVisorEmbedPdf`.

## Matriz FE → BE

| Acción | Endpoint | Entrada | Salida | Uso FE |
|---|---|---|---|---|
| Cargar root | `.../query` | `flatDocuments` + `IncludeConfig=true` | `Rows + Config` | Render `AppTreeTable` + columnas dinámicas |
| Cargar children | `.../query` | `hierarchical` + `ParentRowId` | `Rows` | Expand/collapse |
| ver_documento | `.../action` | `ActionId=ver_documento` + payload | `DocumentResolveRequest` | Resolver visor |
| resolve visor | `.../resolve` | `NombreGabinete + IdDocumento` | `fileUrl` | `AppVisorEmbedPdf(fileUrl)` |

## Errores y resiliencia

- Si `success=false`, mostrar `errors[0].errorMessage` (si existe) como mensaje en UI.
- En `ver_documento`, si hay error, NO cambiar el documento activo.

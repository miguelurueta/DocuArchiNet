# SCRUMCORE-218 - Integración BackEnd (Normalización Contrato Action)

## Endpoint: Query (Listado)

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- Service FE: `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`

### Request

Se mantiene contrato de query root con `IncludeConfig: true`.

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

### Response soportada por FE

El frontend ahora soporta ambos formatos:

1) Legacy:
```json
{
  "success": true,
  "data": {
    "Rows": [],
    "Config": {
      "TableId": "InboxListaDocumentosRadicado",
      "Columns": [],
      "CellActions": [],
      "MenuActions": []
    }
  }
}
```

2) Actual:
```json
{
  "success": true,
  "data": {
    "Rows": [],
    "TableId": "InboxListaDocumentosRadicado",
    "Columns": [],
    "CellActions": [],
    "MenuActions": []
  }
}
```

## Endpoint: Action

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- Service FE: `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`

### Request normalizado en FE

El request usa:
- `TableId` efectivo de `tableIdRef` (backend o fallback)
- `RowId`
- `Payload` con prioridad de identificador

Regla de payload:
- Si existe `IdDocumento`, se envía `Payload.IdDocumento`.
- Si no existe `IdDocumento` y existe `DocumentId`, se envía `Payload.DocumentId`.
- `Payload.NombreGabinete` se conserva cuando está disponible.

Ejemplo (`IdDocumento` presente):
```json
{
  "TableId": "InboxListaDocumentosRadicado",
  "ViewMode": "flatDocuments",
  "ActionId": "ver_documento",
  "RowId": "ROW-1",
  "NodeType": "documento",
  "Payload": {
    "IdDocumento": 15416,
    "NombreGabinete": "WF_DOCS"
  }
}
```

Ejemplo (fallback `DocumentId`):
```json
{
  "TableId": "InboxListaDocumentosRadicado",
  "ViewMode": "flatDocuments",
  "ActionId": "ver_documento",
  "RowId": "ROW-2",
  "NodeType": "documento",
  "Payload": {
    "DocumentId": 15417,
    "NombreGabinete": "WF_DOCS"
  }
}
```

## Matriz FE -> BE

| Operación | Endpoint | Entrada clave | Salida usada por FE |
|---|---|---|---|
| Cargar root | `.../query` | `IncludeConfig=true` | Tabla dinámica (legacy o directa) + rows |
| Ejecutar acción | `.../action` | `TableId + RowId + Payload` | Resultado de acción / resolve request |

## Confirmación de alcance

- No se cambiaron endpoints.
- No se cambió backend.
- No se alteraron rutas.

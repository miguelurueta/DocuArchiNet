# SCRUMCORE-224 - Integracion BackEnd

## Estado de integracion
No aplica cambio backend.

## Confirmacion explicita
- Endpoints: intactos.
- Contratos backend: intactos.
- Request/response existentes: reutilizados sin cambios.

## Endpoints usados (sin modificar)
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- `POST /api/gestor-documental/documentos/visualizacion/resolve`

## Uso de campos para conteo (lectura no intrusiva)
El contador total intenta leer, en orden:
1. `Total`
2. `TotalRecords`
3. fallback `rows.length`

## Comportamiento post-mutacion
- Tras `agregar_item` o `eliminar_item`, se prioriza conteo runtime (`rows/treeRows` actuales) para evitar desincronizacion.
- No se agregan nuevos parametros ni headers en llamadas.

## Ejemplos JSON relevantes

### Ejemplo con Total
```json
{
  "success": true,
  "data": {
    "Rows": [{"RowId": "r1"}],
    "Total": 25
  }
}
```

### Ejemplo con TotalRecords
```json
{
  "success": true,
  "data": {
    "Rows": [{"RowId": "r1"}],
    "TotalRecords": 25
  }
}
```

### Ejemplo sin total (fallback)
```json
{
  "success": true,
  "data": {
    "Rows": [{"RowId": "r1"}, {"RowId": "r2"}]
  }
}
```

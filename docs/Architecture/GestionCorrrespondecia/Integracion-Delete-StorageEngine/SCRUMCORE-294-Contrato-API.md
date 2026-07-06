# SCRUMCORE-294 - Contrato API Delete StorageEngine

## Endpoint

```http
DELETE /api/gestor-documental/eliminar-documento/{idAlmacen:long}?nombreGabinete={nombreGabinete}&sourceModule=WORKFLOW
```

## Claims requeridos

- `defaulalias`
- `usuarioid`

## Request ID

- Header recomendado: `X-Request-Id`
- Si no se envia, backend puede generar `meta.requestId`
- El frontend debe preservar el request id para soporte cuando exista

## Contracto de respuesta esperado

```json
{
  "success": false,
  "message": "texto tecnico resumido",
  "data": null,
  "meta": {
    "Status": "business",
    "RequestId": "req-123"
  },
  "errors": [
    {
      "Type": "Business",
      "Code": "DEL-BIZ-SHARED-ACTIVE",
      "Field": "shared",
      "Message": "Delete blocked by active shared relation",
      "UserMessage": "El documento tiene relaciones compartidas activas"
    }
  ]
}
```

## Lectura frontend

| Campo | Uso |
| --- | --- |
| `errors[0].UserMessage` | Mensaje principal de usuario |
| `errors[0].Message` | Fallback secundario |
| `message` | Fallback tecnico |
| `meta.requestId` | Correlacion de soporte |

## Severidad recomendada

| Caso | Severidad |
| --- | --- |
| `400 validation` | warning |
| `400 business` | warning |
| `401` | warning |
| `403` | error |
| `404` | error |
| `409 business` | warning |
| `500` | error |

## Codigos funcionales relevantes

- `DEL-BIZ-SHARED-ACTIVE`
- `DEL-BIZ-PRODUCTION-RADICADO-ACTIVE`
- `DELETE_FORBIDDEN_OWNER`
- `DELETE_WORKFLOW_BLOCKED`
- `DELETE_RADICADO_INVENTORY_BLOCKED`
- `DELETE_EXPEDIENTE_BLOCKED`
- `DELETE_SIGNED_DOCUMENT_BLOCKED`
- `DELETE_SIGNED_VERSIONS_BLOCKED`
- `DELETE_DB_CONCURRENCY_CONFLICT`
- `DELETE_DB_INCONSISTENT_STATE`
- `DELETE_DB_MUTATION_FAILED`
- `DELETE_PHYSICAL_FAILED`

## Reglas

- No renderizar rutas fisicas, SQL, tokens o stack traces como copy principal.
- No inferir `DELETE_WORKFLOW_BLOCKED` solo como "workflow activo"; usar `UserMessage`.
- `DEL-FS-COMPENSATION-FAILED` y `DEL-INT-UNEXPECTED` se tratan como error tecnico.

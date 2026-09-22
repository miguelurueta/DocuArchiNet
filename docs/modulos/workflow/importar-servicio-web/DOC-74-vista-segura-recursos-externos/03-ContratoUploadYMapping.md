# Contrato y mapping

## Request `GetPreview`

Usa el envelope ASMX existente con `SchemaVersion`, `OperationId`, `CorrelationId`, `TaskId`, `ProviderId` y `ExternalKey`. `TaskId` se contrasta en servidor contra sesión.

## Response consumida

| Campo | Uso |
| --- | --- |
| `DescriptorId` | Token opaco base64url para el handler same-origin |
| `ContentType` | Decide vista embebida o fallback |
| `ExpiresAtUtc` | Metadata temporal; backend mantiene la autoridad |
| `Error.Codigo` | Traducción a estados cerrados |

No se transportan bytes, tokens de proveedor, rutas físicas ni respuestas externas completas.

## Mapping de fila

`ExternalKey` se mapea a `externalKey`. Metadata `INTERNAL_DOCUMENT_ID`, `IMAGE_ID` o `ID_IMAGEN` se mapea a `internalDocumentId` únicamente para buscar una fila autorizada de `GridView_list_documento_relacion_wf`. El adaptador exige coincidencia de `id_wf`, identidad dentro de `idd_wf` y tarea confiable antes de usar la selección generada por servidor. No se mezcla con `DescriptorId`.

No hay upload ni deduplicación documental en DOC-74. La deduplicación implementada corresponde a la promesa de preview por apertura.

# Contratos e integraciones

- Ticket: DOC-56
- Cambio OpenSpec: doc-56-pruebas-integracion
- Clasificacion: cross_cutting

## Envelope v1

Solicitudes y respuestas fijan `SchemaVersion="1.0"`. La solicitud base contiene `OperationId`, `CorrelationId`, `TaskId` y `ProviderId`; la respuesta base devuelve operación, correlación y `ErrorImportacionServicioDto`.

## `ImportItemResultDto`

| Campos | Objetivo |
|---|---|
| `ClientItemId`, `ExternalKey` | elemento y referencia opaca |
| `Status`, `ReachedPhase` | estado visible y fase interna |
| `TaskId`, `DocumentId` | identidad para refrescar la interfaz |
| `DocumentName`, `ContentType` | presentación del documento |
| `PersistenceKnown`, `Retryable` | recuperación segura |
| `ErrorCode`, `Message`, `CorrelationId` | diagnóstico y trazabilidad |

`ImportItemResultMapper.Map` usa conteos de documento, relación con la tarea y relaciones con otras tareas. Si `Status <> "Disponible"`, asigna `Nothing` a `DocumentId`, `DocumentName` y `ContentType`.

## SII efectivo

`QueryItemsAsync` exige `CodigoBarras`, solicita token y llama `consultarInformacionSello`, enviando el código en el campo externo `radicado`. `SiiImportContractMapper` aplana `inscripciones[].imagenes[]` a `ExternalItemDto`.

`ExternalKey` comienza con `SII2` y representa código de barras, libro, registro e `idanexo`. Es opaca: no es una URL ni autoridad de descarga.

`GetPreviewAsync`, `DownloadAsync` y `ResolveStorageMetadataAsync` decodifican la clave, reconsultan el sello y seleccionan una única imagen. La descarga acepta únicamente HTTP/HTTPS, host permitido, PDF/TIFF/PNG/JPEG y tamaño válido. La URL no aparece en el DTO público.

## Salida confirmada de ejecución

`ExecuteImportIntentResponseDto.Items[]` devuelve `Status="Disponible"`, `TaskId`, `DocumentId`, `DocumentName`, `ContentType`, `ReachedPhase`, `PersistenceKnown=true` y correlación cuando gabinete y relación están confirmados. Sin confirmación, `ProjectExecutionResult` produce `ResultadoIncierto`, `IMPORT_RESULT_UNCERTAIN` y `DocumentId=null`.

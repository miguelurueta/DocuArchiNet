# Contrato de importación y mapeo

Aunque el nombre del documento conserva “Upload”, DOC-50 no carga ni almacena documentos. Publica contratos serializables `SchemaVersion = "1.0"`; una evolución aditiva puede incorporar campos opcionales, mientras que eliminar, reinterpretar o hacer obligatorio un campo exige una versión nueva.

## Campos comunes y nulabilidad

Requests heredan `SchemaVersion`, `OperationId`, `CorrelationId`, `TaskId` y `ProviderId`. Responses heredan versión, operación, correlación y `Error`. Identificadores requeridos por cada operación deben llegar informados. `OperationId` es opcional en preview cuando no exista una operación anterior; tokens de continuación, longitudes, expiración, tipo documental y documento usan `Nothing` cuando contractualmente son opcionales. Las colecciones de respuesta se inicializan vacías.

`GetPreview` devuelve descriptor mediado, tipo, tamaño, disposición y expiración; no expone URL externa, credencial, ruta física ni respuesta cruda. `CreateImportIntent` requiere `IdempotencyKey`; `ExecuteImportIntent` requiere `VersionToken`.

## Operación, DTO y fixture

| Operación | DTOs | Fixture contractual |
| --- | --- | --- |
| ResolveCapabilities | `ResolveCapabilitiesRequestDto`, `ResolveCapabilitiesResponseDto` | `resolve-capabilities-request.json`, `resolve-capabilities-response.json` |
| QueryItems | `QueryItemsRequestDto`, `QueryItemsResponseDto` | `query-items-response.json` |
| GetPreview | `GetPreviewRequestDto`, `GetPreviewResponseDto` | Forma verificada estructuralmente; no se añadió fixture fuera de la lista canónica |
| PreflightImport | `PreflightImportRequestDto`, `PreflightImportResponseDto` | `preflight-import-response.json` |
| CreateImportIntent | `CreateImportIntentRequestDto`, `CreateImportIntentResponseDto` | `create-import-intent-response.json` |
| ExecuteImportIntent | `ExecuteImportIntentRequestDto`, `ExecuteImportIntentResponseDto` | `execute-import-intent-response.json` |
| GetImportIntent | `GetImportIntentRequestDto`, `GetImportIntentResponseDto` | `get-import-intent-response.json` |
| ReconcileImportIntent | `ReconcileImportIntentRequestDto`, `ReconcileImportIntentResponseDto` | `reconcile-import-intent-response.json` |

Los ocho archivos JSON canónicos están en `Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/`, usan `schemaVersion: "1.0"` y datos saneados. El mapeo de capacidades y elementos entre Modelo y DTO ocurre en la fachada; los contratos futuros no tienen ejecución productiva en esta entrega.

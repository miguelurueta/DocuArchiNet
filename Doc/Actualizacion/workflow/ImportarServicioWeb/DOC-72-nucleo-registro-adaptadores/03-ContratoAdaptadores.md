# Contrato HTTP y adaptadores

## Transporte

`ImportarServicioWebApi.create(options)` acepta `options.transport` y `options.baseUrl`. `invoke(operation, request)` rechaza operaciones fuera de la lista cerrada, envía `POST` a `../webservice/WebServiceImportarServicioWebModern.asmx/{Operation}`, usa `credentials: same-origin` y serializa `{ "request": request }`. Una respuesta sin `json()`, HTTP no exitosa o envelope `d` inválido produce `IMPORT_TRANSPORT_FAILED` o `IMPORT_RESPONSE_INVALID`.

## Registro

`normalizeProviderId` aplica conversión a texto, trim y mayúsculas. `register(providerId, adapter)` exige identidad y objeto. `resolve` devuelve copias defensivas de capacidades o uno de estos resultados:

| Condición | Código |
| --- | --- |
| ID vacío | `PROVIDER_NOT_CONFIGURED` |
| ID incluido en `knownNotMigrated` | `PROVIDER_NOT_MIGRATED` |
| ID no registrado | `PROVIDER_NOT_SUPPORTED` |

## Adaptador productivo creado por UI

`createBackendAdapter(api, control)` declara capacidades locales y dos funciones: `queryItems(request)` y `executeImportIntent(request)`. La primera exige tarea positiva, llama `ResolveCapabilities`, propaga `Error.Codigo`, agrega `CodigoBarras` y `PageSize=50`, y llama `QueryItems`. La segunda combina contexto visual con la solicitud recibida y llama `ExecuteImportIntent`.

La declaración local de capacidades no sustituye la respuesta backend: antes de consultar se verifica `ResolveCapabilities`. El código actual no transforma `Capabilities` ni `DocumentTypes` en controles visibles.

## Operaciones publicadas y consumo DOC-72

| Operación ASMX | Cliente generado | Uso actual desde UI |
| --- | --- | --- |
| `ResolveCapabilities` | `resolveCapabilities(request)` | Sí, antes de consulta |
| `QueryItems` | `queryItems(request)` | Sí |
| `GetPreview` | `getPreview(request)` | Cliente disponible; sin control UI |
| `PreflightImport` | `preflightImport(request)` | Cliente disponible; sin control UI |
| `CreateImportIntent` | `createImportIntent(request)` | Cliente disponible; sin control UI |
| `ExecuteImportIntent` | `executeImportIntent(request)` | Adaptador/core disponible; sin disparador UI |
| `GetImportIntent` | `getImportIntent(request)` | Cliente disponible; no usado |
| `ReconcileImportIntent` | `reconcileImportIntent(request)` | Cliente disponible; no usado |

## Autorización y validación backend

Todos los métodos tienen `WebMethod(EnableSession:=True)`. La frontera valida primero `FeatureEnabled`, luego `ValidRequest` y después contexto de sesión/tarea. `ValidRequest` exige `TaskId>0`, `OperationId`, `CorrelationId` y proveedor igual al canónico SII. La ocultación del modal no constituye autorización.

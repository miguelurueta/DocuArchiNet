# Flujo de integración

La frontera futura es HTTPS `POST` v1 en `/webservice/WebServiceImportarServicioWeb.asmx/<Operation>`. DOC-50 publica formas contractuales; solo la fachada de capacidades y consulta tiene orquestación interna, siempre sin red ni persistencia.

| Operación | Request / response | Autoridad y dependencias | Efectos en DOC-50 |
| --- | --- | --- | --- |
| `ResolveCapabilities` | `ResolveCapabilitiesRequestDto` / `ResolveCapabilitiesResponseDto` | contexto, validador, registro, proveedor | Ninguno; delegación disponible |
| `QueryItems` | `QueryItemsRequestDto` / `QueryItemsResponseDto` | contexto, validador, registro, proveedor | Ninguno; delegación disponible |
| `GetPreview` | `GetPreviewRequestDto` / `GetPreviewResponseDto` | tarea, proveedor, elemento y operación | Ninguno; solo contrato mediado |
| `PreflightImport` | `PreflightImportRequestDto` / `PreflightImportResponseDto` | contexto y validación futura | Ninguno; solo contrato |
| `CreateImportIntent` | `CreateImportIntentRequestDto` / `CreateImportIntentResponseDto` | contexto y clave de idempotencia | Ninguno; solo contrato |
| `ExecuteImportIntent` | `ExecuteImportIntentRequestDto` / `ExecuteImportIntentResponseDto` | intención y token de versión | Ninguno; solo contrato |
| `GetImportIntent` | `GetImportIntentRequestDto` / `GetImportIntentResponseDto` | contexto e identidad de intención | Ninguno; solo contrato |
| `ReconcileImportIntent` | `ReconcileImportIntentRequestDto` / `ReconcileImportIntentResponseDto` | estado persistido futuro | Ninguno; solo contrato |

Secuencia implementada para las dos primeras operaciones:

1. La frontera futura construye el contexto con fuentes autorizadas antes de cualquier `Await`.
2. `ValidadorContextoImportacion` aplica reglas en orden y detiene la primera falla.
3. El registro resuelve exactamente `ProviderId`.
4. La fachada delega capacidades o consulta al puerto del proveedor.
5. La fachada devuelve versión, operación, correlación y error seguro.

El comando y resultado documental por elemento son auxiliares de preflight/intención, no una novena operación. Ninguna secuencia de DOC-50 cambia tarea, estado, auditoría, expediente o documento.

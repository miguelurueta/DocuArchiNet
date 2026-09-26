# Inventario técnico verificado

## APIs

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
|---|---|---|---|---|---|---|---|
| Endpoint POST, sesión requerida | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | `WebServiceImportarServicioWebModern` | `/webservice/WebServiceImportarServicioWebModern.asmx/ResolveCapabilities` | `request As ResolveCapabilitiesRequestDto` | `ResolveCapabilitiesResponseDto` | Gate, solicitud, contexto y capacidades | `TryBuildImportContext`, `SiiImportProvider` |
| Endpoint POST, sesión requerida | misma | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/QueryItems` | `request As QueryItemsRequestDto` | `QueryItemsResponseDto` | Consulta no mutadora; barcode cliente sobrescrito | `Class_DAT_ADIC_TAR`, provider, mapper |
| Endpoint POST, sesión requerida | misma | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/GetPreview` | `request As GetPreviewRequestDto` | `GetPreviewResponseDto` | Reconsulta y crea descriptor | provider, descriptor service/repository |
| Endpoint GET/HEAD, sesión requerida | `workflow/ImportarServicioWebPreview.ashx.vb` | `ImportarServicioWebPreview` | `/workflow/ImportarServicioWebPreview.ashx?d={descriptor}` | `d As String` | Binario/cabeceras HTTP | Consume descriptor ligado a autoridad | content service/repository |

Los POST ASMX devuelven HTTP 200 también para errores funcionales, representados por `ErrorImportacionServicioDto`. El handler retorna HTTP 200, 404, 405 o 503 según `Diagramas/04-streaming-actividad.puml`.

## Clases, interfaces, servicios y repositorios

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
|---|---|---|---|---|---|---|---|
| Controller | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | `WebServiceImportarServicioWebModern` | `TryBuildImportContext` | `SolicitudImportacionServicioDto, ByRef ContextoImportacionServicio, ByRef ResultadoContextoSesionWorkflow, ByRef String` | `Boolean` | Reconstruye contexto y exige `ENLASE`/tarea coincidente | sesión, `WorkflowPreviewSessionContextGate`, `Classselecciotarea` |
| Controller | misma | misma | `TryResolveTrustedSiiReferences` | `ContextoImportacionServicio, ByRef String, ByRef String` | `Boolean` | Obtiene recibo/barcode confiables y aplica longitudes 80/15 | `Class_DAT_ADIC_TAR` |
| Interfaz | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb` | `IExternalImportProviderClient` | `ResolveCapabilitiesAsync`, `QueryItemsAsync`, `GetPreviewAsync`, `DownloadAsync` | Ver firmas en código | `Task(Of ...)` | Puerto de proveedor compartido | Implementada por `SiiImportProvider` |
| Service/provider | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | `SiiImportProvider` | `ResolveCapabilitiesAsync` | `ResolveCapabilitiesRequestDto, CancellationToken` | `Task(Of ResolveCapabilitiesResponseDto)` | Publica capability | cliente SII |
| Service/provider | misma | misma | `QueryItemsAsync` | `QueryItemsRequestDto, CancellationToken` | `Task(Of QueryItemsResponseDto)` | Despacha por capability | `SiiExternalImportProviderClient` |
| Service/provider | misma | misma | `GetPreviewContentAsync` | `GetPreviewRequestDto, CancellationToken` | `Task(Of SiiPreviewContent)` | Despacha preview por capability | cliente SII |
| Service HTTP | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | `SiiExternalImportProviderClient` | `QueryAnnexesAsync` | `QueryItemsRequestDto, CancellationToken` | `Task(Of QueryItemsResponseDto)` | Token + `consultarRadicado` + mapping | transport, mapper, telemetría |
| Service HTTP | misma | misma | `GetAnnexPreviewContentAsync` | `GetPreviewRequestDto, CancellationToken` | `Task(Of SiiPreviewContent)` | Reconsulta, resuelve y descarga | allowlist, tamaño, content type |
| Mapper | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiEnlaseAnnexContractMapper.vb` | `SiiEnlaseAnnexContractMapper` | `MapQuery` / `Resolve` | `JObject, QueryItemsRequestDto` / `JObject, String` | `QueryItemsResponseDto` / `SiiEnlaseAnnex` | Identidad `idanexo`, duplicados fail-closed | `SiiImportContractMapper` |
| Service | `Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorService.vb` | `ImportPreviewDescriptorService` | `Create` | `ContextoImportacionServicio, SiiPreviewContent, DateTime` | `ImportPreviewCreationResult` | Token/hash/TTL/autoridad | `IImportPreviewDescriptorRepository` |
| Interfaz repositorio | `Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewModels.vb` | `IImportPreviewDescriptorRepository` | `Create`, `GetAvailableMetadata`, `ClaimAndLoad`, `MarkConsumed`, `CleanupExpired` | Ver implementación | `Boolean`, `ImportPreviewSnapshot`, `Integer` | Puerto de descriptores | `ImportPreviewDescriptorRepository` |
| Repositorio MySQL | `Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorRepository.vb` | `ImportPreviewDescriptorRepository` | `Create`, `GetAvailableMetadata`, `ClaimAndLoad`, `MarkConsumed`, `CleanupExpired` | Snapshot/hash/authority/DateTime | `Boolean`, snapshot o `Integer` | INSERT/SELECT/UPDATE/DELETE parametrizados; filtra usuario+tarea+provider+estado+expiración | connection/executor/transaction factories |
| Handler/security | `workflow/ImportarServicioWebPreview.ashx.vb` | `ImportarServicioWebPreview` | `TryResolveTrustedTaskId` | `HttpContext, ByRef Long` | `Boolean` | Si la selección es ENLASE exige la tarea ENLASE y coincidencia con `SELECCIONTEMPORAL`; fuera de ENLASE usa la tarea estándar | sesión HTTP; sin fallback cruzado |
| Service | `Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewContentService.vb` | `ImportPreviewContentService` | `Head`, `Claim`, `Complete` | descriptor/authority/DateTime o snapshot/DateTime | snapshot / `Boolean` | Metadatos, claim y consumo | repositorio descriptor |

## DTOs y modelos

En estos DTOs no hay atributos declarativos de validación; la obligatoriedad indicada es validación imperativa en Controller/Service.

| DTO/modelo | Propiedad: tipo | Obligatoria / validación |
|---|---|---|
| `SolicitudImportacionServicioDto` | `SchemaVersion:String`; `OperationId:String`; `CorrelationId:String`; `TaskId:Long`; `ProviderId:String`; `Capability:String` | operación/correlación no vacías, tarea > 0, provider `INTEGRACIONSII`; capability vacía=histórica o exactamente `ANEXOS_RADICADO_ENLASE` |
| `QueryItemsRequestDto` | heredadas; `CodigoBarras:String`; `ContinuationToken:String`; `PageSize:Nullable(Of Integer)` | barcode es asignado por servidor y debe medir 1..15; paginación no altera consulta SII |
| `QueryItemsResponseDto` | envelope; `Items:IList(Of ExternalItemDto)`; `ContinuationToken:String`; `ProviderResultCode:String`; `InscriptionCount:Integer`; `ImageCount:Integer`; `Radicado:String` | listas inicializadas; radicado se reconstruye |
| `ExternalItemDto` | `ExternalKey:String`; `DisplayName:String`; `ContentType:String`; `Length:Nullable(Of Long)`; `PreviewAvailable:Boolean`; `PresentationSchemaVersion:String`; `Metadata:IList(Of ImportItemMetadataDto)`; `ImportStatus:String`; `AllowedActions:IList(Of String)` | `ExternalKey=idanexo` obligatorio/único; acción implementada `PREVIEW` |
| `ImportItemMetadataDto` | `SchemaVersion:String`; `Code:String`; `Label:String`; `Value:String` | solo se agregan valores no vacíos |
| `GetPreviewRequestDto` | heredadas; `ExternalKey:String`; `CodigoBarras:String` | external key obligatorio; barcode siempre sobrescrito por servidor |
| `GetPreviewResponseDto` | envelope; `ExternalKey:String`; `DescriptorId:String`; `ContentType:String`; `Length:Nullable(Of Long)`; `Disposition:String`; `ExpiresAtUtc:Nullable(Of DateTime)` | descriptor opaco, expiración del snapshot |
| `ErrorImportacionServicioDto` | `Codigo:String`; `MensajeVisible:String`; `ReferenciaTrazabilidad:String`; `EsReintentable:Boolean` | código saneado; no contiene secreto/URL/cuerpo SII |
| `SiiEnlaseAnnex` | 17 propiedades `String` de contrato y `FileName:String` read-only | `IdAnexo` obligatorio/único; URL solo interna; formato normalizado |

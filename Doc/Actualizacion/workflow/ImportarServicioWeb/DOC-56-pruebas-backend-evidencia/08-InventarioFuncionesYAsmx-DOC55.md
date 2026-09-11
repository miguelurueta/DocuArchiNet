# Inventario detallado de funciones y ASMX implementados

- Ticket: DOC-55
- Cambio: `doc-55-adaptador-sii-asmx`
- Corte auditado: commit `b9ab313f0c6d6ff1d7d26ded81d5b49b216a793f`
- Alcance: miembros ejecutables incorporados por DOC-55, incluidos constructores, métodos públicos, WebMethods y helpers privados.

> El inventario usa el corte final de DOC-55 para no atribuirle operaciones agregadas posteriormente por DOC-56. Las rutas siguen vigentes dentro del repositorio, aunque algunas clases evolucionaron después del corte auditado.

## ASMX implementado

| Elemento | Objeto/clase | Ruta dentro del repositorio | Configuración | Propósito |
|---|---|---|---|---|
| Descriptor ASMX | `WebServiceImportarServicioWebModern` | `webservice/WebServiceImportarServicioWebModern.asmx` | `Language="VB"`, `CodeBehind="WebServiceImportarServicioWebModern.asmx.vb"` | Publica la nueva frontera paralela para el proveedor SII. |
| Code-behind | `WebServiceImportarServicioWebModern` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | `ScriptService`, `WebService`, `BasicProfile1_1`, sesión habilitada por operación | Evalúa gate y contexto, resuelve el proveedor y delega capacidades, consulta y preview. |

## WebMethods públicos

| # | Función | Objeto/clase | Parámetros | Retorno | Ruta dentro del repositorio | Responsabilidad |
|---:|---|---|---|---|---|---|
| 1 | `ResolveCapabilities` | `WebServiceImportarServicioWebModern` | `request As ResolveCapabilitiesRequestDto` | `Task(Of ResolveCapabilitiesResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Valida gate, request y contexto; resuelve `INTEGRACIONSII` y delega en `ResolveCapabilitiesAsync`. |
| 2 | `QueryItems` | `WebServiceImportarServicioWebModern` | `request As QueryItemsRequestDto` | `Task(Of QueryItemsResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Valida la frontera y delega la consulta asíncrona del proveedor; sanea fallos externos. |
| 3 | `GetPreview` | `WebServiceImportarServicioWebModern` | `request As GetPreviewRequestDto` | `Task(Of GetPreviewResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Obtiene metadatos del preview y los media con `SiiPreviewResponseFactory` antes de responder. |

Los tres métodos tienen `[WebMethod(EnableSession:=True)]` y `[ScriptMethod(ResponseFormat:=Json)]`.

## Inventario exhaustivo de miembros ejecutables

| # | Visibilidad | Función o constructor | Objeto/clase | Parámetros | Retorno | Ruta dentro del repositorio | Responsabilidad |
|---:|---|---|---|---|---|---|---|
| 1 | `Public` | `New` | `RegistroClientesProveedoresImportacion` | `clientes As IEnumerable(Of IExternalImportProviderClient)` | Constructor | `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb` | Construye el registro case-insensitive, rechaza clientes nulos, identidades vacías y duplicados. |
| 2 | `Public` | `Resolver` | `RegistroClientesProveedoresImportacion` | `providerId As String` | `ResultadoResolucionClienteProveedorImportacion` | `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb` | Devuelve el cliente registrado o `PROVIDER_NOT_SUPPORTED`; implementa `IRegistroClientesProveedoresImportacion.Resolver`. |
| 3 | `Private Shared` | `Normalizar` | `RegistroClientesProveedoresImportacion` | `providerId As String` | `String` | `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb` | Convierte `Nothing` en vacío y aplica `Trim` a la identidad. |
| 4 | `Public` | `New` | `SiiExternalImportProviderClient` | `transport As ExternalImportHttpTransport`; `baseUri As Uri`; `authorizationHeader As String`; `timeout As TimeSpan`; `maximumResponseBytes As Long` | Constructor | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | Valida e inicializa transporte, URL base, autorización, timeout y límite de respuesta. |
| 5 | `Public` | `QueryItemsAsync` | `SiiExternalImportProviderClient` | `request As QueryItemsRequestDto`; `cancellationToken As CancellationToken` | `Task(Of Byte())` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | Construye `items?taskId=...`, agrega paginación opcional y solicita JSON. |
| 6 | `Public` | `GetPreviewMetadataAsync` | `SiiExternalImportProviderClient` | `request As GetPreviewRequestDto`; `cancellationToken As CancellationToken` | `Task(Of Byte())` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | Solicita `preview/{ExternalKey}` después de exigir una clave externa. |
| 7 | `Public` | `DownloadAsync` | `SiiExternalImportProviderClient` | `externalKey As String`; `correlationId As String`; `cancellationToken As CancellationToken` | `Task(Of Byte())` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | Solicita `resource/{ExternalKey}` como PDF después de validar la clave. |
| 8 | `Private` | `SendAsync` | `SiiExternalImportProviderClient` | `relative As String`; `correlationId As String`; `mediaType As String`; `cancellationToken As CancellationToken` | `Task(Of Byte())` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` | Agrega autorización y correlación, crea `ExternalImportHttpRequest` y delega en el transporte común. |
| 9 | `Public` | `MapQuery` | `SiiImportContractMapper` | `payload As Byte()`; `request As QueryItemsRequestDto` | `QueryItemsResponseDto` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Convierte el JSON de items SII en DTO comunes, genera claves externas y copia paginación. |
| 10 | `Public` | `MapPreview` | `SiiImportContractMapper` | `payload As Byte()`; `request As GetPreviewRequestDto` | `GetPreviewResponseDto` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Mapea descriptor, tipo, longitud, disposición y expiración del preview. |
| 11 | `Public` | `CreateDocumentCommand` | `SiiImportContractMapper` | `item As ImportItemSelectionDto` | `DocumentCommandDto` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Proyecta el elemento seleccionado al comando documental normalizado. |
| 12 | `Public Shared` | `BuildExternalKey` | `SiiImportContractMapper` | `book As String`; `registration As String`; `matricula As String` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Genera la identidad estable `SII:<libro>:<registro>:<matricula>`. |
| 13 | `Private Shared` | `Segment` | `SiiImportContractMapper` | `value As String` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Normaliza un segmento de la clave, sustituye `:` y rechaza valores vacíos. |
| 14 | `Private Shared` | `Value` | `SiiImportContractMapper` | `source As JObject`; `name As String` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Obtiene de forma segura un valor textual del JSON. |
| 15 | `Private Shared` | `NullableLong` | `SiiImportContractMapper` | `token As JToken` | `Nullable(Of Long)` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Convierte un token a entero largo nullable sin propagar formato inválido. |
| 16 | `Private Shared` | `NullableDate` | `SiiImportContractMapper` | `token As JToken` | `Nullable(Of DateTime)` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb` | Convierte una fecha usando cultura invariante y ajuste UTC. |
| 17 | `Public` | `New` | `SiiImportProvider` | `client As SiiExternalImportProviderClient`; `mapper As SiiImportContractMapper` | Constructor | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Inyecta y valida el cliente SII y el mapper contractual. |
| 18 | `Public` | `ResolveCapabilitiesAsync` | `SiiImportProvider` | `request As ResolveCapabilitiesRequestDto`; `cancellationToken As CancellationToken` | `Task(Of ResolveCapabilitiesResponseDto)` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Informa capacidades `QUERY_ITEMS` y `PREVIEW`, ambas con timeout de 30 segundos. |
| 19 | `Public Async` | `QueryItemsAsync` | `SiiImportProvider` | `request As QueryItemsRequestDto`; `cancellationToken As CancellationToken` | `Task(Of QueryItemsResponseDto)` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Valida proveedor, obtiene bytes del cliente y ejecuta `MapQuery`. |
| 20 | `Public Async` | `GetPreviewAsync` | `SiiImportProvider` | `request As GetPreviewRequestDto`; `cancellationToken As CancellationToken` | `Task(Of GetPreviewResponseDto)` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Valida proveedor, obtiene metadatos del cliente y ejecuta `MapPreview`. |
| 21 | `Public` | `DownloadAsync` | `SiiImportProvider` | `externalKey As String`; `correlationId As String`; `cancellationToken As CancellationToken` | `Task(Of Byte())` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Delega la descarga del recurso en el cliente SII. |
| 22 | `Private Shared` | `ValidateProvider` | `SiiImportProvider` | `request As SolicitudImportacionServicioDto` | `Sub` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Exige que el proveedor sea exactamente `INTEGRACIONSII`, sin fallback. |
| 23 | `Public` | `Translate` | `SiiLegacyResultAdapter` | `result As ImportItemResultDto` | `SiiLegacyResult` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb` | Traduce estados modernos a `YES`, `CTRL`, `CTRLRETURN` o código de error. |
| 24 | `Private Shared` | `BuildLegacyList` | `SiiLegacyResultAdapter` | `result As ImportItemResultDto` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb` | Construye el valor delimitado de `dato_lista` esperado por el consumidor histórico. |
| 25 | `Private Shared` | `Clean` | `SiiLegacyResultAdapter` | `value As String` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb` | Elimina el delimitador `|` y espacios del texto legacy. |
| 26 | `Public` | `New` | `SiiPreviewResponseFactory` | `allowedContentTypes As IEnumerable(Of String)`; `maximumBytes As Long` | Constructor | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb` | Configura tipos permitidos y tamaño máximo; rechaza configuraciones vacías o inválidas. |
| 27 | `Public` | `Create` | `SiiPreviewResponseFactory` | `source As GetPreviewResponseDto`; `request As GetPreviewRequestDto`; `context As ContextoImportacionServicio`; `utcNow As DateTime` | `GetPreviewResponseDto` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb` | Valida el preview y devuelve únicamente metadatos saneados con disposición `inline`. |
| 28 | `Private` | `Validate` | `SiiPreviewResponseFactory` | `source As GetPreviewResponseDto`; `request As GetPreviewRequestDto`; `context As ContextoImportacionServicio`; `utcNow As DateTime` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb` | Verifica tarea, proveedor, clave, expiración, tipo, tamaño, disposición y descriptor. |
| 29 | `Private Shared` | `SafeToken` | `SiiPreviewResponseFactory` | `value As String` | `String` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb` | Limita el descriptor a 128 caracteres alfanuméricos, punto, guion o guion bajo. |
| 30 | `Private Shared` | `Failure` | `SiiPreviewResponseFactory` | `request As GetPreviewRequestDto`; `code As String` | `GetPreviewResponseDto` | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb` | Construye una respuesta de preview fallida sin filtrar detalles internos. |
| 31 | `Public Async` | `ResolveCapabilities` | `WebServiceImportarServicioWebModern` | `request As ResolveCapabilitiesRequestDto` | `Task(Of ResolveCapabilitiesResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | WebMethod de capacidades; valida gate/contexto y delega en el proveedor. |
| 32 | `Public Async` | `QueryItems` | `WebServiceImportarServicioWebModern` | `request As QueryItemsRequestDto` | `Task(Of QueryItemsResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | WebMethod de consulta; devuelve error contractual seguro ante gate, request, contexto, proveedor o dependencia externa. |
| 33 | `Public Async` | `GetPreview` | `WebServiceImportarServicioWebModern` | `request As GetPreviewRequestDto` | `Task(Of GetPreviewResponseDto)` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | WebMethod de preview; vincula la respuesta con el contexto autenticado y aplica la fábrica segura. |
| 34 | `Private Shared` | `FeatureEnabled` | `WebServiceImportarServicioWebModern` | Sin parámetros | `Boolean` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Lee `WorkflowCentroTrabajoModernActive` y solo acepta el valor `true`. |
| 35 | `Private Shared` | `TryBuildImportContext` | `WebServiceImportarServicioWebModern` | `request As SolicitudImportacionServicioDto`; `ByRef context As ContextoImportacionServicio` | `Boolean` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Obtiene contexto autenticado y sesión; valida tarea y trámite confiables y construye el contexto moderno. |
| 36 | `Private Shared` | `ValidRequest` | `WebServiceImportarServicioWebModern` | `request As SolicitudImportacionServicioDto` | `Boolean` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Exige tarea, operación, correlación y proveedor canónico SII. |
| 37 | `Private Shared` | `ResolveProvider` | `WebServiceImportarServicioWebModern` | `providerId As String` | `ResultadoResolucionClienteProveedorImportacion` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Lee configuración, compone transporte/cliente/mapper/proveedor/registro y resuelve la identidad solicitada. |
| 38 | `Private Shared` | `MaximumPreviewBytes` | `WebServiceImportarServicioWebModern` | Sin parámetros | `Long` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Lee `ImportarServicioWebPreviewMaximumBytes`; usa 10 MiB por defecto. |
| 39 | `Private Shared` | `ErrorDto` | `WebServiceImportarServicioWebModern` | `code As String` | `ErrorImportacionServicioDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Construye el error público genérico sin detalle sensible. |
| 40 | `Private Shared` | `DisabledCapabilities` | `WebServiceImportarServicioWebModern` | `request As ResolveCapabilitiesRequestDto` | `ResolveCapabilitiesResponseDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Genera respuesta de capacidades con `FEATURE_DISABLED`. |
| 41 | `Private Shared` | `InvalidCapabilities` | `WebServiceImportarServicioWebModern` | `request As ResolveCapabilitiesRequestDto` | `ResolveCapabilitiesResponseDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Genera respuesta de capacidades con `INVALID_REQUEST`. |
| 42 | `Private Shared` | `ProviderCapabilitiesError` | `WebServiceImportarServicioWebModern` | `request As ResolveCapabilitiesRequestDto`; `result As ResultadoResolucionClienteProveedorImportacion` | `ResolveCapabilitiesResponseDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Proyecta un fallo de resolución del proveedor al contrato de capacidades. |
| 43 | `Private Shared` | `FailureQuery` | `WebServiceImportarServicioWebModern` | `request As QueryItemsRequestDto`; `code As String` | `QueryItemsResponseDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Construye una respuesta fallida de consulta preservando operación y correlación. |
| 44 | `Private Shared` | `FailurePreview` | `WebServiceImportarServicioWebModern` | `request As GetPreviewRequestDto`; `code As String` | `GetPreviewResponseDto` | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | Construye una respuesta fallida de preview preservando operación y correlación. |

## Contrato y modelo agregados

Estos miembros no añaden ejecución independiente fuera de las clases anteriores, pero forman parte del contrato incorporado por DOC-55.

| Tipo | Nombre | Objeto | Parámetros o miembros | Retorno | Ruta dentro del repositorio |
|---|---|---|---|---|---|
| Método de interfaz | `Resolver` | `IRegistroClientesProveedoresImportacion` | `providerId As String` | `ResultadoResolucionClienteProveedorImportacion` | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb` |
| Propiedad calculada | `Encontrado` | `ResultadoResolucionClienteProveedorImportacion` | Sin parámetros; evalúa `Cliente` y `Codigo` | `Boolean` | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb` |
| Propiedad | `Cliente` | `ResultadoResolucionClienteProveedorImportacion` | `IExternalImportProviderClient` | Lectura/escritura | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb` |
| Propiedad | `Codigo` | `ResultadoResolucionClienteProveedorImportacion` | `String` | Lectura/escritura | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb` |
| Propiedad | `MensajeVisible` | `ResultadoResolucionClienteProveedorImportacion` | `String` | Lectura/escritura | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb` |
| Propiedad | `Code` | `SiiLegacyResult` | `String` | Lectura/escritura | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb` |
| Propiedad | `dato_lista` | `SiiLegacyResult` | `String` | Lectura/escritura | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb` |

## Resumen de cobertura

| Categoría | Cantidad |
|---|---:|
| WebMethods públicos | 3 |
| Constructores | 4 |
| Métodos públicos de servicios/adaptadores | 14 |
| Helpers privados | 23 |
| Miembros ejecutables inventariados | 44 |
| Método de interfaz adicional | 1 |
| Propiedades del modelo nuevo | 5 |

## Exclusiones deliberadas

No se atribuyen a DOC-55 las operaciones `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` ni `ReconcileImportIntent` que aparecen en la versión actual del ASMX: fueron incorporadas o conectadas después del corte de DOC-55. Tampoco se listan funciones de `ClassAlmacenamiento`, ASMX históricos, transporte HTTP B02 u orquestación B01-B05 porque DOC-55 los reutilizó o verificó sin implementarlos.

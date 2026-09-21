# Inventario técnico verificado

## Repositorio y módulos frontend

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Módulo JS | `js/workflow/importar-servicio-web/importar-servicio-web-api.js` | `ImportarServicioWebApi` | `create` | `options: Object` | cliente dinámico | Crea cliente para ocho operaciones | transporte inyectado o `window.fetch` |
| Función JS | misma | módulo privado | `invoke` | `operation: String`, `request: Object` | `Promise<Object>` | POST y desempaque ASMX | `unwrapAsmx` |
| Función JS | misma | módulo privado | `unwrapAsmx` | `raw: Object` | `Object` | exige propiedad `d`, admite JSON string | `JSON.parse` |
| Módulo JS | `js/workflow/importar-servicio-web/importar-servicio-web-provider-registry.js` | `ImportarServicioWebProviderRegistry` | `create` | `options: Object` | registro | crea mapas de adaptadores/no migrados | `normalizeProviderId` |
| Función JS | misma | registro retornado | `register` | `providerId: any`, `adapter: Object` | registro | almacena identidad y copia capacidades | `copyCapabilities` |
| Función JS | misma | registro retornado | `resolve` | `providerId: any` | resultado de resolución | resuelve o falla cerrado | mapas internos |
| Módulo JS | `js/workflow/importar-servicio-web/importar-servicio-web-core.js` | `ImportarServicioWebCore` | `create` | `options.registry: Object` | core | valida registro e inicializa estado | registro |
| Función JS | misma | core retornado | `open` | `providerId: String` | `Promise<Snapshot>` | resuelve y consulta | `registry.resolve`, `query` |
| Función JS | misma | core retornado | `query` | `request: Object` | `Promise<Snapshot>` | consulta y decide vacío/resultados/error | `adapter.queryItems` |
| Función JS | misma | core retornado | `execute` | `request: Object` | `Promise<Snapshot>` | reutiliza una promesa por instancia | `adapter.executeImportIntent` |
| Función JS | misma | core retornado | `transition` | `next: String`, `update: Object` | `Snapshot` | valida tabla de transiciones | `transitions` |
| Módulo JS | `js/workflow/importar-servicio-web/importar-servicio-web-ui.js` | `ImportarServicioWebUi` | `initialize` | `options: Object` | `Control|null` | enlaza DOM, API, registro y core | tres módulos anteriores |
| Función JS | misma | módulo privado/exportado | `createBackendAdapter` | `api: Object`, `control: Control` | adaptador | une contexto visual con API | `resolveCapabilities`, `queryItems`, `executeImportIntent` |
| Función JS | misma | módulo privado/exportado | `open` | `control: Control`, `event: Event` | `false` | abre modal, enfoca y llama core | `core.open` |
| Función JS | misma | módulo privado/exportado | `close` | `control: Control` | `undefined` | cierra, limpia core y restaura foco | `core.close` |
| Función JS | misma | módulo privado/exportado | `onKeydown` | `control: Control`, `event: KeyboardEvent` | `undefined` | Escape y ciclo de foco | DOM |

## Integración WebForms

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Página | `workflow/Webworkflow.aspx` | `Webworkflow` | `importar-servicio-web-modal` | No aplica | HTML | diálogo, estado y lista | UI JS/CSS |
| Método VB | `workflow/Webworkflow.aspx.vb` | `Webworkflow` | `RegisterImportarServicioWebModernAssets` | ninguno | `System.Void` | registra CSS y cuatro scripts | `Page.Header` |
| Método VB | misma | `Webworkflow` | `RegisterImportarServicioWebScript` | `controlId As String`, `source As String` | `System.Void` | agrega script si no existe | `HtmlGenericControl` |
| Método VB | misma | `Webworkflow` | `RegisterImportarServicioWebModernBootstrap` | ninguno | `System.Void` | escribe atributos de tarea/proveedor y llama `initialize` | `ScriptManager` |
| Configuración | `Web.config` | appSettings | `ImportarServicioWebProviderId` | `String` | No aplica | proveedor para bootstrap; vacío por defecto | registro frontend y `ValidRequest` backend |

## Endpoints ASMX

Todos usan HTTP `POST`, ruta base `/webservice/WebServiceImportarServicioWebModern.asmx/`, sesión ASP.NET habilitada y autorización efectiva por gate + contexto de sesión/tarea. No se observó autorización declarativa por atributo adicional.

| Verbo y ruta | Clase | Método exacto | DTO entrada | DTO salida | Validaciones directas |
| --- | --- | --- | --- | --- | --- |
| POST `/ResolveCapabilities` | `WebServiceImportarServicioWebModern` | `ResolveCapabilities(request As ResolveCapabilitiesRequestDto)` | `ResolveCapabilitiesRequestDto` | `ResolveCapabilitiesResponseDto` | gate, `ValidRequest`, contexto, proveedor |
| POST `/QueryItems` | misma | `QueryItems(request As QueryItemsRequestDto)` | `QueryItemsRequestDto` | `QueryItemsResponseDto` | anteriores + `CodigoBarras` no vacío |
| POST `/GetPreview` | misma | `GetPreview(request As GetPreviewRequestDto)` | `GetPreviewRequestDto` | `GetPreviewResponseDto` | anteriores + `ExternalKey` no vacío |
| POST `/PreflightImport` | misma | `PreflightImport(request As PreflightImportRequestDto)` | `PreflightImportRequestDto` | `PreflightImportResponseDto` | gate, solicitud, contexto |
| POST `/CreateImportIntent` | misma | `CreateImportIntent(request As CreateImportIntentRequestDto)` | `CreateImportIntentRequestDto` | `CreateImportIntentResponseDto` | gate, solicitud, contexto; servicio valida intención |
| POST `/ExecuteImportIntent` | misma | `ExecuteImportIntent(request As ExecuteImportIntentRequestDto)` | `ExecuteImportIntentRequestDto` | `ExecuteImportIntentResponseDto` | gate, solicitud, contexto; orquestador ejecuta |
| POST `/GetImportIntent` | misma | `GetImportIntent(request As GetImportIntentRequestDto)` | `GetImportIntentRequestDto` | `GetImportIntentResponseDto` | gate, solicitud, contexto; reconciliación consulta |
| POST `/ReconcileImportIntent` | misma | `ReconcileImportIntent(request As ReconcileImportIntentRequestDto)` | `ReconcileImportIntentRequestDto` | `ReconcileImportIntentResponseDto` | gate, solicitud, contexto; reconciliación valida |

## Interfaces y composición directamente observadas

| Tipo | Ruta | Interfaz/clase | Métodos relevantes | Implementación/relación observada |
| --- | --- | --- | --- | --- |
| Interfaz | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb` | `IExternalImportProviderClient` | `ResolveCapabilitiesAsync`, `QueryItemsAsync`, `GetPreviewAsync`, `DownloadAsync` | `SiiImportProvider`/cliente compuesto por endpoint |
| Interfaz | misma | `IRegistroClientesProveedoresImportacion` | `Resolver(providerId As String)` | `RegistroClientesProveedoresImportacion` |
| Interfaz | misma | `IImportIntentRepository` | obtener, crear/reutilizar, persistir plan, transición | `MySqlImportIntentRepository` en `Compose` |
| Interfaz | misma | `IImportReconciliationRepository` | `Obtener`, `ObtenerItem` | `MySqlImportReconciliationRepository` en composición |
| Servicio | `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb` | `ImportServiceOrchestrator` | `Execute(contexto, request)` | llamado por endpoint de ejecución |
| Servicio | `Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb` | `ServicioPreflightImportacion` | `Preflight(contexto, request)` | llamado por endpoint preflight |
| Servicio | `Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb` | `ServicioIntencionImportacion` | `Crear(contexto, request)` | llamado por endpoint de creación |
| Servicio | `Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb` | `ServicioReconciliacionImportacion` | consulta, reconciliación y proyección | endpoints execute/get/reconcile |

## DTOs y propiedades

Todas las solicitudes heredan `SolicitudImportacionServicioDto`; todas las respuestas heredan `RespuestaImportacionServicioDto` con `SchemaVersion`, `OperationId`, `CorrelationId` y `Error`. “Obligatorio” refleja validación observada, no anotaciones de nulabilidad (VB.NET legacy no las declara).

| DTO | Propiedades declaradas | Obligatoriedad/validación observada |
| --- | --- | --- |
| `SolicitudImportacionServicioDto` | `SchemaVersion:String`, `OperationId:String`, `CorrelationId:String`, `TaskId:Long`, `ProviderId:String` | `TaskId>0`, IDs no vacíos, proveedor canónico SII; schema se inicializa `1.0` |
| `ResolveCapabilitiesRequestDto` | solo propiedades base | base obligatoria |
| `ResolveCapabilitiesResponseDto` | `ProviderId:String`, `ContextAllowed:Boolean`, `Capabilities:IList<ProviderCapabilityDto>`, `DocumentTypes:IList<ImportDocumentTypeDto>` | listas inicializadas; error posible |
| `QueryItemsRequestDto` | `CodigoBarras:String`, `ContinuationToken:String`, `PageSize:Nullable<Integer>` | código obligatorio en endpoint; token/tamaño opcionales |
| `QueryItemsResponseDto` | `Items:IList<ExternalItemDto>`, `ContinuationToken:String`, `ProviderResultCode:String`, `InscriptionCount:Integer`, `ImageCount:Integer` | `Items` inicializada |
| `GetPreviewRequestDto` | `ExternalKey:String` | external key obligatorio |
| `GetPreviewResponseDto` | `ExternalKey`, `DescriptorId`, `ContentType`, `Length?`, `Disposition`, `ExpiresAtUtc?` | descriptor mediado; campos dependen de éxito |
| `PreflightImportRequestDto` | `Items:IList<ImportItemSelectionDto>` | lista inicializada; reglas en servicio preflight |
| `PreflightImportResponseDto` | `IsValid`, `Requirements`, `Commands`, `ContextFingerprint`, `Executable`, `EffectPlans` | colecciones inicializadas |
| `CreateImportIntentRequestDto` | `IdempotencyKey`, `Items`, `Requirements`, `ContextFingerprint`, `Radicado` | colecciones inicializadas; reglas en servicio de intención |
| `CreateImportIntentResponseDto` | `IntentId`, `Status`, `VersionToken`, `Reused` | valores según persistencia/error |
| `ExecuteImportIntentRequestDto` | `IntentId`, `VersionToken`, `StopRequested` | reglas en orquestador/repositorio |
| `ExecuteImportIntentResponseDto` | `IntentId`, `Accepted`, `Status`, `VersionToken`, `Items`, `ExpedientEffects` | colecciones inicializadas |
| `GetImportIntentRequestDto` | `IntentId` | validación en reconciliación |
| `GetImportIntentResponseDto` | `IntentId`, `Status`, `VersionToken`, `Items`, `ExpedientEffects` | colecciones inicializadas |
| `ReconcileImportIntentRequestDto` | `IntentId`, `ExternalKey` | validación en reconciliación |
| `ReconcileImportIntentResponseDto` | `IntentId`, `Status`, `VersionToken`, `ConfirmedDocumentCount`, `Items`, `ExpedientEffects` | colecciones inicializadas |

El detalle de DTOs subordinados (`ExternalItemDto`, selección, requirements, commands, resultados y efectos) está implementado en el mismo archivo, pero no todos son construidos o interpretados por la UI DOC-72. Se consideran contrato backend ya existente, no propiedad nueva de esta entrega.

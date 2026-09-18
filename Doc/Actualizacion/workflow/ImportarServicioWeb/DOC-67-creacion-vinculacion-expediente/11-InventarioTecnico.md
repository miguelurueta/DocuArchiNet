# Inventario técnico verificado

## Endpoints ASMX

| Tipo | Ruta del archivo | Clase | Nombre exacto | Parámetros y tipos | Retorno | Autorización/Descripción | Dependencias |
|---|---|---|---|---|---|---|---|
| POST ASMX | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | `WebServiceImportarServicioWebModern` | `/webservice/WebServiceImportarServicioWebModern.asmx/ExecuteImportIntent` | `request As ExecuteImportIntentRequestDto` | `ExecuteImportIntentResponseDto` | Sesión Workflow, gate, tarea/contexto y versión | `ImportServiceOrchestrator.Execute` |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/ResolveCapabilities` | `request As ResolveCapabilitiesRequestDto` | `ResolveCapabilitiesResponseDto` | Sesión Workflow y gate | composición/capacidades |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/QueryItems` | `request As QueryItemsRequestDto` | `QueryItemsResponseDto` | Sesión, gate, tarea/contexto | proveedor SII |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/GetPreview` | `request As GetPreviewRequestDto` | `GetPreviewResponseDto` | Sesión, gate, clave externa | proveedor SII |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/PreflightImport` | `request As PreflightImportRequestDto` | `PreflightImportResponseDto` | Sesión, gate, tarea y tipología | `ServicioPreflightImportacion` |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/CreateImportIntent` | `request As CreateImportIntentRequestDto` | `CreateImportIntentResponseDto` | Sesión, gate, selección y preflight | `ServicioIntencionImportacion.Crear` |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/GetImportIntent` | `request As GetImportIntentRequestDto` | `GetImportIntentResponseDto` | Sesión, gate e intención de la tarea | repositorio/mapper |
| POST ASMX | mismo | misma | `/webservice/WebServiceImportarServicioWebModern.asmx/ReconcileImportIntent` | `request As ReconcileImportIntentRequestDto` | `ReconcileImportIntentResponseDto` | Sesión, gate e intención autorizada | reconciliación |

## Servicios, repositorios y puertos principales

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones |
|---|---|---|---|---|---|---|---|
| Servicio | `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb` | `ImportServiceOrchestrator` | `Execute` | `ContextoImportacionServicio, ExecuteImportIntentRequestDto` | `ExecuteImportIntentResponseDto` | Saga secuencial | estado, expediente, storage, relacionados |
| Servicio | `Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb` | `ImportExpedientCoordinator` | `Resolver` | `ContextoImportacionServicio, IntencionImportacionServicio` | `PlanExpedienteImportacion` | Resuelve/crea destinos | configuración, sujeto, caché, repo físico |
| Servicio | `Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb` | `ImportRelatedDocumentCoordinator` | `Procesar` | contexto, intención, plan, gabinete, radicado | `PlanDocumentosRelacionadosImportacion` | Confirma universo `ENLASE` | relación, caché, índices |
| Repositorio | `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb` | `MySqlImportIntentRepository` | `CrearOReutilizar` | `ContextoImportacionServicio, IntencionImportacionServicio` | `ResultadoPersistenciaIntencionImportacion` | Persistencia idempotente | `IImportIntentRepository` |
| Repositorio | mismo | misma | `PersistirPlanExpedientes` | contexto, intención, plan | `Boolean` | Guarda plan lógico | tablas Workflow DOC-67 |
| Repositorio | `Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb` | `PhysicalImportExpedientRepository` | `Buscar/Crear/Verificar` | contexto, configuración, inscripción/sujeto | `ResultadoEfectoExpedienteImportacion` | Pre/postcheck físico | normalizador y gateway |
| Adaptador | `Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb` | `DocumentExpedientRelationAdapter` | `Consultar/Vincular` | contexto, documento, expediente | `ResultadoEfectoExpedienteImportacion` | Relación con postcheck | gateway físico |
| Adaptador | `Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb` | `SiiDocumentIndexAdapter` | `Actualizar/Verificar` | contexto, documento | `ResultadoEfectoExpedienteImportacion` | Índices dinámicos SQL/XML | gateway de índices |

Interfaces implementadas incluidas: `IImportIntentRepository`, `IImportReconciliationRepository`, `IImportExpedientConfigurationRepository`, `ISiiExpedientSubjectResolver`, `IImportExpedientRepository`, `IImportExpedientCacheRepository`, `IImportRelatedDocumentRepository`, `IImportDocumentExpedientRelationPort`, `IImportDocumentLinkCacheRepository`, `IImportDocumentIndexUpdater` e `IImportElectronicIndexVerifier`, declaradas en `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`.

## DTOs de frontera

| DTO | Propiedades relevantes | Tipos | Obligatoriedad/validación |
|---|---|---|---|
| `ExecuteImportIntentRequestDto` | `TaskId`, `ProviderId`, `IntentId`, `VersionToken`, `StopRequested`, correlación | `Long`, `String`, `Boolean` | tarea positiva; proveedor SII; ids/correlación no vacíos |
| `CreateImportIntentRequestDto` | tarea, proveedor, radicado, selección y tipología | escalares + colección | preflight válido; radicado/selección requeridos |
| `ExecuteImportIntentResponseDto` | estado, versión, items, efectos y error | DTOs/colecciones | solo proyecta documento disponible con efectos confirmados |
| `ImportItemResultDto` | identidad externa, fase, documento, estados, error seguro | escalares/nullable | `DocumentId` solo tras persistencia confirmada |
| `ErrorImportacionServicioDto` | `Codigo`, diagnóstico seguro | `String` | sin SQL, secretos ni excepción cruda |

El detalle exhaustivo de propiedades permanece en `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`; la prueba estructural controla los DTOs referenciados por los diagramas. Campos no aplicables a una fila se indican como “No aplica”. Los errores funcionales viajan en el DTO (normalmente HTTP 200); 400/401/403 no son un contrato propio implementado por estos métodos ASMX y 500 queda reservado al fallo no manejado del host.

## Pendientes no declarados como implementados

- No está verificado el contrato remoto que identifica múltiples expedientes afectados por un mismo código de barras; se conserva como pendiente externo.
- La prueba documental no interpreta semánticamente cuerpos VB ni demuestra que todos los caminos de ejecución coincidan con el diagrama.

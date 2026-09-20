Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks

' Puertos del núcleo. Sus implementaciones futuras no pertenecen al modelo.
Public Interface IExternalImportProvider
    ReadOnly Property ProviderId As String
    Function ResolverCapacidades(ByVal contexto As ContextoImportacionServicio) As IList(Of CapacidadProveedorImportacion)
    Function ConsultarElementos(ByVal contexto As ContextoImportacionServicio,
                               ByVal continuationToken As String,
                               ByVal pageSize As Nullable(Of Integer)) As IList(Of ElementoExternoImportacion)
End Interface

' Puerto moderno aditivo. Los adaptadores concretos de proveedor se implementan en entregas posteriores.
Public Interface IExternalImportProviderClient
    ReadOnly Property ProviderId As String
    Function ResolveCapabilitiesAsync(ByVal request As ResolveCapabilitiesRequestDto,
                                      ByVal cancellationToken As CancellationToken) As Task(Of ResolveCapabilitiesResponseDto)
    Function QueryItemsAsync(ByVal request As QueryItemsRequestDto,
                             ByVal cancellationToken As CancellationToken) As Task(Of QueryItemsResponseDto)
    Function GetPreviewAsync(ByVal request As GetPreviewRequestDto,
                             ByVal cancellationToken As CancellationToken) As Task(Of GetPreviewResponseDto)
    Function DownloadAsync(ByVal externalKey As String,
                           ByVal correlationId As String,
                           ByVal cancellationToken As CancellationToken,
                           Optional ByVal intentId As String = Nothing,
                           Optional ByVal clientItemId As String = Nothing,
                           Optional ByVal operationId As String = Nothing,
                           Optional ByVal taskId As Nullable(Of Long) = Nothing,
                           Optional ByVal radicado As String = Nothing,
                           Optional ByVal referenciaProveedor As String = Nothing) As Task(Of Byte())
End Interface

Public Interface IRegistroClientesProveedoresImportacion
    Function Resolver(ByVal providerId As String) As ResultadoResolucionClienteProveedorImportacion
End Interface

Public Interface IRegistroProveedoresImportacion
    Function Resolver(ByVal providerId As String) As ResultadoResolucionProveedorImportacion
End Interface

Public Interface IAutorizacionImportacionRepository
    Function UsuarioAutenticado(ByVal contexto As ContextoImportacionServicio) As Boolean
    Function PermisoVigente(ByVal contexto As ContextoImportacionServicio) As Boolean
    Function TareaOperable(ByVal contexto As ContextoImportacionServicio) As Boolean
    Function RutaCoincide(ByVal contexto As ContextoImportacionServicio) As Boolean
    Function TramiteCoincide(ByVal contexto As ContextoImportacionServicio) As Boolean
    Function ProveedorHabilitado(ByVal contexto As ContextoImportacionServicio) As Boolean
End Interface

Public Interface IImportacionServicioClock
    Function UtcNow() As DateTime
End Interface

' Reservado para entregas posteriores; DOC-50 no aporta implementación ni lo usa para persistir.
Public Interface IImportIntentRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio, ByVal intentId As String) As IntencionImportacionServicio
    Function ObtenerPorIdempotencia(ByVal contexto As ContextoImportacionServicio,
                                    ByVal idempotencyKey As String) As IntencionImportacionServicio
    Function CrearOReutilizar(ByVal contexto As ContextoImportacionServicio,
                              ByVal intencion As IntencionImportacionServicio) As ResultadoPersistenciaIntencionImportacion
    Function PersistirPlanExpedientes(ByVal contexto As ContextoImportacionServicio,
                                      ByVal intencion As IntencionImportacionServicio,
                                      ByVal plan As PlanExpedienteImportacion) As Boolean
    Function ActualizarTransicion(ByVal contexto As ContextoImportacionServicio,
                                  ByVal transicion As TransicionImportacion,
                                  ByVal resultado As ResultadoElementoImportacion) As Boolean
End Interface

Public Interface IImportReconciliationRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio,
                     ByVal intentId As String) As SnapshotReconciliacionImportacion
    Function ObtenerItem(ByVal contexto As ContextoImportacionServicio,
                         ByVal intentId As String,
                         ByVal providerId As String,
                         ByVal externalKey As String) As SnapshotReconciliacionImportacion
End Interface

Public Interface IImportIntentTransitionAudit
    Sub Registrar(ByVal transicion As TransicionImportacion, ByVal aceptada As Boolean, ByVal codigo As String)
End Interface

Public Interface IImportExecutionStep
    ReadOnly Property FaseConfirmada As FaseImportacionServicio
    Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                      ByVal intencion As IntencionImportacionServicio,
                      ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion
End Interface

' Reconstruye el agregado desde la respuesta SII autoritativa; el cliente solo aporta identidades seleccionadas.
Public Interface IImportInscriptionResolver
    Function Resolver(ByVal contexto As ContextoImportacionServicio,
                      ByVal solicitud As CreateImportIntentRequestDto,
                      ByVal items As IList(Of ResultadoElementoImportacion)) As IList(Of InscripcionImportacion)
End Interface

Public Interface IImportStorageMetadataRepository
    Function Resolver(ByVal contexto As ContextoImportacionServicio) As MetadatosAlmacenamientoImportacion
End Interface

' Traduce la identidad TRD pública a la identidad contextual exigida por el almacenamiento legacy.
Public Interface IImportDocumentTypeResolver
    Function Resolver(ByVal contexto As ContextoImportacionServicio,
                      ByVal idTipoDocumentalTrd As Integer,
                      ByVal nombreTipoDocumental As String) As ResolucionTipoDocumentalImportacion
End Interface

Public Interface IImportDocumentTypeCatalogRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio) As IList(Of TipoDocumentalCatalogoImportacion)
End Interface

Public Interface IImportItemStatusRepository
    Function ObtenerLote(ByVal contexto As ContextoImportacionServicio,
                         ByVal providerId As String,
                         ByVal externalKeys As IList(Of String)) As IDictionary(Of String, EstadoItemListadoImportacion)
End Interface

Public Interface IImportIntentConcurrencyGuard
    Function Adquirir(ByVal contexto As ContextoImportacionServicio,
                      ByVal idempotencyKey As String) As ResultadoGuardIntencionImportacion
End Interface

Public Interface IExternalServiceAttemptRecorder
    Sub Registrar(ByVal intento As IntentoServicioExterno)
End Interface

Public Interface IExternalServiceAvailabilityRepository
    Function Consultar(ByVal providerId As String,
                       ByVal operation As String,
                       ByVal desdeUtc As DateTime,
                       ByVal hastaUtc As DateTime) As DisponibilidadServicioExterno
End Interface

Public Interface IImportExpedientConfigurationRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio) As ConfiguracionExpedienteImportacion
End Interface

Public Interface IImportEffectConfigurationRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio) As ImportEffectConfiguration
End Interface

Public Interface ISiiExpedientSubjectResolver
    Function Resolver(ByVal contexto As ContextoImportacionServicio,
                      ByVal inscripcion As InscripcionImportacion,
                      ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportExpedientIdentityNormalizer
    Function Normalizar(ByVal contexto As ContextoImportacionServicio,
                        ByVal nombreGabinete As String,
                        ByVal matricula As String,
                        ByVal proponente As String) As IdentidadExpedienteNormalizada
End Interface

Public Interface IImportExpedientRepository
    Function Buscar(ByVal contexto As ContextoImportacionServicio,
                    ByVal inscripcion As InscripcionImportacion,
                    ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
    Function Crear(ByVal contexto As ContextoImportacionServicio,
                   ByVal inscripcion As InscripcionImportacion,
                   ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
    Function Verificar(ByVal contexto As ContextoImportacionServicio,
                       ByVal idExpediente As Long,
                       ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportExpedientCacheRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio,
                     ByVal inscripcion As InscripcionImportacion) As InscripcionImportacion
    Function RegistrarVerificado(ByVal contexto As ContextoImportacionServicio,
                                 ByVal inscripcion As InscripcionImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportRelatedDocumentRepository
    Function ObtenerPorEnlace(ByVal contexto As ContextoImportacionServicio,
                              ByVal nombreGabinete As String,
                              ByVal radicadoSii As String) As IList(Of DocumentoRelacionadoImportacion)
    Function Persistir(ByVal contexto As ContextoImportacionServicio,
                       ByVal intentId As String,
                       ByVal documento As DocumentoRelacionadoImportacion) As Boolean
End Interface

Public Interface IImportDocumentExpedientRelationPort
    Function Consultar(ByVal contexto As ContextoImportacionServicio,
                       ByVal documento As DocumentoRelacionadoImportacion) As ResultadoEfectoExpedienteImportacion
    Function Vincular(ByVal contexto As ContextoImportacionServicio,
                      ByVal documento As DocumentoRelacionadoImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportDocumentLinkCacheRepository
    Function Obtener(ByVal contexto As ContextoImportacionServicio,
                     ByVal idImagen As Long,
                     ByVal nombreGabinete As String) As EntradaCacheVinculoDocumentoImportacion
    Function RegistrarVerificado(ByVal contexto As ContextoImportacionServicio,
                                 ByVal entrada As EntradaCacheVinculoDocumentoImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportDocumentIndexUpdater
    Function Actualizar(ByVal contexto As ContextoImportacionServicio,
                        ByVal documento As DocumentoRelacionadoImportacion,
                        ByVal inscripcion As InscripcionImportacion) As ResultadoEfectoExpedienteImportacion
End Interface

Public Interface IImportElectronicIndexVerifier
    Function Verificar(ByVal contexto As ContextoImportacionServicio,
                       ByVal documento As DocumentoRelacionadoImportacion) As EvidenciaIndiceElectronicoImportacion
End Interface

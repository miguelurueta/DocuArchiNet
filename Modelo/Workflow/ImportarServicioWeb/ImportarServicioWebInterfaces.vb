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
                           ByVal cancellationToken As CancellationToken) As Task(Of Byte())
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

Public Interface IImportIntentConcurrencyGuard
    Function Adquirir(ByVal contexto As ContextoImportacionServicio,
                      ByVal idempotencyKey As String) As ResultadoGuardIntencionImportacion
End Interface

Imports System
Imports System.Collections.Generic

' Puertos del núcleo. Sus implementaciones futuras no pertenecen al modelo.
Public Interface IExternalImportProvider
    ReadOnly Property ProviderId As String
    Function ResolverCapacidades(ByVal contexto As ContextoImportacionServicio) As IList(Of CapacidadProveedorImportacion)
    Function ConsultarElementos(ByVal contexto As ContextoImportacionServicio,
                               ByVal continuationToken As String,
                               ByVal pageSize As Nullable(Of Integer)) As IList(Of ElementoExternoImportacion)
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
    Function Obtener(ByVal contexto As ContextoImportacionServicio,
                     ByVal intentId As String) As IntencionImportacionServicio
    Function Guardar(ByVal contexto As ContextoImportacionServicio,
                     ByVal intencion As IntencionImportacionServicio) As Boolean
End Interface

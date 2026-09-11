Imports System
Imports System.Collections.Generic

Public NotInheritable Class ContextoIntencionImportacion
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property IdUsuario As Integer
    Public Property IdGrupo As Integer
    Public Property LoginUsuario As String
    Public Property IdTarea As Long
    Public Property IdRuta As Integer
    Public Property IdTramite As Integer
    Public Property ProviderId As String
    Public Property Radicado As String
End Class

Public NotInheritable Class MetadatosAlmacenamientoImportacion
    Public Property NombreRutaWorkflow As String
    Public Property NombreGabinete As String
    Public Property NombreClaseFormatoDocumento As String
End Class

Public NotInheritable Class ResolucionTipoDocumentalImportacion
    Public Property Valida As Boolean
    Public Property Codigo As String
    Public Property IdTipoListaChequeo As Integer
    Public Property IdTipoDocumentalTrd As Integer
    Public Property NombreTipoDocumental As String
End Class

' Modelos internos del núcleo de importación. No conocen DTOs ni infraestructura web.
Public Class CapacidadProveedorImportacion
    Public Property Codigo As String
    Public Property Habilitada As Boolean
    Public Property TimeoutSeconds As Nullable(Of Integer)
End Class

Public Class ProveedorImportacion
    Public Property IdentidadCanonica As String
    Public Property Habilitado As Boolean
    Public Property Capacidades As IList(Of CapacidadProveedorImportacion)

    Public Sub New()
        Capacidades = New List(Of CapacidadProveedorImportacion)()
    End Sub
End Class

Public Class IdentidadExternaImportacion
    Public Property ProviderId As String
    Public Property ExternalKey As String
End Class

Public Class ElementoExternoImportacion
    Public Property Identidad As IdentidadExternaImportacion
    Public Property NombreVisible As String
    Public Property TipoContenido As String
    Public Property Longitud As Nullable(Of Long)
    Public Property PermitePreview As Boolean
End Class

Public Enum FaseImportacionServicio
    Creada
    Validada
    RecursoObtenido
    ExpedientePreparado
    DocumentoAlmacenado
    IndicesActualizados
    CacheActualizado
    ResultadoIncierto
    RequiereDecision
    Reconciliada
    Completada
    FallidaAntesDePersistir
    Parcial
    Detenida
    Omitida
End Enum

Public Class RequisitoPlanImportacion
    Public Property Codigo As String
    Public Property Satisfecho As Boolean
    Public Property MensajeVisible As String
End Class

Public Class ComandoDocumentoImportacion
    Public Property ClientItemId As String
    Public Property IdentidadExterna As IdentidadExternaImportacion
    ' Identidad canónica de tipo_doc_series; nunca es ID_TIPO_DOCUMENTAL_CHEQUEO.
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property NombreTipoDocumental As String
    Public Property NombreArchivo As String
    Public Property TipoContenido As String
End Class

Public Class PlanImportacionServicio
    Public Sub New()
        Requisitos = New List(Of RequisitoPlanImportacion)()
        Comandos = New List(Of ComandoDocumentoImportacion)()
    End Sub

    Public Property Valido As Boolean
    Public Property Requisitos As IList(Of RequisitoPlanImportacion)
    Public Property Comandos As IList(Of ComandoDocumentoImportacion)
End Class

Public Class ResultadoElementoImportacion
    Public Property ClientItemId As String
    Public Property IdentidadExterna As IdentidadExternaImportacion
    Public Property IdTareaDestino As Long
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property NombreTipoDocumental As String
    Public Property NombreArchivo As String
    Public Property TipoContenido As String
    Public Property Fase As FaseImportacionServicio
    Public Property IdDocumento As Nullable(Of Long)
    Public Property CodigoError As String
    Public Property MensajeVisible As String
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property CorrelationId As String
    ' Estado efímero de una ejecución. Nunca se serializa ni se persiste en el repositorio.
    Public Property ContenidoDescargado As Byte()
    Public Property RutaArchivoPreparado As String
    Public Property MetadatosSii As MetadatosDocumentoSii
End Class

Public NotInheritable Class MetadatosDocumentoSii
    Public Property Libro As String
    Public Property Registro As String
    Public Property Duplicado As String
    Public Property Fecha As String
    Public Property Hora As String
    Public Property UsuarioSii As String
    Public Property Acto As String
    Public Property NombreActo As String
    Public Property Matricula As String
    Public Property Proponente As String
    Public Property RazonSocial As String
    Public Property NitCedula As String
    Public Property IdAnexo As String
    Public Property TipoImagen As String
    Public Property TipoAnexo As String
    Public Property TipoSirep As String
    Public Property TipoDigitalizacion As String
    Public Property IdentificadorImagen As String
    Public Property Formato As String
    Public Property FechaDocumento As String
    Public Property Origen As String
    Public Property Observaciones As String
End Class

Public NotInheritable Class IntentoServicioExterno
    Public Property ProviderId As String
    Public Property Operacion As String
    Public Property Exitoso As Boolean
    Public Property CodigoError As String
    Public Property CategoriaError As String
    Public Property CodigoDependencia As String
    Public Property MensajeDiagnostico As String
    Public Property EstadoHttp As Nullable(Of Integer)
    Public Property Reintentable As Boolean
    Public Property TaskId As Nullable(Of Long)
    Public Property Radicado As String
    Public Property CodigoBarras As String
    Public Property ReferenciaProveedor As String
    Public Property IntentId As String
    Public Property ClientItemId As String
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property FechaInicioUtc As DateTime
    Public Property FechaFinUtc As DateTime
    Public Property DuracionMs As Long
End Class

Public NotInheritable Class DisponibilidadServicioExterno
    Public Property ProviderId As String
    Public Property Operacion As String
    Public Property DesdeUtc As DateTime
    Public Property HastaUtc As DateTime
    Public Property Total As Long
    Public Property Exitosos As Long
    Public Property Fallidos As Long
    Public Property DisponibilidadPorcentaje As Decimal
    Public Property LatenciaPromedioMs As Decimal
End Class

Public Enum ConsistenciaDocumentoImportacion
    NoConfirmado
    Confirmado
    RelacionAusente
    RelacionDuplicada
    TareaDistinta
    ResultadoIncierto
End Enum

' Instantánea read-only; representa evidencia persistida sin conocer capas de transporte o datos.
Public Class SnapshotReconciliacionImportacion
    Public Sub New()
        Items = New List(Of SnapshotItemReconciliacionImportacion)()
    End Sub
    Public Property IntentId As String
    Public Property VersionToken As String
    Public Property Fase As FaseImportacionServicio
    Public Property IdUsuario As Integer
    Public Property IdTareaOriginal As Long
    Public Property ProviderId As String
    Public Property Items As IList(Of SnapshotItemReconciliacionImportacion)
End Class

Public Class SnapshotItemReconciliacionImportacion
    Public Property ClientItemId As String
    Public Property ProviderId As String
    Public Property ExternalKey As String
    Public Property IdTareaDestino As Long
    Public Property IdDocumento As Nullable(Of Long)
    Public Property NombreDocumento As String
    Public Property TipoContenido As String
    Public Property Fase As FaseImportacionServicio
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property CodigoError As String
    Public Property MensajeVisible As String
    Public Property CorrelationId As String
    Public Property CantidadDocumentos As Integer
    Public Property CantidadRelaciones As Integer
    Public Property CantidadRelacionesOtraTarea As Integer
End Class

Public Class ResultadoFaseImportacion
    Public Property Exitoso As Boolean
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property IdDocumento As Nullable(Of Long)
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class TransicionImportacion
    Public Property IntentId As String
    Public Property ClientItemId As String
    Public Property FaseAnterior As FaseImportacionServicio
    Public Property FaseNueva As FaseImportacionServicio
    Public Property VersionAnterior As String
    Public Property VersionNueva As String
    Public Property FechaUtc As DateTime
    Public Property CorrelationId As String
    Public Property Codigo As String
End Class

Public Class ResultadoTransicionImportacion
    Public Property Aceptada As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property Transicion As TransicionImportacion
End Class

Public Class IntencionImportacionServicio
    Public Sub New()
        Resultados = New List(Of ResultadoElementoImportacion)()
        Requisitos = New List(Of RequisitoPlanImportacion)()
    End Sub

    Public Property Id As String
    Public Property IdempotencyKey As String
    Public Property HuellaContexto As String
    Public Property ContextoOriginal As ContextoIntencionImportacion
    Public Property VersionToken As String
    Public Property Fase As FaseImportacionServicio
    Public Property FechaCreacionUtc As DateTime
    Public Property FechaActualizacionUtc As DateTime
    Public Property Requisitos As IList(Of RequisitoPlanImportacion)
    Public Property Resultados As IList(Of ResultadoElementoImportacion)
    Public Property DetencionSolicitada As Boolean
End Class

Public Class ResultadoPersistenciaIntencionImportacion
    Public Property Intencion As IntencionImportacionServicio
    Public Property Reutilizada As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class ResultadoGuardIntencionImportacion
    Public Property Adquirido As Boolean
    Public Property Lease As IDisposable
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class ResultadoValidacionContextoImportacion
    Public Property Valido As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public Shared Function Exitoso() As ResultadoValidacionContextoImportacion
        Return New ResultadoValidacionContextoImportacion With {.Valido = True}
    End Function

    Public Shared Function Fallido(ByVal codigo As String,
                                   ByVal mensajeVisible As String) As ResultadoValidacionContextoImportacion
        Return New ResultadoValidacionContextoImportacion With {
            .Valido = False,
            .Codigo = codigo,
            .MensajeVisible = mensajeVisible
        }
    End Function
End Class

Public Class ResultadoResolucionProveedorImportacion
    Public Property Proveedor As IExternalImportProvider
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public ReadOnly Property Encontrado As Boolean
        Get
            Return Proveedor IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Codigo)
        End Get
    End Property
End Class

Public Class ResultadoResolucionClienteProveedorImportacion
    Public Property Cliente As IExternalImportProviderClient
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public ReadOnly Property Encontrado As Boolean
        Get
            Return Cliente IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Codigo)
        End Get
    End Property
End Class

Public Class ContextoImportacionServicio
    Private ReadOnly _idUsuario As Integer
    Private ReadOnly _idGrupo As Integer
    Private ReadOnly _loginUsuario As String
    Private ReadOnly _idTarea As Long
    Private ReadOnly _idRuta As Integer
    Private ReadOnly _idTramite As Integer
    Private ReadOnly _providerId As String
    Private ReadOnly _permiteImportar As Boolean

    Public Sub New(ByVal idUsuario As Integer,
                   ByVal idGrupo As Integer,
                   ByVal loginUsuario As String,
                   ByVal idTarea As Long,
                   ByVal idRuta As Integer,
                   ByVal idTramite As Integer,
                   ByVal providerId As String,
                   ByVal permiteImportar As Boolean)
        _idUsuario = idUsuario
        _idGrupo = idGrupo
        _loginUsuario = loginUsuario
        _idTarea = idTarea
        _idRuta = idRuta
        _idTramite = idTramite
        _providerId = providerId
        _permiteImportar = permiteImportar
    End Sub

    Public ReadOnly Property IdUsuario As Integer
        Get
            Return _idUsuario
        End Get
    End Property

    Public ReadOnly Property IdGrupo As Integer
        Get
            Return _idGrupo
        End Get
    End Property

    Public ReadOnly Property LoginUsuario As String
        Get
            Return _loginUsuario
        End Get
    End Property

    Public ReadOnly Property IdTarea As Long
        Get
            Return _idTarea
        End Get
    End Property

    Public ReadOnly Property IdRuta As Integer
        Get
            Return _idRuta
        End Get
    End Property

    Public ReadOnly Property IdTramite As Integer
        Get
            Return _idTramite
        End Get
    End Property

    Public ReadOnly Property ProviderId As String
        Get
            Return _providerId
        End Get
    End Property

    Public ReadOnly Property PermiteImportar As Boolean
        Get
            Return _permiteImportar
        End Get
    End Property
End Class

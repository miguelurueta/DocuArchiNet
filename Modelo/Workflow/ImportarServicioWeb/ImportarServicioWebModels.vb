Imports System
Imports System.Collections.Generic

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
    Public Property IdTipoDocumental As Nullable(Of Integer)
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
    Public Property Fase As FaseImportacionServicio
    Public Property IdDocumento As Nullable(Of Long)
    Public Property CodigoError As String
    Public Property MensajeVisible As String
End Class

Public Class IntencionImportacionServicio
    Public Sub New()
        Resultados = New List(Of ResultadoElementoImportacion)()
    End Sub

    Public Property Id As String
    Public Property IdempotencyKey As String
    Public Property VersionToken As String
    Public Property Fase As FaseImportacionServicio
    Public Property Resultados As IList(Of ResultadoElementoImportacion)
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

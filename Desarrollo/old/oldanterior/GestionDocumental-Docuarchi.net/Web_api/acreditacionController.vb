Imports System.Net
Imports System.Web.Http

Public Class acreditacionController

    Inherits ApiController
    'Private acreditacion As acreditacion() = New acreditacion() {New acreditacion() With {
    '.ID = 1,
    '.PROGRAMAAC = "Tomato Soup"}}

    'Private migra As New List(Of ArrayItem_integracion)
    'Private migra As acreditacion()
    Dim acreditacion_ As acreditacion()
    Public Function GetAllProducts() As IEnumerable(Of acreditacion)
        'acreditacion_ = New acreditacion() {New acreditacion() With {}}
        'Dim te As New acreditacion


        'acreditacion_ = New acreditacion()
        'acreditacion_.ANUALIDAD = "1"
        'acreditacion_.

        'Dim item As New acreditacion
        'item.ANUALIDAD = 1
        'acreditacion_.
        'item.error_funcion = ""
        'item.idliquidacion = 1
        'item.fecha = "20212"
        'item.tipotramite = "ojo"
        ''item.idmatriculabase = stru_radicado_si(i).idmatriculabase
        ''item.idproponentebase = stru_radicado_si(i).idproponentebase
        ''item.identificacionbase = stru_radicado_si(i).identificacionbase
        ''item.nombrebase = stru_radicado_si(i).nombrebase
        ''item.numerorecibo = stru_radicado_si(i).numerorecibo
        ''item.numerorecuperacion = stru_radicado_si(i).numerorecuperacion
        ''item.numeroradicacion = stru_radicado_si(i).numeroradicacion
        ''item.tramitepresencial = stru_radicado_si(i).tramitepresencial
        'item.firmadoelectronicamente = "no"
        'item.IMP_02_ID_CLAVE = 0
        'item.estado_migrado = 0
        'item.estado_migrado = 0
        'migra.Add(item)
        Return acreditacion_
    End Function

    'Public Function GetProductById(id As Integer) As ArrayItem_integracion
    '    Dim acreditacion = migra.FirstOrDefault(Function(p) p.ID = id)
    '    If acreditacion Is Nothing Then
    '        Throw New HttpResponseException(HttpStatusCode.NotFound)
    '    End If
    '    Return ArrayItem_integracion
    'End Function
End Class

Public Class acreditacion
    Private m_ID As Integer
    Public Property ID() As Integer
        Get
            Return m_ID
        End Get
        Set(value As Integer)
            m_ID = value
        End Set
    End Property

    Dim m_PROGRAMAAC As String
    Public Property PROGRAMAAC() As String
        Get
            Return m_PROGRAMAAC
        End Get
        Set(value As String)
            m_PROGRAMAAC = value
        End Set
    End Property
    Dim m_FACULTAD As String
    Public Property FACULTAD() As String
        Get
            Return m_FACULTAD
        End Get
        Set(value As String)
            m_FACULTAD = value
        End Set
    End Property
    Dim m_ANUALIDAD As String
    Public Property ANUALIDAD() As String
        Get
            Return m_ANUALIDAD
        End Get
        Set(value As String)
            m_ANUALIDAD = value
        End Set
    End Property
    Dim m_TIPODOCUME As String
    Public Property TIPODOCUME() As String
        Get
            Return m_TIPODOCUME
        End Get
        Set(value As String)
            m_TIPODOCUME = value
        End Set
    End Property
End Class

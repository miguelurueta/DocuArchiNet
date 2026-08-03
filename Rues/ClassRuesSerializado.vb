Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization
Public Structure rue_camara
    Dim codigo_camara As String
    Dim emailUsuario As String
    Dim identificacionusuario As String
    Dim nombreUsuario As String
    Dim nitEntidad As String
    Dim nombreEntidad As String
    Dim municipioEntidad As String
    Dim tipoRegistro As String
    Dim expediente As String
End Structure
Public Class ClassRuesSerializado
    Function Serializacion_java(ByVal param As String, ByRef stru As rue_camara) As String
        
        Try
            Dim parram As New parram
            parram = Deserialize(Of parram)(param)
            stru.codigo_camara = parram.codigo_camara
            stru.emailUsuario = parram.emailUsuario
            stru.identificacionusuario = parram.identificacionusuario
            stru.nombreUsuario = parram.nombreUsuario
            stru.nitEntidad = parram.nitEntidad
            stru.nombreEntidad = parram.nombreEntidad
            stru.municipioEntidad = parram.municipioEntidad
            stru.tipoRegistro = parram.tipoRegistro
            stru.expediente = parram.expediente
            Serializacion_java = "YES"
        Catch ex As Exception
            Serializacion_java = "Inconsistencia general función Serializacion_java " & ex.Message
        End Try
    End Function
    Public Function Deserialize(Of T)(context As String) As T
        Dim jsonData As String = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
        Return obj
    End Function
End Class
Public Class parram
    Public expediente As String
    Public Property expediente_() As String
        Get
            Return expediente
        End Get
        Set(value As String)
            expediente = value
        End Set
    End Property
    Public tipoRegistro As String
    Public Property tipoRegistro_() As String
        Get
            Return tipoRegistro
        End Get
        Set(value As String)
            tipoRegistro = value
        End Set
    End Property
    Public municipioEntidad As String
    Public Property municipioEntidad_() As String
        Get
            Return municipioEntidad
        End Get
        Set(value As String)
            municipioEntidad = value
        End Set
    End Property
    Public nombreEntidad As String
    Public Property nombreEntidad_() As String
        Get
            Return nombreEntidad
        End Get
        Set(value As String)
            nombreEntidad = value
        End Set
    End Property
    Public nitEntidad As String
    Public Property nitEntidad_() As String
        Get
            Return nitEntidad
        End Get
        Set(value As String)
            nitEntidad = value
        End Set
    End Property
    Public nombreUsuario As String
    Public Property nombreUsuario_() As String
        Get
            Return nombreUsuario
        End Get
        Set(value As String)
            nombreUsuario = value
        End Set
    End Property
    Public codigo_camara As String
    Public Property codigo_camara_() As String
        Get
            Return codigo_camara
        End Get
        Set(value As String)
            codigo_camara = value
        End Set
    End Property
    Public emailUsuario As String
    Public Property emailUsuario_() As String
        Get
            Return emailUsuario
        End Get
        Set(value As String)
            emailUsuario = value
        End Set
    End Property
    Public identificacionusuario As String
    Public Property identificacionusuario_() As String
        Get
            Return identificacionusuario
        End Get
        Set(value As String)
            identificacionusuario = value
        End Set
    End Property
End Class

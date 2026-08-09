Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class HandlerListaAyuda
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strJson As String = New StreamReader(context.Request.InputStream).ReadToEnd()
        Dim chk_editor_class As New nombre_ayuda_modulo
        chk_editor_class = Deserialize(Of nombre_ayuda_modulo)(strJson)
        Dim ref_clas As New ClassGaExpediente
        Dim result As String = ""
        Dim numero_documento As Integer = 0
        result = ref_clas.Lista_ayuda_aplicacion(chk_editor_class.nombre_ayuda)
        context.Response.Write(result)

    End Sub
    Public Function Deserialize(Of T)(context As String) As t
        Dim jsonData As String = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
        Return obj
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
Public Class nombre_ayuda_modulo
    Public Property chk_editor() As String
        Get
            Return nombre_ayuda
        End Get
        Set(value As String)
            nombre_ayuda = value
        End Set
    End Property
    Public nombre_ayuda As String
    Public Property nombre_modulo_ayuda() As String
        Get
            Return nombre_modulo
        End Get
        Set(value As String)
            nombre_modulo = value
        End Set
    End Property
    Public nombre_modulo As String
End Class
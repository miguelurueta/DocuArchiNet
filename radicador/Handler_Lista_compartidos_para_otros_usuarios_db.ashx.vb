Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Handler_Lista_compartidos_para_otros_usuarios_db
    Implements System.Web.IHttpHandler, System.Web.SessionState.IReadOnlySessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strJson As String = New StreamReader(context.Request.InputStream).ReadToEnd()
        Dim chk_editor_class As New chk_editor
        chk_editor_class = Deserialize(Of chk_editor)(strJson)
        Dim ref_clas As New ClassGaCompartirDocumento
        Dim result As String = ""
        Dim numero_documento As Integer = 0
        result = ref_clas.Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), numero_documento)
        context.Response.Write(numero_documento)

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

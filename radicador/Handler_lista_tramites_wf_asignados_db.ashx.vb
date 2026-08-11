Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Handler_lista_tramites_wf_asignados_db
    Implements System.Web.IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strJson As String = New StreamReader(context.Request.InputStream).ReadToEnd()
        Dim chk_editor_class As New chk_editor
        chk_editor_class = Deserialize(Of chk_editor)(strJson)
        Dim ref_clas As New Class_Lista_tramites_por_responder
        Dim result As String = ""
        Dim numero_documento As Integer = 0
        result = ref_clas.Lista_numero_tramites(HttpContext.Current.Session.Item("Id_Usuario_Workflow"), _
                                                HttpContext.Current.Session.Item("Id_Ruta_Workflow"), _
                                                HttpContext.Current.Session.Item("Id_Grupo_Workflow"), _
                                                HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"), _
                                                HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"), _
                                                numero_documento)
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

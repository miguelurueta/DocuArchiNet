Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Handler_lista_tareas_asignadas_workflow_db
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim Result As String = ""
        Dim ClassListandoTareas As New ClassListandoTareas
        Result = ClassListandoTareas.Inicializa_lista_tarea_workflow_simple(HttpContext.Current.Session.Item("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
        context.Response.Write(HttpContext.Current.Session.Item("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
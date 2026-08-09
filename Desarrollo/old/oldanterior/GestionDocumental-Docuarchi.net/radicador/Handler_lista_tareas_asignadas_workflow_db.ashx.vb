Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Handler_lista_tareas_asignadas_workflow_db
    Implements System.Web.IHttpHandler, System.Web.SessionState.IReadOnlySessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim Result As String = ""
        Dim ClassListandoTareas As New ClassListandoTareas
        Dim numeroTareas As Integer = 0
        Result = ClassListandoTareas.Inicializa_lista_tarea_workflow_simple(numeroTareas)
        context.Response.Write(numeroTareas)
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class

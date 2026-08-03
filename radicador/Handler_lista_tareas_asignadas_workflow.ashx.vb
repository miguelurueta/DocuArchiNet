Imports System.Web
Imports System.Web.Services

Public Class Handler_lista_tareas_asignadas_workflow
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.Write(HttpContext.Current.Session.Item("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
Imports System.Web
Imports System.Web.Services

Public Class refresh_session
    Implements System.Web.IHttpHandler
    Implements IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            context.Response.ContentType = "text/plain"
            If HttpContext.Current.Session("EXTENSION_ARCHIVO_ADJUNTA") Is Nothing Then
                context.Response.Write("-1")
            Else
                context.Response.Write(HttpContext.Current.Session.SessionID)

            End If
        Catch ex As Exception
            context.Response.Write("-1")
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
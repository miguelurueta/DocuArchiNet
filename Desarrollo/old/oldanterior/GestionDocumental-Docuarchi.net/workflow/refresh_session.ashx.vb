Imports System.Web
Imports System.Web.Services

Public Class refresh_session
    Implements System.Web.IHttpHandler
    Implements IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            context.Response.ContentType = "application/json"
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
            context.Response.Cache.SetNoStore()

            Dim idUsuarioGestion As String = Convert.ToString(context.Session.Item("GA_IDUSUARIOGESTION"))
            Dim idUsuarioDocuarchi As String = Convert.ToString(context.Session.Item("ID_USUARIO_DOCUARCHI"))
            Dim tieneUsuarioSesion As Boolean =
                (Not String.IsNullOrWhiteSpace(idUsuarioGestion) AndAlso idUsuarioGestion <> "0") OrElse
                (Not String.IsNullOrWhiteSpace(idUsuarioDocuarchi) AndAlso idUsuarioDocuarchi <> "0")
            Dim autenticado As Boolean = context.User IsNot Nothing AndAlso
                context.User.Identity IsNot Nothing AndAlso
                context.User.Identity.IsAuthenticated

            If autenticado AndAlso tieneUsuarioSesion Then
                context.Response.StatusCode = 200
                context.Response.Write("{""active"":true}")
                Return
            End If

            context.Response.StatusCode = 401
            context.Response.TrySkipIisCustomErrors = True
            context.Response.SuppressFormsAuthenticationRedirect = True
            context.Response.Write("{""active"":false}")
        Catch ex As Exception
            context.Response.StatusCode = 503
            context.Response.TrySkipIisCustomErrors = True
            context.Response.Write("{""active"":false}")
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class

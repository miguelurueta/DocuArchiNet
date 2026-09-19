Imports System
Imports System.Configuration
Imports System.IO
Imports System.Web
Imports System.Web.SessionState

Public NotInheritable Class ImportarServicioWebPreview
    Implements IHttpHandler, IRequiresSessionState

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        If context Is Nothing Then Return
        ApplyDefensiveHeaders(context.Response)
        If Not FeatureEnabled() Then Reject(context, 404) : Return
        Dim method = If(context.Request.HttpMethod, String.Empty).ToUpperInvariant()
        If method <> "GET" AndAlso method <> "HEAD" Then
            context.Response.AppendHeader("Allow", "GET, HEAD")
            Reject(context, 405) : Return
        End If
        Dim descriptor = Convert.ToString(context.Request.QueryString("d"))
        If ImportPreviewDescriptorService.DescriptorHash(descriptor) Is Nothing Then Reject(context, 404) : Return
        Try
            Dim session = New WorkflowPreviewSessionContextGate().AsegurarContexto()
            If session Is Nothing OrElse session.Contexto Is Nothing OrElse Not session.Contexto.EsValido() OrElse
               String.IsNullOrWhiteSpace(session.CadenaConexionWorkflow) Then Reject(context, 404) : Return
            Dim taskId As Long
            If Not Long.TryParse(Convert.ToString(context.Session.Item("ID_TAREA_SELECCIONDA")), taskId) OrElse taskId <= 0 Then Reject(context, 404) : Return
            Dim moduleContext As New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB",
                .IdUsuario = session.Contexto.IdUsuarioWorkflow, .IdGrupo = session.Contexto.IdGrupoWorkflow,
                .LoginUsuario = session.Contexto.LoginUsuario}
            Dim service = ImportPreviewCompositionFactory.CreateContentService(session.CadenaConexionWorkflow, moduleContext)
            Dim authority As New ImportPreviewAuthority With {.UserId = session.Contexto.IdUsuarioWorkflow,
                .TaskId = taskId, .ProviderId = SiiImportProvider.CanonicalProviderId}
            If method = "HEAD" Then
                Dim metadata = service.Head(descriptor, authority, DateTime.UtcNow)
                If metadata Is Nothing Then Reject(context, 404) : Return
                ApplyContentHeaders(context.Response, metadata)
                context.Response.SuppressContent = True
                context.Response.StatusCode = 200
                Return
            End If
            Dim snapshot = service.Claim(descriptor, authority, DateTime.UtcNow)
            If snapshot Is Nothing OrElse snapshot.Content Is Nothing OrElse snapshot.Content.LongLength <> snapshot.ContentLength Then Reject(context, 404) : Return
            ApplyContentHeaders(context.Response, snapshot)
            context.Response.StatusCode = 200
            Try
                Const blockSize As Integer = 65536
                Dim offset As Integer = 0
                While offset < snapshot.Content.Length AndAlso context.Response.IsClientConnected
                    Dim count = Math.Min(blockSize, snapshot.Content.Length - offset)
                    context.Response.OutputStream.Write(snapshot.Content, offset, count)
                    offset += count
                End While
            Finally
                service.Complete(snapshot, DateTime.UtcNow)
            End Try
        Catch
            If Not context.Response.HeadersWritten Then Reject(context, 503)
        End Try
    End Sub

    Private Shared Sub ApplyDefensiveHeaders(ByVal response As HttpResponse)
        response.Cache.SetCacheability(HttpCacheability.Private)
        response.Cache.SetNoStore()
        response.Cache.SetNoServerCaching()
        response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches)
        response.AppendHeader("Pragma", "no-cache")
        response.AppendHeader("X-Content-Type-Options", "nosniff")
        response.AppendHeader("X-Frame-Options", "SAMEORIGIN")
    End Sub

    Private Shared Sub ApplyContentHeaders(ByVal response As HttpResponse, ByVal snapshot As ImportPreviewSnapshot)
        response.ContentType = snapshot.ContentType
        response.AppendHeader("Content-Length", snapshot.ContentLength.ToString(Globalization.CultureInfo.InvariantCulture))
        response.AppendHeader("Content-Disposition", snapshot.ContentDisposition & "; filename=""" & snapshot.SafeFileName & """")
    End Sub

    Private Shared Sub Reject(ByVal context As HttpContext, ByVal statusCode As Integer)
        context.Response.ClearContent()
        context.Response.StatusCode = statusCode
        context.Response.TrySkipIisCustomErrors = True
        context.Response.SuppressContent = True
    End Sub

    Private Shared Function FeatureEnabled() As Boolean
        Return String.Equals(ConfigurationManager.AppSettings("WorkflowCentroTrabajoModernActive"), "true", StringComparison.OrdinalIgnoreCase)
    End Function
End Class

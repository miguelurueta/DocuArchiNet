Imports System
Imports System.ComponentModel
Imports System.Configuration
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Services

' Frontera paralela y delgada para ImportarServicioWeb. No contiene reglas SII ni persistencia.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceImportarServicioWebModern
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Async Function ResolveCapabilities(ByVal request As ResolveCapabilitiesRequestDto) As Task(Of ResolveCapabilitiesResponseDto)
        If Not FeatureEnabled() Then Return DisabledCapabilities(request)
        If Not ValidRequest(request) Then Return InvalidCapabilities(request)
        Try
            Dim importContext As ContextoImportacionServicio = Nothing
            If Not TryBuildImportContext(request, importContext) Then Return ProviderCapabilitiesError(request, New ResultadoResolucionClienteProveedorImportacion With {.Codigo = "FORBIDDEN"})
            Dim provider = ResolveProvider(request.ProviderId)
            If Not provider.Encontrado Then Return ProviderCapabilitiesError(request, provider)
            Return Await provider.Cliente.ResolveCapabilitiesAsync(request, CancellationToken.None).ConfigureAwait(False)
        Catch
            Return InvalidCapabilities(request)
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Async Function QueryItems(ByVal request As QueryItemsRequestDto) As Task(Of QueryItemsResponseDto)
        If Not FeatureEnabled() Then Return FailureQuery(request, "FEATURE_DISABLED")
        If Not ValidRequest(request) Then Return FailureQuery(request, "INVALID_REQUEST")
        Try
            Dim importContext As ContextoImportacionServicio = Nothing
            If Not TryBuildImportContext(request, importContext) Then Return FailureQuery(request, "FORBIDDEN")
            Dim provider = ResolveProvider(request.ProviderId)
            If Not provider.Encontrado Then Return FailureQuery(request, provider.Codigo)
            Return Await provider.Cliente.QueryItemsAsync(request, CancellationToken.None).ConfigureAwait(False)
        Catch
            Return FailureQuery(request, "EXTERNAL_PROVIDER_UNAVAILABLE")
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Async Function GetPreview(ByVal request As GetPreviewRequestDto) As Task(Of GetPreviewResponseDto)
        If Not FeatureEnabled() Then Return FailurePreview(request, "FEATURE_DISABLED")
        If Not ValidRequest(request) OrElse String.IsNullOrWhiteSpace(request.ExternalKey) Then Return FailurePreview(request, "INVALID_REQUEST")
        Try
            Dim context As ContextoImportacionServicio = Nothing
            If Not TryBuildImportContext(request, context) Then Return FailurePreview(request, "PREVIEW_FORBIDDEN")
            Dim provider = ResolveProvider(request.ProviderId)
            If Not provider.Encontrado Then Return FailurePreview(request, provider.Codigo)
            Dim source = Await provider.Cliente.GetPreviewAsync(request, CancellationToken.None).ConfigureAwait(False)
            Return New SiiPreviewResponseFactory(New String() {"application/pdf", "image/png", "image/jpeg"},
                MaximumPreviewBytes()).Create(source, request, context, DateTime.UtcNow)
        Catch
            Return FailurePreview(request, "EXTERNAL_PROVIDER_UNAVAILABLE")
        End Try
    End Function

    Private Shared Function FeatureEnabled() As Boolean
        Return String.Equals(ConfigurationManager.AppSettings("WorkflowCentroTrabajoModernActive"), "true", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function TryBuildImportContext(ByVal request As SolicitudImportacionServicioDto,
                                                  ByRef context As ContextoImportacionServicio) As Boolean
        context = Nothing
        Dim result = New WorkflowPreviewSessionContextGate().AsegurarContexto()
        Dim current = HttpContext.Current
        If result Is Nothing OrElse result.Contexto Is Nothing OrElse Not result.Contexto.EsValido() OrElse
           current Is Nothing OrElse current.Session Is Nothing Then Return False

        Dim trustedTaskId As Long
        Dim trustedProcedureId As Integer
        If Not Long.TryParse(Convert.ToString(current.Session.Item("ID_TAREA_SELECCIONDA")), trustedTaskId) OrElse
           Not Integer.TryParse(Convert.ToString(current.Session.Item("DG_ID_TRAMITE")), trustedProcedureId) OrElse
           trustedTaskId <= 0 OrElse trustedProcedureId <= 0 OrElse request Is Nothing OrElse request.TaskId <> trustedTaskId Then Return False

        context = New ContextoImportacionServicio(result.Contexto.IdUsuarioWorkflow,
            result.Contexto.IdGrupoWorkflow, result.Contexto.LoginUsuario, trustedTaskId,
            result.Contexto.IdRutaWorkflow, trustedProcedureId, request.ProviderId, True)
        Return True
    End Function

    Private Shared Function ValidRequest(ByVal request As SolicitudImportacionServicioDto) As Boolean
        Return request IsNot Nothing AndAlso request.TaskId > 0 AndAlso
            Not String.IsNullOrWhiteSpace(request.OperationId) AndAlso Not String.IsNullOrWhiteSpace(request.CorrelationId) AndAlso
            String.Equals(request.ProviderId, SiiImportProvider.CanonicalProviderId, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ResolveProvider(ByVal providerId As String) As ResultadoResolucionClienteProveedorImportacion
        Dim baseUri As Uri = Nothing
        If Not Uri.TryCreate(ConfigurationManager.AppSettings("ImportarServicioWebSiiBaseUrl"), UriKind.Absolute, baseUri) Then
            Return New ResultadoResolucionClienteProveedorImportacion With {.Codigo = "PROVIDER_NOT_CONFIGURED", .MensajeVisible = "El proveedor solicitado no está disponible."}
        End If
        Dim factory As New ExternalImportHttpClientFactory()
        Dim transport As New ExternalImportHttpTransport(factory, New ExternalImportHttpResponseValidator(), New ExternalImportHttpErrorMapper())
        Dim client As New SiiExternalImportProviderClient(transport, baseUri,
            ConfigurationManager.AppSettings("ImportarServicioWebSiiAuthorization"), TimeSpan.FromSeconds(30), MaximumPreviewBytes())
        Dim registry As New RegistroClientesProveedoresImportacion(New IExternalImportProviderClient() {New SiiImportProvider(client, New SiiImportContractMapper())})
        Return registry.Resolver(providerId)
    End Function

    Private Shared Function MaximumPreviewBytes() As Long
        Dim value As Long
        If Long.TryParse(ConfigurationManager.AppSettings("ImportarServicioWebPreviewMaximumBytes"), value) AndAlso value > 0 Then Return value
        Return 10485760L
    End Function

    Private Shared Function ErrorDto(ByVal code As String) As ErrorImportacionServicioDto
        Return New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = "La operación no está disponible."}
    End Function

    Private Shared Function DisabledCapabilities(ByVal request As ResolveCapabilitiesRequestDto) As ResolveCapabilitiesResponseDto
        Return ProviderCapabilitiesError(request, New ResultadoResolucionClienteProveedorImportacion With {.Codigo = "FEATURE_DISABLED"})
    End Function

    Private Shared Function InvalidCapabilities(ByVal request As ResolveCapabilitiesRequestDto) As ResolveCapabilitiesResponseDto
        Return ProviderCapabilitiesError(request, New ResultadoResolucionClienteProveedorImportacion With {.Codigo = "INVALID_REQUEST"})
    End Function

    Private Shared Function ProviderCapabilitiesError(ByVal request As ResolveCapabilitiesRequestDto, ByVal result As ResultadoResolucionClienteProveedorImportacion) As ResolveCapabilitiesResponseDto
        Return New ResolveCapabilitiesResponseDto With {.OperationId = If(request Is Nothing, String.Empty, request.OperationId),
            .CorrelationId = If(request Is Nothing, String.Empty, request.CorrelationId), .ContextAllowed = False, .Error = ErrorDto(result.Codigo)}
    End Function

    Private Shared Function FailureQuery(ByVal request As QueryItemsRequestDto, ByVal code As String) As QueryItemsResponseDto
        Return New QueryItemsResponseDto With {.OperationId = If(request Is Nothing, String.Empty, request.OperationId),
            .CorrelationId = If(request Is Nothing, String.Empty, request.CorrelationId), .Error = ErrorDto(code)}
    End Function

    Private Shared Function FailurePreview(ByVal request As GetPreviewRequestDto, ByVal code As String) As GetPreviewResponseDto
        Return New GetPreviewResponseDto With {.OperationId = If(request Is Nothing, String.Empty, request.OperationId),
            .CorrelationId = If(request Is Nothing, String.Empty, request.CorrelationId), .Error = ErrorDto(code)}
    End Function
End Class

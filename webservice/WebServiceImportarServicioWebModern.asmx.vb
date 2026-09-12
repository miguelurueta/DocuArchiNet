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
    Public Function ResolveCapabilities(ByVal request As ResolveCapabilitiesRequestDto) As ResolveCapabilitiesResponseDto
        If Not FeatureEnabled() Then Return DisabledCapabilities(request)
        If Not ValidRequest(request) Then Return InvalidCapabilities(request)
        Try
            Dim importContext As ContextoImportacionServicio = Nothing
            Dim contextFailure As String = Nothing
            If Not TryBuildImportContext(request, importContext, contextFailure) Then Return ProviderCapabilitiesError(request, New ResultadoResolucionClienteProveedorImportacion With {.Codigo = contextFailure})
            Dim provider = ResolveProvider(request.ProviderId)
            If Not provider.Encontrado Then Return ProviderCapabilitiesError(request, provider)
            Return provider.Cliente.ResolveCapabilitiesAsync(request, CancellationToken.None).GetAwaiter().GetResult()
        Catch
            Return ProviderCapabilitiesError(request, New ResultadoResolucionClienteProveedorImportacion With {.Codigo = "CAPABILITIES_UNAVAILABLE"})
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function QueryItems(ByVal request As QueryItemsRequestDto) As QueryItemsResponseDto
        If Not FeatureEnabled() Then Return FailureQuery(request, "FEATURE_DISABLED")
        If Not ValidRequest(request) Then Return FailureQuery(request, "INVALID_REQUEST")
        Try
            Dim importContext As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            Dim contextFailure As String = Nothing
            If Not TryBuildImportContext(request, importContext, session, contextFailure) Then Return FailureQuery(request, contextFailure)
            If String.IsNullOrWhiteSpace(request.CodigoBarras) Then Return FailureQuery(request, "SII_BARCODE_UNAVAILABLE")
            request.CodigoBarras = request.CodigoBarras.Trim()
            Dim provider = ResolveProvider(request.ProviderId, CreateAttemptRecorder(session, importContext))
            If Not provider.Encontrado Then Return FailureQuery(request, provider.Codigo)
            Return provider.Cliente.QueryItemsAsync(request, CancellationToken.None).GetAwaiter().GetResult()
        Catch ex As ExternalImportHttpException
            Return FailureQuery(request, SafeExternalCode(ex))
        Catch ex As InvalidOperationException
            Return FailureQuery(request, SafeProviderCode(ex.Message))
        Catch
            Return FailureQuery(request, "EXTERNAL_PROVIDER_UNAVAILABLE")
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function GetPreview(ByVal request As GetPreviewRequestDto) As GetPreviewResponseDto
        If Not FeatureEnabled() Then Return FailurePreview(request, "FEATURE_DISABLED")
        If Not ValidRequest(request) OrElse String.IsNullOrWhiteSpace(request.ExternalKey) Then Return FailurePreview(request, "INVALID_REQUEST")
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            If Not TryBuildImportContext(request, context, session) Then Return FailurePreview(request, "PREVIEW_FORBIDDEN")
            Dim provider = ResolveProvider(request.ProviderId, CreateAttemptRecorder(session, context))
            If Not provider.Encontrado Then Return FailurePreview(request, provider.Codigo)
            Dim source = provider.Cliente.GetPreviewAsync(request, CancellationToken.None).GetAwaiter().GetResult()
            Return New SiiPreviewResponseFactory(New String() {"application/pdf", "image/png", "image/jpeg", "image/tiff"},
                MaximumPreviewBytes()).Create(source, request, context, DateTime.UtcNow)
        Catch ex As ExternalImportHttpException
            Return FailurePreview(request, SafeExternalCode(ex))
        Catch ex As InvalidOperationException
            Return FailurePreview(request, SafeProviderCode(ex.Message))
        Catch
            Return FailurePreview(request, "EXTERNAL_PROVIDER_UNAVAILABLE")
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function PreflightImport(ByVal request As PreflightImportRequestDto) As PreflightImportResponseDto
        If Not FeatureEnabled() Then Return New PreflightImportResponseDto With {.Error = ErrorDto("FEATURE_DISABLED")}
        If Not ValidRequest(request) Then Return New PreflightImportResponseDto With {.Error = ErrorDto("INVALID_REQUEST")}
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            Dim contextFailure As String = Nothing
            If Not TryBuildImportContext(request, context, session, contextFailure) Then
                Return New PreflightImportResponseDto With {.Error = ErrorDto(contextFailure)}
            End If
            Dim composition = Compose(session, request.ProviderId, context)
            Return composition.Preflight.Preflight(context, request)
        Catch
            Return New PreflightImportResponseDto With {.Error = ErrorDto("IMPORT_UNAVAILABLE")}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function CreateImportIntent(ByVal request As CreateImportIntentRequestDto) As CreateImportIntentResponseDto
        If Not FeatureEnabled() Then Return New CreateImportIntentResponseDto With {.Error = ErrorDto("FEATURE_DISABLED")}
        If Not ValidRequest(request) Then Return New CreateImportIntentResponseDto With {.Error = ErrorDto("INVALID_REQUEST")}
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            If Not TryBuildImportContext(request, context, session) Then Return New CreateImportIntentResponseDto With {.Error = ErrorDto("FORBIDDEN")}
            Return Compose(session, request.ProviderId, context).Intents.Crear(context, request)
        Catch
            Return New CreateImportIntentResponseDto With {.Error = ErrorDto("IMPORT_UNAVAILABLE")}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ExecuteImportIntent(ByVal request As ExecuteImportIntentRequestDto) As ExecuteImportIntentResponseDto
        If Not FeatureEnabled() Then Return New ExecuteImportIntentResponseDto With {.Error = ErrorDto("FEATURE_DISABLED")}
        If Not ValidRequest(request) Then Return New ExecuteImportIntentResponseDto With {.Error = ErrorDto("INVALID_REQUEST")}
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            If Not TryBuildImportContext(request, context, session) Then Return New ExecuteImportIntentResponseDto With {.Error = ErrorDto("FORBIDDEN")}
            Dim composition = Compose(session, request.ProviderId, context)
            Dim execution = composition.Orchestrator.Execute(context, request)
            Return composition.Reconciliation.ProjectExecutionResult(context, execution)
        Catch ex As Exception
            Return New ExecuteImportIntentResponseDto With {.Error = ErrorDto("IMPORT_UNAVAILABLE", SafeExecutionDiagnostic(ex.Message))}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function GetImportIntent(ByVal request As GetImportIntentRequestDto) As GetImportIntentResponseDto
        If Not FeatureEnabled() Then Return New GetImportIntentResponseDto With {.Error = ErrorDto("FEATURE_DISABLED")}
        If Not ValidRequest(request) Then Return New GetImportIntentResponseDto With {.Error = ErrorDto("INVALID_REQUEST")}
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            If Not TryBuildImportContext(request, context, session) Then Return New GetImportIntentResponseDto With {.Error = ErrorDto("FORBIDDEN")}
            Return Compose(session, request.ProviderId, context).Reconciliation.GetImportIntent(context, request)
        Catch ex As InvalidOperationException
            Return New GetImportIntentResponseDto With {.Error = ErrorDto(SafeReconciliationCode(ex.Message))}
        Catch
            Return New GetImportIntentResponseDto With {.Error = ErrorDto("IMPORT_UNAVAILABLE")}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ReconcileImportIntent(ByVal request As ReconcileImportIntentRequestDto) As ReconcileImportIntentResponseDto
        If Not FeatureEnabled() Then Return New ReconcileImportIntentResponseDto With {.Error = ErrorDto("FEATURE_DISABLED")}
        If Not ValidRequest(request) Then Return New ReconcileImportIntentResponseDto With {.Error = ErrorDto("INVALID_REQUEST")}
        Try
            Dim context As ContextoImportacionServicio = Nothing : Dim session As ResultadoContextoSesionWorkflow = Nothing
            If Not TryBuildImportContext(request, context, session) Then Return New ReconcileImportIntentResponseDto With {.Error = ErrorDto("FORBIDDEN")}
            Return Compose(session, request.ProviderId, context).Reconciliation.ReconcileImportIntent(context, request)
        Catch
            Return New ReconcileImportIntentResponseDto With {.Error = ErrorDto("IMPORT_UNAVAILABLE")}
        End Try
    End Function

    Private Shared Function FeatureEnabled() As Boolean
        Return String.Equals(ConfigurationManager.AppSettings("WorkflowCentroTrabajoModernActive"), "true", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function TryBuildImportContext(ByVal request As SolicitudImportacionServicioDto,
                                                  ByRef context As ContextoImportacionServicio) As Boolean
        Dim session As ResultadoContextoSesionWorkflow = Nothing
        Dim failureCode As String = Nothing
        Return TryBuildImportContext(request, context, session, failureCode)
    End Function

    Private Shared Function TryBuildImportContext(ByVal request As SolicitudImportacionServicioDto,
                                                  ByRef context As ContextoImportacionServicio,
                                                  ByRef failureCode As String) As Boolean
        Dim session As ResultadoContextoSesionWorkflow = Nothing
        Return TryBuildImportContext(request, context, session, failureCode)
    End Function

    Private Shared Function TryBuildImportContext(ByVal request As SolicitudImportacionServicioDto,
                                                  ByRef context As ContextoImportacionServicio,
                                                  ByRef session As ResultadoContextoSesionWorkflow) As Boolean
        Dim failureCode As String = Nothing
        Return TryBuildImportContext(request, context, session, failureCode)
    End Function

    Private Shared Function TryBuildImportContext(ByVal request As SolicitudImportacionServicioDto,
                                                  ByRef context As ContextoImportacionServicio,
                                                  ByRef session As ResultadoContextoSesionWorkflow,
                                                  ByRef failureCode As String) As Boolean
        context = Nothing
        failureCode = "SESSION_CONTEXT_UNAVAILABLE"
        Dim current = HttpContext.Current
        If current Is Nothing OrElse current.Session Is Nothing Then
            failureCode = "SESSION_HTTP_CONTEXT_UNAVAILABLE"
            Return False
        End If

        'Captura únicamente presencia/validez antes de que el gate fail-closed
        'pueda limpiar el contexto. Nunca expone valores ni conexiones.
        Dim sessionUser As Integer = 0
        Dim sessionGroup As Integer = 0
        Dim sessionRoute As Integer = 0
        Integer.TryParse(Convert.ToString(current.Session.Item("Id_Usuario_Workflow")), sessionUser)
        Integer.TryParse(Convert.ToString(current.Session.Item("Id_Grupo_Workflow")), sessionGroup)
        Integer.TryParse(Convert.ToString(current.Session.Item("Id_Ruta_Workflow")), sessionRoute)
        Dim sessionLoginPresent = Not String.IsNullOrWhiteSpace(Convert.ToString(current.Session.Item("Login_Usuario_Workfow")))
        Dim workflowConnectionPresent = Not String.IsNullOrWhiteSpace(Convert.ToString(current.Session.Item("IP_SERVER_MODULO"))) AndAlso
                                        Not String.IsNullOrWhiteSpace(Convert.ToString(current.Session.Item("DB_NAME_MODULO"))) AndAlso
                                        Not String.IsNullOrWhiteSpace(Convert.ToString(current.Session.Item("USER_DBMS_MODULO"))) AndAlso
                                        Not String.IsNullOrWhiteSpace(Convert.ToString(current.Session.Item("PASW_DBMS_MODULO"))) AndAlso
                                        String.Equals(Convert.ToString(current.Session.Item("TYPE_DBMS_MODULO")).Trim(), "mysql", StringComparison.OrdinalIgnoreCase)
        session = New WorkflowPreviewSessionContextGate().AsegurarContexto()
        Dim result = session
        If result Is Nothing OrElse result.Contexto Is Nothing OrElse Not result.Contexto.EsValido() Then
            If sessionUser <= 0 Then
                failureCode = "SESSION_WORKFLOW_USER_UNAVAILABLE"
            ElseIf sessionGroup <= 0 Then
                failureCode = "SESSION_WORKFLOW_GROUP_UNAVAILABLE"
            ElseIf sessionRoute <= 0 Then
                failureCode = "SESSION_WORKFLOW_ROUTE_UNAVAILABLE"
            ElseIf Not sessionLoginPresent Then
                failureCode = "SESSION_WORKFLOW_LOGIN_UNAVAILABLE"
            ElseIf Not workflowConnectionPresent Then
                failureCode = "SESSION_WORKFLOW_CONNECTION_UNAVAILABLE"
            Else
                failureCode = "SESSION_CONTEXT_GATE_REJECTED"
            End If
            Return False
        End If

        Dim trustedTaskId As Long
        Dim trustedProcedureId As Integer
        If Not Long.TryParse(Convert.ToString(current.Session.Item("ID_TAREA_SELECCIONDA")), trustedTaskId) OrElse trustedTaskId <= 0 Then
            failureCode = "SESSION_TASK_UNAVAILABLE" : Return False
        End If
        If request Is Nothing OrElse request.TaskId <> trustedTaskId Then failureCode = "TASK_CONTEXT_MISMATCH" : Return False
        If Not Integer.TryParse(Convert.ToString(current.Session.Item("DG_ID_TRAMITE")), trustedProcedureId) OrElse trustedProcedureId <= 0 Then
            Dim routeName = Convert.ToString(current.Session.Item("WF_RUTAWORKFLOW"))
            Dim resolution = New Classselecciotarea().Solicita_id_tipo_tramite_tarea_workflow(
                trustedTaskId, result.Contexto.IdRutaWorkflow, routeName, trustedProcedureId)
            If Not String.Equals(resolution, "YES", StringComparison.OrdinalIgnoreCase) OrElse trustedProcedureId <= 0 Then
                failureCode = "SERVER_PROCEDURE_UNAVAILABLE" : Return False
            End If
        End If

        context = New ContextoImportacionServicio(result.Contexto.IdUsuarioWorkflow,
            result.Contexto.IdGrupoWorkflow, result.Contexto.LoginUsuario, trustedTaskId,
            result.Contexto.IdRutaWorkflow, trustedProcedureId, request.ProviderId, True)
        failureCode = Nothing
        Return True
    End Function

    Private Shared Function Compose(ByVal session As ResultadoContextoSesionWorkflow, ByVal providerId As String,
                                    ByVal trustedContext As ContextoImportacionServicio) As ImportComposition
        If session Is Nothing OrElse String.IsNullOrWhiteSpace(session.CadenaConexionWorkflow) Then Throw New InvalidOperationException("IMPORT_CONTEXT_UNAVAILABLE")
        Dim connections As IModuleConnectionFactory = New WorkflowModuleConnectionFactory(session.CadenaConexionWorkflow)
        If String.IsNullOrWhiteSpace(session.CadenaConexionDocuarchi) Then Throw New InvalidOperationException("DOCUARCHI_RECONCILIATION_UNAVAILABLE")
        Dim docuarchiConnections As IModuleConnectionFactory = New DocuarchiModuleConnectionFactory(session.CadenaConexionDocuarchi)
        If String.IsNullOrWhiteSpace(session.CadenaConexionRadicacion) Then Throw New InvalidOperationException("RADICACION_CONTEXT_UNAVAILABLE")
        Dim radicacionConnections As IModuleConnectionFactory = New RadicacionModuleConnectionFactory(session.CadenaConexionRadicacion)
        Dim executor As IDataExecutor = New AdoNetDataExecutor()
        Dim repository As IImportIntentRepository = New MySqlImportIntentRepository(connections, executor, New DbTransactionFactory())
        Dim validator = New ValidadorContextoImportacion(New MySqlSiiImportAuthorizationRepository(connections, executor, trustedContext))
        Dim clock As IImportacionServicioClock = New SystemImportacionServicioClock()
        Dim attempts As IExternalServiceAttemptRecorder = New MySqlExternalServiceTelemetryRepository(docuarchiConnections, executor, ModuleContext(trustedContext))
        Dim clients = BuildProviderRegistry(attempts)
        Dim documentTypes As IImportDocumentTypeResolver = New MySqlImportDocumentTypeResolver(radicacionConnections, executor)
        Dim steps As New Collections.Generic.List(Of IImportExecutionStep) From {
            New DownloadImportExecutionStep(clients), New PrepareImportExecutionStep(), New PrepareImportIndicesExecutionStep(),
            New StoreImportExecutionStep(New MySqlImportStorageMetadataRepository(connections, executor), documentTypes, New LegacyImportDocumentStorageAdapter()),
            New CompleteImportExecutionStep(FaseImportacionServicio.CacheActualizado), New CompleteImportExecutionStep(FaseImportacionServicio.Completada)}
        Return New ImportComposition With {
            .Preflight = New ServicioPreflightImportacion(validator, documentTypes),
            .Intents = New ServicioIntencionImportacion(repository, New MySqlImportIntentConcurrencyGuard(connections, executor), clock),
            .Orchestrator = New ImportServiceOrchestrator(validator, repository, New ImportIntentStateMachine(clock, New SafeImportIntentTransitionAudit()), steps),
            .Reconciliation = New ServicioReconciliacionImportacion(validator, New MySqlImportReconciliationRepository(connections, docuarchiConnections, executor), New ImportItemResultMapper())}
    End Function

    Private Shared Function BuildProviderRegistry(ByVal attempts As IExternalServiceAttemptRecorder) As RegistroClientesProveedoresImportacion
        Dim resolved = ResolveProvider(SiiImportProvider.CanonicalProviderId, attempts)
        If resolved Is Nothing OrElse Not resolved.Encontrado Then Throw New InvalidOperationException("PROVIDER_NOT_CONFIGURED")
        Return New RegistroClientesProveedoresImportacion(New IExternalImportProviderClient() {resolved.Cliente})
    End Function

    Private NotInheritable Class ImportComposition
        Public Property Preflight As ServicioPreflightImportacion
        Public Property Intents As ServicioIntencionImportacion
        Public Property Orchestrator As ImportServiceOrchestrator
        Public Property Reconciliation As ServicioReconciliacionImportacion
    End Class

    Private Shared Function ValidRequest(ByVal request As SolicitudImportacionServicioDto) As Boolean
        Return request IsNot Nothing AndAlso request.TaskId > 0 AndAlso
            Not String.IsNullOrWhiteSpace(request.OperationId) AndAlso Not String.IsNullOrWhiteSpace(request.CorrelationId) AndAlso
            String.Equals(request.ProviderId, SiiImportProvider.CanonicalProviderId, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ResolveProvider(ByVal providerId As String, Optional ByVal attempts As IExternalServiceAttemptRecorder = Nothing) As ResultadoResolucionClienteProveedorImportacion
        Dim baseUri As Uri = Nothing
        Dim companyCode As String = Nothing, serviceUser As String = Nothing, servicePassword As String = Nothing, baseUrl As String = Nothing
        Dim legacySettings As New Class_ws_usuarioworkflowsii()
        If legacySettings.solicita_usuario_validacion_sii(companyCode, serviceUser, servicePassword) <> "YES" Then Return ProviderConfigurationFailure("SII_CREDENTIAL_RECORD_UNAVAILABLE")
        If String.IsNullOrWhiteSpace(companyCode) OrElse String.IsNullOrWhiteSpace(serviceUser) OrElse String.IsNullOrWhiteSpace(servicePassword) Then Return ProviderConfigurationFailure("SII_CREDENTIAL_FIELDS_EMPTY")
        If legacySettings.Solicita_url_nombrefuncion_restfull(baseUrl, "solicitarToken") <> "YES" Then Return ProviderConfigurationFailure("SII_TOKEN_URL_RECORD_UNAVAILABLE")
        If Not Uri.TryCreate(baseUrl, UriKind.Absolute, baseUri) Then Return ProviderConfigurationFailure("SII_TOKEN_URL_INVALID")
        If Not String.Equals(baseUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) AndAlso
           Not String.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) Then Return ProviderConfigurationFailure("SII_TOKEN_URL_SCHEME_UNSUPPORTED")
        Dim factory As New ExternalImportHttpClientFactory(True)
        Dim transport As New ExternalImportHttpTransport(factory, New ExternalImportHttpResponseValidator(), New ExternalImportHttpErrorMapper())
        Dim allowedHosts = If(ConfigurationManager.AppSettings("ImportarServicioWebSiiDownloadHosts"), String.Empty).
            Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
        Dim recorder = If(attempts, CType(New NullExternalServiceAttemptRecorder(), IExternalServiceAttemptRecorder))
        Dim client As New SiiExternalImportProviderClient(transport, baseUri, companyCode, serviceUser, servicePassword,
            TimeSpan.FromSeconds(30), MaximumPreviewBytes(), allowedHosts, recorder)
        Dim registry As New RegistroClientesProveedoresImportacion(New IExternalImportProviderClient() {New SiiImportProvider(client, New SiiImportContractMapper())})
        Return registry.Resolver(providerId)
    End Function

    Private Shared Function CreateAttemptRecorder(ByVal session As ResultadoContextoSesionWorkflow, ByVal context As ContextoImportacionServicio) As IExternalServiceAttemptRecorder
        If session Is Nothing OrElse String.IsNullOrWhiteSpace(session.CadenaConexionDocuarchi) Then Return New NullExternalServiceAttemptRecorder()
        Return New MySqlExternalServiceTelemetryRepository(New DocuarchiModuleConnectionFactory(session.CadenaConexionDocuarchi), New AdoNetDataExecutor(), ModuleContext(context))
    End Function

    Private Shared Function ModuleContext(ByVal context As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=context.IdUsuario,.IdGrupo=context.IdGrupo,.LoginUsuario=context.LoginUsuario}
    End Function

    Private Shared Function ProviderConfigurationFailure(ByVal code As String) As ResultadoResolucionClienteProveedorImportacion
        Return New ResultadoResolucionClienteProveedorImportacion With {.Codigo = code, .MensajeVisible = "El proveedor solicitado no está disponible."}
    End Function

    Private Shared Function MaximumPreviewBytes() As Long
        Dim value As Long
        If Long.TryParse(ConfigurationManager.AppSettings("ImportarServicioWebPreviewMaximumBytes"), value) AndAlso value > 0 Then Return value
        Return 10485760L
    End Function

    Private Shared Function ErrorDto(ByVal code As String, Optional ByVal diagnostic As String = Nothing) As ErrorImportacionServicioDto
        Return New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = If(String.IsNullOrWhiteSpace(diagnostic), "La operación no está disponible.", diagnostic)}
    End Function

    Private Shared Function SafeExecutionDiagnostic(ByVal value As String) As String
        Dim diagnosticText As String = System.Text.RegularExpressions.Regex.Replace(If(value, String.Empty), "[\r\n\t]+", " ").Trim()
        If diagnosticText.Length = 0 Then Return "La ejecución falló sin informar detalle."
        diagnosticText = System.Text.RegularExpressions.Regex.Replace(diagnosticText,
            "(?i)\b(password|pwd|contrase(?:ña|na)|token|authorization|cookie|connection\s*string|cadena\s+de\s+conexi(?:ó|o)n)\b\s*[:=]\s*[^;,\s]+", "$1=[REDACTED]")
        diagnosticText = System.Text.RegularExpressions.Regex.Replace(diagnosticText, "(?i)(https?://[^?\s]+)\?[^\s]+", "$1?[REDACTED]")
        If diagnosticText.Length > 500 Then diagnosticText = diagnosticText.Substring(0, 500) & "…"
        Return diagnosticText
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

    Private Shared Function SafeExternalCode(ByVal exception As ExternalImportHttpException) As String
        If exception Is Nothing OrElse exception.ErrorDetail Is Nothing Then Return "EXTERNAL_PROVIDER_UNAVAILABLE"
        Dim code = exception.ErrorDetail.Code
        If code = "EXTERNAL_ACCESS_DENIED" OrElse code = "EXTERNAL_UNAVAILABLE" OrElse code = "EXTERNAL_INVALID_RESPONSE" OrElse
           code = "EXTERNAL_CANCELLED" OrElse code = "EXTERNAL_TIMEOUT" Then Return code
        Return "EXTERNAL_PROVIDER_UNAVAILABLE"
    End Function

    Private Shared Function SafeProviderCode(ByVal candidate As String) As String
        Dim code = If(candidate, String.Empty)
        If code.StartsWith("SII_RESOURCE_HOST_NOT_ALLOWED_", StringComparison.Ordinal) AndAlso code.Length <= 80 AndAlso
           Text.RegularExpressions.Regex.IsMatch(code, "^[A-Z0-9_]+$") Then Return code
        If code = "SII_RESOURCE_URL_INVALID" OrElse code = "SII_RESOURCE_SCHEME_NOT_ALLOWED" OrElse code = "SII_RESOURCE_HOST_NOT_ALLOWED" OrElse
           code = "SII_RESOURCE_FORMAT_UNSUPPORTED" OrElse code = "SII_ITEM_NOT_FOUND" OrElse
           code = "SII_TOKEN_INVALID" OrElse code = "SII_TOKEN_INVALID_CREDENTIALS" OrElse code = "SII_TOKEN_REJECTED" OrElse
           code = "SII_QUERY_PROVIDER_ERROR" OrElse code = "SII_QUERY_REJECTED" Then Return code
        Return "EXTERNAL_PROVIDER_UNAVAILABLE"
    End Function

    Private Shared Function SafeReconciliationCode(ByVal candidate As String) As String
        If candidate = "WORKFLOW_RECONCILIATION_UNAVAILABLE" OrElse candidate = "DOCUARCHI_RECONCILIATION_UNAVAILABLE" Then Return candidate
        Return "IMPORT_UNAVAILABLE"
    End Function
End Class

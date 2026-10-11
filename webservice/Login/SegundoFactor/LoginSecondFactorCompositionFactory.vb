Imports System
Imports System.Web

' Composition root exclusivo de Login 2FA. Resuelve Session una vez y entrega puertos tipados.
Public NotInheritable Class LoginSecondFactorCompositionFactory
    Private Sub New()
    End Sub

    Public Shared Function Create(ByVal requestContext As HttpContext,
                                  ByVal finalizer As ILegacyLoginFinalizer,
                                  ByVal internalUserId As Long,
                                  ByVal moduleType As String,
                                  ByVal normalizedLogin As String) As ISecondFactorAuthenticationService
        If requestContext Is Nothing OrElse requestContext.Session Is Nothing Then Throw New InvalidOperationException("SECOND_FACTOR_SESSION_INVALID")
        If finalizer Is Nothing Then Throw New ArgumentNullException(NameOf(finalizer))
        If internalUserId <= 0 OrElse internalUserId > Integer.MaxValue OrElse String.IsNullOrWhiteSpace(moduleType) OrElse String.IsNullOrWhiteSpace(normalizedLogin) Then Throw New InvalidOperationException("SECOND_FACTOR_IDENTITY_INVALID")

        Dim docuarchiConnection As String = ModuleSessionConnectionStringResolver.Resolve(requestContext, "DA_")
        Dim radicacionConnection As String = ModuleSessionConnectionStringResolver.Resolve(requestContext, "RA_")
        Dim workflowConnection As String = ModuleSessionConnectionStringResolver.Resolve(requestContext)
        If String.IsNullOrWhiteSpace(docuarchiConnection) OrElse String.IsNullOrWhiteSpace(radicacionConnection) OrElse String.IsNullOrWhiteSpace(workflowConnection) Then
            Throw New InvalidOperationException("SECOND_FACTOR_CONNECTION_UNAVAILABLE")
        End If

        Dim executor As IDataExecutor = New AdoNetDataExecutor()
        Dim clock As ISecondFactorClock = New SystemSecondFactorClock()
        Dim docuarchiFactory As IModuleConnectionFactory = New DocuarchiModuleConnectionFactory(docuarchiConnection)
        Dim radicacionFactory As IModuleConnectionFactory = New RadicacionModuleConnectionFactory(radicacionConnection)
        Dim workflowFactory As IModuleConnectionFactory = New WorkflowModuleConnectionFactory(workflowConnection)
        Dim principals As ISecondFactorPrincipalRepositoryResolver = New SecondFactorPrincipalRepositoryResolver(
            New MySqlDocuarchiSecondFactorPrincipalRepository(docuarchiFactory, executor),
            New MySqlGestorSecondFactorPrincipalRepository(New GestorModuleConnectionFactory(radicacionConnection), executor),
            New MySqlRadicacionSecondFactorPrincipalRepository(radicacionFactory, executor),
            New MySqlWorkflowSecondFactorPrincipalRepository(workflowFactory, executor))
        Dim moduleContext As New ContextoModulo With {
            .CodigoModulo = moduleType.Trim().ToUpperInvariant(),
            .IdUsuario = CInt(internalUserId),
            .IdGrupo = 0,
            .LoginUsuario = normalizedLogin.Trim().ToUpperInvariant()}
        If Not moduleContext.EsValido() Then Throw New InvalidOperationException("SECOND_FACTOR_IDENTITY_INVALID")

        Dim challenges As ISecondFactorChallengeRepository = New MySqlSecondFactorChallengeRepository(docuarchiFactory, executor, New DbTransactionFactory(), moduleContext, clock)
        Dim email As ISecondFactorEmailSender = New SecondFactorSmtpEmailSender(
            New MySqlSecondFactorSmtpConfigurationRepository(radicacionFactory, executor, moduleContext),
            New SecondFactorSmtpTransport(New FrameworkSmtpClientFactory()))
        Dim store As IPendingSecondFactorContextStore = New SessionPendingSecondFactorContextStore(New HttpSessionStateWrapper(requestContext.Session), clock)
        Dim diagnostics As ISecondFactorDiagnosticSink = New EventLogSecondFactorDiagnosticSink()

        Return New SecondFactorAuthenticationService(
            clock,
            New CryptographicSecondFactorOtpGenerator(),
            New HmacSecondFactorCodeProtector(New AppSettingsSecondFactorKeyProvider()),
            challenges,
            email,
            New SecondFactorRecipientResolver(principals),
            store,
            finalizer,
            New FormsAuthenticationTicketIssuer(),
            New LegacyLoginLocalRouteResolver(),
            diagnostics)
    End Function

    Public Shared Function CreateFromPending(ByVal requestContext As HttpContext,
                                             ByVal finalizer As ILegacyLoginFinalizer,
                                             ByVal pending As PendingSecondFactorContext) As ISecondFactorAuthenticationService
        If pending Is Nothing Then Throw New ArgumentNullException(NameOf(pending))
        Return Create(requestContext, finalizer, pending.InternalUserId, pending.ModuleType, pending.NormalizedLogin)
    End Function
End Class

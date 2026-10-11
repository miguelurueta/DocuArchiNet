Imports System

' Decide si el login legacy finaliza o se detiene antes de cualquier efecto autenticado.
Public NotInheritable Class SecondFactorPreAuthenticationService
    Implements ISecondFactorPreAuthenticationService

    Private ReadOnly _modules As ISecondFactorLoginModuleRepository
    Private ReadOnly _principals As ISecondFactorPrincipalRepositoryResolver
    Private ReadOnly _finalizer As ILegacyLoginFinalizer
    Private ReadOnly _authentication As ISecondFactorAuthenticationService
    Private ReadOnly _authenticationFactory As Func(Of SecondFactorLoginModule, SecondFactorPrincipal, ISecondFactorAuthenticationService)
    Private ReadOnly _diagnostics As ISecondFactorDiagnosticSink

    Public Sub New(ByVal modules As ISecondFactorLoginModuleRepository,
                   ByVal principals As ISecondFactorPrincipalRepositoryResolver,
                   ByVal finalizer As ILegacyLoginFinalizer)
        Me.New(modules, principals, finalizer, Nothing, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal modules As ISecondFactorLoginModuleRepository,
                   ByVal principals As ISecondFactorPrincipalRepositoryResolver,
                   ByVal finalizer As ILegacyLoginFinalizer,
                   ByVal authentication As ISecondFactorAuthenticationService)
        Me.New(modules, principals, finalizer, authentication, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal modules As ISecondFactorLoginModuleRepository,
                   ByVal principals As ISecondFactorPrincipalRepositoryResolver,
                   ByVal finalizer As ILegacyLoginFinalizer,
                   ByVal authentication As ISecondFactorAuthenticationService,
                   ByVal authenticationFactory As Func(Of SecondFactorLoginModule, SecondFactorPrincipal, ISecondFactorAuthenticationService))
        Me.New(modules, principals, finalizer, authentication, authenticationFactory, Nothing)
    End Sub

    Public Sub New(ByVal modules As ISecondFactorLoginModuleRepository,
                   ByVal principals As ISecondFactorPrincipalRepositoryResolver,
                   ByVal finalizer As ILegacyLoginFinalizer,
                   ByVal authentication As ISecondFactorAuthenticationService,
                   ByVal authenticationFactory As Func(Of SecondFactorLoginModule, SecondFactorPrincipal, ISecondFactorAuthenticationService),
                   ByVal diagnostics As ISecondFactorDiagnosticSink)
        If modules Is Nothing Then Throw New ArgumentNullException(NameOf(modules))
        If principals Is Nothing Then Throw New ArgumentNullException(NameOf(principals))
        If finalizer Is Nothing Then Throw New ArgumentNullException(NameOf(finalizer))
        _modules = modules
        _principals = principals
        _finalizer = finalizer
        _authentication = authentication
        _authenticationFactory = authenticationFactory
        _diagnostics = If(diagnostics, NullSecondFactorDiagnosticSink.Instance)
    End Sub

    Public Function Execute(ByVal request As SecondFactorPreAuthenticationRequest) As SecondFactorPreAuthenticationResult Implements ISecondFactorPreAuthenticationService.Execute
        If request Is Nothing Then Return Rejected("PREAUTHENTICATION_INVALID")
        Dim correlationId As Guid = Guid.NewGuid()
        Dim currentStage As SecondFactorDiagnosticStage = SecondFactorDiagnosticStage.PREAUTHENTICATION
        SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.STARTED)
        Try
            currentStage = SecondFactorDiagnosticStage.LOGIN_MODULE_RESOLUTION
            Dim loginModule As SecondFactorLoginModule = _modules.Resolve(request.CompanyName, request.ModuleName)
            If loginModule Is Nothing Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("LOGIN_MODULE_NOT_FOUND")
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            currentStage = SecondFactorDiagnosticStage.PRINCIPAL_REPOSITORY_RESOLUTION
            Dim repository As ISecondFactorPrincipalRepository = _principals.Resolve(loginModule.ModuleType)
            If repository Is Nothing Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("LOGIN_MODULE_UNSUPPORTED")
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            request.PrincipalContext.CodigoModulo = loginModule.ModuleType
            request.PrincipalContext.LoginUsuario = request.ValidatedLogin
            currentStage = SecondFactorDiagnosticStage.PRINCIPAL_RESOLUTION
            Dim principal As SecondFactorPrincipal = repository.Resolve(request.PrincipalContext)
            If principal Is Nothing Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("SECOND_FACTOR_PRINCIPAL_NOT_FOUND")
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            If loginModule.Configuration.IsRequired Then
                currentStage = SecondFactorDiagnosticStage.RECIPIENT_VALIDATION
                If String.IsNullOrWhiteSpace(principal.EmailAddress) Then
                    SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                    Return Rejected("SECOND_FACTOR_EMAIL_UNAVAILABLE")
                End If
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
                Dim authentication As ISecondFactorAuthenticationService = _authentication
                currentStage = SecondFactorDiagnosticStage.AUTHENTICATION_SERVICE_RESOLUTION
                If authentication Is Nothing AndAlso _authenticationFactory IsNot Nothing Then
                    authentication = _authenticationFactory(loginModule, principal)
                End If
                If authentication Is Nothing Then
                    SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                    Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.SECOND_FACTOR_REQUIRED,
                                                                   "SECOND_FACTOR_REQUIRED",
                                                                   loginModule,
                                                                   principal,
                                                                   Nothing)
                End If
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
                currentStage = SecondFactorDiagnosticStage.AUTHENTICATION_START
                Dim started As SecondFactorAuthenticationResult = authentication.Start(New SecondFactorAuthenticationStartRequest(loginModule, principal, correlationId))
                If started Is Nothing OrElse started.State <> SecondFactorAuthenticationState.PENDING Then
                    SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                    Return Rejected(If(started Is Nothing OrElse String.IsNullOrWhiteSpace(started.PublicCode), "SECOND_FACTOR_START_FAILED", started.PublicCode))
                End If
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
                Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.SECOND_FACTOR_REQUIRED,
                                                               started.PublicCode,
                                                               loginModule,
                                                               principal,
                                                               Nothing)
            End If

            Dim finalizationContext As New LegacyLoginFinalizationContext(loginModule.EmpresaId,
                                                                          loginModule.ModuloId,
                                                                          loginModule.ModuleType,
                                                                          principal.InternalUserId,
                                                                          principal.NormalizedLogin)
            Dim finalization As LegacyLoginFinalizationResult = _finalizer.FinalizeLogin(finalizationContext)
            If finalization Is Nothing OrElse Not finalization.Success Then
                Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.REJECTED,
                                                               If(finalization Is Nothing OrElse String.IsNullOrWhiteSpace(finalization.PublicCode), "LOGIN_FINALIZATION_FAILED", finalization.PublicCode),
                                                               loginModule,
                                                               principal,
                                                               finalization)
            End If
            Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.FINALIZED,
                                                           "FINALIZED",
                                                           loginModule,
                                                           principal,
                                                           finalization)
        Catch
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.FAILED)
            Return Rejected("PREAUTHENTICATION_FAILED")
        End Try
    End Function

    Private Sub SafeRecord(ByVal correlationId As Guid,
                           ByVal stage As SecondFactorDiagnosticStage,
                           ByVal outcome As SecondFactorDiagnosticOutcome)
        Try
            _diagnostics.Record(correlationId, stage, outcome)
        Catch
            ' La observabilidad nunca altera el login.
        End Try
    End Sub

    Private Shared Function Rejected(ByVal publicCode As String) As SecondFactorPreAuthenticationResult
        Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.REJECTED,
                                                       publicCode,
                                                       Nothing,
                                                       Nothing,
                                                       Nothing)
    End Function
End Class

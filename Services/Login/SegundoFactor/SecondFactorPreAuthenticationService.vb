Imports System

' Decide si el login legacy finaliza o se detiene antes de cualquier efecto autenticado.
Public NotInheritable Class SecondFactorPreAuthenticationService
    Implements ISecondFactorPreAuthenticationService

    Private ReadOnly _modules As ISecondFactorLoginModuleRepository
    Private ReadOnly _principals As ISecondFactorPrincipalRepositoryResolver
    Private ReadOnly _finalizer As ILegacyLoginFinalizer

    Public Sub New(ByVal modules As ISecondFactorLoginModuleRepository,
                   ByVal principals As ISecondFactorPrincipalRepositoryResolver,
                   ByVal finalizer As ILegacyLoginFinalizer)
        If modules Is Nothing Then Throw New ArgumentNullException(NameOf(modules))
        If principals Is Nothing Then Throw New ArgumentNullException(NameOf(principals))
        If finalizer Is Nothing Then Throw New ArgumentNullException(NameOf(finalizer))
        _modules = modules
        _principals = principals
        _finalizer = finalizer
    End Sub

    Public Function Execute(ByVal request As SecondFactorPreAuthenticationRequest) As SecondFactorPreAuthenticationResult Implements ISecondFactorPreAuthenticationService.Execute
        If request Is Nothing Then Return Rejected("PREAUTHENTICATION_INVALID")
        Try
            Dim loginModule As SecondFactorLoginModule = _modules.Resolve(request.CompanyName, request.ModuleName)
            If loginModule Is Nothing Then Return Rejected("LOGIN_MODULE_NOT_FOUND")

            Dim repository As ISecondFactorPrincipalRepository = _principals.Resolve(loginModule.ModuleType)
            If repository Is Nothing Then Return Rejected("LOGIN_MODULE_UNSUPPORTED")

            request.PrincipalContext.CodigoModulo = loginModule.ModuleType
            request.PrincipalContext.LoginUsuario = request.ValidatedLogin
            Dim principal As SecondFactorPrincipal = repository.Resolve(request.PrincipalContext)
            If principal Is Nothing Then Return Rejected("SECOND_FACTOR_PRINCIPAL_NOT_FOUND")

            If loginModule.Configuration.IsRequired Then
                If String.IsNullOrWhiteSpace(principal.EmailAddress) Then
                    Return Rejected("SECOND_FACTOR_EMAIL_UNAVAILABLE")
                End If
                Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.SECOND_FACTOR_REQUIRED,
                                                               "SECOND_FACTOR_REQUIRED",
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
            Return Rejected("PREAUTHENTICATION_FAILED")
        End Try
    End Function

    Private Shared Function Rejected(ByVal publicCode As String) As SecondFactorPreAuthenticationResult
        Return New SecondFactorPreAuthenticationResult(SecondFactorPreAuthenticationStatus.REJECTED,
                                                       publicCode,
                                                       Nothing,
                                                       Nothing,
                                                       Nothing)
    End Function
End Class

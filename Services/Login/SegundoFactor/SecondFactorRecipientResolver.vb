Imports System

Public NotInheritable Class SecondFactorRecipientResolver
    Implements ISecondFactorRecipientResolver

    Private ReadOnly _principals As ISecondFactorPrincipalRepositoryResolver

    Public Sub New(ByVal principals As ISecondFactorPrincipalRepositoryResolver)
        If principals Is Nothing Then Throw New ArgumentNullException(NameOf(principals))
        _principals = principals
    End Sub

    Public Function Resolve(ByVal identity As SegundoFactorIdentity) As SecondFactorRecipient Implements ISecondFactorRecipientResolver.Resolve
        If identity Is Nothing Then Return Nothing
        Dim repository As ISecondFactorPrincipalRepository = _principals.Resolve(identity.TipoUsuario)
        If repository Is Nothing Then Return Nothing
        Dim principal As SecondFactorPrincipal = repository.Resolve(New ContextoPreautenticacionModulo With {
            .CodigoModulo = identity.TipoUsuario,
            .IdUsuario = 0,
            .IdGrupo = 0,
            .LoginUsuario = identity.LoginNormalizado})
        If principal Is Nothing OrElse principal.InternalUserId.ToString(Globalization.CultureInfo.InvariantCulture) <> identity.IdInterno OrElse String.IsNullOrWhiteSpace(principal.EmailAddress) Then Return Nothing
        Return New SecondFactorRecipient(principal.EmailAddress, principal.MaskedDestination)
    End Function
End Class

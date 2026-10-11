Imports System
Imports System.Web.Security

Public NotInheritable Class FormsAuthenticationTicketIssuer
    Implements ISecondFactorAuthenticationTicketIssuer

    Private ReadOnly _issueCookie As Action(Of String, Boolean)

    Public Sub New()
        Me.New(AddressOf FormsAuthentication.SetAuthCookie)
    End Sub

    Public Sub New(ByVal issueCookie As Action(Of String, Boolean))
        If issueCookie Is Nothing Then Throw New ArgumentNullException(NameOf(issueCookie))
        _issueCookie = issueCookie
    End Sub

    Public Sub Issue(ByVal normalizedLogin As String) Implements ISecondFactorAuthenticationTicketIssuer.Issue
        If String.IsNullOrWhiteSpace(normalizedLogin) Then Throw New ArgumentException("El login es obligatorio.", NameOf(normalizedLogin))
        _issueCookie(normalizedLogin.Trim().ToUpperInvariant(), False)
    End Sub
End Class

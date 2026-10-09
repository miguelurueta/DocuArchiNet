Imports System
Imports System.Net
Imports System.Net.Mail

Public Interface ISecondFactorSmtpClient
    Inherits IDisposable
    Sub Send(ByVal message As MailMessage)
End Interface

Public Interface ISecondFactorSmtpClientFactory
    Function Create(ByVal configuration As SecondFactorSmtpConfiguration) As ISecondFactorSmtpClient
End Interface

Public NotInheritable Class FrameworkSmtpClientFactory
    Implements ISecondFactorSmtpClientFactory

    Public Function Create(ByVal configuration As SecondFactorSmtpConfiguration) As ISecondFactorSmtpClient Implements ISecondFactorSmtpClientFactory.Create
        If configuration Is Nothing Then Throw New ArgumentNullException(NameOf(configuration))
        Dim client As New SmtpClient(configuration.Host, configuration.Port) With {
            .Timeout = configuration.TimeoutMilliseconds,
            .EnableSsl = configuration.EnableSsl,
            .UseDefaultCredentials = configuration.UseDefaultCredentials}
        If Not configuration.UseDefaultCredentials Then
            client.Credentials = New NetworkCredential(configuration.Username, configuration.Password)
        End If
        Return New FrameworkSmtpClientAdapter(client)
    End Function
End Class

Public NotInheritable Class FrameworkSmtpClientAdapter
    Implements ISecondFactorSmtpClient

    Private ReadOnly _client As SmtpClient
    Private _disposed As Boolean

    Public Sub New(ByVal client As SmtpClient)
        If client Is Nothing Then Throw New ArgumentNullException(NameOf(client))
        _client = client
    End Sub

    Public Sub Send(ByVal message As MailMessage) Implements ISecondFactorSmtpClient.Send
        If _disposed Then Throw New ObjectDisposedException(NameOf(FrameworkSmtpClientAdapter))
        If message Is Nothing Then Throw New ArgumentNullException(NameOf(message))
        _client.Send(message)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        _disposed = True
        _client.Dispose()
    End Sub
End Class

Imports System
Imports System.Globalization
Imports System.Net.Mail
Imports System.Text

Public NotInheritable Class SecondFactorSmtpTransport
    Implements ISecondFactorSmtpTransport

    Private Const Subject As String = "Código de verificación"
    Private ReadOnly _clients As ISecondFactorSmtpClientFactory

    Public Sub New(ByVal clients As ISecondFactorSmtpClientFactory)
        If clients Is Nothing Then Throw New ArgumentNullException(NameOf(clients))
        _clients = clients
    End Sub

    Public Function Send(ByVal configuration As SecondFactorSmtpConfiguration,
                         ByVal message As SecondFactorEmailMessage) As SecondFactorSmtpDelivery Implements ISecondFactorSmtpTransport.Send
        If configuration Is Nothing OrElse message Is Nothing Then Return FailedDelivery()
        Try
            Using mail As MailMessage = CreateMessage(configuration, message)
                Using client As ISecondFactorSmtpClient = _clients.Create(configuration)
                    client.Send(mail)
                End Using
            End Using
            Return New SecondFactorSmtpDelivery(SecondFactorSmtpDeliveryStatus.Submitted, "OTP_SUBMITTED")
        Catch
            Return FailedDelivery()
        End Try
    End Function

    Private Shared Function CreateMessage(ByVal configuration As SecondFactorSmtpConfiguration,
                                          ByVal message As SecondFactorEmailMessage) As MailMessage
        Dim mail As New MailMessage()
        Try
            mail.From = New MailAddress(configuration.SenderAddress)
            mail.To.Add(New MailAddress(message.Recipient.EmailAddress))
            mail.Subject = Subject
            mail.SubjectEncoding = Encoding.UTF8
            mail.BodyEncoding = Encoding.UTF8
            mail.IsBodyHtml = configuration.IsBodyHtml
            mail.Body = BuildBody(message, configuration.IsBodyHtml)
            Return mail
        Catch
            mail.Dispose()
            Throw
        End Try
    End Function

    Private Shared Function BuildBody(ByVal message As SecondFactorEmailMessage,
                                      ByVal isHtml As Boolean) As String
        Dim expiration As String = message.ExpiresAtUtc.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture)
        If isHtml Then
            Return "<p>Su código de verificación es <strong>" & message.Code & "</strong>.</p>" &
                   "<p>Expira: " & expiration & ".</p>" &
                   "<p>No comparta este código. Si no inició este acceso, ignore este mensaje.</p>"
        End If
        Return "Su código de verificación es " & message.Code & "." & Environment.NewLine &
               "Expira: " & expiration & "." & Environment.NewLine &
               "No comparta este código. Si no inició este acceso, ignore este mensaje."
    End Function

    Private Shared Function FailedDelivery() As SecondFactorSmtpDelivery
        Return New SecondFactorSmtpDelivery(SecondFactorSmtpDeliveryStatus.Failed, "OTP_DELIVERY_FAILED")
    End Function
End Class

Imports System

Public NotInheritable Class SecondFactorSmtpEmailSender
    Implements ISecondFactorEmailSender

    Private ReadOnly _configurations As ISecondFactorSmtpConfigurationRepository
    Private ReadOnly _transport As ISecondFactorSmtpTransport

    Public Sub New(ByVal configurations As ISecondFactorSmtpConfigurationRepository,
                   ByVal transport As ISecondFactorSmtpTransport)
        If configurations Is Nothing Then Throw New ArgumentNullException(NameOf(configurations))
        If transport Is Nothing Then Throw New ArgumentNullException(NameOf(transport))
        _configurations = configurations
        _transport = transport
    End Sub

    Public Function Send(ByVal message As SecondFactorEmailMessage) As SecondFactorDeliveryResult Implements ISecondFactorEmailSender.Send
        Try
            If message Is Nothing Then Return Result(False, "OTP_DELIVERY_FAILED")
            Dim resolution As SecondFactorSmtpConfigurationResolution = _configurations.Resolve()
            If resolution Is Nothing Then Return Result(False, "OTP_DELIVERY_FAILED")

            Select Case resolution.Status
                Case SecondFactorSmtpDeliveryStatus.Disabled
                    Return Result(False, "OTP_DISABLED")
                Case SecondFactorSmtpDeliveryStatus.InvalidConfiguration
                    Return Result(False, "OTP_CONFIGURATION_INVALID")
                Case SecondFactorSmtpDeliveryStatus.AmbiguousConfiguration
                    Return Result(False, "OTP_CONFIGURATION_AMBIGUOUS")
                Case SecondFactorSmtpDeliveryStatus.Submitted
                    Dim delivery As SecondFactorSmtpDelivery = _transport.Send(resolution.Configuration, message)
                    If delivery IsNot Nothing AndAlso delivery.Status = SecondFactorSmtpDeliveryStatus.Submitted Then
                        Return Result(True, "OTP_SUBMITTED")
                    End If
                    Return Result(False, "OTP_DELIVERY_FAILED")
                Case Else
                    Return Result(False, "OTP_DELIVERY_FAILED")
            End Select
        Catch
            Return Result(False, "OTP_DELIVERY_FAILED")
        End Try
    End Function

    Private Shared Function Result(ByVal success As Boolean, ByVal publicCode As String) As SecondFactorDeliveryResult
        Return New SecondFactorDeliveryResult With {.Success = success, .PublicCode = publicCode}
    End Function
End Class

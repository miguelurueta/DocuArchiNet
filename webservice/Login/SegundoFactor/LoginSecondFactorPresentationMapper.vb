Imports System

Public NotInheritable Class LoginSecondFactorPresentationMapper
    Public Function MapState(ByVal result As SecondFactorAuthenticationResult) As SegundoFactorEstadoDto
        If result Is Nothing Then Return EmptyState()
        Return New SegundoFactorEstadoDto With {
            .RequiereCodigo = result.State = SecondFactorAuthenticationState.PENDING,
            .EstadoPublico = PublicState(result.State),
            .DestinoEnmascarado = result.MaskedDestination,
            .ExpiraEnSegundos = result.ExpiresInSeconds,
            .ReenvioDisponibleEnSegundos = result.ResendAvailableInSeconds}
    End Function

    Public Function MapResult(ByVal result As SecondFactorAuthenticationResult) As SegundoFactorResultadoDto
        If result Is Nothing Then Return Failure("SECOND_FACTOR_UNAVAILABLE")
        Dim code As String = NormalizePublicCode(result.PublicCode)
        Return New SegundoFactorResultadoDto With {
            .Exito = result.Success,
            .Codigo = code,
            .MensajeVisible = PublicMessage(code),
            .Estado = MapState(result),
            .RutaLocal = If(result.Success AndAlso result.State = SecondFactorAuthenticationState.COMPLETED, result.LocalRoute, String.Empty)}
    End Function

    Public Function Failure(ByVal publicCode As String) As SegundoFactorResultadoDto
        Dim code As String = NormalizePublicCode(publicCode)
        Return New SegundoFactorResultadoDto With {.Exito = False, .Codigo = code, .MensajeVisible = PublicMessage(code), .Estado = EmptyState(), .RutaLocal = String.Empty}
    End Function

    Private Shared Function NormalizePublicCode(ByVal publicCode As String) As String
        Dim code As String = If(publicCode, String.Empty).Trim().ToUpperInvariant()
        Select Case code
            Case "SECOND_FACTOR_REQUIRED", "SECOND_FACTOR_PENDING", "SECOND_FACTOR_RESENT",
                 "SECOND_FACTOR_RESEND_COOLDOWN", "SECOND_FACTOR_RESEND_LIMIT",
                 "SECOND_FACTOR_CODE_INVALID", "SECOND_FACTOR_COMPLETED", "SECOND_FACTOR_CANCELLED",
                 "SECOND_FACTOR_UNAVAILABLE", "SECOND_FACTOR_SESSION_INVALID",
                 "SECOND_FACTOR_DELIVERY_FAILED", "SECOND_FACTOR_START_FAILED",
                 "SECOND_FACTOR_VERIFICATION_FAILED", "SECOND_FACTOR_RESEND_FAILED",
                 "SECOND_FACTOR_EMAIL_UNAVAILABLE", "SECOND_FACTOR_BLOCKED",
                 "LOGIN_FINALIZATION_FAILED", "PREAUTHENTICATION_FAILED"
                Return code
            Case Else
                Return "SECOND_FACTOR_UNAVAILABLE"
        End Select
    End Function

    Private Shared Function EmptyState() As SegundoFactorEstadoDto
        Return New SegundoFactorEstadoDto With {.RequiereCodigo = False, .EstadoPublico = "NO_DISPONIBLE", .DestinoEnmascarado = String.Empty, .ExpiraEnSegundos = 0, .ReenvioDisponibleEnSegundos = 0}
    End Function

    Private Shared Function PublicState(ByVal state As SecondFactorAuthenticationState) As String
        Select Case state
            Case SecondFactorAuthenticationState.PENDING : Return "PENDIENTE"
            Case SecondFactorAuthenticationState.COMPLETED : Return "COMPLETADO"
            Case Else : Return "NO_DISPONIBLE"
        End Select
    End Function

    Private Shared Function PublicMessage(ByVal code As String) As String
        Select Case If(code, String.Empty).Trim().ToUpperInvariant()
            Case "SECOND_FACTOR_REQUIRED", "SECOND_FACTOR_PENDING" : Return "Ingrese el código enviado a su correo."
            Case "SECOND_FACTOR_RESENT" : Return "Se envió un código nuevo."
            Case "SECOND_FACTOR_RESEND_COOLDOWN" : Return "Espere antes de solicitar un código nuevo."
            Case "SECOND_FACTOR_RESEND_LIMIT" : Return "Alcanzó el límite de reenvíos. Reinicie el ingreso."
            Case "SECOND_FACTOR_CODE_INVALID" : Return "El código no es válido."
            Case "SECOND_FACTOR_SESSION_INVALID" : Return "La verificación ya no está disponible. Inicie sesión nuevamente."
            Case "SECOND_FACTOR_DELIVERY_FAILED" : Return "No fue posible enviar el código de verificación. Intente nuevamente."
            Case "SECOND_FACTOR_START_FAILED", "PREAUTHENTICATION_FAILED" : Return "No fue posible iniciar la verificación. Inicie sesión nuevamente."
            Case "SECOND_FACTOR_VERIFICATION_FAILED" : Return "No fue posible validar el código. Inicie sesión nuevamente."
            Case "SECOND_FACTOR_RESEND_FAILED" : Return "No fue posible reenviar el código. Inicie sesión nuevamente."
            Case "SECOND_FACTOR_EMAIL_UNAVAILABLE" : Return "No hay un correo válido disponible para la verificación."
            Case "SECOND_FACTOR_BLOCKED" : Return "Se agotaron los intentos permitidos. Inicie sesión nuevamente."
            Case "LOGIN_FINALIZATION_FAILED" : Return "El código fue validado, pero no fue posible completar el ingreso. Inicie sesión nuevamente."
            Case "SECOND_FACTOR_COMPLETED" : Return "Verificación completada."
            Case "SECOND_FACTOR_CANCELLED" : Return "Verificación cancelada."
            Case Else : Return "No fue posible completar la verificación. Reinicie el ingreso."
        End Select
    End Function
End Class

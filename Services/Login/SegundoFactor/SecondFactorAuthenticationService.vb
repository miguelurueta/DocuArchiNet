Imports System

' Orquesta Login 2FA sin conocer HTTP, Session, Forms Authentication, SQL ni SMTP concreto.
Public NotInheritable Class SecondFactorAuthenticationService
    Implements ISecondFactorAuthenticationService

    Private ReadOnly _clock As ISecondFactorClock
    Private ReadOnly _otp As ISecondFactorOtpGenerator
    Private ReadOnly _protector As ISecondFactorCodeProtector
    Private ReadOnly _challenges As ISecondFactorChallengeRepository
    Private ReadOnly _email As ISecondFactorEmailSender
    Private ReadOnly _recipients As ISecondFactorRecipientResolver
    Private ReadOnly _contexts As IPendingSecondFactorContextStore
    Private ReadOnly _finalizer As ILegacyLoginFinalizer
    Private ReadOnly _tickets As ISecondFactorAuthenticationTicketIssuer
    Private ReadOnly _routes As ILoginLocalRouteResolver
    Private ReadOnly _diagnostics As ISecondFactorDiagnosticSink

    Public Sub New(ByVal clock As ISecondFactorClock,
                   ByVal otp As ISecondFactorOtpGenerator,
                   ByVal protector As ISecondFactorCodeProtector,
                   ByVal challenges As ISecondFactorChallengeRepository,
                   ByVal email As ISecondFactorEmailSender,
                   ByVal recipients As ISecondFactorRecipientResolver,
                   ByVal contexts As IPendingSecondFactorContextStore,
                   ByVal finalizer As ILegacyLoginFinalizer,
                   ByVal tickets As ISecondFactorAuthenticationTicketIssuer,
                   ByVal routes As ILoginLocalRouteResolver)
        Me.New(clock, otp, protector, challenges, email, recipients, contexts, finalizer, tickets, routes, Nothing)
    End Sub

    Public Sub New(ByVal clock As ISecondFactorClock,
                   ByVal otp As ISecondFactorOtpGenerator,
                   ByVal protector As ISecondFactorCodeProtector,
                   ByVal challenges As ISecondFactorChallengeRepository,
                   ByVal email As ISecondFactorEmailSender,
                   ByVal recipients As ISecondFactorRecipientResolver,
                   ByVal contexts As IPendingSecondFactorContextStore,
                   ByVal finalizer As ILegacyLoginFinalizer,
                   ByVal tickets As ISecondFactorAuthenticationTicketIssuer,
                   ByVal routes As ILoginLocalRouteResolver,
                   ByVal diagnostics As ISecondFactorDiagnosticSink)
        If clock Is Nothing Then Throw New ArgumentNullException(NameOf(clock))
        If otp Is Nothing Then Throw New ArgumentNullException(NameOf(otp))
        If protector Is Nothing Then Throw New ArgumentNullException(NameOf(protector))
        If challenges Is Nothing Then Throw New ArgumentNullException(NameOf(challenges))
        If email Is Nothing Then Throw New ArgumentNullException(NameOf(email))
        If recipients Is Nothing Then Throw New ArgumentNullException(NameOf(recipients))
        If contexts Is Nothing Then Throw New ArgumentNullException(NameOf(contexts))
        If finalizer Is Nothing Then Throw New ArgumentNullException(NameOf(finalizer))
        If tickets Is Nothing Then Throw New ArgumentNullException(NameOf(tickets))
        If routes Is Nothing Then Throw New ArgumentNullException(NameOf(routes))
        _clock = clock
        _otp = otp
        _protector = protector
        _challenges = challenges
        _email = email
        _recipients = recipients
        _contexts = contexts
        _finalizer = finalizer
        _tickets = tickets
        _routes = routes
        _diagnostics = If(diagnostics, NullSecondFactorDiagnosticSink.Instance)
    End Sub

    Public Function Start(ByVal request As SecondFactorAuthenticationStartRequest) As SecondFactorAuthenticationResult Implements ISecondFactorAuthenticationService.Start
        If request Is Nothing OrElse Not request.LoginModule.Configuration.IsRequired Then Return Rejected("SECOND_FACTOR_UNAVAILABLE")
        Dim challengeId As Guid = Guid.Empty
        Dim correlationId As Guid = request.CorrelationId
        Dim currentStage As SecondFactorDiagnosticStage = SecondFactorDiagnosticStage.AUTHENTICATION_START
        SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.STARTED)
        Try
            currentStage = SecondFactorDiagnosticStage.IDENTITY_BUILD
            Dim identity As SegundoFactorIdentity = BuildIdentity(request.LoginModule, request.Principal)
            Dim recipient As New SecondFactorRecipient(request.Principal.EmailAddress, request.Principal.MaskedDestination)
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Dim now As DateTime = _clock.UtcNow
            Dim expiresAt As DateTime = now.AddMinutes(request.LoginModule.Configuration.ExpirationMinutes)
            challengeId = Guid.NewGuid()
            Dim nonce As String = Guid.NewGuid().ToString("N")
            Dim code As String = _otp.GenerateCode()
            Dim protection As New SecondFactorProtectionContext(SegundoFactorPurpose.LOGIN, challengeId, identity.ClaveCanonica, nonce)
            currentStage = SecondFactorDiagnosticStage.CODE_PROTECTION
            Dim protectedCode As String = _protector.Protect(protection, code)
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Dim challenge As New SegundoFactorChallenge(challengeId, identity, SegundoFactorPurpose.LOGIN, SegundoFactorChallengeState.CREATED, 0, 0, now, expiresAt)
            currentStage = SecondFactorDiagnosticStage.CHALLENGE_CREATION
            If Not _challenges.Create(challenge, protectedCode, nonce) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return DeliveryFailed(challengeId, now, correlationId)
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            currentStage = SecondFactorDiagnosticStage.EMAIL_DELIVERY
            Dim delivery As SecondFactorDeliveryResult = _email.Send(New SecondFactorEmailMessage(recipient, code, expiresAt))
            If delivery Is Nothing OrElse Not delivery.Success Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                SafeRecordDeliveryDetail(correlationId, delivery)
                Return DeliveryFailed(challengeId, _clock.UtcNow, correlationId)
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            currentStage = SecondFactorDiagnosticStage.CHALLENGE_SENT_CONFIRMATION
            If Not _challenges.MarkSent(challengeId, _clock.UtcNow) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return DeliveryFailed(challengeId, _clock.UtcNow, correlationId)
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            Dim pending As New PendingSecondFactorContext(request.LoginModule.EmpresaId,
                                                           request.LoginModule.ModuloId,
                                                           request.LoginModule.ModuleType,
                                                           request.Principal.InternalUserId,
                                                           request.Principal.NormalizedLogin,
                                                           request.Principal.MaskedDestination,
                                                           challengeId,
                                                           nonce,
                                                           0,
                                                           now,
                                                           expiresAt)
            currentStage = SecondFactorDiagnosticStage.PENDING_CONTEXT_SAVE
            _contexts.Save(pending)
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_START, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Return PendingResult("SECOND_FACTOR_REQUIRED", pending)
        Catch
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.FAILED)
            If challengeId <> Guid.Empty Then SafeMarkDeliveryFailed(challengeId, correlationId)
            _contexts.Clear()
            Return Rejected("SECOND_FACTOR_START_FAILED")
        End Try
    End Function

    Public Function GetStatus() As SecondFactorAuthenticationResult Implements ISecondFactorAuthenticationService.GetStatus
        Dim correlationId As Guid = Guid.NewGuid()
        SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_STATUS, SecondFactorDiagnosticOutcome.STARTED)
        Try
            Dim pending As PendingSecondFactorContext = _contexts.GetCurrent()
            If pending Is Nothing OrElse String.IsNullOrWhiteSpace(pending.ModuleType) Then
                SafeRecord(correlationId, SecondFactorDiagnosticStage.SESSION_CONTEXT_RESOLUTION, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("SECOND_FACTOR_SESSION_INVALID")
            End If
            SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_STATUS, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Return PendingResult("SECOND_FACTOR_PENDING", pending)
        Catch
            SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_STATUS, SecondFactorDiagnosticOutcome.FAILED)
            _contexts.Clear()
            Return Rejected("SECOND_FACTOR_SESSION_INVALID")
        End Try
    End Function

    Public Function Verify(ByVal code As String) As SecondFactorAuthenticationResult Implements ISecondFactorAuthenticationService.Verify
        Dim correlationId As Guid = Guid.NewGuid()
        Dim currentStage As SecondFactorDiagnosticStage = SecondFactorDiagnosticStage.AUTHENTICATION_VERIFY
        SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.STARTED)
        If Not SecondFactorContractValidation.IsSixDigitCode(code) Then
            SafeRecord(correlationId, SecondFactorDiagnosticStage.CODE_VALIDATION, SecondFactorDiagnosticOutcome.REJECTED)
            Return Rejected("SECOND_FACTOR_CODE_INVALID")
        End If
        Dim pending As PendingSecondFactorContext = Nothing
        Try
            currentStage = SecondFactorDiagnosticStage.SESSION_CONTEXT_RESOLUTION
            pending = _contexts.GetCurrent()
            If pending Is Nothing OrElse String.IsNullOrWhiteSpace(pending.ModuleType) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return InvalidSession()
            End If
            Dim identity As SegundoFactorIdentity = BuildIdentity(pending)
            currentStage = SecondFactorDiagnosticStage.CHALLENGE_LOOKUP
            Dim data As SecondFactorChallengeVerificationData = _challenges.GetVerificationData(pending.ChallengeId, pending.SessionNonce)
            If data Is Nothing OrElse Not String.Equals(data.Challenge.CanonicalIdentity, identity.ClaveCanonica, StringComparison.Ordinal) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return InvalidSession()
            End If

            Dim protection As New SecondFactorProtectionContext(SegundoFactorPurpose.LOGIN, pending.ChallengeId, identity.ClaveCanonica, pending.SessionNonce)
            currentStage = SecondFactorDiagnosticStage.CODE_VERIFICATION
            If Not _protector.Verify(protection, code, data.ProtectedCode) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                currentStage = SecondFactorDiagnosticStage.FAILED_ATTEMPT_REGISTRATION
                Dim failed As SecondFactorStoredChallenge = _challenges.RegisterFailedAttemptData(pending.ChallengeId, data.Challenge.Attempts)
                If failed Is Nothing OrElse failed.Attempts >= SegundoFactorConfiguration.MaxAttempts Then
                    _contexts.Clear()
                    SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                    Return Rejected("SECOND_FACTOR_BLOCKED")
                End If
                Return PendingResult("SECOND_FACTOR_CODE_INVALID", pending)
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            currentStage = SecondFactorDiagnosticStage.FINALIZATION_ACQUISITION
            If Not _challenges.TryBeginFinalization(pending.ChallengeId, data.Challenge.Attempts) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return InvalidSession()
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            currentStage = SecondFactorDiagnosticStage.LEGACY_FINALIZATION
            Dim finalization As LegacyLoginFinalizationResult = _finalizer.FinalizeLogin(New LegacyLoginFinalizationContext(pending.EmpresaId, pending.ModuloId, pending.ModuleType, pending.InternalUserId, pending.NormalizedLogin))
            If finalization Is Nothing OrElse Not finalization.Success Then
                _challenges.FailFinalization(pending.ChallengeId)
                _contexts.Clear()
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("LOGIN_FINALIZATION_FAILED")
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            currentStage = SecondFactorDiagnosticStage.CHALLENGE_COMPLETION
            If Not _challenges.Complete(pending.ChallengeId) Then
                _contexts.Clear()
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return Rejected("LOGIN_FINALIZATION_FAILED")
            End If
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)

            currentStage = SecondFactorDiagnosticStage.LOCAL_ROUTE_RESOLUTION
            Dim localRoute As String = _routes.Resolve()
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            currentStage = SecondFactorDiagnosticStage.TICKET_ISSUANCE
            _tickets.Issue(pending.NormalizedLogin)
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.SUCCEEDED)
            _contexts.Clear()
            SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_VERIFY, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Return New SecondFactorAuthenticationResult(True, "SECOND_FACTOR_COMPLETED", SecondFactorAuthenticationState.COMPLETED, String.Empty, 0, 0, localRoute)
        Catch
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.FAILED)
            If pending IsNot Nothing Then SafeFailFinalization(pending.ChallengeId, correlationId)
            _contexts.Clear()
            Return Rejected("SECOND_FACTOR_VERIFICATION_FAILED")
        End Try
    End Function

    Public Function Resend() As SecondFactorAuthenticationResult Implements ISecondFactorAuthenticationService.Resend
        Dim replacementId As Guid = Guid.Empty
        Dim correlationId As Guid = Guid.NewGuid()
        Dim currentStage As SecondFactorDiagnosticStage = SecondFactorDiagnosticStage.AUTHENTICATION_RESEND
        SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.STARTED)
        Try
            currentStage = SecondFactorDiagnosticStage.SESSION_CONTEXT_RESOLUTION
            Dim pending As PendingSecondFactorContext = _contexts.GetCurrent()
            If pending Is Nothing OrElse String.IsNullOrWhiteSpace(pending.ModuleType) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return InvalidSession()
            End If
            Dim now As DateTime = _clock.UtcNow
            Dim availableAt As DateTime = pending.CreatedAtUtc.AddSeconds(SegundoFactorConfiguration.ResendCooldownSeconds)
            If now < availableAt Then Return PendingResult("SECOND_FACTOR_RESEND_COOLDOWN", pending)
            If pending.ResendCount >= SegundoFactorConfiguration.MaxResends Then Return PendingResult("SECOND_FACTOR_RESEND_LIMIT", pending)

            Dim identity As SegundoFactorIdentity = BuildIdentity(pending)
            currentStage = SecondFactorDiagnosticStage.RECIPIENT_RESOLUTION
            Dim recipient As SecondFactorRecipient = _recipients.Resolve(identity)
            If recipient Is Nothing Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return RejectedAndClear("SECOND_FACTOR_EMAIL_UNAVAILABLE")
            End If
            Dim lifetime As TimeSpan = pending.ExpiresAtUtc.Subtract(pending.CreatedAtUtc)
            Dim expiresAt As DateTime = now.Add(lifetime)
            replacementId = Guid.NewGuid()
            Dim nonce As String = Guid.NewGuid().ToString("N")
            Dim code As String = _otp.GenerateCode()
            Dim protection As New SecondFactorProtectionContext(SegundoFactorPurpose.LOGIN, replacementId, identity.ClaveCanonica, nonce)
            Dim replacement As New SegundoFactorChallenge(replacementId, identity, SegundoFactorPurpose.LOGIN, SegundoFactorChallengeState.CREATED, 0, pending.ResendCount + 1, now, expiresAt)
            currentStage = SecondFactorDiagnosticStage.CHALLENGE_REPLACEMENT
            If Not _challenges.ReplaceForResend(pending.ChallengeId, replacement, _protector.Protect(protection, code), nonce, now) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                Return InvalidSession()
            End If

            currentStage = SecondFactorDiagnosticStage.EMAIL_DELIVERY
            Dim delivery As SecondFactorDeliveryResult = _email.Send(New SecondFactorEmailMessage(recipient, code, expiresAt))
            If delivery Is Nothing OrElse Not delivery.Success OrElse Not _challenges.MarkSent(replacementId, _clock.UtcNow) Then
                SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.REJECTED)
                SafeRecordDeliveryDetail(correlationId, delivery)
                Return DeliveryFailed(replacementId, _clock.UtcNow, correlationId)
            End If
            Dim nextContext As New PendingSecondFactorContext(pending.EmpresaId, pending.ModuloId, pending.ModuleType, pending.InternalUserId, pending.NormalizedLogin, recipient.MaskedDestination, replacementId, nonce, pending.ResendCount + 1, now, expiresAt)
            _contexts.Save(nextContext)
            SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_RESEND, SecondFactorDiagnosticOutcome.SUCCEEDED)
            Return PendingResult("SECOND_FACTOR_RESENT", nextContext)
        Catch
            SafeRecord(correlationId, currentStage, SecondFactorDiagnosticOutcome.FAILED)
            If replacementId <> Guid.Empty Then SafeMarkDeliveryFailed(replacementId, correlationId)
            _contexts.Clear()
            Return Rejected("SECOND_FACTOR_RESEND_FAILED")
        End Try
    End Function

    Public Function Cancel() As SecondFactorAuthenticationResult Implements ISecondFactorAuthenticationService.Cancel
        Dim correlationId As Guid = Guid.NewGuid()
        SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_CANCEL, SecondFactorDiagnosticOutcome.STARTED)
        Try
            Dim pending As PendingSecondFactorContext = _contexts.GetCurrent()
            If pending IsNot Nothing AndAlso Not _challenges.Revoke(pending.ChallengeId) Then
                SafeRecord(correlationId, SecondFactorDiagnosticStage.CHALLENGE_REVOCATION, SecondFactorDiagnosticOutcome.REJECTED)
            Else
                SafeRecord(correlationId, SecondFactorDiagnosticStage.AUTHENTICATION_CANCEL, SecondFactorDiagnosticOutcome.SUCCEEDED)
            End If
        Catch
            ' Cancelar permanece idempotente y nunca expone el detalle interno.
            SafeRecord(correlationId, SecondFactorDiagnosticStage.CHALLENGE_REVOCATION, SecondFactorDiagnosticOutcome.FAILED)
        Finally
            _contexts.Clear()
        End Try
        Return New SecondFactorAuthenticationResult(True, "SECOND_FACTOR_CANCELLED", SecondFactorAuthenticationState.NONE, String.Empty, 0, 0, String.Empty)
    End Function

    Private Function DeliveryFailed(ByVal challengeId As Guid,
                                    ByVal observedAtUtc As DateTime,
                                    ByVal correlationId As Guid) As SecondFactorAuthenticationResult
        Try
            If Not _challenges.MarkDeliveryFailed(challengeId, observedAtUtc) Then
                SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.REJECTED)
            End If
        Catch
            SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.FAILED)
        End Try
        _contexts.Clear()
        Return Rejected("SECOND_FACTOR_DELIVERY_FAILED")
    End Function

    Private Sub SafeMarkDeliveryFailed(ByVal challengeId As Guid, ByVal correlationId As Guid)
        Try
            If Not _challenges.MarkDeliveryFailed(challengeId, _clock.UtcNow) Then
                SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.REJECTED)
            End If
        Catch
            SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.FAILED)
        End Try
    End Sub

    Private Sub SafeFailFinalization(ByVal challengeId As Guid, ByVal correlationId As Guid)
        Try
            If Not _challenges.FailFinalization(challengeId) Then
                SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.REJECTED)
            End If
        Catch
            SafeRecord(correlationId, SecondFactorDiagnosticStage.COMPENSATION, SecondFactorDiagnosticOutcome.FAILED)
        End Try
    End Sub

    Private Sub SafeRecordDeliveryDetail(ByVal correlationId As Guid,
                                         ByVal delivery As SecondFactorDeliveryResult)
        If delivery Is Nothing Then Return
        Select Case If(delivery.PublicCode, String.Empty).Trim().ToUpperInvariant()
            Case "OTP_DISABLED"
                SafeRecord(correlationId, SecondFactorDiagnosticStage.DELIVERY_CONFIGURATION_DISABLED, SecondFactorDiagnosticOutcome.REJECTED)
            Case "OTP_CONFIGURATION_INVALID"
                SafeRecord(correlationId, SecondFactorDiagnosticStage.DELIVERY_CONFIGURATION_INVALID, SecondFactorDiagnosticOutcome.REJECTED)
            Case "OTP_CONFIGURATION_AMBIGUOUS"
                SafeRecord(correlationId, SecondFactorDiagnosticStage.DELIVERY_CONFIGURATION_AMBIGUOUS, SecondFactorDiagnosticOutcome.REJECTED)
        End Select
    End Sub

    Private Sub SafeRecord(ByVal correlationId As Guid,
                           ByVal stage As SecondFactorDiagnosticStage,
                           ByVal outcome As SecondFactorDiagnosticOutcome)
        Try
            _diagnostics.Record(correlationId, stage, outcome)
        Catch
            ' La observabilidad nunca altera el login.
        End Try
    End Sub

    Private Function InvalidSession() As SecondFactorAuthenticationResult
        _contexts.Clear()
        Return Rejected("SECOND_FACTOR_SESSION_INVALID")
    End Function

    Private Function RejectedAndClear(ByVal publicCode As String) As SecondFactorAuthenticationResult
        _contexts.Clear()
        Return Rejected(publicCode)
    End Function

    Private Shared Function Rejected(ByVal publicCode As String) As SecondFactorAuthenticationResult
        Return New SecondFactorAuthenticationResult(False, publicCode, SecondFactorAuthenticationState.REJECTED, String.Empty, 0, 0, String.Empty)
    End Function

    Private Function PendingResult(ByVal publicCode As String, ByVal context As PendingSecondFactorContext) As SecondFactorAuthenticationResult
        Dim now As DateTime = _clock.UtcNow
        Dim expires As Integer = RemainingSeconds(context.ExpiresAtUtc, now)
        Dim resend As Integer = RemainingSeconds(context.CreatedAtUtc.AddSeconds(SegundoFactorConfiguration.ResendCooldownSeconds), now)
        Return New SecondFactorAuthenticationResult(False, publicCode, SecondFactorAuthenticationState.PENDING, context.MaskedDestination, expires, resend, String.Empty)
    End Function

    Private Shared Function RemainingSeconds(ByVal untilUtc As DateTime, ByVal nowUtc As DateTime) As Integer
        Return Math.Max(0, CInt(Math.Ceiling(untilUtc.Subtract(nowUtc).TotalSeconds)))
    End Function

    Private Shared Function BuildIdentity(ByVal loginModule As SecondFactorLoginModule, ByVal principal As SecondFactorPrincipal) As SegundoFactorIdentity
        Return New SegundoFactorIdentity(loginModule.EmpresaId, loginModule.ModuloId, loginModule.ModuleType, principal.InternalUserId.ToString(Globalization.CultureInfo.InvariantCulture), principal.NormalizedLogin)
    End Function

    Private Shared Function BuildIdentity(ByVal pending As PendingSecondFactorContext) As SegundoFactorIdentity
        Return New SegundoFactorIdentity(pending.EmpresaId, pending.ModuloId, pending.ModuleType, pending.InternalUserId.ToString(Globalization.CultureInfo.InvariantCulture), pending.NormalizedLogin)
    End Function
End Class

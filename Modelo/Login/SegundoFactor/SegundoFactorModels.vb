Imports System

' Contratos de dominio internos para Login 2FA. No dependen de WebForms, SMTP ni SQL.
Public Enum SegundoFactorPurpose
    LOGIN = 1
End Enum

Public Enum SegundoFactorChallengeState
    CREATED = 1
    SENT = 2
    FINALIZING = 3
    COMPLETED = 4
    DELIVERY_FAILED = 5
    BLOCKED = 6
    EXPIRED = 7
    REVOKED = 8
    FINALIZATION_FAILED = 9
End Enum

Public NotInheritable Class SegundoFactorIdentity
    Public Sub New(ByVal empresaId As Integer,
                   ByVal moduloId As Integer,
                   ByVal tipoUsuario As String,
                   ByVal idInterno As String,
                   ByVal login As String)
        If empresaId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(empresaId))
        If moduloId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(moduloId))
        If String.IsNullOrWhiteSpace(tipoUsuario) Then Throw New ArgumentException("El tipo de usuario es obligatorio.", NameOf(tipoUsuario))
        If String.IsNullOrWhiteSpace(idInterno) Then Throw New ArgumentException("El identificador interno es obligatorio.", NameOf(idInterno))
        If String.IsNullOrWhiteSpace(login) Then Throw New ArgumentException("El login es obligatorio.", NameOf(login))

        Me.EmpresaId = empresaId
        Me.ModuloId = moduloId
        Me.TipoUsuario = tipoUsuario.Trim().ToUpperInvariant()
        Me.IdInterno = idInterno.Trim()
        Me.LoginNormalizado = login.Trim().ToUpperInvariant()
        Me.ClaveCanonica = String.Format(Globalization.CultureInfo.InvariantCulture,
                                         "{0}:{1}:{2}:{3}",
                                         empresaId,
                                         moduloId,
                                         Me.TipoUsuario,
                                         Me.IdInterno)
    End Sub

    Public ReadOnly Property EmpresaId As Integer
    Public ReadOnly Property ModuloId As Integer
    Public ReadOnly Property TipoUsuario As String
    Public ReadOnly Property IdInterno As String
    Public ReadOnly Property LoginNormalizado As String
    Public ReadOnly Property ClaveCanonica As String
End Class

Public NotInheritable Class SegundoFactorConfiguration
    Public Const EmailProviderType As Integer = 1
    Public Const MaxAttempts As Integer = 5
    Public Const ResendCooldownSeconds As Integer = 60
    Public Const MaxResends As Integer = 2

    Public Sub New(ByVal requiereSegundoFactor As Integer?,
                   ByVal providerType As Integer?,
                   ByVal expirationMinutes As Integer?)
        If Not requiereSegundoFactor.HasValue OrElse requiereSegundoFactor.Value = 0 Then
            IsRequired = False
            Return
        End If
        If requiereSegundoFactor.Value <> 1 Then
            Throw New ArgumentOutOfRangeException(NameOf(requiereSegundoFactor), "La activación 2FA solo admite 0, 1 o NULL.")
        End If
        If Not providerType.HasValue OrElse providerType.Value <> EmailProviderType Then
            Throw New ArgumentOutOfRangeException(NameOf(providerType), "El único proveedor soportado es EMAIL=1.")
        End If
        If Not expirationMinutes.HasValue OrElse expirationMinutes.Value < 1 OrElse expirationMinutes.Value > 10 Then
            Throw New ArgumentOutOfRangeException(NameOf(expirationMinutes), "La expiración debe estar entre 1 y 10 minutos.")
        End If

        IsRequired = True
        Me.ProviderType = providerType.Value
        Me.ExpirationMinutes = expirationMinutes.Value
    End Sub

    Public ReadOnly Property IsRequired As Boolean
    Public ReadOnly Property ProviderType As Integer
    Public ReadOnly Property ExpirationMinutes As Integer
End Class

Public NotInheritable Class SegundoFactorChallenge
    Public Sub New(ByVal challengeId As Guid,
                   ByVal identity As SegundoFactorIdentity,
                   ByVal purpose As SegundoFactorPurpose,
                   ByVal state As SegundoFactorChallengeState,
                   ByVal attempts As Integer,
                   ByVal resendCount As Integer,
                   ByVal createdAtUtc As DateTime,
                   ByVal expiresAtUtc As DateTime)
        If challengeId = Guid.Empty Then Throw New ArgumentException("El challenge es obligatorio.", NameOf(challengeId))
        If identity Is Nothing Then Throw New ArgumentNullException(NameOf(identity))
        If purpose <> SegundoFactorPurpose.LOGIN Then Throw New ArgumentOutOfRangeException(NameOf(purpose))
        If Not [Enum].IsDefined(GetType(SegundoFactorChallengeState), state) Then Throw New ArgumentOutOfRangeException(NameOf(state))
        If attempts < 0 OrElse attempts > SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(attempts))
        If resendCount < 0 OrElse resendCount > SegundoFactorConfiguration.MaxResends Then Throw New ArgumentOutOfRangeException(NameOf(resendCount))
        EnsureUtc(createdAtUtc, NameOf(createdAtUtc))
        EnsureUtc(expiresAtUtc, NameOf(expiresAtUtc))
        If expiresAtUtc <= createdAtUtc Then Throw New ArgumentException("La expiración debe ser posterior a la creación.", NameOf(expiresAtUtc))

        Me.ChallengeId = challengeId
        Me.Identity = identity
        Me.Purpose = purpose
        Me.State = state
        Me.Attempts = attempts
        Me.ResendCount = resendCount
        Me.CreatedAtUtc = createdAtUtc
        Me.ExpiresAtUtc = expiresAtUtc
    End Sub

    Public ReadOnly Property ChallengeId As Guid
    Public ReadOnly Property Identity As SegundoFactorIdentity
    Public ReadOnly Property Purpose As SegundoFactorPurpose
    Public ReadOnly Property State As SegundoFactorChallengeState
    Public ReadOnly Property Attempts As Integer
    Public ReadOnly Property ResendCount As Integer
    Public ReadOnly Property CreatedAtUtc As DateTime
    Public ReadOnly Property ExpiresAtUtc As DateTime

    Public Function IsExpired(ByVal clock As ISecondFactorClock) As Boolean
        If clock Is Nothing Then Throw New ArgumentNullException(NameOf(clock))
        Return clock.UtcNow >= ExpiresAtUtc
    End Function

    Private Shared Sub EnsureUtc(ByVal value As DateTime, ByVal parameterName As String)
        If value.Kind <> DateTimeKind.Utc Then Throw New ArgumentException("La fecha debe expresarse en UTC.", parameterName)
    End Sub
End Class

Public NotInheritable Class SecondFactorStoredChallenge
    Public Sub New(ByVal challengeId As Guid,
                   ByVal canonicalIdentity As String,
                   ByVal purpose As SegundoFactorPurpose,
                   ByVal state As SegundoFactorChallengeState,
                   ByVal attempts As Integer,
                   ByVal createdAtUtc As DateTime,
                   ByVal expiresAtUtc As DateTime)
        If challengeId = Guid.Empty Then Throw New ArgumentException("El challenge es obligatorio.", NameOf(challengeId))
        If String.IsNullOrWhiteSpace(canonicalIdentity) Then Throw New ArgumentException("La identidad canónica es obligatoria.", NameOf(canonicalIdentity))
        If purpose <> SegundoFactorPurpose.LOGIN Then Throw New ArgumentOutOfRangeException(NameOf(purpose))
        If Not [Enum].IsDefined(GetType(SegundoFactorChallengeState), state) Then Throw New ArgumentOutOfRangeException(NameOf(state))
        If attempts < 0 OrElse attempts > SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(attempts))
        EnsureUtc(createdAtUtc, NameOf(createdAtUtc))
        EnsureUtc(expiresAtUtc, NameOf(expiresAtUtc))
        If expiresAtUtc <= createdAtUtc Then Throw New ArgumentException("La expiración debe ser posterior a la creación.", NameOf(expiresAtUtc))

        Me.ChallengeId = challengeId
        Me.CanonicalIdentity = canonicalIdentity.Trim()
        Me.Purpose = purpose
        Me.State = state
        Me.Attempts = attempts
        Me.CreatedAtUtc = createdAtUtc
        Me.ExpiresAtUtc = expiresAtUtc
    End Sub

    Public ReadOnly Property ChallengeId As Guid
    Public ReadOnly Property CanonicalIdentity As String
    Public ReadOnly Property Purpose As SegundoFactorPurpose
    Public ReadOnly Property State As SegundoFactorChallengeState
    Public ReadOnly Property Attempts As Integer
    Public ReadOnly Property CreatedAtUtc As DateTime
    Public ReadOnly Property ExpiresAtUtc As DateTime

    Private Shared Sub EnsureUtc(ByVal value As DateTime, ByVal parameterName As String)
        If value.Kind <> DateTimeKind.Utc Then Throw New ArgumentException("La fecha debe expresarse en UTC.", parameterName)
    End Sub
End Class

Public NotInheritable Class SecondFactorChallengeVerificationData
    Public Sub New(ByVal challenge As SecondFactorStoredChallenge, ByVal protectedCode As String)
        If challenge Is Nothing Then Throw New ArgumentNullException(NameOf(challenge))
        If String.IsNullOrWhiteSpace(protectedCode) Then Throw New ArgumentException("El código protegido es obligatorio.", NameOf(protectedCode))
        Me.Challenge = challenge
        Me.ProtectedCode = protectedCode.Trim()
    End Sub

    Public ReadOnly Property Challenge As SecondFactorStoredChallenge
    Public ReadOnly Property ProtectedCode As String
End Class

Public NotInheritable Class SecondFactorProtectionContext
    Public Sub New(ByVal purpose As SegundoFactorPurpose,
                   ByVal challengeId As Guid,
                   ByVal canonicalIdentity As String,
                   ByVal sessionBinding As String)
        If purpose <> SegundoFactorPurpose.LOGIN Then Throw New ArgumentOutOfRangeException(NameOf(purpose))
        If challengeId = Guid.Empty Then Throw New ArgumentException("El challenge es obligatorio.", NameOf(challengeId))
        If String.IsNullOrWhiteSpace(canonicalIdentity) Then Throw New ArgumentException("La identidad canónica es obligatoria.", NameOf(canonicalIdentity))
        If String.IsNullOrWhiteSpace(sessionBinding) Then Throw New ArgumentException("El vínculo de sesión es obligatorio.", NameOf(sessionBinding))
        Me.Purpose = purpose
        Me.ChallengeId = challengeId
        Me.CanonicalIdentity = canonicalIdentity.Trim()
        Me.SessionBinding = sessionBinding.Trim()
    End Sub

    Public ReadOnly Property Purpose As SegundoFactorPurpose
    Public ReadOnly Property ChallengeId As Guid
    Public ReadOnly Property CanonicalIdentity As String
    Public ReadOnly Property SessionBinding As String
End Class

Public NotInheritable Class SecondFactorKeyMaterial
    Private ReadOnly _keyBytes As Byte()

    Public Sub New(ByVal keyId As String, ByVal keyBytes As Byte())
        If String.IsNullOrWhiteSpace(keyId) OrElse keyId.Contains(":") Then Throw New ArgumentException("El identificador de llave es inválido.", NameOf(keyId))
        If keyBytes Is Nothing OrElse keyBytes.Length < 32 Then Throw New ArgumentException("La llave HMAC debe tener al menos 32 bytes.", NameOf(keyBytes))
        Me.KeyId = keyId.Trim()
        _keyBytes = DirectCast(keyBytes.Clone(), Byte())
    End Sub

    Public ReadOnly Property KeyId As String
    Public ReadOnly Property KeyBytes As Byte()
        Get
            Return DirectCast(_keyBytes.Clone(), Byte())
        End Get
    End Property
End Class

Public NotInheritable Class SecondFactorRecipient
    Public Sub New(ByVal emailAddress As String, ByVal maskedDestination As String)
        If String.IsNullOrWhiteSpace(emailAddress) Then Throw New ArgumentException("El correo es obligatorio.", NameOf(emailAddress))
        If Not SecondFactorContractValidation.IsMaskedEmailDestination(maskedDestination) Then Throw New ArgumentException("El destino debe estar enmascarado.", NameOf(maskedDestination))
        Me.EmailAddress = emailAddress.Trim()
        Me.MaskedDestination = maskedDestination.Trim()
    End Sub

    Public ReadOnly Property EmailAddress As String
    Public ReadOnly Property MaskedDestination As String
End Class

Public NotInheritable Class SecondFactorEmailMessage
    Public Sub New(ByVal recipient As SecondFactorRecipient, ByVal code As String, ByVal expiresAtUtc As DateTime)
        If recipient Is Nothing Then Throw New ArgumentNullException(NameOf(recipient))
        If Not SecondFactorContractValidation.IsSixDigitCode(code) Then Throw New ArgumentException("El OTP debe tener seis dígitos.", NameOf(code))
        If expiresAtUtc.Kind <> DateTimeKind.Utc Then Throw New ArgumentException("La expiración debe expresarse en UTC.", NameOf(expiresAtUtc))
        Me.Recipient = recipient
        Me.Code = code
        Me.ExpiresAtUtc = expiresAtUtc
    End Sub
    Public ReadOnly Property Recipient As SecondFactorRecipient
    Public ReadOnly Property Code As String
    Public ReadOnly Property ExpiresAtUtc As DateTime
End Class

Public NotInheritable Class SecondFactorDeliveryResult
    Public Property Success As Boolean
    Public Property PublicCode As String
End Class

Public NotInheritable Class LegacyLoginFinalizationResult
    Public Property Success As Boolean
    Public Property RedirectRequired As Boolean
    Public Property LocalRoute As String
    Public Property PublicCode As String
End Class

Public Enum SecondFactorPreAuthenticationStatus
    FINALIZED = 1
    SECOND_FACTOR_REQUIRED = 2
    REJECTED = 3
End Enum

Public NotInheritable Class LegacyLoginFinalizationContext
    Public Sub New(ByVal empresaId As Integer,
                   ByVal moduloId As Integer,
                   ByVal moduleType As String,
                   ByVal internalUserId As Long,
                   ByVal normalizedLogin As String)
        If empresaId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(empresaId))
        If moduloId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(moduloId))
        If String.IsNullOrWhiteSpace(moduleType) Then Throw New ArgumentException("El tipo de módulo es obligatorio.", NameOf(moduleType))
        If internalUserId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(internalUserId))
        If String.IsNullOrWhiteSpace(normalizedLogin) Then Throw New ArgumentException("El login es obligatorio.", NameOf(normalizedLogin))

        Me.EmpresaId = empresaId
        Me.ModuloId = moduloId
        Me.ModuleType = moduleType.Trim().ToUpperInvariant()
        Me.InternalUserId = internalUserId
        Me.NormalizedLogin = normalizedLogin.Trim().ToUpperInvariant()
    End Sub

    Public ReadOnly Property EmpresaId As Integer
    Public ReadOnly Property ModuloId As Integer
    Public ReadOnly Property ModuleType As String
    Public ReadOnly Property InternalUserId As Long
    Public ReadOnly Property NormalizedLogin As String
End Class

Public NotInheritable Class SecondFactorLoginModule
    Public Sub New(ByVal empresaId As Integer,
                   ByVal moduloId As Integer,
                   ByVal moduleName As String,
                   ByVal moduleType As String,
                   ByVal configuration As SegundoFactorConfiguration)
        If empresaId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(empresaId))
        If moduloId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(moduloId))
        If String.IsNullOrWhiteSpace(moduleName) Then Throw New ArgumentException("El módulo es obligatorio.", NameOf(moduleName))
        If String.IsNullOrWhiteSpace(moduleType) Then Throw New ArgumentException("El tipo de módulo es obligatorio.", NameOf(moduleType))
        If configuration Is Nothing Then Throw New ArgumentNullException(NameOf(configuration))

        Me.EmpresaId = empresaId
        Me.ModuloId = moduloId
        Me.ModuleName = moduleName.Trim()
        Me.ModuleType = moduleType.Trim().ToUpperInvariant()
        Me.Configuration = configuration
    End Sub

    Public ReadOnly Property EmpresaId As Integer
    Public ReadOnly Property ModuloId As Integer
    Public ReadOnly Property ModuleName As String
    Public ReadOnly Property ModuleType As String
    Public ReadOnly Property Configuration As SegundoFactorConfiguration
End Class

Public NotInheritable Class SecondFactorPrincipal
    Public Sub New(ByVal internalUserId As Long,
                   ByVal normalizedLogin As String,
                   ByVal emailAddress As String)
        If internalUserId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(internalUserId))
        If String.IsNullOrWhiteSpace(normalizedLogin) Then Throw New ArgumentException("El login es obligatorio.", NameOf(normalizedLogin))

        Me.InternalUserId = internalUserId
        Me.NormalizedLogin = normalizedLogin.Trim().ToUpperInvariant()
        Me.EmailAddress = If(emailAddress, String.Empty).Trim()
        Me.MaskedDestination = If(String.IsNullOrWhiteSpace(Me.EmailAddress),
                                  String.Empty,
                                  SecondFactorContractValidation.MaskEmailAddress(Me.EmailAddress))
    End Sub

    Public ReadOnly Property InternalUserId As Long
    Public ReadOnly Property NormalizedLogin As String
    Friend ReadOnly Property EmailAddress As String
    Public ReadOnly Property MaskedDestination As String
End Class

Public NotInheritable Class SecondFactorPreAuthenticationRequest
    Public Sub New(ByVal companyName As String,
                   ByVal moduleName As String,
                   ByVal validatedLogin As String,
                   ByVal principalContext As ContextoPreautenticacionModulo)
        If String.IsNullOrWhiteSpace(companyName) Then Throw New ArgumentException("La empresa es obligatoria.", NameOf(companyName))
        If String.IsNullOrWhiteSpace(moduleName) Then Throw New ArgumentException("El módulo es obligatorio.", NameOf(moduleName))
        If String.IsNullOrWhiteSpace(validatedLogin) Then Throw New ArgumentException("El login validado es obligatorio.", NameOf(validatedLogin))
        If principalContext Is Nothing OrElse Not principalContext.EsValido() Then Throw New ArgumentException("El contexto de conexión es inválido.", NameOf(principalContext))

        Me.CompanyName = companyName.Trim()
        Me.ModuleName = moduleName.Trim()
        Me.ValidatedLogin = validatedLogin.Trim()
        Me.PrincipalContext = principalContext
    End Sub

    Public ReadOnly Property CompanyName As String
    Public ReadOnly Property ModuleName As String
    Public ReadOnly Property ValidatedLogin As String
    Public ReadOnly Property PrincipalContext As ContextoPreautenticacionModulo
End Class

Public NotInheritable Class SecondFactorPreAuthenticationResult
    Public Sub New(ByVal status As SecondFactorPreAuthenticationStatus,
                   ByVal publicCode As String,
                   ByVal loginModule As SecondFactorLoginModule,
                   ByVal principal As SecondFactorPrincipal,
                   ByVal finalization As LegacyLoginFinalizationResult)
        If Not [Enum].IsDefined(GetType(SecondFactorPreAuthenticationStatus), status) Then Throw New ArgumentOutOfRangeException(NameOf(status))
        If String.IsNullOrWhiteSpace(publicCode) Then Throw New ArgumentException("El código público es obligatorio.", NameOf(publicCode))
        Me.Status = status
        Me.PublicCode = publicCode.Trim().ToUpperInvariant()
        Me.LoginModule = loginModule
        Me.Principal = principal
        Me.Finalization = finalization
    End Sub

    Public ReadOnly Property Status As SecondFactorPreAuthenticationStatus
    Public ReadOnly Property PublicCode As String
    Public ReadOnly Property LoginModule As SecondFactorLoginModule
    Public ReadOnly Property Principal As SecondFactorPrincipal
    Public ReadOnly Property Finalization As LegacyLoginFinalizationResult
End Class

Public NotInheritable Class PendingSecondFactorContext
    Public Sub New(ByVal empresaId As Integer,
                   ByVal moduloId As Integer,
                   ByVal internalUserId As Long,
                   ByVal normalizedLogin As String,
                   ByVal maskedDestination As String,
                   ByVal challengeId As Guid,
                   ByVal sessionNonce As String,
                   ByVal createdAtUtc As DateTime,
                   ByVal expiresAtUtc As DateTime)
        If empresaId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(empresaId))
        If moduloId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(moduloId))
        If internalUserId <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(internalUserId))
        If String.IsNullOrWhiteSpace(normalizedLogin) Then Throw New ArgumentException("El login es obligatorio.", NameOf(normalizedLogin))
        If Not SecondFactorContractValidation.IsMaskedEmailDestination(maskedDestination) Then Throw New ArgumentException("El destino debe estar enmascarado.", NameOf(maskedDestination))
        If challengeId = Guid.Empty Then Throw New ArgumentException("El challenge es obligatorio.", NameOf(challengeId))
        If String.IsNullOrWhiteSpace(sessionNonce) Then Throw New ArgumentException("El nonce de sesión es obligatorio.", NameOf(sessionNonce))
        If createdAtUtc.Kind <> DateTimeKind.Utc OrElse expiresAtUtc.Kind <> DateTimeKind.Utc Then Throw New ArgumentException("Las fechas deben expresarse en UTC.")
        If expiresAtUtc <= createdAtUtc Then Throw New ArgumentException("La expiración debe ser posterior a la creación.", NameOf(expiresAtUtc))

        Me.EmpresaId = empresaId
        Me.ModuloId = moduloId
        Me.InternalUserId = internalUserId
        Me.NormalizedLogin = normalizedLogin.Trim().ToUpperInvariant()
        Me.MaskedDestination = maskedDestination.Trim()
        Me.ChallengeId = challengeId
        Me.SessionNonce = sessionNonce.Trim()
        Me.CreatedAtUtc = createdAtUtc
        Me.ExpiresAtUtc = expiresAtUtc
    End Sub

    Public ReadOnly Property EmpresaId As Integer
    Public ReadOnly Property ModuloId As Integer
    Public ReadOnly Property InternalUserId As Long
    Public ReadOnly Property NormalizedLogin As String
    Public ReadOnly Property MaskedDestination As String
    Public ReadOnly Property ChallengeId As Guid
    Public ReadOnly Property SessionNonce As String
    Public ReadOnly Property CreatedAtUtc As DateTime
    Public ReadOnly Property ExpiresAtUtc As DateTime
End Class

Public NotInheritable Class SecondFactorContractValidation
    Private Sub New()
    End Sub

    Public Shared Function IsSixDigitCode(ByVal code As String) As Boolean
        If code Is Nothing OrElse code.Length <> 6 Then Return False
        For Each character As Char In code
            If character < "0"c OrElse character > "9"c Then Return False
        Next
        Return True
    End Function

    Public Shared Function IsMaskedEmailDestination(ByVal destination As String) As Boolean
        If String.IsNullOrWhiteSpace(destination) Then Return False
        Dim normalized As String = destination.Trim()
        For Each character As Char In normalized
            If Char.IsWhiteSpace(character) Then Return False
        Next
        Dim separator As Integer = normalized.IndexOf("@"c)
        Return separator > 0 AndAlso
               separator < normalized.Length - 1 AndAlso
               normalized.IndexOf("@"c, separator + 1) < 0 AndAlso
               normalized.Substring(0, separator).Contains("*") AndAlso
               Not normalized.Substring(separator + 1).Contains("*")
    End Function

    Public Shared Function MaskEmailAddress(ByVal emailAddress As String) As String
        If String.IsNullOrWhiteSpace(emailAddress) Then Throw New ArgumentException("El correo es obligatorio.", NameOf(emailAddress))
        Dim normalized As String = emailAddress.Trim()
        Dim separator As Integer = normalized.IndexOf("@"c)
        If separator <= 0 OrElse separator = normalized.Length - 1 OrElse normalized.IndexOf("@"c, separator + 1) >= 0 Then
            Throw New ArgumentException("El correo no tiene un formato válido.", NameOf(emailAddress))
        End If
        Dim visibleLocal As String = If(separator = 1, "*", normalized.Substring(0, 1) & "***")
        Return visibleLocal & normalized.Substring(separator)
    End Function
End Class

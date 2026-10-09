Imports System

' Puertos internos. Las implementaciones de infraestructura pertenecen a entregas separadas.
Public Interface ISecondFactorClock
    ReadOnly Property UtcNow As DateTime
End Interface

Public Interface ISecondFactorOtpGenerator
    Function GenerateCode() As String
End Interface

Public Interface ISecondFactorCodeProtector
    Function Protect(ByVal context As SecondFactorProtectionContext, ByVal code As String) As String
    Function Verify(ByVal context As SecondFactorProtectionContext, ByVal code As String, ByVal protectedCode As String) As Boolean
End Interface

Public Interface ISecondFactorKeyProvider
    Function GetActiveKey() As SecondFactorKeyMaterial
    Function TryGetKey(ByVal keyId As String, ByRef key As SecondFactorKeyMaterial) As Boolean
End Interface

Public Interface ISecondFactorConfigurationRepository
    Function GetConfiguration(ByVal moduleId As Integer) As SegundoFactorConfiguration
End Interface

Public Interface ISecondFactorChallengeRepository
    Function Create(ByVal challenge As SegundoFactorChallenge, ByVal protectedCode As String, ByVal sessionBindingHash As String) As Boolean
    Function GetForVerification(ByVal challengeId As Guid, ByVal sessionBindingHash As String) As SegundoFactorChallenge
    Function GetVerificationData(ByVal challengeId As Guid, ByVal sessionBindingHash As String) As SecondFactorChallengeVerificationData
    Function MarkSent(ByVal challengeId As Guid, ByVal sentAtUtc As DateTime) As Boolean
    Function MarkDeliveryFailed(ByVal challengeId As Guid, ByVal failedAtUtc As DateTime) As Boolean
    Function RegisterFailedAttempt(ByVal challengeId As Guid, ByVal expectedAttempts As Integer) As SegundoFactorChallenge
    Function RegisterFailedAttemptData(ByVal challengeId As Guid, ByVal expectedAttempts As Integer) As SecondFactorStoredChallenge
    Function TryBeginFinalization(ByVal challengeId As Guid, ByVal expectedAttempts As Integer) As Boolean
    Function Complete(ByVal challengeId As Guid) As Boolean
    Function FailFinalization(ByVal challengeId As Guid) As Boolean
    Function Revoke(ByVal challengeId As Guid) As Boolean
    Function ReplaceForResend(ByVal previousChallengeId As Guid, ByVal replacement As SegundoFactorChallenge, ByVal protectedCode As String, ByVal sessionBindingHash As String, ByVal requestedAtUtc As DateTime) As Boolean
    Function Expire(ByVal challengeId As Guid, ByVal observedAtUtc As DateTime) As Boolean
End Interface

Public Interface ISecondFactorRecipientResolver
    Function Resolve(ByVal identity As SegundoFactorIdentity) As SecondFactorRecipient
End Interface

Public Interface ISecondFactorEmailSender
    Function Send(ByVal message As SecondFactorEmailMessage) As SecondFactorDeliveryResult
End Interface

Public Interface ISecondFactorSmtpConfigurationRepository
    Function Resolve() As SecondFactorSmtpConfigurationResolution
End Interface

Public Interface ISecondFactorSmtpTransport
    Function Send(ByVal configuration As SecondFactorSmtpConfiguration,
                  ByVal message As SecondFactorEmailMessage) As SecondFactorSmtpDelivery
End Interface

Public Interface ILegacyLoginFinalizer
    Function FinalizeLogin(ByVal context As PendingSecondFactorContext) As LegacyLoginFinalizationResult
End Interface

Public Interface IPendingSecondFactorContextStore
    Sub Save(ByVal context As PendingSecondFactorContext)
    Function GetCurrent() As PendingSecondFactorContext
    Sub Clear()
End Interface

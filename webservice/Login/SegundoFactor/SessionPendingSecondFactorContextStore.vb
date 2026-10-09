Imports System
Imports System.Web

' Único adaptador WebForms de DOC-91. El dominio no conoce Session.
Public NotInheritable Class SessionPendingSecondFactorContextStore
    Implements IPendingSecondFactorContextStore

    Private Const SessionKey As String = "LOGIN_SECOND_FACTOR_PENDING_V1"
    Private ReadOnly _session As HttpSessionStateBase
    Private ReadOnly _clock As ISecondFactorClock

    Public Sub New(ByVal session As HttpSessionStateBase)
        Me.New(session, New SystemSecondFactorClock())
    End Sub

    Public Sub New(ByVal session As HttpSessionStateBase, ByVal clock As ISecondFactorClock)
        If session Is Nothing Then Throw New ArgumentNullException(NameOf(session))
        If clock Is Nothing Then Throw New ArgumentNullException(NameOf(clock))
        _session = session
        _clock = clock
    End Sub

    Public Sub Save(ByVal context As PendingSecondFactorContext) Implements IPendingSecondFactorContextStore.Save
        If context Is Nothing Then Throw New ArgumentNullException(NameOf(context))
        If context.ExpiresAtUtc <= _clock.UtcNow Then Throw New ArgumentException("No se puede guardar un contexto expirado.", NameOf(context))
        _session.Item(SessionKey) = context
    End Sub

    Public Function GetCurrent() As PendingSecondFactorContext Implements IPendingSecondFactorContextStore.GetCurrent
        Dim context As PendingSecondFactorContext = TryCast(_session.Item(SessionKey), PendingSecondFactorContext)
        If context Is Nothing Then Return Nothing
        If context.ExpiresAtUtc <= _clock.UtcNow Then
            Clear()
            Return Nothing
        End If
        Return context
    End Function

    Public Sub Clear() Implements IPendingSecondFactorContextStore.Clear
        _session.Remove(SessionKey)
    End Sub
End Class

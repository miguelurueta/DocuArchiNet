Imports System
Imports System.Web

' Frontera única que materializa el contexto 2FA desde la Session del request actual.
Public NotInheritable Class LoginSecondFactorSessionGate
    Private ReadOnly _context As HttpContextBase
    Private ReadOnly _clock As ISecondFactorClock

    Public Sub New()
        Me.New(If(HttpContext.Current Is Nothing, Nothing, New HttpContextWrapper(HttpContext.Current)), New SystemSecondFactorClock())
    End Sub

    Public Sub New(ByVal context As HttpContextBase, ByVal clock As ISecondFactorClock)
        _context = context
        _clock = clock
    End Sub

    Public Function AsegurarContexto() As PendingSecondFactorContext
        If _context Is Nothing OrElse _context.Session Is Nothing OrElse _clock Is Nothing Then Return Nothing
        Dim store As New SessionPendingSecondFactorContextStore(_context.Session, _clock)
        Dim pending As PendingSecondFactorContext = store.GetCurrent()
        If pending Is Nothing OrElse String.IsNullOrWhiteSpace(pending.ModuleType) Then
            store.Clear()
            Return Nothing
        End If
        Return pending
    End Function
End Class

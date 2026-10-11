Imports System
Imports System.Diagnostics

' Usa una fuente existente del registro Application; no crea fuentes en tiempo de ejecución.
Public NotInheritable Class EventLogSecondFactorDiagnosticSink
    Implements ISecondFactorDiagnosticSink

    Private Const EventSource As String = "Application"
    Private Const DiagnosticEventId As Integer = 9501
    Private ReadOnly _fallback As ISecondFactorDiagnosticSink

    Public Sub New()
        Me.New(New TraceSecondFactorDiagnosticSink())
    End Sub

    Public Sub New(ByVal fallback As ISecondFactorDiagnosticSink)
        _fallback = If(fallback, NullSecondFactorDiagnosticSink.Instance)
    End Sub

    Public Sub Record(ByVal correlationId As Guid,
                      ByVal stage As SecondFactorDiagnosticStage,
                      ByVal outcome As SecondFactorDiagnosticOutcome) Implements ISecondFactorDiagnosticSink.Record
        If correlationId = Guid.Empty Then Return
        If Not [Enum].IsDefined(GetType(SecondFactorDiagnosticStage), stage) Then Return
        If Not [Enum].IsDefined(GetType(SecondFactorDiagnosticOutcome), outcome) Then Return
        If Not ShouldPersist(stage, outcome) Then Return

        Dim message As String = String.Format(Globalization.CultureInfo.InvariantCulture,
                                              "Login2FA correlation={0} stage={1} outcome={2}",
                                              correlationId.ToString("D"),
                                              stage.ToString(),
                                              outcome.ToString())
        Try
            Dim entryType As EventLogEntryType = If(outcome = SecondFactorDiagnosticOutcome.SUCCEEDED,
                                                    EventLogEntryType.Information,
                                                    EventLogEntryType.Warning)
            EventLog.WriteEntry(EventSource, message, entryType, DiagnosticEventId)
        Catch
            Try
                _fallback.Record(correlationId, stage, outcome)
            Catch
                ' La telemetría nunca puede cambiar el resultado del login.
            End Try
        End Try
    End Sub

    Private Shared Function ShouldPersist(ByVal stage As SecondFactorDiagnosticStage,
                                          ByVal outcome As SecondFactorDiagnosticOutcome) As Boolean
        Return outcome = SecondFactorDiagnosticOutcome.FAILED OrElse
               outcome = SecondFactorDiagnosticOutcome.REJECTED OrElse
               (stage = SecondFactorDiagnosticStage.AUTHENTICATION_START AndAlso outcome = SecondFactorDiagnosticOutcome.SUCCEEDED)
    End Function
End Class

Imports System
Imports System.Diagnostics

' Emite exclusivamente campos de catálogo; nunca recibe datos de usuario, OTP o excepciones.
Public NotInheritable Class TraceSecondFactorDiagnosticSink
    Implements ISecondFactorDiagnosticSink

    Public Sub Record(ByVal correlationId As Guid,
                      ByVal stage As SecondFactorDiagnosticStage,
                      ByVal outcome As SecondFactorDiagnosticOutcome) Implements ISecondFactorDiagnosticSink.Record
        If correlationId = Guid.Empty Then Return
        If Not [Enum].IsDefined(GetType(SecondFactorDiagnosticStage), stage) Then Return
        If Not [Enum].IsDefined(GetType(SecondFactorDiagnosticOutcome), outcome) Then Return

        Try
            Trace.TraceInformation("Login2FA correlation={0} stage={1} outcome={2}",
                                   correlationId.ToString("D"),
                                   stage.ToString(),
                                   outcome.ToString())
        Catch
            ' La telemetría nunca puede cambiar el resultado del login.
        End Try
    End Sub
End Class

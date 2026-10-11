Imports System

' Preserva los consumidores existentes y permite pruebas aisladas sin infraestructura.
Public NotInheritable Class NullSecondFactorDiagnosticSink
    Implements ISecondFactorDiagnosticSink

    Public Shared ReadOnly Instance As ISecondFactorDiagnosticSink = New NullSecondFactorDiagnosticSink()

    Private Sub New()
    End Sub

    Public Sub Record(ByVal correlationId As Guid,
                      ByVal stage As SecondFactorDiagnosticStage,
                      ByVal outcome As SecondFactorDiagnosticOutcome) Implements ISecondFactorDiagnosticSink.Record
    End Sub
End Class

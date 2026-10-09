Imports System

Public NotInheritable Class SystemSecondFactorClock
    Implements ISecondFactorClock

    Public ReadOnly Property UtcNow As DateTime Implements ISecondFactorClock.UtcNow
        Get
            Return DateTime.UtcNow
        End Get
    End Property
End Class

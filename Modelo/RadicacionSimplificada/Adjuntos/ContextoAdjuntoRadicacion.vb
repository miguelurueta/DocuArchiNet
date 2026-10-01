Imports System

'Identidad autoritativa e inmutable de una carga de Radicación Simplificada.
Public NotInheritable Class ContextoAdjuntoRadicacion
    Private ReadOnly _idRegistroEstado As Long
    Private ReadOnly _radicado As String
    Private ReadOnly _idTareaWorkflow As Long
    Private ReadOnly _idTipoTramite As Integer
    Private ReadOnly _idPlantilla As Integer
    Private ReadOnly _nombreGabinete As String

    Public Sub New(ByVal idRegistroEstado As Long,
                   ByVal radicado As String,
                   ByVal idTareaWorkflow As Long,
                   ByVal idTipoTramite As Integer,
                   ByVal idPlantilla As Integer,
                   ByVal nombreGabinete As String)
        _idRegistroEstado = idRegistroEstado
        _radicado = If(radicado, String.Empty).Trim()
        _idTareaWorkflow = idTareaWorkflow
        _idTipoTramite = idTipoTramite
        _idPlantilla = idPlantilla
        _nombreGabinete = If(nombreGabinete, String.Empty).Trim()
    End Sub

    Public ReadOnly Property IdRegistroEstado As Long
        Get
            Return _idRegistroEstado
        End Get
    End Property

    Public ReadOnly Property Radicado As String
        Get
            Return _radicado
        End Get
    End Property

    Public ReadOnly Property IdTareaWorkflow As Long
        Get
            Return _idTareaWorkflow
        End Get
    End Property

    Public ReadOnly Property IdTipoTramite As Integer
        Get
            Return _idTipoTramite
        End Get
    End Property

    Public ReadOnly Property IdPlantilla As Integer
        Get
            Return _idPlantilla
        End Get
    End Property

    Public ReadOnly Property NombreGabinete As String
        Get
            Return _nombreGabinete
        End Get
    End Property

    Public Function EsValido() As Boolean
        Return IdRegistroEstado > 0 AndAlso IdTareaWorkflow > 0 AndAlso
               IdTipoTramite > 0 AndAlso IdPlantilla > 0 AndAlso
               Not String.IsNullOrWhiteSpace(Radicado)
    End Function
End Class

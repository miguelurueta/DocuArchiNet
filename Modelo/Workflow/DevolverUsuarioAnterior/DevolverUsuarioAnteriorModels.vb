Imports System
Imports System.Collections.Generic

'Modelos internos exclusivos; no contienen controles de presentación, conectores ni datos provenientes del navegador.
Public Class TareaDevolverUsuarioAnterior
    Public Property IdTarea As Long
    Public Property IdEstadoActual As Long
    Public Property IdRuta As Integer
    Public Property IdActividadActual As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadFlujoActual As Integer
    Public Property EstaActiva As Boolean
End Class

Public Class UsuarioHistoricoDevolverUsuarioAnterior
    Public Property IdEstadoHistorico As Long
    Public Property IdUsuarioWorkflow As Integer
    Public Property IdActividad As Integer
    Public Property IdRuta As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadFlujoTrabajo As Integer
    Public Property IdUsuarioWorkflowFlujoTrabajo As Integer
    Public Property NombreUsuario As String
    Public Property NombreActividad As String
End Class

Public Class ResultadoHistorialDevolverUsuarioAnterior
    Public Property UsuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String

    Public ReadOnly Property EsValido As Boolean
        Get
            Return UsuarioHistorico IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CodigoBloqueo)
        End Get
    End Property
End Class

Public Class ResultadoAutorizacionDevolverUsuarioAnterior
    Public Property Autorizado As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class SolicitudPreviewDevolverUsuarioAnterior
    Public Property IdTarea As Long
End Class

Public Class SolicitudEjecutarDevolverUsuarioAnterior
    Public Property IdTarea As Long
    Public Property TokenVersion As String
End Class

Public Class ResultadoGuardDevolverUsuarioAnterior
    Public Property Adquirido As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property Lease As IDevolverUsuarioAnteriorConcurrencyLease
End Class

Public Class ResultadoEjecucionDevolverUsuarioAnterior
    Public Sub New()
        Advertencias = New List(Of String)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property EsReintentable As Boolean
    Public Property Advertencias As IList(Of String)
End Class

Public Class AuditoriaDevolverUsuarioAnterior
    Public Property IdTarea As Long
    Public Property IdUsuarioWorkflow As Integer
    Public Property IdRuta As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadOrigen As Integer
    Public Property IdActividadDestino As Integer
    Public Property FechaUtc As DateTime
    Public Property DuracionMilisegundos As Long
    Public Property Resultado As String
    Public Property CodigoFuncional As String
    Public Property Referencia As String
End Class

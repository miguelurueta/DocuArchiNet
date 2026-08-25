Imports System
Imports System.Collections.Generic

' Modelos internos exclusivos de Devolver a actividad anterior.
Public NotInheritable Class TiposContextoDevolverActividad
    Public Const Ruta As String = "RUTA"
    Public Const Flujo As String = "FLUJO"

    Private Sub New()
    End Sub
End Class

Public Class TareaDevolverActividad
    Public Property IdTarea As Long
    Public Property IdEstado As Long
    Public Property IdRuta As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadActual As Integer
    Public Property IdActividadFlujoActual As Integer
    Public Property IdGrupoActual As Integer
    Public Property Radicado As String
    Public Property NombreGrupoActual As String
    Public Property TipoContexto As String
    Public Property TokenVersion As String
    Public Property EstaActiva As Boolean
End Class

Public Class SolicitudPreviewDevolverActividad
    Public Property IdTarea As Long
    Public Property Termino As String
    Public Property Cursor As String
    Public Property TamanoPagina As Integer
    'Campos internos derivados de un cursor validado; no forman parte del DTO público.
    Public Property OrdenDespuesDe As Integer
    Public Property IdConectorDespuesDe As Integer
End Class

Public Class SolicitudEjecutarDevolverActividad
    Public Property IdTarea As Long
    Public Property IdConector As Integer
    Public Property TokenVersion As String
End Class

Public Class DestinoDevolverActividad
    Public Property IdConector As Integer
    Public Property TipoContexto As String
    Public Property IdActividadOrigen As Integer
    Public Property IdActividadDestino As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadFlujoOrigen As Integer
    Public Property IdActividadFlujoDestino As Integer
    Public Property IdUsuarioWorkflowDestino As Integer
    Public Property IdGrupoWorkflowDestino As Integer
    Public Property NombreActividad As String
    Public Property NombreUsuarioDestino As String
    Public Property NombreGrupoDestino As String
    Public Property RequiereNotificacion As Boolean
    Public Property Orden As Integer
End Class

Public Class ResultadoBusquedaDevolverActividad
    Public Sub New()
        Destinos = New List(Of DestinoDevolverActividad)()
    End Sub

    Public Property Destinos As IList(Of DestinoDevolverActividad)
    Public Property CursorSiguiente As String
    Public Property HayMas As Boolean
    Public Property TamanoPagina As Integer
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class ResultadoResolucionDevolverActividad
    Public Property Destino As DestinoDevolverActividad
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String

    Public ReadOnly Property EsValido As Boolean
        Get
            Return Destino IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CodigoBloqueo)
        End Get
    End Property
End Class

Public Class ResultadoAutorizacionDevolverActividad
    Public Property Autorizado As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class ResultadoGuardDevolverActividad
    Public Property Adquirido As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property Lease As IDevolverActividadConcurrencyLease
End Class

Public Class ResultadoEjecucionDevolverActividad
    Public Sub New()
        Advertencias = New List(Of String)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property EsReintentable As Boolean
    Public Property ReferenciaAuditoria As String
    Public Property Advertencias As IList(Of String)
End Class

Public Class AuditoriaDevolverActividad
    Public Property IdTarea As Long
    Public Property IdUsuarioWorkflow As Integer
    Public Property IdRuta As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadOrigen As Integer
    Public Property IdActividadDestino As Integer
    Public Property IdConector As Integer
    Public Property FechaUtc As DateTime
    Public Property DuracionMilisegundos As Long
    Public Property Resultado As String
    Public Property CodigoFuncional As String
    Public Property Referencia As String
End Class

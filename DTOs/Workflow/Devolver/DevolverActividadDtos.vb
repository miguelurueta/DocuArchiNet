Imports System
Imports System.Collections.Generic

' Contratos serializables exclusivos de Devolver a actividad anterior.
<Serializable()>
Public Class ErrorDevolverActividadDto
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property ReferenciaTrazabilidad As String
End Class

<Serializable()>
Public Class ContextoDevolverActividadDto
    Public Property Radicado As String
    Public Property ActividadActual As String
    Public Property GrupoActual As String
    Public Property TipoContexto As String
End Class

<Serializable()>
Public Class DestinoDevolverActividadDto
    Public Property IdConector As Integer
    Public Property NombreActividad As String
    Public Property Destinatario As String
    Public Property GrupoDestino As String
    Public Property TipoContexto As String
    Public Property Orden As Integer
End Class

<Serializable()>
Public Class PrevisualizacionDevolverActividadDto
    Public Sub New()
        Contexto = New ContextoDevolverActividadDto()
        Destinos = New List(Of DestinoDevolverActividadDto)()
    End Sub

    Public Property IdTarea As Long
    Public Property Contexto As ContextoDevolverActividadDto
    Public Property TokenVersion As String
    Public Property HayMas As Boolean
    Public Property CursorSiguiente As String
    Public Property TamanoPagina As Integer
    Public Property Destinos As IList(Of DestinoDevolverActividadDto)
    Public Property [Error] As ErrorDevolverActividadDto
End Class

<Serializable()>
Public Class ResultadoDevolverActividadDto
    Public Sub New()
        Advertencias = New List(Of String)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property EsReintentable As Boolean
    Public Property TokenVersion As String
    Public Property ActividadDestino As String
    Public Property ReferenciaAuditoria As String
    Public Property Advertencias As IList(Of String)
    Public Property [Error] As ErrorDevolverActividadDto
End Class

Public NotInheritable Class CodigosBloqueoDevolverActividad
    Public Const ContextoInvalido As String = "WORKFLOW_RETURN_CONTEXT_INVALID"
    Public Const TareaInvalida As String = "WORKFLOW_RETURN_TASK_INVALID"
    Public Const TareaNoDisponible As String = "WORKFLOW_RETURN_TASK_UNAVAILABLE"
    Public Const PermisoDenegado As String = "WORKFLOW_RETURN_FORBIDDEN"
    Public Const ContextoInconsistente As String = "WORKFLOW_RETURN_CONTEXT_INCONSISTENT"
    Public Const ConectorInvalido As String = "WORKFLOW_RETURN_CONNECTOR_INVALID"
    Public Const ConectorNoDisponible As String = "WORKFLOW_RETURN_CONNECTOR_UNAVAILABLE"
    Public Const TerminoInvalido As String = "WORKFLOW_RETURN_SEARCH_TERM_INVALID"
    Public Const CursorInvalido As String = "WORKFLOW_RETURN_CURSOR_INVALID"
    Public Const VersionInvalida As String = "WORKFLOW_RETURN_VERSION_INVALID"
    Public Const VersionConflicto As String = "WORKFLOW_RETURN_VERSION_CONFLICT"
    Public Const EnProgreso As String = "WORKFLOW_RETURN_IN_PROGRESS"
    Public Const NoDisponible As String = "WORKFLOW_RETURN_UNAVAILABLE"
    Public Const Rechazada As String = "WORKFLOW_RETURN_REJECTED"
    Public Const SinDestinos As String = "WORKFLOW_RETURN_NO_DESTINATIONS"

    Private Sub New()
    End Sub
End Class

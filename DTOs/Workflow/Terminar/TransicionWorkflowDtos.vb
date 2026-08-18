Imports System
Imports System.Collections.Generic

'DTOs serializables para endpoints posteriores. No exponen SQL, HTML, Session ni excepciones.
<Serializable()>
Public Class DestinoTransicionDto
    Public Property Id As Integer
    Public Property IdActividadDestino As Integer
    Public Property Nombre As String
    Public Property Destinatario As String
    Public Property Grupo As String
    Public Property Tipo As String
    Public Property Orden As Integer
End Class

<Serializable()>
Public Class ContextoPrevisualizacionTransicionDto
    Public Property Radicado As String
    Public Property ActividadOrigen As String
    Public Property GrupoActual As String
End Class

<Serializable()>
Public Class RequisitoTransicionDto
    Public Property Codigo As String
    Public Property Descripcion As String
    Public Property Obligatorio As Boolean
    Public Property Satisfecho As Boolean
End Class

<Serializable()>
Public Class ResultadoPrevisualizacionDestinosDto
    Public Sub New()
        Destinos = New List(Of DestinoTransicionDto)()
    End Sub

    Public Property Destinos As IList(Of DestinoTransicionDto)
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

<Serializable()>
Public Class ErrorTransicionDto
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property ReferenciaTrazabilidad As String
End Class

<Serializable()>
Public Class PrevisualizacionTransicionDto
    Public Sub New()
        Destinos = New List(Of DestinoTransicionDto)()
        Requisitos = New List(Of RequisitoTransicionDto)()
        Contexto = New ContextoPrevisualizacionTransicionDto()
    End Sub

    Public Property IdTarea As Long
    Public Property Origen As String
    Public Property TipoDecision As String
    Public Property Contexto As ContextoPrevisualizacionTransicionDto
    Public Property Destinos As IList(Of DestinoTransicionDto)
    Public Property Requisitos As IList(Of RequisitoTransicionDto)
    Public Property RequiereNotificacion As Boolean
    Public Property TokenVersion As String
    Public Property [Error] As ErrorTransicionDto
End Class

<Serializable()>
Public Class ResultadoTransicionDto
    Public Sub New()
        Advertencias = New List(Of String)()
        Requisitos = New List(Of RequisitoTransicionDto)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property MensajeFuncional As String
    Public Property CodigoBloqueo As String
    Public Property Advertencias As IList(Of String)
    Public Property ActividadDestino As String
    Public Property Destino As DestinoTransicionDto
    Public Property TokenVersion As String
    Public Property ReferenciaAuditoria As String
    Public Property EsReintentable As Boolean
    Public Property Requisitos As IList(Of RequisitoTransicionDto)
    Public Property [Error] As ErrorTransicionDto
End Class

<Serializable()>
Public Class HabilitacionWorkflowModernDto
    Public Property Estado As String
    Public Property Codigo As String
    Public Property MensajeFuncional As String
    Public Property Activo As Boolean
End Class

Public NotInheritable Class CodigosBloqueoPrevisualizacion
    Public Const ContextoInvalido As String = "WORKFLOW_CONTEXT_INVALID"
    Public Const ModernoInactivo As String = "WORKFLOW_MODERN_INACTIVE"
    Public Const ModernoAlcancePilotoRequerido As String = "WORKFLOW_MODERN_PILOT_SCOPE_REQUIRED"
    Public Const ModernoMetadatosPilotoInvalidos As String = "WORKFLOW_MODERN_PILOT_METADATA_INVALID"
    Public Const ModernoRollbackActivo As String = "WORKFLOW_MODERN_ROLLBACK_ACTIVE"
    Public Const ModernoAlcanceOficialInconsistente As String = "WORKFLOW_MODERN_OFFICIAL_SCOPE_CONFLICT"
    Public Const TareaInvalida As String = "WORKFLOW_TASK_INVALID"
    Public Const TareaNoDisponible As String = "WORKFLOW_TASK_UNAVAILABLE"
    Public Const RutaCerrada As String = "WORKFLOW_ROUTE_CLOSED"
    Public Const ConectorNoDisponible As String = "WORKFLOW_CONNECTOR_UNAVAILABLE"
    Public Const ConectorInvalido As String = "WORKFLOW_CONNECTOR_INVALID"
    Public Const TransicionInconsistente As String = "WORKFLOW_TRANSITION_INCONSISTENT"
    Public Const SinDestinos As String = "WORKFLOW_NO_DESTINATIONS"
    Public Const VersionInvalida As String = "WORKFLOW_VERSION_INVALID"
    Public Const VersionConflicto As String = "WORKFLOW_VERSION_CONFLICT"
    Public Const TransicionEnProgreso As String = "WORKFLOW_TRANSITION_IN_PROGRESS"
    Public Const TransicionNoDisponible As String = "WORKFLOW_TRANSITION_UNAVAILABLE"
    Public Const RequisitoNoCumplido As String = "WORKFLOW_REQUIREMENT_NOT_MET"
    Public Const TransicionRechazada As String = "WORKFLOW_TRANSITION_REJECTED"

    Private Sub New()
    End Sub
End Class

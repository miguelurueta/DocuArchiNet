Imports System
Imports System.Collections.Generic

'Contratos serializables exclusivos de Devolver a usuario anterior.
<Serializable()>
Public Class ErrorDevolverUsuarioAnteriorDto
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property ReferenciaTrazabilidad As String
End Class

<Serializable()>
Public Class ContextoDevolverUsuarioAnteriorDto
    Public Property ActividadActual As String
    Public Property ActividadAnterior As String
    Public Property UsuarioAnterior As String
End Class

<Serializable()>
Public Class PrevisualizacionDevolverUsuarioAnteriorDto
    Public Sub New()
        Contexto = New ContextoDevolverUsuarioAnteriorDto()
    End Sub

    Public Property IdTarea As Long
    Public Property Contexto As ContextoDevolverUsuarioAnteriorDto
    Public Property TokenVersion As String
    Public Property [Error] As ErrorDevolverUsuarioAnteriorDto
End Class

<Serializable()>
Public Class ResultadoDevolverUsuarioAnteriorDto
    Public Sub New()
        Advertencias = New List(Of String)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property EsReintentable As Boolean
    Public Property ActividadDestino As String
    Public Property UsuarioDestino As String
    Public Property ReferenciaAuditoria As String
    Public Property Advertencias As IList(Of String)
    Public Property [Error] As ErrorDevolverUsuarioAnteriorDto
End Class

Public NotInheritable Class CodigosBloqueoDevolverUsuarioAnterior
    Public Const ContextoInvalido As String = "WORKFLOW_RETURN_USER_CONTEXT_INVALID"
    Public Const PermisoDenegado As String = "WORKFLOW_RETURN_USER_FORBIDDEN"
    Public Const TareaInvalida As String = "WORKFLOW_RETURN_USER_TASK_INVALID"
    Public Const TareaNoDisponible As String = "WORKFLOW_RETURN_USER_TASK_UNAVAILABLE"
    Public Const HistorialNoDisponible As String = "WORKFLOW_RETURN_USER_HISTORY_UNAVAILABLE"
    Public Const HistorialGrupo As String = "WORKFLOW_RETURN_USER_HISTORY_GROUP"
    Public Const DestinoNoDisponible As String = "WORKFLOW_RETURN_USER_DESTINATION_UNAVAILABLE"
    Public Const AutoDevolucion As String = "WORKFLOW_RETURN_USER_SELF"
    Public Const VersionInvalida As String = "WORKFLOW_RETURN_USER_VERSION_INVALID"
    Public Const VersionConflicto As String = "WORKFLOW_RETURN_USER_VERSION_CONFLICT"
    Public Const EnProgreso As String = "WORKFLOW_RETURN_USER_IN_PROGRESS"
    Public Const Rechazada As String = "WORKFLOW_RETURN_USER_REJECTED"
    Public Const NoDisponible As String = "WORKFLOW_RETURN_USER_UNAVAILABLE"

    Private Sub New()
    End Sub
End Class

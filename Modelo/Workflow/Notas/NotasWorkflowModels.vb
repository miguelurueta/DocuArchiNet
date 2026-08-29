Imports System
Imports System.Collections.Generic

'Modelos internos de Notas. No transportan Session, controles WebForms ni metadatos de ruta recibidos desde cliente.
Public NotInheritable Class CodigosResultadoNotasWorkflow
    Public Const Exito As String = "OK"
    Public Const Forbidden As String = "Forbidden"
    Public Const TaskNotActive As String = "TaskNotActive"
    Public Const NoteNotFound As String = "NoteNotFound"
    Public Const NotOwner As String = "NotOwner"
    Public Const VersionConflict As String = "VersionConflict"
    Public Const InvalidContent As String = "InvalidContent"
    Public Const Unavailable As String = "Unavailable"

    Private Sub New()
    End Sub
End Class

Public Class NotaWorkflow
    Public Property IdNota As Long
    Public Property IdTarea As Long
    Public Property IdAutorWorkflow As Integer
    Public Property IdActividadOrigen As Integer
    Public Property Contenido As String
    Public Property Version As String
    Public Property FechaCreacionUtc As DateTime
End Class

Public Class SolicitudListarNotasWorkflow
    Public Property IdTarea As Long
    Public Property Cursor As String
    Public Property TamanoPagina As Integer
End Class

Public Class SolicitudContarNotasWorkflow
    Public Property IdTarea As Long
End Class

Public Class SolicitudCrearNotaWorkflow
    Public Property IdTarea As Long
    Public Property Contenido As String
    Public Property IdSolicitudCliente As String
End Class

Public Class SolicitudConsultarNotaWorkflow
    Public Property IdTarea As Long
    Public Property IdNota As Long
End Class

Public Class SolicitudActualizarNotaWorkflow
    Public Property IdTarea As Long
    Public Property IdNota As Long
    Public Property Contenido As String
    Public Property Version As String
End Class

Public Class SolicitudEliminarNotaWorkflow
    Public Property IdTarea As Long
    Public Property IdNota As Long
    Public Property Version As String
End Class

Public Class ResultadoNotasWorkflow
    Public Sub New()
        Notas = New List(Of NotaWorkflow)()
        Codigo = CodigosResultadoNotasWorkflow.Exito
        MensajeFuncional = String.Empty
    End Sub

    Public Property Codigo As String
    Public Property MensajeFuncional As String
    Public Property Nota As NotaWorkflow
    Public Property Notas As IList(Of NotaWorkflow)
    Public Property Contador As Integer
    Public Property CursorSiguiente As String
    Public Property TieneMas As Boolean

    Public ReadOnly Property EsExitoso As Boolean
        Get
            Return String.Equals(Codigo, CodigosResultadoNotasWorkflow.Exito, StringComparison.Ordinal)
        End Get
    End Property
End Class

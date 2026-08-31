Imports System
Imports System.Collections.Generic

'DTOs reservados para el futuro transporte de Notas. No incluyen identidad, permisos, ruta ni detalles de infraestructura.
<Serializable()>
Public Class NotaWorkflowDto
    Public Property IdNota As Long
    Public Property IdTarea As Long
    Public Property Autor As String
    Public Property ActividadOrigen As String
    Public Property Contenido As String
    Public Property Version As String
    Public Property FechaCreacionUtc As DateTime
End Class

<Serializable()>
Public Class SolicitudListarNotasDto
    Public Property IdTarea As Long
    Public Property Cursor As String
    Public Property TamanoPagina As Integer
End Class

<Serializable()>
Public Class SolicitudContarNotasDto
    Public Property IdTarea As Long
End Class

<Serializable()>
Public Class SolicitudCrearNotaDto
    Public Property IdTarea As Long
    Public Property Contenido As String
    Public Property IdSolicitudCliente As String
End Class

<Serializable()>
Public Class SolicitudConsultarNotaDto
    Public Property IdTarea As Long
    Public Property IdNota As Long
End Class

<Serializable()>
Public Class SolicitudActualizarNotaDto
    Public Property IdTarea As Long
    Public Property IdNota As Long
    Public Property Contenido As String
    Public Property Version As String
End Class

<Serializable()>
Public Class SolicitudEliminarNotaDto
    Public Property IdTarea As Long
    Public Property IdNota As Long
    Public Property Version As String
End Class

<Serializable()>
Public Class ResultadoNotasDto
    Public Sub New()
        Notas = New List(Of NotaWorkflowDto)()
    End Sub

    Public Property Codigo As String
    Public Property CodigoBloqueo As String
    Public Property Exito As Boolean
    Public Property MensajeFuncional As String
    Public Property Nota As NotaWorkflowDto
    Public Property Notas As IList(Of NotaWorkflowDto)
    Public Property Contador As Integer
    Public Property CursorSiguiente As String
    Public Property TieneMas As Boolean
End Class

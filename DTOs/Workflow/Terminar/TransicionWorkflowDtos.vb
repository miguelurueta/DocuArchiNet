Imports System
Imports System.Collections.Generic

'DTOs serializables para endpoints posteriores. No exponen SQL, HTML, Session ni excepciones.
<Serializable()>
Public Class DestinoTransicionDto
    Public Property Id As Integer
    Public Property Nombre As String
    Public Property Tipo As String
    Public Property Orden As Integer
End Class

<Serializable()>
Public Class RequisitoTransicionDto
    Public Property Codigo As String
    Public Property Descripcion As String
    Public Property Obligatorio As Boolean
    Public Property Satisfecho As Boolean
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
    End Sub

    Public Property IdTarea As Long
    Public Property Origen As String
    Public Property TipoDecision As String
    Public Property Destinos As IList(Of DestinoTransicionDto)
    Public Property Requisitos As IList(Of RequisitoTransicionDto)
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

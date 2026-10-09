Imports System

' DTO públicos previstos para una frontera posterior. No incluyen identidad ni estado interno.
<Serializable()>
Public Class SegundoFactorEstadoDto
    Public Property RequiereCodigo As Boolean
    Public Property DestinoEnmascarado As String
    Public Property ExpiraEnSegundos As Integer
    Public Property ReenvioDisponibleEnSegundos As Integer
End Class

<Serializable()>
Public Class SegundoFactorResultadoDto
    Public Property Exito As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property Estado As SegundoFactorEstadoDto
End Class

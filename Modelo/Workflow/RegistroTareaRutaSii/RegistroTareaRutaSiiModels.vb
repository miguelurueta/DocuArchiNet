Imports System

Public Class SolicitudRegistroTareaRutaSii
    Public Property recibo As String
    Public Property id_tramite As Integer
    Public Property id_actividad As Integer
End Class

Public Class ConsultaAutoritativaRegistroRutaSii
    Public Property Recibo As Class_parram_consultarRecibo
    Public Property Radicado As Class_parram_consultarRadicado
End Class

Public Class DatosAutoritativosRegistroRutaSii
    Public Property Recibo As String
    Public Property CodigoBarras As String
    Public Property Matricula As String
    Public Property RazonSocial As String
    Public Property SubtipoTramite As String
    Public Property IdTramite As Integer
    Public Property NombreTramite As String
    Public Property DescripcionTramite As String
    Public Property IdActividad As Integer
    Public Property CodigoSede As String
    Public Property IdGabinete As Integer
    Public Property NombreGabinete As String
    Public Property IdRuta As Integer
    Public Property NombreRuta As String
End Class

Public Class EventoRelacionRutaSii
    Public Property OperationId As String
    Public Property IdTarea As Long
    Public Property IdRuta As Integer
    Public Property Recibo As String
    Public Property Matricula As String
    Public Property NombreGabinete As String
End Class

Public Class ResultadoRegistroTareaRutaSii
    Public Property Codigo As String
    Public Property Mensaje As String
    Public Property Evento As EventoRelacionRutaSii
End Class

Option Strict On
Imports System
Imports System.Collections.Generic

Public Class ReferenciaConexionExcel
    Public Property ConexionId As UInteger
    Public Property Nombre As String
End Class
Public Class AperturaImportacionExcel
    Public Property CsrfToken As String
    Public Property MaxFilasDatos As Integer
    Public Property MaxArchivoMB As Integer
    Public Property Conexiones As New List(Of ReferenciaConexionExcel)
End Class
Public Class SolicitudListaPlantillasExcel
    Public Property ConexionId As UInteger
    Public Property Termino As String
    Public Property Pagina As Integer = 1
    Public Property TamanoPagina As Integer = 25
End Class
Public Class SolicitudDetallePlantillaExcel
    Public Property ConexionId As UInteger
    Public Property PlantillaId As UInteger
End Class
Public Class SolicitudGuardarPlantillaExcel
    Public Property ConexionId As UInteger
    Public Property RevisionEsperada As UInteger
    Public Property HuellaEsperada As String
    Public Property Plantilla As PlantillaExcel
End Class
Public Class RespuestaImportacionExcel
    Public Property ok As Boolean
    Public Property codigo As String
    Public Property mensaje As String
    Public Property operacionId As String
End Class
Public Class RespuestaAperturaExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As AperturaImportacionExcel
End Class
Public Class RespuestaListaPlantillasExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As PaginaPlantillasExcel
End Class
Public Class RespuestaPlantillaExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As PlantillaExcel
End Class

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
    Public Property PermiteNuevaCarga As Boolean = True
    Public Property CodigoCapacidad As String
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

Public Class SolicitudInicioRecepcionExcel
    Inherits SolicitudDetallePlantillaExcel
    Public Property RevisionEsperada As UInteger
    Public Property HuellaEsperada As String
End Class
Public Class InicioRecepcionExcel
    Public Property IdCarga As String
    Public Property ReferenciaRecepcion As String
    Public Property ExpiraUtc As DateTime
    Public Property RevisionPlantilla As UInteger
    Public Property PlantillaHash As String
    Public Property Formato As String
End Class
Public Class SolicitudRecepcionExcel
    Inherits SolicitudDetallePlantillaExcel
    Public Property IdCarga As String
    Public Property ReferenciaRecepcion As String
End Class
Public Class SolicitudPrepararExcel
    Inherits SolicitudDetallePlantillaExcel
    Public Property IdCarga As String
    Public Property RevisionEsperada As UInteger
    Public Property HuellaEsperada As String
    Public Property PreparacionHashEsperada As String
    Public Property Hoja As String
    Public Property Modo As String
End Class
Public Class SolicitudConsultaCargaExcel
    Public Property ConexionId As UInteger
    Public Property IdCarga As String
End Class
Public Class SolicitudConfirmarExcel
    Inherits SolicitudConsultaCargaExcel
    Public Property ArchivoHash As String
    Public Property ConfiguracionHash As String
    Public Property EsquemaHash As String
    Public Property ConfirmacionReemplazo As Boolean
End Class
'DTO seguro: no incluye Archivo.Referencia ni documento interno de configuración.
Public Class EstadoCargaExcel
    Public Property IdCarga As String
    Public Property PlantillaId As UInteger
    Public Property Estado As String
    Public Property NombreArchivo As String
    Public Property Bytes As Long
    Public Property ArchivoHash As String
    Public Property Tabla As String
    Public Property Modo As String
    Public Property RevisionPlantilla As Nullable(Of UInteger)
    Public Property ConfiguracionHash As String
    Public Property EsquemaHash As String
    Public Property FilasPrevistas As Nullable(Of Integer)
    Public Property CodigoIncidencia As String
    Public Property FilasConfirmadas As Integer
    Public Property ConfirmadoUtc As Nullable(Of DateTime)
    Public Property EstadoEstructura As String
    Public Property Hojas As New List(Of String)
End Class
Public Class PreparacionCargaExcel
    Public Property Carga As EstadoCargaExcel
    Public Property Codigo As String
    Public Property CampoUnico As String
    Public Property Hoja As String
    Public Property LecturaCompleta As Boolean
    Public Property ValidacionCompleta As Boolean
    Public Property DuplicadosEvaluados As Boolean
    Public Property FilasObservadas As Integer
    Public Property TotalIncidencias As Long
    Public Property Muestra As New List(Of FilaNormalizadaExcel)
    Public Property Incidencias As New List(Of IncidenciaLecturaExcel)
    Public ReadOnly Property IncidenciasTruncadas As Boolean
        Get
            Return TotalIncidencias > Incidencias.Count
        End Get
    End Property
End Class
Public Class RespuestaInicioRecepcionExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As InicioRecepcionExcel
End Class
Public Class RespuestaEstadoCargaExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As EstadoCargaExcel
End Class
Public Class RespuestaPreparacionExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As PreparacionCargaExcel
End Class

'Propuesta sin plantilla ni carga durable. No constituye validación de importación.
Public Class AnalisisPlantillaExcel
    Public Property Formato As String
    Public Property Hoja As String
    Public Property Hojas As New List(Of String)
    Public Property ArchivoHash As String
    Public Property FilasObservadas As Integer
    Public Property Campos As New List(Of CampoPlantillaExcel)
    Public Property Muestra As New List(Of List(Of String))
    Public Property Avisos As New List(Of String)
End Class
Public Class RespuestaAnalisisPlantillaExcel
    Inherits RespuestaImportacionExcel
    Public Property datos As AnalisisPlantillaExcel
End Class

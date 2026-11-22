Option Strict On
Imports System
Imports System.Collections.Generic

Public Class ImportacionExcelException
    Inherits Exception
    Public ReadOnly Property Codigo As String
    Public Sub New(codigo As String, mensaje As String)
        MyBase.New(mensaje)
        Me.Codigo = codigo
    End Sub
End Class

Public Class ContextoImportacionExcel
    Public Property UsuarioId As UInteger
    Public Property EmpresaId As UInteger
    Public Property Autorizado As Boolean
End Class

'Los nombres físicos permiten un snapshot compatible con los consumidores históricos.
Public Class CampoPlantillaExcel
    Public Property ID_CAMPOS_PLANTILLA As UInteger
    Public Property CAMPO_FUENTE As String
    Public Property CAMPO_DESTINO As String
    Public Property TIPO_CAMPO As String
    Public Property REMPLAZA_COMA As Nullable(Of UInteger)
    Public Property REMPLAZA_PUNTO As Nullable(Of UInteger)
    Public Property OPCION_SPLITER As Nullable(Of UInteger)
    Public Property CARACTER_SPLITER As String
    Public Property NUMERO_MATRIZ_EXPLITER As Nullable(Of UInteger)
End Class

Public Class PlantillaExcel
    Public Property ID_PLANTILLAIMP As UInteger
    Public Property IMP01_XMLCONEXION_ID_XMLCONEXION As UInteger
    Public Property NOMBRE_PLANTILLA As String
    Public Property CONEXION_FUENTE As String
    Public Property LIBRO_EXCEL As String
    Public Property RUTA_ARCHIVO_FUENTE As String
    Public Property ELIMINA_TABLA As UInteger
    Public Property ELIMINA_INSERTA_RECORD As UInteger
    Public Property INSERTA_RECORD As UInteger
    Public Property ESTADO_PLANTILLA As UInteger
    Public Property CAMPO_UNICO As String
    Public Property REVISION_CARGA As UInteger
    Public Property Campos As New List(Of CampoPlantillaExcel)
    Public Property Modo As String
    Public Property Formato As String
    Public Property Huella As String
End Class

Public Class PaginaPlantillasExcel
    Public Property Total As Integer
    Public Property Pagina As Integer
    Public Property TamanoPagina As Integer
    Public Property Plantillas As New List(Of ResumenPlantillaExcel)
End Class

Public Class ResumenPlantillaExcel
    Public Property PlantillaId As UInteger
    Public Property Nombre As String
    Public Property Revision As UInteger
    Public Property Activa As Boolean
End Class

Public Class ColumnaDestinoExcel
    Public Property Nombre As String
    Public Property Tipo As String
    Public Property TipoCompleto As String
    Public Property Longitud As Nullable(Of Long)
    Public Property Nulable As Boolean
    Public Property Predeterminado As String
    Public Property Charset As String
    Public Property Collation As String
    Public Property Extra As String
End Class

Public Class DestinoPlantillaExcel
    Public Property Existe As Boolean
    Public Property Motor As String
    Public Property Columnas As New List(Of ColumnaDestinoExcel)
End Class

Public Interface IPlantillaExcelRepository
    Function Listar(termino As String, pagina As Integer, tamano As Integer) As PaginaPlantillasExcel
    Function Obtener(id As UInteger) As PlantillaExcel
    Function Destino(tabla As String) As DestinoPlantillaExcel
    Function Guardar(plantilla As PlantillaExcel, revisionEsperada As UInteger, huellaEsperada As String) As PlantillaExcel
End Interface

'Contratos internos del lector. No transportan rutas, SQL ni autoridad de sesión.
Public Class LimitesLecturaExcel
    Public Property MaxFilasDatos As Integer = 10000
    Public Property MaxArchivoMB As Integer = 10
End Class
Public Class SolicitudLecturaExcel
    Public Property FormatoEsperado As String
    Public Property NombreArchivo As String
    Public Property Hoja As String
    Public Property Plantilla As PlantillaExcel
    Public Property Destino As DestinoPlantillaExcel
    Public Property Limites As New LimitesLecturaExcel
End Class
Public Class EncabezadoLecturaExcel
    Public Property Columna As Integer
    Public Property Nombre As String
End Class
Public Class IncidenciaLecturaExcel
    Public Property Codigo As String
    Public Property Mensaje As String
    Public Property Hoja As String
    Public Property Fila As Nullable(Of Integer)
    Public Property Columna As Nullable(Of Integer)
    Public Property CampoFuente As String
    Public Property CampoDestino As String
End Class
Public Class ValorNormalizadoExcel
    Public Property Columna As Integer
    Public Property CampoFuente As String
    Public Property CampoDestino As String
    'Tipo semántico; números canónicos evitan limitar DECIMAL65 al Decimal CLR.
    Public Property Tipo As String
    Public Property EsNulo As Boolean
    Public Property Texto As String
    Public Property NumeroCanonico As String
    Public Property Fecha As Nullable(Of DateTime)
End Class
Public Class FilaNormalizadaExcel
    Public Property Fila As Integer
    Public Property Valores As New List(Of ValorNormalizadoExcel)
End Class
Public Class ResultadoLecturaExcel
    Public Property Formato As String
    Public Property Hoja As String
    Public Property Hojas As New List(Of String)
    Public Property Encabezados As New List(Of EncabezadoLecturaExcel)
    Public Property Filas As New List(Of FilaNormalizadaExcel)
    Public Property FilasDatosObservadas As Integer
    Public Property TotalIncidencias As Long
    Public Property Incidencias As New List(Of IncidenciaLecturaExcel)
    Public Property LecturaCompleta As Boolean
    Public ReadOnly Property IncidenciasTruncadas As Boolean
        Get
            Return TotalIncidencias > Incidencias.Count
        End Get
    End Property
    Public ReadOnly Property Valido As Boolean
        Get
            Return LecturaCompleta AndAlso TotalIncidencias = 0 AndAlso FilasDatosObservadas > 0
        End Get
    End Property
End Class
Friend Class CeldaLibroExcel
    Friend Property Columna As Integer
    Friend Property Valor As Object
    Friend Property Formato As String
    Friend Property ErrorCodigo As String
End Class
Friend Class FilaLibroExcel
    Friend Property Fila As Integer
    Friend Property Celdas As New List(Of CeldaLibroExcel)
End Class

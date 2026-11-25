Option Strict On
Imports System
Imports System.Collections.Generic

Public Class ImportacionExcelException
    Inherits Exception
    Public ReadOnly Property Codigo As String
    Public Property Detalles As New List(Of DetalleConfiguracionExcel)
    Public Sub New(codigo As String, mensaje As String)
        MyBase.New(mensaje)
        Me.Codigo = codigo
    End Sub
End Class

'Sólo reglas y referencias de formulario; nunca SQL, valores de celda o mensajes del proveedor.
Public Class DetalleConfiguracionExcel
    Public Property Campo As String
    Public Property FilaCampo As Nullable(Of Integer)
    Public Property CampoDestino As String
    Public Property Regla As String
    Public Property Limite As Nullable(Of Integer)
    Public Property LongitudActual As Nullable(Of Integer)
    Public Property TipoEsperado As String
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

Public Class ContextoCargaExcel
    Inherits ContextoImportacionExcel
    Public Property ModuloId As UInteger
    Public Property ConexionId As UInteger
    Public Property IdentidadConexion As String
    Public ReadOnly Property ConexionRef As String
        Get
            Return "DA:" & ModuloId.ToString(Globalization.CultureInfo.InvariantCulture) & ":" & ConexionId.ToString(Globalization.CultureInfo.InvariantCulture)
        End Get
    End Property
End Class
Public Class ArchivoRecibidoExcel
    Public Property Nombre As String
    Public Property Referencia As String
    Public Property Bytes As Long
    Public Property Hash As String
    Public Property Formato As String
    Public Property Hojas As New List(Of String)
End Class
Public Class CargaExcelEjecucion
    Public Property Id As String
    Public Property PlantillaId As UInteger
    Public Property EmpresaId As UInteger
    Public Property UsuarioId As UInteger
    Public Property ConexionRef As String
    Public Property Tabla As String
    Public Property Archivo As ArchivoRecibidoExcel
    Public Property Estado As String
    Public Property Modo As String
    Public Property RevisionPlantilla As Nullable(Of UInteger)
    Public Property ConfiguracionDoc As String
    Public Property ConfiguracionHash As String
    Public Property EsquemaHash As String
    Public Property FilasPrevistas As Nullable(Of Integer)
    Public Property CodigoIncidencia As String
    Public Property FilasConfirmadas As Integer
    Public Property ConfirmadoUtc As Nullable(Of DateTime)
End Class
Public Class PoliticaDestinoCargaExcel
    Public Property FuenteId As String
    Public Property AmbitoRef As String
    Public Property Revision As UInteger
    Public Property Modo As String
    Public Property ColumnaEmpresa As String
End Class
Public Class ComponenteIndiceCargaExcel
    Public Property Columna As String
    Public Property Posicion As Integer
    Public Property Prefijo As Nullable(Of Integer)
End Class
Public Class IndiceCargaExcel
    Public Property Nombre As String
    Public Property Unico As Boolean
    Public Property Componentes As New List(Of ComponenteIndiceCargaExcel)
End Class
Public Class EsquemaCargaExcel
    Public Property Destino As DestinoPlantillaExcel
    Public Property Indices As New List(Of IndiceCargaExcel)
    Public Property Politica As PoliticaDestinoCargaExcel
    Public Property IdentidadFisica As String
    Public Property Hash As String
End Class
Public Class IncidenciasClavesExcel
    Public Property Total As Long
    Public Property Muestra As New List(Of IncidenciaLecturaExcel)
End Class
Public Interface IArchivoRetenidoExcel
    Inherits IDisposable
    ReadOnly Property Contenido As IO.Stream
    Sub Verificar()
End Interface

'La confirmación amplía el repositorio sin cambiar las sesiones de recepción DOC124.
Public Interface IConfirmacionExcelRepository
    Function Consultar(ctx As ContextoCargaExcel, id As String) As CargaExcelEjecucion
    Function AbrirConfirmacion(ctx As ContextoCargaExcel, id As String) As ISesionConfirmacionExcel
    Function ConsultarEstructura(ctx As ContextoCargaExcel, carga As CargaExcelEjecucion, plantilla As PlantillaExcel) As String
End Interface
Public Interface ISesionConfirmacionExcel
    Inherits IDisposable
    Function Obtener() As CargaExcelEjecucion
    Sub BloquearDestino(tabla As String)
    Function LeerPlantilla(id As UInteger) As PlantillaExcel
    Function Inspeccionar(p As PlantillaExcel, carga As CargaExcelEjecucion) As EsquemaCargaExcel
    Function ValidarClaves(p As PlantillaExcel, e As EsquemaCargaExcel, filas As List(Of FilaNormalizadaExcel), modo As String) As IncidenciasClavesExcel
    Sub AsegurarEstructura(p As PlantillaExcel, carga As CargaExcelEjecucion)
    Function Ejecutar(carga As CargaExcelEjecucion, filas As List(Of FilaNormalizadaExcel), validar As Action(Of PlantillaExcel, EsquemaCargaExcel)) As CargaExcelEjecucion
End Interface
Public Interface IAlmacenArchivoExcel
    Function Recibir(ctx As ContextoCargaExcel, id As String, nombre As String, formato As String, contenido As IO.Stream, limites As LimitesLecturaExcel) As ArchivoRecibidoExcel
    Function Retener(ctx As ContextoCargaExcel, id As String, archivo As ArchivoRecibidoExcel) As IArchivoRetenidoExcel
End Interface
Public Interface ICargaExcelRepository
    Function Abrir(ctx As ContextoCargaExcel, id As String) As ISesionCargaExcel
    Function Consultar(ctx As ContextoCargaExcel, id As String) As CargaExcelEjecucion
End Interface
Public Interface ISesionCargaExcel
    Inherits IDisposable
    Function Obtener() As CargaExcelEjecucion
    Function RegistrarRecibida(ejecucion As CargaExcelEjecucion) As CargaExcelEjecucion
    Function Inspeccionar(plantilla As PlantillaExcel, modo As String) As EsquemaCargaExcel
    Function ValidarClaves(plantilla As PlantillaExcel, esquema As EsquemaCargaExcel, filas As List(Of FilaNormalizadaExcel), modo As String) As IncidenciasClavesExcel
    Function Publicar(preparada As CargaExcelEjecucion, hashEsperado As String) As CargaExcelEjecucion
    Sub Rechazar(hashEsperado As String, codigo As String)
End Interface

Option Strict On
Imports System.Collections.Generic

'Configuracion administrativa de solo lectura; nunca se recibe del navegador.
Public Class ProveedorIndicesVersion
    Public Property Estado As String = "BORRADOR"
    Public Property Revision As UInteger = 1UI
    Public Property ProveedorId As String
    Public Property VersionId As String
    Public Property AmbitoRef As String
    Public Property Activo As Boolean
    Public Property Tipo As String = "TABLA"
    Public Property ConexionRef As String
    Public Property Tabla As String
    Public Property ColumnaClave As String
    Public Property TipoClave As String = "TEXTO"
    Public Property ColumnaEmpresa As String
    Public Property Propiedades As New List(Of PropiedadProveedorIndices)()
End Class

Public Class PropiedadProveedorIndices
    Public Property Nombre As String
    Public Property Columna As String
    Public Property Tipo As String = "TEXTO"
End Class

Public Class ResultadoProveedorIndices
    'OK, SIN_COINCIDENCIA, AMBIGUO, CONFIGURACION_INVALIDA,
    'ACCESO_DENEGADO, CLAVE_INVALIDA, ORIGEN_NO_DISPONIBLE.
    Public Property Codigo As String
    Public Property Valores As New Dictionary(Of String, String)(System.StringComparer.Ordinal)
End Class

Public Interface IProveedorDatosIndices
    Function Consultar(contexto As ContextoPerfilImportacion, version As ProveedorIndicesVersion,
                       clave As String) As ResultadoProveedorIndices
End Interface

Public Interface IProveedorIndicesRepository
    Function Listar(contexto As ContextoPerfilImportacion, pagina As Integer, tamano As Integer, proveedorId As String, versionId As String) As List(Of OpcionProveedorIndices)
    Function Obtener(contexto As ContextoPerfilImportacion, proveedorId As String,
                     versionId As String) As ProveedorIndicesVersion
End Interface

'Contrato visible: no incluye conexion, tabla, columnas fisicas ni credenciales.
Public Class OpcionProveedorIndices
    Public Property ProveedorId As String
    Public Property VersionId As String
    Public Property Nombre As String
    Public Property Numero As UInteger
    Public Property TipoClave As String
    Public Property Propiedades As New List(Of OpcionPropiedadProveedorIndices)()
End Class
Public Class OpcionPropiedadProveedorIndices
    Public Property Nombre As String
    Public Property Tipo As String
End Class

Public Class FuenteProveedorIndices
    Public Property ProveedorId As String
    Public Property VersionId As String
    Public Property Propiedad As String
    Public Property Clave As ClaveProveedorIndices
End Class

Public Class ClaveProveedorIndices
    Public Property FuenteTipo As String
    Public Property ValorConstante As String
    Public Property PropiedadExpediente As String
    Public Property Extraccion As String = "COMPLETO"
    Public Property Separador As String
    Public Property Posicion As Integer
    Public Property QuitarExtension As Boolean
    Public Property RecortarEspacios As Boolean
    Public Property TipoValor As String = "TEXTO"
    Public Property FormatoFecha As String
    Public Property LongitudMaxima As Integer = 160
End Class

Option Strict On
Imports System
Imports System.Collections.Generic

Public Class DocumentoFuenteImportacion
    Public Property FuenteId As String
    Public Property ConexionRef As String
    Public Property Tabla As String
    Public Property ModoAislamiento As String
    Public Property ColumnaEmpresa As String
    Public Property Columnas As New List(Of String)()
    Public Property Activo As Boolean
    Public Property Revision As UInteger
End Class
Public Class CambioFuenteImportacion
    Public Property Documento As DocumentoFuenteImportacion
    Public Property Motivo As String
End Class
Public Class SolicitudFuenteImportacion
    Public Property FuenteId As String
    Public Property Pagina As Integer = 1
    Public Property Termino As String
    Public Property Cambio As CambioFuenteImportacion
End Class
Public Class HistorialFuenteImportacion
    Public Property HistorialId As String
    Public Property Revision As UInteger
    Public Property Accion As String
    Public Property ActorRef As String
    Public Property FechaUtc As DateTime
    Public Property Motivo As String
    Public Property Estado As DocumentoFuenteImportacion
End Class
Public Interface IFuenteImportacionRepository
    Function Listar(c As ContextoPerfilImportacion, pagina As Integer, termino As String) As List(Of DocumentoFuenteImportacion)
    Function Obtener(c As ContextoPerfilImportacion, fuenteId As String) As DocumentoFuenteImportacion
    Function Guardar(c As ContextoPerfilImportacion, accion As String, cambio As CambioFuenteImportacion) As DocumentoFuenteImportacion
    Function Historial(c As ContextoPerfilImportacion, fuenteId As String, pagina As Integer) As List(Of HistorialFuenteImportacion)
End Interface

Option Strict On
Imports System.Collections.Generic

Public Class ContratoProveedorIndices
    Public Property ConexionRef As String
    Public Property Tabla As String
    Public Property ColumnaClave As String
    Public Property TipoClave As String = "TEXTO"
    Public Property ColumnaEmpresa As String
    Public Property Propiedades As New List(Of PropiedadProveedorIndices)()
End Class
Public Class DocumentoProveedorAdmin
    Public Property ProveedorId As String
    Public Property VersionId As String
    Public Property Codigo As String
    Public Property Nombre As String
    Public Property Activo As Boolean
    Public Property RevisionProveedor As UInteger
    Public Property Numero As UInteger
    Public Property Estado As String
    Public Property Revision As UInteger
    Public Property Contrato As New ContratoProveedorIndices()
End Class
Public Class SolicitudProveedorAdmin
    Public Property ProveedorId As String
    Public Property VersionId As String
    Public Property Codigo As String
    Public Property Nombre As String
    Public Property RevisionProveedor As UInteger
    Public Property Revision As UInteger
    Public Property Activo As Boolean
    Public Property Contrato As ContratoProveedorIndices
    Public Property ClaveEjemplo As String
    Public Property Pagina As Integer = 1
    Public Property Termino As String
End Class
Public Interface IAdministracionProveedorRepository
    Function Listar(c As ContextoPerfilImportacion, solicitud As SolicitudProveedorAdmin) As List(Of DocumentoProveedorAdmin)
    Function Obtener(c As ContextoPerfilImportacion, proveedorId As String, versionId As String) As DocumentoProveedorAdmin
    Function Mutar(c As ContextoPerfilImportacion, accion As String, solicitud As SolicitudProveedorAdmin, huella As Byte()) As DocumentoProveedorAdmin
End Interface

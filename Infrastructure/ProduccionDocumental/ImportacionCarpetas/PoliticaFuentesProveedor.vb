Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

'Allowlist de servidor independiente del proveedor que el usuario esta creando.
Public Class FuenteProveedorAutorizada
    Public Property AmbitoRef As String
    Public Property ConexionRef As String
    Public Property ExclusivaEmpresa As Boolean = False
    Public Property ColumnaEmpresaAutorizada As String
    Public Property Tabla As String
    Public Property Columnas As New List(Of String)()
End Class

Public Class PoliticaFuentesProveedor
    Private ReadOnly _fuentes As List(Of FuenteProveedorAutorizada)
    Private ReadOnly _tablas As RestriccionTablasFuente
    Public Sub New(documento As String, Optional tablas As RestriccionTablasFuente = Nothing)
        _tablas = If(tablas, New RestriccionTablasFuente())
        _fuentes = PerfilImportacionJson.Leer(Of List(Of FuenteProveedorAutorizada))(If(String.IsNullOrWhiteSpace(documento), "[]", documento))
        If _fuentes Is Nothing OrElse _fuentes.Count > 256 Then Throw Invalida()
        Dim identidades As New HashSet(Of String)(StringComparer.Ordinal)
        For Each f In _fuentes
            If f Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(f.AmbitoRef) OrElse Not {"RA", "DA"}.Contains(f.ConexionRef) OrElse Not Identificador(f.Tabla) OrElse f.Columnas Is Nothing OrElse f.Columnas.Count < 1 OrElse f.Columnas.Count > 128 Then Throw Invalida()
            If f.Columnas.Any(Function(c) Not Identificador(c)) OrElse f.Columnas.Distinct(StringComparer.OrdinalIgnoreCase).Count() <> f.Columnas.Count OrElse Not identidades.Add(f.AmbitoRef & ":" & f.ConexionRef & ":" & f.Tabla) Then Throw Invalida()
            If f.ColumnaEmpresaAutorizada IsNot Nothing Then
                If f.ExclusivaEmpresa Then
                    If f.ColumnaEmpresaAutorizada <> "" Then Throw Invalida()
                ElseIf Not f.Columnas.Contains(f.ColumnaEmpresaAutorizada) Then
                    Throw Invalida()
                End If
            End If
        Next
    End Sub
    Public Function Fuentes(contexto As ContextoPerfilImportacion) As List(Of FuenteProveedorAutorizada)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Contexto no autorizado.")
        Return _fuentes.Where(Function(f) f.AmbitoRef = contexto.AmbitoRef AndAlso _tablas.Permite(f.ConexionRef, f.Tabla)).Select(Function(f) New FuenteProveedorAutorizada With {.AmbitoRef = f.AmbitoRef, .ConexionRef = f.ConexionRef, .Tabla = f.Tabla, .ExclusivaEmpresa = f.ExclusivaEmpresa, .ColumnaEmpresaAutorizada = f.ColumnaEmpresaAutorizada, .Columnas = f.Columnas.ToList()}).ToList()
    End Function
    Public Function Permite(contexto As ContextoPerfilImportacion, version As ProveedorIndicesVersion) As Boolean
        If version Is Nothing OrElse version.Propiedades Is Nothing Then Return False
        Dim f = Fuentes(contexto).SingleOrDefault(Function(x) x.ConexionRef = version.ConexionRef AndAlso x.Tabla = version.Tabla)
        If f IsNot Nothing AndAlso f.ColumnaEmpresaAutorizada IsNot Nothing AndAlso If(version.ColumnaEmpresa, "") <> f.ColumnaEmpresaAutorizada Then Return False
        Return f IsNot Nothing AndAlso f.Columnas.Contains(version.ColumnaClave) AndAlso (If(String.IsNullOrEmpty(version.ColumnaEmpresa), f.ExclusivaEmpresa, f.Columnas.Contains(version.ColumnaEmpresa))) AndAlso version.Propiedades.All(Function(p) p IsNot Nothing AndAlso f.Columnas.Contains(p.Columna))
    End Function
    Private Shared Function Identificador(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z")
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La politica de fuentes no es valida.")
    End Function
End Class

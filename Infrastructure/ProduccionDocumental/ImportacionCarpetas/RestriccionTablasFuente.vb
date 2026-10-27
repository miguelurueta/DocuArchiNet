Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class TablaFuenteAdicional
    Public Property ConexionRef As String
    Public Property Tabla As String
End Class

'Limite de seguridad del servidor, independiente del permiso de administrar fuentes.
Public Class RestriccionTablasFuente
    Public Const Prefijo As String = "imp_02"
    Private ReadOnly _adicionales As List(Of TablaFuenteAdicional)
    Public Sub New(Optional documento As String = Nothing)
        _adicionales = PerfilImportacionJson.Leer(Of List(Of TablaFuenteAdicional))(If(String.IsNullOrWhiteSpace(documento), "[]", documento))
        If _adicionales Is Nothing OrElse _adicionales.Count > 64 Then Throw Invalida()
        Dim claves As New HashSet(Of String)(StringComparer.Ordinal)
        For Each x In _adicionales
            If x Is Nothing OrElse Not {"RA", "DA"}.Contains(x.ConexionRef) OrElse Not Nombre(x.Tabla) OrElse Not claves.Add(x.ConexionRef & ":" & x.Tabla) Then Throw Invalida()
        Next
    End Sub
    Public Function Permite(conexion As String, tabla As String) As Boolean
        Return {"RA", "DA"}.Contains(conexion) AndAlso Nombre(tabla) AndAlso (tabla.StartsWith(Prefijo, StringComparison.Ordinal) OrElse _adicionales.Any(Function(x) x.ConexionRef = conexion AndAlso x.Tabla = tabla))
    End Function
    Public Function Adicionales(conexion As String) As List(Of String)
        Return _adicionales.Where(Function(x) x.ConexionRef = conexion).Select(Function(x) x.Tabla).ToList()
    End Function
    Private Shared Function Nombre(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z")
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Configuracion de tablas adicionales no valida.")
    End Function
End Class

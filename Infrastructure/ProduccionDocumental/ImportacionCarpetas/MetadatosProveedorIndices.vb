Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Class MetadatosProveedorIndices
    Private ReadOnly _politica As PoliticaFuentesProveedor
    Private ReadOnly _conexion As Func(Of String, String)
    Public Sub New(politica As PoliticaFuentesProveedor, conexion As Func(Of String, String))
        _politica = politica : _conexion = conexion
    End Sub
    Public Function Consultar(c As ContextoPerfilImportacion, tipo As String, origen As String, tabla As String, pagina As Integer) As List(Of OpcionPerfilImportacion)
        If c Is Nothing OrElse Not c.PuedeAdministrarProveedores Then Throw Denegado()
        If pagina < 1 OrElse pagina > 1000 Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Pagina no valida.")
        Dim fuentes = _politica.Fuentes(c)
        If tipo = "CONEXIONES" Then Return fuentes.Select(Function(f) f.ConexionRef).Distinct().OrderBy(Function(x) x, StringComparer.Ordinal).Skip((pagina - 1) * 25).Take(25).Select(Function(x) New OpcionPerfilImportacion With {.value = x, .text = x}).ToList()
        If Not fuentes.Any(Function(f) f.ConexionRef = origen) Then Throw Denegado()
        If tipo = "TABLAS" Then Return fuentes.Where(Function(f) f.ConexionRef = origen).OrderBy(Function(f) f.Tabla, StringComparer.Ordinal).Skip((pagina - 1) * 25).Take(25).Select(Function(f) New OpcionPerfilImportacion With {.value = f.Tabla, .text = f.Tabla}).ToList()
        If tipo <> "COLUMNAS" Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Catalogo no valido.")
        Dim fuente = fuentes.SingleOrDefault(Function(f) f.ConexionRef = origen AndAlso f.Tabla = tabla)
        If fuente Is Nothing Then Throw Denegado()
        Dim cadena = _conexion(origen)
        If String.IsNullOrWhiteSpace(cadena) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Origen no disponible.")
        Dim opciones As New List(Of OpcionPerfilImportacion)()
        Try
            Using cn As New MySqlConnection(cadena)
                cn.Open()
                Using cmd As New MySqlCommand("SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla ORDER BY ORDINAL_POSITION", cn)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@tabla", tabla)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim nombre = CStr(reader("COLUMN_NAME"))
                            If Not fuente.Columnas.Contains(nombre) Then Continue While
                            Dim fisico = CStr(reader("DATA_TYPE"))
                            Dim tipoValor = If({"varchar", "char"}.Contains(fisico), "TEXTO", If({"tinyint", "smallint", "mediumint", "int", "bigint"}.Contains(fisico), "ENTERO", If({"decimal", "numeric"}.Contains(fisico), "DECIMAL", If(fisico = "date", "FECHA", "NO_SOPORTADO"))))
                            Dim longitud = If(Convert.IsDBNull(reader("CHARACTER_MAXIMUM_LENGTH")), 64L, Convert.ToInt64(reader("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture))
                            If longitud > 4096 Then tipoValor = "NO_SOPORTADO"
                            opciones.Add(New OpcionPerfilImportacion With {.value = nombre, .text = nombre, .Tipo = tipoValor, .Longitud = CInt(Math.Min(longitud, 4096L))})
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudieron consultar los metadatos.")
        End Try
        Return opciones.Skip((pagina - 1) * 25).Take(25).ToList()
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "Fuente no autorizada.")
    End Function
End Class

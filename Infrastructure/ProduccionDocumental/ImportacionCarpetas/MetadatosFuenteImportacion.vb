Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

Public Class MetadatosFuenteImportacion
    Private ReadOnly _conexion As Func(Of String, String)
    Private ReadOnly _tablas As RestriccionTablasFuente
    Public Sub New(conexion As Func(Of String, String), Optional tablas As RestriccionTablasFuente = Nothing)
        _conexion = conexion
        _tablas = If(tablas, New RestriccionTablasFuente())
    End Sub
    Public Function Consultar(c As ContextoPerfilImportacion, tipo As String, origen As String, tabla As String, pagina As Integer) As List(Of OpcionPerfilImportacion)
        ReglasFuenteImportacion.Autorizar(c) : ReglasFuenteImportacion.Pagina(pagina)
        If tipo = "CONEXIONES" Then
            Return {"RA", "DA"}.Where(Function(x) Not String.IsNullOrWhiteSpace(_conexion(x))).Skip((pagina - 1) * 25).Take(25).Select(Function(x) New OpcionPerfilImportacion With {.value = x, .text = x}).ToList()
        End If
        If Not {"RA", "DA"}.Contains(origen) OrElse Not {"TABLAS", "COLUMNAS"}.Contains(tipo) Then Throw Invalida()
        If tipo = "COLUMNAS" AndAlso Not Nombre(tabla) Then Throw Invalida()
        If tipo = "COLUMNAS" AndAlso Not _tablas.Permite(origen, tabla) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Tabla fuera del alcance autorizado del servidor.")
        Dim cadena = _conexion(origen)
        If String.IsNullOrWhiteSpace(cadena) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Conexion no disponible.")
        Dim opciones As New List(Of OpcionPerfilImportacion)()
        Try
            Using cn As New MySqlConnection(cadena)
                cn.Open()
                Dim adicionales = _tablas.Adicionales(origen)
                Dim filtro = "LEFT(BINARY TABLE_NAME,6)=BINARY @prefijo"
                For i = 0 To adicionales.Count - 1
                    filtro &= " OR BINARY TABLE_NAME=BINARY @adicional" & i.ToString(Globalization.CultureInfo.InvariantCulture)
                Next
                Dim sql = If(tipo = "TABLAS", "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_TYPE IN ('BASE TABLE','VIEW') AND (" & filtro & ") AND TABLE_NAME REGEXP '^[A-Za-z_][A-Za-z0-9_]{0,63}$' ORDER BY TABLE_NAME LIMIT 25 OFFSET @offset", "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND COLUMN_NAME REGEXP '^[A-Za-z_][A-Za-z0-9_]{0,63}$' ORDER BY ORDINAL_POSITION LIMIT 25 OFFSET @offset")
                Using cmd As New MySqlCommand(sql, cn) With {.CommandTimeout = 5}
                    cmd.Parameters.AddWithValue("@offset", (pagina - 1) * 25)
                    If tipo = "TABLAS" Then
                        cmd.Parameters.AddWithValue("@prefijo", RestriccionTablasFuente.Prefijo)
                        For i = 0 To adicionales.Count - 1
                            cmd.Parameters.AddWithValue("@adicional" & i.ToString(Globalization.CultureInfo.InvariantCulture), adicionales(i))
                        Next
                    End If
                    If tipo = "COLUMNAS" Then cmd.Parameters.AddWithValue("@tabla", tabla)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim nombreColumna = CStr(r(0))
                            If Not Nombre(nombreColumna) Then Continue While
                            If tipo = "TABLAS" AndAlso Not _tablas.Permite(origen, nombreColumna) Then Continue While
                            Dim o As New OpcionPerfilImportacion With {.value = nombreColumna, .text = nombreColumna}
                            If tipo = "COLUMNAS" Then
                                Dim fisico = CStr(r(1)), longitud = If(r.IsDBNull(2), 64L, Convert.ToInt64(r(2)))
                                o.Tipo = If({"varchar", "char"}.Contains(fisico), "TEXTO", If({"tinyint", "smallint", "mediumint", "int", "bigint"}.Contains(fisico), "ENTERO", If({"decimal", "numeric"}.Contains(fisico), "DECIMAL", If(fisico = "date", "FECHA", "NO_SOPORTADO"))))
                                If longitud > 4096 Then o.Tipo = "NO_SOPORTADO"
                                o.Longitud = CInt(Math.Min(longitud, 4096L))
                            End If
                            opciones.Add(o)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudieron consultar metadatos.")
        End Try
        Return opciones
    End Function
    Public Sub Validar(c As ContextoPerfilImportacion, d As DocumentoFuenteImportacion)
        Dim copia = PerfilImportacionJson.Leer(Of DocumentoFuenteImportacion)(PerfilImportacionJson.Escribir(d))
        copia.FuenteId = Nothing : copia.Revision = 0
        ReglasFuenteImportacion.Validar("ALTA", New CambioFuenteImportacion With {.Documento = copia, .Motivo = "Validacion de estructura"})
        Dim columnas As New List(Of OpcionPerfilImportacion)()
        'Limite explicito: no truncar silenciosamente una tabla de mas de 4096 columnas.
        For pagina = 1 To 165
            Dim lote = Consultar(c, "COLUMNAS", d.ConexionRef, d.Tabla, pagina)
            columnas.AddRange(lote)
            If columnas.Count > 4096 Then Throw Invalida()
            If lote.Count < 25 Then Exit For
        Next
        If d.Columnas.Any(Function(x) Not columnas.Any(Function(o) o.value = x AndAlso o.Tipo <> "NO_SOPORTADO")) Then Throw Invalida()
        If d.ModoAislamiento = "COLUMNA" AndAlso Not columnas.Any(Function(o) o.value = d.ColumnaEmpresa AndAlso {"TEXTO", "ENTERO"}.Contains(o.Tipo)) Then Throw Invalida()
    End Sub
    Private Shared Function Nombre(v As String) As Boolean
        Return v IsNot Nothing AndAlso Regex.IsMatch(v, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z")
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("CONFIGURACION_INVALIDA", "Revise tabla, columnas y aislamiento de la fuente.")
    End Function
End Class

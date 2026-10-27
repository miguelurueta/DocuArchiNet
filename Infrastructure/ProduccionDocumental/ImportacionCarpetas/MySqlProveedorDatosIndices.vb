Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

'Adaptador comun: no contiene nombres de tablas de negocio ni resuelve sesion.
Public Class MySqlProveedorDatosIndices
    Implements IProveedorDatosIndices
    Private ReadOnly _conexion As Func(Of String, String)
    Private ReadOnly _leer As Func(Of String, String, Object(), DataTable)
    Private ReadOnly _autorizada As Func(Of ContextoPerfilImportacion, ProveedorIndicesVersion, Boolean)

    Public Sub New(conexion As Func(Of String, String), Optional leer As Func(Of String, String, Object(), DataTable) = Nothing, Optional autorizada As Func(Of ContextoPerfilImportacion, ProveedorIndicesVersion, Boolean) = Nothing)
        If conexion Is Nothing Then Throw New ArgumentNullException("conexion")
        _conexion = conexion
        _leer = If(leer, AddressOf LeerMySql)
        _autorizada = autorizada
    End Sub

    Public Function Consultar(contexto As ContextoPerfilImportacion, version As ProveedorIndicesVersion, clave As String) As ResultadoProveedorIndices Implements IProveedorDatosIndices.Consultar
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse
            version Is Nothing OrElse Not version.Activo OrElse version.AmbitoRef <> contexto.AmbitoRef Then Return Resultado("ACCESO_DENEGADO")
        If Not ConfiguracionValida(version) Then Return Resultado("CONFIGURACION_INVALIDA")
        If _autorizada Is Nothing OrElse Not _autorizada(contexto, version) Then Return Resultado("ACCESO_DENEGADO")
        If String.IsNullOrEmpty(clave) OrElse clave.Length > 4096 Then Return Resultado("CLAVE_INVALIDA")
        If clave.Any(Function(c) c = ChrW(0) OrElse Char.IsSurrogate(c)) Then Return Resultado("CLAVE_INVALIDA")
        If version.TipoClave = "FECHA" Then
            Dim fecha As DateTime
            If Not DateTime.TryParseExact(clave, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then Return Resultado("CLAVE_INVALIDA")
        End If
        Try
            Dim cadena = _conexion(version.ConexionRef)
            If String.IsNullOrWhiteSpace(cadena) Then Return Resultado("ORIGEN_NO_DISPONIBLE")
            Dim meta = _leer(cadena, "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,NUMERIC_SCALE,CHARACTER_SET_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla", {"@tabla", version.Tabla})
            Dim campos As New Dictionary(Of String, OpcionPerfilImportacion)(StringComparer.Ordinal)
            For Each row As DataRow In meta.Rows
                Dim nombre = CStr(row("COLUMN_NAME"))
                If campos.ContainsKey(nombre) Then Return Resultado("CONFIGURACION_INVALIDA")
                campos.Add(nombre, Campo(row))
            Next
            If Not Compatible(campos, version.ColumnaClave, version.TipoClave) Then Return Resultado("CONFIGURACION_INVALIDA")
            Dim filtrarEmpresa = Not String.IsNullOrEmpty(version.ColumnaEmpresa)
            If filtrarEmpresa AndAlso (Not campos.ContainsKey(version.ColumnaEmpresa) OrElse Not {"TEXTO", "ENTERO"}.Contains(campos(version.ColumnaEmpresa).Tipo)) Then Return Resultado("CONFIGURACION_INVALIDA")
            For Each propiedad In version.Propiedades
                If Not Compatible(campos, propiedad.Columna, propiedad.Tipo) Then Return Resultado("CONFIGURACION_INVALIDA")
            Next
            If Not PerfilImportacionCatalogAdapter.ValorCompatible(campos(version.ColumnaClave), clave) Then Return Resultado("CLAVE_INVALIDA")
            If filtrarEmpresa AndAlso Not PerfilImportacionCatalogAdapter.ValorCompatible(campos(version.ColumnaEmpresa), contexto.AmbitoRef) Then Return Resultado("CONFIGURACION_INVALIDA")
            Dim columnas = String.Join(",", version.Propiedades.Select(Function(p) Identificador(p.Columna) & " AS " & Identificador(p.Nombre)))
            Dim sql = "SELECT " & columnas & " FROM " & Identificador(version.Tabla) & " WHERE " & Identificador(version.ColumnaClave) & "=@clave" & If(filtrarEmpresa, " AND " & Identificador(version.ColumnaEmpresa) & "=@empresa", "") & " LIMIT 2"
            Dim parametros As Object() = If(filtrarEmpresa, New Object() {"@clave", clave, "@empresa", contexto.AmbitoRef}, New Object() {"@clave", clave})
            Dim datos = _leer(cadena, sql, parametros)
            If datos.Rows.Count = 0 Then Return Resultado("SIN_COINCIDENCIA")
            If datos.Rows.Count <> 1 Then Return Resultado("AMBIGUO")
            Dim salida = Resultado("OK")
            For Each propiedad In version.Propiedades
                Dim valor = datos.Rows(0)(propiedad.Nombre)
                Dim texto As String = Nothing
                If Not Convert.IsDBNull(valor) Then
                    texto = If(TypeOf valor Is DateTime, CType(valor, DateTime).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), Convert.ToString(valor, CultureInfo.InvariantCulture))
                    If texto.Length > 4096 Then Return Resultado("CONFIGURACION_INVALIDA")
                End If
                salida.Valores.Add(propiedad.Nombre, texto)
            Next
            Return salida
        Catch ex As MySqlException
            Return Resultado("ORIGEN_NO_DISPONIBLE")
        Catch ex As TimeoutException
            Return Resultado("ORIGEN_NO_DISPONIBLE")
        End Try
    End Function

    Public Shared Function ConfiguracionValida(v As ProveedorIndicesVersion) As Boolean
        If v Is Nothing OrElse v.Tipo <> "TABLA" OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(v.ProveedorId) OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(v.VersionId) OrElse
            Not NombreValido(v.ConexionRef) OrElse Not NombreValido(v.Tabla) OrElse
            Not NombreValido(v.ColumnaClave) OrElse (Not String.IsNullOrEmpty(v.ColumnaEmpresa) AndAlso Not NombreValido(v.ColumnaEmpresa)) OrElse
            Not TipoValido(v.TipoClave) OrElse v.Propiedades Is Nothing OrElse v.Propiedades.Count < 1 OrElse v.Propiedades.Count > 128 Then Return False
        Dim nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each p In v.Propiedades
            If p Is Nothing OrElse Not NombreValido(p.Nombre) OrElse Not NombreValido(p.Columna) OrElse Not TipoValido(p.Tipo) OrElse Not nombres.Add(p.Nombre) Then Return False
        Next
        Return True
    End Function

    Private Shared Function NombreValido(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z", RegexOptions.CultureInvariant)
    End Function
    Private Shared Function TipoValido(tipo As String) As Boolean
        Return {"TEXTO", "ENTERO", "DECIMAL", "FECHA"}.Contains(tipo)
    End Function
    Private Shared Function Identificador(valor As String) As String
        If Not NombreValido(valor) Then Throw New ArgumentException("Identificador no valido.")
        Return "`" & valor & "`"
    End Function
    Private Shared Function Compatible(campos As Dictionary(Of String, OpcionPerfilImportacion), columna As String, tipo As String) As Boolean
        Return campos.ContainsKey(columna) AndAlso campos(columna).Tipo = tipo
    End Function
    Private Shared Function Campo(row As DataRow) As OpcionPerfilImportacion
        Dim fisico = CStr(row("DATA_TYPE"))
        Dim tipo = If({"char", "varchar"}.Contains(fisico), "TEXTO", If({"tinyint", "smallint", "mediumint", "int", "bigint"}.Contains(fisico), "ENTERO", If({"decimal", "numeric"}.Contains(fisico), "DECIMAL", If(fisico = "date", "FECHA", "NO_SOPORTADO"))))
        Dim longitud = If(Convert.IsDBNull(row("CHARACTER_MAXIMUM_LENGTH")), 64L, Convert.ToInt64(row("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture))
        If longitud > 4096 Then tipo = "NO_SOPORTADO"
        Return New OpcionPerfilImportacion With {.Tipo = tipo, .TipoFisico = CStr(row("COLUMN_TYPE")), .Longitud = CInt(Math.Min(longitud, 4096L)), .Requerida = True,
            .Precision = If(Convert.IsDBNull(row("NUMERIC_PRECISION")), 0, Convert.ToInt32(row("NUMERIC_PRECISION"))),
            .Escala = If(Convert.IsDBNull(row("NUMERIC_SCALE")), 0, Convert.ToInt32(row("NUMERIC_SCALE"))),
            .Charset = Convert.ToString(row("CHARACTER_SET_NAME"), CultureInfo.InvariantCulture)}
    End Function
    Private Shared Function Resultado(codigo As String) As ResultadoProveedorIndices
        Return New ResultadoProveedorIndices With {.Codigo = codigo}
    End Function
    Private Shared Function LeerMySql(cadena As String, sql As String, parametros As Object()) As DataTable
        Using conexion As New MySqlConnection(cadena)
            conexion.Open()
            Using cmd As New MySqlCommand(sql, conexion)
                cmd.CommandTimeout = 5
                For i = 0 To parametros.Length - 1 Step 2
                    cmd.Parameters.AddWithValue(CStr(parametros(i)), parametros(i + 1))
                Next
                Using reader = cmd.ExecuteReader()
                    Dim tabla As New DataTable()
                    tabla.Load(reader)
                    Return tabla
                End Using
            End Using
        End Using
    End Function
End Class

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data.Common
Imports System.Globalization
Imports System.Web
Imports MySql.Data.MySqlClient

'Adapta la conexión DA del módulo; no administra otra cuenta ni otro almacén de secretos.
Public Class ImportacionExcelConexionAdapter
    Private ReadOnly _fabrica As Func(Of String, DbConnection)
    Private ReadOnly _leerLimite As Func(Of String, String)
    Public Sub New()
        Me.New(Function(cadena) New MySqlConnection(cadena), Function(clave) ConfigurationManager.AppSettings(clave))
    End Sub
    Public Sub New(fabrica As Func(Of String, DbConnection), Optional leerLimite As Func(Of String, String) = Nothing)
        If fabrica Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        _fabrica = fabrica
        _leerLimite = leerLimite
        If _leerLimite Is Nothing Then _leerLimite = Function(clave) ConfigurationManager.AppSettings(clave)
    End Sub
    Public Function Limite(clave As String, predeterminado As Integer) As Integer
        Dim setting = _leerLimite("ImportacionExcel." & clave)
        If setting Is Nothing Then Return predeterminado
        Dim valor As Integer
        If Not Integer.TryParse(setting, NumberStyles.None, CultureInfo.InvariantCulture, valor) OrElse valor < 1 OrElse (clave = "MaxArchivoMB" AndAlso valor > Integer.MaxValue \ 1000000) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Return valor
    End Function
    Private Shared Function CadenaDA(http As HttpContext, ctx As ContextoImportacionExcel, ByRef moduloId As UInteger) As String
        ServicioPlantillasExcel.Autorizar(ctx)
        If http Is Nothing OrElse http.Session Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("SESION_REQUERIDA")
        Dim empresaId As UInteger
        If Not UInteger.TryParse(Convert.ToString(http.Session("DA_ID_EMPRESA"), CultureInfo.InvariantCulture), NumberStyles.None, CultureInfo.InvariantCulture, empresaId) OrElse empresaId <> ctx.EmpresaId OrElse Not UInteger.TryParse(Convert.ToString(http.Session("DA_ID_MODULO"), CultureInfo.InvariantCulture), NumberStyles.None, CultureInfo.InvariantCulture, moduloId) OrElse moduloId = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        'Mismo recurso que utiliza la importación de carpetas para su DA.
        Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        If String.IsNullOrEmpty(cadena) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Return cadena
    End Function
    Private Function ConexionVigente(http As HttpContext, ctx As ContextoImportacionExcel, moduloEsperado As UInteger, cadenaEsperada As String) As DbConnection
        Dim moduloActual As UInteger
        Dim actual = CadenaDA(http, ctx, moduloActual)
        If moduloActual <> moduloEsperado OrElse Not String.Equals(actual, cadenaEsperada, StringComparison.Ordinal) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        Dim builder As New MySqlConnectionStringBuilder(actual)
        builder.AllowUserVariables = False
        builder.PersistSecurityInfo = False
        builder.ConnectionTimeout = 5
        Return _fabrica(builder.ConnectionString)
    End Function
    Public Function Referencias(http As HttpContext, ctx As ContextoImportacionExcel) As List(Of ReferenciaConexionExcel)
        Dim modulo As UInteger
        Dim cadena = CadenaDA(http, ctx, modulo)
        Dim result = LeerReferencias(http, ctx, modulo, cadena, Nothing)
        If result.Count = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Return result
    End Function
    Public Function Resolver(http As HttpContext, ctx As ContextoImportacionExcel, conexionId As UInteger) As IPlantillaExcelRepository
        If conexionId = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        Dim modulo As UInteger
        Dim cadena = CadenaDA(http, ctx, modulo)
        Dim referencias = LeerReferencias(http, ctx, modulo, cadena, conexionId)
        If referencias.Count <> 1 Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        'La fábrica comprueba de nuevo el origen antes de abrir cada conexión del repositorio.
        Return New MySqlPlantillaExcelRepository(Function() ConexionVigente(http, ctx, modulo, cadena), conexionId)
    End Function
    Private Function LeerReferencias(http As HttpContext, ctx As ContextoImportacionExcel, modulo As UInteger, cadena As String, conexionId As Nullable(Of UInteger)) As List(Of ReferenciaConexionExcel)
        Dim builder As New MySqlConnectionStringBuilder(cadena)
        Dim result As New List(Of ReferenciaConexionExcel)
        Dim ids As New HashSet(Of UInteger)
        Try
            Using cn = ConexionVigente(http, ctx, modulo, cadena)
                cn.Open()
                Using cmd = cn.CreateCommand()
                    'Nunca seleccionar USUARIO_ADMIN_BD o PASW_USUARIO_ADMIN_DB.
                    cmd.CommandText = "SELECT ID_XMLCONEXION,NOMBRE_CONEXION,IP_NOMBRE_SERVIDOR,NOMBRE_DB_DEFAULT,DBMS_BD FROM imp01_xmlconexion WHERE IP_NOMBRE_SERVIDOR=@servidor AND NOMBRE_DB_DEFAULT=@base AND DBMS_BD=@proveedor" & If(conexionId.HasValue, " AND ID_XMLCONEXION=@id LIMIT 2", " ORDER BY ID_XMLCONEXION LIMIT 101")
                    cmd.CommandTimeout = 5
                    ImportacionExcelContextAdapter.Parametro(cmd, "@servidor", builder.Server)
                    ImportacionExcelContextAdapter.Parametro(cmd, "@base", builder.Database)
                    ImportacionExcelContextAdapter.Parametro(cmd, "@proveedor", "mysql")
                    If conexionId.HasValue Then ImportacionExcelContextAdapter.Parametro(cmd, "@id", conexionId.Value)
                    Using reader = cmd.ExecuteReader()
                        Dim filas As Integer = 0
                        While reader.Read()
                            filas += 1
                            If filas > 100 OrElse (conexionId.HasValue AndAlso filas > 1) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
                            'La comparación ordinal evita equivalencias de collation del catálogo.
                            If Not DestinoCoincide(builder.Server, builder.Database, Convert.ToString(reader(2), CultureInfo.InvariantCulture), Convert.ToString(reader(3), CultureInfo.InvariantCulture)) OrElse Not String.Equals(Convert.ToString(reader(4), CultureInfo.InvariantCulture), "mysql", StringComparison.OrdinalIgnoreCase) Then Continue While
                            Dim id = Convert.ToUInt32(reader(0), CultureInfo.InvariantCulture)
                            Dim nombre = Convert.ToString(reader(1), CultureInfo.InvariantCulture)
                            If id = 0 OrElse Not ids.Add(id) OrElse String.IsNullOrWhiteSpace(nombre) OrElse nombre.Length > 120 OrElse (conexionId.HasValue AndAlso id <> conexionId.Value) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
                            result.Add(New ReferenciaConexionExcel With {.ConexionId = id, .Nombre = nombre})
                        End While
                    End Using
                End Using
            End Using
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
        Return result
    End Function
    Public Shared Function DestinoCoincide(servidor As String, base As String, servidorAutorizado As String, baseAutorizada As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(servidor) AndAlso Not String.IsNullOrWhiteSpace(base) AndAlso String.Equals(servidor, servidorAutorizado, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(base, baseAutorizada, StringComparison.Ordinal)
    End Function
End Class

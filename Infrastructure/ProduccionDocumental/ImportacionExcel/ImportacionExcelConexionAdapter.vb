Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports MySql.Data.MySqlClient

'DTO exclusivamente de configuración protegida; no es retorno del servicio web.
Friend Class VinculoDestinoExcel
    Public Property empresaId As UInteger
    Public Property conexionId As UInteger
    Public Property nombre As String
    Public Property connectionStringName As String
End Class

Public Class ImportacionExcelConexionAdapter
    Private ReadOnly _config As Configuration
    Private ReadOnly _fabrica As Func(Of String, DbConnection)
    Public Sub New()
        Me.New(System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~"), Function(cadena) New MySqlConnection(cadena))
    End Sub
    Public Sub New(configuracion As Configuration, fabrica As Func(Of String, DbConnection))
        If configuracion Is Nothing OrElse fabrica Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        _config = configuracion
        _fabrica = fabrica
    End Sub
    Public Function Limite(clave As String, predeterminado As Integer) As Integer
        Dim setting = DirectCast(_config.GetSection("appSettings"), AppSettingsSection).Settings("ImportacionExcel." & clave)
        If setting Is Nothing Then Return predeterminado
        Dim valor As Integer
        If Not Integer.TryParse(setting.Value, NumberStyles.None, CultureInfo.InvariantCulture, valor) OrElse valor < 1 OrElse (clave = "MaxArchivoMB" AndAlso valor > Integer.MaxValue \ 1000000) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Return valor
    End Function
    Private Function Vinculos(ctx As ContextoImportacionExcel) As List(Of VinculoDestinoExcel)
        ServicioPlantillasExcel.Autorizar(ctx)
        Dim app = DirectCast(_config.GetSection("appSettings"), AppSettingsSection)
        Dim cs = DirectCast(_config.GetSection("connectionStrings"), ConnectionStringsSection)
        If Not app.SectionInformation.IsProtected OrElse Not cs.SectionInformation.IsProtected OrElse app.Settings("ImportacionExcel.Destinos") Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Try
            Dim todos = JsonConvert.DeserializeObject(Of List(Of VinculoDestinoExcel))(app.Settings("ImportacionExcel.Destinos").Value, New JsonSerializerSettings With {.MissingMemberHandling = MissingMemberHandling.Error, .MaxDepth = 8})
            If todos Is Nothing OrElse todos.Count = 0 OrElse todos.Count > 100 Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
            Dim ids As New HashSet(Of String)(StringComparer.Ordinal)
            For Each v In todos
                If v Is Nothing OrElse v.empresaId = 0 OrElse v.conexionId = 0 OrElse String.IsNullOrWhiteSpace(v.nombre) OrElse v.nombre.Length > 120 OrElse String.IsNullOrWhiteSpace(v.connectionStringName) OrElse Not ids.Add(v.empresaId.ToString(CultureInfo.InvariantCulture) & ":" & v.conexionId.ToString(CultureInfo.InvariantCulture)) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
            Next
            Return todos.Where(Function(v) v.empresaId = ctx.EmpresaId).ToList()
        Catch ex As JsonException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Public Function Referencias(http As HttpContext, ctx As ContextoImportacionExcel) As List(Of ReferenciaConexionExcel)
        Dim result As New List(Of ReferenciaConexionExcel)
        For Each v In Vinculos(ctx)
            Resolver(http, ctx, v.conexionId)
            result.Add(New ReferenciaConexionExcel With {.ConexionId = v.conexionId, .Nombre = v.nombre})
        Next
        If result.Count = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Return result
    End Function
    Public Function Resolver(http As HttpContext, ctx As ContextoImportacionExcel, conexionId As UInteger) As IPlantillaExcelRepository
        Dim v = Vinculos(ctx).SingleOrDefault(Function(x) x.conexionId = conexionId)
        If v Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        Dim cs = DirectCast(_config.GetSection("connectionStrings"), ConnectionStringsSection).ConnectionStrings(v.connectionStringName)
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Try
            Dim builder As New MySqlConnectionStringBuilder(cs.ConnectionString)
            If http Is Nothing OrElse http.Session Is Nothing OrElse Not String.Equals(Convert.ToString(http.Session("DA_TYPE_DBMS_MODULO")), "mysql", StringComparison.OrdinalIgnoreCase) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
            If builder.Port <> 3306UI OrElse Not DestinoCoincide(builder.Server, builder.Database, Convert.ToString(http.Session("DA_IP_SERVER_MODULO")), Convert.ToString(http.Session("DA_DB_NAME_MODULO"))) OrElse String.IsNullOrWhiteSpace(builder.UserID) OrElse String.IsNullOrWhiteSpace(builder.Password) OrElse builder.UserID.Equals("root", StringComparison.OrdinalIgnoreCase) OrElse builder.UserID = Convert.ToString(http.Session("DA_USER_DBMS_MODULO")) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
            builder.AllowUserVariables = False
            builder.PersistSecurityInfo = False
            builder.ConnectionTimeout = 5
            Dim cadena = builder.ConnectionString
            Using cn = _fabrica(cadena)
                cn.Open()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText = "SELECT IP_NOMBRE_SERVIDOR,NOMBRE_DB_DEFAULT,DBMS_BD FROM imp01_xmlconexion WHERE ID_XMLCONEXION=@id LIMIT 2"
                    cmd.CommandTimeout = 5
                    ImportacionExcelContextAdapter.Parametro(cmd, "@id", conexionId)
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Not DestinoCoincide(builder.Server, builder.Database, Convert.ToString(reader(0)), Convert.ToString(reader(1))) OrElse Not String.Equals(Convert.ToString(reader(2)), "mysql", StringComparison.OrdinalIgnoreCase) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
                        If reader.Read() Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
                    End Using
                End Using
            End Using
            Return New MySqlPlantillaExcelRepository(Function() _fabrica(cadena), conexionId)
        Catch ex As ArgumentException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Public Shared Function DestinoCoincide(servidor As String, base As String, servidorAutorizado As String, baseAutorizada As String) As Boolean
        'Coincidencia exacta sin resolver DNS ni equivalencias elegidas por el cliente.
        Return Not String.IsNullOrWhiteSpace(servidor) AndAlso Not String.IsNullOrWhiteSpace(base) AndAlso String.Equals(servidor, servidorAutorizado, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(base, baseAutorizada, StringComparison.Ordinal)
    End Function
End Class

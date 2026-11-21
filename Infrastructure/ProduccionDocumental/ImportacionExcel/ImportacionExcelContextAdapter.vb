Option Strict On
Imports System
Imports System.Data.Common
Imports System.Globalization
Imports System.Web
Imports MySql.Data.MySqlClient

Public Class ImportacionExcelContextAdapter
    Private ReadOnly _abrir As Func(Of String, DbConnection)
    Public Sub New()
        Me.New(Function(cadena) New MySqlConnection(cadena))
    End Sub
    Public Sub New(fabrica As Func(Of String, DbConnection))
        If fabrica Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        _abrir = fabrica
    End Sub
    Public Function Resolver(http As HttpContext) As ContextoImportacionExcel
        If http Is Nothing OrElse http.Session Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("SESION_REQUERIDA")
        Dim usuario, empresa As UInteger
        Dim usuarioTexto = Convert.ToString(http.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture)
        Dim empresaTexto = Convert.ToString(http.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture)
        Dim login = Convert.ToString(http.Session("GA_LOGINUSUARIOGESTION"), CultureInfo.InvariantCulture)
        If Not UInteger.TryParse(usuarioTexto, NumberStyles.None, CultureInfo.InvariantCulture, usuario) OrElse usuario = 0 OrElse String.IsNullOrWhiteSpace(login) OrElse String.Equals(login, "CONSULTAPUBLICO", StringComparison.OrdinalIgnoreCase) Then Throw ServicioPlantillasExcel.ErrorControlado("SESION_REQUERIDA")
        If Not UInteger.TryParse(empresaTexto, NumberStyles.None, CultureInfo.InvariantCulture, empresa) OrElse empresa = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
        If String.IsNullOrEmpty(cadena) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        Try
            Using cn = _abrir(cadena)
                cn.Open()
                Using cmd = cn.CreateCommand()
                    cmd.CommandTimeout = 5
                    cmd.CommandText = "SELECT p.ADMINISTRAR_CARGAS_IMPORTACION FROM remit_dest_interno_perfil_produccion p INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=p.remit_dest_interno_idremit_dest_interno INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa WHERE u.id_Remit_Dest_Int=@usuario AND e.ID_EMPRESA=@empresa AND u.Estado_Usuario=1 LIMIT 2"
                    Parametro(cmd, "@usuario", usuario)
                    Parametro(cmd, "@empresa", empresa)
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Not PermisoValido(reader(0)) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
                        If reader.Read() Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
                    End Using
                End Using
            End Using
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
        Return New ContextoImportacionExcel With {.UsuarioId = usuario, .EmpresaId = empresa, .Autorizado = True}
    End Function
    Public Shared Function PermisoValido(valor As Object) As Boolean
        Return valor IsNot Nothing AndAlso Not Convert.IsDBNull(valor) AndAlso String.Equals(Convert.ToString(valor, CultureInfo.InvariantCulture), "1", StringComparison.Ordinal)
    End Function
    Friend Shared Sub Parametro(cmd As DbCommand, nombre As String, valor As Object)
        Dim p = cmd.CreateParameter()
        p.ParameterName = nombre
        p.Value = If(valor, DBNull.Value)
        cmd.Parameters.Add(p)
    End Sub
End Class

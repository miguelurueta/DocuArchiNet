Option Strict On
Imports System
Imports System.Globalization
Imports System.Web
Imports MySql.Data.MySqlClient

Public Class PerfilImportacionContextAdapter
    Public Function Resolver(http As HttpContext) As ContextoPerfilImportacion
        If http Is Nothing OrElse http.Session Is Nothing Then Throw SesionRequerida()
        Dim usuario = Convert.ToString(http.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture)
        Dim empresa = Convert.ToString(http.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture)
        Dim login = Convert.ToString(http.Session("GA_LOGINUSUARIOGESTION"), CultureInfo.InvariantCulture)
        If Not ValidadorPerfilImportacion.ReferenciaValida(usuario) OrElse String.IsNullOrWhiteSpace(login) OrElse String.Equals(login, "CONSULTAPUBLICO", StringComparison.OrdinalIgnoreCase) Then Throw SesionRequerida()
        If Not ValidadorPerfilImportacion.ReferenciaValida(empresa) Then Throw Denegado()
        Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
        If String.IsNullOrWhiteSpace(cadena) Then Throw Denegado()
        Try
            Using conexion As New MySqlConnection(cadena)
                conexion.Open()
                'La relación empresarial se comprueba en RA, no en empresa de módulo ni BD por defecto.
                Using cmd As New MySqlCommand("SELECT e.ID_EMPRESA FROM remit_dest_interno u INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa WHERE u.id_Remit_Dest_int=@usuario LIMIT 2", conexion)
                    cmd.Parameters.AddWithValue("@usuario", UInteger.Parse(usuario, CultureInfo.InvariantCulture))
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Convert.ToString(reader(0), CultureInfo.InvariantCulture) <> empresa Then Throw Denegado()
                        If reader.Read() Then Throw Denegado()
                    End Using
                End Using
                Using cmd As New MySqlCommand("SELECT ADMINISTRAR_PERFILES_IMPORTACION FROM remit_dest_interno_perfil_produccion WHERE remit_dest_interno_idremit_dest_interno=@usuario LIMIT 2", conexion)
                    cmd.Parameters.AddWithValue("@usuario", UInteger.Parse(usuario, CultureInfo.InvariantCulture))
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Not PermisoValido(reader(0)) Then Throw Denegado()
                        If reader.Read() Then Throw Denegado()
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            'Columna ausente, conexión fallida o lectura incompleta nunca otorgan acceso.
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar la autorización del perfil.")
        End Try
        Return New ContextoPerfilImportacion With {.UsuarioRef = usuario, .AmbitoRef = empresa, .PuedeAdministrar = True}
    End Function

    Public Shared Function PermisoValido(valor As Object) As Boolean
        If valor Is Nothing OrElse Convert.IsDBNull(valor) Then Return False
        Return String.Equals(Convert.ToString(valor, CultureInfo.InvariantCulture), "1", StringComparison.Ordinal)
    End Function

    Public Sub AutorizarProveedores(http As HttpContext, contexto As ContextoPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar Then Throw Denegado()
        contexto.PuedeAdministrarProveedores = False
        Try
            Using conexion As New MySqlConnection(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
                conexion.Open()
                Using cmd As New MySqlCommand("SELECT a.ADMINISTRAR_PROVEEDORES_IMPORTACION FROM remit_dest_interno_perfil_produccion a INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=a.remit_dest_interno_idremit_dest_interno WHERE u.id_Remit_Dest_Int=@usuario AND u.Empresa_Gestion_Documental_id_empresa=@empresa AND u.Estado_Usuario=1 LIMIT 2", conexion)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@usuario", contexto.UsuarioRef)
                    cmd.Parameters.AddWithValue("@empresa", contexto.AmbitoRef)
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Not PermisoValido(reader(0)) Then Throw Denegado()
                        If reader.Read() Then Throw Denegado()
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el permiso de proveedores.")
        End Try
        contexto.PuedeAdministrarProveedores = True
    End Sub
    Public Sub AutorizarFuentes(http As HttpContext, contexto As ContextoPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar Then Throw Denegado()
        contexto.PuedeAdministrarFuentes = False
        Try
            Using conexion As New MySqlConnection(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
                conexion.Open()
                Using cmd As New MySqlCommand("SELECT a.ADMINISTRAR_FUENTES_IMPORTACION FROM remit_dest_interno_perfil_produccion a INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=a.remit_dest_interno_idremit_dest_interno WHERE u.id_Remit_Dest_Int=@usuario AND u.Empresa_Gestion_Documental_id_empresa=@empresa AND u.Estado_Usuario=1 LIMIT 2", conexion)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@usuario", contexto.UsuarioRef)
                    cmd.Parameters.AddWithValue("@empresa", contexto.AmbitoRef)
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() OrElse Not PermisoValido(reader(0)) Then Throw Denegado()
                        If reader.Read() Then Throw Denegado()
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el permiso de fuentes.")
        End Try
        contexto.PuedeAdministrarFuentes = True
    End Sub
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "Contexto no autorizado.")
    End Function
    Private Shared Function SesionRequerida() As PerfilImportacionException
        Return New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesión de Gestión vigente.")
    End Function
End Class

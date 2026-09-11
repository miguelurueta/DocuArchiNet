Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Autoriza importación SII con permisos persistidos y un contexto ya resuelto por el gate de sesión.
Public NotInheritable Class MySqlSiiImportAuthorizationRepository
    Implements IAutorizacionImportacionRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _trusted As ContextoImportacionServicio

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor,
                   ByVal trusted As ContextoImportacionServicio)
        If connections Is Nothing OrElse executor Is Nothing OrElse trusted Is Nothing Then
            Throw New ArgumentNullException("dependency")
        End If
        _connections = connections
        _executor = executor
        _trusted = trusted
    End Sub

    Public Function UsuarioAutenticado(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.UsuarioAutenticado
        Return SameContext(contexto)
    End Function

    Public Function PermisoVigente(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.PermisoVigente
        If Not SameContext(contexto) Then Return False
        Const sql As String = "SELECT ADJUNTAR_IMAGENES_PREDETERMINADA FROM PERMISOS_USUARIO_WORKFLOW WHERE Usuario_Workflow_idU_suario=@idUsuario LIMIT 1"
        Return ReadBoolean(contexto, sql,
            Function(reader As IDataReader) As Boolean
                If Not reader.Read() Then Return False
                Return Enabled(reader("ADJUNTAR_IMAGENES_PREDETERMINADA"))
            End Function)
    End Function

    Public Function TareaOperable(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.TareaOperable
        If Not SameContext(contexto) Then Return False
        Const sql As String = "SELECT COUNT(*) FROM estados_tarea_workflow WHERE Inicio_Tareas_Workflow_id_Tarea=@idTarea AND ID_USUARIO=@idUsuario AND FECHA_SELECCION IS NOT NULL AND FECHA_FIN IS NULL AND ESTADO_TAREA=0"
        Return ReadBoolean(contexto, sql,
            Function(reader As IDataReader) As Boolean
                Return reader.Read() AndAlso Convert.ToInt32(reader(0)) = 1
            End Function)
    End Function

    Public Function RutaCoincide(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.RutaCoincide
        If Not SameContext(contexto) Then Return False
        Const sql As String = "SELECT COUNT(*) FROM estados_tarea_workflow WHERE Inicio_Tareas_Workflow_id_Tarea=@idTarea AND ID_USUARIO=@idUsuario AND Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=@idRuta AND FECHA_FIN IS NULL AND ESTADO_TAREA=0"
        Return ReadBoolean(contexto, sql,
            Function(reader As IDataReader) As Boolean
                Return reader.Read() AndAlso Convert.ToInt32(reader(0)) = 1
            End Function)
    End Function

    Public Function TramiteCoincide(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.TramiteCoincide
        Return SameContext(contexto) AndAlso contexto.IdTramite = _trusted.IdTramite
    End Function

    Public Function ProveedorHabilitado(ByVal contexto As ContextoImportacionServicio) As Boolean Implements IAutorizacionImportacionRepository.ProveedorHabilitado
        Return SameContext(contexto) AndAlso
            String.Equals(contexto.ProviderId, SiiImportProvider.CanonicalProviderId, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function ReadBoolean(ByVal contexto As ContextoImportacionServicio,
                                 ByVal sql As String,
                                 ByVal projector As Func(Of IDataReader, Boolean)) As Boolean
        Try
            Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
                Dim parameters As New List(Of IDataParameter) From {
                    New MySqlParameter("@idUsuario", contexto.IdUsuario),
                    New MySqlParameter("@idTarea", contexto.IdTarea),
                    New MySqlParameter("@idRuta", contexto.IdRuta)}
                Return _executor.ExecuteReader(connection, Nothing, sql, parameters, projector)
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function SameContext(ByVal contexto As ContextoImportacionServicio) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.PermiteImportar AndAlso
            contexto.IdUsuario = _trusted.IdUsuario AndAlso contexto.IdGrupo = _trusted.IdGrupo AndAlso
            contexto.IdTarea = _trusted.IdTarea AndAlso contexto.IdRuta = _trusted.IdRuta AndAlso
            contexto.IdTramite = _trusted.IdTramite AndAlso
            String.Equals(contexto.LoginUsuario, _trusted.LoginUsuario, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function Enabled(ByVal value As Object) As Boolean
        Dim text = Convert.ToString(value).Trim()
        Return text = "1" OrElse String.Equals(text, "true", StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(text, "yes", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ModuleContext(ByVal contexto As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB",
            .IdUsuario = contexto.IdUsuario, .IdGrupo = contexto.IdGrupo, .LoginUsuario = contexto.LoginUsuario}
    End Function
End Class

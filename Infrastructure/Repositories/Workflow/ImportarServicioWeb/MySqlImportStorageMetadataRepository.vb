Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportStorageMetadataRepository
    Implements IImportStorageMetadataRepository
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        _connections = connections : _executor = executor
    End Sub

    Public Function Resolver(ByVal contexto As ContextoImportacionServicio) As MetadatosAlmacenamientoImportacion Implements IImportStorageMetadataRepository.Resolver
        If contexto Is Nothing Then Return Nothing
        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Const routeSql As String = "SELECT Nombre_Ruta FROM rutas_workflow WHERE ID_RUTA=@idRuta LIMIT 1"
            Dim routeName = _executor.ExecuteReader(connection, Nothing, routeSql,
                New List(Of IDataParameter) From {P("@idRuta", contexto.IdRuta)},
                Function(reader) If(reader.Read(), Convert.ToString(reader("Nombre_Ruta")), String.Empty))
            If Not SafeIdentifier(routeName) Then Return Nothing
            Dim cabinetSql = "SELECT cg.NOMBRE_GABINETE FROM dat_adic_tar" & routeName &
                " dat INNER JOIN configuracion_gabinete cg ON cg.id_Gabinete=dat.ID_GABINETE " &
                "WHERE dat.INICIO_TAREAS_WORKFLOW_ID_TAREA=@idTarea LIMIT 1"
            Dim cabinetName = _executor.ExecuteReader(connection, Nothing, cabinetSql,
                New List(Of IDataParameter) From {P("@idTarea", contexto.IdTarea)},
                Function(reader) If(reader.Read(), Convert.ToString(reader("NOMBRE_GABINETE")), String.Empty))
            If String.IsNullOrWhiteSpace(cabinetName) Then Return Nothing

            Return New MetadatosAlmacenamientoImportacion With {.NombreRutaWorkflow=routeName,
                .NombreGabinete=cabinetName, .NombreClaseFormatoDocumento="DOCUMENTO ELECTRONICO"}
        End Using
    End Function

    Private Shared Function SafeIdentifier(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z0-9_]+$")
    End Function
    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class

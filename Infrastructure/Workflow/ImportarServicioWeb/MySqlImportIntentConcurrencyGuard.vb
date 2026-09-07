Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportIntentConcurrencyGuard
    Implements IImportIntentConcurrencyGuard
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor
    End Sub
    Public Function Adquirir(ByVal context As ContextoImportacionServicio, ByVal key As String) As ResultadoGuardIntencionImportacion Implements IImportIntentConcurrencyGuard.Adquirir
        If context Is Nothing OrElse String.IsNullOrWhiteSpace(key) Then Return New ResultadoGuardIntencionImportacion With {.Codigo = "INVALID_INTENT"}
        Dim connection As IDbConnection = Nothing
        Try
            connection = _connections.CreateOpenConnection(ModuleContext(context))
            Dim name = LockName(context, key)
            Dim value = _executor.ExecuteScalar(connection, Nothing, "SELECT GET_LOCK(@lockName, 0)", New List(Of IDataParameter) From {New MySqlParameter("@lockName", name)})
            If Convert.ToString(value, CultureInfo.InvariantCulture) <> "1" Then Return New ResultadoGuardIntencionImportacion With {.Codigo = "INTENT_IN_PROGRESS", .MensajeVisible = "La intención está siendo procesada."}
            Dim result As New ResultadoGuardIntencionImportacion With {.Adquirido = True, .Lease = New ImportIntentLease(connection, _executor, name)}
            connection = Nothing : Return result
        Catch
            Return New ResultadoGuardIntencionImportacion With {.Codigo = "INTENT_UNAVAILABLE", .MensajeVisible = "No fue posible reservar la intención."}
        Finally
            If connection IsNot Nothing Then connection.Dispose()
        End Try
    End Function
    Private Shared Function ModuleContext(ByVal value As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = value.IdUsuario, .IdGrupo = value.IdGrupo, .LoginUsuario = value.LoginUsuario}
    End Function
    Private Shared Function LockName(ByVal value As ContextoImportacionServicio, ByVal key As String) As String
        Using sha = SHA256.Create()
            Return "import-intent-" & BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value.IdUsuario & "|" & value.IdTarea & "|" & key.Trim()))).Replace("-", "").Substring(0, 40).ToLowerInvariant()
        End Using
    End Function
End Class

Friend NotInheritable Class ImportIntentLease
    Implements IDisposable
    Private _connection As IDbConnection
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _name As String
    Public Sub New(ByVal connection As IDbConnection, ByVal executor As IDataExecutor, ByVal name As String)
        _connection = connection : _executor = executor : _name = name
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dim connection = _connection : _connection = Nothing
        If connection Is Nothing Then Return
        Try
            _executor.ExecuteScalar(connection, Nothing, "SELECT RELEASE_LOCK(@lockName)", New List(Of IDataParameter) From {New MySqlParameter("@lockName", _name)})
        Finally
            connection.Dispose()
        End Try
    End Sub
End Class

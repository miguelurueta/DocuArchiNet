Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient

' Los nombres de columna proceden de configuración interna, pero aun así se validan
' como identificadores. Los valores siempre viajan como parámetros.
Public NotInheritable Class MySqlPhysicalExpedientIdentityLookup
    Implements IPhysicalExpedientIdentityLookup

    Private Shared ReadOnly SafeIdentifier As New Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections
        _executor = executor
    End Sub

    Public Function Adquirir(ByVal contexto As ContextoImportacionServicio,
                             ByVal configuracion As ConfiguracionExpedienteImportacion) As IDisposable Implements IPhysicalExpedientIdentityLookup.Adquirir
        Dim connection = _connections.CreateOpenConnection(ModuleContext(contexto))
        Try
            Dim lockName = BuildLockName(configuracion)
            Dim acquired = _executor.ExecuteScalar(connection, Nothing, "SELECT GET_LOCK(@lockName,5)",
                New List(Of IDataParameter) From {New MySqlParameter("@lockName", lockName)})
            If acquired Is Nothing OrElse Convert.IsDBNull(acquired) OrElse Convert.ToInt32(acquired) <> 1 Then
                connection.Dispose()
                Return Nothing
            End If
            Return New IdentityLock(connection, _executor, lockName)
        Catch
            connection.Dispose()
            Throw
        End Try
    End Function

    Public Function Buscar(ByVal contexto As ContextoImportacionServicio,
                           ByVal configuracion As ConfiguracionExpedienteImportacion) As IList(Of Long) Implements IPhysicalExpedientIdentityLookup.Buscar
        If contexto Is Nothing OrElse configuracion Is Nothing OrElse configuracion.CamposIdentidad Is Nothing OrElse configuracion.CamposIdentidad.Count = 0 Then
            Throw New InvalidOperationException("EXPEDIENT_IDENTITY_CONFIGURATION_INVALID")
        End If

        Dim sql As New StringBuilder("SELECT ID_EXPEDIENTE FROM expediente_archivo WHERE ")
        Dim parameters As New List(Of IDataParameter)()
        For index As Integer = 0 To configuracion.CamposIdentidad.Count - 1
            Dim field = configuracion.CamposIdentidad(index)
            Dim name = If(field.NombreCampo, String.Empty).Trim()
            If Not SafeIdentifier.IsMatch(name) Then Throw New InvalidOperationException("EXPEDIENT_IDENTITY_FIELD_INVALID")
            If String.IsNullOrWhiteSpace(field.Valor) Then Throw New InvalidOperationException("EXPEDIENT_IDENTITY_VALUE_UNRESOLVED")
            If index > 0 Then sql.Append(" AND ")
            sql.Append("`").Append(name).Append("`=@identity").Append(index)
            parameters.Add(New MySqlParameter("@identity" & index, field.Valor.Trim()))
        Next
        sql.Append(" ORDER BY ID_EXPEDIENTE LIMIT 2")

        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Return _executor.ExecuteReader(connection, Nothing, sql.ToString(), parameters, AddressOf MapIds)
        End Using
    End Function

    Public Function BuscarPrincipal(ByVal contexto As ContextoImportacionServicio,
                                    ByVal nombreGabinete As String,
                                    ByVal valorIdentidad As String) As IList(Of Long) Implements IPhysicalExpedientIdentityLookup.BuscarPrincipal
        If contexto Is Nothing OrElse String.IsNullOrWhiteSpace(nombreGabinete) OrElse String.IsNullOrWhiteSpace(valorIdentidad) Then Throw New InvalidOperationException("EXPEDIENT_IDENTITY_INPUT_INVALID")
        Const sql As String = "SELECT ID_EXPEDIENTE FROM expediente_archivo WHERE GABINETE_PRODUCION=@cabinet AND CODIGO_UNICO=@identity ORDER BY ID_EXPEDIENTE LIMIT 2"
        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Return _executor.ExecuteReader(connection, Nothing, sql,
                New List(Of IDataParameter) From {
                    New MySqlParameter("@cabinet", nombreGabinete.Trim()),
                    New MySqlParameter("@identity", valorIdentidad.Trim())
                }, AddressOf MapIds)
        End Using
    End Function

    Private Shared Function MapIds(ByVal reader As IDataReader) As IList(Of Long)
        Dim values As New List(Of Long)()
        While reader.Read()
            If Not Convert.IsDBNull(reader("ID_EXPEDIENTE")) Then values.Add(Convert.ToInt64(reader("ID_EXPEDIENTE")))
        End While
        Return values
    End Function

    Private Shared Function BuildLockName(ByVal configuration As ConfiguracionExpedienteImportacion) As String
        If configuration Is Nothing OrElse configuration.CamposIdentidad Is Nothing OrElse configuration.CamposIdentidad.Count = 0 Then Throw New InvalidOperationException("EXPEDIENT_IDENTITY_CONFIGURATION_INVALID")
        Dim raw As New StringBuilder(If(configuration.NombreGabinete, String.Empty).Trim().ToUpperInvariant())
        For Each field In configuration.CamposIdentidad
            raw.Append("|").Append(If(field.NombreCampo, String.Empty).Trim().ToUpperInvariant()).Append("=").Append(If(field.Valor, String.Empty).Trim())
        Next
        Using sha = SHA256.Create()
            Dim hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw.ToString()))).Replace("-", String.Empty)
            Return "DOC67_EXP_" & hash.Substring(0, 48)
        End Using
    End Function

    Private NotInheritable Class IdentityLock
        Implements IDisposable
        Private _connection As IDbConnection
        Private ReadOnly _executor As IDataExecutor
        Private ReadOnly _name As String

        Public Sub New(ByVal connection As IDbConnection, ByVal executor As IDataExecutor, ByVal name As String)
            _connection = connection : _executor = executor : _name = name
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If _connection Is Nothing Then Return
            Try
                _executor.ExecuteScalar(_connection, Nothing, "SELECT RELEASE_LOCK(@lockName)",
                    New List(Of IDataParameter) From {New MySqlParameter("@lockName", _name)})
            Finally
                _connection.Dispose() : _connection = Nothing
            End Try
        End Sub
    End Class

    Private Shared Function ModuleContext(ByVal value As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {
            .CodigoModulo = "IMPORTAR_SERVICIO_WEB",
            .IdUsuario = value.IdUsuario,
            .IdGrupo = value.IdGrupo,
            .LoginUsuario = value.LoginUsuario
        }
    End Function
End Class

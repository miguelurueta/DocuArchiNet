Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports MySql.Data.MySqlClient

Public MustInherit Class MySqlSecondFactorPrincipalRepositoryBase
    Implements ISecondFactorPrincipalRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _selectSql As String
    Private ReadOnly _idColumn As String
    Private ReadOnly _loginColumn As String
    Private ReadOnly _emailColumn As String

    Protected Sub New(ByVal connections As IModuleConnectionFactory,
                      ByVal executor As IDataExecutor,
                      ByVal selectSql As String,
                      ByVal idColumn As String,
                      ByVal loginColumn As String,
                      ByVal emailColumn As String)
        If connections Is Nothing Then Throw New ArgumentNullException(NameOf(connections))
        If executor Is Nothing Then Throw New ArgumentNullException(NameOf(executor))
        _connections = connections
        _executor = executor
        _selectSql = selectSql
        _idColumn = idColumn
        _loginColumn = loginColumn
        _emailColumn = emailColumn
    End Sub

    Public Function Resolve(ByVal context As ContextoPreautenticacionModulo) As SecondFactorPrincipal Implements ISecondFactorPrincipalRepository.Resolve
        If context Is Nothing OrElse Not context.EsValido() Then Throw New ArgumentException("El contexto de principal es inválido.", NameOf(context))
        Using connection As IDbConnection = _connections.CreateOpenConnection(context)
            Return _executor.ExecuteReader(connection,
                                           Nothing,
                                           _selectSql,
                                           Parameters(New MySqlParameter("@login", context.LoginUsuario.Trim())),
                                           AddressOf ProjectSingle)
        End Using
    End Function

    Private Function ProjectSingle(ByVal reader As IDataReader) As SecondFactorPrincipal
        If reader Is Nothing OrElse Not reader.Read() Then Return Nothing
        Dim id As Long
        If Not Long.TryParse(Convert.ToString(reader(_idColumn), CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, id) OrElse id <= 0 Then
            Throw New InvalidOperationException("SECOND_FACTOR_PRINCIPAL_INVALID")
        End If
        Dim login As String = DbString(reader, _loginColumn)
        Dim email As String = DbString(reader, _emailColumn)
        If reader.Read() Then Throw New InvalidOperationException("SECOND_FACTOR_PRINCIPAL_AMBIGUOUS")
        Return New SecondFactorPrincipal(id, login, email)
    End Function

    Private Shared Function DbString(ByVal record As IDataRecord, ByVal name As String) As String
        Dim value As Object = record(name)
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return Nothing
        Return Convert.ToString(value, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function Parameters(ParamArray values As IDataParameter()) As IList(Of IDataParameter)
        Return New List(Of IDataParameter)(values)
    End Function
End Class

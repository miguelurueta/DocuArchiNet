Imports System
Imports System.Configuration
Imports System.Data
Imports MySql.Data.MySqlClient

'Factory compartido: recibe contexto comun y una configuracion compuesta por cada modulo.
Public Class ModuleConnectionFactory
    Implements IModuleConnectionFactory

    Private ReadOnly _connectionStringName As String

    Public Sub New(ByVal connectionStringName As String)
        If String.IsNullOrWhiteSpace(connectionStringName) Then
            Throw New ArgumentException("Se requiere el nombre de la cadena de conexion.", NameOf(connectionStringName))
        End If

        _connectionStringName = connectionStringName.Trim()
    End Sub

    Public Function CreateOpenConnection(ByVal contexto As ContextoModulo) As IDbConnection Implements IModuleConnectionFactory.CreateOpenConnection
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Throw New InvalidOperationException("MODULE_CONTEXT_INVALID")
        End If

        Dim setting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(_connectionStringName)
        If setting Is Nothing OrElse String.IsNullOrWhiteSpace(setting.ConnectionString) Then
            Throw New InvalidOperationException("MODULE_CONNECTION_NOT_CONFIGURED")
        End If

        Dim connection As New MySqlConnection(setting.ConnectionString)
        connection.Open()
        Return connection
    End Function
End Class

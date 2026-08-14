Imports System
Imports System.Data
Imports MySql.Data.MySqlClient

'Factoría de conexión creada a partir de una configuración ya resuelta por Presentation.
'No conoce HttpContext ni Session; los repositorios reciben solo esta dependencia tipada.
Public Class ModuleSnapshotConnectionFactory
    Implements IModuleConnectionFactory

    Private ReadOnly _connectionString As String

    Protected Sub New(ByVal connectionString As String, ByVal moduleName As String)
        If String.IsNullOrWhiteSpace(connectionString) Then
            Throw New ArgumentException("Se requiere la conexión del módulo " & moduleName & ".", NameOf(connectionString))
        End If

        _connectionString = connectionString
    End Sub

    Public Function CreateOpenConnection(ByVal contexto As ContextoModulo) As IDbConnection Implements IModuleConnectionFactory.CreateOpenConnection
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Throw New InvalidOperationException("MODULE_CONTEXT_INVALID")
        End If

        Dim connection As New MySqlConnection(_connectionString)
        connection.Open()
        Return connection
    End Function
End Class

Public NotInheritable Class WorkflowModuleConnectionFactory
    Inherits ModuleSnapshotConnectionFactory

    Public Sub New(ByVal connectionString As String)
        MyBase.New(connectionString, "Workflow")
    End Sub
End Class

Public NotInheritable Class DocuarchiModuleConnectionFactory
    Inherits ModuleSnapshotConnectionFactory

    Public Sub New(ByVal connectionString As String)
        MyBase.New(connectionString, "Docuarchi")
    End Sub
End Class

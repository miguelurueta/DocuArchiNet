Imports System
Imports System.Web
Imports MySql.Data.MySqlClient

'Materializa una cadena de conexión desde la configuración de módulo ya cargada en sesión.
'No abre conexiones ni conoce reglas funcionales de Workflow, Radicación o Docuarchi.
Public NotInheritable Class ModuleSessionConnectionStringResolver
    Private Sub New()
    End Sub

    Public Shared Function Resolve(ByVal requestContext As HttpContext,
                                   Optional ByVal sessionPrefix As String = "") As String
        If requestContext Is Nothing OrElse requestContext.Session Is Nothing Then Return String.Empty

        Dim server As String = Convert.ToString(requestContext.Session.Item(sessionPrefix & "IP_SERVER_MODULO")).Trim()
        Dim database As String = Convert.ToString(requestContext.Session.Item(sessionPrefix & "DB_NAME_MODULO")).Trim()
        Dim user As String = Convert.ToString(requestContext.Session.Item(sessionPrefix & "USER_DBMS_MODULO")).Trim()
        Dim password As String = Convert.ToString(requestContext.Session.Item(sessionPrefix & "PASW_DBMS_MODULO"))
        Dim provider As String = Convert.ToString(requestContext.Session.Item(sessionPrefix & "TYPE_DBMS_MODULO")).Trim()
        If Not String.Equals(provider, "mysql", StringComparison.OrdinalIgnoreCase) OrElse
           String.IsNullOrWhiteSpace(server) OrElse String.IsNullOrWhiteSpace(database) OrElse
           String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(password) Then
            Return String.Empty
        End If

        Dim builder As New MySqlConnectionStringBuilder With {
            .Server = server,
            .Database = database,
            .UserID = user,
            .Password = password,
            .Pooling = Enabled(requestContext.Session.Item(sessionPrefix & "ACTIVA_POOL_DBMS"))
        }
        Dim maximumPoolSize As Integer = 0
        If Integer.TryParse(Convert.ToString(requestContext.Session.Item(sessionPrefix & "NUMERO_DBMS_CONEX")), maximumPoolSize) AndAlso
           maximumPoolSize > 0 Then
            builder.MaximumPoolSize = maximumPoolSize
        End If
        Return builder.ConnectionString
    End Function

    Private Shared Function Enabled(ByVal value As Object) As Boolean
        Dim text As String = Convert.ToString(value).Trim()
        Return text = "1" OrElse String.Equals(text, "true", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(text, "yes", StringComparison.OrdinalIgnoreCase)
    End Function
End Class

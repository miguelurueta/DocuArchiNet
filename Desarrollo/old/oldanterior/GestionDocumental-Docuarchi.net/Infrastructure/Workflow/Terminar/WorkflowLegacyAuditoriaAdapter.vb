Imports System
Imports System.Data
Imports System.Web
Imports MySql.Data.MySqlClient

'Registra la trazabilidad adicional usando el log existente de Workflow.
Public Class WorkflowLegacyAuditoriaAdapter
    Implements IAuditoriaTransicionRepository

    Public Function Registrar(ByVal auditoria As AuditoriaTransicion) As Boolean Implements IAuditoriaTransicionRepository.Registrar
        If auditoria Is Nothing OrElse auditoria.IdUsuarioWorkflow <= 0 OrElse auditoria.IdTarea <= 0 Then
            Return False
        End If

        Try
            Dim detalle As String = String.Format(
                Globalization.CultureInfo.InvariantCulture,
                "WorkflowModern; Ref={0}; Tarea={1}; Origen={2}; Destino={3}; Mecanismo={4}; Resultado={5}",
                auditoria.Referencia,
                auditoria.IdTarea,
                auditoria.IdActividadOrigen,
                auditoria.IdActividadDestino,
                auditoria.Mecanismo,
                auditoria.Resultado)
            Dim requestContext As HttpContext = HttpContext.Current
            If requestContext Is Nothing OrElse requestContext.Session Is Nothing Then Return False

            Dim builder As New MySqlConnectionStringBuilder With {
                .Server = Convert.ToString(requestContext.Session.Item("IP_SERVER_MODULO")).Trim(),
                .Database = Convert.ToString(requestContext.Session.Item("DB_NAME_MODULO")).Trim(),
                .UserID = Convert.ToString(requestContext.Session.Item("USER_DBMS_MODULO")).Trim(),
                .Password = Convert.ToString(requestContext.Session.Item("PASW_DBMS_MODULO")),
                .Pooling = False
            }
            If String.IsNullOrWhiteSpace(builder.Server) OrElse
               String.IsNullOrWhiteSpace(builder.Database) OrElse
               String.IsNullOrWhiteSpace(builder.UserID) Then Return False

            Using connection As New MySqlConnection(builder.ConnectionString)
                connection.Open()
                Using command As New MySqlCommand(
                    "INSERT INTO log_usuario (Usuario_Workflow_idU_suario, Fecha_Inicio_Seccion, Direccion_Ip_Nombre, Valor_Log) " &
                    "VALUES (@usuario, @fecha, @ip, @detalle)", connection)
                    command.Parameters.Add("@usuario", MySqlDbType.Int32).Value = auditoria.IdUsuarioWorkflow
                    command.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = auditoria.FechaUtc
                    command.Parameters.Add("@ip", MySqlDbType.VarChar, 255).Value = Convert.ToString(requestContext.Session.Item("ip_host_name"))
                    command.Parameters.Add("@detalle", MySqlDbType.Text).Value = detalle
                    Return command.ExecuteNonQuery() = 1
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
End Class

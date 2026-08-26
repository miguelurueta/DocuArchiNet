Imports System
Imports System.Data
Imports System.Web
Imports MySql.Data.MySqlClient

'Adaptador exclusivo de auditoría. Registra solo telemetría saneada posterior a la operación.
Public Class WorkflowLegacyDevolverUsuarioAnteriorAuditoriaAdapter
    Implements IDevolverUsuarioAnteriorAuditoriaRepository

    Public Function Registrar(ByVal auditoria As AuditoriaDevolverUsuarioAnterior) As Boolean Implements IDevolverUsuarioAnteriorAuditoriaRepository.Registrar
        If auditoria Is Nothing OrElse auditoria.IdTarea <= 0 OrElse auditoria.IdUsuarioWorkflow <= 0 Then Return False
        Try
            Dim contexto As HttpContext = HttpContext.Current
            If contexto Is Nothing OrElse contexto.Session Is Nothing Then Return False
            Dim builder As New MySqlConnectionStringBuilder With {
                .Server = Convert.ToString(contexto.Session.Item("IP_SERVER_MODULO")).Trim(),
                .Database = Convert.ToString(contexto.Session.Item("DB_NAME_MODULO")).Trim(),
                .UserID = Convert.ToString(contexto.Session.Item("USER_DBMS_MODULO")).Trim(),
                .Password = Convert.ToString(contexto.Session.Item("PASW_DBMS_MODULO")),
                .Pooling = False}
            If String.IsNullOrWhiteSpace(builder.Server) OrElse String.IsNullOrWhiteSpace(builder.Database) OrElse String.IsNullOrWhiteSpace(builder.UserID) Then Return False
            Dim detalle As String = String.Format(Globalization.CultureInfo.InvariantCulture,
                "WorkflowModern; Ref={0}; Usuario={1}; Tarea={2}; Ruta={3}; Flujo={4}; Origen={5}; Destino={6}; DuracionMs={7}; Resultado={8}; Codigo={9}; Mecanismo=ASMX_DEVOLVER_USUARIO_ANTERIOR",
                Referencia(auditoria.Referencia), auditoria.IdUsuarioWorkflow, auditoria.IdTarea, auditoria.IdRuta, auditoria.IdFlujoTrabajo,
                auditoria.IdActividadOrigen, auditoria.IdActividadDestino, Math.Max(0, auditoria.DuracionMilisegundos), Resultado(auditoria.Resultado), Codigo(auditoria.CodigoFuncional))
            Using conexion As New MySqlConnection(builder.ConnectionString)
                conexion.Open()
                Using comando As New MySqlCommand("INSERT INTO log_usuario (Usuario_Workflow_idU_suario, Fecha_Inicio_Seccion, Direccion_Ip_Nombre, Valor_Log) VALUES (@usuario, @fecha, @ip, @detalle)", conexion)
                    comando.Parameters.Add("@usuario", MySqlDbType.Int32).Value = auditoria.IdUsuarioWorkflow
                    comando.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = auditoria.FechaUtc
                    comando.Parameters.Add("@ip", MySqlDbType.VarChar, 255).Value = Convert.ToString(contexto.Session.Item("ip_host_name"))
                    comando.Parameters.Add("@detalle", MySqlDbType.Text).Value = detalle
                    Return comando.ExecuteNonQuery() = 1
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Shared Function Resultado(ByVal valor As String) As String
        If String.Equals(valor, "EXITO", StringComparison.OrdinalIgnoreCase) Then Return "EXITO"
        If String.Equals(valor, "BLOQUEADO", StringComparison.OrdinalIgnoreCase) Then Return "BLOQUEADO"
        Return "ERROR"
    End Function

    Private Shared Function Codigo(ByVal valor As String) As String
        Dim normalizado As String = If(valor, String.Empty).Trim().ToUpperInvariant()
        Return If(System.Text.RegularExpressions.Regex.IsMatch(normalizado, "^[A-Z0-9_]{1,80}$"), normalizado, "WORKFLOW_UNKNOWN")
    End Function

    Private Shared Function Referencia(ByVal valor As String) As String
        Dim normalizado As String = If(valor, String.Empty).Trim()
        Return If(System.Text.RegularExpressions.Regex.IsMatch(normalizado, "^[A-Za-z0-9-]{1,64}$"), normalizado, "WF-RUS-SIN-REFERENCIA")
    End Function
End Class

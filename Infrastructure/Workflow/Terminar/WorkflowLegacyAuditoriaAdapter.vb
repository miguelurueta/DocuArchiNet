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
                "WorkflowModern; Ref={0}; Canal={1}; Usuario={2}; Tarea={3}; Ruta={4}; Flujo={5}; Origen={6}; Destino={7}; Conector={8}; DuracionMs={9}; Resultado={10}; Codigo={11}; Mecanismo={12}",
                NormalizarReferencia(auditoria.Referencia),
                NormalizarCanal(auditoria.Canal),
                auditoria.IdUsuarioWorkflow,
                auditoria.IdTarea,
                auditoria.IdRutaWorkflow,
                auditoria.IdFlujoTrabajo,
                auditoria.IdActividadOrigen,
                auditoria.IdActividadDestino,
                auditoria.IdConector,
                Math.Max(0, auditoria.DuracionMilisegundos),
                NormalizarResultado(auditoria.Resultado),
                NormalizarCodigo(auditoria.CodigoFuncional),
                NormalizarMecanismo(auditoria.Mecanismo))
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

    Private Shared Function NormalizarCanal(ByVal canal As String) As String
        If String.Equals(canal, "MODERNO", StringComparison.OrdinalIgnoreCase) Then Return "MODERNO"
        If String.Equals(canal, "LEGACY", StringComparison.OrdinalIgnoreCase) Then Return "LEGACY"
        Return "DESCONOCIDO"
    End Function

    Private Shared Function NormalizarResultado(ByVal resultado As String) As String
        If String.Equals(resultado, "EXITO", StringComparison.OrdinalIgnoreCase) Then Return "EXITO"
        If String.Equals(resultado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase) Then Return "BLOQUEADO"
        If String.Equals(resultado, "ERROR", StringComparison.OrdinalIgnoreCase) Then Return "ERROR"
        Return "ERROR"
    End Function

    Private Shared Function NormalizarMecanismo(ByVal mecanismo As String) As String
        Dim valor As String = If(mecanismo, String.Empty).Trim().ToUpperInvariant()
        If valor = "ASMX_MODERNO" OrElse valor = "ASMX_ENVIO_GRUPO" OrElse valor = "ASMX_ENVIO_USUARIO" Then
            Return valor
        End If
        Return "ASMX_DESCONOCIDO"
    End Function

    Private Shared Function NormalizarCodigo(ByVal codigo As String) As String
        Dim valor As String = If(codigo, String.Empty).Trim().ToUpperInvariant()
        If System.Text.RegularExpressions.Regex.IsMatch(valor, "^[A-Z0-9_]{1,80}$") Then Return valor
        Return "WORKFLOW_UNKNOWN"
    End Function

    Private Shared Function NormalizarReferencia(ByVal referencia As String) As String
        Dim valor As String = If(referencia, String.Empty).Trim()
        If System.Text.RegularExpressions.Regex.IsMatch(valor, "^[A-Za-z0-9-]{1,64}$") Then Return valor
        Return "WF-MOD-SIN-REFERENCIA"
    End Function
End Class

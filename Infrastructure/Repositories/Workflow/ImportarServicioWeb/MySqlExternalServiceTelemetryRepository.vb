Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlExternalServiceTelemetryRepository
    Implements IExternalServiceAttemptRecorder
    Implements IExternalServiceAvailabilityRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _context As ContextoModulo

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor, ByVal context As ContextoModulo)
        If connections Is Nothing OrElse executor Is Nothing OrElse context Is Nothing OrElse Not context.EsValido() Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor : _context = context
    End Sub

    Public Sub Registrar(ByVal intento As IntentoServicioExterno) Implements IExternalServiceAttemptRecorder.Registrar
        If intento Is Nothing OrElse String.IsNullOrWhiteSpace(intento.ProviderId) OrElse String.IsNullOrWhiteSpace(intento.Operacion) OrElse String.IsNullOrWhiteSpace(intento.CorrelationId) Then Throw New ArgumentException("EXTERNAL_ATTEMPT_INVALID")
        Const sql As String = "INSERT INTO ra_ser_intento_serviciointegracion " &
            "(Id_ser_servicioIntegracion,NombreServicioSnapshot,Operacion,Exitoso,CodigoError,CategoriaError,CodigoDependencia,MensajeDiagnostico,EstadoHttp,Reintentable,TaskId,Radicado,CodigoBarras,ReferenciaProveedor,IntentId,ClientItemId,OperationId,CorrelationId,FechaInicioUtc,FechaFinUtc,DuracionMs) " &
            "SELECT Id_ser_servicioIntegracion,NombreServicio,@operation,@success,@errorCode,@category,@dependencyCode,@diagnostic,@httpStatus,@retryable,@taskId,@radicado,@codigoBarras,@referenciaProveedor,@intentId,@clientItemId,@operationId,@correlationId,@started,@finished,@duration " &
            "FROM ra_ser_serviciointegracion WHERE NombreServicio=@providerId AND EstadoServicio=1"
        Using connection = _connections.CreateOpenConnection(_context)
            Dim affected = _executor.ExecuteNonQuery(connection, Nothing, sql, New List(Of IDataParameter) From {
                P("@operation", intento.Operacion), P("@success", intento.Exitoso), P("@errorCode", NullIfEmpty(intento.CodigoError)), P("@category", NullIfEmpty(intento.CategoriaError)),
                P("@dependencyCode", NullIfEmpty(intento.CodigoDependencia)), P("@diagnostic", NullIfEmpty(intento.MensajeDiagnostico)), P("@httpStatus", If(intento.EstadoHttp.HasValue, CType(intento.EstadoHttp.Value, Object), DBNull.Value)),
                P("@retryable", intento.Reintentable), P("@taskId", If(intento.TaskId.HasValue, CType(intento.TaskId.Value, Object), DBNull.Value)),
                P("@radicado", NullIfEmpty(intento.Radicado)), P("@codigoBarras", NullIfEmpty(intento.CodigoBarras)), P("@referenciaProveedor", NullIfEmpty(intento.ReferenciaProveedor)),
                P("@intentId", NullIfEmpty(intento.IntentId)), P("@clientItemId", NullIfEmpty(intento.ClientItemId)), P("@operationId", NullIfEmpty(intento.OperationId)),
                P("@correlationId", intento.CorrelationId), P("@started", intento.FechaInicioUtc), P("@finished", intento.FechaFinUtc), P("@duration", Math.Max(0, intento.DuracionMs)), P("@providerId", intento.ProviderId)})
            If affected <> 1 Then Throw New InvalidOperationException("INTEGRATION_PROVIDER_NOT_REGISTERED_OR_DISABLED")
        End Using
    End Sub

    Public Function Consultar(ByVal providerId As String, ByVal operation As String, ByVal desdeUtc As DateTime, ByVal hastaUtc As DateTime) As DisponibilidadServicioExterno Implements IExternalServiceAvailabilityRepository.Consultar
        If String.IsNullOrWhiteSpace(providerId) OrElse String.IsNullOrWhiteSpace(operation) OrElse hastaUtc <= desdeUtc Then Throw New ArgumentException("EXTERNAL_AVAILABILITY_QUERY_INVALID")
        Const sql As String = "SELECT COUNT(*) total,SUM(CASE WHEN a.Exitoso=1 THEN 1 ELSE 0 END) exitosos,AVG(a.DuracionMs) latencia " &
            "FROM ra_ser_intento_serviciointegracion a INNER JOIN ra_ser_serviciointegracion s ON s.Id_ser_servicioIntegracion=a.Id_ser_servicioIntegracion " &
            "WHERE s.NombreServicio=@providerId AND a.Operacion=@operation AND a.FechaInicioUtc>=@fromUtc AND a.FechaInicioUtc<@toUtc"
        Using connection = _connections.CreateOpenConnection(_context)
            Return _executor.ExecuteReader(connection, Nothing, sql, New List(Of IDataParameter) From {P("@providerId", providerId), P("@operation", operation), P("@fromUtc", desdeUtc), P("@toUtc", hastaUtc)}, Function(reader) MapAvailability(reader, providerId, operation, desdeUtc, hastaUtc))
        End Using
    End Function

    Private Shared Function MapAvailability(ByVal reader As IDataReader, ByVal provider As String, ByVal operation As String, ByVal fromUtc As DateTime, ByVal toUtc As DateTime) As DisponibilidadServicioExterno
        Dim result As New DisponibilidadServicioExterno With {.ProviderId=provider,.Operacion=operation,.DesdeUtc=fromUtc,.HastaUtc=toUtc}
        If reader.Read() Then
            result.Total = If(reader.IsDBNull(0), 0, Convert.ToInt64(reader(0))) : result.Exitosos = If(reader.IsDBNull(1), 0, Convert.ToInt64(reader(1)))
            result.Fallidos = result.Total - result.Exitosos : result.DisponibilidadPorcentaje = If(result.Total = 0, 0D, Decimal.Round(CDec(result.Exitosos) * 100D / CDec(result.Total), 4))
            result.LatenciaPromedioMs = If(reader.IsDBNull(2), 0D, Decimal.Round(Convert.ToDecimal(reader(2)), 2))
        End If
        Return result
    End Function

    Private Shared Function NullIfEmpty(ByVal value As String) As Object
        Return If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value.Trim())
    End Function
    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
End Class

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportIntentRepository
    Implements IImportIntentRepository
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _transactions As ITransactionFactory
    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor, ByVal transactions As ITransactionFactory)
        If connections Is Nothing OrElse executor Is Nothing OrElse transactions Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor : _transactions = transactions
    End Sub
    Public Function Obtener(ByVal context As ContextoImportacionServicio, ByVal id As String) As IntencionImportacionServicio Implements IImportIntentRepository.Obtener
        Return ReadOne(context, "i.intent_id=@value", id)
    End Function
    Public Function ObtenerPorIdempotencia(ByVal context As ContextoImportacionServicio, ByVal key As String) As IntencionImportacionServicio Implements IImportIntentRepository.ObtenerPorIdempotencia
        Return ReadOne(context, "i.user_id=@userId AND i.task_id=@taskId AND i.idempotency_key=@value", key)
    End Function
    Public Function CrearOReutilizar(ByVal context As ContextoImportacionServicio, ByVal intent As IntencionImportacionServicio) As ResultadoPersistenciaIntencionImportacion Implements IImportIntentRepository.CrearOReutilizar
        Using connection = _connections.CreateOpenConnection(ModuleContext(context))
            Using transaction = _transactions.BeginTransaction(connection)
                Try
                    Dim sql = "INSERT INTO workflow_import_intent (intent_id,idempotency_key,payload_hash,operation_id,correlation_id,user_id,group_id,user_login,task_id,route_id,procedure_id,provider_id,status,version_token,created_utc,updated_utc) VALUES (@intentId,@key,@hash,@operation,@correlation,@userId,@groupId,@login,@taskId,@routeId,@procedureId,@providerId,@status,@version,@created,@updated)"
                    _executor.ExecuteNonQuery(connection, transaction, sql, Params(intent))
                    For Each requirement In intent.Requisitos
                        _executor.ExecuteNonQuery(connection, transaction, "INSERT INTO workflow_import_intent_requirement (intent_id,requirement_code,is_satisfied,visible_message) VALUES (@intentId,@code,@satisfied,@message)", New List(Of IDataParameter) From {P("@intentId", intent.Id), P("@code", requirement.Codigo), P("@satisfied", requirement.Satisfecho), P("@message", requirement.MensajeVisible)})
                    Next
                    For Each item In intent.Resultados
                        _executor.ExecuteNonQuery(connection, transaction, "INSERT INTO workflow_import_intent_item (intent_id,client_item_id,provider_id,external_key,target_task_id,document_type_id,file_name,content_type,status) VALUES (@intentId,@clientId,@providerId,@externalKey,@taskId,@documentTypeId,@fileName,@contentType,@status)", New List(Of IDataParameter) From {P("@intentId", intent.Id), P("@clientId", item.ClientItemId), P("@providerId", item.IdentidadExterna.ProviderId), P("@externalKey", item.IdentidadExterna.ExternalKey), P("@taskId", item.IdTareaDestino), P("@documentTypeId", item.IdTipoDocumental), P("@fileName", item.NombreArchivo), P("@contentType", item.TipoContenido), P("@status", item.Fase.ToString())})
                    Next
                    transaction.Commit()
                    Return New ResultadoPersistenciaIntencionImportacion With {.Intencion = intent}
                Catch
                    transaction.Rollback()
                    Dim existing = ObtenerPorIdempotencia(context, intent.IdempotencyKey)
                    If existing IsNot Nothing AndAlso existing.HuellaContexto = intent.HuellaContexto Then Return New ResultadoPersistenciaIntencionImportacion With {.Intencion = existing, .Reutilizada = True}
                    Return New ResultadoPersistenciaIntencionImportacion With {.Codigo = "IDEMPOTENCY_CONFLICT", .MensajeVisible = "La clave idempotente corresponde a otra solicitud."}
                End Try
            End Using
        End Using
    End Function
    Private Function ReadOne(ByVal context As ContextoImportacionServicio, ByVal whereClause As String, ByVal value As String) As IntencionImportacionServicio
        Using connection = _connections.CreateOpenConnection(ModuleContext(context))
            Dim intent = _executor.ExecuteReader(connection, Nothing, "SELECT i.* FROM workflow_import_intent i WHERE " & whereClause & " LIMIT 1", New List(Of IDataParameter) From {P("@userId", context.IdUsuario), P("@taskId", context.IdTarea), P("@value", value)}, AddressOf MapHeader)
            If intent Is Nothing Then Return Nothing
            intent.Requisitos = _executor.ExecuteReader(connection, Nothing, "SELECT requirement_code,is_satisfied,visible_message FROM workflow_import_intent_requirement WHERE intent_id=@intentId ORDER BY requirement_code", New List(Of IDataParameter) From {P("@intentId", intent.Id)}, AddressOf MapRequirements)
            intent.Resultados = _executor.ExecuteReader(connection, Nothing, "SELECT client_item_id,provider_id,external_key,target_task_id,document_type_id,file_name,content_type,status FROM workflow_import_intent_item WHERE intent_id=@intentId ORDER BY client_item_id", New List(Of IDataParameter) From {P("@intentId", intent.Id)}, AddressOf MapItems)
            Return intent
        End Using
    End Function
    Private Shared Function MapHeader(ByVal reader As IDataReader) As IntencionImportacionServicio
        If Not reader.Read() Then Return Nothing
        Return New IntencionImportacionServicio With {.Id=Convert.ToString(reader("intent_id")),.IdempotencyKey=Convert.ToString(reader("idempotency_key")),.HuellaContexto=Convert.ToString(reader("payload_hash")),.VersionToken=Convert.ToString(reader("version_token")),.Fase=CType([Enum].Parse(GetType(FaseImportacionServicio),Convert.ToString(reader("status"))),FaseImportacionServicio),.FechaCreacionUtc=Convert.ToDateTime(reader("created_utc")),.FechaActualizacionUtc=Convert.ToDateTime(reader("updated_utc")),.ContextoOriginal=New ContextoIntencionImportacion With {.OperationId=Convert.ToString(reader("operation_id")),.CorrelationId=Convert.ToString(reader("correlation_id")),.IdUsuario=Convert.ToInt32(reader("user_id")),.IdGrupo=Convert.ToInt32(reader("group_id")),.LoginUsuario=Convert.ToString(reader("user_login")),.IdTarea=Convert.ToInt64(reader("task_id")),.IdRuta=Convert.ToInt32(reader("route_id")),.IdTramite=Convert.ToInt32(reader("procedure_id")),.ProviderId=Convert.ToString(reader("provider_id"))}}
    End Function
    Private Shared Function MapRequirements(ByVal reader As IDataReader) As IList(Of RequisitoPlanImportacion)
        Dim values As New List(Of RequisitoPlanImportacion)()
        While reader.Read()
            values.Add(New RequisitoPlanImportacion With {.Codigo=Convert.ToString(reader("requirement_code")),.Satisfecho=Convert.ToBoolean(reader("is_satisfied")),.MensajeVisible=Convert.ToString(reader("visible_message"))})
        End While
        Return values
    End Function
    Private Shared Function MapItems(ByVal reader As IDataReader) As IList(Of ResultadoElementoImportacion)
        Dim values As New List(Of ResultadoElementoImportacion)()
        While reader.Read()
            Dim documentType As Nullable(Of Integer) = If(reader.IsDBNull(reader.GetOrdinal("document_type_id")), Nothing, New Nullable(Of Integer)(Convert.ToInt32(reader("document_type_id"))))
            values.Add(New ResultadoElementoImportacion With {.ClientItemId=Convert.ToString(reader("client_item_id")),.IdentidadExterna=New IdentidadExternaImportacion With {.ProviderId=Convert.ToString(reader("provider_id")),.ExternalKey=Convert.ToString(reader("external_key"))},.IdTareaDestino=Convert.ToInt64(reader("target_task_id")),.IdTipoDocumental=documentType,.NombreArchivo=Convert.ToString(reader("file_name")),.TipoContenido=Convert.ToString(reader("content_type")),.Fase=CType([Enum].Parse(GetType(FaseImportacionServicio),Convert.ToString(reader("status"))),FaseImportacionServicio)})
        End While
        Return values
    End Function
    Private Shared Function Params(ByVal i As IntencionImportacionServicio) As IList(Of IDataParameter)
        Dim c=i.ContextoOriginal : Return New List(Of IDataParameter) From {P("@intentId",i.Id),P("@key",i.IdempotencyKey),P("@hash",i.HuellaContexto),P("@operation",c.OperationId),P("@correlation",c.CorrelationId),P("@userId",c.IdUsuario),P("@groupId",c.IdGrupo),P("@login",c.LoginUsuario),P("@taskId",c.IdTarea),P("@routeId",c.IdRuta),P("@procedureId",c.IdTramite),P("@providerId",c.ProviderId),P("@status",i.Fase.ToString()),P("@version",i.VersionToken),P("@created",i.FechaCreacionUtc),P("@updated",i.FechaActualizacionUtc)}
    End Function
    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class

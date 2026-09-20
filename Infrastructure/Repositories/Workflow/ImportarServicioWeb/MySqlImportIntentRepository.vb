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
                    Dim sql = "INSERT INTO workflow_import_intent (intent_id,idempotency_key,payload_hash,operation_id,correlation_id,user_id,group_id,user_login,task_id,route_id,procedure_id,provider_id,radicado,status,version_token,created_utc,updated_utc) VALUES (@intentId,@key,@hash,@operation,@correlation,@userId,@groupId,@login,@taskId,@routeId,@procedureId,@providerId,@radicado,@status,@version,@created,@updated)"
                    _executor.ExecuteNonQuery(connection, transaction, sql, Params(intent))
                    For Each requirement In intent.Requisitos
                        _executor.ExecuteNonQuery(connection, transaction, "INSERT INTO workflow_import_intent_requirement (intent_id,requirement_code,is_satisfied,visible_message) VALUES (@intentId,@code,@satisfied,@message)", New List(Of IDataParameter) From {P("@intentId", intent.Id), P("@code", requirement.Codigo), P("@satisfied", requirement.Satisfecho), P("@message", requirement.MensajeVisible)})
                    Next
                    For Each inscription In intent.Inscripciones
                        Const inscriptionSql As String = "INSERT INTO workflow_import_inscription (intent_id,inscription_key,inscription_ordinal,book_code,registry_number,matricula,normalized_matricula,proponente,subject_identification,subject_name,owner_matricula,owner_identification,owner_name,cabinet_name,expedient_id,expedient_role,expedient_status,cache_status,created_utc,updated_utc) VALUES (@intentId,@key,@ordinal,@book,@registry,@matricula,@normalized,@proponente,@subjectId,@subjectName,@ownerMatricula,@ownerId,@ownerName,@cabinet,@expedientId,@role,@expedientStatus,@cacheStatus,@created,@updated)"
                        _executor.ExecuteNonQuery(connection, transaction, inscriptionSql, InscriptionParams(intent, inscription))
                    Next
                    For Each item In intent.Resultados
                        _executor.ExecuteNonQuery(connection, transaction, "INSERT INTO workflow_import_intent_item (intent_id,client_item_id,inscription_key,provider_id,external_key,target_task_id,document_type_id,document_type_name,file_name,content_type,status,expedient_id,storage_status,relation_status,index_status,cache_status) VALUES (@intentId,@clientId,@inscriptionKey,@providerId,@externalKey,@taskId,@documentTypeId,@documentTypeName,@fileName,@contentType,@status,@expedientId,@storageStatus,@relationStatus,@indexStatus,@cacheStatus)", New List(Of IDataParameter) From {P("@intentId", intent.Id), P("@clientId", item.ClientItemId), P("@inscriptionKey", item.ClaveInscripcion), P("@providerId", item.IdentidadExterna.ProviderId), P("@externalKey", item.IdentidadExterna.ExternalKey), P("@taskId", item.IdTareaDestino), P("@documentTypeId", item.IdTipoDocumental), P("@documentTypeName", item.NombreTipoDocumental), P("@fileName", item.NombreArchivo), P("@contentType", item.TipoContenido), P("@status", item.Fase.ToString()), P("@expedientId", item.IdExpediente), P("@storageStatus", item.EstadoAlmacenamiento.ToString()), P("@relationStatus", item.EstadoRelacion.ToString()), P("@indexStatus", item.EstadoIndice.ToString()), P("@cacheStatus", item.EstadoCache.ToString())})
                    Next
                    transaction.Commit()
                    Return New ResultadoPersistenciaIntencionImportacion With {.Intencion = intent}
                Catch
                    transaction.Rollback()
                    Try
                        Dim existing = ObtenerPorIdempotencia(context, intent.IdempotencyKey)
                        If existing IsNot Nothing AndAlso existing.HuellaContexto = intent.HuellaContexto Then Return New ResultadoPersistenciaIntencionImportacion With {.Intencion = existing, .Reutilizada = True}
                        If existing IsNot Nothing Then Return New ResultadoPersistenciaIntencionImportacion With {.Codigo = "IDEMPOTENCY_CONFLICT", .MensajeVisible = "La clave idempotente corresponde a otra solicitud."}
                    Catch
                        Return New ResultadoPersistenciaIntencionImportacion With {.Codigo = "INTENT_PERSISTENCE_UNAVAILABLE", .MensajeVisible = "No fue posible verificar la persistencia de la intención."}
                    End Try
                    Return New ResultadoPersistenciaIntencionImportacion With {.Codigo = "INTENT_PERSISTENCE_UNAVAILABLE", .MensajeVisible = "No fue posible persistir la intención."}
                End Try
            End Using
        End Using
    End Function
    Public Function PersistirPlanExpedientes(ByVal context As ContextoImportacionServicio, ByVal intent As IntencionImportacionServicio, ByVal plan As PlanExpedienteImportacion) As Boolean Implements IImportIntentRepository.PersistirPlanExpedientes
        If context Is Nothing OrElse intent Is Nothing OrElse plan Is Nothing OrElse plan.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then Return False
        Using connection = _connections.CreateOpenConnection(ModuleContext(context))
            Using transaction = _transactions.BeginTransaction(connection)
                Try
                    For Each inscription In plan.Inscripciones
                        Const inscriptionSql As String = "UPDATE workflow_import_inscription SET normalized_matricula=@normalized,expedient_id=@expedientId,expedient_role=@role,expedient_status=@expedientStatus,cache_status=@cacheStatus,updated_utc=@updated WHERE intent_id=@intentId AND inscription_key=@key"
                        Dim inscriptionParameters As New List(Of IDataParameter) From {P("@normalized", inscription.MatriculaNormalizada), P("@expedientId", inscription.IdExpediente), P("@role", inscription.RolExpediente.ToString()), P("@expedientStatus", inscription.EstadoExpediente.ToString()), P("@cacheStatus", inscription.EstadoCache.ToString()), P("@updated", DateTime.UtcNow), P("@intentId", intent.Id), P("@key", inscription.ClaveInscripcion)}
                        If _executor.ExecuteNonQuery(connection, transaction, inscriptionSql, inscriptionParameters) <> 1 Then transaction.Rollback() : Return False
                    Next
                    For Each item In intent.Resultados
                        Dim inscription As InscripcionImportacion = Nothing
                        For Each candidate In plan.Inscripciones
                            If String.Equals(candidate.ClaveInscripcion, item.ClaveInscripcion, StringComparison.Ordinal) Then inscription = candidate : Exit For
                        Next
                        If inscription Is Nothing OrElse
                           (plan.Modo = ModoExpedienteImportacion.GestionarExpediente AndAlso Not inscription.IdExpediente.HasValue) Then
                            transaction.Rollback() : Return False
                        End If
                        item.IdExpediente = inscription.IdExpediente
                        item.EstadoCache = inscription.EstadoCache
                        If plan.Modo = ModoExpedienteImportacion.SinExpediente Then
                            item.EstadoRelacion = EstadoEfectoExpedienteImportacion.NoAplica
                            item.EstadoCache = EstadoEfectoExpedienteImportacion.NoAplica
                        End If
                        Const itemSql As String = "UPDATE workflow_import_intent_item SET expedient_id=@expedientId,storage_status=@storageStatus,relation_status=@relationStatus,index_status=@indexStatus,cache_status=@cacheStatus WHERE intent_id=@intentId AND client_item_id=@clientId AND inscription_key=@key"
                        Dim itemParameters As New List(Of IDataParameter) From {P("@expedientId", item.IdExpediente), P("@storageStatus", item.EstadoAlmacenamiento.ToString()), P("@relationStatus", item.EstadoRelacion.ToString()), P("@indexStatus", item.EstadoIndice.ToString()), P("@cacheStatus", item.EstadoCache.ToString()), P("@intentId", intent.Id), P("@clientId", item.ClientItemId), P("@key", item.ClaveInscripcion)}
                        If _executor.ExecuteNonQuery(connection, transaction, itemSql, itemParameters) <> 1 Then transaction.Rollback() : Return False
                    Next
                    transaction.Commit() : Return True
                Catch
                    transaction.Rollback() : Return False
                End Try
            End Using
        End Using
    End Function
    Public Function ActualizarTransicion(ByVal context As ContextoImportacionServicio, ByVal cambio As TransicionImportacion, ByVal item As ResultadoElementoImportacion) As Boolean Implements IImportIntentRepository.ActualizarTransicion
        If context Is Nothing OrElse cambio Is Nothing OrElse item Is Nothing Then Return False
        Using connection = _connections.CreateOpenConnection(ModuleContext(context))
            Using transaction = _transactions.BeginTransaction(connection)
                Try
                    If item.IdDocumento.HasValue Then item.EstadoAlmacenamiento = EstadoEfectoExpedienteImportacion.Confirmado
                    Dim itemSql = "UPDATE workflow_import_intent_item SET status=@status,document_id=@documentId,expedient_id=@expedientId,storage_status=@storageStatus,relation_status=@relationStatus,index_status=@indexStatus,cache_status=@cacheStatus,persistence_known=@known,retryable=@retryable,error_code=@errorCode,visible_message=@message,correlation_id=@correlation WHERE intent_id=@intentId AND client_item_id=@clientId AND status=@previousStatus"
                    If _executor.ExecuteNonQuery(connection, transaction, itemSql, New List(Of IDataParameter) From {P("@status", cambio.FaseNueva.ToString()), P("@documentId", item.IdDocumento), P("@expedientId", item.IdExpediente), P("@storageStatus", item.EstadoAlmacenamiento.ToString()), P("@relationStatus", item.EstadoRelacion.ToString()), P("@indexStatus", item.EstadoIndice.ToString()), P("@cacheStatus", item.EstadoCache.ToString()), P("@known", item.PersistenciaConocida), P("@retryable", item.Reintentable), P("@errorCode", item.CodigoError), P("@message", item.MensajeVisible), P("@correlation", item.CorrelationId), P("@intentId", cambio.IntentId), P("@clientId", cambio.ClientItemId), P("@previousStatus", cambio.FaseAnterior.ToString())}) <> 1 Then
                        transaction.Rollback() : Return False
                    End If
                    Dim intentSql = "UPDATE workflow_import_intent SET status=@status,version_token=@newVersion,updated_utc=@updated WHERE intent_id=@intentId AND user_id=@userId AND task_id=@taskId AND version_token=@oldVersion"
                    If _executor.ExecuteNonQuery(connection, transaction, intentSql, New List(Of IDataParameter) From {P("@status", cambio.FaseNueva.ToString()), P("@newVersion", cambio.VersionNueva), P("@updated", cambio.FechaUtc), P("@intentId", cambio.IntentId), P("@userId", context.IdUsuario), P("@taskId", context.IdTarea), P("@oldVersion", cambio.VersionAnterior)}) <> 1 Then
                        transaction.Rollback() : Return False
                    End If
                    Dim auditSql = "INSERT INTO workflow_import_intent_transition (intent_id,client_item_id,previous_status,new_status,previous_version,new_version,occurred_utc,correlation_id,result_code) VALUES (@intentId,@clientId,@previousStatus,@status,@oldVersion,@newVersion,@updated,@correlation,@resultCode)"
                    _executor.ExecuteNonQuery(connection, transaction, auditSql, New List(Of IDataParameter) From {P("@intentId", cambio.IntentId), P("@clientId", cambio.ClientItemId), P("@previousStatus", cambio.FaseAnterior.ToString()), P("@status", cambio.FaseNueva.ToString()), P("@oldVersion", cambio.VersionAnterior), P("@newVersion", cambio.VersionNueva), P("@updated", cambio.FechaUtc), P("@correlation", cambio.CorrelationId), P("@resultCode", cambio.Codigo)})
                    transaction.Commit() : Return True
                Catch
                    transaction.Rollback() : Return False
                End Try
            End Using
        End Using
    End Function
    Private Function ReadOne(ByVal context As ContextoImportacionServicio, ByVal whereClause As String, ByVal value As String) As IntencionImportacionServicio
        Using connection = _connections.CreateOpenConnection(ModuleContext(context))
            Dim intent = _executor.ExecuteReader(connection, Nothing, "SELECT i.* FROM workflow_import_intent i WHERE " & whereClause & " LIMIT 1", New List(Of IDataParameter) From {P("@userId", context.IdUsuario), P("@taskId", context.IdTarea), P("@value", value)}, AddressOf MapHeader)
            If intent Is Nothing Then Return Nothing
            intent.Requisitos = _executor.ExecuteReader(connection, Nothing, "SELECT requirement_code,is_satisfied,visible_message FROM workflow_import_intent_requirement WHERE intent_id=@intentId ORDER BY requirement_code", New List(Of IDataParameter) From {P("@intentId", intent.Id)}, AddressOf MapRequirements)
            intent.Inscripciones = _executor.ExecuteReader(connection, Nothing, "SELECT inscription_key,inscription_ordinal,book_code,registry_number,matricula,normalized_matricula,proponente,subject_identification,subject_name,owner_matricula,owner_identification,owner_name,cabinet_name,expedient_id,expedient_role,expedient_status,cache_status FROM workflow_import_inscription WHERE intent_id=@intentId ORDER BY inscription_ordinal", New List(Of IDataParameter) From {P("@intentId", intent.Id)}, AddressOf MapInscriptions)
            intent.Resultados = _executor.ExecuteReader(connection, Nothing, "SELECT client_item_id,inscription_key,provider_id,external_key,target_task_id,document_type_id,document_type_name,file_name,content_type,status,document_id,expedient_id,storage_status,relation_status,index_status,cache_status,persistence_known,retryable,error_code,visible_message,correlation_id FROM workflow_import_intent_item WHERE intent_id=@intentId ORDER BY client_item_id", New List(Of IDataParameter) From {P("@intentId", intent.Id)}, AddressOf MapItems)
            Return intent
        End Using
    End Function
    Private Shared Function MapHeader(ByVal reader As IDataReader) As IntencionImportacionServicio
        If Not reader.Read() Then Return Nothing
        Return New IntencionImportacionServicio With {.Id=Convert.ToString(reader("intent_id")),.IdempotencyKey=Convert.ToString(reader("idempotency_key")),.HuellaContexto=Convert.ToString(reader("payload_hash")),.VersionToken=Convert.ToString(reader("version_token")),.Fase=CType([Enum].Parse(GetType(FaseImportacionServicio),Convert.ToString(reader("status"))),FaseImportacionServicio),.FechaCreacionUtc=Convert.ToDateTime(reader("created_utc")),.FechaActualizacionUtc=Convert.ToDateTime(reader("updated_utc")),.ContextoOriginal=New ContextoIntencionImportacion With {.OperationId=Convert.ToString(reader("operation_id")),.CorrelationId=Convert.ToString(reader("correlation_id")),.IdUsuario=Convert.ToInt32(reader("user_id")),.IdGrupo=Convert.ToInt32(reader("group_id")),.LoginUsuario=Convert.ToString(reader("user_login")),.IdTarea=Convert.ToInt64(reader("task_id")),.IdRuta=Convert.ToInt32(reader("route_id")),.IdTramite=Convert.ToInt32(reader("procedure_id")),.ProviderId=Convert.ToString(reader("provider_id")),.Radicado=Convert.ToString(reader("radicado"))}}
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
            Dim documentId As Nullable(Of Long) = If(reader.IsDBNull(reader.GetOrdinal("document_id")), Nothing, New Nullable(Of Long)(Convert.ToInt64(reader("document_id"))))
            Dim expedientId As Nullable(Of Long) = If(reader.IsDBNull(reader.GetOrdinal("expedient_id")), Nothing, New Nullable(Of Long)(Convert.ToInt64(reader("expedient_id"))))
            values.Add(New ResultadoElementoImportacion With {.ClientItemId=Convert.ToString(reader("client_item_id")),.ClaveInscripcion=Convert.ToString(reader("inscription_key")),.IdentidadExterna=New IdentidadExternaImportacion With {.ProviderId=Convert.ToString(reader("provider_id")),.ExternalKey=Convert.ToString(reader("external_key"))},.IdTareaDestino=Convert.ToInt64(reader("target_task_id")),.IdTipoDocumental=documentType,.NombreTipoDocumental=Convert.ToString(reader("document_type_name")),.NombreArchivo=Convert.ToString(reader("file_name")),.TipoContenido=Convert.ToString(reader("content_type")),.Fase=CType([Enum].Parse(GetType(FaseImportacionServicio),Convert.ToString(reader("status"))),FaseImportacionServicio),.IdDocumento=documentId,.IdExpediente=expedientId,.EstadoAlmacenamiento=ParseEffect(reader("storage_status")),.EstadoRelacion=ParseEffect(reader("relation_status")),.EstadoIndice=ParseEffect(reader("index_status")),.EstadoCache=ParseEffect(reader("cache_status")),.PersistenciaConocida=Convert.ToBoolean(reader("persistence_known")),.Reintentable=Convert.ToBoolean(reader("retryable")),.CodigoError=Convert.ToString(reader("error_code")),.MensajeVisible=Convert.ToString(reader("visible_message")),.CorrelationId=Convert.ToString(reader("correlation_id"))})
        End While
        Return values
    End Function
    Private Shared Function ParseEffect(ByVal value As Object) As EstadoEfectoExpedienteImportacion
        Dim parsed As EstadoEfectoExpedienteImportacion
        If [Enum].TryParse(Convert.ToString(value), True, parsed) Then Return parsed
        Return EstadoEfectoExpedienteImportacion.Pendiente
    End Function
    Private Shared Function MapInscriptions(ByVal reader As IDataReader) As IList(Of InscripcionImportacion)
        Dim values As New List(Of InscripcionImportacion)()
        While reader.Read()
            Dim expedientId As Nullable(Of Long) = If(reader.IsDBNull(reader.GetOrdinal("expedient_id")), Nothing, New Nullable(Of Long)(Convert.ToInt64(reader("expedient_id"))))
            values.Add(New InscripcionImportacion With {.ClaveInscripcion=Convert.ToString(reader("inscription_key")),.Orden=Convert.ToInt32(reader("inscription_ordinal")),.Libro=Convert.ToString(reader("book_code")),.Registro=Convert.ToString(reader("registry_number")),.Matricula=Convert.ToString(reader("matricula")),.MatriculaNormalizada=Convert.ToString(reader("normalized_matricula")),.Proponente=Convert.ToString(reader("proponente")),.IdentificacionSujeto=Convert.ToString(reader("subject_identification")),.RazonSocial=Convert.ToString(reader("subject_name")),.MatriculaPropietario=Convert.ToString(reader("owner_matricula")),.IdentificacionPropietario=Convert.ToString(reader("owner_identification")),.NombrePropietario=Convert.ToString(reader("owner_name")),.NombreGabinete=Convert.ToString(reader("cabinet_name")),.IdExpediente=expedientId,.RolExpediente=CType([Enum].Parse(GetType(RolExpedienteImportacion),Convert.ToString(reader("expedient_role"))),RolExpedienteImportacion),.EstadoExpediente=CType([Enum].Parse(GetType(EstadoEfectoExpedienteImportacion),Convert.ToString(reader("expedient_status"))),EstadoEfectoExpedienteImportacion),.EstadoCache=CType([Enum].Parse(GetType(EstadoEfectoExpedienteImportacion),Convert.ToString(reader("cache_status"))),EstadoEfectoExpedienteImportacion)})
        End While
        Return values
    End Function
    Private Shared Function InscriptionParams(ByVal intent As IntencionImportacionServicio, ByVal i As InscripcionImportacion) As IList(Of IDataParameter)
        Return New List(Of IDataParameter) From {P("@intentId",intent.Id),P("@key",i.ClaveInscripcion),P("@ordinal",i.Orden),P("@book",i.Libro),P("@registry",i.Registro),P("@matricula",i.Matricula),P("@normalized",i.MatriculaNormalizada),P("@proponente",i.Proponente),P("@subjectId",i.IdentificacionSujeto),P("@subjectName",i.RazonSocial),P("@ownerMatricula",i.MatriculaPropietario),P("@ownerId",i.IdentificacionPropietario),P("@ownerName",i.NombrePropietario),P("@cabinet",i.NombreGabinete),P("@expedientId",i.IdExpediente),P("@role",i.RolExpediente.ToString()),P("@expedientStatus",i.EstadoExpediente.ToString()),P("@cacheStatus",i.EstadoCache.ToString()),P("@created",intent.FechaCreacionUtc),P("@updated",intent.FechaActualizacionUtc)}
    End Function
    Private Shared Function Params(ByVal i As IntencionImportacionServicio) As IList(Of IDataParameter)
        Dim c=i.ContextoOriginal : Return New List(Of IDataParameter) From {P("@intentId",i.Id),P("@key",i.IdempotencyKey),P("@hash",i.HuellaContexto),P("@operation",c.OperationId),P("@correlation",c.CorrelationId),P("@userId",c.IdUsuario),P("@groupId",c.IdGrupo),P("@login",c.LoginUsuario),P("@taskId",c.IdTarea),P("@routeId",c.IdRuta),P("@procedureId",c.IdTramite),P("@providerId",c.ProviderId),P("@radicado",c.Radicado),P("@status",i.Fase.ToString()),P("@version",i.VersionToken),P("@created",i.FechaCreacionUtc),P("@updated",i.FechaActualizacionUtc)}
    End Function
    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class

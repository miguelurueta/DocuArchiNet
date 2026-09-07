Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Todas las lecturas de reconciliación son parametrizadas y read-only.
Public NotInheritable Class MySqlImportReconciliationRepository
    Implements IImportReconciliationRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio, ByVal intentId As String) As SnapshotReconciliacionImportacion Implements IImportReconciliationRepository.Obtener
        Return ReadSnapshot(contexto, intentId, Nothing, Nothing)
    End Function

    Public Function ObtenerItem(ByVal contexto As ContextoImportacionServicio, ByVal intentId As String, ByVal providerId As String, ByVal externalKey As String) As SnapshotReconciliacionImportacion Implements IImportReconciliationRepository.ObtenerItem
        If String.IsNullOrWhiteSpace(providerId) OrElse String.IsNullOrWhiteSpace(externalKey) Then Return Nothing
        Return ReadSnapshot(contexto, intentId, providerId.Trim(), externalKey.Trim())
    End Function

    Private Function ReadSnapshot(ByVal contexto As ContextoImportacionServicio, ByVal intentId As String, ByVal providerId As String, ByVal externalKey As String) As SnapshotReconciliacionImportacion
        If contexto Is Nothing OrElse String.IsNullOrWhiteSpace(intentId) Then Return Nothing
        Dim filter = If(externalKey Is Nothing, String.Empty, " AND item.provider_id=@providerId AND item.external_key=@externalKey")
        Dim sql = "SELECT intent.intent_id,intent.version_token,intent.status AS intent_status,intent.user_id,intent.task_id,intent.provider_id AS intent_provider," &
                  "item.client_item_id,item.provider_id,item.external_key,item.target_task_id,item.document_id," &
                  "COALESCE((SELECT rpd.SEGUNDO_NOMBRE_DOCUMENTO FROM registro_producion_documental rpd WHERE rpd.ID_DOCUMENTO_DOCUARCHI_ALMACEN=item.document_id LIMIT 1),item.file_name) AS file_name,item.content_type,item.status AS item_status," &
                  "item.persistence_known,item.retryable,item.error_code,item.visible_message,item.correlation_id," &
                  "(SELECT COUNT(*) FROM registro_producion_documental document_record WHERE document_record.ID_DOCUMENTO_DOCUARCHI_ALMACEN=item.document_id) AS document_count," &
                  "(SELECT COUNT(*) FROM workflow_import_intent_item same_item WHERE same_item.intent_id=item.intent_id AND same_item.document_id=item.document_id AND same_item.target_task_id=intent.task_id) AS relation_count," &
                  "(SELECT COUNT(*) FROM workflow_import_intent_item other_item WHERE other_item.document_id=item.document_id AND other_item.target_task_id<>intent.task_id) AS other_task_count " &
                  "FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id " &
                  "WHERE intent.intent_id=@intentId AND intent.user_id=@userId AND intent.task_id=@taskId" & filter & " ORDER BY item.client_item_id"
        Dim parameters As New List(Of IDataParameter) From {P("@intentId", intentId.Trim()), P("@userId", contexto.IdUsuario), P("@taskId", contexto.IdTarea)}
        If externalKey IsNot Nothing Then parameters.Add(P("@providerId", providerId)) : parameters.Add(P("@externalKey", externalKey))
        Using connection = _connections.CreateOpenConnection(New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=contexto.IdUsuario,.IdGrupo=contexto.IdGrupo,.LoginUsuario=contexto.LoginUsuario})
            Return _executor.ExecuteReader(connection, Nothing, sql, parameters, AddressOf MapSnapshot)
        End Using
    End Function

    Private Shared Function MapSnapshot(ByVal reader As IDataReader) As SnapshotReconciliacionImportacion
        Dim result As SnapshotReconciliacionImportacion = Nothing
        While reader.Read()
            If result Is Nothing Then
                result = New SnapshotReconciliacionImportacion With {.IntentId=Convert.ToString(reader("intent_id")),.VersionToken=Convert.ToString(reader("version_token")),.Fase=ParsePhase(reader("intent_status")),.IdUsuario=Convert.ToInt32(reader("user_id")),.IdTareaOriginal=Convert.ToInt64(reader("task_id")),.ProviderId=Convert.ToString(reader("intent_provider"))}
            End If
            result.Items.Add(New SnapshotItemReconciliacionImportacion With {.ClientItemId=Convert.ToString(reader("client_item_id")),.ProviderId=Convert.ToString(reader("provider_id")),.ExternalKey=Convert.ToString(reader("external_key")),.IdTareaDestino=Convert.ToInt64(reader("target_task_id")),.IdDocumento=NullableLong(reader,"document_id"),.NombreDocumento=Convert.ToString(reader("file_name")),.TipoContenido=Convert.ToString(reader("content_type")),.Fase=ParsePhase(reader("item_status")),.PersistenciaConocida=Convert.ToBoolean(reader("persistence_known")),.Reintentable=Convert.ToBoolean(reader("retryable")),.CodigoError=Convert.ToString(reader("error_code")),.MensajeVisible=Convert.ToString(reader("visible_message")),.CorrelationId=Convert.ToString(reader("correlation_id")),.CantidadDocumentos=Convert.ToInt32(reader("document_count")),.CantidadRelaciones=Convert.ToInt32(reader("relation_count")),.CantidadRelacionesOtraTarea=Convert.ToInt32(reader("other_task_count"))})
        End While
        Return result
    End Function

    Private Shared Function ParsePhase(ByVal value As Object) As FaseImportacionServicio
        Return CType([Enum].Parse(GetType(FaseImportacionServicio), Convert.ToString(value)), FaseImportacionServicio)
    End Function
    Private Shared Function NullableLong(ByVal reader As IDataReader, ByVal name As String) As Nullable(Of Long)
        If reader.IsDBNull(reader.GetOrdinal(name)) Then Return Nothing
        Return New Nullable(Of Long)(Convert.ToInt64(reader(name)))
    End Function
    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
End Class

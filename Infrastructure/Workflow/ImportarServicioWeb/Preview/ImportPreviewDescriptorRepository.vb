Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class ImportPreviewDescriptorRepository
    Implements IImportPreviewDescriptorRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _transactions As ITransactionFactory
    Private ReadOnly _moduleContext As ContextoModulo

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor,
                   ByVal transactions As ITransactionFactory, ByVal moduleContext As ContextoModulo)
        If connections Is Nothing Then Throw New ArgumentNullException("connections")
        If executor Is Nothing Then Throw New ArgumentNullException("executor")
        If transactions Is Nothing Then Throw New ArgumentNullException("transactions")
        If moduleContext Is Nothing OrElse Not moduleContext.EsValido() Then Throw New ArgumentException("PREVIEW_MODULE_CONTEXT_INVALID", "moduleContext")
        _connections = connections
        _executor = executor
        _transactions = transactions
        _moduleContext = moduleContext
    End Sub

    Public Function Create(ByVal snapshot As ImportPreviewSnapshot) As Boolean Implements IImportPreviewDescriptorRepository.Create
        If snapshot Is Nothing OrElse snapshot.DescriptorHash Is Nothing OrElse snapshot.DescriptorHash.Length <> 32 OrElse
           snapshot.ResourceHash Is Nothing OrElse snapshot.ResourceHash.Length <> 32 OrElse snapshot.Content Is Nothing Then Return False
        Const sql As String = "INSERT INTO workflow_import_preview_descriptor (descriptor_hash,resource_hash,user_id,task_id,provider_id,content_type,content_length,content_disposition,safe_file_name,content,status,expires_utc,created_utc) VALUES (@descriptorHash,@resourceHash,@userId,@taskId,@providerId,@contentType,@contentLength,@disposition,@fileName,@content,@status,@expires,@created)"
        Using connection = _connections.CreateOpenConnection(_moduleContext)
            Return _executor.ExecuteNonQuery(connection, Nothing, sql, New List(Of IDataParameter) From {
                P("@descriptorHash", snapshot.DescriptorHash), P("@resourceHash", snapshot.ResourceHash),
                P("@userId", snapshot.UserId), P("@taskId", snapshot.TaskId), P("@providerId", snapshot.ProviderId),
                P("@contentType", snapshot.ContentType), P("@contentLength", snapshot.ContentLength),
                P("@disposition", snapshot.ContentDisposition), P("@fileName", snapshot.SafeFileName),
                P("@content", snapshot.Content), P("@status", "Disponible"), P("@expires", snapshot.ExpiresUtc),
                P("@created", snapshot.CreatedUtc)}) = 1
        End Using
    End Function

    Public Function GetAvailableMetadata(ByVal descriptorHash As Byte(), ByVal authority As ImportPreviewAuthority,
                                         ByVal utcNow As DateTime) As ImportPreviewSnapshot Implements IImportPreviewDescriptorRepository.GetAvailableMetadata
        Const sql As String = "SELECT id,user_id,task_id,provider_id,content_type,content_length,content_disposition,safe_file_name,status,expires_utc,created_utc FROM workflow_import_preview_descriptor WHERE descriptor_hash=@hash AND user_id=@userId AND task_id=@taskId AND provider_id=@providerId AND status='Disponible' AND expires_utc>@now LIMIT 1"
        Using connection = _connections.CreateOpenConnection(_moduleContext)
            Return _executor.ExecuteReader(connection, Nothing, sql, AuthorityParameters(descriptorHash, authority, utcNow), AddressOf MapMetadata)
        End Using
    End Function

    Public Function ClaimAndLoad(ByVal descriptorHash As Byte(), ByVal authority As ImportPreviewAuthority,
                                 ByVal utcNow As DateTime) As ImportPreviewSnapshot Implements IImportPreviewDescriptorRepository.ClaimAndLoad
        Using connection = _connections.CreateOpenConnection(_moduleContext)
            Using transaction = _transactions.BeginTransaction(connection)
                Try
                    Const claimSql As String = "UPDATE workflow_import_preview_descriptor SET status='Reclamado',claimed_utc=@now WHERE descriptor_hash=@hash AND user_id=@userId AND task_id=@taskId AND provider_id=@providerId AND status='Disponible' AND expires_utc>@now"
                    Dim parameters = AuthorityParameters(descriptorHash, authority, utcNow)
                    If _executor.ExecuteNonQuery(connection, transaction, claimSql, parameters) <> 1 Then transaction.Rollback() : Return Nothing
                    Const readSql As String = "SELECT id,user_id,task_id,provider_id,content_type,content_length,content_disposition,safe_file_name,content,status,expires_utc,created_utc FROM workflow_import_preview_descriptor WHERE descriptor_hash=@hash AND user_id=@userId AND task_id=@taskId AND provider_id=@providerId AND status='Reclamado' LIMIT 1"
                    Dim snapshot = _executor.ExecuteReader(connection, transaction, readSql,
                        AuthorityParameters(descriptorHash, authority, utcNow), AddressOf MapContent)
                    If snapshot Is Nothing Then transaction.Rollback() : Return Nothing
                    transaction.Commit()
                    Return snapshot
                Catch
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function

    Public Function MarkConsumed(ByVal id As Long, ByVal utcNow As DateTime) As Boolean Implements IImportPreviewDescriptorRepository.MarkConsumed
        Using connection = _connections.CreateOpenConnection(_moduleContext)
            Return _executor.ExecuteNonQuery(connection, Nothing,
                "UPDATE workflow_import_preview_descriptor SET status='Consumido',consumed_utc=@now,content=X'' WHERE id=@id AND status='Reclamado'",
                New List(Of IDataParameter) From {P("@now", utcNow), P("@id", id)}) = 1
        End Using
    End Function

    Public Function CleanupExpired(ByVal utcNow As DateTime) As Integer Implements IImportPreviewDescriptorRepository.CleanupExpired
        Using connection = _connections.CreateOpenConnection(_moduleContext)
            Return _executor.ExecuteNonQuery(connection, Nothing,
                "DELETE FROM workflow_import_preview_descriptor WHERE expires_utc<=@now OR (status='Consumido' AND consumed_utc<=@retention)",
                New List(Of IDataParameter) From {P("@now", utcNow), P("@retention", utcNow.AddMinutes(-5))})
        End Using
    End Function

    Private Shared Function AuthorityParameters(ByVal hash As Byte(), ByVal authority As ImportPreviewAuthority,
                                                ByVal utcNow As DateTime) As IList(Of IDataParameter)
        If hash Is Nothing OrElse hash.Length <> 32 OrElse authority Is Nothing Then Return New List(Of IDataParameter)()
        Return New List(Of IDataParameter) From {P("@hash", hash), P("@userId", authority.UserId),
            P("@taskId", authority.TaskId), P("@providerId", authority.ProviderId), P("@now", utcNow)}
    End Function

    Private Shared Function MapMetadata(ByVal reader As IDataReader) As ImportPreviewSnapshot
        If Not reader.Read() Then Return Nothing
        Return Map(reader, False)
    End Function

    Private Shared Function MapContent(ByVal reader As IDataReader) As ImportPreviewSnapshot
        If Not reader.Read() Then Return Nothing
        Return Map(reader, True)
    End Function

    Private Shared Function Map(ByVal reader As IDataReader, ByVal includeContent As Boolean) As ImportPreviewSnapshot
        Dim result As New ImportPreviewSnapshot With {
            .Id = Convert.ToInt64(reader("id")), .UserId = Convert.ToInt32(reader("user_id")),
            .TaskId = Convert.ToInt64(reader("task_id")), .ProviderId = Convert.ToString(reader("provider_id")),
            .ContentType = Convert.ToString(reader("content_type")), .ContentLength = Convert.ToInt64(reader("content_length")),
            .ContentDisposition = Convert.ToString(reader("content_disposition")), .SafeFileName = Convert.ToString(reader("safe_file_name")),
            .Status = Convert.ToString(reader("status")), .ExpiresUtc = Convert.ToDateTime(reader("expires_utc")),
            .CreatedUtc = Convert.ToDateTime(reader("created_utc"))}
        If includeContent Then result.Content = ReadBytes(reader, "content")
        Return result
    End Function

    Private Shared Function ReadBytes(ByVal record As IDataRecord, ByVal columnName As String) As Byte()
        Dim ordinal = record.GetOrdinal(columnName)
        If record.IsDBNull(ordinal) Then Return Nothing
        Dim length = record.GetBytes(ordinal, 0, Nothing, 0, 0)
        If length <= 0 Then Return New Byte() {}
        If length > Integer.MaxValue Then Throw New InvalidOperationException("PREVIEW_CONTENT_TOO_LARGE")
        Dim value(CInt(length) - 1) As Byte
        Dim offset As Long = 0
        While offset < length
            Dim read = record.GetBytes(ordinal, offset, value, CInt(offset), CInt(length - offset))
            If read <= 0 Then Throw New InvalidOperationException("PREVIEW_CONTENT_READ_INCOMPLETE")
            offset += read
        End While
        Return value
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function
End Class

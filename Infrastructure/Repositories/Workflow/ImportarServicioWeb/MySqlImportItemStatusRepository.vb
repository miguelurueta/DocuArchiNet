Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportItemStatusRepository
    Implements IImportItemStatusRepository
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _documentConnections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        Me.New(connections, connections, executor)
    End Sub

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal documentConnections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse documentConnections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections=connections : _documentConnections=documentConnections : _executor=executor
    End Sub

    Public Function ObtenerLote(ByVal contexto As ContextoImportacionServicio, ByVal providerId As String,
                                ByVal externalKeys As IList(Of String)) As IDictionary(Of String, EstadoItemListadoImportacion) Implements IImportItemStatusRepository.ObtenerLote
        If contexto Is Nothing OrElse contexto.IdTarea<=0 OrElse String.IsNullOrWhiteSpace(providerId) OrElse externalKeys Is Nothing Then Throw New InvalidOperationException("ITEM_STATUS_CONTEXT_INVALID")
        Dim result As New Dictionary(Of String,EstadoItemListadoImportacion)(StringComparer.Ordinal)
        Dim parameters As New List(Of IDataParameter) From {New MySqlParameter("@taskId",contexto.IdTarea),New MySqlParameter("@providerId",providerId.Trim())}
        Dim placeholders As New List(Of String)()
        For Each raw In externalKeys
            Dim key=If(raw,String.Empty).Trim()
            If key.Length=0 OrElse result.ContainsKey(key) Then Continue For
            result.Add(key,New EstadoItemListadoImportacion())
            Dim name="@externalKey" & placeholders.Count.ToString(Globalization.CultureInfo.InvariantCulture)
            placeholders.Add(name) : parameters.Add(New MySqlParameter(name,key))
        Next
        If placeholders.Count=0 Then Return result
        Dim sql As String = "SELECT item.external_key,i.status AS intent_status,item.status AS item_status,item.document_id,item.persistence_known," &
            "item.relation_status,item.index_status,item.cache_status,item.error_code," &
            "(SELECT related.cabinet_name FROM workflow_import_related_document related WHERE related.intent_id=item.intent_id " &
            "AND related.task_id=i.task_id AND related.image_id=item.document_id LIMIT 1) AS cabinet_name " &
            "FROM workflow_import_intent i INNER JOIN workflow_import_intent_item item ON item.intent_id=i.intent_id " &
            "WHERE i.task_id=@taskId AND item.provider_id=@providerId AND item.external_key IN (" & String.Join(",",placeholders.ToArray()) & ") " &
            "ORDER BY i.created_utc DESC"
        Dim rows As IList(Of StatusRow)
        Using connection=_connections.CreateOpenConnection(ModuleContext(contexto))
            rows=_executor.ExecuteReader(connection,Nothing,sql,parameters,AddressOf Map)
        End Using
        Dim existing=ObtenerDocumentosExistentes(contexto,rows)
        Dim verifiedMissing As ISet(Of String)=New HashSet(Of String)(StringComparer.Ordinal)
        For Each row In rows
            Dim verifiable=row.HistoricamenteConfirmado AndAlso row.DocumentId>0 AndAlso SafeIdentifier(row.CabinetName)
            Dim state=result(row.Key), physicallyPresent=verifiable AndAlso existing.Contains(PhysicalKey(row.CabinetName,row.DocumentId))
            If verifiable AndAlso Not physicallyPresent Then verifiedMissing.Add(row.Key)
            state.TieneAntecedente=True
            state.Confirmado = state.Confirmado OrElse physicallyPresent
            ' Un documento históricamente confirmado pero eliminado vuelve a estar disponible.
            ' Si la evidencia física no puede verificarse, se conserva como novedad y nunca se habilita a ciegas.
            state.TieneNovedad = state.TieneNovedad OrElse row.TieneNovedad OrElse (row.HistoricamenteConfirmado AndAlso Not verifiable)
        Next
        For Each pair In result
            If pair.Value.Confirmado OrElse verifiedMissing.Contains(pair.Key) Then pair.Value.TieneNovedad=False
        Next
        Return result
    End Function

    Private NotInheritable Class StatusRow
        Public Property Key As String
        Public Property DocumentId As Long
        Public Property CabinetName As String
        Public Property HistoricamenteConfirmado As Boolean
        Public Property TieneNovedad As Boolean
    End Class
    Private Shared Function Map(ByVal reader As IDataReader) As IList(Of StatusRow)
        Dim result As New List(Of StatusRow)()
        While reader.Read()
            Dim intentStatus=Convert.ToString(reader("intent_status")), itemStatus=Convert.ToString(reader("item_status"))
            Dim confirmed=Not Convert.IsDBNull(reader("document_id")) AndAlso Convert.ToBoolean(reader("persistence_known")) AndAlso
                String.Equals(intentStatus,"Completada",StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(itemStatus,"Completada",StringComparison.OrdinalIgnoreCase) AndAlso
                ResolvedEffect(reader("relation_status")) AndAlso
                ResolvedEffect(reader("index_status")) AndAlso
                ResolvedEffect(reader("cache_status"))
            result.Add(New StatusRow With {.Key=Convert.ToString(reader("external_key")),
                .DocumentId=If(Convert.IsDBNull(reader("document_id")),0,Convert.ToInt64(reader("document_id"))),
                .CabinetName=If(Convert.IsDBNull(reader("cabinet_name")),String.Empty,Convert.ToString(reader("cabinet_name"))),
                .HistoricamenteConfirmado=confirmed,
                .TieneNovedad=Not confirmed OrElse Not String.IsNullOrWhiteSpace(Convert.ToString(reader("error_code")))})
        End While
        Return result
    End Function

    Private Shared Function ResolvedEffect(ByVal value As Object) As Boolean
        Dim status=If(Convert.IsDBNull(value),String.Empty,Convert.ToString(value))
        Return String.Equals(status,"Confirmado",StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(status,"NoAplica",StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function ObtenerDocumentosExistentes(ByVal contexto As ContextoImportacionServicio,
                                                   ByVal rows As IList(Of StatusRow)) As ISet(Of String)
        Dim result As ISet(Of String)=New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim byCabinet As New Dictionary(Of String,List(Of Long))(StringComparer.OrdinalIgnoreCase)
        For Each row In rows
            If Not row.HistoricamenteConfirmado OrElse row.DocumentId<=0 OrElse Not SafeIdentifier(row.CabinetName) Then Continue For
            If Not byCabinet.ContainsKey(row.CabinetName) Then byCabinet.Add(row.CabinetName,New List(Of Long)())
            If Not byCabinet(row.CabinetName).Contains(row.DocumentId) Then byCabinet(row.CabinetName).Add(row.DocumentId)
        Next
        Using connection=_documentConnections.CreateOpenConnection(ModuleContext(contexto))
            For Each cabinet In byCabinet
                Dim parameters As New List(Of IDataParameter)(), placeholders As New List(Of String)()
                For index=0 To cabinet.Value.Count-1
                    Dim name="@documentId" & index.ToString(Globalization.CultureInfo.InvariantCulture)
                    placeholders.Add(name) : parameters.Add(New MySqlParameter(name,cabinet.Value(index)))
                Next
                Dim sql="SELECT ID FROM `" & cabinet.Key & "` WHERE ID IN (" & String.Join(",",placeholders.ToArray()) & ")"
                Dim ids=_executor.ExecuteReader(connection,Nothing,sql,parameters,AddressOf MapDocumentIds)
                For Each id In ids : result.Add(PhysicalKey(cabinet.Key,id)) : Next
            Next
        End Using
        Return result
    End Function

    Private Shared Function MapDocumentIds(ByVal reader As IDataReader) As IList(Of Long)
        Dim result As New List(Of Long)()
        While reader.Read() : result.Add(Convert.ToInt64(reader("ID"))) : End While
        Return result
    End Function

    Private Shared Function PhysicalKey(ByVal cabinet As String, ByVal documentId As Long) As String
        Return If(cabinet,String.Empty).Trim().ToUpperInvariant() & "|" & documentId.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function

    Private Shared Function SafeIdentifier(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value,"^[A-Za-z][A-Za-z0-9_]{0,63}$")
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class

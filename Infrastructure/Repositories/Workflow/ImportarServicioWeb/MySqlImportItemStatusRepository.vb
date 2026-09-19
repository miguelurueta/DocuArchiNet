Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportItemStatusRepository
    Implements IImportItemStatusRepository
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections=connections : _executor=executor
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
            "item.relation_status,item.index_status,item.cache_status,item.error_code " &
            "FROM workflow_import_intent i INNER JOIN workflow_import_intent_item item ON item.intent_id=i.intent_id " &
            "WHERE i.task_id=@taskId AND item.provider_id=@providerId AND item.external_key IN (" & String.Join(",",placeholders.ToArray()) & ") " &
            "ORDER BY i.created_utc DESC"
        Using connection=_connections.CreateOpenConnection(ModuleContext(contexto))
            Dim rows=_executor.ExecuteReader(connection,Nothing,sql,parameters,AddressOf Map)
            For Each row In rows
                Dim state=result(row.Key) : state.TieneAntecedente=True
                state.Confirmado = state.Confirmado OrElse row.Confirmado
                state.TieneNovedad = state.TieneNovedad OrElse row.TieneNovedad
            Next
            For Each state In result.Values : If state.Confirmado Then state.TieneNovedad=False
            Next
            Return result
        End Using
    End Function

    Private NotInheritable Class StatusRow
        Public Property Key As String
        Public Property Confirmado As Boolean
        Public Property TieneNovedad As Boolean
    End Class
    Private Shared Function Map(ByVal reader As IDataReader) As IList(Of StatusRow)
        Dim result As New List(Of StatusRow)()
        While reader.Read()
            Dim intentStatus=Convert.ToString(reader("intent_status")), itemStatus=Convert.ToString(reader("item_status"))
            Dim confirmed=Not Convert.IsDBNull(reader("document_id")) AndAlso Convert.ToBoolean(reader("persistence_known")) AndAlso
                String.Equals(intentStatus,"Completada",StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(itemStatus,"Completada",StringComparison.OrdinalIgnoreCase) AndAlso
                String.Equals(Convert.ToString(reader("relation_status")),"Confirmado",StringComparison.OrdinalIgnoreCase) AndAlso
                String.Equals(Convert.ToString(reader("index_status")),"Confirmado",StringComparison.OrdinalIgnoreCase) AndAlso
                String.Equals(Convert.ToString(reader("cache_status")),"Confirmado",StringComparison.OrdinalIgnoreCase)
            result.Add(New StatusRow With {.Key=Convert.ToString(reader("external_key")),.Confirmado=confirmed,
                .TieneNovedad=Not confirmed OrElse Not String.IsNullOrWhiteSpace(Convert.ToString(reader("error_code")))})
        End While
        Return result
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class

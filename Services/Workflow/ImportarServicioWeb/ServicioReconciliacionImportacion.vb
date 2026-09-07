Imports System
Imports System.Collections.Generic

Public NotInheritable Class ServicioReconciliacionImportacion
    Private ReadOnly _validator As ValidadorContextoImportacion
    Private ReadOnly _repository As IImportReconciliationRepository
    Private ReadOnly _mapper As ImportItemResultMapper

    Public Sub New(ByVal validator As ValidadorContextoImportacion, ByVal repository As IImportReconciliationRepository, ByVal mapper As ImportItemResultMapper)
        If validator Is Nothing OrElse repository Is Nothing OrElse mapper Is Nothing Then Throw New ArgumentNullException("dependency")
        _validator=validator : _repository=repository : _mapper=mapper
    End Sub

    Public Function [Get](ByVal context As ContextoImportacionServicio, ByVal request As GetImportIntentRequestDto) As GetImportIntentResponseDto
        Dim response As New GetImportIntentResponseDto With {.OperationId=If(request Is Nothing,Nothing,request.OperationId),.CorrelationId=If(request Is Nothing,Nothing,request.CorrelationId)}
        Dim snapshot = AuthorizedSnapshot(context, If(request Is Nothing,Nothing,request.IntentId), Nothing)
        If snapshot Is Nothing Then response.Error=SafeError() : Return response
        response.IntentId=snapshot.IntentId : response.Status=snapshot.Fase.ToString() : response.VersionToken=snapshot.VersionToken
        For Each item In Project(snapshot) : response.Items.Add(item) : Next
        Return response
    End Function

    Public Function Reconcile(ByVal context As ContextoImportacionServicio, ByVal request As ReconcileImportIntentRequestDto) As ReconcileImportIntentResponseDto
        Dim response As New ReconcileImportIntentResponseDto With {.OperationId=If(request Is Nothing,Nothing,request.OperationId),.CorrelationId=If(request Is Nothing,Nothing,request.CorrelationId)}
        Dim snapshot = AuthorizedSnapshot(context, If(request Is Nothing,Nothing,request.IntentId), request)
        If snapshot Is Nothing Then response.Error=SafeError() : Return response
        response.IntentId=snapshot.IntentId : response.Status=snapshot.Fase.ToString() : response.VersionToken=snapshot.VersionToken
        For Each item In Project(snapshot)
            response.Items.Add(item) : If item.Status="Disponible" Then response.ConfirmedDocumentCount+=1
        Next
        Return response
    End Function

    Private Function AuthorizedSnapshot(ByVal context As ContextoImportacionServicio, ByVal intentId As String, ByVal request As ReconcileImportIntentRequestDto) As SnapshotReconciliacionImportacion
        If context Is Nothing OrElse String.IsNullOrWhiteSpace(intentId) OrElse Not _validator.Validar(context).Valido Then Return Nothing
        If request IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(request.ExternalKey) Then Return _repository.ObtenerItem(context,intentId,context.ProviderId,request.ExternalKey)
        Return _repository.Obtener(context,intentId)
    End Function

    Private Function Project(ByVal snapshot As SnapshotReconciliacionImportacion) As IList(Of ImportItemResultDto)
        Dim result As New List(Of ImportItemResultDto)() : Dim confirmed As New HashSet(Of String)(StringComparer.Ordinal)
        For Each item In snapshot.Items
            Dim mapped=_mapper.Map(item,snapshot.IdTareaOriginal)
            If mapped.Status="Disponible" Then
                Dim key=mapped.TaskId.ToString() & ":" & mapped.DocumentId.Value.ToString()
                If confirmed.Contains(key) Then Continue For
                confirmed.Add(key)
            End If
            result.Add(mapped)
        Next
        Return result
    End Function
    Private Shared Function SafeError() As ErrorImportacionServicioDto
        Return New ErrorImportacionServicioDto With {.Codigo="IMPORT_INTENT_UNAVAILABLE",.MensajeVisible="No fue posible consultar la intención.",.EsReintentable=False}
    End Function
End Class

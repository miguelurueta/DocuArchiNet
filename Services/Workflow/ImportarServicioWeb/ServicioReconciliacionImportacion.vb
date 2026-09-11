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

    Public Function GetImportIntent(ByVal context As ContextoImportacionServicio, ByVal request As GetImportIntentRequestDto) As GetImportIntentResponseDto
        Dim response As New GetImportIntentResponseDto With {.OperationId=If(request Is Nothing,Nothing,request.OperationId),.CorrelationId=If(request Is Nothing,Nothing,request.CorrelationId)}
        Dim snapshot = AuthorizedSnapshot(context, If(request Is Nothing,Nothing,request.IntentId), Nothing)
        If snapshot Is Nothing Then response.Error=SafeError() : Return response
        response.IntentId=snapshot.IntentId : response.VersionToken=snapshot.VersionToken
        For Each item In Project(snapshot) : response.Items.Add(item) : Next
        response.Status=AggregateStatus(response.Items)
        Return response
    End Function

    Public Function ReconcileImportIntent(ByVal context As ContextoImportacionServicio, ByVal request As ReconcileImportIntentRequestDto) As ReconcileImportIntentResponseDto
        Dim response As New ReconcileImportIntentResponseDto With {.OperationId=If(request Is Nothing,Nothing,request.OperationId),.CorrelationId=If(request Is Nothing,Nothing,request.CorrelationId)}
        Dim snapshot = AuthorizedSnapshot(context, If(request Is Nothing,Nothing,request.IntentId), request)
        If snapshot Is Nothing Then response.Error=SafeError() : Return response
        response.IntentId=snapshot.IntentId : response.VersionToken=snapshot.VersionToken
        For Each item In Project(snapshot)
            response.Items.Add(item) : If item.Status="Disponible" Then response.ConfirmedDocumentCount+=1
        Next
        response.Status=AggregateStatus(response.Items)
        Return response
    End Function

    Public Function ProjectExecutionResult(ByVal context As ContextoImportacionServicio, ByVal execution As ExecuteImportIntentResponseDto) As ExecuteImportIntentResponseDto
        If execution Is Nothing Then Throw New ArgumentNullException("execution")
        If execution.Error IsNot Nothing OrElse Not execution.Accepted Then Return execution

        Dim snapshot = AuthorizedSnapshot(context, execution.IntentId, Nothing)
        If snapshot Is Nothing Then Return ProtectUnconfirmedExecution(context, execution)

        Dim response As New ExecuteImportIntentResponseDto With {
            .OperationId=execution.OperationId,
            .CorrelationId=execution.CorrelationId,
            .IntentId=snapshot.IntentId,
            .Accepted=True,
            .VersionToken=snapshot.VersionToken}
        For Each item In Project(snapshot) : response.Items.Add(item) : Next
        response.Status=AggregateStatus(response.Items)
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
    Private Shared Function ProtectUnconfirmedExecution(ByVal context As ContextoImportacionServicio, ByVal execution As ExecuteImportIntentResponseDto) As ExecuteImportIntentResponseDto
        execution.Status="ResultadoIncierto"
        For Each item In execution.Items
            item.ReachedPhase=item.Status
            item.Status="ResultadoIncierto"
            item.TaskId=If(context Is Nothing,0,context.IdTarea)
            item.DocumentId=Nothing : item.DocumentName=Nothing : item.ContentType=Nothing
            item.ErrorCode="IMPORT_RESULT_UNCERTAIN" : item.Message="Se está verificando el resultado."
            item.Retryable=False
        Next
        Return execution
    End Function
    Private Shared Function SafeError() As ErrorImportacionServicioDto
        Return New ErrorImportacionServicioDto With {.Codigo="IMPORT_INTENT_UNAVAILABLE",.MensajeVisible="No fue posible consultar la intención.",.EsReintentable=False}
    End Function
    Private Shared Function AggregateStatus(ByVal items As IList(Of ImportItemResultDto)) As String
        If items Is Nothing OrElse items.Count=0 Then Return "Verificando"
        Dim available=0 : Dim failed=0 : Dim stopped=0 : Dim uncertain=0
        For Each item In items
            If item.Status="Disponible" Then available+=1
            If item.Status="Fallido" OrElse item.Status="Inconsistente" Then failed+=1
            If item.Status="Detenido" Then stopped+=1
            If item.Status="ResultadoIncierto" OrElse item.Status="Verificando" Then uncertain+=1
        Next
        If available=items.Count Then Return "Completado"
        If stopped=items.Count Then Return "Detenido"
        If failed=items.Count Then Return "Fallido"
        If uncertain=items.Count Then Return "ResultadoIncierto"
        Return "Parcial"
    End Function
End Class

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
        For Each effect In ProjectExpedientEffects(snapshot) : response.ExpedientEffects.Add(effect) : Next
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
        For Each effect In ProjectExpedientEffects(snapshot) : response.ExpedientEffects.Add(effect) : Next
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
        For Each item In Project(snapshot)
            PreserveExecutionProjection(context, execution, item)
            PreserveWorkflowExecutionProjection(context, execution, item)
            response.Items.Add(item)
        Next
        For Each effect In ProjectExpedientEffects(snapshot) : response.ExpedientEffects.Add(effect) : Next
        response.Status=AggregateStatus(response.Items)
        Return response
    End Function

    Private Shared Sub PreserveExecutionProjection(ByVal context As ContextoImportacionServicio,
                                                   ByVal execution As ExecuteImportIntentResponseDto,
                                                   ByVal authoritative As ImportItemResultDto)
        If context Is Nothing OrElse execution Is Nothing OrElse authoritative Is Nothing OrElse
           Not String.Equals(context.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) OrElse
           Not String.Equals(authoritative.Status, "Disponible", StringComparison.Ordinal) OrElse
           Not authoritative.DocumentId.HasValue OrElse authoritative.DocumentId.Value <= 0 OrElse
           authoritative.TaskId <> context.IdTarea OrElse String.IsNullOrWhiteSpace(authoritative.ClientItemId) OrElse
           String.IsNullOrWhiteSpace(authoritative.ExternalKey) Then Return

        For Each candidate In execution.Items
            If candidate Is Nothing OrElse candidate.EnlaseProjection Is Nothing OrElse
               Not candidate.DocumentId.HasValue OrElse candidate.DocumentId.Value <> authoritative.DocumentId.Value OrElse
               candidate.TaskId <> authoritative.TaskId OrElse
               Not String.Equals(candidate.ClientItemId, authoritative.ClientItemId, StringComparison.Ordinal) OrElse
               Not String.Equals(candidate.ExternalKey, authoritative.ExternalKey, StringComparison.Ordinal) Then Continue For

            Dim projection = candidate.EnlaseProjection
            If projection.DocumentId <> authoritative.DocumentId.Value OrElse projection.TaskId <> authoritative.TaskId OrElse
               String.IsNullOrWhiteSpace(projection.CabinetName) OrElse String.IsNullOrWhiteSpace(projection.Radicado) OrElse
               String.IsNullOrWhiteSpace(projection.StorageType) OrElse String.IsNullOrWhiteSpace(projection.DocumentName) OrElse
               String.IsNullOrWhiteSpace(projection.IconClass) Then Continue For
            authoritative.EnlaseProjection = projection
            Exit For
        Next
    End Sub

    Private Shared Sub PreserveWorkflowExecutionProjection(ByVal context As ContextoImportacionServicio,
                                                           ByVal execution As ExecuteImportIntentResponseDto,
                                                           ByVal authoritative As ImportItemResultDto)
        If context Is Nothing OrElse execution Is Nothing OrElse authoritative Is Nothing OrElse
           String.Equals(context.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) OrElse
           Not String.Equals(authoritative.Status, "Disponible", StringComparison.Ordinal) OrElse
           Not authoritative.DocumentId.HasValue OrElse authoritative.DocumentId.Value <= 0 OrElse
           authoritative.TaskId <> context.IdTarea OrElse String.IsNullOrWhiteSpace(authoritative.ClientItemId) OrElse
           String.IsNullOrWhiteSpace(authoritative.ExternalKey) Then Return

        For Each candidate In execution.Items
            If candidate Is Nothing OrElse candidate.WorkflowProjection Is Nothing OrElse
               Not candidate.DocumentId.HasValue OrElse candidate.DocumentId.Value <> authoritative.DocumentId.Value OrElse
               candidate.TaskId <> authoritative.TaskId OrElse
               Not String.Equals(candidate.ClientItemId, authoritative.ClientItemId, StringComparison.Ordinal) OrElse
               Not String.Equals(candidate.ExternalKey, authoritative.ExternalKey, StringComparison.Ordinal) Then Continue For

            Dim projection = candidate.WorkflowProjection
            If projection.DocumentId <> authoritative.DocumentId.Value OrElse projection.TaskId <> authoritative.TaskId OrElse
               String.IsNullOrWhiteSpace(projection.CabinetName) OrElse String.IsNullOrWhiteSpace(projection.Radicado) OrElse
               String.IsNullOrWhiteSpace(projection.StorageType) OrElse String.IsNullOrWhiteSpace(projection.DocumentTypeName) OrElse
               String.IsNullOrWhiteSpace(projection.IconClass) Then Continue For
            authoritative.WorkflowProjection = projection
            Exit For
        Next
    End Sub

    Private Function AuthorizedSnapshot(ByVal context As ContextoImportacionServicio, ByVal intentId As String, ByVal request As ReconcileImportIntentRequestDto) As SnapshotReconciliacionImportacion
        If context Is Nothing OrElse String.IsNullOrWhiteSpace(intentId) OrElse Not _validator.Validar(context).Valido Then Return Nothing
        Dim snapshot As SnapshotReconciliacionImportacion
        If request IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(request.ExternalKey) Then
            snapshot = _repository.ObtenerItem(context,intentId,context.ProviderId,request.ExternalKey)
        Else
            snapshot = _repository.Obtener(context,intentId)
        End If
        If snapshot Is Nothing OrElse Not _validator.Validar(context, snapshot.ContextoOriginal).Valido Then Return Nothing
        Return snapshot
    End Function

    Private Function Project(ByVal snapshot As SnapshotReconciliacionImportacion) As IList(Of ImportItemResultDto)
        Dim result As New List(Of ImportItemResultDto)() : Dim confirmed As New HashSet(Of String)(StringComparer.Ordinal)
        Dim requiresExpedientEvidence = snapshot.ContextoOriginal Is Nothing OrElse
            Not String.Equals(snapshot.ContextoOriginal.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase)
        For Each item In snapshot.Items
            Dim mapped=_mapper.Map(item,snapshot.IdTareaOriginal)
            If requiresExpedientEvidence Then ApplyExpedientEvidence(mapped, item, snapshot.DocumentosRelacionados)
            If mapped.Status="Disponible" Then
                Dim key=mapped.TaskId.ToString() & ":" & mapped.DocumentId.Value.ToString()
                If confirmed.Contains(key) Then Continue For
                confirmed.Add(key)
            End If
            result.Add(mapped)
        Next
        Return result
    End Function

    Private Function ProjectExpedientEffects(ByVal snapshot As SnapshotReconciliacionImportacion) As IList(Of ImportItemExpedientEffectsDto)
        Dim result As New List(Of ImportItemExpedientEffectsDto)()
        If snapshot Is Nothing OrElse snapshot.DocumentosRelacionados Is Nothing Then Return result
        For Each document In snapshot.DocumentosRelacionados
            Dim clientItemId As String = Nothing
            For Each item In snapshot.Items
                If item.IdDocumento.HasValue AndAlso item.IdDocumento.Value = document.IdImagen Then
                    clientItemId = item.ClientItemId
                    Exit For
                End If
            Next
            result.Add(_mapper.MapExpedientEffects(clientItemId, document))
        Next
        Return result
    End Function

    Private Sub ApplyExpedientEvidence(ByVal mapped As ImportItemResultDto,
                                       ByVal item As SnapshotItemReconciliacionImportacion,
                                       ByVal documents As IList(Of DocumentoRelacionadoImportacion))
        If Not item.IdDocumento.HasValue OrElse documents Is Nothing Then Return
        Dim matches As New List(Of DocumentoRelacionadoImportacion)()
        For Each document In documents
            If document.IdImagen = item.IdDocumento.Value Then matches.Add(document)
        Next
        If matches.Count = 0 Then
            RejectExpedientEvidence(mapped, ImportExpedientResultCodes.ExpedientUnresolved)
            Return
        End If
        If matches.Count > 1 Then
            RejectExpedientEvidence(mapped, ImportExpedientResultCodes.RelationDuplicated)
            Return
        End If
        Dim effect = _mapper.MapExpedientEffects(item.ClientItemId, matches(0))
        If effect.ResultCode <> ImportExpedientResultCodes.Confirmed Then RejectExpedientEvidence(mapped, effect.ResultCode)
    End Sub

    Private Shared Sub RejectExpedientEvidence(ByVal mapped As ImportItemResultDto, ByVal code As String)
        mapped.Status = If(code = ImportExpedientResultCodes.EffectUncertain, "ResultadoIncierto", "Inconsistente")
        mapped.ErrorCode = code : mapped.Message = "Los efectos documentales requieren reconciliación."
        mapped.DocumentId = Nothing : mapped.DocumentName = Nothing : mapped.ContentType = Nothing
    End Sub
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
        Dim available=0 : Dim failed=0 : Dim stopped=0 : Dim uncertain=0 : Dim recoverable=0
        For Each item In items
            If item.Status="Disponible" Then available+=1
            If item.Status="Fallido" OrElse item.Status="Inconsistente" Then failed+=1
            If item.Status="Detenido" Then stopped+=1
            If item.Status="ResultadoIncierto" OrElse item.Status="Verificando" Then uncertain+=1
            If item.Status="Recuperable" Then recoverable+=1
        Next
        If available=items.Count Then Return "Completado"
        If stopped=items.Count Then Return "Detenido"
        If failed=items.Count Then Return "Fallido"
        If uncertain=items.Count Then Return "ResultadoIncierto"
        If recoverable=items.Count Then Return "Recuperable"
        Return "Parcial"
    End Function
End Class

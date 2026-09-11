Imports System
Imports System.Collections.Generic

Public NotInheritable Class ImportServiceOrchestrator
    Private ReadOnly _validator As ValidadorContextoImportacion
    Private ReadOnly _repository As IImportIntentRepository
    Private ReadOnly _machine As ImportIntentStateMachine
    Private ReadOnly _steps As IList(Of IImportExecutionStep)

    Public Sub New(ByVal validator As ValidadorContextoImportacion, ByVal repository As IImportIntentRepository,
                   ByVal machine As ImportIntentStateMachine, ByVal steps As IList(Of IImportExecutionStep))
        If validator Is Nothing OrElse repository Is Nothing OrElse machine Is Nothing OrElse steps Is Nothing Then Throw New ArgumentNullException("dependency")
        _validator = validator : _repository = repository : _machine = machine : _steps = steps
    End Sub

    Public Function Execute(ByVal contexto As ContextoImportacionServicio, ByVal request As ExecuteImportIntentRequestDto) As ExecuteImportIntentResponseDto
        Dim response = New ExecuteImportIntentResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        Dim validacion = _validator.Validar(contexto)
        If request Is Nothing OrElse Not validacion.Valido Then Return ErrorExecute(response, If(request Is Nothing, "INVALID_REQUEST", validacion.Codigo), "No fue posible iniciar la ejecución.")
        Dim intent = _repository.Obtener(contexto, request.IntentId)
        If intent Is Nothing Then Return ErrorExecute(response, "INTENT_NOT_FOUND", "La intención no está disponible.")
        If Not String.Equals(intent.VersionToken, request.VersionToken, StringComparison.Ordinal) Then Return ErrorExecute(response, "VERSION_CONFLICT", "La intención cambió; consulte su estado.")
        intent.DetencionSolicitada = request.StopRequested
        response.IntentId = intent.Id
        For Each item In intent.Resultados
            item.CorrelationId = request.CorrelationId
            If intent.DetencionSolicitada Then
                Dim fasePendiente = item.Fase : ImportItemResultFactory.Detenido(item) : item.Fase = fasePendiente
                Avanzar(contexto, intent, item, FaseImportacionServicio.Detenida, request.CorrelationId)
                response.Items.Add(MapItem(item)) : Continue For
            End If
            If item.Fase = FaseImportacionServicio.Creada AndAlso Not Avanzar(contexto, intent, item, FaseImportacionServicio.Validada, request.CorrelationId) Then Return ErrorExecute(response, "VERSION_CONFLICT", "La intención cambió; consulte su estado.")
            If EsReintentoSeguro(item) Then
                item.CodigoError = Nothing : item.MensajeVisible = Nothing : item.Reintentable = False
                If Not Avanzar(contexto, intent, item, FaseImportacionServicio.Validada, request.CorrelationId) Then Return ErrorExecute(response, "VERSION_CONFLICT", "La intención cambió; consulte su estado.")
            ElseIf item.Fase <> FaseImportacionServicio.Validada AndAlso item.Fase <> FaseImportacionServicio.Creada Then
                Return ErrorExecute(response, "IMPORT_RETRY_NOT_ALLOWED", "La intención no admite reintento.")
            End If
            For Each stepItem In _steps
                If intent.DetencionSolicitada Then Exit For
                Dim result = stepItem.Ejecutar(contexto, intent, item)
                If result Is Nothing OrElse Not result.Exitoso Then
                    Dim faseConfirmada = item.Fase
                    Dim fallo = If(result, New ResultadoFaseImportacion With {.Codigo = "EXECUTION_UNAVAILABLE", .MensajeVisible = "No fue posible continuar."})
                    ImportItemResultFactory.DesdeFallo(item, fallo, Not fallo.PersistenciaConocida)
                    Dim faseFallo = item.Fase : item.Fase = faseConfirmada
                    Avanzar(contexto, intent, item, faseFallo, request.CorrelationId) : Exit For
                End If
                item.IdDocumento = If(result.IdDocumento.HasValue, result.IdDocumento, item.IdDocumento)
                item.PersistenciaConocida = result.PersistenciaConocida
                If Not Avanzar(contexto, intent, item, stepItem.FaseConfirmada, request.CorrelationId) Then Return ErrorExecute(response, "VERSION_CONFLICT", "La intención cambió; consulte su estado.")
            Next
            If intent.DetencionSolicitada Then ImportItemResultFactory.Detenido(item)
            response.Items.Add(MapItem(item))
        Next
        response.Accepted = True : response.Status = AggregateStatus(intent).ToString() : response.VersionToken = intent.VersionToken
        Return response
    End Function

    Private Shared Function EsReintentoSeguro(ByVal item As ResultadoElementoImportacion) As Boolean
        If item Is Nothing OrElse Not item.PersistenciaConocida OrElse item.IdDocumento.HasValue Then Return False
        If item.Fase = FaseImportacionServicio.RecursoObtenido OrElse
           item.Fase = FaseImportacionServicio.ExpedientePreparado OrElse
           item.Fase = FaseImportacionServicio.IndicesActualizados Then Return True
        If Not item.Reintentable Then Return False
        Return item.Fase = FaseImportacionServicio.FallidaAntesDePersistir OrElse
               item.Fase = FaseImportacionServicio.Parcial OrElse item.Fase = FaseImportacionServicio.Detenida
    End Function

    Public Function [Get](ByVal contexto As ContextoImportacionServicio, ByVal request As GetImportIntentRequestDto) As GetImportIntentResponseDto
        Dim response = New GetImportIntentResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        Dim validacion = _validator.Validar(contexto)
        If request Is Nothing OrElse Not validacion.Valido Then Return ErrorGet(response, If(request Is Nothing, "INVALID_REQUEST", validacion.Codigo))
        Dim intent = _repository.Obtener(contexto, request.IntentId)
        If intent Is Nothing Then Return ErrorGet(response, "INTENT_NOT_FOUND")
        response.IntentId = intent.Id : response.Status = intent.Fase.ToString() : response.VersionToken = intent.VersionToken
        For Each item In intent.Resultados : response.Items.Add(MapItem(item)) : Next
        Return response
    End Function

    Private Function Avanzar(ByVal contexto As ContextoImportacionServicio, ByVal intent As IntencionImportacionServicio, ByVal item As ResultadoElementoImportacion, ByVal destino As FaseImportacionServicio, ByVal correlationId As String) As Boolean
        Dim nuevaVersion = Guid.NewGuid().ToString("N")
        Dim decision = _machine.Intentar(intent.Id, item, destino, intent.VersionToken, nuevaVersion, correlationId)
        If Not decision.Aceptada OrElse Not _repository.ActualizarTransicion(contexto, decision.Transicion, item) Then Return False
        item.Fase = destino : intent.Fase = destino : intent.VersionToken = nuevaVersion : Return True
    End Function

    Private Shared Function AggregateStatus(ByVal intent As IntencionImportacionServicio) As FaseImportacionServicio
        Dim completas = 0 : Dim detenidas = 0
        For Each item In intent.Resultados
            If item.Fase = FaseImportacionServicio.Completada Then completas += 1
            If item.Fase = FaseImportacionServicio.Detenida Then detenidas += 1
            If item.Fase = FaseImportacionServicio.ResultadoIncierto Then Return FaseImportacionServicio.ResultadoIncierto
        Next
        If completas = intent.Resultados.Count Then Return FaseImportacionServicio.Completada
        If detenidas = intent.Resultados.Count Then Return FaseImportacionServicio.Detenida
        Return FaseImportacionServicio.Parcial
    End Function

    Private Shared Function MapItem(ByVal item As ResultadoElementoImportacion) As ImportItemResultDto
        Return New ImportItemResultDto With {.ClientItemId = item.ClientItemId, .ExternalKey = If(item.IdentidadExterna Is Nothing, Nothing, item.IdentidadExterna.ExternalKey), .Status = item.Fase.ToString(), .DocumentId = item.IdDocumento, .ErrorCode = item.CodigoError, .Message = item.MensajeVisible, .PersistenceKnown = item.PersistenciaConocida, .Retryable = item.Reintentable, .CorrelationId = item.CorrelationId}
    End Function
    Private Shared Function ErrorExecute(ByVal r As ExecuteImportIntentResponseDto, ByVal code As String, ByVal message As String) As ExecuteImportIntentResponseDto
        r.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = message} : Return r
    End Function
    Private Shared Function ErrorGet(ByVal r As GetImportIntentResponseDto, ByVal code As String) As GetImportIntentResponseDto
        r.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = "No fue posible consultar la intención."} : Return r
    End Function
End Class

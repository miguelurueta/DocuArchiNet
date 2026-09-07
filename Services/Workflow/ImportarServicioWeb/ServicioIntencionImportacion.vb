Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class ServicioIntencionImportacion
    Private ReadOnly _repository As IImportIntentRepository
    Private ReadOnly _guard As IImportIntentConcurrencyGuard
    Private ReadOnly _clock As IImportacionServicioClock

    Public Sub New(ByVal repository As IImportIntentRepository, ByVal guard As IImportIntentConcurrencyGuard,
                   ByVal clock As IImportacionServicioClock)
        If repository Is Nothing Then Throw New ArgumentNullException("repository")
        If guard Is Nothing Then Throw New ArgumentNullException("guard")
        If clock Is Nothing Then Throw New ArgumentNullException("clock")
        _repository = repository : _guard = guard : _clock = clock
    End Sub

    Public Function Crear(ByVal contexto As ContextoImportacionServicio,
                          ByVal request As CreateImportIntentRequestDto) As CreateImportIntentResponseDto
        Dim response As New CreateImportIntentResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        If Not Valid(contexto, request) Then Return Fail(response, "INVALID_INTENT", "No fue posible crear la intención.")
        Dim lockResult = _guard.Adquirir(contexto, request.IdempotencyKey.Trim())
        If lockResult Is Nothing OrElse Not lockResult.Adquirido OrElse lockResult.Lease Is Nothing Then Return Fail(response, "INTENT_IN_PROGRESS", "La intención está siendo procesada.")
        Using lockResult.Lease
            Dim intent = BuildIntent(contexto, request)
            Dim stored = _repository.CrearOReutilizar(contexto, intent)
            If stored Is Nothing OrElse stored.Intencion Is Nothing Then Return Fail(response, If(stored Is Nothing, "INTENT_UNAVAILABLE", stored.Codigo), If(stored Is Nothing, "No fue posible crear la intención.", stored.MensajeVisible))
            response.IntentId = stored.Intencion.Id
            response.Status = stored.Intencion.Fase.ToString()
            response.VersionToken = stored.Intencion.VersionToken
            response.Reused = stored.Reutilizada
            Return response
        End Using
    End Function

    Private Function BuildIntent(ByVal context As ContextoImportacionServicio, ByVal request As CreateImportIntentRequestDto) As IntencionImportacionServicio
        Dim now = _clock.UtcNow()
        Dim intent As New IntencionImportacionServicio With {
            .Id = Guid.NewGuid().ToString("N"), .IdempotencyKey = request.IdempotencyKey.Trim(),
            .ContextoOriginal = New ContextoIntencionImportacion With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId, .IdUsuario = context.IdUsuario, .IdGrupo = context.IdGrupo, .LoginUsuario = context.LoginUsuario, .IdTarea = context.IdTarea, .IdRuta = context.IdRuta, .IdTramite = context.IdTramite, .ProviderId = context.ProviderId.Trim()},
            .Fase = FaseImportacionServicio.Creada, .FechaCreacionUtc = now, .FechaActualizacionUtc = now}
        For Each requirement In request.Requirements
            intent.Requisitos.Add(New RequisitoPlanImportacion With {.Codigo = requirement.Codigo, .Satisfecho = requirement.Satisfecho, .MensajeVisible = requirement.MensajeVisible})
        Next
        For Each item In request.Items
            intent.Resultados.Add(New ResultadoElementoImportacion With {.ClientItemId = item.ClientItemId.Trim(), .IdentidadExterna = New IdentidadExternaImportacion With {.ProviderId = context.ProviderId.Trim(), .ExternalKey = item.ExternalKey.Trim()}, .IdTareaDestino = item.TargetTaskId, .IdTipoDocumental = item.DocumentTypeId, .NombreArchivo = item.FileName, .TipoContenido = item.ContentType, .Fase = FaseImportacionServicio.Creada})
        Next
        intent.HuellaContexto = Hash(Canonical(intent))
        intent.VersionToken = intent.HuellaContexto
        Return intent
    End Function

    Public Shared Function Canonical(ByVal intent As IntencionImportacionServicio) As String
        Dim parts As New List(Of String) From {Field(intent.ContextoOriginal.OperationId), Field(intent.ContextoOriginal.IdUsuario.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdGrupo.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdTarea.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdRuta.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdTramite.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.ProviderId.ToLowerInvariant())}
        Dim requirements As New List(Of String)()
        For Each value In intent.Requisitos
            requirements.Add(Field(value.Codigo) & Field(If(value.Satisfecho, "1", "0")))
        Next
        requirements.Sort(StringComparer.Ordinal)
        Dim items As New List(Of String)()
        For Each value In intent.Resultados
            items.Add(Field(value.IdentidadExterna.ProviderId.ToLowerInvariant()) & Field(value.IdentidadExterna.ExternalKey) & Field(value.IdTareaDestino.ToString(CultureInfo.InvariantCulture)) & Field(If(value.IdTipoDocumental.HasValue, value.IdTipoDocumental.Value.ToString(CultureInfo.InvariantCulture), String.Empty)))
        Next
        items.Sort(StringComparer.Ordinal)
        parts.Add(String.Join(String.Empty, requirements)) : parts.Add(String.Join(String.Empty, items))
        Return String.Join("|", parts)
    End Function

    Private Shared Function Field(ByVal value As String) As String
        Dim safe = If(value, String.Empty).Trim()
        Return safe.Length.ToString(CultureInfo.InvariantCulture) & ":" & safe
    End Function
    Private Shared Function Hash(ByVal value As String) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function
    Private Shared Function Valid(ByVal context As ContextoImportacionServicio, ByVal request As CreateImportIntentRequestDto) As Boolean
        Return context IsNot Nothing AndAlso request IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(request.IdempotencyKey) AndAlso request.Items IsNot Nothing AndAlso request.Items.Count > 0 AndAlso request.Requirements IsNot Nothing
    End Function
    Private Shared Function Fail(ByVal response As CreateImportIntentResponseDto, ByVal code As String, ByVal message As String) As CreateImportIntentResponseDto
        response.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = message, .EsReintentable = code = "INTENT_IN_PROGRESS" OrElse code = "INTENT_UNAVAILABLE"}
        Return response
    End Function
End Class

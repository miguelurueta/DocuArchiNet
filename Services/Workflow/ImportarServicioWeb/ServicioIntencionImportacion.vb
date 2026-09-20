Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class ServicioIntencionImportacion
    Private ReadOnly _repository As IImportIntentRepository
    Private ReadOnly _guard As IImportIntentConcurrencyGuard
    Private ReadOnly _clock As IImportacionServicioClock
    Private ReadOnly _inscriptions As IImportInscriptionResolver
    Private ReadOnly _preflight As ServicioPreflightImportacion

    Public Sub New(ByVal repository As IImportIntentRepository, ByVal guard As IImportIntentConcurrencyGuard,
                   ByVal clock As IImportacionServicioClock)
        Me.New(repository, guard, clock, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal repository As IImportIntentRepository, ByVal guard As IImportIntentConcurrencyGuard,
                   ByVal clock As IImportacionServicioClock, ByVal inscriptions As IImportInscriptionResolver)
        Me.New(repository, guard, clock, inscriptions, Nothing)
    End Sub

    Public Sub New(ByVal repository As IImportIntentRepository, ByVal guard As IImportIntentConcurrencyGuard,
                   ByVal clock As IImportacionServicioClock, ByVal inscriptions As IImportInscriptionResolver,
                   ByVal preflight As ServicioPreflightImportacion)
        If repository Is Nothing Then Throw New ArgumentNullException("repository")
        If guard Is Nothing Then Throw New ArgumentNullException("guard")
        If clock Is Nothing Then Throw New ArgumentNullException("clock")
        _repository = repository : _guard = guard : _clock = clock : _inscriptions = inscriptions
        _preflight = preflight
    End Sub

    Public Function Crear(ByVal contexto As ContextoImportacionServicio,
                          ByVal request As CreateImportIntentRequestDto) As CreateImportIntentResponseDto
        Dim response As New CreateImportIntentResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        If Not Valid(contexto, request) Then Return Fail(response, "INVALID_INTENT", "No fue posible crear la intención.")
        If _preflight IsNot Nothing Then
            Dim preflightRequest As New PreflightImportRequestDto With {
                .OperationId = request.OperationId, .CorrelationId = request.CorrelationId,
                .TaskId = request.TaskId, .ProviderId = request.ProviderId, .Items = request.Items}
            Dim current = _preflight.Preflight(contexto, preflightRequest)
            If current Is Nothing OrElse Not current.Executable OrElse current.Error IsNot Nothing OrElse
               Not String.Equals(current.ContextFingerprint, request.ContextFingerprint, StringComparison.Ordinal) OrElse
               Not EquivalentRequirements(current.Requirements, request.Requirements) Then
                Return Fail(response, "PREFLIGHT_STALE", "El plan de importación cambió; repita la validación.")
            End If
        End If
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

    Private Shared Function EquivalentRequirements(ByVal expected As IList(Of ImportRequirementDto), ByVal supplied As IList(Of ImportRequirementDto)) As Boolean
        If expected Is Nothing OrElse supplied Is Nothing OrElse expected.Count <> supplied.Count Then Return False
        Dim values As New Dictionary(Of String, Boolean)(StringComparer.Ordinal)
        For Each requirement In supplied
            If requirement Is Nothing OrElse String.IsNullOrWhiteSpace(requirement.Codigo) OrElse values.ContainsKey(requirement.Codigo) Then Return False
            values.Add(requirement.Codigo, requirement.Satisfecho)
        Next
        For Each requirement In expected
            Dim satisfied As Boolean
            If requirement Is Nothing OrElse Not values.TryGetValue(requirement.Codigo, satisfied) OrElse satisfied <> requirement.Satisfecho Then Return False
        Next
        Return True
    End Function

    Private Function BuildIntent(ByVal context As ContextoImportacionServicio, ByVal request As CreateImportIntentRequestDto) As IntencionImportacionServicio
        Dim now = _clock.UtcNow()
        Dim intent As New IntencionImportacionServicio With {
            .Id = Guid.NewGuid().ToString("N"), .IdempotencyKey = request.IdempotencyKey.Trim(),
            .ContextoOriginal = New ContextoIntencionImportacion With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId, .IdUsuario = context.IdUsuario, .IdGrupo = context.IdGrupo, .LoginUsuario = context.LoginUsuario, .IdTarea = context.IdTarea, .IdRuta = context.IdRuta, .IdTramite = context.IdTramite, .ProviderId = context.ProviderId.Trim(), .Radicado = request.Radicado.Trim()},
            .Fase = FaseImportacionServicio.Creada, .FechaCreacionUtc = now, .FechaActualizacionUtc = now}
        For Each requirement In request.Requirements
            intent.Requisitos.Add(New RequisitoPlanImportacion With {.Codigo = requirement.Codigo, .Satisfecho = requirement.Satisfecho, .MensajeVisible = requirement.MensajeVisible})
        Next
        For Each item In request.Items
            intent.Resultados.Add(New ResultadoElementoImportacion With {.ClientItemId = item.ClientItemId.Trim(), .IdentidadExterna = New IdentidadExternaImportacion With {.ProviderId = context.ProviderId.Trim(), .ExternalKey = item.ExternalKey.Trim()}, .IdTareaDestino = item.TargetTaskId, .IdTipoDocumental = item.DocumentTypeId, .NombreTipoDocumental = item.DocumentTypeName.Trim(), .NombreArchivo = item.FileName, .TipoContenido = item.ContentType, .Fase = FaseImportacionServicio.Creada})
        Next
        If String.Equals(context.ProviderId, SiiImportProvider.CanonicalProviderId, StringComparison.OrdinalIgnoreCase) Then
            If _inscriptions Is Nothing Then Throw New InvalidOperationException("SII_INSCRIPTION_RESOLVER_UNAVAILABLE")
            intent.Inscripciones = _inscriptions.Resolver(context, request, intent.Resultados)
        End If
        intent.HuellaContexto = Hash(Canonical(intent))
        intent.VersionToken = intent.HuellaContexto
        Return intent
    End Function

    Public Shared Function Canonical(ByVal intent As IntencionImportacionServicio) As String
        Dim parts As New List(Of String) From {Field(intent.ContextoOriginal.OperationId), Field(intent.ContextoOriginal.IdUsuario.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdGrupo.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdTarea.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdRuta.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.IdTramite.ToString(CultureInfo.InvariantCulture)), Field(intent.ContextoOriginal.ProviderId.ToLowerInvariant()), Field(intent.ContextoOriginal.Radicado)}
        Dim requirements As New List(Of String)()
        For Each value In intent.Requisitos
            requirements.Add(Field(value.Codigo) & Field(If(value.Satisfecho, "1", "0")))
        Next
        requirements.Sort(StringComparer.Ordinal)
        Dim items As New List(Of String)()
        For Each value In intent.Resultados
            items.Add(Field(value.IdentidadExterna.ProviderId.ToLowerInvariant()) & Field(value.IdentidadExterna.ExternalKey) & Field(value.IdTareaDestino.ToString(CultureInfo.InvariantCulture)) & Field(If(value.IdTipoDocumental.HasValue, value.IdTipoDocumental.Value.ToString(CultureInfo.InvariantCulture), String.Empty)) & Field(value.NombreTipoDocumental))
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
        If context Is Nothing OrElse request Is Nothing OrElse String.IsNullOrWhiteSpace(request.IdempotencyKey) OrElse String.IsNullOrWhiteSpace(request.Radicado) OrElse request.Items Is Nothing OrElse request.Items.Count = 0 OrElse request.Requirements Is Nothing Then Return False
        For Each item In request.Items
            If item Is Nothing OrElse Not item.DocumentTypeId.HasValue OrElse item.DocumentTypeId.Value <= 0 OrElse String.IsNullOrWhiteSpace(item.DocumentTypeName) OrElse item.DocumentTypeName.Trim().Length > 255 Then Return False
        Next
        Return True
    End Function
    Private Shared Function Fail(ByVal response As CreateImportIntentResponseDto, ByVal code As String, ByVal message As String) As CreateImportIntentResponseDto
        response.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = message, .EsReintentable = code = "INTENT_IN_PROGRESS" OrElse code = "INTENT_UNAVAILABLE"}
        Return response
    End Function
End Class

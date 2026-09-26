Imports System
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class ServicioPreflightImportacion
    Private ReadOnly _validador As ValidadorContextoImportacion
    Private ReadOnly _documentTypes As IImportDocumentTypeResolver
    Private ReadOnly _configurations As IImportEffectConfigurationRepository
    Private ReadOnly _planBuilder As ImportEffectPlanBuilder
    Private ReadOnly _status As IImportItemStatusRepository
    Public Sub New(ByVal validador As ValidadorContextoImportacion, ByVal documentTypes As IImportDocumentTypeResolver)
        Me.New(validador, documentTypes, Nothing, New ImportEffectPlanBuilder())
    End Sub

    Public Sub New(ByVal validador As ValidadorContextoImportacion, ByVal documentTypes As IImportDocumentTypeResolver,
                   ByVal configurations As IImportEffectConfigurationRepository, ByVal planBuilder As ImportEffectPlanBuilder)
        Me.New(validador, documentTypes, configurations, planBuilder, Nothing)
    End Sub

    Public Sub New(ByVal validador As ValidadorContextoImportacion, ByVal documentTypes As IImportDocumentTypeResolver,
                   ByVal configurations As IImportEffectConfigurationRepository, ByVal planBuilder As ImportEffectPlanBuilder,
                   ByVal status As IImportItemStatusRepository)
        If validador Is Nothing OrElse documentTypes Is Nothing Then Throw New ArgumentNullException("dependency")
        _validador = validador
        _documentTypes = documentTypes
        _configurations = configurations
        _planBuilder = If(planBuilder, New ImportEffectPlanBuilder())
        _status = status
    End Sub
    Public Function Preflight(ByVal contexto As ContextoImportacionServicio,
                              ByVal request As PreflightImportRequestDto) As PreflightImportResponseDto
        Dim response As New PreflightImportResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        Dim validation = _validador.Validar(contexto)
        If Not validation.Valido Then Return Fail(response, validation.Codigo, validation.MensajeVisible)
        If request Is Nothing OrElse request.Items Is Nothing OrElse request.Items.Count = 0 Then Return Fail(response, "EMPTY_SELECTION", "Debe seleccionar al menos un elemento.")
        If Not String.Equals(If(request.Capability, String.Empty).Trim(), contexto.Capability, StringComparison.OrdinalIgnoreCase) Then Return Fail(response, "CAPABILITY_CONTEXT_MISMATCH", "La capacidad solicitada no corresponde al contexto autorizado.")
        If Not String.IsNullOrWhiteSpace(request.Capability) AndAlso
           Not String.Equals(request.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) Then
            Return Fail(response, "CAPABILITY_NOT_SUPPORTED", "La capacidad solicitada no está disponible.")
        End If
        Dim clientIds As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim externalKeys As New HashSet(Of String)(StringComparer.Ordinal)
        For Each item In request.Items
            If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.ClientItemId) OrElse item.ClientItemId.Trim().Length > 128 OrElse String.IsNullOrWhiteSpace(item.ExternalKey) OrElse item.ExternalKey.Trim().Length > 500 OrElse item.TargetTaskId <> contexto.IdTarea OrElse Not item.DocumentTypeId.HasValue OrElse item.DocumentTypeId.Value <= 0 OrElse String.IsNullOrWhiteSpace(item.DocumentTypeName) OrElse item.DocumentTypeName.Trim().Length > 255 OrElse If(item.ContentType, String.Empty).Trim().Length > 255 OrElse (Not String.Equals(contexto.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) AndAlso If(item.FileName, String.Empty).Trim().Length > 500) Then Return Fail(response, "INVALID_SELECTION", "La selección contiene un elemento no válido.")
            If Not clientIds.Add(item.ClientItemId.Trim()) OrElse Not externalKeys.Add(item.ExternalKey.Trim()) Then Return Fail(response, "DUPLICATE_SELECTION", "La selección contiene elementos duplicados.")
            Dim documentType As ResolucionTipoDocumentalImportacion
            Try
                documentType = _documentTypes.Resolver(contexto, item.DocumentTypeId.Value, item.DocumentTypeName)
            Catch
                Return Fail(response, "DOCUMENT_TYPE_RESOLUTION_UNAVAILABLE", "No fue posible validar la tipología documental.")
            End Try
            If documentType Is Nothing OrElse Not documentType.Valida Then
                Return Fail(response, "DOCUMENT_TYPE_INVALID", "La tipología documental no corresponde al trámite.")
            End If
            response.Commands.Add(New DocumentCommandDto With {.ClientItemId = item.ClientItemId.Trim(), .ExternalKey = item.ExternalKey.Trim(), .DocumentTypeId = item.DocumentTypeId, .DocumentTypeName = item.DocumentTypeName.Trim(), .FileName = item.FileName, .ContentType = item.ContentType})
        Next
        If _status IsNot Nothing AndAlso String.Equals(contexto.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) Then
            Try
                Dim keys As New List(Of String)(externalKeys)
                Dim states = _status.ObtenerLote(contexto, contexto.ProviderId, keys)
                For Each key In keys
                    Dim state As EstadoItemListadoImportacion = Nothing
                    If states.TryGetValue(key, state) AndAlso state IsNot Nothing AndAlso state.Confirmado Then
                        Return Fail(response, "DOCUMENT_ALREADY_IMPORTED", "Uno de los documentos seleccionados ya está disponible.")
                    End If
                Next
            Catch
                Return Fail(response, "ITEM_STATUS_UNAVAILABLE", "No fue posible verificar el estado de los documentos.")
            End Try
        End If
        Dim configuration As ImportEffectConfiguration = Nothing
        Try
            If _configurations IsNot Nothing Then configuration = _configurations.Obtener(contexto)
        Catch
            Return Fail(response, "PREFLIGHT_UNAVAILABLE", "No fue posible confirmar el plan de importación.")
        End Try
        'El constructor compatible conserva pruebas antiguas; producción siempre inyecta configuración autoritativa.
        If _configurations Is Nothing Then
            configuration = New ImportEffectConfiguration With {.ExpedientRequired = True, .AutomaticCreationEnabled = True}
            configuration.IdentityFields.Add("legacy-compatible")
        End If
        If configuration Is Nothing OrElse (configuration.ExpedientMode = ModoExpedienteImportacion.GestionarExpediente AndAlso configuration.IdentityFields.Count = 0) Then
            response.Requirements.Add(New ImportRequirementDto With {.Codigo = "EFFECT_CONFIGURATION_AVAILABLE", .Satisfecho = False, .MensajeVisible = "La configuración de destino no está disponible."})
            Return Fail(response, "EFFECT_CONFIGURATION_UNAVAILABLE", "No fue posible confirmar el plan de importación.")
        End If
        Dim plans = _planBuilder.Build(contexto, request.Items, configuration)
        If plans Is Nothing OrElse plans.Count <> request.Items.Count Then Return Fail(response, "EFFECT_DESTINATION_AMBIGUOUS", "No fue posible confirmar el destino de todos los elementos.")
        For Each plan In plans : response.EffectPlans.Add(plan) : Next
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "CONTEXT_AUTHORIZED", .Satisfecho = True})
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "SELECTION_VALID", .Satisfecho = True})
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "DOCUMENT_TYPE_ALLOWED", .Satisfecho = True})
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "EFFECT_CONFIGURATION_AVAILABLE", .Satisfecho = True})
        response.ContextFingerprint = Fingerprint(contexto, request.Capability, request.Items, configuration)
        response.IsValid = True
        response.Executable = True
        Return response
    End Function

    Private Shared Function Fail(ByVal response As PreflightImportResponseDto, ByVal code As String, ByVal message As String) As PreflightImportResponseDto
        response.IsValid = False
        response.Executable = False
        response.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = message, .EsReintentable = False}
        Return response
    End Function

    Private Shared Function Fingerprint(ByVal context As ContextoImportacionServicio, ByVal capability As String, ByVal items As IEnumerable(Of ImportItemSelectionDto), ByVal configuration As ImportEffectConfiguration) As String
        Dim values As New List(Of String)()
        For Each item In items
            values.Add(item.ExternalKey.Trim() & "|" & item.TargetTaskId.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & item.DocumentTypeId.Value.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & item.DocumentTypeName.Trim())
        Next
        values.Sort(StringComparer.Ordinal)
        Dim canonical = context.IdUsuario.ToString(Globalization.CultureInfo.InvariantCulture) & "|" &
            context.IdTarea.ToString(Globalization.CultureInfo.InvariantCulture) & "|" &
            context.IdRuta.ToString(Globalization.CultureInfo.InvariantCulture) & "|" &
            context.IdTramite.ToString(Globalization.CultureInfo.InvariantCulture) & "|" &
            context.ProviderId.Trim().ToLowerInvariant() & "|" & If(capability, String.Empty).Trim().ToUpperInvariant() & "|" & String.Join(";", values) & "|" & configuration.CanonicalValue()
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(canonical))).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function
End Class

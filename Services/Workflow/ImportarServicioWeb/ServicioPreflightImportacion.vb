Imports System
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class ServicioPreflightImportacion
    Private ReadOnly _validador As ValidadorContextoImportacion
    Private ReadOnly _documentTypes As IImportDocumentTypeResolver
    Public Sub New(ByVal validador As ValidadorContextoImportacion, ByVal documentTypes As IImportDocumentTypeResolver)
        If validador Is Nothing OrElse documentTypes Is Nothing Then Throw New ArgumentNullException("dependency")
        _validador = validador
        _documentTypes = documentTypes
    End Sub

    Public Function Preflight(ByVal contexto As ContextoImportacionServicio,
                              ByVal request As PreflightImportRequestDto) As PreflightImportResponseDto
        Dim response As New PreflightImportResponseDto With {.OperationId = If(request Is Nothing, Nothing, request.OperationId), .CorrelationId = If(request Is Nothing, Nothing, request.CorrelationId)}
        Dim validation = _validador.Validar(contexto)
        If Not validation.Valido Then Return Fail(response, validation.Codigo, validation.MensajeVisible)
        If request Is Nothing OrElse request.Items Is Nothing OrElse request.Items.Count = 0 Then Return Fail(response, "EMPTY_SELECTION", "Debe seleccionar al menos un elemento.")
        Dim clientIds As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim externalKeys As New HashSet(Of String)(StringComparer.Ordinal)
        For Each item In request.Items
            If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.ClientItemId) OrElse String.IsNullOrWhiteSpace(item.ExternalKey) OrElse item.TargetTaskId <> contexto.IdTarea OrElse Not item.DocumentTypeId.HasValue OrElse item.DocumentTypeId.Value <= 0 OrElse String.IsNullOrWhiteSpace(item.DocumentTypeName) OrElse item.DocumentTypeName.Trim().Length > 255 Then Return Fail(response, "INVALID_SELECTION", "La selección contiene un elemento no válido.")
            If Not clientIds.Add(item.ClientItemId.Trim()) OrElse Not externalKeys.Add(item.ExternalKey.Trim()) Then Return Fail(response, "DUPLICATE_SELECTION", "La selección contiene elementos duplicados.")
            Dim documentType As ResolucionTipoDocumentalImportacion
            Try
                documentType = _documentTypes.Resolver(contexto, item.DocumentTypeId.Value, item.DocumentTypeName)
            Catch
                Return Fail(response, "DOCUMENT_TYPE_RESOLUTION_UNAVAILABLE", "No fue posible validar la tipología documental.")
            End Try
            If documentType Is Nothing OrElse Not documentType.Valida Then
                Return Fail(response, If(documentType Is Nothing OrElse String.IsNullOrWhiteSpace(documentType.Codigo), "DOCUMENT_TYPE_RESOLUTION_FAILED", documentType.Codigo), "La tipología documental no corresponde al trámite.")
            End If
            response.Commands.Add(New DocumentCommandDto With {.ClientItemId = item.ClientItemId.Trim(), .ExternalKey = item.ExternalKey.Trim(), .DocumentTypeId = item.DocumentTypeId, .DocumentTypeName = item.DocumentTypeName.Trim(), .FileName = item.FileName, .ContentType = item.ContentType})
        Next
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "CONTEXT_AUTHORIZED", .Satisfecho = True})
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "SELECTION_VALID", .Satisfecho = True})
        response.Requirements.Add(New ImportRequirementDto With {.Codigo = "DOCUMENT_TYPE_ALLOWED", .Satisfecho = True})
        response.ContextFingerprint = Fingerprint(contexto, request.Items)
        response.IsValid = True
        Return response
    End Function

    Private Shared Function Fail(ByVal response As PreflightImportResponseDto, ByVal code As String, ByVal message As String) As PreflightImportResponseDto
        response.IsValid = False
        response.Error = New ErrorImportacionServicioDto With {.Codigo = code, .MensajeVisible = message, .EsReintentable = False}
        Return response
    End Function

    Private Shared Function Fingerprint(ByVal context As ContextoImportacionServicio, ByVal items As IEnumerable(Of ImportItemSelectionDto)) As String
        Dim values As New List(Of String)()
        For Each item In items
            values.Add(item.ExternalKey.Trim() & "|" & item.TargetTaskId.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & item.DocumentTypeId.Value.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & item.DocumentTypeName.Trim())
        Next
        values.Sort(StringComparer.Ordinal)
        Dim canonical = context.IdUsuario.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & context.IdTarea.ToString(Globalization.CultureInfo.InvariantCulture) & "|" & context.ProviderId.Trim().ToLowerInvariant() & "|" & String.Join(";", values)
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(canonical))).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function
End Class

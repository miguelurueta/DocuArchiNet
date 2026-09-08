Imports System
Imports System.Collections.Generic

' Media metadatos de preview; nunca devuelve contenido, rutas, tokens ni detalles de excepción.
Public NotInheritable Class SiiPreviewResponseFactory
    Private ReadOnly _allowedContentTypes As ISet(Of String)
    Private ReadOnly _maximumBytes As Long

    Public Sub New(ByVal allowedContentTypes As IEnumerable(Of String), ByVal maximumBytes As Long)
        If allowedContentTypes Is Nothing Then Throw New ArgumentNullException("allowedContentTypes")
        If maximumBytes <= 0 Then Throw New ArgumentOutOfRangeException("maximumBytes")
        _allowedContentTypes = New HashSet(Of String)(allowedContentTypes, StringComparer.OrdinalIgnoreCase)
        If _allowedContentTypes.Count = 0 Then Throw New ArgumentException("Debe existir al menos un tipo permitido.", "allowedContentTypes")
        _maximumBytes = maximumBytes
    End Sub

    Public Function Create(ByVal source As GetPreviewResponseDto,
                           ByVal request As GetPreviewRequestDto,
                           ByVal context As ContextoImportacionServicio,
                           ByVal utcNow As DateTime) As GetPreviewResponseDto
        Dim validationError As String = Validate(source, request, context, utcNow)
        If validationError.Length > 0 Then Return Failure(request, validationError)

        Return New GetPreviewResponseDto With {
            .OperationId = request.OperationId,
            .CorrelationId = request.CorrelationId,
            .ExternalKey = request.ExternalKey.Trim(),
            .DescriptorId = SafeToken(source.DescriptorId),
            .ContentType = source.ContentType.Trim().ToLowerInvariant(),
            .Length = source.Length,
            .Disposition = "inline",
            .ExpiresAtUtc = source.ExpiresAtUtc
        }
    End Function

    Private Function Validate(ByVal source As GetPreviewResponseDto,
                              ByVal request As GetPreviewRequestDto,
                              ByVal context As ContextoImportacionServicio,
                              ByVal utcNow As DateTime) As String
        If request Is Nothing OrElse context Is Nothing OrElse source Is Nothing Then Return "PREVIEW_INVALID"
        If request.TaskId <= 0 OrElse request.TaskId <> context.IdTarea OrElse
           Not String.Equals(request.ProviderId, context.ProviderId, StringComparison.OrdinalIgnoreCase) OrElse
           Not String.Equals(request.ProviderId, SiiImportProvider.CanonicalProviderId, StringComparison.OrdinalIgnoreCase) OrElse
           Not String.Equals(request.ExternalKey, source.ExternalKey, StringComparison.Ordinal) Then Return "PREVIEW_FORBIDDEN"
        If Not source.ExpiresAtUtc.HasValue OrElse source.ExpiresAtUtc.Value.ToUniversalTime() <= utcNow.ToUniversalTime() Then Return "PREVIEW_EXPIRED"
        If String.IsNullOrWhiteSpace(source.ContentType) OrElse Not _allowedContentTypes.Contains(source.ContentType.Trim()) Then Return "PREVIEW_CONTENT_TYPE_INVALID"
        If Not source.Length.HasValue OrElse source.Length.Value <= 0 OrElse source.Length.Value > _maximumBytes Then Return "PREVIEW_SIZE_INVALID"
        Dim disposition As String = If(source.Disposition, String.Empty).Trim()
        If disposition.Length > 0 AndAlso Not String.Equals(disposition, "inline", StringComparison.OrdinalIgnoreCase) AndAlso
           Not String.Equals(disposition, "attachment", StringComparison.OrdinalIgnoreCase) Then Return "PREVIEW_DISPOSITION_INVALID"
        If SafeToken(source.DescriptorId).Length = 0 Then Return "PREVIEW_INVALID"
        Return String.Empty
    End Function

    Private Shared Function SafeToken(ByVal value As String) As String
        Dim candidate As String = If(value, String.Empty).Trim()
        If candidate.Length = 0 OrElse candidate.Length > 128 OrElse
           Not Text.RegularExpressions.Regex.IsMatch(candidate, "^[A-Za-z0-9._-]+$") Then Return String.Empty
        Return candidate
    End Function

    Private Shared Function Failure(ByVal request As GetPreviewRequestDto, ByVal code As String) As GetPreviewResponseDto
        Return New GetPreviewResponseDto With {
            .OperationId = If(request Is Nothing, String.Empty, request.OperationId),
            .CorrelationId = If(request Is Nothing, String.Empty, request.CorrelationId),
            .Error = New ErrorImportacionServicioDto With {
                .Codigo = code,
                .MensajeVisible = "La previsualización no está disponible."
            }
        }
    End Function
End Class

Imports System
Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

' Cliente de transporte SII. No interpreta contratos ni expone credenciales.
Public NotInheritable Class SiiExternalImportProviderClient
    Private ReadOnly _transport As ExternalImportHttpTransport
    Private ReadOnly _baseUri As Uri
    Private ReadOnly _authorizationHeader As String
    Private ReadOnly _timeout As TimeSpan
    Private ReadOnly _maximumResponseBytes As Long

    Public Sub New(ByVal transport As ExternalImportHttpTransport, ByVal baseUri As Uri,
                   ByVal authorizationHeader As String, ByVal timeout As TimeSpan,
                   ByVal maximumResponseBytes As Long)
        If transport Is Nothing Then Throw New ArgumentNullException("transport")
        If baseUri Is Nothing OrElse Not baseUri.IsAbsoluteUri Then Throw New ArgumentException("Se requiere URI SII absoluta.", "baseUri")
        If timeout <= TimeSpan.Zero Then Throw New ArgumentOutOfRangeException("timeout")
        If maximumResponseBytes <= 0 Then Throw New ArgumentOutOfRangeException("maximumResponseBytes")
        _transport = transport
        _baseUri = baseUri
        _authorizationHeader = If(authorizationHeader, String.Empty).Trim()
        _timeout = timeout
        _maximumResponseBytes = maximumResponseBytes
    End Sub

    Public Function QueryItemsAsync(ByVal request As QueryItemsRequestDto,
                                    ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If request Is Nothing Then Throw New ArgumentNullException("request")
        Dim relative = "items?taskId=" & request.TaskId.ToString(Globalization.CultureInfo.InvariantCulture)
        If request.PageSize.HasValue Then relative &= "&pageSize=" & request.PageSize.Value.ToString(Globalization.CultureInfo.InvariantCulture)
        If Not String.IsNullOrWhiteSpace(request.ContinuationToken) Then relative &= "&continuationToken=" & Uri.EscapeDataString(request.ContinuationToken)
        Return SendAsync(relative, request.CorrelationId, "application/json", cancellationToken)
    End Function

    Public Function GetPreviewMetadataAsync(ByVal request As GetPreviewRequestDto,
                                            ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If request Is Nothing OrElse String.IsNullOrWhiteSpace(request.ExternalKey) Then Throw New ArgumentException("ExternalKey es obligatorio.", "request")
        Return SendAsync("preview/" & Uri.EscapeDataString(request.ExternalKey), request.CorrelationId, "application/json", cancellationToken)
    End Function

    Public Function DownloadAsync(ByVal externalKey As String, ByVal correlationId As String,
                                  ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If String.IsNullOrWhiteSpace(externalKey) Then Throw New ArgumentException("ExternalKey es obligatorio.", "externalKey")
        Return SendAsync("resource/" & Uri.EscapeDataString(externalKey), correlationId, "application/pdf", cancellationToken)
    End Function

    Private Function SendAsync(ByVal relative As String, ByVal correlationId As String,
                               ByVal mediaType As String, ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        Dim headers As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If _authorizationHeader.Length > 0 Then headers.Add("Authorization", _authorizationHeader)
        headers.Add("X-Correlation-Id", correlationId)
        Dim prepared = New ExternalImportHttpRequest(New Uri(_baseUri, relative), HttpMethod.Get, Nothing, Nothing,
            headers, _timeout, _maximumResponseBytes, New String() {mediaType}, correlationId)
        Return _transport.SendAsync(prepared, cancellationToken)
    End Function
End Class

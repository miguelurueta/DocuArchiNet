Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports System.Threading.Tasks

Public NotInheritable Class ExternalImportHttpRequest
    Private ReadOnly _headers As IReadOnlyDictionary(Of String, String)
    Private ReadOnly _allowedMediaTypes As ISet(Of String)
    Private ReadOnly _body As Byte()

    Public Sub New(ByVal requestUri As Uri, ByVal method As HttpMethod, ByVal body As Byte(),
                   ByVal contentType As String, ByVal headers As IDictionary(Of String, String),
                   ByVal timeout As TimeSpan, ByVal maximumResponseBytes As Long,
                   ByVal allowedMediaTypes As IEnumerable(Of String), ByVal correlationId As String)
        If requestUri Is Nothing OrElse Not requestUri.IsAbsoluteUri Then Throw New ArgumentException("Se requiere una URI absoluta.", "requestUri")
        If method Is Nothing Then Throw New ArgumentNullException("method")
        If timeout <= TimeSpan.Zero Then Throw New ArgumentOutOfRangeException("timeout")
        If maximumResponseBytes <= 0 Then Throw New ArgumentOutOfRangeException("maximumResponseBytes")
        If String.IsNullOrWhiteSpace(correlationId) Then Throw New ArgumentException("correlationId es obligatorio.", "correlationId")

        Me.RequestUri = requestUri
        Me.Method = method
        _body = If(body Is Nothing, New Byte() {}, CType(body.Clone(), Byte()))
        Me.ContentType = If(contentType, String.Empty).Trim()
        Me.Timeout = timeout
        Me.MaximumResponseBytes = maximumResponseBytes
        Me.CorrelationId = correlationId.Trim()

        Dim headerCopy As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If headers IsNot Nothing Then
            For Each item In headers
                headerCopy.Add(item.Key, item.Value)
            Next
        End If
        _headers = New ReadOnlyDictionary(Of String, String)(headerCopy)

        Dim mediaTypeCopy As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If allowedMediaTypes IsNot Nothing Then
            For Each item In allowedMediaTypes
                If Not String.IsNullOrWhiteSpace(item) Then mediaTypeCopy.Add(item.Trim())
            Next
        End If
        If mediaTypeCopy.Count = 0 Then Throw New ArgumentException("Debe existir al menos un media type permitido.", "allowedMediaTypes")
        _allowedMediaTypes = mediaTypeCopy
    End Sub

    Public ReadOnly Property RequestUri As Uri
    Public ReadOnly Property Method As HttpMethod
    Public ReadOnly Property ContentType As String
    Public ReadOnly Property Timeout As TimeSpan
    Public ReadOnly Property MaximumResponseBytes As Long
    Public ReadOnly Property CorrelationId As String
    Public ReadOnly Property Headers As IReadOnlyDictionary(Of String, String)
        Get
            Return _headers
        End Get
    End Property
    Public ReadOnly Property AllowedMediaTypes As ISet(Of String)
        Get
            Return New HashSet(Of String)(_allowedMediaTypes, StringComparer.OrdinalIgnoreCase)
        End Get
    End Property
    Public Function GetBody() As Byte()
        Return CType(_body.Clone(), Byte())
    End Function
End Class

Public NotInheritable Class ExternalImportHttpTransport
    Private ReadOnly _factory As IExternalImportHttpClientFactory
    Private ReadOnly _validator As ExternalImportHttpResponseValidator
    Private ReadOnly _mapper As ExternalImportHttpErrorMapper

    Public Sub New(ByVal factory As IExternalImportHttpClientFactory,
                   ByVal validator As ExternalImportHttpResponseValidator,
                   ByVal mapper As ExternalImportHttpErrorMapper)
        If factory Is Nothing Then Throw New ArgumentNullException("factory")
        If validator Is Nothing Then Throw New ArgumentNullException("validator")
        If mapper Is Nothing Then Throw New ArgumentNullException("mapper")
        _factory = factory
        _validator = validator
        _mapper = mapper
    End Sub

    Public Async Function SendAsync(ByVal prepared As ExternalImportHttpRequest,
                                    ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If prepared Is Nothing Then Throw New ArgumentNullException("prepared")
        Using timeoutSource As New CancellationTokenSource(prepared.Timeout)
            Using linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token)
                Try
                    Using request As New HttpRequestMessage(prepared.Method, prepared.RequestUri)
                        Dim body = prepared.GetBody()
                        If body.Length > 0 OrElse Not String.IsNullOrWhiteSpace(prepared.ContentType) Then
                            request.Content = New ByteArrayContent(body)
                            If Not String.IsNullOrWhiteSpace(prepared.ContentType) Then
                                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(prepared.ContentType)
                            End If
                        End If
                        For Each header In prepared.Headers
                            If Not request.Headers.TryAddWithoutValidation(header.Key, header.Value) Then
                                If request.Content Is Nothing OrElse Not request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value) Then
                                    Throw New ExternalImportHttpException(_mapper.InvalidResponse(prepared.CorrelationId))
                                End If
                            End If
                        Next
                        Using response = Await _factory.GetClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedSource.Token).ConfigureAwait(False)
                            Return Await _validator.ReadValidatedAsync(response, prepared.AllowedMediaTypes,
                                prepared.MaximumResponseBytes, prepared.CorrelationId, linkedSource.Token).ConfigureAwait(False)
                        End Using
                    End Using
                Catch ex As ExternalImportHttpException
                    Throw
                Catch ex As Exception
                    Throw New ExternalImportHttpException(_mapper.FromTransport(ex, cancellationToken.IsCancellationRequested,
                                                                                prepared.CorrelationId), ex)
                End Try
            End Using
        End Using
    End Function
End Class

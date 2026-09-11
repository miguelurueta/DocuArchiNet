Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Net.Http
Imports System.Diagnostics
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Token y consultarInformacionSello se ejecutan en servidor; nunca se acepta una URL del navegador.
Public NotInheritable Class SiiExternalImportProviderClient
    Private ReadOnly _transport As ExternalImportHttpTransport
    Private ReadOnly _baseUri As Uri
    Private ReadOnly _companyCode, _serviceUser, _servicePassword As String
    Private ReadOnly _timeout As TimeSpan
    Private ReadOnly _maximumResponseBytes As Long
    Private ReadOnly _allowedDownloadHosts As ISet(Of String)
    Private ReadOnly _attempts As IExternalServiceAttemptRecorder
    Private ReadOnly _resolvedMetadata As New ConcurrentDictionary(Of String, MetadatosDocumentoSii)(StringComparer.Ordinal)

    Public Sub New(ByVal transport As ExternalImportHttpTransport, ByVal baseUri As Uri,
                   ByVal companyCode As String, ByVal serviceUser As String, ByVal servicePassword As String,
                   ByVal timeout As TimeSpan, ByVal maximumResponseBytes As Long, ByVal allowedDownloadHosts As IEnumerable(Of String))
        Me.New(transport, baseUri, companyCode, serviceUser, servicePassword, timeout, maximumResponseBytes, allowedDownloadHosts, New NullExternalServiceAttemptRecorder())
    End Sub

    Public Sub New(ByVal transport As ExternalImportHttpTransport, ByVal baseUri As Uri,
                   ByVal companyCode As String, ByVal serviceUser As String, ByVal servicePassword As String,
                   ByVal timeout As TimeSpan, ByVal maximumResponseBytes As Long, ByVal allowedDownloadHosts As IEnumerable(Of String),
                   ByVal attempts As IExternalServiceAttemptRecorder)
        If transport Is Nothing Then Throw New ArgumentNullException("transport")
        If baseUri Is Nothing OrElse Not baseUri.IsAbsoluteUri OrElse
           (baseUri.Scheme <> Uri.UriSchemeHttp AndAlso baseUri.Scheme <> Uri.UriSchemeHttps) Then
            Throw New ArgumentException("Se requiere URI SII HTTP o HTTPS absoluta.", "baseUri")
        End If
        If String.IsNullOrWhiteSpace(companyCode) OrElse String.IsNullOrWhiteSpace(serviceUser) OrElse String.IsNullOrWhiteSpace(servicePassword) Then Throw New ArgumentException("Configuración SII incompleta.")
        If companyCode.Trim().Length > 2 OrElse serviceUser.Trim().Length > 20 OrElse servicePassword.Length > 64 Then Throw New ArgumentException("SII_CREDENTIAL_FIELDS_INVALID_LENGTH")
        If attempts Is Nothing Then Throw New ArgumentNullException("attempts")
        _transport = transport : _baseUri = baseUri : _companyCode = companyCode.Trim() : _serviceUser = serviceUser.Trim() : _servicePassword = servicePassword : _attempts = attempts
        _timeout = timeout : _maximumResponseBytes = maximumResponseBytes
        _allowedDownloadHosts = New HashSet(Of String)(If(allowedDownloadHosts, New String() {}), StringComparer.OrdinalIgnoreCase)
        If _allowedDownloadHosts.Count = 0 Then _allowedDownloadHosts.Add(baseUri.Host)
    End Sub

    Public Function QueryItemsAsync(ByVal request As QueryItemsRequestDto, ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If request Is Nothing OrElse String.IsNullOrWhiteSpace(request.CodigoBarras) Then Throw New ArgumentException("Código de barras obligatorio.", "request")
        If request.CodigoBarras.Trim().Length > 15 Then Throw New ArgumentException("SII_BARCODE_INVALID", "request")
        Return QuerySealAsync(request.CodigoBarras.Trim(), request.CorrelationId, cancellationToken, Nothing, Nothing,
            request.OperationId, request.TaskId, Nothing, request.CodigoBarras.Trim(), request.CodigoBarras.Trim())
    End Function

    Public Async Function GetPreviewMetadataAsync(ByVal request As GetPreviewRequestDto, ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        Dim selected = Await ResolveImageAsync(request.ExternalKey, request.CorrelationId, cancellationToken, Nothing, Nothing,
            request.OperationId, request.TaskId, Nothing, request.ExternalKey).ConfigureAwait(False)
        Dim content = Await DownloadSelectedAsync(selected, request.CorrelationId, cancellationToken, Nothing, Nothing,
            request.OperationId, request.TaskId, Nothing, selected.CodigoBarras, request.ExternalKey).ConfigureAwait(False)
        Dim result As New JObject From {{"descriptorId", request.ExternalKey}, {"contentType", selected.ContentType},
            {"length", content.LongLength}, {"disposition", "inline"}, {"expiresAtUtc", DateTime.UtcNow.AddMinutes(5)}}
        Return Encoding.UTF8.GetBytes(result.ToString(Formatting.None))
    End Function

    Public Async Function DownloadAsync(ByVal externalKey As String, ByVal correlationId As String, ByVal cancellationToken As CancellationToken,
                                        Optional ByVal intentId As String = Nothing, Optional ByVal clientItemId As String = Nothing,
                                        Optional ByVal operationId As String = Nothing, Optional ByVal taskId As Nullable(Of Long) = Nothing,
                                        Optional ByVal radicado As String = Nothing, Optional ByVal referenciaProveedor As String = Nothing) As Task(Of Byte())
        Dim selected = Await ResolveImageAsync(externalKey, correlationId, cancellationToken, intentId, clientItemId,
            operationId, taskId, radicado, referenciaProveedor).ConfigureAwait(False)
        _resolvedMetadata(CacheKey(externalKey, correlationId)) = selected.Metadata
        Return Await DownloadSelectedAsync(selected, correlationId, cancellationToken, intentId, clientItemId, operationId,
            taskId, radicado, selected.CodigoBarras, referenciaProveedor).ConfigureAwait(False)
    End Function

    Private Function DownloadSelectedAsync(ByVal selected As ResolvedImage, ByVal correlationId As String,
                                           ByVal cancellationToken As CancellationToken, Optional ByVal intentId As String = Nothing,
                                           Optional ByVal clientItemId As String = Nothing, Optional ByVal operationId As String = Nothing,
                                           Optional ByVal taskId As Nullable(Of Long) = Nothing, Optional ByVal radicado As String = Nothing,
                                           Optional ByVal codigoBarras As String = Nothing, Optional ByVal referenciaProveedor As String = Nothing) As Task(Of Byte())
        Dim uri As Uri = Nothing
        If Not Uri.TryCreate(selected.Url, UriKind.Absolute, uri) Then Throw New InvalidOperationException("SII_RESOURCE_URL_INVALID")
        If uri.Scheme <> Uri.UriSchemeHttp AndAlso uri.Scheme <> Uri.UriSchemeHttps Then Throw New InvalidOperationException("SII_RESOURCE_SCHEME_NOT_ALLOWED")
        If Not IsAllowedDownloadHost(uri.Host) Then
            Dim diagnosticHost = Text.RegularExpressions.Regex.Replace(uri.DnsSafeHost.ToUpperInvariant(), "[^A-Z0-9]", "_")
            If diagnosticHost.Length > 40 Then diagnosticHost = diagnosticHost.Substring(0, 40)
            Throw New InvalidOperationException("SII_RESOURCE_HOST_NOT_ALLOWED_" & diagnosticHost)
        End If
        Return ObserveAsync("DESCARGAR_ANEXO", correlationId, Function() SendAsync(uri, HttpMethod.Get, Nothing, Nothing, correlationId, New String() {selected.ContentType}, cancellationToken),
            intentId, clientItemId, operationId, taskId, radicado, codigoBarras, referenciaProveedor)
    End Function

    Private Function IsAllowedDownloadHost(ByVal host As String) As Boolean
        Dim normalizedHost = If(host, String.Empty).Trim().TrimEnd("."c)
        For Each allowed In _allowedDownloadHosts
            Dim normalizedAllowed = If(allowed, String.Empty).Trim().Trim("."c)
            If normalizedAllowed.Length > 0 AndAlso
               (String.Equals(normalizedHost, normalizedAllowed, StringComparison.OrdinalIgnoreCase) OrElse
                normalizedHost.EndsWith("." & normalizedAllowed, StringComparison.OrdinalIgnoreCase)) Then Return True
        Next
        Return False
    End Function

    Public Async Function ResolveStorageMetadataAsync(ByVal externalKey As String, ByVal correlationId As String, ByVal cancellationToken As CancellationToken,
                                                      Optional ByVal intentId As String = Nothing, Optional ByVal clientItemId As String = Nothing,
                                                      Optional ByVal operationId As String = Nothing, Optional ByVal taskId As Nullable(Of Long) = Nothing,
                                                      Optional ByVal radicado As String = Nothing, Optional ByVal referenciaProveedor As String = Nothing) As Task(Of MetadatosDocumentoSii)
        Dim cached As MetadatosDocumentoSii = Nothing
        If _resolvedMetadata.TryRemove(CacheKey(externalKey, correlationId), cached) Then Return cached
        Dim selected = Await ResolveImageAsync(externalKey, correlationId, cancellationToken, intentId, clientItemId,
            operationId, taskId, radicado, referenciaProveedor).ConfigureAwait(False)
        Return selected.Metadata
    End Function

    Private Async Function QuerySealAsync(ByVal codigoBarras As String, ByVal correlationId As String, ByVal cancellationToken As CancellationToken,
                                          Optional ByVal intentId As String = Nothing, Optional ByVal clientItemId As String = Nothing,
                                          Optional ByVal operationId As String = Nothing, Optional ByVal taskId As Nullable(Of Long) = Nothing,
                                          Optional ByVal radicado As String = Nothing, Optional ByVal businessBarcode As String = Nothing,
                                          Optional ByVal referenciaProveedor As String = Nothing) As Task(Of Byte())
        Dim token = Await ObserveAsync("SOLICITAR_TOKEN", correlationId,
            Async Function()
                Dim tokenBytes = Await PostAsync(Endpoint("solicitarToken"), New Dictionary(Of String, String) From {{"codigoempresa", _companyCode}, {"usuariows", _serviceUser}, {"clavews", _servicePassword}}, correlationId, cancellationToken).ConfigureAwait(False)
                Dim tokenJson = SiiImportContractMapper.NormalizeSource(Encoding.UTF8.GetString(tokenBytes))
                Dim value = SiiImportContractMapper.Value(tokenJson, "token"), resultCode = SiiImportContractMapper.Value(tokenJson, "codigoerror")
                If resultCode = "9999" Then Throw New InvalidOperationException("SII_TOKEN_INVALID_CREDENTIALS")
                If value.Length = 0 OrElse value.Length > 256 OrElse SiiImportContractMapper.Value(tokenJson, "mensajeerror").Length > 0 OrElse (resultCode.Length > 0 AndAlso resultCode <> "0000") Then Throw New InvalidOperationException("SII_TOKEN_REJECTED")
                Return value
            End Function, intentId, clientItemId, operationId, taskId, radicado, businessBarcode, referenciaProveedor).ConfigureAwait(False)
        Return Await ObserveAsync("CONSULTAR_SELLO", correlationId,
            Async Function()
                Dim bytes = Await PostAsync(Endpoint("consultarInformacionSello"), New Dictionary(Of String, String) From {{"codigoempresa", _companyCode}, {"usuariows", _serviceUser}, {"token", token}, {"radicado", codigoBarras}}, correlationId, cancellationToken).ConfigureAwait(False)
                Dim source = SiiImportContractMapper.NormalizeSource(Encoding.UTF8.GetString(bytes)), resultCode = SiiImportContractMapper.Value(source, "codigoerror")
                If resultCode = "9998" Then Throw New InvalidOperationException("SII_TOKEN_INVALID")
                If resultCode = "9999" Then Throw New InvalidOperationException("SII_QUERY_PROVIDER_ERROR")
                If resultCode.Length > 0 AndAlso resultCode <> "0000" Then Throw New InvalidOperationException("SII_QUERY_REJECTED")
                Return bytes
            End Function, intentId, clientItemId, operationId, taskId, radicado, businessBarcode, referenciaProveedor).ConfigureAwait(False)
    End Function

    Private Function Endpoint(ByVal methodName As String) As Uri
        Return New Uri(_baseUri.AbsoluteUri.TrimEnd("/"c) & "/" & methodName, UriKind.Absolute)
    End Function

    Private Async Function ResolveImageAsync(ByVal externalKey As String, ByVal correlationId As String, ByVal cancellationToken As CancellationToken,
                                             Optional ByVal intentId As String = Nothing, Optional ByVal clientItemId As String = Nothing,
                                             Optional ByVal operationId As String = Nothing, Optional ByVal taskId As Nullable(Of Long) = Nothing,
                                             Optional ByVal radicado As String = Nothing, Optional ByVal referenciaProveedor As String = Nothing) As Task(Of ResolvedImage)
        Dim codigoBarras As String = Nothing, book As String = Nothing, registration As String = Nothing, attachmentId As String = Nothing
        If Not SiiImportContractMapper.TryParseExternalKey(externalKey, codigoBarras, book, registration, attachmentId) Then Throw New ArgumentException("ExternalKey SII inválido.")
        Dim source = SiiImportContractMapper.NormalizeSource(Encoding.UTF8.GetString(Await QuerySealAsync(codigoBarras, correlationId, cancellationToken,
            intentId, clientItemId, operationId, taskId, radicado, codigoBarras, referenciaProveedor).ConfigureAwait(False)))
        Dim resultCode = SiiImportContractMapper.Value(source, "codigoerror")
        If resultCode = "9998" Then Throw New InvalidOperationException("SII_TOKEN_INVALID")
        If resultCode = "9999" Then Throw New InvalidOperationException("SII_QUERY_PROVIDER_ERROR")
        Dim inscriptions = TryCast(SiiImportContractMapper.Token(source, "inscripciones"), JArray)
        If inscriptions IsNot Nothing Then
            For Each inscription As JObject In inscriptions
                If Value(inscription, "libro") = book AndAlso Value(inscription, "registro") = registration Then
                    Dim images = TryCast(SiiImportContractMapper.Token(inscription, "imagenes"), JArray)
                    If images Is Nothing Then Continue For
                    For Each image As JObject In images
                        If Value(image, "idanexo") = attachmentId Then
                            Dim format = Value(image, "formato").ToLowerInvariant()
                            Return New ResolvedImage With {.Url = Value(image, "url"), .ContentType = SiiImportContractMapper.ContentTypeForFormat(format), .CodigoBarras = codigoBarras,
                                .Metadata = New MetadatosDocumentoSii With {.Libro = book, .Registro = registration, .Fecha = Value(inscription, "fecha"),
                                .Duplicado = Value(inscription, "dupli"), .Hora = Value(inscription, "hora"), .UsuarioSii = Value(inscription, "usuariosii"),
                                .Acto = Value(inscription, "acto"), .NombreActo = Value(inscription, "nacto"), .Matricula = Value(inscription, "matricula"),
                                .Proponente = Value(inscription, "proponente"), .RazonSocial = If(Value(image, "nombre").Length > 0, Value(image, "nombre"), Value(inscription, "nombre")),
                                .NitCedula = If(Value(image, "identificacion").Length > 0, Value(image, "identificacion"), Value(inscription, "identificacion")), .IdAnexo = attachmentId,
                                .TipoImagen = Value(image, "tipo"), .TipoAnexo = Value(image, "tipoanexo"), .TipoSirep = Value(image, "tiposirep"),
                                .TipoDigitalizacion = Value(image, "tipodigitalizacion"), .IdentificadorImagen = Value(image, "identificador"), .Formato = format,
                                .FechaDocumento = Value(image, "fechadocumento"), .Origen = Value(image, "origen"), .Observaciones = Value(image, "observaciones")}}
                        End If
                    Next
                End If
            Next
        End If
        Throw New InvalidOperationException("SII_ITEM_NOT_FOUND")
    End Function

    Private Function PostAsync(ByVal uri As Uri, ByVal values As IDictionary(Of String, String), ByVal correlationId As String, ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        Return SendAsync(uri, HttpMethod.Post, Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(values)), "application/json", correlationId, New String() {"application/json", "text/json"}, cancellationToken)
    End Function
    Private Function SendAsync(ByVal uri As Uri, ByVal method As HttpMethod, ByVal body As Byte(), ByVal contentType As String, ByVal correlationId As String, ByVal mediaTypes As IEnumerable(Of String), ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        Dim headers As New Dictionary(Of String, String) From {{"X-Correlation-Id", correlationId}}
        Return _transport.SendAsync(New ExternalImportHttpRequest(uri, method, body, contentType, headers, _timeout, _maximumResponseBytes, mediaTypes, correlationId), cancellationToken)
    End Function
    Private Async Function ObserveAsync(Of T)(ByVal operation As String, ByVal correlationId As String, ByVal action As Func(Of Task(Of T)),
                                              Optional ByVal intentId As String = Nothing, Optional ByVal clientItemId As String = Nothing,
                                              Optional ByVal operationId As String = Nothing, Optional ByVal taskId As Nullable(Of Long) = Nothing,
                                              Optional ByVal radicado As String = Nothing, Optional ByVal codigoBarras As String = Nothing,
                                              Optional ByVal referenciaProveedor As String = Nothing) As Task(Of T)
        Dim started = DateTime.UtcNow, watch = Stopwatch.StartNew(), failure As Exception = Nothing
        Try
            Return Await action().ConfigureAwait(False)
        Catch ex As Exception
            failure = ex
            Throw
        Finally
            watch.Stop()
            Try
                _attempts.Registrar(ExternalServiceAttemptNormalizer.Create("INTEGRACIONSII", operation, correlationId, started, DateTime.UtcNow,
                    watch.ElapsedMilliseconds, failure, intentId, clientItemId, operationId, taskId, radicado, codigoBarras, referenciaProveedor))
            Catch
                ' La telemetria nunca cambia ni duplica el resultado funcional de SII.
            End Try
        End Try
    End Function
    Private Shared Function CacheKey(ByVal externalKey As String, ByVal correlationId As String) As String
        Return If(correlationId, String.Empty) & "|" & If(externalKey, String.Empty)
    End Function
    Private Shared Function Value(ByVal source As JObject, ByVal name As String) As String
        Return SiiImportContractMapper.Value(source, name)
    End Function
    Private NotInheritable Class ResolvedImage
        Public Property Url As String
        Public Property ContentType As String
        Public Property CodigoBarras As String
        Public Property Metadata As MetadatosDocumentoSii
    End Class
End Class

Imports System
Imports System.Threading
Imports System.Threading.Tasks

Public NotInheritable Class SiiImportProvider
    Implements IExternalImportProviderClient

    Public Const CanonicalProviderId As String = "INTEGRACIONSII"
    Private ReadOnly _client As SiiExternalImportProviderClient
    Private ReadOnly _mapper As SiiImportContractMapper

    Public Sub New(ByVal client As SiiExternalImportProviderClient, ByVal mapper As SiiImportContractMapper)
        If client Is Nothing Then Throw New ArgumentNullException("client")
        If mapper Is Nothing Then Throw New ArgumentNullException("mapper")
        _client = client
        _mapper = mapper
    End Sub

    Public ReadOnly Property ProviderId As String Implements IExternalImportProviderClient.ProviderId
        Get
            Return CanonicalProviderId
        End Get
    End Property

    Public Function ResolveCapabilitiesAsync(ByVal request As ResolveCapabilitiesRequestDto,
        ByVal cancellationToken As CancellationToken) As Task(Of ResolveCapabilitiesResponseDto) Implements IExternalImportProviderClient.ResolveCapabilitiesAsync
        Dim response As New ResolveCapabilitiesResponseDto With {.OperationId = request.OperationId,
            .CorrelationId = request.CorrelationId, .ProviderId = CanonicalProviderId, .ContextAllowed = True}
        response.Capabilities.Add(New ProviderCapabilityDto With {.Codigo = "QUERY_ITEMS", .Habilitada = True, .TimeoutSeconds = 30})
        response.Capabilities.Add(New ProviderCapabilityDto With {.Codigo = "PREVIEW", .Habilitada = True, .TimeoutSeconds = 30})
        Return Task.FromResult(response)
    End Function

    Public Async Function QueryItemsAsync(ByVal request As QueryItemsRequestDto,
        ByVal cancellationToken As CancellationToken) As Task(Of QueryItemsResponseDto) Implements IExternalImportProviderClient.QueryItemsAsync
        ValidateProvider(request)
        Dim payload = Await _client.QueryItemsAsync(request, cancellationToken).ConfigureAwait(False)
        Return _mapper.MapQuery(payload, request)
    End Function

    Public Async Function GetPreviewAsync(ByVal request As GetPreviewRequestDto,
        ByVal cancellationToken As CancellationToken) As Task(Of GetPreviewResponseDto) Implements IExternalImportProviderClient.GetPreviewAsync
        ValidateProvider(request)
        Dim payload = Await _client.GetPreviewMetadataAsync(request, cancellationToken).ConfigureAwait(False)
        Return _mapper.MapPreview(payload, request)
    End Function

    Public Function DownloadAsync(ByVal externalKey As String, ByVal correlationId As String,
        ByVal cancellationToken As CancellationToken, Optional ByVal intentId As String = Nothing,
        Optional ByVal clientItemId As String = Nothing, Optional ByVal operationId As String = Nothing,
        Optional ByVal taskId As Nullable(Of Long) = Nothing, Optional ByVal radicado As String = Nothing,
        Optional ByVal referenciaProveedor As String = Nothing) As Task(Of Byte()) Implements IExternalImportProviderClient.DownloadAsync
        Return _client.DownloadAsync(externalKey, correlationId, cancellationToken, intentId, clientItemId, operationId, taskId, radicado, referenciaProveedor)
    End Function

    Public Function ResolveStorageMetadataAsync(ByVal externalKey As String, ByVal correlationId As String,
        ByVal cancellationToken As CancellationToken, Optional ByVal intentId As String = Nothing,
        Optional ByVal clientItemId As String = Nothing, Optional ByVal operationId As String = Nothing,
        Optional ByVal taskId As Nullable(Of Long) = Nothing, Optional ByVal radicado As String = Nothing,
        Optional ByVal referenciaProveedor As String = Nothing) As Task(Of MetadatosDocumentoSii)
        Return _client.ResolveStorageMetadataAsync(externalKey, correlationId, cancellationToken, intentId, clientItemId, operationId, taskId, radicado, referenciaProveedor)
    End Function

    Private Shared Sub ValidateProvider(ByVal request As SolicitudImportacionServicioDto)
        If request Is Nothing OrElse Not String.Equals(request.ProviderId, CanonicalProviderId, StringComparison.OrdinalIgnoreCase) Then
            Throw New ArgumentException("Proveedor SII inválido.", "request")
        End If
    End Sub
End Class

Imports System
Imports System.Collections.Generic
Imports System.Threading

Public NotInheritable Class SiiImportInscriptionResolver
    Implements IImportInscriptionResolver

    Private ReadOnly _provider As SiiImportProvider
    Private ReadOnly _configuration As IImportExpedientConfigurationRepository

    Public Sub New(ByVal provider As SiiImportProvider,
                   ByVal configuration As IImportExpedientConfigurationRepository)
        If provider Is Nothing Then Throw New ArgumentNullException("provider")
        If configuration Is Nothing Then Throw New ArgumentNullException("configuration")
        _provider = provider
        _configuration = configuration
    End Sub

    Public Function Resolver(ByVal context As ContextoImportacionServicio,
                             ByVal request As CreateImportIntentRequestDto,
                             ByVal items As IList(Of ResultadoElementoImportacion)) As IList(Of InscripcionImportacion) Implements IImportInscriptionResolver.Resolver
        If context Is Nothing OrElse request Is Nothing OrElse items Is Nothing OrElse items.Count = 0 Then Throw New InvalidOperationException("SII_INSCRIPTION_CONTEXT_INVALID")
        Dim barcode As String = Nothing
        For Each item In items
            Dim itemBarcode As String = Nothing, book As String = Nothing, registration As String = Nothing, attachment As String = Nothing
            If item Is Nothing OrElse item.IdentidadExterna Is Nothing OrElse Not SiiImportContractMapper.TryParseExternalKey(item.IdentidadExterna.ExternalKey, itemBarcode, book, registration, attachment) Then Throw New InvalidOperationException("SII_INSCRIPTION_ITEM_INVALID")
            If barcode Is Nothing Then barcode = itemBarcode
            If Not String.Equals(barcode, itemBarcode, StringComparison.Ordinal) Then Throw New InvalidOperationException("SII_INSCRIPTION_BARCODE_CONFLICT")
        Next
        Dim configuration As ConfiguracionExpedienteImportacion = Nothing
        Try
            configuration = _configuration.Obtener(context)
        Catch ex As InvalidOperationException
            Throw
        Catch
            Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_QUERY_FAILED")
        End Try
        If configuration Is Nothing OrElse String.IsNullOrWhiteSpace(configuration.NombreGabinete) Then Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_UNAVAILABLE")
        Dim query As New QueryItemsRequestDto With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId, .TaskId = context.IdTarea, .ProviderId = context.ProviderId, .CodigoBarras = barcode}
        Dim inscriptions As IList(Of InscripcionImportacion) = Nothing
        Try
            inscriptions = _provider.ResolveInscriptionsAsync(query, items, configuration.NombreGabinete.Trim(), CancellationToken.None).GetAwaiter().GetResult()
        Catch ex As InvalidOperationException
            Throw
        Catch
            Throw New InvalidOperationException("SII_INSCRIPTION_QUERY_FAILED")
        End Try
        If inscriptions Is Nothing OrElse inscriptions.Count = 0 Then Throw New InvalidOperationException("SII_INSCRIPTIONS_UNAVAILABLE")
        For Each item In items
            If String.IsNullOrWhiteSpace(item.ClaveInscripcion) Then Throw New InvalidOperationException("SII_INSCRIPTION_ITEM_UNRESOLVED")
        Next
        Return inscriptions
    End Function
End Class

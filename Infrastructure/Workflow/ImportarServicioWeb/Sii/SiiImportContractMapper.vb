Imports System
Imports System.Text
Imports Newtonsoft.Json.Linq

Public NotInheritable Class SiiImportContractMapper
    Public Function MapQuery(ByVal payload As Byte(), ByVal request As QueryItemsRequestDto) As QueryItemsResponseDto
        If payload Is Nothing Then Throw New ArgumentNullException("payload")
        Dim source = JObject.Parse(Encoding.UTF8.GetString(payload))
        Dim response As New QueryItemsResponseDto With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId}
        For Each item As JObject In source("items")
            Dim externalKey = BuildExternalKey(Value(item, "book"), Value(item, "registration"), Value(item, "matricula"))
            response.Items.Add(New ExternalItemDto With {
                .ExternalKey = externalKey,
                .DisplayName = (Value(item, "act") & " - " & Value(item, "news")).Trim(" "c, "-"c),
                .ContentType = Value(item, "contentType"),
                .Length = NullableLong(item("length")),
                .PreviewAvailable = True
            })
        Next
        response.ContinuationToken = Value(source, "continuationToken")
        Return response
    End Function

    Public Function MapPreview(ByVal payload As Byte(), ByVal request As GetPreviewRequestDto) As GetPreviewResponseDto
        Dim source = JObject.Parse(Encoding.UTF8.GetString(payload))
        Return New GetPreviewResponseDto With {
            .OperationId = request.OperationId, .CorrelationId = request.CorrelationId,
            .ExternalKey = request.ExternalKey, .DescriptorId = Value(source, "descriptorId"),
            .ContentType = Value(source, "contentType"), .Length = NullableLong(source("length")),
            .Disposition = Value(source, "disposition"), .ExpiresAtUtc = NullableDate(source("expiresAtUtc"))
        }
    End Function

    Public Function CreateDocumentCommand(ByVal item As ImportItemSelectionDto) As DocumentCommandDto
        If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.ExternalKey) Then Throw New ArgumentException("Item SII inválido.", "item")
        Return New DocumentCommandDto With {.ClientItemId = item.ClientItemId, .ExternalKey = item.ExternalKey,
            .DocumentTypeId = item.DocumentTypeId, .FileName = item.FileName, .ContentType = item.ContentType}
    End Function

    Public Shared Function BuildExternalKey(ByVal book As String, ByVal registration As String, ByVal matricula As String) As String
        Return "SII:" & Segment(book) & ":" & Segment(registration) & ":" & Segment(matricula)
    End Function

    Private Shared Function Segment(ByVal value As String) As String
        Dim normalized = If(value, String.Empty).Trim().Replace(":", "-")
        If normalized.Length = 0 Then Throw New ArgumentException("La identidad SII está incompleta.")
        Return normalized
    End Function

    Private Shared Function Value(ByVal source As JObject, ByVal name As String) As String
        Return If(source(name) Is Nothing, String.Empty, source(name).ToString().Trim())
    End Function

    Private Shared Function NullableLong(ByVal token As JToken) As Nullable(Of Long)
        If token Is Nothing Then Return Nothing
        Dim value As Long
        If Long.TryParse(token.ToString(), value) Then Return value
        Return Nothing
    End Function

    Private Shared Function NullableDate(ByVal token As JToken) As Nullable(Of DateTime)
        If token Is Nothing Then Return Nothing
        Dim value As DateTime
        If DateTime.TryParse(token.ToString(), Globalization.CultureInfo.InvariantCulture,
                             Globalization.DateTimeStyles.AdjustToUniversal, value) Then Return value
        Return Nothing
    End Function
End Class

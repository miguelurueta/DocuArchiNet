Imports System
Imports System.Text
Imports Newtonsoft.Json.Linq

Public NotInheritable Class SiiImportContractMapper
    Public Function MapQuery(ByVal payload As Byte(), ByVal request As QueryItemsRequestDto) As QueryItemsResponseDto
        If payload Is Nothing OrElse request Is Nothing Then Throw New ArgumentNullException("payload")
        Dim source = NormalizeSource(Encoding.UTF8.GetString(payload))
        Dim resultCode = Value(source, "codigoerror")
        If resultCode = "9998" Then Throw New InvalidOperationException("SII_TOKEN_INVALID")
        If resultCode = "9999" Then Throw New InvalidOperationException("SII_QUERY_PROVIDER_ERROR")
        If Value(source, "mensajeerror").Length > 0 OrElse (resultCode.Length > 0 AndAlso resultCode <> "0000") Then Throw New InvalidOperationException("SII_QUERY_REJECTED")
        Dim response As New QueryItemsResponseDto With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId}
        Dim inscriptions = TryCast(Token(source, "inscripciones"), JArray)
        If inscriptions Is Nothing Then response.ProviderResultCode = "SII_INSCRIPTIONS_MISSING" : Return response
        response.InscriptionCount = inscriptions.Count
        If inscriptions.Count = 0 Then response.ProviderResultCode = "SII_INSCRIPTIONS_EMPTY" : Return response
        For Each inscription As JObject In inscriptions
            Dim images = TryCast(Token(inscription, "imagenes"), JArray)
            If images Is Nothing Then Continue For
            For Each image As JObject In images
                response.ImageCount += 1
                Dim format = Value(image, "formato").ToLowerInvariant()
                response.Items.Add(New ExternalItemDto With {
                    .ExternalKey = BuildExternalKey(request.CodigoBarras, Value(inscription, "libro"), Value(inscription, "registro"), Value(image, "idanexo")),
                    .DisplayName = DisplayName(inscription, image),
                    .ContentType = ContentTypeForFormat(format),
                    .PreviewAvailable = True})
            Next
        Next
        response.ProviderResultCode = If(response.ImageCount = 0, "SII_IMAGES_EMPTY", "SII_ITEMS_READY")
        Return response
    End Function

    Public Function MapPreview(ByVal payload As Byte(), ByVal request As GetPreviewRequestDto) As GetPreviewResponseDto
        Dim source = JObject.Parse(Encoding.UTF8.GetString(payload))
        Return New GetPreviewResponseDto With {.OperationId = request.OperationId, .CorrelationId = request.CorrelationId,
            .ExternalKey = request.ExternalKey, .DescriptorId = Value(source, "descriptorId"), .ContentType = Value(source, "contentType"),
            .Length = NullableLong(Token(source, "length")), .Disposition = Value(source, "disposition"),
            .ExpiresAtUtc = NullableDateTime(Token(source, "expiresAtUtc"))}
    End Function

    Public Function CreateDocumentCommand(ByVal item As ImportItemSelectionDto) As DocumentCommandDto
        If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.ExternalKey) Then Throw New ArgumentException("Item SII inválido.", "item")
        Return New DocumentCommandDto With {.ClientItemId = item.ClientItemId, .ExternalKey = item.ExternalKey,
            .DocumentTypeId = item.DocumentTypeId, .DocumentTypeName = item.DocumentTypeName,
            .FileName = item.FileName, .ContentType = item.ContentType}
    End Function

    Public Shared Function BuildExternalKey(ByVal codigoBarras As String, ByVal book As String, ByVal registration As String, ByVal attachmentId As String) As String
        Return "SII2." & Encode(codigoBarras) & "." & Encode(book) & "." & Encode(registration) & "." & Encode(attachmentId)
    End Function

    Public Shared Function ContentTypeForFormat(ByVal format As String) As String
        Select Case If(format, String.Empty).Trim().ToLowerInvariant()
            Case "pdf" : Return "application/pdf"
            Case "png" : Return "image/png"
            Case "jpg", "jpeg" : Return "image/jpeg"
            Case "tif", "tiff" : Return "image/tiff"
            Case Else : Throw New InvalidOperationException("SII_RESOURCE_FORMAT_UNSUPPORTED")
        End Select
    End Function

    Public Shared Function TryParseExternalKey(ByVal key As String, ByRef radicado As String, ByRef book As String,
                                               ByRef registration As String, ByRef attachmentId As String) As Boolean
        Try
            Dim parts = If(key, String.Empty).Split("."c)
            If parts.Length <> 5 OrElse parts(0) <> "SII2" Then Return False
            radicado = Decode(parts(1)) : book = Decode(parts(2)) : registration = Decode(parts(3)) : attachmentId = Decode(parts(4))
            Return radicado.Length > 0 AndAlso book.Length > 0 AndAlso registration.Length > 0 AndAlso attachmentId.Length > 0
        Catch
            Return False
        End Try
    End Function

    Private Shared Function Encode(ByVal value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Throw New ArgumentException("La identidad SII está incompleta.")
        Return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Trim())).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
    End Function
    Private Shared Function Decode(ByVal value As String) As String
        Dim normalized = value.Replace("-", "+").Replace("_", "/")
        normalized &= New String("="c, (4 - normalized.Length Mod 4) Mod 4)
        Return Encoding.UTF8.GetString(Convert.FromBase64String(normalized)).Trim()
    End Function
    Private Shared Function DisplayName(ByVal inscription As JObject, ByVal image As JObject) As String
        Dim result = (Value(inscription, "nacto") & " - " & Value(image, "nombre")).Trim(" "c, "-"c)
        Return If(result.Length > 160, result.Substring(0, 160), result)
    End Function
    Friend Shared Function Value(ByVal source As JObject, ByVal name As String) As String
        Dim tokenValue = Token(source, name)
        Return If(tokenValue Is Nothing, String.Empty, tokenValue.ToString().Trim())
    End Function
    Friend Shared Function Token(ByVal source As JObject, ByVal name As String) As JToken
        Return If(source Is Nothing, Nothing, source.GetValue(name, StringComparison.OrdinalIgnoreCase))
    End Function
    Friend Shared Function NormalizeSource(ByVal json As String) As JObject
        Dim source = JObject.Parse(json)
        Dim wrapped = Token(source, "d")
        If wrapped Is Nothing Then Return source
        If wrapped.Type = JTokenType.Object Then Return DirectCast(wrapped, JObject)
        If wrapped.Type = JTokenType.String AndAlso wrapped.ToString().TrimStart().StartsWith("{") Then Return JObject.Parse(wrapped.ToString())
        Return source
    End Function
    Private Shared Function NullableLong(ByVal token As JToken) As Nullable(Of Long)
        Dim result As Long
        If token IsNot Nothing AndAlso Long.TryParse(token.ToString(), result) Then Return result
        Return Nothing
    End Function
    Private Shared Function NullableDateTime(ByVal token As JToken) As Nullable(Of DateTime)
        If token Is Nothing OrElse token.Type = JTokenType.Null Then Return Nothing

        If token.Type = JTokenType.Date Then
            Try
                Return token.ToObject(Of DateTimeOffset)().UtcDateTime
            Catch ex As InvalidCastException
                Return Nothing
            Catch ex As FormatException
                Return Nothing
            End Try
        End If

        Dim rawValue As String = If(token.Type = JTokenType.String,
            token.Value(Of String)(), token.ToString(Newtonsoft.Json.Formatting.None))
        Dim result As DateTimeOffset
        If DateTimeOffset.TryParse(rawValue, Globalization.CultureInfo.InvariantCulture,
            Globalization.DateTimeStyles.RoundtripKind, result) Then Return result.UtcDateTime
        Return Nothing
    End Function
End Class

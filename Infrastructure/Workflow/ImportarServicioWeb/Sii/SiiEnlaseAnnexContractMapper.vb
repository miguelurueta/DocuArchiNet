Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json.Linq

' Mapea exclusivamente imagenes de consultarRadicado; no conoce sesión, SQL ni almacenamiento.
Public NotInheritable Class SiiEnlaseAnnexContractMapper
    Private Sub New()
    End Sub

    Public Shared Function MapQuery(ByVal source As JObject, ByVal request As QueryItemsRequestDto) As QueryItemsResponseDto
        Dim response As New QueryItemsResponseDto With {.OperationId = request.OperationId,
            .CorrelationId = request.CorrelationId, .ProviderResultCode = SiiImportContractMapper.Value(source, "codigoerror")}
        Dim images = TryCast(SiiImportContractMapper.Token(source, "imagenes"), JArray)
        If images Is Nothing Then Return response
        Dim identities As New HashSet(Of String)(StringComparer.Ordinal)
        For Each token In images
            Dim image = TryCast(token, JObject)
            If image Is Nothing Then Throw New InvalidOperationException("SII_ANNEX_CONTRACT_INVALID")
            Dim annex = Parse(image)
            If annex.IdAnexo.Length = 0 OrElse Not identities.Add(annex.IdAnexo) Then Throw New InvalidOperationException("SII_ANNEX_ID_INVALID")
            response.Items.Add(ToItem(annex))
        Next
        response.ImageCount = response.Items.Count
        Return response
    End Function

    Public Shared Function Resolve(ByVal source As JObject, ByVal externalKey As String) As SiiEnlaseAnnex
        Dim images = TryCast(SiiImportContractMapper.Token(source, "imagenes"), JArray)
        If images Is Nothing Then Throw New InvalidOperationException("SII_ANNEX_NOT_FOUND")
        Dim match As SiiEnlaseAnnex = Nothing
        Dim identities As New HashSet(Of String)(StringComparer.Ordinal)
        For Each token In images
            Dim image = TryCast(token, JObject)
            If image Is Nothing Then Throw New InvalidOperationException("SII_ANNEX_CONTRACT_INVALID")
            Dim candidate = Parse(image)
            If candidate.IdAnexo.Length = 0 OrElse Not identities.Add(candidate.IdAnexo) Then Throw New InvalidOperationException("SII_ANNEX_ID_INVALID")
            If String.Equals(candidate.IdAnexo, externalKey, StringComparison.Ordinal) Then match = candidate
        Next
        If match Is Nothing Then Throw New InvalidOperationException("SII_ANNEX_NOT_FOUND")
        Return match
    End Function

    Private Shared Function Parse(ByVal image As JObject) As SiiEnlaseAnnex
        Dim format = Value(image, "formato").Trim().TrimStart("."c).ToLowerInvariant()
        Return New SiiEnlaseAnnex With {.IdAnexo = Value(image, "idanexo").Trim(), .Url = Value(image, "url").Trim(),
            .Formato = format, .ContentType = SiiImportContractMapper.ContentTypeForFormat(format),
            .Tipo = Value(image, "tipo"), .TipoAnexo = Value(image, "tipoanexo"), .TipoSirep = Value(image, "tiposirep"),
            .TipoDigitalizacion = Value(image, "tipodigitalizacion"), .Identificador = Value(image, "identificador"),
            .Identificacion = Value(image, "identificacion"), .Nombre = Value(image, "nombre"),
            .Matricula = Value(image, "matricula"), .Proponente = Value(image, "proponente"),
            .FechaDocumento = Value(image, "fechadocumento"), .Origen = Value(image, "origen"),
            .Observaciones = Value(image, "observaciones")}
    End Function

    Private Shared Function ToItem(ByVal annex As SiiEnlaseAnnex) As ExternalItemDto
        Dim display = If(annex.Observaciones.Trim().Length > 0, annex.Observaciones.Trim(), "Anexo " & annex.IdAnexo)
        Dim item As New ExternalItemDto With {.ExternalKey = annex.IdAnexo, .DisplayName = display,
            .ContentType = annex.ContentType, .PreviewAvailable = True, .PresentationSchemaVersion = "1.1"}
        item.AllowedActions.Add("PREVIEW")
        AddMetadata(item, "tipo", "Tipo", annex.Tipo)
        AddMetadata(item, "tipoAnexo", "Tipo de anexo", annex.TipoAnexo)
        AddMetadata(item, "fechaDocumento", "Fecha del documento", annex.FechaDocumento)
        AddMetadata(item, "origen", "Origen", annex.Origen)
        Return item
    End Function

    Private Shared Sub AddMetadata(ByVal item As ExternalItemDto, ByVal code As String, ByVal label As String, ByVal value As String)
        If Not String.IsNullOrWhiteSpace(value) Then item.Metadata.Add(New ImportItemMetadataDto With {.Code = code, .Label = label, .Value = value.Trim()})
    End Sub

    Private Shared Function Value(ByVal source As JObject, ByVal name As String) As String
        Return SiiImportContractMapper.Value(source, name)
    End Function
End Class

Public NotInheritable Class SiiEnlaseAnnex
    Public Property IdAnexo As String
    Public Property Url As String
    Public Property Formato As String
    Public Property ContentType As String
    Public Property Tipo As String
    Public Property TipoAnexo As String
    Public Property TipoSirep As String
    Public Property TipoDigitalizacion As String
    Public Property Identificador As String
    Public Property Identificacion As String
    Public Property Nombre As String
    Public Property Matricula As String
    Public Property Proponente As String
    Public Property FechaDocumento As String
    Public Property Origen As String
    Public Property Observaciones As String
    Public ReadOnly Property FileName As String
        Get
            Return "anexo-" & IdAnexo & If(Formato.Length > 0, "." & Formato, String.Empty)
        End Get
    End Property
End Class

Imports System
Imports System.Collections.Generic

Public NotInheritable Class ImportItemPresentationService
    Private ReadOnly _catalog As IImportDocumentTypeCatalogRepository
    Private ReadOnly _status As IImportItemStatusRepository
    Public Sub New(ByVal catalog As IImportDocumentTypeCatalogRepository, ByVal status As IImportItemStatusRepository)
        If catalog Is Nothing OrElse status Is Nothing Then Throw New ArgumentNullException("dependency")
        _catalog=catalog : _status=status
    End Sub

    Public Sub EnrichCapabilities(ByVal context As ContextoImportacionServicio, ByVal response As ResolveCapabilitiesResponseDto)
        If response Is Nothing OrElse response.Error IsNot Nothing Then Return
        response.DocumentTypes.Clear()
        For Each item In _catalog.Obtener(context)
            response.DocumentTypes.Add(New ImportDocumentTypeDto With {.DocumentTypeId=item.IdTipoDocumentalTrd,.Name=item.Nombre,
                .Required=item.Obligatorio,.SortOrder=item.Orden})
        Next
    End Sub

    Public Sub EnrichItems(ByVal context As ContextoImportacionServicio, ByVal providerId As String, ByVal response As QueryItemsResponseDto)
        If response Is Nothing OrElse response.Error IsNot Nothing Then Return
        Dim keys As New List(Of String)()
        For Each item In response.Items : keys.Add(item.ExternalKey) : Next
        Dim states=_status.ObtenerLote(context,providerId,keys)
        For Each item In response.Items
            Dim state As EstadoItemListadoImportacion=Nothing
            If Not states.TryGetValue(item.ExternalKey,state) Then state=New EstadoItemListadoImportacion()
            item.PresentationSchemaVersion="1.1" : item.AllowedActions.Clear()
            If state.Confirmado Then
                item.ImportStatus="Importado" : item.AllowedActions.Add("View")
            ElseIf state.TieneNovedad Then
                item.ImportStatus="ConNovedad" : item.AllowedActions.Add("Review")
            Else
                item.ImportStatus="Disponible" : item.AllowedActions.Add("Preview") : item.AllowedActions.Add("Import")
            End If
        Next
    End Sub

    Public Sub ApplyPagination(ByVal request As QueryItemsRequestDto, ByVal response As QueryItemsResponseDto)
        If request Is Nothing OrElse response Is Nothing OrElse response.Error IsNot Nothing OrElse Not request.PageSize.HasValue Then Return
        Dim size=Math.Min(Math.Max(request.PageSize.Value,1),100), offset As Integer=0
        If Not String.IsNullOrWhiteSpace(request.ContinuationToken) AndAlso
           (Not request.ContinuationToken.StartsWith("offset:",StringComparison.Ordinal) OrElse Not Integer.TryParse(request.ContinuationToken.Substring(7),offset) OrElse offset<0) Then
            Throw New InvalidOperationException("CONTINUATION_TOKEN_INVALID")
        End If
        Dim source=New List(Of ExternalItemDto)(response.Items), page As New List(Of ExternalItemDto)()
        For index=offset To Math.Min(source.Count,offset+size)-1 : page.Add(source(index)) : Next
        response.Items=page
        response.ContinuationToken=If(offset+page.Count<source.Count,"offset:" & (offset+page.Count).ToString(Globalization.CultureInfo.InvariantCulture),Nothing)
    End Sub
End Class
